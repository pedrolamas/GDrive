using System;
using System.IO;
using System.Net;
using System.Text;
using AgFx;
using Newtonsoft.Json;
using RestSharp;

namespace PedroLamas.WP7.GDrive.Service
{
    public class RestSharpLoadRequest : LoadRequest
    {
        private readonly RestClient _client;
        private RestRequest _request;

        public RestSharpLoadRequest(LoadContext loadContext, RestClient client, string resource)
            : base(loadContext)
        {
            _client = client;
            _request = new RestRequest(resource);
        }

        public override void Execute(Action<LoadRequestResult> result)
        {
            PriorityQueue.AddNetworkWorkItem(() =>
            {
                _client.ExecuteAsync(_request, response =>
                {
                    if (response.ErrorException != null)
                    {
                        result(new LoadRequestResult(response.ErrorException));
                    }
                    else if (response.StatusCode != HttpStatusCode.OK)
                    {
                        result(new LoadRequestResult(new WebException("Bad web response, StatusCode=" + response.StatusCode)));
                    }
                    else
                    {
                        var byteArray = Encoding.UTF8.GetBytes(response.Content);

                        result(new LoadRequestResult(new MemoryStream(byteArray)));
                    }
                });
            });
        }
    }

    public class RestSharpLoadContext : LoadContext
    {
        public RestSharpLoadContext()
            : base(DateTime.Now.ToString())
        {

        }
    }

    public class RestSharpLoadRequestResult : IDataLoader<RestSharpLoadContext>
    {
        private static readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public object Deserialize(RestSharpLoadContext loadContext, Type objectType, Stream stream)
        {
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                return _jsonSerializer.Deserialize(streamReader, objectType);
            }
        }

        public LoadRequest GetLoadRequest(RestSharpLoadContext loadContext, Type objectType)
        {
            return new RestSharpLoadRequest(loadContext, null, null);
        }
    }
}