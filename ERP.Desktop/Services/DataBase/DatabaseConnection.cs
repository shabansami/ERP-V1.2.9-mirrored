using ERP.DAL.Utilites;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.Services.DataBase
{
    public static class DatabaseConnection
    {
        private static Configuration _config;

        //Get connection string from App.Config file
        //public static string GetConnection()
        //{
        //    _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    string encryptedConnection = _config.ConnectionStrings.ConnectionStrings["VTSEntities"].ConnectionString;
        //    return VTSAuth.Decrypt(encryptedConnection);
        //}

        //Save connection string to App.config file
        //public static void SaveConnection(string value)
        //{
        //    string encryptedConnection = VTSAuth.Encrypt(value);
        //    _config.ConnectionStrings.ConnectionStrings["VTSEntities"].ConnectionString = encryptedConnection;
        //    _config.ConnectionStrings.ConnectionStrings["VTSEntities"].ProviderName = "System.Data.SqlClient";
        //    _config.Save(ConfigurationSaveMode.Modified);
        //}

        public static string GetDecryptedSchema()
        {
            return VTSAuth.Decrypt(Properties.Settings.Default.Schema);
        }

        public static void SaveEncryptedSchema(string schema)
        {
            Properties.Settings.Default.Schema = VTSAuth.Encrypt(schema);
            Properties.Settings.Default.Save();
        }

        public static bool ValidConnection()
        {
            try
            {
                var db = DBContext.UnitDbContext;
                {
                    db.Database.Connection.Open();
                    db.Database.Connection.Close();
                }

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
