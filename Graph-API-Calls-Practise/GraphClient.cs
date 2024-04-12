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

        public async Task<List<DirectoryObject>> GetGroupMembersAsync(string groupId)
        {
            List<DirectoryObject> users = new List<DirectoryObject>();
            try
            {
                var usersResult = await _graphServiceClient.Groups[groupId].Members.GetAsync((requestConfiguration) =>
                {
                    // will fetch at most 999 records (members)
                    requestConfiguration.QueryParameters.Top = 999;
                    // specifies which parameters to include in the response
                    requestConfiguration.QueryParameters.Select = new string[] { "id", "displayName" };
                });

                users = users.Union(usersResult.Value.Where(w => w.GetType() == typeof(User) || w.GetType() == typeof(Group)).OrderBy(o => o.Id).ToList()).ToList();
                // w.getType checks the @odota.type field, to only include objects of type User or Group
                // orderBy uses Id as parameter for ordering the objects
                // first toList converts sorted collection to a list
                // union for combining the filtered (sorted) list with the existing users collection
                // Union creates new collection which includes elements from both collections
                // second toList converts combined collection to a list
                // Union is used bcs it only includes unique values, so no duplicates

                // retrieves next page of the usersResult object
                var nextPageLink = usersResult.OdataNextLink;

                // while loop will be executed as long as there is some next page
                while (nextPageLink != null)
                {
                    var nextPageRequestInformation = new RequestInformation
                    {
                        HttpMethod = Method.GET,
                        UrlTemplate = nextPageLink,
                    };

                    // this is actual variable for storing the next page results (data) 
                    // nextPageResult will store collection of objects (response), each element is parsed into a DirectoryObjectCollectionResponse
                    var nextPageResult = await _graphServiceClient.RequestAdapter.SendAsync(nextPageRequestInformation, (parseNode) => new DirectoryObjectCollectionResponse());
                    // we use Union again but this time on nextPageResult
                    users = users.Union(nextPageResult.Value.Where(w => w.GetType() == typeof(User) || w.GetType() == typeof(Group)).OrderBy(o => o.Id).ToList()).ToList();
                    nextPageLink = nextPageResult.OdataNextLink;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error group members: {ex.Message}"); ;
                throw;
            }

            return users;
        }

        public void DisplayGroupMembers(List<DirectoryObject> users)
        {
            // directory object on default exposes the id, as it can be used for various collections
            // thats why on default in does not know what displayName is
            // as a result we need to convert object in collection to either User or Group instance to access its properties
            foreach (var user in users) 
            {
                if (user is User)
                {
                    User userObject = (User)user;
                    Console.WriteLine($"User Id: {userObject.Id}, Display Name: {userObject.DisplayName}");
                }
                else if (user is Group)
                {
                    Group groupObject = (Group)user;
                    Console.WriteLine($"Group Id: {groupObject.Id}, Display Name: {groupObject.DisplayName}");
                }
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
