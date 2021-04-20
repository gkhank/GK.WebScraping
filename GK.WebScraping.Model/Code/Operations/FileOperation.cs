using System;
using System.Collections.Generic;
using System.Text;

namespace GK.WebScraping.Model.Code.Operations
{
    public class FileOperation
    {

        public Guid OperationID { get; }
        public OperationType Type { get; }
        public string Content { get; }
        public string FullPath { get; }


        private Dictionary<String, Object> _metadata;
        public Dictionary<String, Object> Metadata
        {
            get
            {
                if (this._metadata == null)
                    this._metadata = new Dictionary<String, Object>();

                return this._metadata;
            }
        }

        private FileOperation(String fullPath, OperationType type, String content = null)
        {
            this.OperationID = Guid.NewGuid();
            this.Type = type;
            this.Content = content;
            this.FullPath = fullPath;
        }

        public static FileOperation Create(String fullPath, OperationType type, String content = null)
        {
            return new FileOperation(fullPath, type, content);
        }

        public enum OperationType
        {
            CreateOrUpdate,
            Read
        }
    }
}
