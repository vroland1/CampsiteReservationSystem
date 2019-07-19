using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectOrganizer.Tests.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;
using Capstone.DAL;
using System.Data.SqlClient;

namespace Capstone.Tests
{
    [TestClass]
    public class ParkSqlDAOTest : DatabaseTest
    {
        private ParkSqlDAO dao;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            dao = new ParkSqlDAO(ConnectionString);

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(
                    @"  SET IDENTITY_INSERT park ON
                        INSERT INTO park (park_id, name, location, establish_date, area, visitors, description) VALUES (1, 'FakePark', 'Somewhere', '1910-10-10', 10, 10, 'fun')
                        SET IDENTITY_INSERT park OFF
                        INSERT INTO park (name, location, establish_date, area, visitors, description) VALUES ('FakePark2', 'Somewhere2', '1910-10-10', 10, 10, 'fun')",
                    connection);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void Get_All_Parks()
        {
            IList<Park> parks = dao.GetParks();

            Assert.AreEqual(2, parks.Count);
            Assert.AreEqual("FakePark", parks[0].Name);
        }

        [TestMethod]
        public void Select_Park_By_Valid_Park_Id()
        {
            Park park = dao.SelectPark(1);

            Assert.AreEqual("FakePark", park.Name);
        }
    }
}
