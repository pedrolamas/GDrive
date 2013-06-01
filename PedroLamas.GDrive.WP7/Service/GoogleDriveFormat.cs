using System.Collections.Generic;
using Newtonsoft.Json;

namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveFormat
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("targets")]
        public IEnumerable<string> Targets { get; set; }
    }
}