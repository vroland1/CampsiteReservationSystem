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
    public class SiteSqlDAOTest : DatabaseTest
    {
        private SiteSqlDAO dao;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            dao = new SiteSqlDAO(ConnectionString);

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(
                    @"  SET IDENTITY_INSERT park ON
                        INSERT INTO park (park_id, name, location, establish_date, area, visitors, description) VALUES (1, 'fakePark', 'somewhere', '1000-10-10', 10, 10, 'fun')
                        SET IDENTITY_INSERT park OFF
                        SET IDENTITY_INSERT campground ON
                        INSERT INTO campground (campground_id, park_id, name, open_from_mm, open_to_mm, daily_fee) VALUES (1, 1, 'fakeCampground', 05, 09, 100)
                        SET IDENTITY_INSERT campground OFF
                        SET IDENTITY_INSERT site ON
                        INSERT INTO site (site_id, campground_id, site_number, max_occupancy, accessible, max_rv_length, utilities) VALUES (1, 1, 1, 10, 1, 20, 1)
                        SET IDENTITY_INSERT site OFF
                        SET IDENTITY_INSERT reservation ON
                        INSERT INTO reservation (reservation_id, site_id, name, from_date, to_date, create_date) VALUES
                        (1, 1, 'Family Reservation', '2019-05-10', '2019-05-20', '2019-05-02 09:22:59:340')
                        SET IDENTITY_INSERT reservation OFF" , connection);


                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void SitesAvailable_are_available()
        {
            IList<Site> sites = dao.SitesAvailableForReservationByCampground(1, Convert.ToDateTime("2019-05-21"), Convert.ToDateTime("2019-05-25"));

            Assert.AreEqual(1, sites.Count);
        }

        [TestMethod]
        public void Single_date_overlap_test()
        {
            IList<Site> sites = dao.SitesAvailableForReservationByCampground(1, Convert.ToDateTime("2019-05-18"), Convert.ToDateTime("2019-05-25"));

            Assert.AreEqual(0, sites.Count);
        }

        [TestMethod]
        public void Full_date_overlap_test_within_previous_reservation()
        {
            IList<Site> sites = dao.SitesAvailableForReservationByCampground(1, Convert.ToDateTime("2019-05-11"), Convert.ToDateTime("2019-05-18"));

            Assert.AreEqual(0, sites.Count);
        }

        [TestMethod]
        public void Full_date_overlap_test_outside_reservation()
        {
            IList<Site> sites = dao.SitesAvailableForReservationByCampground(1, Convert.ToDateTime("2019-05-08"), Convert.ToDateTime("2019-05-25"));

            Assert.AreEqual(0, sites.Count);
        }

        [TestMethod]
        public void Overlapping_end_previous_reseration_date()
        {
            IList<Site> sites = dao.SitesAvailableForReservationByCampground(1, Convert.ToDateTime("2019-05-20"), Convert.ToDateTime("2019-05-25"));

            Assert.AreEqual(1, sites.Count);
        }
    }
}
