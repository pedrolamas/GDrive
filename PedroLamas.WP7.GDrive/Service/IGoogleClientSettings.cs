using System.Collections.Generic;

namespace PedroLamas.WP7.GDrive.Service
{
    public interface IGoogleClientSettings
    {
        string ClientId { get; }

        string ClientSecret { get; }

        IEnumerable<string> Scopes { get; }
    }
}