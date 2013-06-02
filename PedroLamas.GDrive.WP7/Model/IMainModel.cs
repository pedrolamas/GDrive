using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PedroLamas.GDrive.Service;

namespace PedroLamas.GDrive.Model
{
    public interface IMainModel
    {
        IList<AccountModel> AvailableAccounts { get; }

        AccountModel CurrentAccount { get; set; }

        bool ExecuteInitialLoad { get; set; }

        GoogleDriveFile CurrentFolder { get; }

        GoogleDriveFile SelectedFile { get; set; }

        string CurrentFolderId { get; }

        string CurrentPath { get; }

        Task CheckToken(CancellationToken cancellationToken);

        void Clear();

        void Push(GoogleDriveFile item);

        bool TryPeek(out GoogleDriveFile item);

        bool TryPop(out GoogleDriveFile item);

        void Save();
    }
}