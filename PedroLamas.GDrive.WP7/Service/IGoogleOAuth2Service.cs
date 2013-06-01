using PedroLamas.ServiceModel;
using RestSharp;

namespace PedroLamas.GDrive.Service
{
    public interface IGoogleOAuth2Service
    {
        RestRequestAsyncHandle GetUserInfo(GoogleAuthToken authToken, ResultCallback<GoogleUserInfoResponse> callback, object state);
    }
}