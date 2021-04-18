using System;
using System.Collections.Generic;
using System.Text;

namespace GK.WebScraping.Mapper.Service.Thread
{
    public static  class OperationCollection<Type>
    {
        private static readonly object _lock = new object();
        private static volatile HashSet<Type> _ongoing = null;
        public static HashSet<Type> OngoingProcesses
        {
            get
            {
                lock (_lock)
                {
                    if (_ongoing == null)
                    {
                        _ongoing = new HashSet<Type>();
                    }
                    return _ongoing;
                }
            }
        }


        private static volatile HashSet<Type> _completed = null;
        public static HashSet<Type> CompletedProcesses
        {
            get
            {
                lock (_lock)
                {
                    if (_completed == null)
                    {
                        _completed = new HashSet<Type>();
                    }
                    return _completed;
                }
            }
        }

        private static void InnerMarkAsFinished(Type item)
        {
            CompletedProcesses.Add(item);
            OngoingProcesses.Remove(item);
        }

        private static void InnerMarkAsOngoing(Type item)
        {
            OngoingProcesses.Add(item);
        }

        internal static void MarkAsCompleted(params Type[] items)
        {
            foreach (var item in items)
                InnerMarkAsFinished(item);
        }

        internal static void MarkAsOngoing(params Type[] items)
        {
            foreach (var item in items)
                InnerMarkAsOngoing(item);
        }

        internal static void DisposeCompleted() {
            OperationCollection<Type>.CompletedProcesses.Clear();
        }
    }
}
