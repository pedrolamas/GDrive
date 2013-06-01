using System.Collections.Generic;

namespace PedroLamas.GDrive.Service
{
    public class GoogleClientSettings : IGoogleClientSettings
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public IEnumerable<string> Scopes { get; set; }
    }
}