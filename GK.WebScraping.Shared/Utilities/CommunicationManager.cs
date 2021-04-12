using GK.WebScraping.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GK.WebScraping.Shared.Utilities
{
    public class CommunicationManager
    {
        private static CommunicationManager instance = null;
        private static readonly object _lock = new object();
        private readonly Dictionary<String, IStore> _handlers;

        public CommunicationManager()
        {
            this._handlers = new Dictionary<string, IStore>();
            this.Init();
        }

        private void Init()
        {
            WebScrapingContext
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

        public Boolean TryGetStore(String name, out IStore store)
        {
            return this._handlers.TryGetValue(name, out store);
        }

        public String[] GetAllStores()
        {
            return this._handlers.Keys.ToArray();
        }
    }
}
