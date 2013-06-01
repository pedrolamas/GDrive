using System.Threading;
using System.Threading.Tasks;

namespace PedroLamas.GDrive.Service
{
    public interface IGoogleOAuth2Service
    {
        Task<GoogleUserInfoResponse> GetUserInfo(GoogleAuthToken authToken, CancellationToken cancellationToken);
    }
}