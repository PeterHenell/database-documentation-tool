using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtendedPropertiesDocumentationTool.DataAccess
{
    public class MetadataFacade
    {

        ColumnMetadataAccess columnMA = new ColumnMetadataAccess();
        DatabaseMetadataAccess databaseMA = new DatabaseMetadataAccess();
        ParameterMetadataAccess parameterMA = new ParameterMetadataAccess();
        StoredProcedureMetadataAccess storedProcedureMA = new StoredProcedureMetadataAccess();
        TableMetadataAccess tableMA = new TableMetadataAccess();


        internal void GetStoredProcedureMetaDataDetails(Entities.StoredProcedureMetaData storedprocedureMetadata, string connStr)
        {
            parameterMA.GetStoredProcedureMetaDataDetails(storedprocedureMetadata, connStr);
        }

        internal void GetTableMetaDetails(Entities.TableMetadata tableMetadata, string connStr)
        {
            tableMA.GetTableMetaDetails(tableMetadata, connStr);
        }

        internal Entities.DatabaseMetaData GetMetaDataForDatabase(string connStr)
        {
            return databaseMA.GetMetaDataForDatabase(connStr);
        }

        internal void SaveTableMetaData(Entities.TableMetadata table, string connStr)
        {
            tableMA.SaveTableMetaData(table, connStr);
        }

        internal string GenerateWikiMarkupForTables(string connStr)
        {
            return tableMA.GenerateWikiMarkupForTables(connStr);
        }

        internal string GenerateWikiMarkupForStoredProcedures(string connStr)
        {
            return storedProcedureMA.GenerateWikiMarkupForStoredProcedures(connStr);
        }

        internal string GenerateWikiMarkupForTables(string ConnectionString, Entities.TableMetadata tmd)
        {
            return tableMA.GenerateWikiMarkupForTables(ConnectionString, tmd);
        }

        internal string GenerateWikiMarkupForSelectedStoredProcedure(Entities.StoredProcedureMetaData storeprocedureMetadata, string connStr)
        {
            return storedProcedureMA.GenerateWikiMarkupForStoredProcedures(connStr, storeprocedureMetadata);
        }

        internal void SaveStoredProcedureMetadata(Entities.StoredProcedureMetaData storedProcedureMetadata, string connStr)
        {
            storedProcedureMA.SaveStoredProcedureMetadata(storedProcedureMetadata, connStr);
        }
    }
}
