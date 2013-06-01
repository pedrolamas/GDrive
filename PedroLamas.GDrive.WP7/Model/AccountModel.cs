using Newtonsoft.Json;
using PedroLamas.GDrive.Service;

namespace PedroLamas.GDrive.Model
{
    public class AccountModel
    {
        #region Properties

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public GoogleAuthToken AuthToken { get; set; }

        [JsonProperty]
        public GoogleDriveAbout Info { get; set; }

        #endregion

        public AccountModel()
        {
        }

        public AccountModel(string name, GoogleAuthToken authToken)
        {
            Name = name;
            AuthToken = authToken;
        }
    }
}