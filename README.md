![Tests](https://github.com/timok19/AffirmationGenerator/actions/workflows/tests.yml/badge.svg)
![Fly Deploy](https://github.com/timok19/AffirmationGenerator/actions/workflows/fly-deploy.yml/badge.svg)

# Affirmation Generator

## 1. Application Info
The Affirmation Generator is a web application designed to provide users with positive, uplifting affirmations in multiple languages to boost morale and mental well-being. It features a daily limit on affirmation generation to encourage mindful consumption and prevents overuse through rate limiting.

## 2. Getting Started

### Prerequisites
*   [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
*   [Node.js](https://nodejs.org/) (LTS recommended)
*   [pnpm](https://pnpm.io/) package manager
*   [Docker](https://www.docker.com/) (optional, for containerized run)

### Running Locally
1.  **Clone the repository:**
    ```bash
    git clone https://github.com/timok19/AffirmationGenerator.git
    cd AffirmationGenerator
    ```

2.  **Configuration:**
    Ensure you have the required API keys. For local development, use [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) to avoid committing sensitive data:
    ```bash
    cd AffirmationGenerator.Server
    dotnet user-secrets init
    dotnet user-secrets set "Infrastructure:DeepLTranslatorClientOptions:ApiKey" "DEEPL_API_KEY"
    dotnet user-secrets set "Infrastructure:AffirmationClientOptions:BaseUrl" "https://www.affirmations.dev"
    ```
    Alternatively, update `appsettings.json`.

3.  **Start the Application:**
    Navigate to the server directory and run:
    ```bash
    dotnet run
    ```
    This will start the ASP.NET Core backend and automatically launch the React frontend via the configured SPA proxy.
    The application will be available at: `https://localhost:7006` (or `http://localhost:5095`).

### Running with Docker
1.  **Build the image:**
    ```bash
    docker build -t affirmation-generator .
    ```
2.  **Run the container:**
    ```bash
    docker run -p 8080:8080 -e Infrastructure__DeepLTranslatorClientOptions__ApiKey="DEEPL_API_KEY" -e Infrastructure__AffirmationClientOptions__BaseUrl="https://www.affirmations.dev" affirmation-generator
    ```
    Access the application at: `http://localhost:8080`

## 3. Architecture

### System-Level Architecture
The application is built using a **Client-Server** model packaged into a single Docker container for easy deployment.
*   **Backend:** ASP.NET Core Web API (NET 10.0) acts as the server. It handles business logic, communicates with external APIs (Affirmation service, DeepL), enforces rate limiting, and serves the frontend as static files.
*   **Frontend:** A React Single Page Application (SPA) that provides the user interface. It consumes the backend API to fetch and display affirmations.
*   **Deployment:** The project uses a multi-stage `Dockerfile`. It builds the React frontend with Node.js, the .NET backend with the .NET SDK, and combines them into a final runtime image where ASP.NET serves the React static assets from `wwwroot`.

### Code-Level Architecture
**Backend (`AffirmationGenerator.Server`):**
The backend follows **Clean Architecture** principles, organizing code into distinct layers:
*   **Api:** Contains Controllers (`AffirmationsController`) that define endpoints and handle HTTP requests/responses. It also manages Rate Limiting policies.
*   **Application:** Contains business logic implemented using the CQRS pattern (e.g., `GetAffirmationQuery`, `GetRemainingAffirmationsQuery`).
*   **Infrastructure:** Implements interfaces for external services, such as:
    *   `AffirmationClient`: Fetches raw affirmations from an external source.
    *   `DeepLTranslatorClient`: Translates affirmations using the DeepL API.
*   **Domain:** Defines core data models (`AffirmationLanguage`) and error types.

**Frontend (`AffirmationGenerator.Client`):**
The frontend is a **React** application built with **Vite**.
*   **State Management:** Uses React Hooks (`useState`, `useEffect`) within `App.tsx` to manage application state (current affirmation, language, remaining count).
*   **API Communication:** Uses `Axios` to communicate with the backend endpoints (`/affirmations`, `/affirmations/remaining`).
*   **Styling:** Utilizes **Tailwind CSS v4** and **DaisyUI v5** for responsive and modern UI components.

## 4. Configuration
To configure the application, you need to set up the following settings in `appsettings.json` or via Environment Variables (recommended for production):

**Required Configuration:**
```json
{
  "Infrastructure": {
    "DeepLTranslatorClientOptions": {
      "ApiKey": "YOUR_DEEPL_API_KEY"
    },
    "AffirmationClientOptions": {
      "BaseUrl": "URL_TO_AFFIRMATION_SERVICE"
    }
  }
}
```

*   **`Infrastructure:DeepLTranslatorClientOptions:ApiKey`**: The API Key for the DeepL translation service.
*   **`Infrastructure:AffirmationClientOptions:BaseUrl`**: The base URL for the external affirmation provider service.

## 5. React Components & Libraries

### Libraries Used
*   **React 19:** Core UI library.
*   **Vite:** Fast build tool and development server.
*   **Tailwind CSS (v4):** Utility-first CSS framework.
*   **DaisyUI (v5):** Component library for Tailwind CSS.
*   **Axios:** Promise-based HTTP client for making API requests.
*   **TypeScript:** Adds static typing to JavaScript for better developer experience and code quality.



## 6. Useful links

*   [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
*   [React](https://react.dev/)
*   [Vite](https://vitejs.dev/)
*   [Tailwind CSS](https://tailwindcss.com/)
*   [DaisyUI](https://daisyui.com/)
*   [Axios](https://axios-http.com/)
*   [TypeScript](https://www.typescriptlang.org/)
*   [Docker](https://www.docker.com/)
*   [DeepL API](https://www.deepl.com/pro-api)