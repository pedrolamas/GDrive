using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Extensions;
using Newtonsoft.Json;

namespace PedroLamas.GDrive.Helpers
{
    public static class ExtensionMethods
    {
        private static readonly string[] _availableSuffixes = new[] { "B", "KB", "MB", "GB", "TB" };

        public static string ToQueryString(this IDictionary<string, string> dictionary)
        {
            if (dictionary.Count == 0)
            {
                return string.Empty;
            }

            return "?" + string.Join("&", dictionary
                .Select(x => string.Format("{0}={1}", Uri.EscapeUriString(x.Key), Uri.EscapeUriString(x.Value))));
        }

        public static async Task<T> SendAndDeserialize<T>(this HttpClient client, HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            var response = await client.SendAsync(requestMessage, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotModified)
            {
                return default(T);
            }

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        public static string FormatAsSize(this int size)
        {
            if (size <= 1024)
            {
                return "1 KB";
            }

            var suffixIndex = 0;

            while (size > 1024)
            {
                size = size / 1024;

                suffixIndex++;
            }

            return "{0} {1}".FormatWithInvariantCulture(size, _availableSuffixes[suffixIndex]);
        }
    }

    
    
    //public enum ResultStatus
    //{
    //    Completed,
    //    Empty,
    //    Aborted,
    //    Error
    //}

    //public class Result<T>
    //{
    //    #region Properties

    //    public ResultStatus Status { get; private set; }

    //    public T Data { get; private set; }

    //    public HttpStatusCode StatusCode { get; private set; }

    //    public DateTime? LastModified { get; private set; }

    //    public string ETag { get; private set; }

    //    public Exception Error { get; private set; }

    //    #endregion

    //    public Result(T data, HttpStatusCode statusCode, DateTime? lastModified, string etag)
    //    {
    //        Data = data;
    //        StatusCode = statusCode;
    //        LastModified = lastModified;
    //        ETag = etag;

    //        switch (statusCode)
    //        {
    //            case HttpStatusCode.OK:
    //                Status = ResultStatus.Completed;
    //                break;

    //            case HttpStatusCode.NoContent:
    //            case HttpStatusCode.NotModified:
    //                Status = ResultStatus.Empty;
    //                break;

    //            default:
    //                Status = ResultStatus.Error;
    //                break;
    //        }
    //    }

    //    public Result(Exception error)
    //    {
    //        Error = error;

    //        Status = ResultStatus.Error;
    //    }

    //    public Result(ResultStatus status)
    //    {
    //        Status = status;
    //    }
    //}
}