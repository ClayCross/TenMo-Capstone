using System;
using System.Collections.Generic;
using TenmoClient.Data;
using TenmoClient.Views;

namespace TenmoClient
{
    class Program
    {

        static void Main(string[] args)
        {
            string apiBaseUrl = "https://localhost:44315";

            AuthService authService = new AuthService();
            new LoginRegisterMenu(authService, apiBaseUrl).Show();

            Console.WriteLine("\r\nThank you for using TEnmo!!!\r\n");
        }
    }
}
