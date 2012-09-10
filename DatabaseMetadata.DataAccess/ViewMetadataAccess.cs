using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DatabaseMetadata.Entities;
using System.Collections.ObjectModel;
using System.Data.Common;

namespace DatabaseMetadata.DataAccess
{
    public class ViewMetadataAccess : MetadataAccessBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="md"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public ViewMetadata GetViewMetaDetails(ViewMetadata md, string connStr)
        {
            if (md != null)
            {
                ColumnMetadataAccess cma = new ColumnMetadataAccess();

                md.Columns = cma.GetColumnMetadata(md.Level1Name, md.Schema, connStr, Level1Types.View);

                return md;
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tmd"></param>
        /// <param name="connStr"></param>
        public void SaveViewMetaData(ViewMetadata tmd, string connStr)
        {
            if (tmd != null)
            {
                SaveMetadata(tmd, connStr);
                ColumnMetadataAccess cma = new ColumnMetadataAccess();
                foreach (ColumnMetaData item in tmd.Columns)
                {
                    SaveMetadata(item, connStr);
                }
                
            }
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="connStr"></param>
        ///// <returns></returns>
        //public string GenerateWikiMarkupForViews(string connStr)
        //{
        //    return GenerateWikiMarkupForViews(connStr, null);
        //}



//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="connStr"></param>
//        /// <param name="tmd"></param>
//        /// <returns></returns>
//        public string GenerateWikiMarkupForViews(string connStr, ViewMetadata tmd)
//        {
//            StringBuilder sb = new StringBuilder();
//            string result = string.Empty;
//            string sql = @"
//                            ;
//with InformationSchemaViews as 
//(	
//	  SELECT		    
//		 name as View_name,
//	     SCHEMA_NAME(schema_id ) as View_Schema
//	  FROM 
//		 sys.objects 
//	  where 
//		type = 'u' and is_ms_shipped = 0
//)
//
//SELECT 
//	WikiMarkup FROM 
//	InformationSchemaViews iss
//	CROSS APPLY(
//		SELECT 'h2. ' + iss.View_SCHEMA + '.' + iss.View_NAME + ' View' WikiMarkup UNION ALL
//
//		SELECT ISNULL( 
//					( SELECT CAST(VALUE AS NVARCHAR(MAX)) FROM fn_listextendedProperty('Description', 
//					 'SCHEMA', iss.View_SCHEMA
//					 ,'View', iss.View_NAME
//					 , NULL, NULL))
//			 , '{color:#ff0000}{*}DESCRIPTION MISSING{*}{color}') + ' \\' UNION ALL
//
//		SELECT '||COLUMN_NAME|' +  '|IS_NULLABLE|' +  '|DATA_TYPE|' +  '|CHARACTER_MAXIMUM_LENGTH|' +  '|COLUMN_DEFAULT|' + '|Description|' UNION ALL
//
//		SELECT '|' + ISNULL(COLUMN_NAME,'') + '|' + ISNULL(IS_NULLABLE,'') + '|' + ISNULL(DATA_TYPE, '') + '|' + ISNULL(CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR(50)), 'N/A')  + '|' + ISNULL(COLUMN_DEFAULT, 'none') + '|' + 
//				ISNULL(
//					(SELECT CAST(VALUE AS NVARCHAR(MAX)) FROM fn_listextendedProperty('Description', 
//					 'SCHEMA', iss.View_SCHEMA
//					 ,'View', iss.View_NAME
//					 , 'COLUMN', Column_Name))
//				, '{color:#ff0000}{*}DESCRIPTION MISSING{*}{color}') +' |'
//
//		 FROM INFORMATION_SCHEMA.COLUMNS
//		 WHERE table_name = iss.View_NAME
//	 ) AllTheGoodies
//	 
//                             ";

//            string whereStatement = "";
//            if (tmd != null)
//            {
//                whereStatement = string.Concat("WHERE iss.View_Name = '", tmd.Level1Name, "' and iss.View_schema = '", tmd.Schema, "'");
//                sql = string.Concat(sql, whereStatement);
//            }

//            sql = string.Concat(sql, "ORDER BY iss.View_Schema, iss.View_Name");

//            using (DbCommand cmd = CommandFactory.Create(sql, connStr))
//            {
//                cmd.Connection.Open();

//                DbDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    sb.AppendLine(reader[0].ToString());
//                }
//            }

//            return sb.ToString();
//        }


        /// <summary>
        /// Get list of all Views and their Descriptions. Will only include user defined Views and are not ms_shipped
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public ObservableCollection<ViewMetadata> GetMasterViewMetadata(string connStr)
        {
           ObservableCollection<ViewMetadata> Views = new ObservableCollection<ViewMetadata>();

            string sql = @"
;
with InformationSchemaViews as 
(	
	  SELECT		    
		 TABLE_NAME as View_name,
	      table_schema as View_Schema
	  FROM 
		 INFORMATION_SCHEMA.views 
)

SELECT View_NAME
        , View_SCHEMA
        , VALUE 
        , CASE 
            WHEN VALUE IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) 
            END AS HasDescription
    FROM
    InformationSchemaViews iss
    OUTER APPLY
    (
        SELECT VALUE FROM fn_listextendedProperty('Description', 
                'SCHEMA', iss.View_SCHEMA
                ,'View', iss.View_NAME
                , NULL, NULL)
    ) descr
        
        order by View_schema, View_name";


            using (DbCommand cmd = CommandFactory.Create(sql, connStr))
            {
                cmd.Connection.Open();
                DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Views.Add(new ViewMetadata(
                        reader.GetString(reader.GetOrdinal("View_Name")),
                        "NULL",
                        reader.GetString(reader.GetOrdinal("View_SCHEMA")),
                        reader.GetStringOrEmpty(reader.GetOrdinal("Value")),
                        !reader.GetBoolean(reader.GetOrdinal("HasDescription"))));
                }

            }
            return Views;
        }
    }
}
