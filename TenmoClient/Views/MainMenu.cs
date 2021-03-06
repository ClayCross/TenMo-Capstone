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
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Transfers");
            Console.WriteLine($"{"ID", -10}{"From/To", -19}{"Amount", -10}\n");
            Console.WriteLine("-----------------------------------------");


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
            List<Transfer> transfers = transferDAO.GetPendingByUser(UserService.GetUserId());

            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Pending Transfers");
            Console.WriteLine($"{"ID",-10}{"To",-19}{"Amount",-10}\n");
            Console.WriteLine("-----------------------------------------");


            foreach (Transfer transfer in transfers)
            {
                Console.WriteLine($"{transfer.TransferId,-10}{transfer.UserNameTo,-19}{transfer.Amount,-10:c}");
            }

            int transferId = GetInteger("\nPlease enter transfer ID to approve/reject (0 to cancel): ");
            if (transferId == 0)
            {
                return MenuOptionResult.DoNotWaitAfterMenuSelection;
            }

            Transfer selectedTransfer = transfers.Find(t => t.TransferId == transferId);
            Console.Clear();
            Console.WriteLine("1: Approve");
            Console.WriteLine("2: Reject");
            Console.WriteLine("0: Don't approve or reject");

            int action = GetInteger("Please choose an option: ");

            if (action == 1)
            {
                Account userAccount = accountDAO.GetAccountByUserId(UserService.GetUserId());
                if (selectedTransfer.Amount > userAccount.Balance)
                {
                    Console.WriteLine("You do not have enough funds to complete the transfer.");
                    return MenuOptionResult.WaitAfterMenuSelection;
                }

                selectedTransfer.TransferStatus = TransferStatus.Approved;

                bool wasTransferred = transferDAO.UpdateTransfer(selectedTransfer);

                if (wasTransferred)
                {
                    Console.WriteLine("Successful transfer!");
                }
                else
                {
                    Console.WriteLine("Transfer failed.");
                }

            }
            else if (action == 2)
            {
                selectedTransfer.TransferStatus = TransferStatus.Rejected;

                bool wasTransferred = transferDAO.UpdateTransfer(selectedTransfer);

                if (wasTransferred)
                {
                    Console.WriteLine("Transfer rejection complete!");
                }
                else
                {
                    Console.WriteLine("Transfer rejection failed.");
                }

            }
            else
            {
                return MenuOptionResult.DoNotWaitAfterMenuSelection;
            }


            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult SendTEBucks()
        {
            List<DisplayAccount> accounts = accountDAO.GetAllAccounts();
            accounts = accounts.Where(a => a.AccountId != UserService.GetUserId()).ToList();

            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"Users");
            Console.WriteLine($"{"ID", -11}{"Name", -10}");
            Console.WriteLine("-----------------------------------------");

            foreach (DisplayAccount account in accounts)
            {

                Console.WriteLine($"{account.AccountId, -10} {account.Username, -10}");
            }

            bool validId = false;
            int accountToId = -1;

            while (!validId)
            {
                accountToId = GetInteger("Enter ID of user you are sending to (0 to cancel): ");

                if (accountToId == 0)
                {
                    return MenuOptionResult.DoNotWaitAfterMenuSelection;
                }
                DisplayAccount verifiedAccount = accounts.Find(a => a.AccountId == accountToId);
                if (verifiedAccount != null)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Please provide a valid ID.");
                }

            }
           
            decimal transferAmount = GetDecimal("Enter amount: ");
            Account userAccount = accountDAO.GetAccountByUserId(UserService.GetUserId());

            if(transferAmount > userAccount.Balance)
            {
                Console.WriteLine("I'm sorry you don't have enough funds for the transfer");
                return MenuOptionResult.WaitAfterMenuSelection;
            }
            DisplayAccount toAccount = accounts.Find(a => a.AccountId == accountToId);
            Transfer transfer = new Transfer();
            transfer.TransferStatus = TransferStatus.Approved;
            transfer.TransferType = TransferType.Send;
            transfer.AccountFrom = userAccount.AccountId;
            transfer.AccountTo = accountToId;
            transfer.Amount = transferAmount;
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
            List<DisplayAccount> accounts = accountDAO.GetAllAccounts();
            accounts = accounts.Where(a => a.AccountId != UserService.GetUserId()).ToList();

            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"Users");
            Console.WriteLine($"{"ID",-11}{"Name",-10}");
            Console.WriteLine("-----------------------------------------");

            foreach (DisplayAccount account in accounts)
            {

                Console.WriteLine($"{account.AccountId, -10} {account.Username, -10}");
            }

            bool validId = false;
            int fromAccountId = -1;

            while (!validId)
            {
                fromAccountId = GetInteger("Enter ID of user you are requesting from (0 to cancel): ");

                if (fromAccountId == 0)
                {
                    return MenuOptionResult.DoNotWaitAfterMenuSelection;
                }
                DisplayAccount verifiedAccount = accounts.Find(a => a.AccountId == fromAccountId);
                if (verifiedAccount != null)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Please provide a valid ID.");
                }

            }
            decimal requestAmount = GetDecimal("Enter Amount");
            DisplayAccount accountFrom = accounts.Find(a => a.AccountId == fromAccountId);

            Transfer transfer = new Transfer();
            transfer.TransferStatus = TransferStatus.Pending;
            transfer.TransferType = TransferType.Request;
            transfer.AccountFrom = accountFrom.AccountId;
            transfer.AccountTo = UserService.GetUserId();
            transfer.Amount = requestAmount;
            transfer.UserNameTo = UserService.GetUserName();
            transfer.UserNameFrom = accountFrom.Username;

            bool wasRequested = transferDAO.CreateTransferRequest(transfer);

            if (wasRequested)
            {
                Console.WriteLine("Successfully requested transfer!");
            }
            else
            {
                Console.WriteLine("Transfer request failed.");
            }

            return MenuOptionResult.WaitAfterMenuSelection;
        }



        private MenuOptionResult Logout()
        {
            UserService.SetLogin(new API_User()); //wipe out previous login info
            return MenuOptionResult.CloseMenuAfterSelection;
        }

    }
}
