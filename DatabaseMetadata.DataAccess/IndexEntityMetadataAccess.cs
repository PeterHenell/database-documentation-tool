using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using DatabaseMetadata.Entities;
using System.Data.Common;

namespace DatabaseMetadata.DataAccess
{
    public class IndexEntityMetadataAccess : MetadataAccessBase
    {
        public ObservableCollection<IndexEntity> GetIndexMetadata(string tableName, string tableSchema, string connStr)
        {
            ObservableCollection<IndexEntity> indexes = new ObservableCollection<IndexEntity>();

            string sql = string.Format(@"
SELECT  
    name ,
    index_id ,
    type_desc ,
    is_unique ,
    is_primary_key ,
    is_unique_constraint ,
    fill_factor ,
    has_filter ,
    filter_definition,
    VALUE, 
	CASE WHEN VALUE IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END AS HasDescription
FROM 
	sys.indexes 
OUTER APPLY
(
	SELECT VALUE FROM FN_LISTEXTENDEDPROPERTY('Description', 
		 'SCHEMA', '{1}'
		 ,'TABLE', '{0}'
		 ,CASE is_unique WHEN 1 THEN 'CONSTRAINT' ELSE 'INDEX' END, name)

) descr
WHERE 
	object_id = OBJECT_ID('{1}.{0}')
    AND name IS NOT NULL	 
", tableName, tableSchema);


            using (DbCommand cmd = CommandFactory.Create(sql, connStr))
            {
                cmd.Connection.Open();

                DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    indexes.Add(new IndexEntity(
                        tableName,
                        reader.GetString(reader.GetOrdinal("name")),
                        tableSchema,
                        reader.GetStringOrEmpty(reader.GetOrdinal("value")),
                        !reader.GetBoolean(reader.GetOrdinal("HasDescription")),
                        reader.GetInt32(reader.GetOrdinal("index_id")),
                        reader.GetString(reader.GetOrdinal("type_desc")),
                        reader.GetBoolean(reader.GetOrdinal("is_unique")),
                        reader.GetBoolean(reader.GetOrdinal("is_primary_key")),
                        reader.GetBoolean(reader.GetOrdinal("is_unique_constraint")),
                        reader.GetByte(reader.GetOrdinal("fill_factor")),
                        reader.GetBoolean(reader.GetOrdinal("has_filter")),
                        reader.GetStringOrEmpty(reader.GetOrdinal("filter_definition"))
                    ));

                }
                reader.Close();
            }

            return indexes;
        }

    }
}
