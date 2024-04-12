using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Me.SendMail;
using Microsoft.Graph.Models;
using System;
namespace Graph_API_Calls_Practise
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            GraphClient graphClient = new GraphClient(Credentials.TENANT_ID, Credentials.CLIENT_ID, Credentials.CLIENT_SECRET);
            Menu menu = new Menu();
            InputHandler inputHandler = new InputHandler();

            int defaultMenuOptionChosen, apiMenuOptionChosen;

            do
            {
                defaultMenuOptionChosen = menu.DisplayMenu(menu.DefaultMenu);
                switch (defaultMenuOptionChosen)
                {
                    // GET
                    case 1:
                        apiMenuOptionChosen = menu.DisplayMenu(menu.GetMenu);
                        switch (apiMenuOptionChosen)
                        {
                            case 1:
                                var user = await graphClient.GetUserAsync(inputHandler.InputUserId());
                                await Console.Out.WriteLineAsync(user.DisplayName);
                                break;
                            case 2:
                                var guestUsers = await graphClient.GetGuestUsersAsync();
                                graphClient.DisplayGuestUsers(guestUsers);
                                break;
                            case 3:
                                var group = await graphClient.GetGroupAsync(inputHandler.InputGroupId());
                                await Console.Out.WriteLineAsync(group.DisplayName);
                                break;
                            case 4:
                                var groupMembers = await graphClient.GetGroupMembersAsync(inputHandler.InputGroupId());
                                graphClient.DisplayGroupMembers(groupMembers);
                                break;
                        }
                        break;
                    // POST
                    case 2:
                        apiMenuOptionChosen = menu.DisplayMenu(menu.PostMenu);
                        switch (apiMenuOptionChosen)
                        {
                            case 1:
                                await graphClient.SendMailAsync(inputHandler.InputUserId(), inputHandler.InputRecipientEmail());
                                break;
                            case 2:
                                await graphClient.AddUserToGroupAsync(inputHandler.InputUserId(), inputHandler.InputGroupId());
                                break;
                        }
                        break;
                    // PATCH
                    case 3:
                        apiMenuOptionChosen = menu.DisplayMenu(menu.PatchMenu);
                        switch (apiMenuOptionChosen)
                        {
                            case 1:
                                await graphClient.UpdateUserAsync(inputHandler.InputUserId());
                                break;
                            case 2:
                                break;
                        }
                        break;
                    // DELETE
                    case 4:
                        apiMenuOptionChosen = menu.DisplayMenu(menu.DeleteMenu);
                        switch (apiMenuOptionChosen)
                        {
                            case 1:
                                await graphClient.DeleteUserAsync(inputHandler.InputUserId());
                                break;
                            case 2:
                                await graphClient.RemoveMemberFromGroupAsync(inputHandler.InputUserId(), inputHandler.InputGroupId());
                                break;
                        }
                        break;
                    case 5:
                        await Console.Out.WriteLineAsync("Exiting...");
                        break;
                }
            } while (defaultMenuOptionChosen != 5);

            //InteractiveBrowserCredentials
            /*var me = await graphClient.Me.GetAsync();
            Console.WriteLine($"Hello {me?.DisplayName}!");*/
        }
    }
}
