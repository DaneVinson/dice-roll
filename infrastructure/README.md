# Infrastructure

This folder contains a minimal Bicep setup for provisioning the Azure resources needed to host the app in Azure App Service.

## Resources

- App Service plan
- Azure Web App
- Basic app settings for zip/package deployment

## Files

- `main.bicep`: reusable resource definition
- `dev.bicepparam`: example development parameters

## Prerequisites

- Azure CLI installed
- An existing Azure subscription
- A resource group to deploy into

## Example commands

Create a resource group if needed:

```powershell
az group create --name dice-roll-dev-rg --location eastus
```

Deploy the Bicep template into that resource group:

```powershell
az deployment group create --resource-group dice-roll-dev-rg --template-file infrastructure/main.bicep --parameters infrastructure/dev.bicepparam
```

## Notes

- The template is set up for a Windows App Service so it matches the current pipeline `webApp` deployment type.
- Replace the names in `dev.bicepparam` with globally unique values before deployment.
- After provisioning, update `azure-pipelines.yml` so `azureServiceConnection` and `webAppName` match your Azure resources.
