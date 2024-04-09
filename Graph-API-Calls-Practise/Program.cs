using System;
namespace Graph_API_Calls_Practise
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            GraphClient graphClient = new GraphClient(Credentials.TENANT_ID, Credentials.CLIENT_ID, Credentials.CLIENT_SECRET);
        }
    }
}
