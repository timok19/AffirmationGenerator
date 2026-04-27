# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

AffirmationGenerator is a fullstack app that serves daily positive affirmations with multi-language support (via DeepL) and per-IP rate limiting (via Redis). It is deployed as a single Docker container to Fly.io.

## Commands

### Frontend (`AffirmationGenerator.Client/`)

```bash
pnpm run dev       # Start Vite dev server (port 5173)
pnpm run build     # TypeScript check + Vite build
pnpm run lint      # ESLint (max-warnings=0 — must be clean)
pnpm run format    # Prettier formatting
```

### Backend (`AffirmationGenerator.Server/`)

```bash
dotnet run                               # Start API (https://localhost:7006) + SPA proxy
dotnet build AffirmationGenerator.slnx
dotnet test AffirmationGenerator.slnx   # Run all unit tests
dotnet test --filter "FullyQualifiedName~ClassName"  # Run a single test class
```

### Docker

```bash
docker build -t affirmation-generator .
docker run -p 8080:8080 affirmation-generator
```

## Architecture

### Stack
- **Frontend**: React 19 + Vite + TypeScript + Tailwind CSS v4 + DaisyUI + TanStack React Query + Axios
- **Backend**: ASP.NET Core 10.0 with Clean Architecture
- **External services**: affirmations.dev (source), DeepL API (translation), Redis (rate limiting)

### How the pieces connect

In development, `dotnet run` starts the .NET server which launches the Vite dev server as a SPA proxy. API calls from Vite are proxied to `https://localhost:7006`. In production, the multi-stage Docker build compiles the React app and embeds it as static files in `wwwroot`; the .NET server serves both the SPA and the API.

### Backend layer structure (Clean Architecture)

| Layer | Responsibility |
|---|---|
| `Api/` | Controllers, rate limiting policy, HTTP models |
| `Application/` | CQRS-style query handlers, service orchestration |
| `Infrastructure/` | External HTTP clients (Refit), Redis client |
| `Domain/` | `AffirmationLanguage` enum, domain errors |
| `Core/` | `Result<T>` error-handling pattern, shared extensions |

Each layer has its own `DiConfig.cs` for DI registration; all are wired in `Program.cs`.

### API endpoints

| Method | Path | Description |
|---|---|---|
| GET | `/affirmations?targetLanguage={lang}` | Fetch affirmation (returns `{text, remainingCount}`) |
| GET | `/affirmations/remaining` | Remaining quota and `resetInSeconds` |
| GET | `/affirmations/languages` | Supported language codes |
| GET | `/health` | Health check |
| GET | `/swagger` | Swagger UI (dev only) |

### Rate limiting

Fixed-window limiter keyed on client IP (configurable via `Application:ClientOptions:ClientIpHeaderName` for proxy scenarios). Default: 10 requests/day per IP. Counter stored in Redis; falls back to in-memory if Redis is unavailable.

### Error handling

The backend uses a `Result<T>` type (`Core/Result.cs`) — all query handlers return `Result<T>` instead of throwing. Controllers map domain errors to HTTP responses.

## Local Development Setup

1. Set the DeepL API key via user secrets:
   ```bash
   dotnet user-secrets set "Infrastructure:DeepLTranslatorClientOptions:ApiKey" "YOUR_KEY" \
     --project AffirmationGenerator.Server
   ```
2. Redis is optional locally — the app falls back gracefully.
3. Swagger UI is available at `https://localhost:7006/swagger` in dev.

## Configuration Keys

```
Application:ClientOptions:MaxRequestsPerDay       # Default: 10
Application:ClientOptions:ClientIpHeaderName      # Header name for real IP (e.g. Fly.io)
Infrastructure:DeepLTranslatorClientOptions:ApiKey
Infrastructure:AffirmationClientOptions:BaseUrl
Infrastructure:RedisClientOptions:ConnectionString
```

## CI/CD

- **PR to main** → `build-test.yml` (restore → build → test)
- **Push to main** → `fly-deploy.yml` (build-test + deploy to Fly.io)
- Pre-commit hooks via `dotnet husky` (defined in `.husky/`)
