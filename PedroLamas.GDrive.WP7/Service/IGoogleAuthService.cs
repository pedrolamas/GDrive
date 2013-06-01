using System;
using PedroLamas.ServiceModel;
using RestSharp;

namespace PedroLamas.GDrive.Service
{
    public interface IGoogleAuthService
    {
        Uri GetAuthUri();

        RestRequestAsyncHandle ExchangeAuthorizationCode(string code, ResultCallback<GoogleAuthToken> callback, object state);

        RestRequestAsyncHandle RefreshToken(GoogleAuthToken authToken, ResultCallback<GoogleAuthToken> callback, object state);

        RestClient CreateRestClient(string baseUrl);

        RestRequest CreateRestRequest(GoogleAuthToken authToken, string resource, Method method);

        void CheckTokenAndExecute<T>(GoogleAuthToken authToken, ResultCallback<T> callback, Action successAction, object state);
    }
}