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

        public static Shift Create(Shift shift)
        {
            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                conn.Open();
                var sql = @"Insert Into Shifts(locationid, name, start, [end]) 
                            Values (@locationId, @name, @start, @end)";
                conn.Execute(sql,
                    new
                    {
                        shift.LocationID,
                        shift.Name,
                        start = shift.Start.ToShortTimeString(),
                        end = shift.End.ToShortTimeString()
                    }
                );
                SetIdentity<int>(conn, id => shift.ShiftID = id);
                return shift;
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