using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    interface IReservationDAO
    {
        int MakeReservation(Reservation reservation);

        IList<Reservation> GetReservations(int selectedParkId);
    }
}
