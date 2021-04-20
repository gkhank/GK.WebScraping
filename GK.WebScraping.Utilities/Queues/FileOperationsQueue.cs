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
    public class FileOperationsQueue : ApplicationQueueBase<FileOperation>
    {

        #region Singleton
        private static volatile FileOperationsQueue _instance;

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

        public FileOperationsQueue() : base()
        {
            this.Queue = new PriorityQueue<FileOperation>(
                Configuration.Instance.Queues.FileOperationQueue.Capacity,
                new FileOperationComparer());

        }

        protected override async Task<Object> ProcessItem(FileOperation operation)
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

        /// <summary>
        /// Base class already logs the error. This method is for adding extra operations for custom queues.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="item"></param>
        protected override void OnException(Exception ex, FileOperation item)
        { }

        protected override void DisposeItem(FileOperation item)
        {
        }

        protected class FileOperationComparer : IComparer<FileOperation>
        {
            public int Compare(FileOperation p1, FileOperation p2)
            {
                int x = (Int32)p1.Type;
                int y = (Int32)p2.Type;

                if (x == y)
                    return 0;
                else if (x > y)
                    return -1;
                else
                    return 1;
            }
        }

    }
}
