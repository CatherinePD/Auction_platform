using System.Configuration;

namespace Auction.DataAccess
{
    public static class ConnectionConstants
    {
        private static string _connectionStringName = "Watchman";

        public static string ConnectionString =>
            ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString;

        public static string DatabaseName
        {
            get
            {
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(ConnectionString);
                string database = builder.InitialCatalog;

                return database;
            }
        }
    }
}
