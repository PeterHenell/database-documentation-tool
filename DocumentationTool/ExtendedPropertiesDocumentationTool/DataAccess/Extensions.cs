using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace ExtendedPropertiesDocumentationTool.DataAccess
{
    public static class Extensions
    {
        public static string GetStringOrEmpty(this SqlDataReader reader, int i)
        {
            if (reader.IsDBNull(i))
                return "";
            else
                return reader.GetString(i);
        }
    
    }

}
