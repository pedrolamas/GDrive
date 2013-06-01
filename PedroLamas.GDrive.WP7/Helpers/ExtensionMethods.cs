using System;
using System.Collections.Generic;
using System.Linq;

namespace PedroLamas.GDrive.Helpers
{
    public static class ExtensionMethods
    {
        public static string ToQueryString(this IDictionary<string, string> dictionary)
        {
            if (dictionary.Count == 0)
            {
                return string.Empty;
            }

            return "?" + string.Join("&", dictionary
                .Select(x => string.Format("{0}={1}", Uri.EscapeUriString(x.Key), Uri.EscapeUriString(x.Value))));
        }
    }
}
