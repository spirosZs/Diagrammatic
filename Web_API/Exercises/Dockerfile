FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Exercises/Exercises.csproj", "Exercises/"]
COPY ["Exercises.Common/Exercises.Common.csproj", "Exercises.Common/"]
COPY ["Exercises.Data/Exercises.Data.csproj", "Exercises.Data/"]
COPY ["Exercises.Core/Exercises.Core.csproj", "Exercises.Core/"]
RUN dotnet restore "Exercises/Exercises.csproj"
COPY . .
WORKDIR "/src/Exercises"
RUN dotnet build "Exercises.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Exercises.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Exercises.dll"]
