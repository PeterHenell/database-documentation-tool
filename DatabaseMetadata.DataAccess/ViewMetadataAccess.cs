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


        /// <summary>
        /// Get list of all Views and their Descriptions. Will only include user defined Views and are not ms_shipped
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public ObservableCollection<ViewMetadata> GetMasterViewMetadata(string connStr)
        {
           ObservableCollection<ViewMetadata> Views = new ObservableCollection<ViewMetadata>();

            string sql = string.Format(@"
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
        SELECT VALUE FROM fn_listextendedProperty('{0}', 
                'SCHEMA', iss.View_SCHEMA
                ,'View', iss.View_NAME
                , NULL, NULL)
    ) descr
        
        order by View_schema, View_name", ApplicationSettings.Default.ExtendedPropKey);


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
