using PedroLamas.WP7.ServiceModel;
using RestSharp;

namespace PedroLamas.WP7.GDrive.Service
{
    public interface IGoogleDriveService
    {
        RestRequestAsyncHandle About(GoogleAuthToken authToken, GoogleDriveAboutRequest aboutRequest, ResultCallback<GoogleDriveAbout> callback, object state);

        RestRequestAsyncHandle FilesInsert(GoogleAuthToken authToken, GoogleDriveFilesInsertRequest filesInsertRequest, ResultCallback<GoogleDriveFile> callback, object state);

        RestRequestAsyncHandle FilesGet(GoogleAuthToken authToken, GoogleDriveFilesGetRequest filesGetRequest, ResultCallback<GoogleDriveFile> callback, object state);

        RestRequestAsyncHandle FilesList(GoogleAuthToken authToken, GoogleDriveFilesListRequest filesListRequest, ResultCallback<GoogleDriveFilesListResponse> callback, object state);

        RestRequestAsyncHandle FilesDelete(GoogleAuthToken authToken, string fileId, ResultCallback<GoogleDriveFile> callback, object state);

        RestRequestAsyncHandle FilesTrash(GoogleAuthToken authToken, string fileId, ResultCallback<GoogleDriveFile> callback, object state);

        RestRequestAsyncHandle FilesUpdate(GoogleAuthToken authToken, string fileId, GoogleDriveFilesUpdateRequest filesUpdateRequest, ResultCallback<GoogleDriveFile> callback, object state);

        RestRequestAsyncHandle ChildrenList(GoogleAuthToken authToken, GoogleDriveChildrenListRequest childrenListRequest, ResultCallback<GoogleDriveChildrenListResponse> callback, object state);
    }
}