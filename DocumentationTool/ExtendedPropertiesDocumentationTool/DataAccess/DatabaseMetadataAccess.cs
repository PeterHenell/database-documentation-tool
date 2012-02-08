using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using ExtendedPropertiesDocumentationTool.Entities;

namespace ExtendedPropertiesDocumentationTool.DataAccess
{
    public class DatabaseMetadataAccess
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public DatabaseMetaData GetMetaDataForDatabase(string connStr)
        {
            DatabaseMetaData dmd = new DatabaseMetaData();

            StoredProcedureMetadataAccess spma = new StoredProcedureMetadataAccess();
            TableMetadataAccess tma = new TableMetadataAccess();

            dmd.Tables = tma.GetMasterTableMetadata(connStr);
            dmd.StoredProcedures = spma.GetStoredProcedureMetaData(connStr);

            return dmd;
        }
    }
}
