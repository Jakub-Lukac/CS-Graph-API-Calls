using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;
using Azure.Identity;

namespace Graph_API_Calls_Practise
{
    internal class GraphClient
    {
        private GraphServiceClient _graphServiceClient;

        public GraphClient(string tenantId, string clientId, string clientSecret)
        {
            _graphServiceClient = CreateGraphClient(tenantId, clientId, clientSecret);  
        }

        private GraphServiceClient CreateGraphClient(string tenantId, string clientId, string clientSecret)
        {
            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var options = new ClientSecretCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };

            var clientSecretCredentials = new ClientSecretCredential(tenantId, clientId, clientSecret);

            return new GraphServiceClient(clientSecretCredentials, scopes);
        }
    }
}
