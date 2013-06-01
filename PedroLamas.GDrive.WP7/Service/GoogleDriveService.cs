using Cimbalino.Phone.Toolkit.Extensions;
using Newtonsoft.Json;
using PedroLamas.ServiceModel;
using RestSharp;

namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly IGoogleAuthService _googleAuthService;

        private readonly RestClient _client;
        private readonly RestClient _clientForUploads;

        public GoogleDriveService(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;

            _client = googleAuthService.CreateRestClient("https://www.googleapis.com/drive/v2");
            _clientForUploads = googleAuthService.CreateRestClient("https://www.googleapis.com/upload/drive/v2");
        }

        public RestRequestAsyncHandle About(GoogleAuthToken authToken, GoogleDriveAboutRequest aboutRequest, ResultCallback<GoogleDriveAbout> callback, object state)
        {
            var request = _googleAuthService.CreateRestRequest(authToken, "about", Method.GET);

            if (!string.IsNullOrEmpty(aboutRequest.ETag))
            {
                request.AddHeader("If-None-Match", aboutRequest.ETag);
            }

            return _client.GetResultAsync(request, callback, state);
        }

        public RestRequestAsyncHandle FilesInsert(GoogleAuthToken authToken, GoogleDriveFilesInsertRequest filesInsertRequest, ResultCallback<GoogleDriveFile> callback, object state)
        {
            var request = _googleAuthService.CreateRestRequest(authToken, "files", Method.POST);

            if (filesInsertRequest.Convert.HasValue)
            {
                request.AddUrlSegment("convert", filesInsertRequest.Convert.ToString());
            }
            if (filesInsertRequest.Ocr.HasValue)
            {
                request.AddUrlSegment("ocr", filesInsertRequest.Ocr.ToString());
            }
            if (!string.IsNullOrEmpty(filesInsertRequest.Fields))
            {
                request.AddUrlSegment("fields", filesInsertRequest.Fields);
            }

            var metadata = new GoogleDriveFile()
            {
                Title = filesInsertRequest.Filename,
                MimeType = filesInsertRequest.ContentType,
                Parents = new GoogleDriveParent[] { 
                    new GoogleDriveParent() {
                        Id = filesInsertRequest.FolderId
                    }
                }
            };

            if (filesInsertRequest.FileContent == null)
            {
                request.AddBody(metadata);

                return _client.GetResultAsync(request, callback, state);
            }
            else
            {
                request.AddUrlSegment("uploadType", "multipart");

                var metadataBytes = JsonConvert.SerializeObject(metadata, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                }).GetBytes();

                request.AddFile(null, metadataBytes, null, "application/json");
                request.AddFile(null, filesInsertRequest.FileContent, null, filesInsertRequest.ContentType);

                return _clientForUploads.GetResultAsync(request, callback, state);
            }
        }

        public RestRequestAsyncHandle FilesGet(GoogleAuthToken authToken, GoogleDriveFilesGetRequest filesGetRequest, ResultCallback<GoogleDriveFile> callback, object state)
        {
            var request = _googleAuthService.CreateRestRequest(authToken, "files/{fileId}", Method.GET);

            request.AddParameter("fileId", filesGetRequest.FileId);

            if (!string.IsNullOrEmpty(filesGetRequest.Fields))
            {
                request.AddUrlSegment("fields", filesGetRequest.Fields);
            }

            if (!string.IsNullOrEmpty(filesGetRequest.ETag))
            {
                request.AddHeader("If-None-Match", filesGetRequest.ETag);
            }

            return _client.GetResultAsync(request, callback, state);
        }

        public RestRequestAsyncHandle FilesList(GoogleAuthToken authToken, GoogleDriveFilesListRequest filesListRequest, ResultCallback<GoogleDriveFilesListResponse> callback, object state)
        {
            var request = _googleAuthService.CreateRestRequest(authToken, "files", Method.GET);

            if (filesListRequest.MaxResults.HasValue)
            {
                request.AddUrlSegment("maxResults", filesListRequest.MaxResults.Value.ToString());
            }

            if (!string.IsNullOrEmpty(filesListRequest.PageToken))
            {
                request.AddUrlSegment("pageToken", filesListRequest.PageToken);
            }

            if (!string.IsNullOrEmpty(filesListRequest.Projection))
            {
                request.AddUrlSegment("projection", filesListRequest.Projection);
            }

            if (!string.IsNullOrEmpty(filesListRequest.Query))
            {
                request.AddUrlSegment("q", filesListRequest.Query);
            }

            if (!string.IsNullOrEmpty(filesListRequest.Fields))
            {
                request.AddUrlSegment("fields", filesListRequest.Fields);
            }

            if (!string.IsNullOrEmpty(filesListRequest.ETag))
            {
                request.AddHeader("If-None-Match", filesListRequest.ETag);
            }

            return _client.GetResultAsync(request, callback, state);
        }

        public RestRequestAsyncHandle FilesDelete(GoogleAuthToken authToken, string fileId, ResultCallback<GoogleDriveFile> callback, object state)
        {
            var request = _googleAuthService.CreateRestRequest(authToken, "files/{fileId}", Method.DELETE);

            request.AddParameter("fileId", fileId);

            return _client.GetResultAsync(request, callback, state);
        }

        public RestRequestAsyncHandle FilesTrash(GoogleAuthToken authToken, string fileId, ResultCallback<GoogleDriveFile> callback, object state)
        {
            var request = _googleAuthService.CreateRestRequest(authToken, "files/{fileId}/trash", Method.POST);

            request.AddParameter("fileId", fileId);

            return _client.GetResultAsync(request, callback, state);
        }

        public RestRequestAsyncHandle FilesUpdate(GoogleAuthToken authToken, string fileId, GoogleDriveFilesUpdateRequest filesUpdateRequest, ResultCallback<GoogleDriveFile> callback, object state)
        {
            var request = _googleAuthService.CreateRestRequest(authToken, "files/{fileId}", Method.PUT);

            request.AddParameter("fileId", fileId, ParameterType.UrlSegment);

            if (!string.IsNullOrEmpty(filesUpdateRequest.Fields))
            {
                request.AddUrlSegment("fields", filesUpdateRequest.Fields);
            }

            request.AddBody(filesUpdateRequest.File);

            return _client.GetResultAsync(request, callback, state);
        }
    }
}