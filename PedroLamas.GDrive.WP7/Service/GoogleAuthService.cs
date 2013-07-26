using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using PedroLamas.GDrive.Helpers;

namespace PedroLamas.GDrive.Service
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private const int SafeExpirationWindow = 60;

        private readonly IGoogleClientSettings _clientSettings;

        private readonly HttpClient _client;

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

            var completeUrl = "https://accounts.google.com/o/oauth2/auth" + queryStringValues.ToQueryString();

            return new Uri(completeUrl);
        }

        public async Task<GoogleAuthToken> ExchangeAuthorizationCode(string code, CancellationToken cancellationToken)
        {
            var formPostValues = new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", _clientSettings.ClientId },
                { "client_secret", _clientSettings.ClientSecret },
                { "redirect_uri", "http://localhost" },
                { "grant_type", "authorization_code" }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(formPostValues)
            };

            var result = await _client.SendAndDeserialize<GoogleAuthToken>(requestMessage, cancellationToken);

            result.ExpiresDateTime = DateTime.Now.AddSeconds(result.ExpiresIn - SafeExpirationWindow);

            return result;
        }

        public async Task<GoogleAuthToken> RefreshToken(GoogleAuthToken authToken, CancellationToken cancellationToken)
        {
            var formPostValues = new Dictionary<string, string>
            {
                { "client_id", _clientSettings.ClientId },
                { "client_secret", _clientSettings.ClientSecret },
                { "refresh_token", authToken.RefreshToken },
                { "grant_type", "refresh_token" }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(formPostValues)
            };

            var result = await _client.SendAndDeserialize<GoogleAuthToken>(requestMessage, cancellationToken);

            result.RefreshToken = authToken.RefreshToken;
            result.ExpiresDateTime = DateTime.Now.AddSeconds(result.ExpiresIn - SafeExpirationWindow);

            return result;
        }

        public HttpClient CreateRestClient(string baseUrl)
        {
            var handler = new HttpClientHandler();

            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl),
            };

            if (handler.SupportsAutomaticDecompression)
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("GDrive_gzip")));
            }
            else
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("GDrive")));
            }

            return client;
        }

        public HttpRequestMessage CreateRestRequest(GoogleAuthToken authToken, string resource, HttpMethod method)
        {
            var request = new HttpRequestMessage(method, resource);

            request.Headers.Authorization = new AuthenticationHeaderValue(authToken.TokenType, authToken.AccessToken);

            return request;
        }
    }
}