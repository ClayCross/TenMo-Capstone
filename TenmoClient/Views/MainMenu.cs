﻿using MenuFramework;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.DAL;
using TenmoClient.Data;

namespace TenmoClient.Views
{
    public class MainMenu : ConsoleMenu
    {
        private IAccountDAO accountDAO;

        public MainMenu(string apiBaseUrl)
        {
            this.accountDAO = new AccountApiDAO(apiBaseUrl);

            AddOption("View your current balance", ViewBalance)
                .AddOption("View your past transfers", ViewTransfers)
                .AddOption("View your pending requests", ViewRequests)
                .AddOption("Send TE bucks", SendTEBucks)
                .AddOption("Request TE bucks", RequestTEBucks)
                .AddOption("Log in as different user", Logout)
                .AddOption("Exit", Exit);
        }

        protected override void OnBeforeShow()
        {
            Console.WriteLine($"TE Account Menu for User: {UserService.GetUserName()}");
        }

        private MenuOptionResult ViewBalance()
        {
            Account account = accountDAO.GetAccountByUserId(UserService.GetUserId());

            Console.WriteLine($"Your current balance is: {account.Balance: c}");

            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult ViewTransfers()
        {
            Console.WriteLine("Not yet implemented!");
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult ViewRequests()
        {
            Console.WriteLine("Not yet implemented!");
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult SendTEBucks()
        {
            List<DisplayAccount> accounts = accountDAO.GetAllAccounts();
            //TODO Take users account out of list
            foreach (DisplayAccount account in accounts)
            {

                Console.WriteLine($"{account.AccountId} {account.Username}");
            }

            decimal accountTo = GetDecimal("What account do you want to send money to?");
            decimal moneyToSend = GetDecimal("How much money would you like to send?");
            Account userAccount = accountDAO.GetAccountByUserId(UserService.GetUserId());
            if(moneyToSend > userAccount.Balance)
            {
                Console.WriteLine("I'm sorry you don't have enough finds for the transfer");
                return MenuOptionResult.WaitAfterMenuSelection;
            }

            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult RequestTEBucks()
        {
            Console.WriteLine("Not yet implemented!");
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult Logout()
        {
            UserService.SetLogin(new API_User()); //wipe out previous login info
            return MenuOptionResult.CloseMenuAfterSelection;
        }

    }
}
