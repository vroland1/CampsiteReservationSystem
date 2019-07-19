using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectOrganizer.Tests.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;
using Capstone.DAL;
using System.Data.SqlClient;

namespace Capstone.Tests.DAL
{
    [TestClass]
    public class ReservationSqlDAOTest : DatabaseTest
    {
        private ReservationSqlDAO dao;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            dao = new ReservationSqlDAO(ConnectionString);

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(
                    @"SET IDENTITY_INSERT park ON
                        INSERT INTO park (park_id, name, location, establish_date, area, visitors, description) VALUES (1, 'fakePark', 'somewhere', '1000-10-10', 10, 10, 'fun')
                        SET IDENTITY_INSERT park OFF
                        SET IDENTITY_INSERT campground ON
                        INSERT INTO campground (campground_id, park_id, name, open_from_mm, open_to_mm, daily_fee) VALUES (1, 1, 'fakeCampground', 05, 09, 100)
                        SET IDENTITY_INSERT campground OFF
                        SET IDENTITY_INSERT site ON
                        INSERT INTO site (site_id, campground_id, site_number, max_occupancy, accessible, max_rv_length, utilities) VALUES (1, 1, 1, 10, 1, 20, 1)
                        SET IDENTITY_INSERT site OFF"
                        , connection);


                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void MakeReservation()
        {
            int reservationId = 0;

            Reservation newReservation = new Reservation
            {
                SiteId = 1,
                FromDate = Convert.ToDateTime("2019-05-10"),
                ToDate = Convert.ToDateTime("2019-05-15"),
                Name = "Fake Reservation"
            };

            reservationId = dao.MakeReservation(newReservation);

            Assert.AreNotEqual(0, reservationId);
        }
    }
}
