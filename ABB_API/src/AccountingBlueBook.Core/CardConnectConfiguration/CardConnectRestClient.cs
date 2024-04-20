using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.CardConnectConfiguration
{
    public class CardConnectRestClient
    {

        private String url;
        private String userpass;
        private String username;
        private String password;
        private enum OPERATIONS { GET, PUT, POST, DELETE };
        // Endpoint names
        private static String ENDPOINT_AUTH = "auth";
        public CardConnectRestClient(string url, string username, string password)
        {
            if (isEmpty(url)) throw new ArgumentException("url parameter is required");
            if (isEmpty(username)) throw new ArgumentException("username parameter is required");
            if (isEmpty(password)) throw new ArgumentException("password parameter is required");

            if (!url.EndsWith("/")) url = url + "/";
            this.url = url;
            this.username = username;
            this.password = password;
            this.userpass = username + ":" + password;
        }
        private Boolean isEmpty(String s)
        {
            if (s == null) return true;
            if (s.Length <= 0) return true;
            if ("".Equals(s)) return true;
            return false;
        }
        public JObject authorizeTransaction(JObject request)
        {
            return (JObject)send(ENDPOINT_AUTH, OPERATIONS.POST, request);
        }
        private object send(string endpoint, OPERATIONS operation, JObject request)
        {
          
            HttpClient httpClient= new HttpClient();
            httpClient.BaseAddress = new Uri(url);

            RestClientOptions restClientOptions = new RestClientOptions(url);

            // Set authentication credentials
            restClientOptions.Authenticator = new HttpBasicAuthenticator(username, password);

            // Create HttpClient
            RestClient client = new RestClient(httpClient, restClientOptions);

            // Create REST request
            RestRequest rest = null;
            switch (operation)
            {
                case OPERATIONS.PUT: rest = new RestRequest(endpoint, Method.Put); break;
                case OPERATIONS.GET: rest = new RestRequest(endpoint, Method.Get); break;
                case OPERATIONS.POST: rest = new RestRequest(endpoint, Method.Post); break;
                case OPERATIONS.DELETE: rest = new RestRequest(endpoint, Method.Delete); break;
            }

            rest.RequestFormat = DataFormat.Json;
            rest.AddHeader("Content-Type", "application/json");

            String data = (request != null) ? request.ToString() : "";
            rest.AddParameter("application/json", data, ParameterType.RequestBody);
            var response = client.Execute(rest);

            JsonTextReader jsreader = new JsonTextReader(new StringReader(response.Content));

            try
            {
                return new JsonSerializer().Deserialize(jsreader);
            }
            catch (JsonReaderException jx)
            {
                return null;
            }
        }

    }
}
