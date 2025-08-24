@description('The Azure region for deploying resources')
param location string = resourceGroup().location

@description('Name of the API Management instance')
@minLength(3)
@maxLength(50)
param apiManagementName string

@description('Name of the Cosmos DB account')
@minLength(3)
@maxLength(44)
param cosmosDbAccountName string

@description('Name of the Function App')
@minLength(2)
param functionAppName string

@description('Name of the Key Vault')
@minLength(3)
@maxLength(24)
param keyVaultName string

@description('Name of the Storage Account')
@minLength(3)
@maxLength(24)
param storageAccountName string

@description('Name of the Application Insights instance')
param appInsightsName string

@description('Service Bus Connection String')
@secure()
param serviceBusConnectionString string

@description('Publisher email for API Management')
param publisherEmail string

@description('Publisher name for API Management')
param publisherName string

// Deploy Key Vault first since other resources will need secrets from it
module keyvault 'keyvault.bicep' = {
  name: 'keyvault-deployment'
  params: {
    location: location
    keyVaultName: keyVaultName
  }
}

// Deploy Cosmos DB with dependency on Key Vault
module cosmos 'cosmos.bicep' = {
  name: 'cosmosdb-deployment'
  params: {
    location: location
    cosmosDbAccountName: cosmosDbAccountName
  }
  dependsOn: [
    keyvault
  ]
}

// Deploy Function App with dependencies
module functions 'functions.bicep' = {
  name: 'functions-deployment'
  params: {
    location: location
    functionAppName: functionAppName
    storageAccountName: storageAccountName
    appInsightsName: appInsightsName
    serviceBusConnectionString: serviceBusConnectionString
  }
  dependsOn: [
    cosmos
    keyvault
  ]
}

// Deploy API Management last since it needs to integrate with Function App
module apiManagement 'apiManagement.bicep' = {
  name: 'apim-deployment'
  params: {
    location: location
    apiManagementName: apiManagementName
    publisherEmail: publisherEmail
    publisherName: publisherName
  }
  dependsOn: [
    functions
    keyvault
  ]
}

// Output the key resources for reference
output apiManagementUrl string = apiManagement.outputs.apiManagementId
output functionAppHostName string = functions.outputs.functionAppUrl
output cosmosDbEndpoint string = cosmos.outputs.cosmosDbEndpoint
output keyVaultUri string = keyvault.outputs.keyVaultUri
