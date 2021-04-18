using System;
using System.Collections.Generic;
using System.Text;

namespace GK.WebScraping.Model
{
    public class DatabaseProcessKey
    {
        public Guid OperationID { get; set; }
        public DatabaseQueuePriorityType Priority { get; set; }

        private DatabaseProcessKey(DatabaseQueuePriorityType priorityType)
        {
            this.OperationID = Guid.NewGuid();
            this.Priority = priorityType;
        }

        public static DatabaseProcessKey GenerateKey(DatabaseQueuePriorityType priorityType)
        {
            return new DatabaseProcessKey(priorityType);
        }

    }
}
