using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtendedPropertiesDocumentationTool.Entities
{
    public class ColumnMetaData : EntityBase
    {
        //public string ColumnName { get; set; }
        //public string Description { get; set; }
        //public bool IsNewDescription { get; set; }
        public int Index { get; set; }

        public ColumnMetaData(string name, string schema, string description, bool isNewDescription, int index) 
            : base(name,schema, description, isNewDescription)
        {
            this.Index = index;
        }

        //public bool HasChanges { get; set; }
    }
}
