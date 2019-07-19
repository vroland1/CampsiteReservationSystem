using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Capstone.Models;

namespace Capstone.DAL
{
    public class CampgroundSqlDAO : ICampgroundDAO
    {
        private string connectionString;

        public CampgroundSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public string GetCampgroundName(int siteId)
        {
            string campgroundName = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $@"SELECT cg.name FROM campground cg JOIN site s ON s.campground_id = 
                cg.campground_id WHERE site_id = @siteId";

                SqlCommand command = connection.CreateCommand();
                command.CommandText = query;

                command.Parameters.AddWithValue("@siteId", siteId);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    campgroundName = reader["name"] as string;
                }
            }

            return campgroundName;
        }

        public IList<Campground> GetCampgrounds(int selectedParkId)
        {
            IList<Campground> campgrounds = new List<Campground>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT campground_id, park_id, name, open_from_mm, open_to_mm, daily_fee FROM campground WHERE park_id = @selectedParkId";

                SqlCommand command = connection.CreateCommand();
                command.CommandText = query;

                command.Parameters.AddWithValue("@selectedParkId", selectedParkId);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Campground campground = new Campground();
                    campground.Id = Convert.ToInt32(reader["campground_id"]);
                    campground.ParkId = Convert.ToInt32(reader["park_id"]);
                    campground.Name = reader["name"] as string;
                    campground.OpeningMonth = Convert.ToInt32(reader["open_from_mm"]);
                    campground.ClosingMonth = Convert.ToInt32(reader["open_to_mm"]);
                    campground.DailyFee = Convert.ToDecimal(reader["daily_fee"]);
                
                    campgrounds.Add(campground);
                }
            }

            return campgrounds;
        }

    }
}
