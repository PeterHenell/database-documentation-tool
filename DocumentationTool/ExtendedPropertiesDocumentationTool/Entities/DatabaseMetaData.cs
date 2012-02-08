using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ExtendedPropertiesDocumentationTool.Entities
{
    public class DatabaseMetaData
    {

        public ObservableCollection<TableMetadata> Tables { get; set; }
        public ObservableCollection<StoredProcedureMetaData> StoredProcedures { get; set; }

        public DatabaseMetaData()
        {
            Tables = new ObservableCollection<TableMetadata>();
            StoredProcedures = new ObservableCollection<StoredProcedureMetaData>();
        }


    }
}
