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
}
