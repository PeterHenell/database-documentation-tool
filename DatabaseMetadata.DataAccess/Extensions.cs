using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using System.Data.Common;

namespace DatabaseMetadata.DataAccess
{
    public static class Extensions
    {
        public static string GetStringOrEmpty(this DbDataReader reader, int i)
        {
            if (reader.IsDBNull(i))
                return string.Empty;
            else
                return reader.GetString(i);
        }
    
    }

}
