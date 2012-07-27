using System;
using System.Collections.Generic;
using PedroLamas.WP7.GDrive.Service;
using PedroLamas.WP7.ServiceModel;

namespace PedroLamas.WP7.GDrive.Model
{
    public interface IMainModel
    {
        IList<AccountModel> AvailableAccounts { get; }

        AccountModel CurrentAccount { get; set; }

        bool ExecuteInitialLoad { get; set; }

        void CheckTokenAndExecute<T>(Action<GoogleAuthToken, ResultCallback<T>, object> action, ResultCallback<T> callback, object state);

        void Save();
    }
}