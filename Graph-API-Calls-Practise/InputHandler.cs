using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_API_Calls_Practise
{
    internal class InputHandler
    {
        private readonly string INPUT_TABLE = "{0,-30}{1,1}";
       public string InputUserId()
        {
            // later maybe different logic for user id validation
            string id;

            do
            {
                Console.Write(INPUT_TABLE, "Enter user ID", ": ");
                id = Console.ReadLine().Trim();
            } while (string.IsNullOrEmpty(id));

            return id;
        }
        public string InputGroupId()
        {
            // later maybe different logic for group id validation
            string id;

            do
            {
                Console.Write(INPUT_TABLE, "Enter group ID", ": ");
                id = Console.ReadLine().Trim();
            } while (string.IsNullOrEmpty(id));

            return id;
        }
        public string InputRecipientEmail()
        {
            string email;

            do
            {
                Console.Write(INPUT_TABLE, "Enter recipient email", ": ");
                email = Console.ReadLine().Trim();
            } while (string.IsNullOrEmpty(email) && email.Contains("@"));

            return email;
        }
        public string InputMessage()
        {
            string message;

            do
            {
                Console.Write(INPUT_TABLE, "Enter your message", ": ");
                message = Console.ReadLine().Trim();
            } while (string.IsNullOrEmpty(message));

            return message;
        }
    }
}
