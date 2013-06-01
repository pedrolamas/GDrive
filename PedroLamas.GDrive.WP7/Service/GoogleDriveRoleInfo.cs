using System.Collections.Generic;
using Newtonsoft.Json;

namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveRoleInfo
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("roleSets")]
        public IEnumerable<GoogleDriveRoleSet> RoleSets { get; set; }
    }
}