using LagoVista.Core.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.ContentManager.App.Models
{
    public class TextResource : TableEntity
    {
        public string Status { get; set; }
        public string Notes { get; set; }
        public string Text { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        public bool Edits { get; set; }

        public string Issues { get; set; }

        public bool Reviewed { get; set; }

        public bool Approved { get; set; }
    }

    public class TextResourceCopy : TableEntity
    {
        public string Notes { get; set; }
        public string Text { get; set; }
        public string OriginalText { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastUpdatedDate { get; set; }
        
        public bool Merged { get; set; }
    }
}
