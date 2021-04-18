using GK.WebScraping.Model;
using GK.WebScraping.Model.Code.Collections.GK.WebScraping.Model.Collection;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;

namespace GK.WebScraping.DB
{
    public class ContextManager
    {
        /// <summary>
        /// Transaction queues holds all transactions to be processed in order. Write transactions should always be prioritised. Therefore we are implementing a custom comparer to prioritise those.
        /// Key: ThreadID,
        /// Value : Transaction
        /// </summary>
        private static PriorityQueue<KeyValuePair<DatabaseProcessKey, IDbContextTransaction>> _queue;
        private static Dictionary<Guid, WebScrapingContext> _activeContexts;
        private static object _lock = new object();

        public ContextManager()
        {
            _queue = new PriorityQueue<KeyValuePair<DatabaseProcessKey, IDbContextTransaction>>(new DatabasePriorityComparer());
            _activeContexts = new Dictionary<Guid, WebScrapingContext>();
        }

        public void QueueRead(DatabaseProcessKey key, IDbContextTransaction transaction)
        {
            if (_activeContexts.ContainsKey(key.ThreadID))
                throw new Exception("No active context could be found for this thread. Please define database context before queuing a transaction");
            _queue.Enqueue(new KeyValuePair<DatabaseProcessKey, IDbContextTransaction>(key, transaction));
        }

        public void InitContext(DatabaseProcessKey key)
        {
            if (_activeContexts.ContainsKey(key.ThreadID))
                throw new Exception("There is an active context for thread '{0}'. Please make sure you are not calling this method multiple times");
            _activeContexts.Add(key.ThreadID, new WebScrapingContext());
        }

        public Boolean ProcessNext()
        {
            if (_queue.Count > 0)
            {
                var processing = _queue.NextItem;

                return true;
            }

            return false;
        }
    }

    class DatabasePriorityComparer : IComparer<KeyValuePair<DatabaseProcessKey, IDbContextTransaction>>
    {
        public int Compare(KeyValuePair<DatabaseProcessKey, IDbContextTransaction> p1, KeyValuePair<DatabaseProcessKey, IDbContextTransaction> p2)
        {
            var isFirstPrio = p1.Key.Priority.Equals(DatabaseQueuePriorityType.Write);
            var isSecondPrio = p2.Key.Priority.Equals(DatabaseQueuePriorityType.Write);


            if (isFirstPrio == isSecondPrio)
            {
                return 0;
            }
            else if (isFirstPrio)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}
