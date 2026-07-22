FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SistemGradum.API/SistemGradum.API.csproj", "SistemGradum.API/"]
COPY ["SistemGradum.Application/SistemGradum.Application.csproj", "SistemGradum.Application/"]
COPY ["SistemGradum.Domain/SistemGradum.Domain.csproj", "SistemGradum.Domain/"]
COPY ["SistemGradum.Infrastructure/SistemGradum.Infrastructure.csproj", "SistemGradum.Infrastructure/"]
RUN dotnet restore "./SistemGradum.API/SistemGradum.API.csproj"
COPY . .
WORKDIR "/src/SistemGradum.API"
RUN dotnet build "./SistemGradum.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SistemGradum.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SistemGradum.API.dll"]
