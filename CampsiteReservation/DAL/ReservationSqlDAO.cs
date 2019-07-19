using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ReservationSqlDAO : IReservationDAO
    {
        private string connectionString;

        public ReservationSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public int MakeReservation(Reservation reservation)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string insertProjectEmployee = @"INSERT INTO reservation (site_id, name, from_date, to_date, create_date) 
                                            VALUES (@siteID, @reservationName, @fromDate, @toDate, @now)
                                            SELECT SCOPE_IDENTITY()";

                SqlCommand command = connection.CreateCommand();
                command.CommandText = insertProjectEmployee;

                command.Parameters.AddWithValue("@siteId", reservation.SiteId);
                command.Parameters.AddWithValue("@reservationName", reservation.Name);
                command.Parameters.AddWithValue("@fromDate", reservation.FromDate);
                command.Parameters.AddWithValue("@toDate", reservation.ToDate);
                command.Parameters.AddWithValue("@now", DateTime.Now);

                connection.Open();

                reservation.Id = Convert.ToInt32(command.ExecuteScalar());
            }

                return reservation.Id;
        }


        public IList<Reservation> GetReservations(int selectedParkId)
        {
            IList<Reservation> reservations = new List<Reservation>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string getReservations = @"SELECT r.site_id, r.from_date, r.to_date
                                        FROM reservation r
                                        JOIN site s
                                        ON r.site_id = s.site_id
                                        JOIN campground cg
                                        ON s.campground_id = cg.campground_id
                                        JOIN park p
                                        ON cg.park_id = p.park_id
                                        WHERE (r.from_date < (@datetimecurrent + 30)) AND p.park_id = @selectedParkId";

                SqlCommand command = connection.CreateCommand();
                command.CommandText = getReservations;

                command.Parameters.AddWithValue("@datetimecurrent", DateTime.Today);
                command.Parameters.AddWithValue("@selectedParkId", selectedParkId);
               
                connection.Open();
                command.ExecuteNonQuery();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Reservation reservation = new Reservation();

                    reservation.SiteId = Convert.ToInt32(reader["site_id"]);
                    reservation.FromDate = Convert.ToDateTime(reader["from_date"]);
                    reservation.ToDate = Convert.ToDateTime(reader["to_date"]);

                    reservations.Add(reservation);
                }
            }

            return reservations;
        }
    }
}
