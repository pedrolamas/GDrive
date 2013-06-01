using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PedroLamas.GDrive.Service
{
    public interface IGoogleAuthService
    {
        Uri GetAuthUri();

        Task<GoogleAuthToken> ExchangeAuthorizationCode(string code, CancellationToken cancellationToken);

        Task<GoogleAuthToken> RefreshToken(GoogleAuthToken authToken, CancellationToken cancellationToken);

        HttpClient CreateRestClient(string baseUrl);

        HttpRequestMessage CreateRestRequest(GoogleAuthToken authToken, string resource, HttpMethod method);

        //Task<T> CheckTokenAndExecute<T>(GoogleAuthToken authToken, Action successAction, CancellationToken cancellationToken);
    }
}