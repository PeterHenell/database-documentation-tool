using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DatabaseMetadata.Entities
{
    public partial class ViewMetadata : EntityBase
    {
        public System.Collections.ObjectModel.ObservableCollection<ColumnMetaData> Columns { get; set; }



        public override string ToString()
        {
            return string.Format("{0}.{1}", Schema, Level1Name);
        }

        public ViewMetadata(string level1Name, string level2Name, string schema, string description, bool isNewDescription)
            : base(level1Name, level2Name, schema, description, isNewDescription)
        {
            Columns = new ObservableCollection<ColumnMetaData>();

            this.Level1Type = Level1Types.View;
            this.Level2Type = Level2Types.NULL;
        }
    }
}
