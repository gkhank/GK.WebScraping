using System;
using System.Collections.Generic;
using System.Text;

namespace GK.WebScraping.Model
{
    public enum ProcessUpdateType
    {
        Found,
        EndingThread,
        Error,
        UpdateRunCount,
        ThreadStopped,
        StartingThread,
        LoopStarting
    }

    public enum StatusType
    {
        None = 0,
        Active = 1,
        Deleted = 16
    }

    public enum MapStatusType
    {
        None,
        ContentReady,
        LinksRead,
        Mapped
    }

    public enum PriorityType
    {
        High = 1,
        Normal = 2,
        Low = 3
    }
}
