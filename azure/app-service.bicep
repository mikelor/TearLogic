@description('Specifies the Azure region where the resources should be deployed.')
param location string = resourceGroup().location

@description('Specifies the name of the App Service plan.')
param appServicePlanName string = 'tl-appservice-plan'

@description('Specifies the SKU name for the App Service plan.')
@allowed([
  'F1'
  'D1'
  'B1'
  'B2'
  'B3'
  'S1'
  'S2'
  'S3'
  'P1v2'
  'P2v2'
  'P1v3'
  'P2v3'
])
param appServicePlanSkuName string = 'B1'

@description('Defines the SKU tier for the App Service plan.')
@allowed([
  'Free'
  'Shared'
  'Basic'
  'Standard'
  'PremiumV2'
  'PremiumV3'
])
param appServicePlanSkuTier string = 'Basic'

@description('Configures the number of workers in the App Service plan.')
@minValue(1)
param appServicePlanCapacity int = 1

@description('Specifies the name of the App Service.')
param appServiceName string = 'tl-api-${uniqueString(resourceGroup().id)}'

@description('Defines the Linux runtime stack for the App Service.')
param linuxFxVersion string = 'DOTNETCORE|10.0'

@description('Optional health check path (e.g., /health). Leave empty to disable.')
param healthCheckPath string = ''

@description('Declares custom application settings for the App Service.')
param appSettings array = [
  {
    name: 'ASPNETCORE_ENVIRONMENT'
    value: 'Production'
  }
  // For self-contained/zip deploy best practice:
  {
    name: 'WEBSITE_RUN_FROM_PACKAGE'
    value: '1'
  }
  {
    name: 'SCM_DO_BUILD_DURING_DEPLOYMENT'
    value: 'false'
  }
]

@description('Applies key/value pair tags to all provisioned resources.')
param appServiceTags object = {
  Environment: 'Production'
  Workload: 'TearLogic'
}

var normalizedAppSettings = [
  for setting in appSettings: {
    name: setting.name
    value: setting.value
  }
]

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: appServicePlanSkuName
    tier: appServicePlanSkuTier
    capacity: appServicePlanCapacity
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
  tags: appServiceTags
}

resource appService 'Microsoft.Web/sites@2023-12-01' = {
  name: appServiceName
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: linuxFxVersion
      appSettings: normalizedAppSettings
      ftpsState: 'Disabled'
      alwaysOn: false
      http20Enabled: true
      minTlsVersion: '1.2'
      healthCheckPath: empty(healthCheckPath) ? null : healthCheckPath
    }
  }
  tags: appServiceTags
}

output webAppName string = appService.name
output webAppHostname string = appService.properties.defaultHostName
output appServicePlanId string = appServicePlan.id