using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient.DAL
{
    public class AccountApiDAO : IAccountDAO
    {
        private RestClient client;
        public AccountApiDAO(string baseApiUrl)
        {
            this.client = new RestClient(baseApiUrl);
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
        }

        public Account GetAccountByUserId(int userId)
        {
            RestRequest request = new RestRequest($"/users/{userId}");
            IRestResponse<Account> response = client.Get<Account>(request);
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
