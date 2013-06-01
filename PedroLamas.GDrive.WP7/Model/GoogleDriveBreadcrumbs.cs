using System.Collections.Generic;
using System.Linq;
using PedroLamas.GDrive.Service;

namespace PedroLamas.GDrive.Model
{
    public class GoogleDriveBreadcrumbs
    {
        private readonly Stack<GoogleDriveFile> _stack = new Stack<GoogleDriveFile>();
        private readonly object _lock = new object();

        public string CurrentPath
        {
            get
            {
                return "/" + string.Join("/", _stack
                    .Select(x => x.Title)
                    .Reverse()
                    .ToArray());
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _stack.Clear();
            }
        }

        public void Push(GoogleDriveFile item)
        {
            lock (_lock)
            {
                _stack.Push(item);
            }
        }

        public bool TryPeek(out GoogleDriveFile item)
        {
            lock (_lock)
            {
                if (_stack.Count == 0)
                {
                    item = null;

                    return false;
                }

                item = _stack.Peek();

                return true;
            }
        }

        public bool TryPop(out GoogleDriveFile item)
        {
            lock (_lock)
            {
                if (_stack.Count == 0)
                {
                    item = null;

                    return false;
                }

                item = _stack.Pop();

                return true;
            }
        }
    }
}