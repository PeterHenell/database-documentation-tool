using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace ExtendedPropertiesDocumentationTool.DataAccess
{
    /// <summary>
    /// SQL Command factory for creating sql commands from the provided query string.
    /// They will all return a SqlCommand or SqlDataAdapter with an open connection
    /// </summary>
    public static class CommandFactory
    {
        /// <summary>
        /// Creates a SqlCommand from the suplied <paramref name="sqlQuery"/>. The command type is Text
        /// </summary>
        /// <param name="sqlQuery">The string to execute in the database</param>
        /// <returns>An sql command with an open connection to the database</returns>
        public static SqlCommand Create(string sqlQuery, string connStr)
        {
            return Create(sqlQuery, connStr, CommandType.Text);
        }

        /// <summary>
        /// Creates a SqlCommand from the suplied <paramref name="sqlQuery"/>.
        /// </summary>
        /// <param name="sqlQuery">The string to execute in the database</param>
        /// <param name="commandType">The command type to be used.</param>
        /// <returns>An sql command with an open connection to the database</returns>
        public static SqlCommand Create(string sqlQuery, string connStr, CommandType commandType)
        {
            SqlCommand cmd = new SqlCommand(sqlQuery, ConnectionFactory.Create(connStr)) { CommandType = commandType };
            return cmd;
        }

        ///// <summary>
        ///// Creates an SqlDataAdapter from the supplied query string. The <paramref name="sqlQuery"/> will be the SelectCommand of the adapter.
        ///// </summary>
        ///// <param name="sqlQuery">The string to execute in the database</param>
        ///// <returns>An sql dataadapter with an open connection to the database.</returns>
        //public static SqlDataAdapter CreateAdapter(string sqlQuery)
        //{
        //    SqlDataAdapter adapter = new SqlDataAdapter(Create(sqlQuery));
        //    return adapter;
        //}

        private static class ConnectionFactory
        {
            public static SqlConnection Create(string connStr)
            {
                SqlConnection con = new SqlConnection(connStr);
                con.Open();
                con.Close();
                return con;
            }
        }
    }
}
