param cosmosDbAccountName string
param location string = resourceGroup().location
param databaseName string = 'ProductsDatabase'
param containerName string = 'ProductsContainer'

resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: cosmosDbAccountName
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        location: location
        failoverPriority: 0
      }
    ]
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-03-15' = {
  parent: cosmosDb
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
  }
}

resource container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-03-15' = {
  parent: database
  name: containerName
  properties: {
    resource: {
      id: containerName
      partitionKey: {
        paths: [
          '/categoryId'
        ]
        kind: 'Hash'
      }
    }
  }
}

output cosmosDbEndpoint string = cosmosDb.properties.documentEndpoint
output cosmosDbPrimaryKey string = listKeys(cosmosDb.id, '2021-03-15').primaryMasterKey