using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_API_Calls_Practise
{
    internal class Menu
    {
        private List<string> _defaultMenu;
        private List<string> _getMenu;
        private List<string> _postMenu;
        private List<string> _patchMenu;
        private List<string> _deleteMenu;

        public Menu()
        {
            _defaultMenu = new List<string>() { "REST API Requests Menu", "GET", "POST", "PUT", "DELETE", "EXIT" };
            _getMenu = new List<string>() {"GET Request Menu", "Get user", "Get guest users", "Get group" };
            _postMenu = new List<string>() { "POST Request Menu", "Send mail", "Add member to a group" };
            _patchMenu = new List<string>() { "PATCH Request Menu" ,"Update user"};
            _deleteMenu = new List<string>() { "DELETE Request Menu", "Delete user" , "Remove member from group" };
        }

        public int DisplayMenu(List<string> menuOptions)
        {
            int optionChosen;
            Console.WriteLine($"{menuOptions[0]}");
            for (int i = 1; i < menuOptions.Count; i++)
            {
                Console.WriteLine($"{i}. {menuOptions[i]}");
            }

            do
            {
                Console.Write($"\nChoose option between 1 - {menuOptions.Count - 1} : ");
            } while (!int.TryParse(Console.ReadLine(), out optionChosen) || (optionChosen <= 0 || optionChosen >= menuOptions.Count));

            return optionChosen;
        }

        public  List<string> DefaultMenu { get => _defaultMenu;}
        public  List<string> GetMenu { get => _getMenu; }
        public List<string> PostMenu { get => _postMenu; }
        public List<string> PatchMenu { get => _patchMenu; }
        public List<string> DeleteMenu { get => _deleteMenu; }
    }

}
