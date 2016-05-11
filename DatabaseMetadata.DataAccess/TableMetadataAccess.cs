using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DatabaseMetadata.Entities;
using System.Collections.ObjectModel;
using System.Data.Common;
using ExtendedPropertiesDocumentationTool;

namespace DatabaseMetadata.DataAccess
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
                IndexEntityMetadataAccess ima = new IndexEntityMetadataAccess();

                md.Columns = cma.GetColumnMetadata(md.Level1Name, md.Schema, connStr, Level1Types.Table);
                md.Indexes = ima.GetIndexMetadata(md.Level1Name, md.Schema, connStr);

                return md;
            }
            return null;
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="tmd"></param>
        ///// <param name="connStr"></param>
        //public void SaveTableMetaData(TableMetadata tmd, string connStr)
        //{
        //    if (tmd != null)
        //    {
        //        SaveMetadata(tmd, connStr);
        //        ColumnMetadataAccess cma = new ColumnMetadataAccess();
        //        cma.SaveColumnMetadata(tmd, connStr);

        //        IndexEntityMetadataAccess iema = new IndexEntityMetadataAccess();
        //        iema.SaveIndexMetadata(tmd, connStr);
                
        //    }




        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="connStr"></param>
        ///// <returns></returns>
        //public string GenerateWikiMarkupForTables(string connStr)
        //{
        //    return GenerateWikiMarkupForTables(connStr, null);
        //}



       


        /// <summary>
        /// Get list of all tables and their Descriptions. Will only include user defined tables and are not ms_shipped
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public ObservableCollection<TableMetadata> GetMasterTableMetadata(string connStr)
        {
           ObservableCollection<TableMetadata> tables = new ObservableCollection<TableMetadata>();

            string sql = string.Format(@"
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
	                            SELECT VALUE FROM fn_listextendedProperty('{0}', 
		                                'SCHEMA', iss.TABLE_SCHEMA
		                                ,'TABLE', iss.TABLE_NAME
		                                , NULL, NULL)
                            ) descr
                                
                                order by table_schema, table_name", ApplicationSettings.Default.ExtendedPropKey);


            using (DbCommand cmd = CommandFactory.Create(sql, connStr))
            {
                cmd.Connection.Open();
                DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tables.Add(new TableMetadata(
                        reader.GetString(reader.GetOrdinal("Table_Name")),
                        "NULL",
                        reader.GetString(reader.GetOrdinal("TABLE_SCHEMA")),
                        reader.GetStringOrEmpty(reader.GetOrdinal("Value")),
                        !reader.GetBoolean(reader.GetOrdinal("HasDescription"))));
                }

            }
            return tables;
        }
    }
}
