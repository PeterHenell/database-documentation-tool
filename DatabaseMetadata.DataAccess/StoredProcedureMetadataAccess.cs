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
    public class StoredProcedureMetadataAccess : MetadataAccessBase
    {

        ParameterMetadataAccess _pma;

        public StoredProcedureMetadataAccess()
        {
            _pma = new ParameterMetadataAccess();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public ObservableCollection<StoredProcedureMetaData> GetStoredProcedureMetaData(string connStr)
        {
            ObservableCollection<StoredProcedureMetaData> spmd = new ObservableCollection<StoredProcedureMetaData>();

            string sql = string.Format(@"select  name ,
        SCHEMA_NAME(schema_id) as SchemaName
        , Value as Description
		, CASE WHEN VALUE IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END AS HasDescription
from sys.objects 
outer apply
(	
	select value from fn_listextendedProperty('{0}',
	'SCHEMA', SCHEMA_NAME(schema_id)
	,'PROCEDURE', name, NULL, NULL)
) descr

where type = 'p' and is_ms_shipped = 0
order by schema_id, name", ApplicationSettings.Default.ExtendedPropKey);

            using (DbCommand cmd = CommandFactory.Create(sql, connStr))
            {
                cmd.Connection.Open();
                DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    spmd.Add(new StoredProcedureMetaData
                    (
                        reader.GetString(reader.GetOrdinal("Name")),
                        "NULL",
                        reader.GetString(reader.GetOrdinal("SchemaName")),
                        reader.GetStringOrEmpty(reader.GetOrdinal("Description")),
                        !reader.GetBoolean(reader.GetOrdinal("HasDescription"))
                    ));
                }

                return spmd;
            }


        }
    }
}
