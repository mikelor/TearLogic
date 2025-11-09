# TearLogic Developer Guide

Version 1 of the TearLogic CB Insights integration ships a production-ready REST surface, typed Kiota client, and agent documentation that collectively let us stand up competitive-intelligence workflows on top of CB Insights data.

## Version 1 Highlights

- **Comprehensive organization discovery.** `OrganizationLookupController` supports name-based discovery (single name, multi-name, and raw lookup requests) with model validation so downstream callers get consistently shaped `OrgLookupResponse` payloads.【F:src/TearLogic.Api/Controllers/OrganizationLookupController.cs†L9-L107】
- **360° company context.** Firmographics, financial transactions, relationships, scouting reports, and outlook endpoints expose the data required to build the TearLogic tear sheet without bespoke CB Insights calls.【F:src/TearLogic.Api/Controllers/FirmographicsController.cs†L9-L103】【F:src/TearLogic.Api/Controllers/FinancialTransactionsController.cs†L9-L120】【F:src/TearLogic.Api/Controllers/OrganizationRelationshipsController.cs†L9-L81】【F:src/TearLogic.Api/Controllers/ScoutingReportController.cs†L9-L148】【F:src/TearLogic.Api/Controllers/OutlookController.cs†L9-L88】
- **Conversational access with streaming.** The ChatCbi endpoints provide both standard and streaming responses so conversational clients can choose between low-latency chunks or a single response body.【F:src/TearLogic.Api/Controllers/ChatCbiController.cs†L9-L81】
- **Shared command and validation primitives.** Request DTOs translate HTTP payloads into Kiota models, while reusable command handlers, controller extensions, and resource-backed logging centralize cross-cutting behaviors.【F:src/TearLogic.Api/Requests/OrganizationLookupRequest.cs†L1-L64】【F:src/TearLogic.Api/Commands/CommandHandler.cs†L1-L31】【F:src/TearLogic.Api/Controllers/Extensions/ControllerValidationExtensions.cs†L9-L35】【F:src/TearLogic.Api/Resources/LogMessages.resx†L1-L200】

## Solution Layout

| Folder | Description |
| --- | --- |
| `Agent/` | Documentation for the Glean agent surface that orchestrates CB Insights lookups before responses are rendered in downstream apps. |
| `TearLogic.Api/` | ASP.NET Core service that exposes TearLogic-specific REST endpoints and proxies CB Insights datasets. It houses the command handlers, request models, controllers, and infrastructure used at runtime. |
| `TearLogic.Clients/` | Kiota-generated API client that wraps the CB Insights OpenAPI description so that the API can be consumed from C#. |
| `TearLogic.slnx` | Visual Studio solution file that groups the API and client projects. |
| `kiota-cbinsights.cmd` | Helper script that re-generates the Kiota client from the upstream CB Insights OpenAPI document. |

## TearLogic.Api

### Composition & Cross-cutting Services

`Program.cs` wires up controller discovery, Swagger/OpenAPI metadata, localization-backed logging providers, strongly-typed CB Insights options, resilient HTTP clients (retry + circuit breaker), and the command-handler dependency graph that powers each endpoint.【F:src/TearLogic.Api/Program.cs†L9-L115】 Options validation ensures the CB Insights client ID/secret are present before the API starts.【F:src/TearLogic.Api/Program.cs†L41-L57】【F:src/TearLogic.Api/Configuration/CBInsightsOptions.cs†L1-L35】

### Feature Map

| Area | Controller | Key Routes |
| --- | --- | --- |
| Organization discovery | `OrganizationLookupController` | `POST /api/cbinsights/organizations`, `GET /api/cbinsights/organizations`, `GET /api/cbinsights/organizations/{organization}` |
| Firmographics | `FirmographicsController` | `POST /api/cbinsights/firmographics` |
| Financial transactions | `FinancialTransactionsController` | `GET /api/cbinsights/organizations/{id}/financialtransactions/{fundings|investments|portfolioexits}` |
| Relationships & leadership | `OrganizationRelationshipsController` | `GET /api/cbinsights/organizations/{id}/businessrelationships`, `GET /api/cbinsights/organizations/{id}/managementandboard` |
| Scouting reports | `ScoutingReportController` | `POST /api/cbinsights/organizations/{id}/scoutingreport`, `POST /api/cbinsights/organizations/{id}/scoutingreport/stream` |
| Outlook & scoring | `OutlookController` | `POST /api/cbinsights/organizations/{id}/outlook`, `POST /api/cbinsights/organizations/{id}/outlook/stream`, `GET /api/cbinsights/organizations/{id}/mosaicscore` |
| Conversational insights | `ChatCbiController` | `POST /api/cbinsights/chat`, `POST /api/cbinsights/chat/stream` |

