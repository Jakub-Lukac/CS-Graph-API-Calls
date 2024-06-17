using System;
using Microsoft.Graph.Beta;
namespace Graph_Beta
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            GraphClient graphClient = new GraphClient(Credentials.TENANT_ID, Credentials.CLIENT_ID, Credentials.CLIENT_SECRET);
            var r = await graphClient.RBAC();
            graphClient.Display(r);
        }
    }
}
