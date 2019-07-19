using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    interface ICampgroundDAO
    {
        // ability to select a park and view campgrounds
        IList<Campground> GetCampgrounds(int parkId);

        string GetCampgroundName(int siteId);
    }
}
