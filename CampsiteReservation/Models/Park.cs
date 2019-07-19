using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Park
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime EstablishedDate { get; set; }
        public int Area { get; set; }
        public int AnnualVisitorCount { get; set; }
        public string Description { get; set; }
    }
}
