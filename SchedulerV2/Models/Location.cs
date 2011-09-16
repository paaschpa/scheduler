using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data.Common;
using System.Configuration;
using SchedulerV2.DataAccess.DapperLibrary;

namespace SchedulerV2.Models
{
    public class Location
    {
        public Location()
        {
            _shifts = null;
        }

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

        private IEnumerable<Shift> _shifts;
        public IEnumerable<Shift> Shifts { 
            get {
                if (_shifts == null)
                {
                    _shifts = Shift.ListForLocation(LocationID);
                    return _shifts;
                }
                return _shifts;
            } 
        }

        public static Location FindById(int locationId)
        {
            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                conn.Open();
                var sql = @"Select * From Locations Where LocationID = @locationId";

                var schedule = conn.Query<Location>(sql, new { locationId = locationId });

                return schedule.SingleOrDefault();
            }
        }

    }
}