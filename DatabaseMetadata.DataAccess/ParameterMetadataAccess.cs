using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using DatabaseMetadata.Entities;
using System.Data.Common;

namespace DatabaseMetadata.DataAccess
{
    public class ParameterMetadataAccess : MetadataAccessBase
    {
        public void GetStoredProcedureMetaDataDetails(StoredProcedureMetaData storedProcedureMetadata, string connStr)
        {
            int indexCount = 0;
            ObservableCollection<ParameterMetaData> parameters = new ObservableCollection<ParameterMetaData>();

            string sql = string.Format(@"select 
                                           PARAMETER_NAME
		                                    , value as Description
		                                    , CASE WHEN VALUE IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END AS HasDescription
                                        from INFORMATION_SCHEMA.PARAMETERS
                                        outer apply
                                        (	
	                                        select value from fn_listextendedProperty('Description',
	                                         'SCHEMA', SPECIFIC_SCHEMA
	                                        , 'PROCEDURE', SPECIFIC_NAME
	                                        , 'PARAMETER', PARAMETER_NAME)
                                        ) descr
                                        where SPECIFIC_NAME = '{0}' and SPECIFIC_SCHEMA = '{1}' ", storedProcedureMetadata.Level1Name, storedProcedureMetadata.Schema);


            using (DbCommand cmd = CommandFactory.Create(sql, connStr))
            {
                cmd.Connection.Open();

                DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    parameters.Add(new ParameterMetaData
                    (
                        storedProcedureMetadata.Level1Name,
                        reader.GetString(reader.GetOrdinal("PARAMETER_NAME")),
                        storedProcedureMetadata.Schema,
                        reader.GetStringOrEmpty(reader.GetOrdinal("Description")),
                        !reader.GetBoolean(reader.GetOrdinal("HasDescription")),
                        indexCount++
                    ));
                }
                reader.Close();
            }

            storedProcedureMetadata.Parameters = parameters;

        }
    }
}
