using Microsoft.Graph;
using static System.Formats.Asn1.AsnWriter;

namespace B2CAutomatedAppRegistration
{
    internal class GraphHelper
    {

        private readonly GraphServiceClient _client;
        public GraphHelper(GraphServiceClient client)
        {
            _client = client;
        }

        public async Task<Application> GetAppByName(string name)
        {
            var apps = await _client.Applications.Request().Filter($"displayName eq  '{name}'").GetAsync().ConfigureAwait(false);
            if (apps.Count != 1)
            {
                    throw new Exception($"Cannot find the application with displayName '{name}'");
            }
            return apps[0];
        }
        public async Task<Application> GetAppByID(string appId)
        {
            var apps = await _client.Applications.Request().Filter($"appId eq  '{appId}'").GetAsync().ConfigureAwait(false);
            if (apps.Count != 1)
            {
                throw new Exception($"Cannot find the application with appId '{appId}'");
            }
            return apps[0];
        }

        public async Task<ServicePrincipal> GetServicePrincipalByAppId(string appId)
        {
            var sp = await _client.ServicePrincipals.Request().Filter($"appId eq  '{appId}'").GetAsync().ConfigureAwait(false);
            if (sp.Count != 1)
            {
                throw new Exception($"Cannot find the ServicePrincipal with appId '{appId}'");
            }
            return sp[0];
        }

        public Application GetAppslicationTemplate(string displayName)
        {
            var newApp = new Application
            {
                DisplayName = displayName,
                IsFallbackPublicClient = false,
                SignInAudience = "AzureADandPersonalMicrosoftAccount",
                RequiredResourceAccess = new List<RequiredResourceAccess>()
                {
                    new RequiredResourceAccess 
                    {
                        ResourceAppId = "2ce112fc-06c9-49e5-b726-59f09fac29cf",
                        ResourceAccess = new ResourceAccess[]
                        {
                            new ResourceAccess
                            {
                                Id = new Guid("735b0efa-b608-4d66-a389-15a2d2f25abb"),
                                Type = "Scope"
                            }
                        }
                    },
                    new RequiredResourceAccess // The Guids under this node are reserved Microsoft Graph
                    {
                        ResourceAppId = "00000003-0000-0000-c000-000000000000",
                        ResourceAccess = new ResourceAccess[]
                        {
                            new ResourceAccess
                            {
                                Id = new Guid("e1fe6dd8-ba31-4d61-89e7-88639da4683d"),
                                Type = "Scope"
                            }
                        }
                    }
                }
            };
            return newApp;
        }

    }
}
