FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /repo

COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY Dealmatcher.Backend.sln ./

COPY src/Dealmatcher.Backend.Domain/Dealmatcher.Backend.Domain.csproj ./src/Dealmatcher.Backend.Domain/
COPY src/Dealmatcher.Backend.UseCases/Dealmatcher.Backend.UseCases.csproj ./src/Dealmatcher.Backend.UseCases/
COPY src/Dealmatcher.Backend.Infrastructure/Dealmatcher.Backend.Infrastructure.csproj ./src/Dealmatcher.Backend.Infrastructure/
COPY src/Dealmatcher.Backend.API/Dealmatcher.Backend.API.csproj ./src/Dealmatcher.Backend.API/

RUN dotnet restore src/Dealmatcher.Backend.API/Dealmatcher.Backend.API.csproj

COPY src/ ./src/
RUN dotnet publish src/Dealmatcher.Backend.API/Dealmatcher.Backend.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "Dealmatcher.Backend.API.dll"]
