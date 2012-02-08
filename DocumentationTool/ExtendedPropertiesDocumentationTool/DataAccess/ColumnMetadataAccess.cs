using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using ExtendedPropertiesDocumentationTool.Entities;

namespace ExtendedPropertiesDocumentationTool.DataAccess
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
        internal ObservableCollection<ColumnMetaData> GetColumnMetadata(string tableName, string tableSchema, string connStr)
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
	SELECT VALUE FROM fn_listextendedProperty('Description', 
		 'SCHEMA', iss.TABLE_SCHEMA
		 ,'TABLE', iss.TABLE_NAME
		 , 'COLUMN', iss.Column_Name)

) descr
WHERE 
	table_name = '{0}' 
	AND table_schema = '{1}'", tableName, tableSchema);


            using (SqlCommand cmd = CommandFactory.Create(sql, connStr))
            {
                cmd.Connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    columns.Add(new ColumnMetaData(
                        reader.GetString(reader.GetOrdinal("column_name")), 
                        tableSchema, 
                        reader.GetStringOrEmpty(reader.GetOrdinal("value")), 
                        !reader.GetBoolean(reader.GetOrdinal("HasDescription")), 
                        indexCount++));
                }
                reader.Close();
            }

            return columns;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tmd"></param>
        /// <param name="sqlAdd"></param>
        /// <param name="sqlUpdate"></param>
        /// <param name="sqlDrop"></param>
        /// <param name="cmd"></param>
        internal void SaveColumnMetadata(TableMetadata tmd, string connStr)
        {
            foreach (ColumnMetaData item in tmd.Columns)
            {
                SaveModes sm = SaveModes.New;
                try
                {
                    if (item.IsNewDescription)
                        sm = SaveModes.New;
                    else
                        sm = SaveModes.Update;

                    if (string.IsNullOrEmpty(item.Description))
                    {
                        sm = SaveModes.Delete;
                    }

                    if (item.HasChanges)
                    {
                        SaveDescription(sm, connStr, Level1Types.Table, item.Description, tmd.Schema, tmd.Name, Level2Types.Column, item.Name);
                        item.IsNewDescription = false;
                    }
                    
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }

            }
        }

    }
}
