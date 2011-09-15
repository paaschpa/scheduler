using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchedulerV2.Models
{
    public class Location
    {
        public int LocationID { get; set; }
        public int BrandID { get; set; }
        public String Number { get; set; }
        public string Name { get; set; }
        public String City { get; set; }
        public Employee DOUser { get; set; }
        public int GMUserID { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }
        public String Email { get; set; }
        public int ParentID { get; set; }


    }
}