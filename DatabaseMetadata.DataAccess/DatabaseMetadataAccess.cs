using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DatabaseMetadata.Entities;

namespace DatabaseMetadata.DataAccess
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
            ViewMetadataAccess vma = new ViewMetadataAccess();

            dmd.Tables = tma.GetMasterTableMetadata(connStr);
            dmd.Views = vma.GetMasterViewMetadata(connStr);

            ColumnMetadataAccess cma = new ColumnMetadataAccess();

            foreach (var item in dmd.Views)
            {
                item.Columns = cma.GetColumnMetadata(item.Level1Name, item.Schema, connStr,Level1Types.View);
            }
            dmd.StoredProcedures = spma.GetStoredProcedureMetaData(connStr);

            return dmd;
        }
    }
}
