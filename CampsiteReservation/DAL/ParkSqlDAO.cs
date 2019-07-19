using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ParkSqlDAO : IParkDAO
    {
        private string connectionString;
        private string ParkSelectQuery = "SELECT park_id, name, location, establish_date, area, visitors, description FROM park";

        public ParkSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public IList<Park> GetParks()
        {
            IList<Park> parks = new List<Park>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = ParkSelectQuery;

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                parks = MapResultsToParks(reader);
            }

            return parks;
        }

        public Park SelectPark(int parkId)
        {
            Park park = new Park();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $@"{ParkSelectQuery} WHERE park_id = @parkId";

                SqlCommand command = connection.CreateCommand();
                command.CommandText = query;

                command.Parameters.AddWithValue("@parkId", parkId);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                park = MapResultsToParks(reader)[0];
            }

            return park;
        }

        public IList<Park> MapResultsToParks(SqlDataReader reader)
        {
            IList<Park> parks = new List<Park>();

            while (reader.Read())
            {
                Park park = new Park();
                park.Id = Convert.ToInt32(reader["park_id"]);
                park.Name = reader["name"] as string;
                park.Location = reader["location"] as string;
                park.EstablishedDate = Convert.ToDateTime(reader["establish_date"]);
                park.Area = Convert.ToInt32(reader["area"]);
                park.AnnualVisitorCount = Convert.ToInt32(reader["visitors"]);
                park.Description = reader["description"] as string;

                parks.Add(park);
            }

            return parks;
        }
    }
}
