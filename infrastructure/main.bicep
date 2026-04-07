targetScope = 'resourceGroup'

@description('Azure region for the resources. Defaults to the resource group location.')
param location string = resourceGroup().location

@description('Name of the App Service plan.')
param appServicePlanName string

@description('Name of the Azure Web App.')
param webAppName string

@description('SKU name for the App Service plan.')
param appServicePlanSkuName string

@description('SKU tier for the App Service plan.')
param appServicePlanSkuTier string

@description('Whether Always On should be enabled for the web app.')
param alwaysOn bool = false

@description('Additional app settings to apply to the web app.')
param appSettings object = {
  ASPNETCORE_ENVIRONMENT: 'Debug'
  WEBSITE_RUN_FROM_PACKAGE: '1'
}

@description('Tags applied to all provisioned resources.')
param tags object = {}

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: {
    name: appServicePlanSkuName
    tier: appServicePlanSkuTier
  }
  kind: 'app'
  properties: {
    reserved: false
  }
}

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: webAppName
  location: location
  tags: tags
  kind: 'app'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      alwaysOn: alwaysOn
      appSettings: [for setting in items(appSettings): {
        name: setting.key
        value: string(setting.value)
      }]
    }
  }
}

output appServicePlanId string = appServicePlan.id
output webAppId string = webApp.id
output webAppDefaultHostName string = webApp.properties.defaultHostName
