using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ExtendedPropertiesDocumentationTool.Entities
{
    public class TableMetadata : EntityBase
    {
        public System.Collections.ObjectModel.ObservableCollection<ColumnMetaData> Columns { get; set; }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Schema, Name);
        }

        public TableMetadata(string name, string schema, string description, bool isNewDescription)
            : base(name, schema, description, isNewDescription)
        {
            Columns = new ObservableCollection<ColumnMetaData>();
        }
    }
}
