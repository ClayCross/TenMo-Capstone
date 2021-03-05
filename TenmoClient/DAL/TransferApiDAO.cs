using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient.DAL
{
    public class TransferApiDAO : ITransferDAO
    {
        IRestClient client;
        public TransferApiDAO(string baseApiUrl)
        {
            this.client = new RestClient(baseApiUrl);
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
        }

        public bool CreateTransfer(Transfer transfer)
        {
            RestRequest request = new RestRequest("/transfers");
            request.AddJsonBody(transfer);
            IRestResponse response = client.Post(request);
            CheckResponse(response);

            if ((int)response.StatusCode == 200)
            {
                return true;
            }

            else
            {
                return false;
            }

        }

        public List<Transfer> GetTransfersByUser(int id)
        {
            RestRequest request = new RestRequest($"/users/{id}/transfers");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            CheckResponse(response);

            return response.Data;
        }

        private static void CheckResponse(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error - unable to reach the server.");
            }
            if (!response.IsSuccessful)
            {
                throw new Exception($"Error - server return error response {response.StatusCode} ({(int)response.StatusCode}) ");
            }
        }



    }
}
