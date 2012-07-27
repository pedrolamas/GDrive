using Newtonsoft.Json;
using RestSharp.Serializers;

namespace PedroLamas.WP7.GDrive.Service
{
    public class NewtonsoftJsonSerializer : ISerializer
    {
        private static readonly ISerializer _serializer = new NewtonsoftJsonSerializer();
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static ISerializer StaticInstance
        {
            get
            {
                return _serializer;
            }
        }

        public string ContentType { get; set; }

        public string DateFormat { get; set; }

        public string Namespace { get; set; }

        public string RootElement { get; set; }

        public NewtonsoftJsonSerializer()
        {
            ContentType = "application/json";
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
        }
    }
}