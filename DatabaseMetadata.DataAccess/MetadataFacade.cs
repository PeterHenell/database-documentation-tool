using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseMetadata.DataAccess;
using Entities = DatabaseMetadata.Entities;
using DatabaseMetadata.Entities;

namespace DatabaseMetadata.DataAccess
{
    public class MetadataFacade
    {

        ColumnMetadataAccess columnMA = new ColumnMetadataAccess();
        DatabaseMetadataAccess databaseMA = new DatabaseMetadataAccess();
        ParameterMetadataAccess parameterMA = new ParameterMetadataAccess();
        StoredProcedureMetadataAccess storedProcedureMA = new StoredProcedureMetadataAccess();
        TableMetadataAccess tableMA = new TableMetadataAccess();
        IndexEntityMetadataAccess indexMA = new IndexEntityMetadataAccess();

        MetadataAccessBase mab = new MetadataAccessBase();
        WikiGenerator wiki = new WikiGenerator();

        public  string CreateWikiMarkupForIndexesOnTable(Entities.TableMetadata tmd, string connStr)
        {
            return wiki.GenerateWikiMarkup(connStr, tmd, Level1Types.Table, Level2Types.Index);
        }
        public  string CreateWikiMarkupForIndexesOnAllTables(string connStr)
        {
            return wiki.GenerateWikiMarkup(connStr, null, Level1Types.Table, Level2Types.Index);
        }

        public  void GetStoredProcedureMetaDataDetails(Entities.StoredProcedureMetaData storedprocedureMetadata, string connStr)
        {
            parameterMA.GetStoredProcedureMetaDataDetails(storedprocedureMetadata, connStr);
        }

        public  void GetTableMetaDetails(Entities.TableMetadata tableMetadata, string connStr)
        {
            tableMA.GetTableMetaDetails(tableMetadata, connStr);
        }

        public  Entities.DatabaseMetaData GetMetaDataForDatabase(string connStr)
        {
            return databaseMA.GetMetaDataForDatabase(connStr);
        }

        public  void SaveTableMetaData(Entities.TableMetadata table, string connStr)
        {
            //tableMA.SaveTableMetaData(table, connStr);
            if (table != null)
            {
                mab.SaveMetadata(table, connStr);
                foreach (var col in table.Columns)
                {
                    mab.SaveMetadata(col, connStr);
                }
                foreach (var ind in table.Indexes)
                {
                    mab.SaveMetadata(ind, connStr);
                }
            }
        }

        public  string GenerateWikiMarkupForStoredProcedures(string connStr)
        {
            return wiki.GenerateWikiMarkup(connStr, null, Level1Types.Procedure, Level2Types.NULL);
        }

        public  string GenerateWikiMarkupForTablesAndViews(string ConnectionString, Entities.EntityBase tmd, Level1Types level1Type)
        {
            return wiki.GenerateWikiMarkup(ConnectionString, tmd, level1Type, Level2Types.NULL);
        }

        public  string GenerateWikiMarkupForSelectedStoredProcedure(Entities.StoredProcedureMetaData storeprocedureMetadata, string connStr)
        {
            return wiki.GenerateWikiMarkup(connStr, storeprocedureMetadata, Level1Types.Procedure, Level2Types.NULL);
        }

        public  void SaveStoredProcedureMetadata(Entities.StoredProcedureMetaData storedProcedureMetadata, string connStr)
        {
            if (storedProcedureMetadata != null)
            {
                mab.SaveMetadata(storedProcedureMetadata, connStr);
                foreach (var parm in storedProcedureMetadata.Parameters)
                {
                    mab.SaveMetadata(parm, connStr);
                }
            }
        }

        public  void SaveViewMetaData(Entities.ViewMetadata vw, string connStr)
        {
            if (vw != null)
            {
                mab.SaveMetadata(vw, connStr);
                foreach (var col in vw.Columns)
                {
                    mab.SaveMetadata(col, connStr);
                }
            }
            
        }

        public string GenerateSQLStatementForTable(string ConnectionString, TableMetadata table, Level1Types level1Types)
        {
            if (table != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(mab.GetSQLQueryText(table, ConnectionString));
                foreach (var col in table.Columns)
                {
                    sb.AppendLine(mab.GetSQLQueryText(col, ConnectionString));
                }
                return sb.ToString();
            }
            else
                return "No Selected Table";
        }
    }
}
