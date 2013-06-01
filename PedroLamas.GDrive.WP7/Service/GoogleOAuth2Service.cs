using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PedroLamas.GDrive.Helpers;

namespace PedroLamas.GDrive.Service
{
    public class GoogleOAuth2Service : IGoogleOAuth2Service
    {
        private readonly IGoogleAuthService _googleAuthService;

        private readonly HttpClient _client;

        public GoogleOAuth2Service(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;

            _client = googleAuthService.CreateRestClient("https://www.googleapis.com/oauth2/v2/");
        }

        public Task<GoogleUserInfoResponse> GetUserInfo(GoogleAuthToken authToken, CancellationToken cancellationToken)
        {
            var request = _googleAuthService.CreateRestRequest(authToken, "userinfo", HttpMethod.Get);

            return _client.SendAndDeserialize<GoogleUserInfoResponse>(request, cancellationToken);
        }
    }
}