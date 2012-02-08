using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using ExtendedPropertiesDocumentationTool.Entities;
using System.Collections.ObjectModel;

namespace ExtendedPropertiesDocumentationTool.DataAccess
{
    public class TableMetadataAccess : MetadataAccessBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="md"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public TableMetadata GetTableMetaDetails(TableMetadata md, string connStr)
        {
            if (md != null)
            {
                ColumnMetadataAccess cma = new ColumnMetadataAccess();

                md.Columns = cma.GetColumnMetadata(md.Name, md.Schema, connStr);

                return md;
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tmd"></param>
        /// <param name="connStr"></param>
        public void SaveTableMetaData(TableMetadata tmd, string connStr)
        {
            if (tmd != null)
            {
                SaveModes sm = SaveModes.New;

                // Figure out if the description is new or if it exists and should be updated.
                if (tmd.IsNewDescription)
                    sm = SaveModes.New;
                else
                    sm = SaveModes.Update;
                // If the Description is empty then we should drop the extended property
                if (string.IsNullOrEmpty(tmd.Description))
                {
                    sm = SaveModes.Delete;
                }
                
                ColumnMetadataAccess cma = new ColumnMetadataAccess();
                cma.SaveColumnMetadata(tmd, connStr);

                if (tmd.HasChanges)
                {
                    SaveDescription(sm, connStr, Level1Types.Table, tmd.Description, tmd.Schema, tmd.Name);
                    tmd.IsNewDescription = false;
                }
                
                
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        internal string GenerateWikiMarkupForTables(string connStr)
        {
            return GenerateWikiMarkupForTables(connStr, null);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="tmd"></param>
        /// <returns></returns>
        internal string GenerateWikiMarkupForTables(string connStr, TableMetadata tmd)
        {
            StringBuilder sb = new StringBuilder();
            string result = string.Empty;
            string sql = @"
                            ;
with InformationSchemaTables as 
(	
	  SELECT		    
		 name as Table_name,
	     SCHEMA_NAME(schema_id ) as Table_Schema
	  FROM 
		 sys.objects 
	  where 
		type = 'u' and is_ms_shipped = 0
)

SELECT 
	WikiMarkup FROM 
	InformationSchemaTables iss
	CROSS APPLY(
		SELECT 'h2. ' + iss.TABLE_SCHEMA + '.' + iss.TABLE_NAME + ' Table' WikiMarkup UNION ALL

		SELECT ISNULL( 
					( SELECT CAST(VALUE AS NVARCHAR(MAX)) FROM fn_listextendedProperty('Description', 
					 'SCHEMA', iss.TABLE_SCHEMA
					 ,'TABLE', iss.TABLE_NAME
					 , NULL, NULL))
			 , '{color:#ff0000}{*}DESCRIPTION MISSING{*}{color}') + ' \\' UNION ALL

		SELECT '||COLUMN_NAME|' +  '|IS_NULLABLE|' +  '|DATA_TYPE|' +  '|CHARACTER_MAXIMUM_LENGTH|' +  '|COLUMN_DEFAULT|' + '|Description|' UNION ALL

		SELECT '|' + ISNULL(COLUMN_NAME,'') + '|' + ISNULL(IS_NULLABLE,'') + '|' + ISNULL(DATA_TYPE, '') + '|' + ISNULL(CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR(50)), 'N/A')  + '|' + ISNULL(COLUMN_DEFAULT, 'none') + '|' + 
				ISNULL(
					(SELECT CAST(VALUE AS NVARCHAR(MAX)) FROM fn_listextendedProperty('Description', 
					 'SCHEMA', iss.TABLE_SCHEMA
					 ,'TABLE', iss.TABLE_NAME
					 , 'COLUMN', Column_Name))
				, '{color:#ff0000}{*}DESCRIPTION MISSING{*}{color}') +' |'

		 FROM INFORMATION_SCHEMA.COLUMNS
		 WHERE TABLE_NAME = iss.TABLE_NAME
	 ) AllTheGoodies
	 
                             ";

            string whereStatement = "";
            if (tmd != null)
            {
                whereStatement = string.Concat("WHERE iss.Table_Name = '", tmd.Name, "' and iss.table_schema = '", tmd.Schema, "'");
                sql = string.Concat(sql, whereStatement);
            }

            sql = string.Concat(sql, "ORDER BY iss.Table_Schema, iss.Table_Name");

            using (SqlCommand cmd = CommandFactory.Create(sql, connStr))
            {
                cmd.Connection.Open();


                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    sb.AppendLine(reader[0].ToString());
                }
            }

            return sb.ToString();
        }


        /// <summary>
        /// Get list of all tables and their Descriptions. Will only include user defined tables and are not ms_shipped
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        internal ObservableCollection<TableMetadata> GetMasterTableMetadata(string connStr)
        {
           ObservableCollection<TableMetadata> tables = new ObservableCollection<TableMetadata>();

            string sql = @"
;
with InformationSchemaTables as 
(	
	  SELECT		    
		 name as Table_name,
	     SCHEMA_NAME(schema_id ) as Table_Schema
	  FROM 
		 sys.objects 
	  where 
		type = 'u' and is_ms_shipped = 0
)

SELECT TABLE_NAME
                                , TABLE_SCHEMA
	                            , VALUE 
	                            , CASE 
                                    WHEN VALUE IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) 
                                    END AS HasDescription
                            FROM
                            InformationSchemaTables iss
                            OUTER APPLY
                            (
	                            SELECT VALUE FROM fn_listextendedProperty('Description', 
		                                'SCHEMA', iss.TABLE_SCHEMA
		                                ,'TABLE', iss.TABLE_NAME
		                                , NULL, NULL)
                            ) descr
                                
                                order by table_schema, table_name";


            using (SqlCommand cmd = CommandFactory.Create(sql, connStr))
            {
                cmd.Connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tables.Add(new TableMetadata(
                        reader.GetString(reader.GetOrdinal("Table_Name")),
                        reader.GetString(reader.GetOrdinal("TABLE_SCHEMA")),
                        reader.GetStringOrEmpty(reader.GetOrdinal("Value")),
                        !reader.GetBoolean(reader.GetOrdinal("HasDescription"))));
                }

            }
            return tables;
        }
    }
}
