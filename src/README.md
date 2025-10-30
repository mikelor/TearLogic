# TearLogic Developer Guide

This document provides a high-level tour of the source tree and explains how the generated CB Insights client is produced.

## Solution Layout

The solution root contains several developer-facing projects:

| Folder | Description |
| --- | --- |
| `Agent/` | Documentation for the Glean agent surface that orchestrates CB Insights lookups before responses are rendered in downstream apps. |
| `TearLogic.Api/` | ASP.NET Core service that exposes TearLogic-specific REST endpoints and proxies CB Insights datasets. It houses the command handlers, request models, controllers, and infrastructure used at runtime. |
| `TearLogic.Clients/` | Kiota-generated API client that wraps the CB Insights OpenAPI description so that the API can be consumed from C#. |
| `TearLogic.slnx` | Visual Studio solution file that groups the API and client projects. |
| `kiota-cbinsights.cmd` | Helper script that re-generates the Kiota client from the upstream CB Insights OpenAPI document. |

## TearLogic.Api

`TearLogic.Api` is an ASP.NET Core web API responsible for normalizing CB Insights access behind TearLogic endpoints:

* **Composition** – `Program.cs` wires up controllers, Swagger, and strongly-typed options that validate CB Insights credentials at startup, and registers typed `HttpClient` instances that share Polly retry/circuit-breaker policies.【F:src/TearLogic.Api/Program.cs†L1-L87】
* **Configuration** – `Configuration/CBInsightsOptions.cs` stores the CB Insights base URL, endpoint paths, credentials, and token cache duration, with data annotation validation to protect against misconfiguration.【F:src/TearLogic.Api/Configuration/CBInsightsOptions.cs†L1-L35】
* **Command layer** – `Commands/` defines a reusable `CommandHandler<TCommand, TResult>` base and feature-specific command models to keep controller logic thin and testable.【F:src/TearLogic.Api/Commands/CommandHandler.cs†L1-L31】
* **Controllers** – `Controllers/` surface organization lookup and firmographics endpoints that call into command handlers, returning TearLogic-specific responses.
* **Infrastructure** – `Infrastructure/CBInsightsClient.cs` encapsulates Microsoft Kiota request adapters, authentication, and logging/error handling, exposing async methods for organization and firmographics lookups.【F:src/TearLogic.Api/Infrastructure/CBInsightsClient.cs†L1-L120】 Supporting infrastructure includes cached token acquisition, Polly policies, and constants in the same folder.
* **OpenAPI definition** – `OpenApi/tearlogic-api.openapi.yaml` captures the TearLogic API contract for documentation and client generation.
* **Diagnostics & Resources** – `Diagnostics/ResourceProviders.cs` and the `Resources/` folder deliver localized log/error message strings consumed across the API.

## TearLogic.Clients and Kiota

`TearLogic.Clients` contains the generated C# client for the CB Insights platform:

* **Source of truth** – The folder stores the upstream `cbinsights_api_v2_openapi3.yaml` and the downloaded `cbinsights_api_v2.json` reference files alongside the generated Kiota output under `kiota/`.
* **Project file** – `TearLogic.Clients.csproj` exposes the Kiota-generated types as a .NET class library that other projects (e.g., `TearLogic.Api`) can reference.
* **Regeneration workflow** – Run `kiota-cbinsights.cmd` from the `src/` directory to recreate the client. The script performs two steps: it invokes `kiota generate` with the CB Insights OpenAPI URL, targeting the `TearLogic.Clients` namespace and writing output into `TearLogic.Clients/kiota`, and then runs `kiota info` to display metadata for the downloaded description.【F:src/kiota-cbinsights.cmd†L1-L2】 Ensure the Kiota CLI is installed and authenticated before running the script.

After regeneration, rebuild the solution so `TearLogic.Api` can pick up any surface changes in the `ICBInsightsClient` abstractions.

## Agent Documentation

The `Agent/README.md` file offers step-by-step guidance for constructing the CB Insights Glean agent, including authentication flow, example actions, and how to call the agent from C# code. Review it when integrating TearLogic APIs with conversational surfaces or automation workflows.【F:src/Agent/README.md†L1-L214】

## Additional Notes for Developers

* Keep CB Insights credentials in secure configuration providers; the defaults in `appsettings.json` are placeholders.
* When adding new CB Insights endpoints, prefer extending the Kiota client rather than writing raw HTTP calls, and register additional command handlers to mirror the existing architecture.
* Update the OpenAPI document and regenerate client stubs whenever TearLogic endpoints change so documentation stays in sync.
