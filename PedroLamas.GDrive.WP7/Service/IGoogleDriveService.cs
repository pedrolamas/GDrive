using System.Threading;
using System.Threading.Tasks;

namespace PedroLamas.GDrive.Service
{
    public interface IGoogleDriveService
    {
        Task<GoogleDriveAbout> About(GoogleAuthToken authToken, GoogleDriveAboutRequest aboutRequest, CancellationToken cancellationToken);

        Task<GoogleDriveFile> FilesInsert(GoogleAuthToken authToken, GoogleDriveFilesInsertRequest filesInsertRequest, CancellationToken cancellationToken);

        Task<GoogleDriveFile> FilesGet(GoogleAuthToken authToken, GoogleDriveFilesGetRequest filesGetRequest, CancellationToken cancellationToken);

        Task<GoogleDriveFilesListResponse> FilesList(GoogleAuthToken authToken, GoogleDriveFilesListRequest filesListRequest, CancellationToken cancellationToken);

        Task<GoogleDriveFile> FilesDelete(GoogleAuthToken authToken, string fileId, CancellationToken cancellationToken);

        Task<GoogleDriveFile> FilesTrash(GoogleAuthToken authToken, string fileId, CancellationToken cancellationToken);

        Task<GoogleDriveFile> FilesUpdate(GoogleAuthToken authToken, string fileId, GoogleDriveFilesUpdateRequest filesUpdateRequest, CancellationToken cancellationToken);

        Task<byte[]> FileDownload(GoogleAuthToken authToken, string downloadUrl, CancellationToken cancellationToken);
    }
}