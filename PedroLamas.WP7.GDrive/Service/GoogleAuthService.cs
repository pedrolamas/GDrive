using System;
using System.Collections.Generic;
using System.Linq;
using PedroLamas.WP7.ServiceModel;
using RestSharp;

namespace PedroLamas.WP7.GDrive.Service
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private const int SAFE_EXPIRATION_WINDOW = 60;

        private readonly IGoogleClientSettings _clientSettings;

        private readonly RestClient _client;

        public GoogleAuthService(IGoogleClientSettings clientSettings)
        {
            _clientSettings = clientSettings;

            _client = CreateRestClient("https://accounts.google.com/o/oauth2/token");
        }

        public Uri GetAuthUri()
        {
            var queryStringValues = new Dictionary<string, string>
            {
                {"response_type", "code"},
                {"client_id", _clientSettings.ClientId},
                {"scope",  string.Join(" ", _clientSettings.Scopes.ToArray())},
                {"redirect_uri", "http://localhost"},
                {"approval_prompt", "force"},
            };

            var queryString = string.Join("&", queryStringValues
                .Select(x => string.Format("{0}={1}", x.Key, Uri.EscapeDataString(x.Value))));

            var completeUrl = "https://accounts.google.com/o/oauth2/auth?" + queryString;

            return new Uri(completeUrl);
        }

        public RestRequestAsyncHandle ExchangeAuthorizationCode(string code, ResultCallback<GoogleAuthToken> callback, object state)
        {
            var request = new RestRequest(Method.POST);

            request.AddParameter("code", code);
            request.AddParameter("client_id", _clientSettings.ClientId);
            request.AddParameter("client_secret", _clientSettings.ClientSecret);
            request.AddParameter("redirect_uri", "http://localhost");
            request.AddParameter("grant_type", "authorization_code");

            return _client.GetResultAsync<GoogleAuthToken>(request, result =>
            {
                if (result.Data != null)
                {
                    var newAuthToken = result.Data;

                    newAuthToken.ExpiresDateTime = DateTime.Now.AddSeconds(newAuthToken.ExpiresIn - SAFE_EXPIRATION_WINDOW);
                }

                callback(result);
            }, state);
        }

        public RestRequestAsyncHandle RefreshToken(GoogleAuthToken authToken, ResultCallback<GoogleAuthToken> callback, object state)
        {
            var request = new RestRequest(Method.POST);

            request.AddParameter("client_id", _clientSettings.ClientId);
            request.AddParameter("client_secret", _clientSettings.ClientSecret);
            request.AddParameter("refresh_token", authToken.RefreshToken);
            request.AddParameter("grant_type", "refresh_token");

            return _client.GetResultAsync<GoogleAuthToken>(request, result =>
            {
                if (result.Data != null)
                {
                    var newAuthToken = result.Data;

                    newAuthToken.RefreshToken = authToken.RefreshToken;
                    newAuthToken.ExpiresDateTime = DateTime.Now.AddSeconds(newAuthToken.ExpiresIn - SAFE_EXPIRATION_WINDOW);
                }

                callback(result);
            }, state);
        }

        public RestClient CreateRestClient(string baseUrl)
        {
            var client = new RestClient(baseUrl)
            {
                UserAgent = "GDrive (gzip)"
            };

            client.AddDefaultHeader("Accept-Encoding", "gzip");

            return client;
        }

        public RestRequest CreateRestRequest(GoogleAuthToken authToken, string resource, Method method)
        {
            var request = new RestRequest(resource, method)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = NewtonsoftJsonSerializer.StaticInstance
            };

            request.AddHeader("Authorization", string.Format("{0} {1}", authToken.TokenType, authToken.AccessToken));

            return request;
        }

        public void CheckTokenAndExecute<T>(GoogleAuthToken authToken, ResultCallback<T> callback, Action successAction, object state)
        {
            lock (_clientSettings)
            {
                if (DateTime.Now > authToken.ExpiresDateTime)
                {
                    RefreshToken(authToken, result =>
                    {
                        if (result.Status != ResultStatus.Completed)
                        {
                            callback(new Result<T>(result.Status, state));
                        }
                        else
                        {
                            successAction();
                        }
                    }, state);
                }
                else
                {
                    successAction();
                }
            }
        }
    }
}