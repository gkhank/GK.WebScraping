using GK.WebScraping.DB;
using GK.WebScraping.Model;
using GK.WebScraping.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GK.WebScraping.Utilities
{
    public class CommunicationManager
    {
        private static CommunicationManager instance = null;
        private static readonly object _lock = new object();
        private readonly Dictionary<Guid, String> _stores;

        public CommunicationManager()
        {
            this._stores = new Dictionary<Guid, String>();
            this.Init();
        }

        private void Init()
        {
            Store[] stores = DatabaseManager.Instance.Stores.ToArray();
            foreach (Store s in stores)
            {
                this._stores.Add(s.StoreId, s.Name);
            }
        }

        public static CommunicationManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new CommunicationManager();
                    }
                    return instance;
                }
            }
        }

        public Boolean TryGetStore(Guid key, out String storeName)
        {
            return this._stores.TryGetValue(key, out storeName);
        }

        public String[] GetAllStores()
        {
            return this._stores.Values.ToArray();
        }
    }
}
