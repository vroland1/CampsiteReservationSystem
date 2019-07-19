using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Capstone.DAL;
using Capstone;

namespace Capstone 
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get the connection string from the appsettings.json file
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            string connectionString = configuration.GetConnectionString("Project");

            ICampgroundDAO campgroundDAO = new CampgroundSqlDAO(connectionString);
            IParkDAO parkDAO = new ParkSqlDAO(connectionString);
            IReservationDAO reservationDAO = new ReservationSqlDAO(connectionString);
            ISiteDAO siteDAO = new SiteSqlDAO(connectionString);

            ProjectCLI projectCLI = new ProjectCLI(campgroundDAO, parkDAO, reservationDAO, siteDAO);
            projectCLI.RunCLI();
        }
    }
}
