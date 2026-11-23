<#
.SYNOPSIS
    Starts the Diagrammatic stack for a classroom and prints the URL students open.

.DESCRIPTION
    Run this on the teacher's laptop. Students join the same Wi-Fi and open the URL
    this script prints on their phones.

    It does three things that a plain "docker compose up" does not:

      1. Re-detects the laptop's LAN IP. The IP changes with every network, and it is
         baked into APP_PUBLIC_API_URL -- the address the *student's browser* uses to
         load the diagram editor iframe. If that is stale (or says localhost) the app
         loads but every diagram is blank, which is the failure this script exists to
         prevent.
      2. Makes sure Windows Firewall lets the class in on the app + API ports. The
         rules are scoped to LocalSubnet, so only devices on the same Wi-Fi can connect.
      3. Brings the stack up and re-creates the app container if the IP changed.

.EXAMPLE
    .\start-classroom.ps1
#>

[CmdletBinding()]
param(
    # Skip the firewall check (useful if the rules are managed centrally).
    [switch] $SkipFirewall
)

$ErrorActionPreference = 'Stop'
Set-Location -Path $PSScriptRoot

$APP_PORT = 8080
$API_PORT = 8083

function Write-Step { param([string] $Text) Write-Host "`n==> $Text" -ForegroundColor Cyan }
function Write-Warn { param([string] $Text) Write-Host "    ! $Text" -ForegroundColor Yellow }

# --- 1. Find the LAN IP on the interface that actually reaches the network ----------
# Picking by lowest-metric default route avoids the VMware/Hyper-V virtual adapters,
# which otherwise win a naive "first IPv4 address" search and hand out an IP no phone
# can reach.
Write-Step 'Detecting this laptop''s address on the Wi-Fi'

$route = Get-NetRoute -DestinationPrefix '0.0.0.0/0' -ErrorAction SilentlyContinue |
         Sort-Object -Property RouteMetric |
         Select-Object -First 1

if ($null -eq $route) {
    throw 'No default route found. Connect the laptop to the classroom Wi-Fi and re-run.'
}

$lanIp = (Get-NetIPAddress -InterfaceIndex $route.ifIndex -AddressFamily IPv4 -ErrorAction Stop |
          Where-Object { $_.IPAddress -ne '127.0.0.1' } |
          Select-Object -First 1).IPAddress

if ([string]::IsNullOrWhiteSpace($lanIp)) {
    throw 'Could not read an IPv4 address for the active network interface.'
}

$adapter = $route.InterfaceAlias
Write-Host "    $lanIp  (on $adapter)"

$netProfile = Get-NetConnectionProfile -InterfaceIndex $route.ifIndex -ErrorAction SilentlyContinue
if ($netProfile -and $netProfile.NetworkCategory -eq 'Public') {
    Write-Warn 'This network is classified Public. The firewall rules below are scoped to'
    Write-Warn 'LocalSubnet so it still works, but if phones cannot connect, that is the'
    Write-Warn 'first thing to check.'
}

# --- 2. Point the iframe URL at that IP ---------------------------------------------
Write-Step 'Updating .env'

if (-not (Test-Path '.env')) {
    Copy-Item '.env.example' '.env'
    Write-Host '    .env did not exist; created it from .env.example.'
    Write-Warn 'Replace JWT_SECRET and SERVICE_API_KEY before using this with real students.'
}

$publicUrl = "http://${lanIp}:${API_PORT}"
$envLines  = Get-Content '.env'
$previous  = ($envLines | Where-Object { $_ -match '^APP_PUBLIC_API_URL=' }) -replace '^APP_PUBLIC_API_URL=', ''

if ($envLines -match '^APP_PUBLIC_API_URL=') {
    $envLines = $envLines -replace '^APP_PUBLIC_API_URL=.*', "APP_PUBLIC_API_URL=$publicUrl"
} else {
    $envLines += "APP_PUBLIC_API_URL=$publicUrl"
}
Set-Content -Path '.env' -Value $envLines -Encoding utf8

$ipChanged = ($previous -ne $publicUrl)
if ($ipChanged) {
    Write-Host "    APP_PUBLIC_API_URL: $previous -> $publicUrl"
} else {
    Write-Host "    APP_PUBLIC_API_URL already correct ($publicUrl)"
}

# --- 3. Firewall -------------------------------------------------------------------
if (-not $SkipFirewall) {
    Write-Step 'Checking Windows Firewall'

    $wanted = @(
        @{ Name = 'Diagrammatic app (classroom)';        Port = $APP_PORT },
        @{ Name = 'Diagrammatic diagram API (classroom)'; Port = $API_PORT }
    )
    $missing = @($wanted | Where-Object {
        $null -eq (Get-NetFirewallRule -DisplayName $_.Name -ErrorAction SilentlyContinue)
    })

    if ($missing.Count -eq 0) {
        Write-Host '    Rules already in place.'
    } else {
        $isAdmin = ([Security.Principal.WindowsPrincipal] `
                    [Security.Principal.WindowsIdentity]::GetCurrent()
                   ).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

        # Build one command that creates every missing rule, so the teacher sees at most
        # one UAC prompt -- and only the first time, since the rules persist.
        $cmds = $missing | ForEach-Object {
            "New-NetFirewallRule -DisplayName '$($_.Name)' -Direction Inbound " +
            "-Action Allow -Protocol TCP -LocalPort $($_.Port) " +
            "-Profile Any -RemoteAddress LocalSubnet | Out-Null"
        }
        $script = $cmds -join '; '

        if ($isAdmin) {
            Invoke-Expression $script
            Write-Host "    Created $($missing.Count) rule(s)."
        } else {
            Write-Host '    Needs administrator rights once. Approve the UAC prompt.'
            $p = Start-Process -FilePath 'powershell.exe' `
                               -ArgumentList '-NoProfile','-NonInteractive','-Command',$script `
                               -Verb RunAs -WindowStyle Hidden -PassThru -Wait
            if ($p.ExitCode -ne 0) {
                Write-Warn 'Firewall rules were not created. Phones will not be able to connect.'
                Write-Warn 'Re-run this script in an Administrator PowerShell.'
            } else {
                Write-Host "    Created $($missing.Count) rule(s)."
            }
        }
    }
}

# --- 4. Start the stack -------------------------------------------------------------
Write-Step 'Starting the stack'

docker compose up -d
if ($LASTEXITCODE -ne 0) { throw 'docker compose up failed. Is Docker Desktop running?' }

# compose only re-reads APP_PUBLIC_API_URL when the container is re-created, so a moved
# laptop needs an explicit nudge or the class gets blank diagrams.
if ($ipChanged) {
    Write-Host '    IP changed; re-creating the app container so it picks up the new URL.'
    docker compose up -d --force-recreate app
    if ($LASTEXITCODE -ne 0) { throw 'Could not re-create the app container.' }
}

# --- 5. Tell the teacher what to write on the board ----------------------------------
$studentUrl = "http://${lanIp}:${APP_PORT}"

Write-Host ''
Write-Host '  ---------------------------------------------------'  -ForegroundColor Green
Write-Host '   Students open this on their phones:'                  -ForegroundColor Green
Write-Host ''
Write-Host "       $studentUrl"                                      -ForegroundColor White
Write-Host ''
Write-Host '   (same Wi-Fi as this laptop; keep the laptop awake)'    -ForegroundColor Green
Write-Host '  ---------------------------------------------------'  -ForegroundColor Green
Write-Host ''
Write-Host 'Stop the stack with:  docker compose down'               -ForegroundColor DarkGray
