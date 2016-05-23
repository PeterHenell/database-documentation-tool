using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Common;
using DatabaseMetadata.Entities;
using ExtendedPropertiesDocumentationTool;

namespace DatabaseMetadata.DataAccess
{
    public class MetadataAccessBase
    {
        string sqlAdd = "sys.sp_addextendedproperty";
        string sqlUpdate = "sys.sp_updateextendedproperty";
        string sqlDrop = "sys.sp_dropextendedproperty";

        protected bool SaveDescription(SaveModes saveMode, string connStr, Level1Types level1Type, string descriptionValue, string objectSchema, string level1Name, Level2Types level2Type, string level2Name)
        {
            string sql = GetSQLTextForSaveMode(saveMode);

            try
            {
                using (DbCommand cmd = CommandFactory.Create(sql, connStr, System.Data.CommandType.StoredProcedure))
                {
                    FillCommand(cmd, saveMode, level1Type, descriptionValue, objectSchema, level1Name, level2Type, level2Name);

                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }

        private static void FillCommand(DbCommand cmd, SaveModes saveMode, Level1Types level1Type, string descriptionValue, string objectSchema, string level1Name, Level2Types level2Type, string level2Name)
        {
            cmd.Connection.Open();

            if (saveMode != SaveModes.Delete)
            {
                cmd.Parameters.Add(CommandFactory.CreateParameter("@value", descriptionValue));
            }

            cmd.Parameters.Add(CommandFactory.CreateParameter("@name", ApplicationSettings.Default.ExtendedPropKey));
            cmd.Parameters.Add(CommandFactory.CreateParameter("@level0type", "SCHEMA"));
            cmd.Parameters.Add(CommandFactory.CreateParameter("@level0name", objectSchema));
            cmd.Parameters.Add(CommandFactory.CreateParameter("@level1type", level1Type.ToString()));
            cmd.Parameters.Add(CommandFactory.CreateParameter("@level1name", level1Name));
            if (level2Type != Level2Types.NULL)
            {
                cmd.Parameters.Add(CommandFactory.CreateParameter("@level2type", level2Type.ToString()));
                cmd.Parameters.Add(CommandFactory.CreateParameter("@level2name", level2Name));
            }
        }

        private string GetSQLTextForSaveMode(SaveModes saveMode)
        {
            string sql = string.Empty;

            if (string.IsNullOrEmpty(ApplicationSettings.Default.ExtendedPropKey))
                throw new ArgumentException("The ExtendedPropertyKey must have a value. Please change it in the Options");

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
            return sql;
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

        internal string GetSQLQueryText(EntityBase entity, string ConnectionString)
        {
            SaveModes saveMode = SaveModes.New;
            if (entity.IsNewDescription)
                saveMode = SaveModes.New;
            else
                saveMode = SaveModes.Update;

            if (string.IsNullOrEmpty(entity.Description))
            {
                saveMode = SaveModes.Delete;
            }

            if (entity.HasChanges)
            {
                string sql = GetSQLTextForSaveMode(saveMode);

                using (DbCommand cmd = CommandFactory.Create(sql, ConnectionString, System.Data.CommandType.StoredProcedure))
                {
                    FillCommand(cmd, saveMode, entity.Level1Type, entity.Description, entity.Schema, entity.Level1Name, entity.Level2Type, entity.Level2Name);

                    return SqlCommandDumper.GetCommandText((System.Data.SqlClient.SqlCommand)cmd);
                }
            }
            else
                return "";
        }
    }
}
