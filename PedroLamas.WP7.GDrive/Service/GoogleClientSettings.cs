using System.Collections.Generic;

namespace PedroLamas.WP7.GDrive.Service
{
    public class GoogleClientSettings : IGoogleClientSettings
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public IEnumerable<string> Scopes { get; set; }
    }
}