using MenuFramework;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.DAL;
using TenmoClient.Data;
using System.Linq;
using static TenmoClient.Data.Enums;

namespace TenmoClient.Views
{
    public class MainMenu : ConsoleMenu
    {
        private IAccountDAO accountDAO;
        private ITransferDAO transferDAO;

        public MainMenu(string apiBaseUrl)
        {
            this.accountDAO = new AccountApiDAO(apiBaseUrl);
            this.transferDAO = new TransferApiDAO(apiBaseUrl);

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

            Console.WriteLine($"Your current balance is: {account.Balance:c}");

            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult ViewTransfers()
        {
            List<Transfer> transfers = transferDAO.GetTransfersByUser(UserService.GetUserId());

            Console.WriteLine("Transfers");
            Console.WriteLine($"{"ID", -10}{"From/To", -19}{"Amount", -10}\n");

            foreach (Transfer transfer in transfers)
            {
                if (transfer.AccountTo == UserService.GetUserId())
                {
                    Console.WriteLine($"{transfer.TransferId,-10}From: {transfer.UserNameFrom,-13}{transfer.Amount, -10:c}");
                }
                else
                {
                    Console.WriteLine($"{transfer.TransferId,-10}To: {transfer.UserNameTo,-15}{transfer.Amount, -10:c}");
                }
            }

            Console.WriteLine();

            bool validId = false;
           Transfer selectedTransfer = null;

            while (!validId)
            {

                int transferId = GetInteger("Please enter the ID to view details (0 to cancel): ");

                if (transferId == 0)
                {
                    return MenuOptionResult.DoNotWaitAfterMenuSelection;
                }
                
                selectedTransfer = transfers.Find(t => t.TransferId == transferId);
                
                if (selectedTransfer != null)
                {
                    break;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("I'm sorry that was not a valid ID.");
                }
            }

            Console.Clear();
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Transfer Details");
            Console.WriteLine("-----------------------------------------\n");
            Console.WriteLine($"Id: {selectedTransfer.TransferId}");
            Console.WriteLine($"From: {selectedTransfer.UserNameFrom}");
            Console.WriteLine($"To: {selectedTransfer.UserNameTo}");
            Console.WriteLine($"Type: {selectedTransfer.TransferType}");
            Console.WriteLine($"Status: {selectedTransfer.TransferStatus}");
            Console.WriteLine($"Amount: {selectedTransfer.Amount:c}");

            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult ViewRequests()
        {
            Console.WriteLine("Not implemented");

            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult SendTEBucks()
        {
            List<DisplayAccount> accounts = accountDAO.GetAllAccounts();
            accounts = accounts.Where(a => a.AccountId != UserService.GetUserId()).ToList();

            foreach (DisplayAccount account in accounts)
            {

                Console.WriteLine($"{account.AccountId} {account.Username}");
            }

            bool validId = false;
            int accountTo = -1;

            while (!validId)
            {
                accountTo = GetInteger("Enter ID of user you are sending to (0 to cancel): ");

                if (accountTo == 0)
                {
                    return MenuOptionResult.DoNotWaitAfterMenuSelection;
                }
                DisplayAccount verifiedAccount = accounts.Find(a => a.AccountId == accountTo);
                if (verifiedAccount != null)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Please provide a valid ID.");
                }

            }
           
            decimal moneyToSend = GetDecimal("Enter amount: ");
            Account userAccount = accountDAO.GetAccountByUserId(UserService.GetUserId());
            if(moneyToSend > userAccount.Balance)
            {
                Console.WriteLine("I'm sorry you don't have enough finds for the transfer");
                return MenuOptionResult.WaitAfterMenuSelection;
            }
            Transfer transfer = new Transfer();
            transfer.TransferStatus = TransferStatus.Approved;
            transfer.TransferType = TransferType.Send;
            transfer.AccountFrom = userAccount.AccountId;
            transfer.AccountTo = accountTo;
            transfer.Amount = moneyToSend;
            DisplayAccount toAccount = accounts.Find(a => a.AccountId == accountTo);
            transfer.UserNameTo = toAccount.Username;
            transfer.UserNameFrom = UserService.GetUserName();

            bool wasTransferred = transferDAO.CreateTransfer(transfer);

            if (wasTransferred)
            {
                Console.WriteLine("Successful transfer!");
            }
            else
            {
                Console.WriteLine("Transfer failed.");
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
