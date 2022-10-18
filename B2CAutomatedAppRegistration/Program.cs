
using Azure.Identity;
using B2CAutomatedAppRegistration;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;



var config = new ConfigurationBuilder()
    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
    .AddJsonFile("config.json", optional: false)
    .Build();

var configSecrets = new ConfigurationBuilder()
    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
    .AddJsonFile("config.secret.json", optional: false)
    .Build();

string publisherAppName = config["publisherAppName"]; 
string tenantId = config["tenantId"];
string clientId = config["clientId"];
string clientSecret = configSecrets["clientSecret"];

string consumerAppPrefix = "consumer-app-03";

var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

var graphClient = new GraphServiceClient(clientSecretCredential);

B2CAppRegistrationSample appRegistrationSample = new B2CAppRegistrationSample(graphClient);

await appRegistrationSample.CreateSampleApp(publisherAppName, consumerAppPrefix).ConfigureAwait(false);
await appRegistrationSample.DeleteAllAppsWithPrefix(consumerAppPrefix);
