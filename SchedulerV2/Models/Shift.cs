using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class Shift
    {
        public int ShiftID { get; set; }
        public int LocationID { get; set; }        
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}