using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using SchedulerV2.DataAccess.Dapper;

namespace SchedulerV2.Models
{
    public class Schedule
    {
        public int ScheduleID { get; set; }
        public Location Location { get; set; }
        public int CreatedByUserID { get; set; }
        public int LastUpdateUserID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Approved { get; set; }
        public int ApprovedBy { get; set; }

        public static IEnumerable<Schedule> ListScheduleForLocation(int locationId)
        {
            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                conn.Open();
                var sql = @"Select ScheduleID, LocationID, CreatedByUserID, LastUpdateUserID, StartDate, EndDate, Approved, ApprovedBy                                                    
                            From Schedules Where LocationID = @locationId";

                var schedules = conn.Query<Schedule>(sql, new { locationId = locationId });

                return schedules.ToList();
            }
        }
    }
}