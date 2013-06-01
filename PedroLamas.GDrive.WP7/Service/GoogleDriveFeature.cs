using Newtonsoft.Json;

namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveFeature
    {
        [JsonProperty("featureName")]
        public string FeatureName { get; set; }

        [JsonProperty("featureRate")]
        public double? FeatureRate { get; set; }
    }
}