#One Time Setup
#What you’ll need to fill in: your GitHub OWNER and REPO (e.g., OWNER=tearlogic, REPO=tearlogic-api).
#This creates an Entra app + SP, adds a federated credential for pushes to main, and grants Website Contributor on the specific Web App.

# ---- Required inputs ----
OWNER="mikelor"
REPO="tearlogic"
SUBSCRIPTION_ID="b7889dc6-971c-455e-b488-387d0f8a820f"
TENANT_ID="e6a916c6-9ab1-40d0-b5e4-07208617ed9e"
RG="tl-rg-dev"
WEBAPP_NAME="tl-dev-api"
APP_DISPLAY_NAME="tearlogic-github-dev-oidc"

# ---- Login & select subscription ----
az account set --subscription "$SUBSCRIPTION_ID"

# ---- Create an Entra app (the workload identity) ----
APP_ID=$(az ad app create --display-name "$APP_DISPLAY_NAME" --query appId -o tsv)

# Create the service principal for that app
SP_OBJECT_ID=$(az ad sp create --id "$APP_ID" --query id -o tsv)

# ---- Add a federated credential for GitHub Actions on main branch ----
# Subject patterns you can use:
#   repo:OWNER/REPO:ref:refs/heads/main
#   repo:OWNER/REPO:environment:production
az ad app federated-credential create \
  --id "$APP_ID" \
  --parameters '{
    "name":"github-main",
    "issuer":"https://token.actions.githubusercontent.com",
    "subject":"repo:'"$OWNER"'/'"$REPO"':ref:refs/heads/main",
    "audiences":["api://AzureADTokenExchange"]
  }'

# ---- Scope a minimal role to the Web App only ----
WEBAPP_ID=$(az webapp show -g "$RG" -n "$WEBAPP_NAME" --query id -o tsv)
az role assignment create \
  --assignee-object-id "$SP_OBJECT_ID" \
  --assignee-principal-type ServicePrincipal \
  --role "Website Contributor" \
  --scope "$WEBAPP_ID"

# ---- Output the values you’ll put into GitHub repo variables ----
echo "AZURE_TENANT_ID=$TENANT_ID"
echo "AZURE_SUBSCRIPTION_ID=$SUBSCRIPTION_ID"
echo "AZURE_CLIENT_ID=$APP_ID"