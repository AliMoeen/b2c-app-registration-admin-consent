using Microsoft.Graph;
using System.Text.RegularExpressions;

namespace B2CAutomatedAppRegistration
{
    internal class B2CAppRegistrationSample
    {
        private readonly GraphServiceClient _client;
        private readonly GraphHelper _graphHelper;

        public B2CAppRegistrationSample(GraphServiceClient client)
        {
            _client = client;
            _graphHelper = new GraphHelper(_client);
        }
        public async Task CreateSampleApp(string publisherAppName, string consumerAppPrefix)
        {
            var publisherApp = await _graphHelper.GetAppByName(publisherAppName);
            var publisherAppId = publisherApp.AppId;
            Console.WriteLine($"publisherAppId: {publisherAppId}");


            var publisherPrincipal = await _graphHelper.GetServicePrincipalByAppId(publisherApp.AppId);
            var publisherPrincipalId = publisherPrincipal.Id;
            Console.WriteLine($"publisherPrincipalId: {publisherPrincipalId}");

            string shortGuid = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
            string serial = DateTime.Now.ToString("MMddHHmmss");
            string appDisplayName = $"{consumerAppPrefix}-{serial}-{shortGuid}";
            var consumerAppTemplate = _graphHelper.GetAppslicationTemplate(appDisplayName);
            Console.WriteLine($"New app name: {consumerAppTemplate.DisplayName}");
            var newApp = await _client.Applications.Request().AddAsync(consumerAppTemplate);

            var consumerObjectId = newApp.Id;
            var consumerAppId = newApp.AppId;
            Console.WriteLine($"consumerObjectId : {consumerObjectId}");
            Console.WriteLine($"consumerAppId : {consumerAppId}");

            ServicePrincipal sp = new ServicePrincipal
            {
                AppId = consumerAppId
            };

            var newServicePrincipal = await _client.ServicePrincipals.Request().AddAsync(sp);
            var consumerAppPrincipalId = newServicePrincipal.Id;
            Console.WriteLine($"consumerAppPrincipalId : {consumerAppPrincipalId}");

            OAuth2PermissionGrant permissionGrant = new OAuth2PermissionGrant
            {
                ClientId = consumerAppPrincipalId,
                ConsentType = "AllPrincipals",
                ResourceId = publisherPrincipalId,
                Scope = "my-favorate-scope-65536"
            };

            var newOauth2PermissionGrant = await _client.Oauth2PermissionGrants.Request().AddAsync(permissionGrant);
            Console.WriteLine($"newOauth2PermissionGrant : {newOauth2PermissionGrant}");

            var passwordCredentialTemplate = new PasswordCredential
            {
                DisplayName = "Auto Created",
                EndDateTime = new DateTimeOffset(DateTime.Now.AddMonths(12))

            };

            var passwordCredential = await _client.Applications[consumerObjectId].AddPassword(passwordCredentialTemplate).Request().PostAsync().ConfigureAwait(false);
            Console.WriteLine($"New app client secret : {passwordCredential.SecretText}");
        }

        public async Task DeleteAllAppsWithPrefix(string prefix)
        {
            var apps2Delete = await _client.Applications.Request().Filter($"startswith(displayName, '{prefix}')").GetAsync().ConfigureAwait(false);
            Console.WriteLine($"Apps found: {apps2Delete.Count}");
            foreach(var app in apps2Delete)
            {
                await _client.Applications[app.Id].Request().DeleteAsync();
                Console.WriteLine($"Deleted application: {app.DisplayName}");
            }
        }
    }



}
