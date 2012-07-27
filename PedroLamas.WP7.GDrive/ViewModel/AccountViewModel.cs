using PedroLamas.WP7.GDrive.Model;

namespace PedroLamas.WP7.GDrive.ViewModel
{
    public class AccountViewModel
    {
        #region Properties

        public AccountModel Model { get; private set; }

        public string Name
        {
            get
            {
                return Model.Name;
            }
        }

        #endregion

        public AccountViewModel(AccountModel model)
        {
            Model = model;
        }
    }
}