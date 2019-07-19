using System;
using System.Collections.Generic;
using System.Text;
using Capstone.DAL;
using Capstone.Models;

namespace Capstone
{
    public class CLIHelper
    {
        public bool ParkMenuSelectValid(int selectInt, int count)
        {
            bool valid = false;

            if (Convert.ToInt32(selectInt) <= count && Convert.ToInt32(selectInt) >= 1)
            {
                valid = true;
            }
            else
            {
                Console.WriteLine("Invalid entry, please try again.");
                Console.WriteLine();
            }

            return valid;
        }

        public bool CampgroundFilterMenuCamp(string input, int count)
        {
            bool done = false;

            try
            {
                Convert.ToInt32(input);
            }
            catch
            {
                Console.WriteLine("Invalid entry, please try again.");
                Console.WriteLine();
                return done;
            }

            done = (Convert.ToInt32(input) <= count && Convert.ToInt32(input) >= 1) ? true : false;

            if (!done)
            {
                Console.WriteLine("Invalid entry, please try again.");
                Console.WriteLine();
            }

            return done;
        }

        public bool DateIsValid(string input)
        {
            try
            {
                Convert.ToDateTime(input);
            }
            catch
            {
                Console.WriteLine("Invalid entry, please try again.");
                return false;
            }

            if(Convert.ToDateTime(input)< DateTime.Today)
            {
                Console.WriteLine("Please enter a date that is in the future.");
                Console.WriteLine();
                return false;
            }

            return true;
        }

        public bool MakeReservationByCampgroundMenu(string input, IList<Site> sites)
        {
            try
            {
                Convert.ToInt32(input);
            }
            catch
            {
                Console.WriteLine("Invalid entry, please try again.");
                Console.WriteLine();
                return false;
            }

            foreach (Site site in sites)
            {
                if (site.SiteNumber == Convert.ToInt32(input))
                {
                    return true;
                }
            }

            Console.WriteLine("Invalid entry, please try again.");
            Console.WriteLine();
            return false;
        }

        public List<string> Split(string inputString, int length)
        {
            List<string> splitString = new List<string>();

            int j = 0;

            for (int i = 0; (inputString.Length / (i + 100)) >= 1; i += length)
            {
                splitString.Add(inputString.Substring(i, length));
                j = i + 100;
            }

            if (inputString.Length % length != 0)
            {
                splitString.Add(inputString.Substring(j, inputString.Length % length));
            }

            return splitString;

        }

        public bool DatesValid(DateTime fromDate, DateTime toDate)
        {
            if ((toDate.Subtract(fromDate)).Days >= 0)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Time span between dates is negative, please try again.");
                return false;
            }
        }

        public bool CampgroundNameValid(string input, Dictionary<int, string> campgroundNames)
        {
            if (campgroundNames.ContainsValue(input))
            {
                foreach (KeyValuePair<int, string> kvp in campgroundNames)
                {
                    if (kvp.Value == input)
                    {
                        break;
                    }
                }

                return true;
            }
            else
            {
                Console.WriteLine("Not a listed campground, please try again.");
                return false;
            }
        }
    }
}
