using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using DatabaseMetadata.Entities;
using System.Data.Common;
using ExtendedPropertiesDocumentationTool;

namespace DatabaseMetadata.DataAccess
{
    public class ColumnMetadataAccess : MetadataAccessBase
    {
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tableSchema"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public ObservableCollection<ColumnMetaData> GetColumnMetadata(string tableName, string tableSchema, string connStr, Level1Types belongToType)
        {
            int indexCount = 0;
            ObservableCollection<ColumnMetaData> columns = new ObservableCollection<ColumnMetaData>();

            string sql = string.Format(@"SELECT 
	column_name 
	, VALUE
	, CASE WHEN VALUE IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END AS HasDescription
FROM 
	INFORMATION_SCHEMA.COLUMNS iss
OUTER APPLY
(
	SELECT VALUE FROM fn_listextendedProperty('{3}', 
		 'SCHEMA', iss.TABLE_SCHEMA
		 ,'{2}', iss.TABLE_NAME
		 , 'COLUMN', iss.Column_Name)

) descr
WHERE 
	table_name = '{0}' 
	AND table_schema = '{1}'", tableName, tableSchema, belongToType, ApplicationSettings.Default.ExtendedPropKey);


            using (DbCommand cmd = CommandFactory.Create(sql, connStr))
            {
                cmd.Connection.Open();

                DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    columns.Add(new ColumnMetaData(
                        tableName,
                        reader.GetString(reader.GetOrdinal("column_name")),
                        tableSchema, 
                        reader.GetStringOrEmpty(reader.GetOrdinal("value")), 
                        !reader.GetBoolean(reader.GetOrdinal("HasDescription")), 
                        indexCount++,
                        belongToType));
                }
                reader.Close();
            }

            return columns;
        }

    }
}
