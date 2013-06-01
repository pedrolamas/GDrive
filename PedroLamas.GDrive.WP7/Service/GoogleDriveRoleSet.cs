using System.Collections.Generic;
using Newtonsoft.Json;

namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveRoleSet
    {
        [JsonProperty("primaryRole")]
        public string PrimaryRole { get; set; }

        [JsonProperty("additionalRoles")]
        public IEnumerable<string> AdditionalRoles { get; set; }
    }
}