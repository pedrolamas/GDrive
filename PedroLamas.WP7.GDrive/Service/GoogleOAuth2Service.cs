using PedroLamas.WP7.ServiceModel;
using RestSharp;

namespace PedroLamas.WP7.GDrive.Service
{
    public class GoogleOAuth2Service : IGoogleOAuth2Service
    {
        private readonly IGoogleAuthService _googleAuthService;

        private readonly RestClient _client;

        public GoogleOAuth2Service(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;

            _client = googleAuthService.CreateRestClient("https://www.googleapis.com/oauth2/v2/");
        }

        public RestRequestAsyncHandle GetUserInfo(GoogleAuthToken authToken, ResultCallback<GoogleUserInfoResponse> callback, object state)
        {
            var request = _googleAuthService.CreateRestRequest(authToken, "userinfo", Method.GET);

            return _client.GetResultAsync(request, callback, state);
        }
    }
}