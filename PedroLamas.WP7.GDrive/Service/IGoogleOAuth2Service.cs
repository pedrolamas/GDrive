using PedroLamas.WP7.ServiceModel;
using RestSharp;

namespace PedroLamas.WP7.GDrive.Service
{
    public interface IGoogleOAuth2Service
    {
        RestRequestAsyncHandle GetUserInfo(GoogleAuthToken authToken, ResultCallback<GoogleUserInfoResponse> callback, object state);
    }
}