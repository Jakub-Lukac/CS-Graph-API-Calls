using System;
using Microsoft.Graph;
using Microsoft.Graph.Me.SendMail;
using Microsoft.Graph.Users.Item.SendMail;
using Microsoft.Graph.Models; // User, Groups
using Azure.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions;
using System.Diagnostics;
using System.Threading.Channels;


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

        public async Task<User> GetUserAsync(string userId)
        {
            try
            {
                var result =  await _graphServiceClient.Users[userId].GetAsync();
                await Console.Out.WriteLineAsync("User successfully retrieved.");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error user: {ex.Message}");
                throw;
            }
        }

        // GET 
        public async Task<List<User>> GetGuestUsersAsync()
        {
            try
            {
                var result = await _graphServiceClient.Users.GetAsync((requestConfiguration) =>
                {
                    requestConfiguration.QueryParameters.Filter = "userType eq 'guest'";
                });
                await Console.Out.WriteLineAsync("List of guest user successfully retrieved.");
                return result.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error users: {ex.Message}");
                throw;
            }
        }
        public void DisplayGuestUsers(List<User> guests)
        {
            foreach (User u in guests)
            {
                Console.WriteLine(u.DisplayName);
            }
        }

        public async Task<Group> GetGroupAsync(string groupId)
        {
            Group group = null;

            try
            {
                if (string.IsNullOrEmpty(groupId))
                {
                    throw new ArgumentNullException("groupId");
                }

                group = await _graphServiceClient.Groups[groupId].GetAsync();
                await Console.Out.WriteLineAsync("Group successfully retrieved.");
                return group;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error groups: {ex.Message}"); ;
                throw;
            }
        }

        // POST
        public async Task SendMailAsync(string senderId, string recipientEmail)
        {
            var requestBody = new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
            {
                Message = new Message
                {
                    Subject = "Meet for lunch?",
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Text,
                        Content = "The new cafeteria is open.",
                    },
                    ToRecipients = new List<Recipient>
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = recipientEmail,
                    },
                },
            },
                    CcRecipients = new List<Recipient>
            {
          
            },
                },
                SaveToSentItems = false,
            };

            try
            {
                await _graphServiceClient.Users[senderId].SendMail.PostAsync(requestBody);
                Console.WriteLine($"Email from {senderId} to {recipientEmail} was successfully sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error mail: {ex.Message}");
                throw;
            }
        }

        public async Task AddUserToGroupAsync(string userId, string groupId)
        {
            try
            {
                var requestBody = new ReferenceCreate
                {
                    OdataId = $"https://graph.microsoft.com/v1.0/directoryObjects/{userId}",
                };

                await _graphServiceClient.Groups[groupId].Members.Ref.PostAsync(requestBody);
                Console.WriteLine($"User {userId} was successfully added to group {groupId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user to a group: {ex.Message}");
                throw;
            }
        }

        // PATCH 
        public async Task UpdateUserAsync(string userId)
        {
            try
            {
                var requestBody = new User
                {
                    City = "Bystrica",
                };

                await _graphServiceClient.Users[userId].PatchAsync(requestBody);
                await Console.Out.WriteLineAsync($"User {userId} was successfully updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                throw;
            }
        }

        // DELETE 
        public async Task DeleteUserAsync(string userId)
        {
            try
            {
                await _graphServiceClient.Users[userId].DeleteAsync();
                await Console.Out.WriteLineAsync($"User {userId} was successfully deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                throw;
            }
        }
        public async Task RemoveMemberFromGroupAsync(string userId, string groupId)
        {
            try
            {
                await _graphServiceClient.Groups[groupId].Members[userId].Ref.DeleteAsync();
                await Console.Out.WriteLineAsync($"User {userId} was successfully deleted from group {groupId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing user: {ex.Message} from group.");
                throw;
            }
        }
    }
}
