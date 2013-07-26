using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Extensions;
using Newtonsoft.Json;
using PedroLamas.GDrive.Helpers;

namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private const string JsonContentType = "application/json";
        
        private readonly IGoogleAuthService _googleAuthService;

        private readonly HttpClient _client;
        private readonly HttpClient _clientForUploads;

        public GoogleDriveService(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;

            _client = googleAuthService.CreateRestClient("https://www.googleapis.com/drive/v2/");
            _clientForUploads = googleAuthService.CreateRestClient("https://www.googleapis.com/upload/drive/v2/");
        }

        public Task<GoogleDriveAbout> About(GoogleAuthToken authToken, GoogleDriveAboutRequest aboutRequest, CancellationToken cancellationToken)
        {
            var request = _googleAuthService.CreateRestRequest(authToken, "about", HttpMethod.Get);

            if (!string.IsNullOrEmpty(aboutRequest.ETag))
            {
                request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(aboutRequest.ETag));
            }

            return _client.SendAndDeserialize<GoogleDriveAbout>(request, cancellationToken);
        }

        public Task<GoogleDriveFile> FilesInsert(GoogleAuthToken authToken, GoogleDriveFilesInsertRequest filesInsertRequest, CancellationToken cancellationToken)
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
                var request = _googleAuthService.CreateRestRequest(authToken, "files" + queryStringValues.ToQueryString(), HttpMethod.Post);

                request.Content = new StringContent(JsonConvert.SerializeObject(metadata), Encoding.UTF8, JsonContentType);

                return _client.SendAndDeserialize<GoogleDriveFile>(request, cancellationToken);
            }
            else
            {
                queryStringValues.Add("uploadType", "multipart");

                var request = _googleAuthService.CreateRestRequest(authToken, "files" + queryStringValues.ToQueryString(), HttpMethod.Post);

                var metadataBytes = JsonConvert.SerializeObject(metadata, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                }).GetBytes();

                var metadataContent = new ByteArrayContent(metadataBytes);

                metadataContent.Headers.ContentType = new MediaTypeHeaderValue(JsonContentType);

                var fileContent = new ByteArrayContent(filesInsertRequest.FileContent);

                fileContent.Headers.ContentType = new MediaTypeHeaderValue(filesInsertRequest.ContentType);

                request.Content = new MultipartFormDataContent
                {
                    metadataContent,
                    fileContent
                };

                return _clientForUploads.SendAndDeserialize<GoogleDriveFile>(request, cancellationToken);
            }
        }

        public Task<GoogleDriveFile> FilesGet(GoogleAuthToken authToken, GoogleDriveFilesGetRequest filesGetRequest, CancellationToken cancellationToken)
        {
            var queryStringValues = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(filesGetRequest.Fields))
            {
                queryStringValues.Add("fields", filesGetRequest.Fields);
            }

            var request = _googleAuthService.CreateRestRequest(authToken, "files/" + filesGetRequest.FileId + queryStringValues.ToQueryString(), HttpMethod.Get);

            if (!string.IsNullOrEmpty(filesGetRequest.ETag))
            {
                request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(filesGetRequest.ETag));
            }

            return _client.SendAndDeserialize<GoogleDriveFile>(request, cancellationToken);
        }

        public Task<GoogleDriveFilesListResponse> FilesList(GoogleAuthToken authToken, GoogleDriveFilesListRequest filesListRequest, CancellationToken cancellationToken)
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

            var request = _googleAuthService.CreateRestRequest(authToken, "files" + queryStringValues.ToQueryString(), HttpMethod.Get);

            if (!string.IsNullOrEmpty(filesListRequest.ETag))
            {
                request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(filesListRequest.ETag));
            }

            return _client.SendAndDeserialize<GoogleDriveFilesListResponse>(request, cancellationToken);
        }

        public Task<GoogleDriveFile> FilesDelete(GoogleAuthToken authToken, string fileId, CancellationToken cancellationToken)
        {
            var request = _googleAuthService.CreateRestRequest(authToken, "files/" + fileId, HttpMethod.Delete);

            return _client.SendAndDeserialize<GoogleDriveFile>(request, cancellationToken);
        }

        public Task<GoogleDriveFile> FilesTrash(GoogleAuthToken authToken, string fileId, CancellationToken cancellationToken)
        {
            var request = _googleAuthService.CreateRestRequest(authToken, "files/" + fileId + "/trash", HttpMethod.Post);

            return _client.SendAndDeserialize<GoogleDriveFile>(request, cancellationToken);
        }

        public Task<GoogleDriveFile> FilesUpdate(GoogleAuthToken authToken, string fileId, GoogleDriveFilesUpdateRequest filesUpdateRequest, CancellationToken cancellationToken)
        {
            var queryStringValues = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(filesUpdateRequest.Fields))
            {
                queryStringValues.Add("fields", filesUpdateRequest.Fields);
            }

            var request = _googleAuthService.CreateRestRequest(authToken, "files/" + fileId + queryStringValues.ToQueryString(), HttpMethod.Put);

            request.Content = new StringContent(JsonConvert.SerializeObject(filesUpdateRequest.File), Encoding.UTF8, JsonContentType);

            return _client.SendAndDeserialize<GoogleDriveFile>(request, cancellationToken);
        }

        public async Task<byte[]> FileDownload(GoogleAuthToken authToken, string downloadUrl, CancellationToken cancellationToken)
        {
            var request = _googleAuthService.CreateRestRequest(authToken,downloadUrl, HttpMethod.Get);

            var response = await _client.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}