using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DatabaseMetadata.Entities
{
    public partial class StoredProcedureMetaData : EntityBase
    {

        System.Collections.ObjectModel.ObservableCollection<ParameterMetaData> _parameters = new ObservableCollection<ParameterMetaData>();
        public System.Collections.ObjectModel.ObservableCollection<ParameterMetaData> Parameters { get { return _parameters; } set { _parameters = value; } }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Schema, Level1Name);
        }

        public StoredProcedureMetaData(string level1Name, string level2Name, string schema, string description, bool isNewDescription)
            : base(level1Name, level2Name, schema, description, isNewDescription)
        {
            this.Level1Type = Level1Types.Procedure;
            this.Level2Type = Level2Types.NULL;
        }
    }
}
