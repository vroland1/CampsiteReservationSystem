using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    interface ISiteDAO
    {
        IList<Site> SitesAvailableForReservationByCampground(int campground_id, DateTime from_date, DateTime to_date);

        IList<Site> SitesAvailableForReservationByPark(int parkId, DateTime fromDate, DateTime toDate);

        int GetSiteNumber(int siteId);

    }
}
