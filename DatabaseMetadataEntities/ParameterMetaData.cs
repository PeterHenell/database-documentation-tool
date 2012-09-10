using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseMetadata.Entities
{
    public partial class ParameterMetaData : EntityBase
    {
        public int Index { get; set; }

        public ParameterMetaData(string level1Name, string level2Name, string schema, string description, bool isNewDescription, int index)
            : base(level1Name, level2Name, schema, description, isNewDescription)
        {
            this.Index = index;
            this.Level1Type = Level1Types.Procedure;
            this.Level2Type = Level2Types.Parameter;
        }
    }
}
