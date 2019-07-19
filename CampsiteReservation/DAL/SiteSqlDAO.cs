using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;
using Capstone.DAL;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class SiteSqlDAO : ISiteDAO
    {
        private string connectionString;

        public SiteSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public IList<Site> SitesAvailableForReservationByCampground(int campgroundId, DateTime fromDate, DateTime toDate)
        {
            IList<Site> sites = new List<Site>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $@"SELECT DISTINCT s.site_id, s.campground_id, s.site_number, s.max_occupancy, s.accessible, 
                                s.max_rv_length, s.utilities FROM site s LEFT JOIN reservation r ON s.site_id = r.site_id 
                                JOIN campground cg ON s.campground_id = cg.campground_id WHERE cg.campground_id = @campgroundId 
                                AND (((@fromDate >= r.to_date OR @toDate <= r.from_date) AND (@fromDate != r.from_date AND @toDate 
                                != r.to_date)) OR (r.site_id IS NULL))";


                SqlCommand command = connection.CreateCommand();
                command.CommandText = query;

                command.Parameters.AddWithValue("@campgroundId", campgroundId);
                command.Parameters.AddWithValue("@fromDate", fromDate);
                command.Parameters.AddWithValue("@toDate", toDate);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                sites = MapResultsToSites(reader);
            }

            return sites;
        }

        public IList<Site> SitesAvailableForReservationByPark(int parkId, DateTime fromDate, DateTime toDate)
        {
            IList<Site> sites = new List<Site>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $@"SELECT DISTINCT s.site_id, s.campground_id, s.site_number, s.max_occupancy, 
                s.accessible, s.max_rv_length, s.utilities FROM site s LEFT JOIN reservation r ON s.site_id = r.site_id 
                JOIN campground cg ON s.campground_id = cg.campground_id JOIN park p ON cg.park_id = p.park_id WHERE 
                (p.park_id = @parkId AND (@fromDate >= r.to_date OR @toDate <= r.from_date)) OR (p.park_id = @parkId AND r.site_id IS NULL)";

                SqlCommand command = connection.CreateCommand();
                command.CommandText = query;

                command.Parameters.AddWithValue("@parkId", parkId);
                command.Parameters.AddWithValue("@fromDate", fromDate);
                command.Parameters.AddWithValue("@toDate", toDate);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                sites = MapResultsToSites(reader); 
            }

            return sites;
        }

        public IList<Site> MapResultsToSites(SqlDataReader reader)
        {
            IList<Site> sites = new List<Site>();

            while (reader.Read())
            {
                Site site = new Site();

                site.Id = Convert.ToInt32(reader["site_id"]);
                site.CampgroundId = Convert.ToInt32(reader["campground_id"]);
                site.SiteNumber = Convert.ToInt32(reader["site_number"]);
                site.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
                site.IsAccessible = Convert.ToBoolean(reader["accessible"]);
                site.MaxRvLength = Convert.ToInt32(reader["max_rv_length"]);
                site.UtilitiesAvailable = Convert.ToBoolean(reader["utilities"]);

                sites.Add(site);
            }

            return sites;
        }

        public int GetSiteNumber(int siteId)
        {
            int siteNumber = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $@"SELECT site_number FROM site WHERE site_id = @siteId";

                SqlCommand command = connection.CreateCommand();
                command.CommandText = query;

                command.Parameters.AddWithValue("@siteId", siteId);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    siteNumber = Convert.ToInt32(reader["site_number"]);
                }

            }

            return siteNumber;
        }
    }
}
