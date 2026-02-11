# Stage 1: Build React Frontend
FROM node:22-alpine AS build-web
WORKDIR /source

# Enable pnpm
RUN corepack enable
COPY AffirmationGenerator.Client/package.json AffirmationGenerator.Client/pnpm-lock.yaml ./
RUN pnpm install --frozen-lockfile
COPY AffirmationGenerator.Client/ ./
RUN pnpm run build

# Stage 2: Build .NET Backend
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-dotnet
ENV HUSKY=0
WORKDIR /src
# Copy project files first to cache restore
COPY AffirmationGenerator.Server/AffirmationGenerator.Server.csproj AffirmationGenerator.Server/
COPY AffirmationGenerator.Client/AffirmationGenerator.Client.esproj AffirmationGenerator.Client/
RUN dotnet restore AffirmationGenerator.Server/AffirmationGenerator.Server.csproj /p:SkipClientBuild=true

# Copy the rest of the source code
COPY . .
WORKDIR /src/AffirmationGenerator.Server
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false /p:SkipClientBuild=true

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

# Copy the published .NET app
COPY --from=build-dotnet /app/publish .
# Copy the built React app to wwwroot so .NET can serve it
COPY --from=build-web /source/dist ./wwwroot

ENTRYPOINT ["dotnet", "AffirmationGenerator.Server.dll"]