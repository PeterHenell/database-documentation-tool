using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseMetadata.Entities
{
    public partial class ColumnMetaData : EntityBase
    {
        public int Index { get; set; }

        public ColumnMetaData(string level1Name,string level2Name, string schema, string description, bool isNewDescription, int index, Level1Types belongToType)
            : base(level1Name, level2Name, schema, description, isNewDescription)
        {
            this.Index = index;
            this.Level1Type = belongToType;
            this.Level2Type = Level2Types.Column;
        }
    }
}
