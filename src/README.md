## Architecture overview
  * Build a custom Action in Glean that calls the CB Insights API (auth, endpoint, mapping). This lets Glean Chat/Agents call CB Insights on demand and return results in chat with citations and guardrails.  

  * Create a Glean Agent in Agent Builder that uses that Action (plus optional pre/post processing, branching, and formatting). Then publish it and note the Agent ID. You can invoke it programmatically via the Agents API.  

  * In your C# service/UI, call the Agents API (stream or blocking) to run the Agent and render responses.  


This pattern keeps CB Insights integration centralized (via Actions), leverages Glean’s agentic orchestration, and gives you a clean C# integration surface.  

## Prerequisites
  * A Glean user-scoped API token with scope for agents (and chat/search if you’ll mix those).  

  * CB Insights API access (account + key) and target endpoints (e.g., company search, deals, market profiles). CB Insights publishes an API & data feeds program and documents their API (v2 available; v1 in maintenance).  

## Resources
  * [Glean Actions documentation](https://docs.glean.com/actions) - how to build custom Actions that call external APIs.  
  * [Glean Agent Builder documentation](https://docs.glean.com/agent-builder) - how to create and publish Glean Agents using Actions.  
  * [Glean Agents API documentation](https://docs.glean.com/agents-api) - how to invoke Glean Agents programmatically.  
  * [CB Insights API documentation](https://www.cbinsights.com/research/api) - details on available endpoints, authentication, and data schemas.

## Glean Action Intructions
  * Auth pre?step: POST  https://api.cbinsights.com/v2/authorize with clientId and clientSecret to get token.  
  * Authorization: include Authorization: Bearer {{steps.pre_auth.response.token}} in the downstream step headers.  
  * Secrets: store CBI clientId/clientSecret in Glean’s action secrets (do not ask users to provide them at runtime).  
  * JSON pointers in mappings reference fields from the CB Insights v2 OpenAPI (e.g., $.orgs[0].orgId).  

### Action 1 — CBInsights.OrganizationLookup (credit?free canonicalization)
Resolves org by domain (preferred) or name to obtain orgId and basic info. This endpoint “never charges credits.”
```json
{
  "name": "CBInsights.OrganizationLookup",
  "description": "Resolve an organization to CB Insights orgId by domain or name.",
  "secrets": {
    "CBI_CLIENT_ID": "string",
    "CBI_CLIENT_SECRET": "string"
  },
  "inputs": {
    "domain": { "type": "string", "required": false, "description": "e.g. example.com" },
    "name": { "type": "string", "required": false, "description": "Exact org name" },
    "limit": { "type": "integer", "required": false, "default": 1, "minimum": 1, "maximum": 100 }
  },
  "steps": [
    {
      "name": "pre_auth",
      "request": {
        "method": "POST",
        "url": "https://api.cbinsights.com/v2/authorize",
        "headers": { "Content-Type": "application/json" },
        "body": {
          "clientId": "{{secrets.CBI_CLIENT_ID}}",
          "clientSecret": "{{secrets.CBI_CLIENT_SECRET}}"
        }
      },
      "extract": {
        "token": "$.token"
      }
    },
    {
      "name": "lookup",
      "request": {
        "method": "POST",
        "url": "https://api.cbinsights.com/v2/organizations",
        "headers": {
          "Authorization": "Bearer {{steps.pre_auth.response.token}}",
          "Content-Type": "application/json"
        },
        "body": {
          "urls": "{{#if inputs.domain}}[\"{{inputs.domain}}\"]{{/if}}",
          "names": "{{#if inputs.name}}[\"{{inputs.name}}\"]{{/if}}",
          "limit": "{{inputs.limit}}"
        }
      }
    }
  ],
  "output_mapping": {
    "org": {
      "orgId": "$.steps.lookup.response.orgs[0].orgId",
      "name": "$.steps.lookup.response.orgs[0].name",
      "urls": "$.steps.lookup.response.orgs[0].urls",
      "description": "$.steps.lookup.response.orgs[0].description",
      "aliases": "$.steps.lookup.response.orgs[0].aliases"
    }
  },
  "on_empty": { "org": null }
}
```

### Action 2 — CBInsights.OrganizationFirmographics (rich profile enrichment)
Returns summary, taxonomy, financials, headcount, identifiers for snapshots and context.
```json
{
  "name": "CBInsights.OrganizationFirmographics",
  "description": "Fetch firmographics for an organization (summary, taxonomy, financials, headcount).",
  "secrets": {
    "CBI_CLIENT_ID": "string",
    "CBI_CLIENT_SECRET": "string"
  },
  "inputs": {
    "orgId": { "type": "integer", "required": false },
    "domain": { "type": "string", "required": false },
    "name": { "type": "string", "required": false },
    "limit": { "type": "integer", "required": false, "default": 1, "minimum": 1, "maximum": 100 },
    "nextPageToken": { "type": "string", "required": false }
  },
  "steps": [
    {
      "name": "pre_auth",
      "request": {
        "method": "POST",
        "url": "https://api.cbinsights.com/v2/authorize",
        "headers": { "Content-Type": "application/json" },
        "body": {
          "clientId": "{{secrets.CBI_CLIENT_ID}}",
          "clientSecret": "{{secrets.CBI_CLIENT_SECRET}}"
        }
      },
      "extract": { "token": "$.token" }
    },
    {
      "name": "firmographics",
      "request": {
        "method": "POST",
        "url": "https://api.cbinsights.com/v2/firmographics",
        "headers": {
          "Authorization": "Bearer {{steps.pre_auth.response.token}}",
          "Content-Type": "application/json"
        },
        "body": {
          "orgIds": "{{#if inputs.orgId}}[{{inputs.orgId}}]{{/if}}",
          "urls": "{{#if inputs.domain}}[\"{{inputs.domain}}\"]{{/if}}",
          "orgNames": "{{#if inputs.name}}[\"{{inputs.name}}\"]{{/if}}",
          "limit": "{{inputs.limit}}",
          "nextPageToken": "{{inputs.nextPageToken}}"
        }
      }
    }
  ],
  "output_mapping": {
    "org": {
      "orgId": "$.steps.firmographics.response.orgs[0].orgId",
      "name": "$.steps.firmographics.response.orgs[0].summary.name",
      "summary": {
        "description": "$.steps.firmographics.response.orgs[0].summary.description",
        "url": "$.steps.firmographics.response.orgs[0].summary.url",
        "profileUrl": "$.steps.firmographics.response.orgs[0].summary.profileUrl",
        "status": "$.steps.firmographics.response.orgs[0].summary.status",
        "stage": "$.steps.firmographics.response.orgs[0].summary.stage",
        "address": {
          "city": "$.steps.firmographics.response.orgs[0].summary.address.city",
          "country": "$.steps.firmographics.response.orgs[0].summary.address.country"
        }
      },
      "taxonomy": {
        "sector": "$.steps.firmographics.response.orgs[0].taxonomy.sector",
        "industry": "$.steps.firmographics.response.orgs[0].taxonomy.industry",
        "subindustry": "$.steps.firmographics.response.orgs[0].taxonomy.subindustry",
        "marketNames": "$.steps.firmographics.response.orgs[0].taxonomy.marketNames"
      },
      "financials": {
        "lastFundingDate": "$.steps.firmographics.response.orgs[0].financials.lastFundingDate",
        "totalFunding": "$.steps.firmographics.response.orgs[0].financials.totalFunding",
        "valuation": "$.steps.firmographics.response.orgs[0].financials.valuation"
      },
      "headcount": {
        "currentHeadcount": "$.steps.firmographics.response.orgs[0].headcount.currentHeadcount"
      }
    },
    "nextPageToken": "$.steps.firmographics.response.nextPageToken",
    "totalHits": "$.steps.firmographics.response.totalHits"
  }
}
```
### Action 3 — CBInsights.OrganizationFundings (latest funding rounds)
Gets funding transactions (date, round, amount, valuation, investors, sources) for a single orgId, with paging.
```json
{
  "name": "CBInsights.OrganizationFundings",
  "description": "Fetch funding transactions for a single orgId.",
  "secrets": {
    "CBI_CLIENT_ID": "string",
    "CBI_CLIENT_SECRET": "string"
  },
  "inputs": {
    "orgId": { "type": "integer", "required": true },
    "limit": { "type": "integer", "required": false, "default": 3, "minimum": 1, "maximum": 100 },
    "nextPageToken": { "type": "string", "required": false }
  },
  "steps": [
    {
      "name": "pre_auth",
      "request": {
        "method": "POST",
        "url": "https://api.cbinsights.com/v2/authorize",
        "headers": { "Content-Type": "application/json" },
        "body": {
          "clientId": "{{secrets.CBI_CLIENT_ID}}",
          "clientSecret": "{{secrets.CBI_CLIENT_SECRET}}"
        }
      },
      "extract": { "token": "$.token" }
    },
    {
      "name": "fundings",
      "request": {
        "method": "POST",
        "url": "https://api.cbinsights.com/v2/organizations/{{inputs.orgId}}/financialtransactions/fundings",
        "headers": {
          "Authorization": "Bearer {{steps.pre_auth.response.token}}",
          "Content-Type": "application/json"
        },
        "body": {
          "limit": "{{inputs.limit}}",
          "nextPageToken": "{{inputs.nextPageToken}}"
        }
      }
    }
  ],
  "output_mapping": {
    "orgId": "{{inputs.orgId}}",
    "funding_rounds": {
      "$map": "$.steps.fundings.response.fundings",
      "date": "$.date",
      "round": "$.round",
      "simplifiedRound": "$.simplifiedRound",
      "amountInMillions": "$.amountInMillions",
      "valuationInMillions": "$.valuationInMillions",
      "dealId": "$.dealId",
      "investors": "$.investors",
      "sources": "$.sources"
    },
    "nextPageToken": "$.steps.fundings.response.nextPageToken",
    "totalHits": "$.steps.fundings.response.totalHits"
  }
}
```
#### Notes
If you need more than N results, loop by passing nextPageToken in subsequent invocations; the response includes nextPageToken and totalHits.

## Recommended Agent Flow
  * Step A: Call CBInsights.OrganizationLookup with domain (preferred) or name; handle no match.  
  * Step B: Call CBInsights.OrganizationFundings with orgId; limit=3 by default; optionally paginate for more.  
  * Step C (optional): Call CBInsights.OrganizationFirmographics to enrich the snapshot.  
  * Step D: Summarize plus include “sources” URLs from fundings for citations in your chat response. Then invoke your Agent from C# via Glean’s Agents API stream/wait endpoints

## Implementation tips
Keep clientId/clientSecret in Glean Action secrets; the pre?auth step injects a fresh token per call. If you later need to optimize, you can centralize token issuance in a dedicated “GetToken” Action and share it across Actions, but this inline pre?step satisfies your current requirement.  

Use the credit?free organizations endpoint first to avoid unnecessary billed calls and to establish a canonical orgId

## CBInsights.FundingsList (bulk funding for multiple orgIds)
Resolves funding transactions for 1–100 orgIds with paging (limit and nextPageToken). POST /v2/financialtransactions/fundings.
```json
{
  "name": "CBInsights.FundingsList",
  "description": "Bulk: list funding transactions for multiple orgIds with pagination.",
  "secrets": {
    "CBI_CLIENT_ID": "string",
    "CBI_CLIENT_SECRET": "string"
  },
  "inputs": {
    "orgIds": { "type": "array", "items": { "type": "integer" }, "required": true, "description": "1-100 orgIds" },
    "limit": { "type": "integer", "required": false, "default": 100, "minimum": 1, "maximum": 100 },
    "nextPageToken": { "type": "string", "required": false }
  },
  "steps": [
    {
      "name": "pre_auth",
      "request": {
        "method": "POST",
        "url": "https://api.cbinsights.com/v2/authorize",
        "headers": { "Content-Type": "application/json" },
        "body": {
          "clientId": "{{secrets.CBI_CLIENT_ID}}",
          "clientSecret": "{{secrets.CBI_CLIENT_SECRET}}"
        }
      },
      "extract": { "token": "$.token" }
    },
    {
      "name": "bulk_fundings",
      "request": {
        "method": "POST",
        "url": "https://api.cbinsights.com/v2/financialtransactions/fundings",
        "headers": {
          "Authorization": "Bearer {{steps.pre_auth.response.token}}",
          "Content-Type": "application/json"
        },
        "body": {
          "orgIds": "{{inputs.orgIds}}",
          "limit": "{{inputs.limit}}",
          "nextPageToken": "{{inputs.nextPageToken}}"
        }
      }
    }
  ],
  "output_mapping": {
    "orgs": {
      "$map": "$.steps.bulk_fundings.response.orgs",
      "orgId": "$.orgId",
      "funding_rounds": {
        "$map": "$.fundings",
        "date": "$.date",
        "round": "$.round",
        "simplifiedRound": "$.simplifiedRound",
        "amountInMillions": "$.amountInMillions",
        "valuationInMillions": "$.valuationInMillions",
        "dealId": "$.dealId",
        "investors": "$.investors",
        "sources": "$.sources"
      },
      "capTableHistory": "$.capTableHistory"
    },
    "nextPageToken": "$.steps.bulk_fundings.response.nextPageToken",
    "totalHits": "$.steps.bulk_fundings.response.totalHits",
    "totalHitsRelation": "$.steps.bulk_fundings.response.totalHitsRelation"
  }
}
```
*The request body accepts up to 100 orgIds and supports pagination via nextPageToken; response returns orgs with fundings and optional capTableHistory.*

## CBInsights.OrganizationOutlook (single?org scores and signals)
Returns current Mosaic (overall and sub?scores), Exit Probability (IPO/M&A), and Commercial Maturity for a single orgId. POST /v2/organizations/{orgId}/outlook.
```json
{
  "name": "CBInsights.OrganizationOutlook",
  "description": "Get Mosaic, Exit Probability, and Commercial Maturity for a single orgId.",
  "secrets": {
    "CBI_CLIENT_ID": "string",
    "CBI_CLIENT_SECRET": "string"
  },
  "inputs": {
    "orgId": { "type": "integer", "required": true }
  },
  "steps": [
    {
      "name": "pre_auth",
      "request": {
        "method": "POST",
        "url": "https://api.cbinsights.com/v2/authorize",
        "headers": { "Content-Type": "application/json" },
        "body": {
          "clientId": "{{secrets.CBI_CLIENT_ID}}",
          "clientSecret": "{{secrets.CBI_CLIENT_SECRET}}"
        }
      },
      "extract": { "token": "$.token" }
    },
    {
      "name": "outlook",
      "request": {
        "method": "POST",
        "url": "https://api.cbinsights.com/v2/organizations/{{inputs.orgId}}/outlook",
        "headers": {
          "Authorization": "Bearer {{steps.pre_auth.response.token}}",
          "Content-Type": "application/json"
        }
      }
    }
  ],
  "output_mapping": {
    "orgId": "{{inputs.orgId}}",
    "mosaic": {
      "overall": {
        "score": "$.steps.outlook.response.mosaicScore.overall.scoreValue",
        "asOfDate": "$.steps.outlook.response.mosaicScore.overall.asOfDate",
        "insights": "$.steps.outlook.response.mosaicScore.overall.scoreInsights"
      },
      "management": "$.steps.outlook.response.mosaicScore.management",
      "market": "$.steps.outlook.response.mosaicScore.market",
      "momentum": "$.steps.outlook.response.mosaicScore.momentum",
      "money": "$.steps.outlook.response.mosaicScore.money"
    },
    "exitProbability": {
      "ipo": "$.steps.outlook.response.exitProbability.ipo",
      "mna": "$.steps.outlook.response.exitProbability.mna"
    },
    "commercialMaturity": "$.steps.outlook.response.commercialMaturity"
  }
}
```
## Notes for wiring into your Agent
  * Start with your existing OrganizationLookup to resolve orgId, then call FundingsList (for one or more orgs) and OrganizationOutlook (for a focused company card), and optionally Firmographics to round out the snapshot.  

  * All Actions use a pre?auth step to POST /v2/authorize with your clientId/clientSecret and include Authorization: Bearer <token> on the downstream request.  

  * Invoke your Agent from C# via Glean’s Agents API (stream or wait) exactly as before.


  Here are the exact input/output schemas and a runnable plan you can drop into your Agent, wired to the Actions we defined.

### Agent definition
**Name:** CBI Company Snapshot  
**Purpose:** Given a company domain or name, resolve its orgId, fetch latest funding rounds, and optionally enrich with firmographics and outlook (Mosaic, Exit Probability, Commercial Maturity). Uses CB Insights v2 endpoints with an inline pre?auth step to obtain a short?lived bearer token. 

---

### Input schema (for your Agent)
```json
{
  "type": "object",
  "properties": {
    "domain": { "type": "string", "description": "Company website domain, e.g., acme.com" },
    "name": { "type": "string", "description": "Exact company name (fallback if domain absent)" },
    "topN": { "type": "integer", "minimum": 1, "maximum": 100, "default": 3, "description": "Number of latest funding rounds" },
    "includeFirmographics": { "type": "boolean", "default": true },
    "includeOutlook": { "type": "boolean", "default": true }
  },
  "required": []
}
```

---

### Output schema (from your Agent)
```json
{
  "type": "object",
  "properties": {
    "org": {
      "type": "object",
      "properties": {
        "orgId": { "type": "integer" },
        "name": { "type": "string" },
        "urls": { "type": "array", "items": { "type": "string" } }
      }
    },
    "firmographics": {
      "type": "object",
      "properties": {
        "summary": {
          "type": "object",
          "properties": {
            "description": { "type": "string" },
            "url": { "type": "string" },
            "profileUrl": { "type": "string" },
            "status": { "type": "string" },
            "stage": { "type": "string" },
            "address": {
              "type": "object",
              "properties": { "city": { "type": "string" }, "country": { "type": "string" } }
            }
          }
        },
        "taxonomy": {
          "type": "object",
          "properties": {
            "sector": { "type": "string" },
            "industry": { "type": "string" },
            "subindustry": { "type": "string" },
            "marketNames": { "type": "array", "items": { "type": "string" } }
          }
        },
        "financials": {
          "type": "object",
          "properties": {
            "lastFundingDate": { "type": "string" },
            "totalFunding": { "type": "number" },
            "valuation": { "type": "number" }
          }
        },
        "headcount": {
          "type": "object",
          "properties": {
            "currentHeadcount": { "type": "integer" }
          }
        }
      }
    },
    "funding_rounds": {
      "type": "array",
      "items": {
        "type": "object",
        "properties": {
          "date": { "type": "string" },
          "round": { "type": "string" },
          "simplifiedRound": { "type": "string" },
          "amountInMillions": { "type": "number" },
          "valuationInMillions": { "type": "number" },
          "dealId": { "type": "integer" },
          "investors": {
            "type": "array",
            "items": {
              "type": "object",
              "properties": {
                "name": { "type": "string" },
                "orgId": { "type": "integer" },
                "isLead": { "type": "boolean" },
                "isNew": { "type": "boolean" },
                "isFollowOn": { "type": "boolean" }
              }
            }
          },
          "sources": { "type": "array", "items": { "type": "string" } }
        }
      }
    },
    "outlook": {
      "type": "object",
      "properties": {
        "mosaic": { "type": "object" },
        "exitProbability": { "type": "object" },
        "commercialMaturity": { "type": "object" }
      }
    },
    "sources": { "type": "array", "items": { "type": "string" } }
  }
}
```

---

### Tools (Actions) the Agent will call
**CBInsights.OrganizationLookup**: POST /v2/organizations (credit?free; resolves org by domain/name to orgId).   
**CBInsights.OrganizationFundings**: POST /v2/organizations/{orgId}/financialtransactions/fundings (latest transactions, paginated).   
**CBInsights.OrganizationFirmographics** (optional): POST /v2/firmographics (rich profile).   
**CBInsights.OrganizationOutlook** (optional): POST /v2/organizations/{orgId}/outlook (Mosaic, Exit Probability, Commercial Maturity). 

All of these include a pre?auth step that POSTs /v2/authorize with clientId/clientSecret and injects Authorization: Bearer <token> for the downstream call. Configure clientId/clientSecret as Action secrets in Glean. 

---

### Plan & Execute outline (nodes you can configure)
```json
{
  "nodes": [
    {
      "name": "ValidateInputs",
      "type": "logic",
      "condition": "!inputs.domain && !inputs.name",
      "action": "ask_user",
      "message": "I can look up a company by domain (preferred) or exact name. Please provide one."
    },
    {
      "name": "LookupOrg",
      "type": "action",
      "actionRef": "CBInsights.OrganizationLookup",
      "params": { "domain": "{{inputs.domain}}", "name": "{{inputs.name}}", "limit": 1 },
      "on_error": { "action": "fail_soft", "message": "Organization lookup failed. Please check the domain or name and try again." },
      "on_result_condition": "!result.org",
      "on_result_action": { "action": "ask_user", "message": "I couldn’t find a match. Can you share the company’s website domain or full legal name?" }
    },
    {
      "name": "GetFundings",
      "type": "action",
      "actionRef": "CBInsights.OrganizationFundings",
      "params": { "orgId": "{{nodes.LookupOrg.result.org.orgId}}", "limit": "{{inputs.topN}}" },
      "on_error": { "action": "warn", "message": "I couldn’t retrieve funding data right now; continuing with basic info." }
    },
    {
      "name": "MaybeFirmographics",
      "type": "branch",
      "condition": "inputs.includeFirmographics == true",
      "then": {
        "name": "GetFirmographics",
        "type": "action",
        "actionRef": "CBInsights.OrganizationFirmographics",
        "params": { "orgId": "{{nodes.LookupOrg.result.org.orgId}}", "limit": 1 },
        "on_error": { "action": "warn", "message": "Firmographics are temporarily unavailable; continuing." }
      }
    },
    {
      "name": "MaybeOutlook",
      "type": "branch",
      "condition": "inputs.includeOutlook == true",
      "then": {
        "name": "GetOutlook",
        "type": "action",
        "actionRef": "CBInsights.OrganizationOutlook",
        "params": { "orgId": "{{nodes.LookupOrg.result.org.orgId}}"},
        "on_error": { "action": "warn", "message": "Outlook scores are temporarily unavailable; continuing." }
      }
    },
    {
      "name": "AssembleOutput",
      "type": "transform",
      "code": "// Pseudocode transform\nconst org = nodes.LookupOrg.result.org || {};\nconst firmo = nodes.GetFirmographics?.result?.org || null;\nconst fund = nodes.GetFundings?.result || { funding_rounds: [] };\nconst outl = nodes.GetOutlook?.result || null;\n\nconst sources = [];\nfor (const fr of (fund.funding_rounds || [])) {\n  if (Array.isArray(fr.sources)) sources.push(...fr.sources);\n}\n\nreturn {\n  org: { orgId: org.orgId, name: org.name, urls: org.urls || [] },\n  firmographics: firmo ? {\n    summary: firmo.summary,\n    taxonomy: firmo.taxonomy,\n    financials: firmo.financials,\n    headcount: firmo.headcount\n  } : null,\n  funding_rounds: fund.funding_rounds || [],\n  outlook: outl ? outl : null,\n  sources: Array.from(new Set(sources)).slice(0, 20)\n};"
    },
    {
      "name": "RenderAnswer",
      "type": "respond",
      "template": "Here’s the snapshot for {{output.org.name}} (OrgID: {{output.org.orgId}}).\\n\\n- Summary: {{output.firmographics.summary.description}}\\n- Sector/Industry: {{output.firmographics.taxonomy.sector}} / {{output.firmographics.taxonomy.industry}}\\n- Headcount: {{output.firmographics.headcount.currentHeadcount}}\\n- Total funding: ${{output.firmographics.financials.totalFunding}}M; Last round date: {{output.firmographics.financials.lastFundingDate}}\\n\\nLatest {{inputs.topN}} funding rounds:\\n{{#each output.funding_rounds}}- {{this.date}} • {{this.simplifiedRound || this.round}} • ${{this.amountInMillions}}M {{#if this.valuationInMillions}}(post: ${{this.valuationInMillions}}M){{/if}} • Investors: {{#if this.investors}}{{this.investors.[0].name}}{{#if this.investors.[1]}}, +{{minus this.investors.length 1}} more{{/if}}{{else}}n/a{{/if}}\\n{{/each}}\\n\\n{{#if output.outlook}}Outlook (if available): Mosaic overall {{output.outlook.mosaic.overall.score}} ({{output.outlook.mosaic.overall.asOfDate}}); Exit Prob (IPO {{output.outlook.exitProbability.ipo.exitProbability}}%, M&A {{output.outlook.exitProbability.mna.exitProbability}}%).{{/if}}\\n\\nSources:\\n{{#each output.sources}}- {{this}}\\n{{/each}}"
    }
  ]
}
```
Notes  
* The lookup step uses CB Insights v2 Organizations endpoint (credit?free) to find orgId, then calls Fundings, optional Firmographics, and optional Outlook.   
* Actions encapsulate a pre?auth POST /v2/authorize and pass Authorization: Bearer <token> to each dataset call. 

---

### Running the Agent from C# (schema?driven)
Use the **Agents API** to run your Agent with these inputs. For real?time chat, call the streaming endpoint; for batch, call the blocking endpoint. You can also fetch your Agent’s input/output schemas via the schemas endpoint to validate payloads in C#. 

Blocking example (wait):
```csharp
var baseUrl = "https://YOUR_INSTANCE/rest/api/v1";
var runUrl = $"{baseUrl}/agents/runs/wait";

var payload = new {
  agentId = "YOUR_AGENT_ID",
  input = new {
    domain = "acme-robotics.com",
    topN = 3,
    includeFirmographics = true,
    includeOutlook = true
  }
};

using var http = new HttpClient();
http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", gleanApiToken);
var req = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
var resp = await http.PostAsync(runUrl, req);
resp.EnsureSuccessStatusCode();
var json = await resp.Content.ReadAsStringAsync();
Console.WriteLine(json);
```
For real?time UX, use the streaming endpoint to process SSE chunks as they arrive. 

---

### Test checklist
* Domain only, name only, both, and no match paths. 

* No fundings; many fundings (pagination using nextPageToken); missing valuation; investor arrays empty. 

* Confirm Action pre?auth success and error handling (400/401/403/424/500) with friendly user messages. 

---

If you want, I can turn this into a ready-to-import Agent draft with your exact Action IDs and your Agent ID placeholders filled in.

--- 
Would you like a “compare multiple companies” variant (bulk fundings + outlook) with a tabular response so you can pass an array of domains and get a cross-company view?