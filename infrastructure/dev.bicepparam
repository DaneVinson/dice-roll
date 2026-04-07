using './main.bicep'

param appServicePlanName = 'yadr-plane'
param webAppName = 'yadr'
param appServicePlanSkuName = 'F1'
param appServicePlanSkuTier = 'Free'
param alwaysOn = false
param appSettings = {
  ASPNETCORE_ENVIRONMENT: 'Debug'
  WEBSITE_RUN_FROM_PACKAGE: '1'
}
param tags = {
  app: 'dice-roll'
  environment: 'dev'
}
