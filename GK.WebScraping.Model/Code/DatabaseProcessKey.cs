using System;
using System.Collections.Generic;
using System.Text;

namespace GK.WebScraping.Model
{
    public class DatabaseProcessKey
    {
        public Guid OperationID { get; set; }
        public PriorityType Priority { get; set; }

        private DatabaseProcessKey(PriorityType priorityType)
        {
            this.OperationID = Guid.NewGuid();
            this.Priority = priorityType;
        }

        public static DatabaseProcessKey GenerateKey(PriorityType priorityType)
        {
            return new DatabaseProcessKey(priorityType);
        }

    }
}
