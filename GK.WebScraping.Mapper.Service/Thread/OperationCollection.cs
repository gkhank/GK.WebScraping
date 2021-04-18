using System;
using System.Collections.Generic;
using System.Text;

namespace GK.WebScraping.Mapper.Service.Thread
{
    public static class OperationCollection<Type>
    {
        private static readonly object _lock = new object();
        private static volatile HashSet<Type> _ongoing = null;
        public static HashSet<Type> OngoingProcesses
        {
            get
            {
                if (_ongoing == null)
                    lock (_lock)
                        if (_ongoing == null)
                            _ongoing = new HashSet<Type>();
                return _ongoing;
            }
        }

        internal static void MarkAsCompleted(params Type[] items)
        {
            lock (_lock)
            {
                foreach (var item in items)
                    OngoingProcesses.Remove(item);
            }
        }

        internal static void MarkAsOngoing(params Type[] items)
        {
            lock (_lock)
                foreach (var item in items)
                    OngoingProcesses.Add(item);
        }
    }
}

