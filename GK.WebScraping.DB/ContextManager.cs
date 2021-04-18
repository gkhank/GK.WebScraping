using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;

namespace GK.WebScraping.DB
{
    public class ContextManager
    {
        /// <summary>
        /// Transaction queue holds all transactions to be processed in order.
        /// Key: ThreadID,
        /// Value : Transaction
        /// </summary>
        private static Queue<KeyValuePair<Guid, IDbContextTransaction>> _transactionQueue;

        private static Dictionary<Guid, WebScrapingContext> _activeContexts;
            -
        private static object _lock = new object();

        public ContextManager()
        {
            _transactionQueue = new Queue<KeyValuePair<Guid, IDbContextTransaction>>();
            _activeContexts = new Dictionary<Guid, WebScrapingContext>();
        }

        public void QueueTransaction(Guid threadID, IDbContextTransaction transaction)
        {
            if (_activeContexts.ContainsKey(threadID))
                throw new Exception("No active context could be found for this thread. Please initiate database context before queuing a transaction");

            _transactionQueue.Enqueue(new KeyValuePair<Guid, IDbContextTransaction>(threadID, transaction));
        }
    }
}
