using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;

namespace ProjectOrganizer.Tests.DAL
{
    [TestClass]
    public abstract class DatabaseTest
    {
        private IConfigurationRoot config;

        /// <summary>
        /// The transaction for each test.
        /// </summary>
        private TransactionScope transaction;

        /// <summary>
        /// The Configuration options specified in appsettings.json
        /// </summary>
        protected IConfigurationRoot Config
        {
            get
            {
                if (config == null)
                {
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");

                    config = builder.Build();
                }
                return config;
            }
        }

        /// <summary>
        /// The database connection string derived from the configuration settings
        /// </summary>
        protected string ConnectionString
        {
            get
            {
                return Config.GetConnectionString("Project");
            }
        }

        [TestInitialize]
        public virtual void Setup()
        {
            // Begin the transaction
            transaction = new TransactionScope();

            // Get the SQL Script to run
            string sql = @"DELETE FROM reservation;
                           DELETE FROM site;
                           DELETE FROM campground;
                           DELETE FROM park;";

            // Execute the script
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Roll back the transaction
            transaction.Dispose();
        }

        /// <summary>
        /// Gets the row count for a table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        protected int GetRowCount(string table)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) FROM {table}", conn);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}
