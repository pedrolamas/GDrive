using System.Collections.Generic;
using Newtonsoft.Json;
using PedroLamas.WP7.GDrive.Service;

namespace PedroLamas.WP7.GDrive.Model
{
    public class AccountModel
    {
        #region Properties

        private GoogleDriveAbout _info;

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public GoogleAuthToken AuthToken { get; set; }

        [JsonProperty]
        public GoogleDriveAbout Info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;

                if (value != null)
                {
                    Root = new GoogleDriveChild()
                    {
                        Id = value.RootFolderId
                    };
                }
            }
        }

        [JsonIgnore]
        public IDictionary<string, GoogleDriveFile> Files { get; set; }

        [JsonIgnore]
        public GoogleDriveChild Root { get; set; }

        [JsonIgnore]
        public Stack<GoogleDriveChild> PathBreadcrumbs { get; private set; }

        [JsonIgnore]
        public GoogleDriveChild CurrentFolder
        {
            get
            {
                return PathBreadcrumbs.Count == 0 ? Root : PathBreadcrumbs.Peek();
            }
        }

        #endregion

        public AccountModel(string name, GoogleAuthToken authToken)
        {
            Name = name;
            AuthToken = authToken;

            Files = new Dictionary<string, GoogleDriveFile>();
            PathBreadcrumbs = new Stack<GoogleDriveChild>();
        }
    }
}