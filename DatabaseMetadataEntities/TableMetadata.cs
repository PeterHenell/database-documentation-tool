using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DatabaseMetadata.Entities
{
    public partial class TableMetadata : EntityBase
    {
        public System.Collections.ObjectModel.ObservableCollection<ColumnMetaData> Columns { get; set; }


        ObservableCollection<IndexEntity> _indexes;
        public ObservableCollection<IndexEntity> Indexes
        {
            get
            {
                return _indexes;
            }
            set
            {
                if (_indexes != value)
                {
                    _indexes = value;
                    OnPropertyChanged("Indexes");
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Schema, Level1Name);
        }

        public TableMetadata(string level1Name, string level2Name, string schema, string description, bool isNewDescription)
            : base(level1Name, level2Name, schema, description, isNewDescription)
        {
            Columns = new ObservableCollection<ColumnMetaData>();
            Indexes = new ObservableCollection<IndexEntity>();

            this.Level1Type = Level1Types.Table;
            this.Level2Type = Level2Types.NULL;
        }
    }
}
