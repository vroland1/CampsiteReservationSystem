using Capstone.DAL;
using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Capstone
{
    class ProjectCLI
    {
        private ICampgroundDAO campgroundDAO;
        private IParkDAO parkDAO;
        private IReservationDAO reservationDAO;
        private ISiteDAO siteDAO;

        private CLIHelper helper = new CLIHelper();

        IList<Campground> campgrounds = new List<Campground>();
        IList<Site> sites = new List<Site>();

        private Dictionary<int, decimal> campgroundDailyFees = new Dictionary<int, decimal>();
        private Dictionary<int, string> campgroundNames = new Dictionary<int, string>();

        private Reservation reservationRequest = new Reservation();

        public int selectedParkId;
        public string selectedParkName;
        public int selectedCampgroundId;

        public ProjectCLI (ICampgroundDAO campgroundDAO, IParkDAO parkDAO, IReservationDAO reservationDAO, ISiteDAO siteDAO)
        {
            this.campgroundDAO = campgroundDAO;
            this.parkDAO = parkDAO;
            this.reservationDAO = reservationDAO;
            this.siteDAO = siteDAO;
        }

        public void RunCLI()
        {
            ViewParks();
        }

        public void ViewParks()
        {
            bool done = false;

            Console.WriteLine("Select a Park for Further Details:");
            Console.WriteLine();
            Console.WriteLine("──▒▒▒▒▒▒▒▒───▒▒▒▒▒▒▒▒───▒▒▒▒▒▒▒▒──");
            Console.WriteLine("─▒▐▒▐▒▒▒▒▌▒─▒▒▌▒▒▐▒▒▌▒─▒▒▌▒▒▐▒▒▌▒─");
            Console.WriteLine("──▒▀▄█▒▄▀▒───▒▀▄▒▌▄▀▒───▒▀▄▒▌▄▀▒──");
            Console.WriteLine("─────██─────────██─────────██─────");
            Console.WriteLine("░░░▄▄██▄░░░░░░░▄██▄░░░░░░░▄██▄░░░░");
            Console.WriteLine();

            
            while (!done)
            {
                IList<Park> parks = parkDAO.GetParks();
                int count = parks.Count;

                for (int i = 0; i < parks.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {parks[i].Name}");
                }

                Console.WriteLine("Q) quit");
                Console.WriteLine();

                string select = Console.ReadLine().ToLower();
                Console.WriteLine();

                if (select == "q")
                {
                    Environment.Exit(1);
                }

                int.TryParse(select, out selectedParkId);
                done = helper.ParkMenuSelectValid(selectedParkId, count);
            }

            ParkDescription();
        }

        public void ParkDescription()
        {
            Park selectedPark = parkDAO.SelectPark(selectedParkId);
            selectedParkName = selectedPark.Name;

            Console.Clear();
            Console.WriteLine("Park Information");
            Console.WriteLine();
            Console.WriteLine($"{selectedParkName} National Park");
            Console.WriteLine($"Location:        {selectedPark.Location}");
            Console.WriteLine($"Established:     {selectedPark.EstablishedDate.ToString("yyyy/MM/dd")}");
            Console.WriteLine($"Area:            {selectedPark.Area} sq km");
            Console.WriteLine($"Annual Visitors: {selectedPark.AnnualVisitorCount}");
            Console.WriteLine();

            List<string> splitLines = helper.Split(selectedPark.Description, 100);
            foreach (string line in splitLines)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine();

            MainMenu();
        }

        public void MainMenu()
        {
            Console.WriteLine("Select a Command");
            Console.WriteLine("1) View Campgrounds");
            Console.WriteLine("2) Search for Reservation");
            Console.WriteLine("3) View Upcoming Reservations");
            Console.WriteLine("4) Return to Previous Screen");
            string command = Console.ReadLine();

            switch (command)
            {
                case "1":
                    Console.Clear();
                    Console.WriteLine("Park Campgrounds");
                    Console.WriteLine($"{selectedParkName} National Park Campgrounds");
                    DisplayCampgrounds();
                    break;
                case "2":
                    ParkFilterMenu();
                    break;
                case "3":
                    DisplayReservations(selectedParkId);
                    break;
                case "4":
                    Console.Clear();
                    ViewParks();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("The command provided was not a valid command, please try again.");                        
                    MainMenu();
                    break;
            }

            CampgroundReservationMenu();
        }

        public void DisplayCampgrounds()
        {
            campgrounds = campgroundDAO.GetCampgrounds(selectedParkId);

            CreateCampgroundDictionaries(campgrounds);

            DateTimeFormatInfo mfi = new DateTimeFormatInfo();

            Console.WriteLine("       Name                                Open          Close         Daily Fee");

            for (int i = 0; i < campgrounds.Count; i++)
            {
                Console.WriteLine("{0,-5}  {1,-32}    {2,-10}    {3,-10}    {4,-5}", $"{i + 1})", campgrounds[i].Name, mfi.GetMonthName(campgrounds[i].OpeningMonth), mfi.GetMonthName(campgrounds[i].ClosingMonth), (campgrounds[i].DailyFee).ToString("$0.00"));
            }

            Console.WriteLine();
        }

        public void CreateCampgroundDictionaries(IList<Campground> campgrounds)
        {
            for (int i = 0; i < campgrounds.Count; i++)
            {
                campgroundDailyFees[campgrounds[i].Id] = campgrounds[i].DailyFee;
                campgroundNames[campgrounds[i].Id] = campgrounds[i].Name;
            }
        }

        public void CampgroundReservationMenu()
        {
            bool done = false;

            while (!done)
            {
                Console.WriteLine("Select a Command");
                Console.WriteLine("1) Search for Available Reservation");
                Console.WriteLine("2) Return to Previous Screen");

                string command = Console.ReadLine();

                switch (command)
                {
                    case "1":
                        done = true;
                        Console.Clear();
                        Console.WriteLine("Search For Campground Reservation");
                        DisplayCampgrounds();
                        CampgroundFilterMenu();
                        break;

                    case "2":
                        done = true;
                        Console.Clear();
                        ParkDescription();
                        MainMenu();
                        break;

                    default:
                        Console.WriteLine("The command provided was not a valid command, please try again.");
                        break;
                }
            }
        }

        public void CampgroundFilterMenu()
        {
            SelectCampground_ReservationByCampground();

            SelectDates();

            sites = siteDAO.SitesAvailableForReservationByCampground(selectedCampgroundId, reservationRequest.FromDate, reservationRequest.ToDate);
            AreThereSitesAvailable(sites);
            int days = (reservationRequest.ToDate.Subtract(reservationRequest.FromDate)).Days + 1;

            AvailableSitesByCampgroundList(sites, days);
        }

        public void SelectCampground_ReservationByCampground()
        {
            bool done = false;
            string input = "";

            while (!done)
            {
                Console.WriteLine("Which campground (enter 0 to cancel)?");
                input = Console.ReadLine();
                Console.WriteLine();

                switch (input)
                {
                    case "0":
                        Console.Clear();
                        DisplayCampgrounds();
                        CampgroundReservationMenu();
                        break;
                    default:
                        done = helper.CampgroundFilterMenuCamp(input, campgroundNames.Count);
                        break;
                }

                selectedCampgroundId = (done) ? campgrounds[Convert.ToInt32(input) - 1].Id : selectedCampgroundId;
            }
        }

        public void SelectDates()
        {
            bool done = false;

            while (!done)
            {
                ArrivalDate();

                DepartureDate();

                done = helper.DatesValid(reservationRequest.FromDate, reservationRequest.ToDate);
            }
        }

        public void ArrivalDate()
        {
            bool done = false;
            string input = "";

            while (!done)
            {
                Console.WriteLine("What is the arrival date? (YYYY-MM-DD)");
                input = Console.ReadLine();
                Console.WriteLine();
                done = helper.DateIsValid(input);
            }

            reservationRequest.FromDate = Convert.ToDateTime(input);
        }

        public void DepartureDate()
        {
            bool done = false;
            string input = "";

            while (!done)
            {
                Console.WriteLine("What is the departure date? (YYYY-MM-DD)");
                input = Console.ReadLine();
                Console.WriteLine();
                done = helper.DateIsValid(input);
            }
            reservationRequest.ToDate = Convert.ToDateTime(input);
        }

        public void AreThereSitesAvailable(IList<Site> sites)
        {
            if (sites.Count == 0)
            {
                bool done = false;

                while (!done)
                {
                    Console.WriteLine("We are sorry, no sites seem to be available. Would you like to chose other dates? (Y/N)");
                    string input = Console.ReadLine().ToLower();

                    switch (input)
                    {
                        case "y":
                            done = true;
                            SelectDates();
                            break;
                        case "n":
                            done = true;
                            ViewParks();
                            break;
                        default:
                            Console.WriteLine("Please enter (y)es or (n)o.");
                            break;
                    }
                }
            }
        }

        public void AvailableSitesByCampgroundList(IList<Site> sites, int days)
        {
            Console.WriteLine("Results Matching Your Search Criteria");
            Console.WriteLine("Site No.   Max Occup.   Accessible?   Max RV Length   Utility    Cost");

            for (int i = 0; i < sites.Count; i++)
            {
                string utilitiesAvailable = (sites[i].UtilitiesAvailable == true) ? "Yes" : "N/A";
                string isAccessible = (sites[i].IsAccessible == true) ? "Yes" : "No";
                string maxRvLengthNumOrNA = (sites[i].MaxRvLength == 0) ? "N/A" : Convert.ToString(sites[i].MaxRvLength);

                Console.WriteLine(String.Format("   {0,-12}{1,-13}{2,-8}      {3,-8}      {4,-5}   {5,-5}", sites[i].SiteNumber, sites[i].MaxOccupancy, isAccessible, maxRvLengthNumOrNA, utilitiesAvailable, (days * campgroundDailyFees[selectedCampgroundId]).ToString("$0.00")));
            }

            MakeReservationByCampgroundMenu(sites);
        }

        public void MakeReservationByCampgroundMenu(IList<Site> sites)
        {
            int siteNumber = Convert.ToInt32(SelectSiteByCampground(sites));

            reservationRequest.SiteId = FindSiteId(sites, siteNumber);

            Console.WriteLine("What name should the reservation be made under?");
            reservationRequest.Name = Console.ReadLine();
            Console.WriteLine();

            reservationDAO.MakeReservation(reservationRequest);

            Console.WriteLine($"The reservation has been made and the confirmation id is {reservationRequest.Id}");
            Console.WriteLine();

            EndMenu();
        }

        public string SelectSiteByCampground(IList<Site> sites)
        {
            string input = "";
            bool done = false;

            while (!done)
            {
                Console.WriteLine("Which site should be reserved (enter 0 to cancel)?");
                input = Console.ReadLine();

                switch (input)
                {
                    case "0":
                        done = true;
                        Console.Clear();
                        Console.WriteLine("Search For Campground Reservation");
                        DisplayCampgrounds();
                        CampgroundFilterMenu();
                        break;
                    default:
                        done = helper.MakeReservationByCampgroundMenu(input, sites);
                        break;
                }
            }

            return input;
        }

        public int FindSiteId(IList<Site> sites, int siteNumber)
        {
            int siteId = 0;

            foreach (Site site in sites)
            {
                if (site.SiteNumber == siteNumber)
                {
                    siteId = site.Id;
                    break;
                }
            }

            return siteId;
        }

        public void EndMenu()
        {
            bool done = false;
            while (!done)
            {
                Console.WriteLine("Would you like to make another reservation? (Y/N)");
                string input = Console.ReadLine().ToLower();

                switch (input)
                {
                    case "y":
                        Console.Clear();
                        ViewParks();
                        break;
                    case "n":
                        Environment.Exit(1);
                        break;
                    default:
                        Console.WriteLine("Please choose Y or N, we are almost done.");
                        break;
                }
            }
        }

        public void ParkFilterMenu()
        {
            SelectDates();

            sites = siteDAO.SitesAvailableForReservationByPark(selectedParkId, reservationRequest.FromDate, reservationRequest.ToDate);
            int days = (reservationRequest.ToDate.Subtract(reservationRequest.FromDate)).Days + 1;

            AvailableSitesByParkList(sites, days);
        }

        public void AvailableSitesByParkList(IList<Site> sites, int days)
        {
            campgrounds = campgroundDAO.GetCampgrounds(selectedParkId);

            CreateCampgroundDictionaries(campgrounds);

            Console.WriteLine("Results Matching Your Search Criteria");
            Console.WriteLine("Campground                       Site No.   Max Occup.   Accessible?   Max RV Length   Utility    Cost");

            for (int i = 0; i < sites.Count; i++)
            {
                string utilitiesAvailable = (sites[i].UtilitiesAvailable == true) ? "Yes" : "N/A";
                string isAccessible = (sites[i].IsAccessible == true) ? "Yes" : "No";
                string maxRvLengthNumOrNA = (sites[i].MaxRvLength == 0) ? "N/A" : Convert.ToString(sites[i].MaxRvLength);

                Console.WriteLine(String.Format("{0,-30}      {1,-6}      {2,-6}       {3,-6}         {4,-6}       {5,-5}   {6,-5}", campgroundNames[sites[i].CampgroundId], sites[i].SiteNumber, sites[i].MaxOccupancy, isAccessible, maxRvLengthNumOrNA, utilitiesAvailable, (days * campgroundDailyFees[sites[i].CampgroundId]).ToString("$0.00")));
            }

            MakeReservationByPark(sites);
        }

        public void MakeReservationByPark(IList<Site> sites)
        {
            SelectCampground_ReservationByPark();

            int siteNumber = Convert.ToInt32(SelectSiteNumber_ReservationByPark(sites));

            reservationRequest.SiteId = FindSiteId(sites, siteNumber);

            Console.WriteLine("What name should the reservation be made under?");
            reservationRequest.Name = Console.ReadLine();

            reservationDAO.MakeReservation(reservationRequest);

            Console.WriteLine($"The reservation has been made and the confirmation id is {reservationRequest.Id}");
            Console.WriteLine();

            EndMenu();
        }

        public void SelectCampground_ReservationByPark()
        {
            string input = "";
            bool done = false;

            while (!done)
            {
                Console.WriteLine("Which campground would you like to camp at? (enter 0 to cancel)");

                input = Console.ReadLine();

                if (input == "0")
                {
                    ParkDescription();
                }

                done = helper.CampgroundNameValid(input, campgroundNames);
            }
        }

        public string SelectSiteNumber_ReservationByPark(IList<Site> sites)
        {
            bool done = false;
            string input = "";

            while (!done)
            {
                Console.WriteLine("Which site number should be reserved?");
                input = Console.ReadLine();

                done = helper.MakeReservationByCampgroundMenu(input, sites);

            }

            return input;
        }

        public void DisplayReservations(int selectedParkId)
        {
            Console.Clear();
            IList<Reservation> reservations = reservationDAO.GetReservations(selectedParkId);

            Console.WriteLine("Campgound                       Site Number        From          To");
            foreach(Reservation reservation in reservations)
            {
                int siteNumber = siteDAO.GetSiteNumber(reservation.SiteId);
                string campgroundName = campgroundDAO.GetCampgroundName(reservation.SiteId);

                Console.WriteLine("{0,-30}       {1,-5}       {2,-10}    {3,-10}", campgroundName, siteNumber, (reservation.FromDate).ToString("yyyy/MM/dd"), (reservation.ToDate).ToString("yyyy/MM/dd"));
            }

            Console.WriteLine();
            Console.WriteLine("Press ENTER to go back.");
            Console.ReadLine();
            Console.Clear();
            ParkDescription();
            MainMenu();
        }

       
        
    }
}
