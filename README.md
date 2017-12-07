---
services: media-services
platforms: dotnet
author: ashivadas
---

# Encode and Deliver Content on Demand with Azure Media Services using .NET SDK

The code shows how to upload, encode media, protect, add a subtitle file and publish an asset in Azure media services.

This is used in conjuction with a Azure Media functions deployment from 

https://github.com/ashivadas/azamsfunctions

## Running this sample

1. Install Visual Studio 2017. 

2. Deploy the Azure functions from: https://github.com/ashivadas/azamsfunctions

3. Use Nuget to install the latest Azure Media Services .NET SDK.
	
	[Install-Package windowsazure.mediaservices.extensions](http://www.nuget.org/packages/windowsazure.mediaservices.extensions).

4. Update the appSettings section of the app.config file with appropriate values. For more information, see [this](https://docs.microsoft.com/azure/media-services/media-services-use-aad-auth-to-access-ams-api) topic.

		<?xml version="1.0"?>
		<configuration>
		  <appSettings>
			    <add key="AMSAADTenantDomain" value="AADTenantDomain" />
			    <add key="AMSRESTAPIEndpoint" value="RESTAPIEndpoint" />
		  </appSettings>
		</configuration>

## Other helpful links

https://docs.microsoft.com/en-us/azure/media-services/media-services-portal-get-started-with-aad

https://www.youtube.com/watch?v=IB4RsKAW0pM


