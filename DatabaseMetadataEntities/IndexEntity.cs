using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseMetadata.Entities
{
    public partial class IndexEntity : EntityBase
    {

        public IndexEntity(string level1Name, string level2Name, string schema, string description, bool isNewDescription
            , int indexId, string typeDescription, bool isUnique, bool isPrimaryKey
            , bool isUniqueConstraint, byte fillFactor, bool hasFilter, string filterDefinition )

            : base(level1Name, level2Name, schema, description, isNewDescription)
        {
            IndexId = indexId;
            TypeDescription = typeDescription;
            IsPrimaryKey = isPrimaryKey;
            IsUnique = isUnique;
            IsUniqueConstraint = isUniqueConstraint;
            HasFilter = hasFilter;
            FillFactor = fillFactor;
            FilterDefinition = filterDefinition;

            this.Level1Type = Level1Types.Table;
            //this.Level2Type = isUnique ? Level2Types.Constraint : Level2Types.Index;
            this.Level2Type = Level2Types.Index;
        }

        int _id;
        public int IndexId
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("IndexId");
                }
            }
        }

        string _typeDescription;
        public string TypeDescription
        {
            get
            {
                return _typeDescription;
            }
            set
            {
                if (_typeDescription != value)
                {
                    _typeDescription = value;
                    OnPropertyChanged("TypeDescription");
                }
            }
        }

        bool _isUnique;
        public bool IsUnique
        {
            get
            {
                return _isUnique;
            }
            set
            {
                if (_isUnique != value)
                {
                    _isUnique = value;
                    OnPropertyChanged("IsUnique");
                }
            }
        }

        bool _isPrimaryKey;
        public bool IsPrimaryKey
        {
            get
            {
                return _isPrimaryKey;
            }
            set
            {
                if (_isPrimaryKey != value)
                {
                    _isPrimaryKey = value;
                    OnPropertyChanged("IsPrimaryKey");
                }
            }
        }

        bool _isUniqueConstraint;
        public bool IsUniqueConstraint
        {
            get
            {
                return _isUniqueConstraint;
            }
            set
            {
                if (_isUniqueConstraint != value)
                {
                    _isUniqueConstraint = value;
                    OnPropertyChanged("IsUniqueConstraint");
                }
            }
        }

        byte _fillFactor;
        public byte FillFactor
        {
            get
            {
                return _fillFactor;
            }
            set
            {
                if (_fillFactor != value)
                {
                    _fillFactor = value;
                    OnPropertyChanged("FillFactor");
                }
            }
        }

        bool _hasFilter;
        public bool HasFilter
        {
            get
            {
                return _hasFilter;
            }
            set
            {
                if (_hasFilter != value)
                {
                    _hasFilter = value;
                    OnPropertyChanged("HasFilter");
                }
            }
        }

        string _filerDefinition;
        public string FilterDefinition
        {
            get
            {
                return _filerDefinition;
            }
            set
            {
                if (_filerDefinition != value)
                {
                    _filerDefinition = value;
                    OnPropertyChanged("FilterDefinition");
                }
            }
        }
    }
}
