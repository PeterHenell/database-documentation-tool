using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ExtendedPropertiesDocumentationTool.DataAccess
{
    public class MetadataAccessBase
    {
        string sqlAdd = "sys.sp_addextendedproperty";
        string sqlUpdate = "sys.sp_updateextendedproperty";
        string sqlDrop = "sys.sp_dropextendedproperty";


        protected bool SaveDescription(SaveModes saveMode, string connStr, Level1Types level1Type, string descriptionValue, string objectSchema, string objectName, Level2Types level2Type, string level2Name)
        {
            string sql = string.Empty;

            switch (saveMode)
            {
                case SaveModes.New:
                    sql = sqlAdd;
                    break;
                case SaveModes.Update:
                    sql = sqlUpdate;
                    break;
                case SaveModes.Delete:
                    sql = sqlDrop;
                    break;
                default:
                    break;
            }

            try
            {
                using (SqlCommand cmd = CommandFactory.Create(sql, connStr, System.Data.CommandType.StoredProcedure))
                {
                    cmd.Connection.Open();
                    
                    if (saveMode != SaveModes.Delete)
                    {
                        cmd.Parameters.AddWithValue("@value", descriptionValue);
                    }

                    cmd.Parameters.AddWithValue("@name", "Description");
                    cmd.Parameters.AddWithValue("@level0type", "SCHEMA");
                    cmd.Parameters.AddWithValue("@level0name", objectSchema);
                    cmd.Parameters.AddWithValue("@level1type", level1Type.ToString());
                    cmd.Parameters.AddWithValue("@level1name", objectName);
                    if (level2Type != Level2Types.NULL)
                    {
                        cmd.Parameters.AddWithValue("@level2type", level2Type.ToString());
                        cmd.Parameters.AddWithValue("@level2name", level2Name);

                    }

                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }

            return true;
        }



        protected bool SaveDescription(SaveModes saveMode, string connStr, Level1Types level1Type, string descriptionValue, string objectSchema, string objectName)
        {
            return SaveDescription(saveMode, connStr, level1Type, descriptionValue, objectSchema, objectName, Level2Types.NULL, string.Empty);
        }
           
    }
}
