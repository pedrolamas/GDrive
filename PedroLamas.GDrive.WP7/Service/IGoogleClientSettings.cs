using System.Collections.Generic;

namespace PedroLamas.GDrive.Service
{
    public interface IGoogleClientSettings
    {
        string ClientId { get; }

        string ClientSecret { get; }

        IEnumerable<string> Scopes { get; }
    }
}