Each controller delegates to a dedicated command handler, letting us keep HTTP concerns thin while the handlers route requests through the shared `ICBInsightsClient`. Validation helpers (for example, verifying numeric identifiers) live in `Controllers/Extensions` so every endpoint enforces consistent rules.【F:src/TearLogic.Api/Controllers/OrganizationLookupController.cs†L9-L107】【F:src/TearLogic.Api/Controllers/FirmographicsController.cs†L9-L103】【F:src/TearLogic.Api/Controllers/FinancialTransactionsController.cs†L9-L120】【F:src/TearLogic.Api/Controllers/OrganizationRelationshipsController.cs†L9-L81】【F:src/TearLogic.Api/Controllers/ScoutingReportController.cs†L9-L148】【F:src/TearLogic.Api/Controllers/OutlookController.cs†L9-L88】【F:src/TearLogic.Api/Controllers/ChatCbiController.cs†L9-L81】【F:src/TearLogic.Api/Controllers/Extensions/ControllerValidationExtensions.cs†L9-L35】 Request objects in `Requests/` project the wire format into Kiota-friendly models that the handlers consume.【F:src/TearLogic.Api/Requests/OrganizationLookupRequest.cs†L1-L64】【F:src/TearLogic.Api/Requests/FinancialTransactionsListRequest.cs†L1-L70】【F:src/TearLogic.Api/Requests/ChatCbiRequest.cs†L1-L82】【F:src/TearLogic.Api/Requests/ManagementAndBoardRequest.cs†L1-L78】

### Infrastructure & CB Insights Integration

`Infrastructure/CBInsightsClient.cs` centralizes outbound calls. It builds Kiota request adapters, caches OAuth tokens, applies resilience policies, emits localized log messages, and exposes async methods for each dataset (firmographics, transactions, scouting, outlook, chat, etc.).【F:src/TearLogic.Api/Infrastructure/CBInsightsClient.cs†L19-L266】 Supporting factories manage authentication (`CBInsightsAuthenticationProvider`, `CBInsightsTokenProvider`, and `CBInsightsRequestAdapterFactory`) and leverage the same configuration and HTTP clients registered in `Program.cs`.【F:src/TearLogic.Api/Infrastructure/CBInsightsAuthenticationProvider.cs†L1-L120】【F:src/TearLogic.Api/Infrastructure/CBInsightsTokenProvider.cs†L1-L160】【F:src/TearLogic.Api/Infrastructure/CBInsightsRequestAdapterFactory.cs†L1-L111】

## TearLogic.Clients and Kiota

`TearLogic.Clients` stores the upstream OpenAPI description and the generated Kiota SDK under `kiota/`. `TearLogic.Clients.csproj` packages these models so `TearLogic.Api` (and any future consumers) can reuse a single typed surface for CB Insights operations.【F:src/TearLogic.Clients/TearLogic.Clients.csproj†L1-L45】 Regenerate the client by running `kiota-cbinsights.cmd` from the `src/` directory; the script invokes `kiota generate` and then prints schema metadata via `kiota info`.【F:src/kiota-cbinsights.cmd†L1-L2】

## Local Development

1. Restore and build the solution:
   ```bash
   dotnet build TearLogic.slnx
   ```
2. Provide CB Insights credentials via user secrets or environment variables (`CBInsights:ClientId`, `CBInsights:ClientSecret`, `CBInsights:BaseUrl`, etc.) before launching the API.【F:src/TearLogic.Api/Program.cs†L41-L57】【F:src/TearLogic.Api/appsettings.json†L9-L18】
3. Run the API locally:
   ```bash
   dotnet run --project TearLogic.Api
   ```
   Swagger UI will be available in development environments with pre-populated server definitions for production, sandbox, and local/dev tunnel scenarios.【F:src/TearLogic.Api/Program.cs†L59-L115】

## Agent Documentation

The `Agent/README.md` file offers step-by-step guidance for constructing the CB Insights Glean agent, including authentication flow, example actions, and how to call the agent from C# code. Review it when integrating TearLogic APIs with conversational surfaces or automation workflows.【F:src/Agent/README.md†L1-L214】

## Additional Notes for Developers

- Keep CB Insights credentials in secure configuration providers; the defaults in `appsettings.json` are placeholders.【F:src/TearLogic.Api/appsettings.json†L9-L18】
- When adding new CB Insights endpoints, extend the Kiota client or the `ICBInsightsClient` rather than issuing raw HTTP calls, and register additional command handlers to mirror the existing architecture.【F:src/TearLogic.Api/Program.cs†L72-L107】【F:src/TearLogic.Api/Infrastructure/CBInsightsClient.cs†L19-L266】
- Update the OpenAPI document and regenerate client stubs whenever TearLogic endpoints change so documentation stays in sync.【F:src/TearLogic.Api/OpenApi/tearlogic-api.openapi.yaml†L1-L200】
