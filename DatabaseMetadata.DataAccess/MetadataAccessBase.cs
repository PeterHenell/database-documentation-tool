using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Common;
using DatabaseMetadata.Entities;

namespace DatabaseMetadata.DataAccess
{
    public class MetadataAccessBase
    {
        string sqlAdd = "sys.sp_addextendedproperty";
        string sqlUpdate = "sys.sp_updateextendedproperty";
        string sqlDrop = "sys.sp_dropextendedproperty";


        protected bool SaveDescription(SaveModes saveMode, string connStr, Level1Types level1Type, string descriptionValue, string objectSchema, string level1Name, Level2Types level2Type, string level2Name)
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
                using (DbCommand cmd = CommandFactory.Create(sql, connStr, System.Data.CommandType.StoredProcedure))
                {
                    cmd.Connection.Open();
                    
                    if (saveMode != SaveModes.Delete)
                    {
                        cmd.Parameters.Add(CommandFactory.CreateParameter("@value", descriptionValue));
                    }

                    cmd.Parameters.Add(CommandFactory.CreateParameter("@name", "Description"));
                    cmd.Parameters.Add(CommandFactory.CreateParameter("@level0type", "SCHEMA"));
                    cmd.Parameters.Add(CommandFactory.CreateParameter("@level0name", objectSchema));
                    cmd.Parameters.Add(CommandFactory.CreateParameter("@level1type", level1Type.ToString()));
                    cmd.Parameters.Add(CommandFactory.CreateParameter("@level1name", level1Name));
                    if (level2Type != Level2Types.NULL)
                    {
                        cmd.Parameters.Add(CommandFactory.CreateParameter("@level2type", level2Type.ToString()));
                        cmd.Parameters.Add(CommandFactory.CreateParameter("@level2name", level2Name));

                    }

                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }

        public void SaveMetadata(Entities.EntityBase entity, string connectionString)
        {
            SaveModes sm = SaveModes.New;
            try
            {
                if (entity.IsNewDescription)
                    sm = SaveModes.New;
                else
                    sm = SaveModes.Update;

                if (string.IsNullOrEmpty(entity.Description))
                {
                    sm = SaveModes.Delete;
                }

                if (entity.HasChanges)
                {
                    SaveDescription(sm, connectionString, entity.Level1Type, entity.Description, entity.Schema, entity.Level1Name, entity.Level2Type, entity.Level2Name);
                    entity.IsNewDescription = false;
                }

            }
            catch (Exception)
            {
                throw;
            }

        }


        //protected bool SaveDescription(SaveModes saveMode, string connStr, Level1Types level1Type, string descriptionValue, string objectSchema, string objectName)
        //{
        //    return SaveDescription(saveMode, connStr, level1Type, descriptionValue, objectSchema, objectName, Level2Types.NULL, string.Empty);
        //}
           
    }
}
