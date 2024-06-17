using System;
using Microsoft.Graph.Beta;
using Azure.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions;
using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Graph.Beta.Models;


namespace Graph_Beta
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
        public async Task<UnifiedRoleDefinitionCollectionResponse> RBAC()
        {
            try
            {
                // var result = await _graphServiceClient.RoleManagement.Exchange.RoleAssignments.GetAsync();
                var result = await _graphServiceClient.RoleManagement.Exchange.RoleDefinitions.GetAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error user: {ex.Message}");
                throw;
            }
        }

        public void Display(UnifiedRoleDefinitionCollectionResponse response)
        {
            foreach (var item in response.Value)
            {
                if (item.IsPrivileged == true)
                {
                    Console.WriteLine(item);
                }
                else
                {
                    Console.WriteLine("nope");
                }
            }
        }
    }
}
