using System.Collections.Generic;
using Cimbalino.Phone.Toolkit.Extensions;
using Newtonsoft.Json;
using PedroLamas.GDrive.Helpers;
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
            var queryStringValues = new Dictionary<string, string>();

            if (filesInsertRequest.Convert.HasValue)
            {
                queryStringValues.Add("convert", filesInsertRequest.Convert.ToString());
            }

            if (filesInsertRequest.Ocr.HasValue)
            {
                queryStringValues.Add("ocr", filesInsertRequest.Ocr.ToString());
            }

            if (!string.IsNullOrEmpty(filesInsertRequest.Fields))
            {
                queryStringValues.Add("fields", filesInsertRequest.Fields);
            }

            var request = _googleAuthService.CreateRestRequest(authToken, "files" + queryStringValues.ToQueryString(), Method.POST);

            var metadata = new GoogleDriveFile()
            {
                Title = filesInsertRequest.Filename,
                MimeType = filesInsertRequest.ContentType,
                Parents = new[] { 
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
            
            request.AddUrlSegment("uploadType", "multipart");

            var metadataBytes = JsonConvert.SerializeObject(metadata, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            }).GetBytes();

            request.AddFile(null, metadataBytes, null, "application/json");
            request.AddFile(null, filesInsertRequest.FileContent, null, filesInsertRequest.ContentType);

            return _clientForUploads.GetResultAsync(request, callback, state);
        }

        public RestRequestAsyncHandle FilesGet(GoogleAuthToken authToken, GoogleDriveFilesGetRequest filesGetRequest, ResultCallback<GoogleDriveFile> callback, object state)
        {
            var queryStringValues = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(filesGetRequest.Fields))
            {
                queryStringValues.Add("fields", filesGetRequest.Fields);
            }

            var request = _googleAuthService.CreateRestRequest(authToken, "files/{fileId}" + queryStringValues.ToQueryString(), Method.GET);

            request.AddUrlSegment("fileId", filesGetRequest.FileId);

            if (!string.IsNullOrEmpty(filesGetRequest.ETag))
            {
                request.AddHeader("If-None-Match", filesGetRequest.ETag);
            }

            return _client.GetResultAsync(request, callback, state);
        }

        public RestRequestAsyncHandle FilesList(GoogleAuthToken authToken, GoogleDriveFilesListRequest filesListRequest, ResultCallback<GoogleDriveFilesListResponse> callback, object state)
        {
            var queryStringValues = new Dictionary<string, string>();

            if (filesListRequest.MaxResults.HasValue)
            {
                queryStringValues.Add("maxResults", filesListRequest.MaxResults.Value.ToStringInvariantCulture());
            }

            if (!string.IsNullOrEmpty(filesListRequest.PageToken))
            {
                queryStringValues.Add("pageToken", filesListRequest.PageToken);
            }

            if (!string.IsNullOrEmpty(filesListRequest.Projection))
            {
                queryStringValues.Add("projection", filesListRequest.Projection);
            }

            if (!string.IsNullOrEmpty(filesListRequest.Query))
            {
                queryStringValues.Add("q", filesListRequest.Query);
            }

            if (!string.IsNullOrEmpty(filesListRequest.Fields))
            {
                queryStringValues.Add("fields", filesListRequest.Fields);
            }

            var request = _googleAuthService.CreateRestRequest(authToken, "files" + queryStringValues.ToQueryString(), Method.GET);

            if (!string.IsNullOrEmpty(filesListRequest.ETag))
            {
                request.AddHeader("If-None-Match", filesListRequest.ETag);
            }

            return _client.GetResultAsync(request, callback, state);
        }

        public RestRequestAsyncHandle FilesDelete(GoogleAuthToken authToken, string fileId, ResultCallback<GoogleDriveFile> callback, object state)
        {
            var request = _googleAuthService.CreateRestRequest(authToken, "files/{fileId}", Method.DELETE);

            request.AddUrlSegment("fileId", fileId);

            return _client.GetResultAsync(request, callback, state);
        }

        public RestRequestAsyncHandle FilesTrash(GoogleAuthToken authToken, string fileId, ResultCallback<GoogleDriveFile> callback, object state)
        {
            var request = _googleAuthService.CreateRestRequest(authToken, "files/{fileId}/trash", Method.POST);

            request.AddUrlSegment("fileId", fileId);

            return _client.GetResultAsync(request, callback, state);
        }

        public RestRequestAsyncHandle FilesUpdate(GoogleAuthToken authToken, string fileId, GoogleDriveFilesUpdateRequest filesUpdateRequest, ResultCallback<GoogleDriveFile> callback, object state)
        {
            var queryStringValues = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(filesUpdateRequest.Fields))
            {
                queryStringValues.Add("fields", filesUpdateRequest.Fields);
            }

            var request = _googleAuthService.CreateRestRequest(authToken, "files/{fileId}" + queryStringValues.ToQueryString(), Method.PUT);

            request.AddUrlSegment("fileId", fileId);

            request.AddBody(filesUpdateRequest.File);

            return _client.GetResultAsync(request, callback, state);
        }
    }
}