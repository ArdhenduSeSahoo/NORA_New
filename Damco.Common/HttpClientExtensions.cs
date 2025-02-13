using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Damco.Common
{
    public static class HttpClientExtensions
    {
        public static HttpResponseMessage SendWithOAuth2ClientCredentials(this HttpClient client, string tokenUrl, string clientId, string clientSecret, string resource, Func<HttpRequestMessage> requestFactory)
        {
            var tokenPayload = GetOAuth2ClientCredentialsPayload(clientId, clientSecret, resource);
            var token = GetToken(tokenUrl, tokenPayload, null);

            client.SetToken(token.Item1, token.Item2);
            client.Timeout = TimeSpan.FromMinutes(5);
            var request = requestFactory();
            Debug.WriteLine($"{request.Method} {client.BaseAddress}{request.RequestUri}");
            var serviceCallResult = client.SendAsync(request).Result;
            if (serviceCallResult.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = GetToken(tokenUrl, tokenPayload, token);
                client.SetToken(token.Item1, token.Item2);
                serviceCallResult = client.SendAsync(requestFactory()).Result;
            }
            return serviceCallResult;
        }

        public static async Task<HttpResponseMessage> SendWithOAuth2ClientCredentialsAsync(this HttpClient client, string tokenUrl, string clientId, string clientSecret, string resource, HttpRequestMessage request)
        {
            var tokenPayload = GetOAuth2ClientCredentialsPayload(clientId, clientSecret, resource);
            var token = GetToken(tokenUrl, tokenPayload, null);

            client.SetToken(token.Item1, token.Item2);
            client.Timeout = TimeSpan.FromMinutes(5);
            var serviceCallResult = await client.SendAsync(request);
            if (serviceCallResult.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = GetToken(tokenUrl, tokenPayload, token);
                client.SetToken(token.Item1, token.Item2);
                serviceCallResult = await client.SendAsync(request);
            }
            return serviceCallResult;
        }

        public static HttpResponseMessage SendWithBasicCredentialsAsync(this HttpClient client, string userName, string password, Func<HttpRequestMessage> requestFactory)
        {
            client.SetBasicCredentials(userName, password);
            var request = requestFactory();
            return client.SendAsync(request).Result;
        }

        public static HttpClient SetBasicCredentials(this HttpClient client, string userName, string password)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
              Convert.ToBase64String(Encoding.ASCII.GetBytes($@"{userName}:{password}"))
            );
            return client;
        }

        private static string GetOAuth2ClientCredentialsPayload(string clientId, string clientSecret, string resource)
        {
            return $"grant_type=client_credentials&client_id={clientId}&client_secret={HttpUtility.UrlEncode(clientSecret)}&resource={resource}";
        }

        static ConcurrentDictionary<Tuple<string, string>, Tuple<string, string, DateTime>> _tokens = new ConcurrentDictionary<Tuple<string, string>, Tuple<string, string, DateTime>>();
        private static Tuple<string, string, DateTime> GetToken(string tokenUrl, string tokenPayload, Tuple<string, string, DateTime> failedToken)
        {
            var key = Tuple.Create(tokenUrl, tokenPayload);
            if (failedToken == null)
            {
                var result = _tokens.GetOrAdd(key, GetTokenFromService);
                if (result.Item3 < DateTime.UtcNow) //Expired
                {
                    Tuple<string, string, DateTime> removed;
                    if (_tokens.TryRemove(key, out removed) && removed != result) //We removed the wrong thing, probably another thread got a new token already
                        _tokens.TryAdd(key, removed); //put it back
                    return _tokens.GetOrAdd(key, GetTokenFromService);
                }
                else
                    return result;
            }
            else
                //Note use we use "AddOrUpdate" because this way we can let the update have the logic
                //to decide if we actually need to get a new token
                return _tokens.AddOrUpdate(key,
                    GetTokenFromService,
                    (k, v) => (v == failedToken ? GetTokenFromService(k) : v) //Note v!= failedToken if another thread already refreshed the token
                );
        }
        private static Tuple<string, string, DateTime> GetTokenFromService(Tuple<string, string> urlAndPayload)
        {
            Debug.WriteLine(nameof(GetTokenFromService));
            var response = new HttpClient().PostAsync(urlAndPayload.Item1, new StringContent(urlAndPayload.Item2, Encoding.UTF8, "application/x-www-form-urlencoded"))
                .Result.Content.ReadAsStringAsync().Result;
            Dictionary<string, string> dictionary;
            try
            {
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            }
            catch
            {
                throw new InvalidOperationException("Response from token service invalid, response is: " + response);
            }
            int expiresIn;
            if (!dictionary.ContainsKey("expires_in") || !int.TryParse((dictionary["expires_in"]?.ToString() ?? ""), out expiresIn))
                expiresIn = 0; //Default
            expiresIn -= 60; //Let token expire one minute early to avoid token expiring during service call
            if (expiresIn < 1) expiresIn = 1; //Need to make sure to use it at least in this run

#if false
            expiresIn = 5;
#endif 

            return Tuple.Create(
                dictionary["token_type"],
                dictionary["access_token"],
                 DateTime.UtcNow.AddSeconds(expiresIn)
            );
        }

        public static HttpClient SetToken(this HttpClient client, string tokenType, string token)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(tokenType, token);
            return client;
        }

        public static HttpRequestMessage ContentAsJson<T>(this HttpRequestMessage message, T content)
        {
            message.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            return message;
        }

        public static HttpRequestMessage ContentAsString(this HttpRequestMessage message, string content, Encoding encoding = null, string mediaType = "application/json")
        {
            message.Content = new StringContent(content, encoding ?? Encoding.UTF8, mediaType);
            return message;
        }

        public static HttpRequestMessage AddHeader(this HttpRequestMessage message, string name, params string[] values)
        {
            message.Headers.Add(name, values);
            return message;
        }


    }

}
