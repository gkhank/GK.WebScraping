using System;
using System.Collections.Generic;
using System.Text;

namespace GK.WebScraping.Model
{
    public class DatabaseProcessKey
    {
        public Guid ThreadID { get; set; }
        public DatabaseQueuePriorityType Priority { get; set; }

        public DatabaseProcessKey(Guid threadID, DatabaseQueuePriorityType priorityType)
        {
            this.ThreadID = threadID;
            this.Priority = priorityType;
        }
    }
}
