using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ExtendedPropertiesDocumentationTool.Entities
{
    public class StoredProcedureMetaData : EntityBase
    {
     
        System.Collections.ObjectModel.ObservableCollection<ColumnMetaData> _parameters = new ObservableCollection<ColumnMetaData>();
        public System.Collections.ObjectModel.ObservableCollection<ColumnMetaData> Parameters { get { return _parameters; } set { _parameters = value; } }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Schema, Name);
        }

        public StoredProcedureMetaData(string name, string schema, string description, bool isNewDescription)
            : base(name, schema, description, isNewDescription)
        {

        }
    }
}
