FROM mcr.microsoft.com/dotnet/skd:9.0 AS build
WORKDIR /repo

COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY Dealmatcher.Backend.sln ./

COPY src/Dealmatcher.Backend.Domain/Dealmatcher.Backend.Domain.csproj ./src/Dealmatcher.Backend.Domain/
COPY src/Dealmatcher.Backend.UseCases/Dealmatcher.Backend.UseCases.csproj ./src/Dealmatcher.Backend.UseCases/
COPY src/Dealmatcher.Backend.Infrastructure/Dealmatcher.Backend.Infrastructure.csproj ./src/Dealmatcher.Backend.Infrastructure/
COPY src/Dealmatcher.Backend.API/Dealmatcher.Backend.API.csproj ./src/Dealmatcher.Backend.API/

COPY tests/Dealmatcher.Backend.FunctionalTests/Dealmatcher.Backend.FunctionalTests.csproj ./tests/Dealmatcher.Backend.FunctionalTests/
COPY tests/Dealmatcher.Backend.IntegrationTests/Dealmatcher.Backend.IntegrationTests.csproj ./tests/Dealmatcher.Backend.IntegrationTests/
COPY tests/Dealmatcher.Backend.UnitTests/Dealmatcher.Backend.UnitTests.csproj ./tests/Dealmatcher.Backend.UnitTests/

RUN dotnet restore

COPY src/ ./src/
RUN dotnet publish src/Dealmatcher.Backend.API/Dealmatcher.Backend.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Dealmatcher.Backend.API.dll"]