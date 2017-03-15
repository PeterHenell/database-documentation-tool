using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseMetadata.Entities;
using System.Data.Common;
using ExtendedPropertiesDocumentationTool;

namespace DatabaseMetadata.DataAccess
{
    public class WikiGenerator
    {

        public string GenerateWikiMarkup(string connStr, EntityBase entity, Level1Types level1Type, Level2Types level2Type)
        {
            switch (level1Type)
            {
                case Level1Types.Table:
                    switch (level2Type)
                    {
                        case Level2Types.NULL:
                            return GenerateWikiMarkupForTablesOrView(connStr, entity, level1Type);
                        
                        case Level2Types.Index:
                        case Level2Types.Constraint:
                            return CreateWikiMarkupForIndexesOnTable(connStr, entity);
                            
                        default:
                            throw new ArgumentException();
                    }
                    
                case Level1Types.Procedure:
                    return GenerateWikiMarkupForStoredProcedures(connStr, entity as StoredProcedureMetaData);
                case Level1Types.View:
                    return GenerateWikiMarkupForTablesOrView(connStr, entity, level1Type);
         
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Creates wikimarkup string for the specified table's indexes
        /// </summary>
        /// <param name="tmd">use NULL to create markup for indexes on ALL tables</param>
        /// <returns></returns>
        private string CreateWikiMarkupForIndexesOnTable(string connStr, EntityBase tmd)
        {
            StringBuilder sb = new StringBuilder();
            string result = string.Empty;
            string sql = string.Format(@"
;with InformationSchemaTables as 
(	
	  SELECT		    
		 name as Table_name,
	     SCHEMA_NAME(schema_id ) as Table_Schema,
	     object_id
	  FROM 
		 sys.objects 
	  where 
		type = 'u' and is_ms_shipped = 0
)

SELECT 
	WikiMarkup FROM 
	InformationSchemaTables iss
CROSS APPLY(

		SELECT '||' + iss.TABLE_SCHEMA + '.' + iss.TABLE_NAME + ' || Index Name || Index Type || Unique || Primary key || Key Columns || Included Columns ||' as WikiMarkup 

		UNION ALL

		SELECT '|' + ISNULL( 
					( SELECT CAST(VALUE AS NVARCHAR(MAX)) FROM fn_listextendedProperty('{0}', 
					 'SCHEMA', iss.TABLE_SCHEMA
					 ,'TABLE', iss.TABLE_NAME
					 , CASE is_unique WHEN 1 THEN 'CONSTRAINT' ELSE 'INDEX' END, name))
			 , '&nbsp;')  
			 + '|' + ind.name
			 + '|' + cast(type_desc as varchar(max)) COLLATE SQL_Latin1_General_CP1_CI_AS
			 + '|' + case is_unique when 1 then 'Yes' else 'No' end
			 + '|' + case is_primary_key when 1 then 'Yes' else 'No' end
			 + '|' + isnull(substring(KeyColumns, 0, len(KeyColumns)) , '&nbsp;')
			 + '|' + isnull(substring(IncludedColumns, 0, len(IncludedColumns)) , '&nbsp;')
			 + '|'
			 
			 
		from 
			sys.indexes ind
		CROSS APPLY ( SELECT col.name + ', ' 
                     FROM sys.index_columns cols
                     INNER JOIN sys.columns col 
						ON cols.object_id = col.object_id and cols.column_id = col.column_id 
                    WHERE cols.index_id = ind.index_id and cols.object_id = ind.object_id and cols.is_included_column = 0 --and key_ordinal > 0
                    ORDER BY cols.key_ordinal
                      FOR XML PATH('') )  D ( KeyColumns )
		CROSS APPLY
		( SELECT col.name + ', ' 
                     FROM sys.index_columns cols
                     INNER JOIN sys.columns col 
						ON cols.object_id = col.object_id and cols.column_id = col.column_id                      
                    WHERE cols.index_id = ind.index_id and cols.object_id = ind.object_id and cols.is_included_column = 1 --and key_ordinal = 0
                    ORDER BY cols.index_column_id
                      FOR XML PATH('') )  DE ( IncludedColumns )
			
		where
			object_id = iss.object_id 
			and ind.type_desc <> 'HEAP'
			
		union all 
		
		select '\\'

) AllTheGoodies

", ApplicationSettings.Default.ExtendedPropKey);
            string whereStatement = "";
            if (tmd != null)
            {
                whereStatement = string.Concat("WHERE iss.Table_Name = '", tmd.Level1Name, "' and iss.table_schema = '", tmd.Schema, "'");
                sql = string.Concat(sql, whereStatement);
            }

            sql = string.Concat(sql, "ORDER BY iss.Table_Schema, iss.Table_Name");

            using (DbCommand cmd = CommandFactory.Create(sql, connStr))
            {
                cmd.Connection.Open();

                DbDataReader reader = cmd.ExecuteReader();
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
        private string GenerateWikiMarkupForStoredProcedures(string connStr, StoredProcedureMetaData spMetadata)
        {
            string result = string.Empty;
            StringBuilder sb = new StringBuilder();

            string sql = @" 
                    SELECT 
                        WikiMarkup
     
                    FROM 
                        sys.objects so

                    outer APPLY
                    (
                        SELECT (
                                'h2. ' +  SCHEMA_NAME(so.schema_id) + '.' + so.Name + '
                    \\' +
                                    ISNULL( (SELECT    CAST(VALUE AS NVARCHAR(MAX))
                                    FROM 
                                        fn_listextendedProperty(
                                            @ExtendedPropertyName
                                            , 'SCHEMA', SCHEMA_NAME(so.schema_id)
                                            ,'PROCEDURE', so.Name
                                            , NULL, NULL)), '{color:#ff0000}{*}DESCRIPTION MISSING{*}{color}')
                                )  AS WikiMarkup


                        UNION ALL
    
                        SELECT CASE 
                            WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.parameters
			                        where SPECIFIC_NAME = so.name and SPECIFIC_SCHEMA = schema_name(so.schema_id)
			                        )
                                THEN
                                    '||Parameter Name|Datatype|Default value?|Is Output column|Description|' 
                                ELSE
                                    'No parameters'
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
                        outer  APPLY
                            fn_listextendedProperty(
                                 @ExtendedPropertyName
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
                        so.name";// ApplicationSettings.Default.ExtendedPropKey);



            using (DbCommand cmd = CommandFactory.Create(sql, connStr))
            {
                // If we are requesting wikimarkup for a specific stored procedure then this sql code need to be included in the search clause.
                if (spMetadata != null)
                {
                    cmd.Parameters.Add(CommandFactory.CreateParameter("SPName", spMetadata.Level1Name));
                    cmd.Parameters.Add(CommandFactory.CreateParameter("SPSchema", spMetadata.Schema));
                    cmd.Parameters.Add(CommandFactory.CreateParameter("ExtendedPropertyName", ApplicationSettings.Default.ExtendedPropKey));
                    
                }
                else
                {
                    cmd.Parameters.Add(CommandFactory.CreateParameter("SPName", DBNull.Value));
                    cmd.Parameters.Add(CommandFactory.CreateParameter("SPSchema", DBNull.Value));
                    cmd.Parameters.Add(CommandFactory.CreateParameter("ExtendedPropertyName", ApplicationSettings.Default.ExtendedPropKey));
                }

                cmd.Connection.Open();
                DbDataReader reader = cmd.ExecuteReader();
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
        /// <param name="item"></param>
        /// <returns></returns>
        private string GenerateWikiMarkupForTablesOrView(string connStr, EntityBase item, Level1Types level1Type)
        {
            if (level1Type != Level1Types.View && level1Type != Level1Types.Table)
            {
                throw new ArgumentException("item must be View or Table (metadata)");
            }

            StringBuilder sb = new StringBuilder();
            string result = string.Empty;
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
		type in ( case when 'Table' = '{0}' then 'u' when 'View' = '{0}' then 'v' end )
        and is_ms_shipped = 0
)

SELECT 
	WikiMarkup FROM 
	InformationSchemaTables iss
	CROSS APPLY(
		SELECT 'h2. ' + iss.TABLE_SCHEMA + '.' + iss.TABLE_NAME + ' {0}' WikiMarkup UNION ALL

		SELECT ISNULL( 
					( SELECT CAST(VALUE AS NVARCHAR(MAX)) FROM fn_listextendedProperty('{0}', 
					 'SCHEMA', iss.TABLE_SCHEMA
					 ,'{1}', iss.TABLE_NAME
					 , NULL, NULL))
			 , '{{color:#ff0000}}{{*}}DESCRIPTION MISSING{{*}}{{color}}') + ' \\' UNION ALL

		SELECT '||COLUMN_NAME|' +  '|IS_NULLABLE|' +  '|DATA_TYPE|' +  '|CHARACTER_MAXIMUM_LENGTH|' +  '|COLUMN_DEFAULT|' + '|Description|' UNION ALL

		SELECT '|' + ISNULL(COLUMN_NAME,'') + '|' + ISNULL(IS_NULLABLE,'') + '|' + ISNULL(DATA_TYPE, '') + '|' + ISNULL(CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR(50)), 'N/A')  + '|' + ISNULL(COLUMN_DEFAULT, 'none') + '|' + 
				ISNULL(
					(SELECT CAST(VALUE AS NVARCHAR(MAX)) FROM fn_listextendedProperty('{1}', 
					 'SCHEMA', iss.TABLE_SCHEMA
					 ,'{1}', iss.TABLE_NAME
					 , 'COLUMN', Column_Name))
				, '{{color:#ff0000}}{{*}}DESCRIPTION MISSING{{*}}{{color}}') +' |'

		 FROM INFORMATION_SCHEMA.COLUMNS
		 WHERE TABLE_NAME = iss.TABLE_NAME
	 ) AllTheGoodies
	 
                             ", level1Type.ToString(), ApplicationSettings.Default.ExtendedPropKey);
            //sql = string.Format(sql, level1Type.ToString());
            string whereStatement = "";
            if (item != null)
            {
                whereStatement = string.Concat("WHERE iss.Table_Name = '", item.Level1Name, "' and iss.table_schema = '", item.Schema, "'");
                sql = string.Concat(sql, whereStatement);
            }

            sql = string.Concat(sql, "ORDER BY iss.Table_Schema, iss.Table_Name");

            using (DbCommand cmd = CommandFactory.Create(sql, connStr))
            {
                cmd.Connection.Open();

                DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    sb.AppendLine(reader[0].ToString());
                }
            }

            return sb.ToString();
        }
    }
}
