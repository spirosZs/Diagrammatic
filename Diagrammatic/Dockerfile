FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Diagrammatic_test.csproj", "./"]
RUN dotnet restore "Diagrammatic_test.csproj"
COPY . .
RUN dotnet publish "Diagrammatic_test.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Diagrammatic_test.dll"]
