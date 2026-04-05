using './main.bicep'

param appServicePlanName = 'dice-roll-plan-dev'
param webAppName = 'dice-roll-web-dev'
param appServicePlanSkuName = 'B1'
param appServicePlanSkuTier = 'Basic'
param alwaysOn = true
param appSettings = {
  ASPNETCORE_ENVIRONMENT: 'Production'
  WEBSITE_RUN_FROM_PACKAGE: '1'
}
param tags = {
  app: 'dice-roll'
  environment: 'dev'
}
