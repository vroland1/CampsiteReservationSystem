using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;

namespace Capstone.DAL
{
    interface IParkDAO
    {
        // returns list of parks sorted alphabetically by name
        IList<Park> GetParks();

        Park SelectPark(int parkId);
    }
}
