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
    public class CampgroundSqlDAOTest : DatabaseTest
    {
        private CampgroundSqlDAO dao;


        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            dao = new CampgroundSqlDAO(ConnectionString);

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(
                    @"  SET IDENTITY_INSERT park ON
                        INSERT INTO park (park_id, name, location, establish_date, area, visitors, description) VALUES (1, 'FakePark', 'Somewhere', '1910-10-10', 10, 10, 'fun')
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
        public void GetCampgrounds()
        {
            IList<Campground> campgrounds = dao.GetCampgrounds(1);

            Assert.AreEqual(1, campgrounds.Count);
        }

        [TestMethod]
        public void GetCampgroundName()
        {
            string campgroundName = dao.GetCampgroundName(1);

            Assert.AreEqual("fakeCampground", campgroundName);
        }
    }
}
