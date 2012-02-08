using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using ExtendedPropertiesDocumentationTool.Entities;

namespace ExtendedPropertiesDocumentationTool.DataAccess
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
        internal string GenerateWikiMarkupForStoredProcedures(string connStr, StoredProcedureMetaData spMetadata)
        {
            string result = string.Empty;
            StringBuilder sb = new StringBuilder();

           


            string sql = @" 
                    SELECT 
                        WikiMarkup
     
                    FROM 
                        sys.objects so

                    CROSS APPLY
                    (
                        SELECT (
                                'h2. ' +  SCHEMA_NAME(so.schema_id) + '.' + so.Name + '
                    \\' +
                                    ISNULL( (SELECT    CAST(VALUE AS NVARCHAR(MAX))
                                    FROM 
                                        fn_listextendedProperty(
                                            'Description'
                                            , 'SCHEMA', SCHEMA_NAME(so.schema_id)
                                            ,'PROCEDURE', so.Name
                                            , NULL, NULL)), '{color:#ff0000}{*}DESCRIPTION MISSING{*}{color}')
                                )  AS WikiMarkup


                        UNION ALL
    
                        SELECT CASE 
                            WHEN EXISTS (SELECT 1 FROM fn_listextendedProperty(
                                'Description'
                                ,'SCHEMA', SCHEMA_NAME(so.schema_id) 
                                ,'PROCEDURE', so.Name
                                ,'PARAMETER', NULL))
                                THEN
                                    '||Parameter Name|Datatype|Default value?|Is Output column|Description|' 
                                ELSE
                                    'No params'
                                END
                        UNION ALL
    
                        SELECT 
                            '|' + p.name + '|' +
                                  --(,Scale)
                                        CASE 
                                        WHEN system_type_id IN (231,167,175,239) THEN 
                                            CAST(TYPE_NAME(p.system_type_id) AS NVARCHAR(100)) + '(' + CAST(max_length AS NVARCHAR(50)) + ')'
                                        WHEN system_type_id = 106 THEN
                                            CAST(TYPE_NAME(p.system_type_id) AS NVARCHAR(100)) + '(' + CAST(PRECISION AS NVARCHAR(50)) + ', ' + CAST(Scale AS NVARCHAR(50)) +  ')'
                                        ELSE
                                            TYPE_NAME(p.system_type_id)
                                        END
                                     + '|' +
                                  CAST(ISNULL(p.default_value, 'N/a') AS NVARCHAR(100)) + '|' +
                                  CASE p.is_output WHEN 1 THEN 'Yes' ELSE 'No' END + '|' +
                                  ISNULL(CAST(VALUE AS NVARCHAR(MAX)), '{color:#ff0000}{*}DESCRIPTION MISSING{*}{color}') + '|'
                        FROM 
                            sys.parameters p
                        CROSS APPLY
                            fn_listextendedProperty(
                                 'Description'
                                ,'SCHEMA', SCHEMA_NAME(so.schema_id) 
                                ,'PROCEDURE', so.Name
                                ,'PARAMETER', p.name)
                        WHERE 
                            p.object_id = so.object_id
        
                        UNION ALL
    
                        SELECT '
                        \\\
                        '    
        
                    ) spInfo        

                    WHERE 
                        so.type IN ('p', 'fn', 'if', 'tf')
                        and is_ms_shipped = 0
                        and 
                            (@SPName IS NULL or (name = @SPName and schema_id(@SPSchema) = schema_id))

                    ORDER BY 
                        so.name";

            

            using (SqlCommand cmd = CommandFactory.Create(sql, connStr))
            {
                 // If we are requesting wikimarkup for a specific stored procedure then this sql code need to be included in the search clause.
                if (spMetadata != null)
                {
                    cmd.Parameters.AddWithValue("SPName", spMetadata.Name);
                    cmd.Parameters.AddWithValue("SPSchema", spMetadata.Schema);
                }
                else
                {
                    cmd.Parameters.AddWithValue("SPName", DBNull.Value);
                    cmd.Parameters.AddWithValue("SPSchema", DBNull.Value);
                }
                
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
        /// 
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        internal string GenerateWikiMarkupForStoredProcedures(string connStr)
        {
            return GenerateWikiMarkupForStoredProcedures(connStr, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public ObservableCollection<StoredProcedureMetaData> GetStoredProcedureMetaData(string connStr)
        {
            ObservableCollection<StoredProcedureMetaData> spmd = new ObservableCollection<StoredProcedureMetaData>();

            string sql = @"select  name ,
        SCHEMA_NAME(schema_id) as SchemaName
        , Value as Description
		, CASE WHEN VALUE IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END AS HasDescription
from sys.objects 
outer apply
(	
	select value from fn_listextendedProperty('Description',
	'SCHEMA', SCHEMA_NAME(schema_id)
	,'PROCEDURE', name, NULL, NULL)
) descr

where type = 'p' and is_ms_shipped = 0
order by schema_id, name";

            using (SqlCommand cmd = CommandFactory.Create(sql, connStr))
            {
                cmd.Connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    spmd.Add(new StoredProcedureMetaData
                    (
                        reader.GetString(reader.GetOrdinal("Name")),
                        reader.GetString(reader.GetOrdinal("SchemaName")),
                        reader.GetStringOrEmpty(reader.GetOrdinal("Description")),
                        !reader.GetBoolean(reader.GetOrdinal("HasDescription"))
                    ));
                }

                return spmd;
            }


        }


      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_selectedSPMetadata"></param>
        /// <param name="connStr"></param>
        internal void SaveStoredProcedureMetadata(StoredProcedureMetaData _selectedSPMetadata, string connStr)
        {
            if (_selectedSPMetadata != null)
            {
                SaveModes sm = SaveModes.New;

                // Figure out if the description is new or if it exists and should be updated.
                if (_selectedSPMetadata.IsNewDescription)
                    sm = SaveModes.New;
                else
                    sm = SaveModes.Update;
                // If the Description is empty then we should drop the extended property
                if (string.IsNullOrEmpty(_selectedSPMetadata.Description))
                {
                    sm = SaveModes.Delete;
                }

                if (_selectedSPMetadata.HasChanges)
                {
                    SaveDescription(sm, connStr, Level1Types.Procedure, _selectedSPMetadata.Description, _selectedSPMetadata.Schema, _selectedSPMetadata.Name);
                    _selectedSPMetadata.IsNewDescription = false;    
                }

                ParameterMetadataAccess pma = new ParameterMetadataAccess();
                pma.SaveParameterMetadata(_selectedSPMetadata, connStr);

                

            }
        }
    }
}
