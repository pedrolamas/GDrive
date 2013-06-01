using System;
using System.Collections.Generic;
using PedroLamas.GDrive.Service;
using PedroLamas.ServiceModel;

namespace PedroLamas.GDrive.Model
{
    public interface IMainModel
    {
        IList<AccountModel> AvailableAccounts { get; }

        AccountModel CurrentAccount { get; set; }

        bool ExecuteInitialLoad { get; set; }

        GoogleDriveFile CurrentFolder { get; }

        string CurrentFolderId { get; }

        string CurrentPath { get; }

        void CheckTokenAndExecute<T>(Action<GoogleAuthToken, ResultCallback<T>, object> action, ResultCallback<T> callback, object state);

        void Clear();

        void Push(GoogleDriveFile item);

        bool TryPeek(out GoogleDriveFile item);

        bool TryPop(out GoogleDriveFile item);

        void Save();
    }
}