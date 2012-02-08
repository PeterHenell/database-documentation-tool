using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using ExtendedPropertiesDocumentationTool.Entities;

namespace ExtendedPropertiesDocumentationTool.DataAccess
{
    public class ParameterMetadataAccess : MetadataAccessBase
    {
        internal void GetStoredProcedureMetaDataDetails(StoredProcedureMetaData storedProcedureMetadata, string connStr)
        {
            int indexCount = 0;
            ObservableCollection<ColumnMetaData> parameters = new ObservableCollection<ColumnMetaData>();

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
                                        where SPECIFIC_NAME = '{0}' and SPECIFIC_SCHEMA = '{1}' ", storedProcedureMetadata.Name, storedProcedureMetadata.Schema);


            using (SqlCommand cmd = CommandFactory.Create(sql, connStr))
            {
                cmd.Connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    parameters.Add(new ColumnMetaData
                    (
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

        internal void SaveParameterMetadata(StoredProcedureMetaData spMetadata, string connStr)
        {
            foreach (ColumnMetaData item in spMetadata.Parameters)
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

                    if (spMetadata.HasChanges)
                    {
                        SaveDescription(sm, connStr, Level1Types.Procedure, item.Description, spMetadata.Schema, spMetadata.Name, Level2Types.Parameter, item.Name);
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
