using GK.WebScraping.Model.Code.Collections.GK.WebScraping.Model.Collection;
using GK.WebScraping.Model.Code.Operations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GK.WebScraping.Utilities.Queues
{
    public class FileOperationsQueue : OperationQueueBase<FileOperation>
    {

        #region Singleton
        private static volatile FileOperationsQueue _instance;
        public override int NumberOfMaximumThreads => 3;

        private HashSet<Guid> _operatingFiles;

        public static FileOperationsQueue Instance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new FileOperationsQueue();

                return _instance;
            }
        }
        #endregion

        public FileOperationsQueue() : base(new FileOperationComparer())
        {
            this._operatingFiles = new HashSet<Guid>();
        }

        public override void Enqueue(FileOperation item)
        {
            while (this._operatingFiles.Contains(item.OperationID))
            { }

            this._operatingFiles.Add(item.OperationID);
            base.Enqueue(item);
        }

        public override void Enqueue(FileOperation item, Action<FileOperation, object> callback)
        {
            this._operatingFiles.Add(item.OperationID);
            base.Enqueue(item, callback);
        }

        protected override async Task<object> ProcessItem(FileOperation operation)
        {
            Boolean isSuccessful = false;
            switch (operation.Type)
            {
                case FileOperation.OperationType.CreateOrUpdate:

                    using (FileStream fs = new FileStream(operation.FullPath, FileMode.OpenOrCreate))
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        await writer.WriteAsync(operation.Content);
                    }

                    isSuccessful = true;
                    break;

                case FileOperation.OperationType.Read:
                    return File.ReadAllText(operation.FullPath);

                default:
                    throw new NotImplementedException();
            }
            return isSuccessful;
        }

        protected override void DisposeItem(FileOperation item)
        {
            this._operatingFiles.Remove(item.OperationID);
        }

        /// <summary>
        /// Base class already logs the error. This method is for adding extra operations for custom queues.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="item"></param>
        protected override void OnException(Exception ex, FileOperation item)
        { }

        protected class FileOperationComparer : IComparer<FileOperation>
        {
            public int Compare(FileOperation p1, FileOperation p2)
            {
                int x = (Int32)p1.Type;
                int y = (Int32)p2.Type;

                if (x == y)
                    return 0;
                else if (x > y)
                    return 1;
                else
                    return -1;
            }
        }

    }
}
