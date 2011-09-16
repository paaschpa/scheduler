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

        public static Schedule FindById(int scheduleId)
        {
            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                conn.Open();
                var sql = @"Select Schedules.*, '' as SplitOn, Locations.* From Schedules Inner Join Locations On Schedules.LocationId = Locations.LocationId Where ScheduleID = @scheduleId";

                var schedule = conn.Query<Schedule, Location, Schedule>(sql, 
                    (sched, location) => { sched.Location = location; return sched; },
                    new { scheduleId = scheduleId }, null, true, "SplitOn");

                return schedule.SingleOrDefault();
            }
        }
        
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

        public static Schedule Create(Schedule schedule)
        {
            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                conn.Open();
                var sql = @"Insert Into Schedules (LocationID, CreatedByUserID, LastUpdateUserID, StartDate, EndDate, Approved, ApprovedBy)
                            Values(@LocationID, @CreatedByUserID, @LastUpdateUserID, @StartDate, @EndDate, @Approved, @ApprovedBy)";
                conn.Execute(sql, 
                    new { schedule.Location.LocationID, schedule.CreatedByUserID, schedule.LastUpdateUserID, schedule.StartDate, schedule.EndDate,
                    schedule.Approved, schedule.ApprovedBy}
                );
                SetIdentity<int>(conn, id => schedule.ScheduleID = id);
                return schedule;
            }
        }

        protected static void SetIdentity<T>(DbConnection connection, Action<T> setId)
        {
            dynamic identity = connection.Query("SELECT @@IDENTITY AS Id").Single();
            T newId = (T)identity.Id;
            setId(newId);
        }
    }
}