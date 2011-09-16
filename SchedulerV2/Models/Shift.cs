using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using SchedulerV2.DataAccess.DapperLibrary;

namespace SchedulerV2.Models
{
    public class Shift
    {
        public int ShiftID { get; set; }
        public int LocationID { get; set; }
        public String Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public static IEnumerable<Shift> ListForLocation(int locationId)
        {
            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                conn.Open();
                var sql = @"Select * From Shifts Where LocationID = @locationId";

                var shifts = conn.Query<Shift>(sql, new { locationId = locationId });

                return shifts;
            }
        }
    }
}