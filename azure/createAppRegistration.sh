OWNER="mikelor"
REPO="TearLogic"
SUBSCRIPTION_ID="b7889dc6-971c-455e-b488-387d0f8a820f"
TENANT_ID="e6a916c6-9ab1-40d0-b5e4-07208617ed9e"
RG="tl-rg-dev"
WEBAPP_NAME="tl-dev-api"
APP_DISPLAY_NAME="tearlogic-github-dev-oidc"

echo "== Using OWNER=$OWNER REPO=$REPO =="
echo "== Target Web App: $WEBAPP_NAME in $RG =="

# ---- Login & select subscription ----
echo "Setting Azure subscription..."
az account set --subscription "$SUBSCRIPTION_ID"

# ---- Create Entra App ----
echo "Creating Entra application..."
APP_ID=$(az ad app create \
  --display-name "$APP_DISPLAY_NAME" \
  --query appId -o tsv)
echo "APP_ID=$APP_ID"

# ---- Create the Service Principal ----
echo "Creating service principal..."
SP_OBJECT_ID=$(az ad sp create --id "$APP_ID" --query id -o tsv)
echo "SP_OBJECT_ID=$SP_OBJECT_ID"

# ---- Add federated credential for GitHub Actions: main branch ----
echo "Adding federated credential for GitHub OIDC..."

az ad app federated-credential create \
  --id "$APP_ID" \
  --parameters "{
    \"name\": \"github-main\",
    \"issuer\": \"https://token.actions.githubusercontent.com\",
    \"subject\": \"repo:${OWNER}/${REPO}:ref:refs/heads/main\",
    \"audiences\": [\"api://AzureADTokenExchange\"]
  }"
echo "Federated credential created."

# ---- Assign minimal role to the Web App ----
echo "Fetching Web App resource ID..."
WEBAPP_ID=$(az webapp show -g "$RG" -n "$WEBAPP_NAME" --query id -o tsv)
echo "WEBAPP_ID=$WEBAPP_ID"

echo "Assigning role Website Contributor..."
az role assignment create \
  --assignee-object-id "$SP_OBJECT_ID" \
  --assignee-principal-type ServicePrincipal \
  --role "Website Contributor" \
  --scope "$WEBAPP_ID"

echo "Role assignment complete."

# ---- Output GitHub Variables ----
echo ""
echo "==============================="
echo "GitHub OIDC Values for Repo"
echo "==============================="
echo "AZURE_TENANT_ID=$TENANT_ID"
echo "AZURE_SUBSCRIPTION_ID=$SUBSCRIPTION_ID"
echo "AZURE_CLIENT_ID=$APP_ID"
echo "==============================="
