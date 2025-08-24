param apiManagementName string
param location string
param publisherEmail string
param publisherName string

resource apiManagement 'Microsoft.ApiManagement/service@2021-04-01' = {
  name: apiManagementName
  location: location
  sku: {
    name: 'Developer'
    capacity: 1
  }
  properties: {
    publisherEmail: publisherEmail
    publisherName: publisherName
    notificationSenderEmail: publisherEmail
  }
}

output apiManagementId string = apiManagement.id