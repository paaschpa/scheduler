using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;
using SchedulerV2.DataAccess.DapperLibrary;
using System.Data.SqlClient;
using System.Configuration;
using Telerik.Web.UI;

namespace SchedulerV2.DataAccess.Calendar
{
    public class SqlSchedulerProvider : DbSchedulerProviderBase
    {
        private IEnumerable<Resource> _types = new[] {
                                                        new Resource("Type", 1, "Standard"), 
                                                        new Resource("Type", 2, "Vacation"),
                                                        new Resource("Type", 3, "Borrowed")
                                                     };

        private DbProviderFactory _dbFactory = SqlClientFactory.Instance;
        public override DbProviderFactory DbFactory
        {
            get
            {
                return _dbFactory;
            }
            set
            {
                _dbFactory = value;
            }
        }

        private System.Web.SessionState.HttpSessionState _session;

        public SqlSchedulerProvider(System.Web.SessionState.HttpSessionState session)
        {
            _session = session;
        }

        public override IEnumerable<Appointment> GetAppointments(RadScheduler owner)
        {
            List<Appointment> appointments = new List<Appointment>();
            var employees = GetAllEmployees();

            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                DbCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = @"SELECT [ID], [ScheduleID], [TypeID], [Subject], [Start], [End], [UserID],
                                    [RecurrenceRule], [RecurrenceParentId] FROM [Appointments] WHERE [ScheduleID]=@ScheduleID";
                cmd.Parameters.Add(CreateParameter("@ScheduleID", _session["ScheduleID"]));

                conn.Open();
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Appointment apt = new Appointment();
                        apt.Owner = owner;
                        apt.ID = reader["ID"];
                        apt.Subject = Convert.ToString(reader["Subject"]);
                        apt.Start = DateTime.SpecifyKind(Convert.ToDateTime(reader["Start"]), DateTimeKind.Utc);
                        apt.End = DateTime.SpecifyKind(Convert.ToDateTime(reader["End"]), DateTimeKind.Utc);
                        apt.RecurrenceRule = Convert.ToString(reader["RecurrenceRule"]);
                        apt.RecurrenceParentID = reader["RecurrenceParentId"] == DBNull.Value ? null : reader["RecurrenceParentId"];

                        if (apt.RecurrenceParentID != null)
                        {
                            apt.RecurrenceState = RecurrenceState.Exception;
                        }
                        else if (apt.RecurrenceRule != string.Empty)
                        {
                            apt.RecurrenceState = RecurrenceState.Master;
                        }

                        var typeID = reader["TypeID"].ToString();
                        apt.Resources.Add(_types.Where(x => x.Key.ToString() == typeID).SingleOrDefault());

                        if (!String.IsNullOrEmpty(reader["UserID"].ToString()))
                        {
                            var employeeID = reader["UserID"].ToString();
                            apt.Resources.Add(employees.Where(x => x.Key.ToString() == employeeID).SingleOrDefault());
                        }

                        apt.Attributes.Add("ScheduleID", reader["ScheduleID"].ToString());

                        appointments.Add(apt);
                    }
                }
            }

            return appointments;
        }

        public override IDictionary<ResourceType, IEnumerable<Resource>> GetResources(ISchedulerInfo schedulerInfo)
        {
            var dict = new Dictionary<ResourceType, IEnumerable<Resource>>()
                           {
                               { new ResourceType("Type"), _types },                                
                               { new ResourceType("User"), GetAllEmployees() }
                           };
            return dict;
        }

        public override void Insert(RadScheduler owner, Appointment appointmentToInsert)
        {
            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                conn.Open();
                using (DbTransaction tran = conn.BeginTransaction())
                {
                    DbCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = tran;

                    PopulateAppointmentParameters(cmd, appointmentToInsert);

                    cmd.CommandText =
                        @"INSERT INTO [Appointments]
        									([ScheduleID], [TypeID], [Subject], [Start], [End], [UserID],
        									[RecurrenceRule], [RecurrenceParentID])
        							VALUES	(@ScheduleID, @TypeID, @Subject, @Start, @End, @UserID, @RecurrenceRule, @RecurrenceParentID)";

                    if (DbFactory is SqlClientFactory)
                    {
                        cmd.CommandText += Environment.NewLine + "SELECT SCOPE_IDENTITY()";
                    }
                    else
                    {
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "SELECT @@IDENTITY";
                    }
                    int identity = Convert.ToInt32(cmd.ExecuteScalar());

                    tran.Commit();
                }
            }
        }

        public override void Update(RadScheduler owner, Appointment appointmentToUpdate)
        {
            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                conn.Open();
                using (DbTransaction tran = conn.BeginTransaction())
                {
                    DbCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = tran;

                    PopulateAppointmentParameters(cmd, appointmentToUpdate);

                    cmd.CommandText =
                        @"UPDATE [Appointments]
        									set [ScheduleID]=@ScheduleID, [TypeID]=@TypeID, 
                                            [Subject]=@Subject, [Start]=@Start, 
                                            [End]=@End, [UserID]=@UserID,
                                            [RecurrenceRule]=@RecurrenceRule, [RecurrenceParentID] = @RecurrenceParentID
        				 WHERE ID=@ID";

                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
            }
        }

        public override void Delete(RadScheduler owner, Appointment appointmentToDelete)
        {
            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                conn.Open();
                using (DbTransaction tran = conn.BeginTransaction())
                {
                    DbCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = tran;

                    cmd.CommandText = @"DELETE [Appointments] WHERE ID=@ID";

                    cmd.Parameters.Add(CreateParameter("@ID", appointmentToDelete.ID));

                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
            }
        }

        private void PopulateAppointmentParameters(DbCommand cmd, Appointment apt)
        {
            cmd.Parameters.Add(CreateParameter("@ScheduleID", apt.Attributes["ScheduleID"] ?? _session["ScheduleID"]));
            cmd.Parameters.Add(CreateParameter("@TypeID", GetTypeFromResources(apt.Resources))); //If User is not entered Type becomes first element
            cmd.Parameters.Add(CreateParameter("@Subject", apt.Subject));
            cmd.Parameters.Add(CreateParameter("@Start", apt.Start));
            cmd.Parameters.Add(CreateParameter("@End", apt.End));

            string userID = null;
            if (apt.Resources[0].Type == "Employee") userID = apt.Resources[0].Key.ToString(); //Users/Employees are first element in Resources
            cmd.Parameters.Add(CreateParameter("@UserID", userID));

            if (apt.ID != null)
            {
                cmd.Parameters.Add(CreateParameter("@ID", apt.ID));
            }

            string rrule = null;
            if (apt.RecurrenceRule != string.Empty)
            {
                rrule = apt.RecurrenceRule;
            }
            cmd.Parameters.Add(CreateParameter("@RecurrenceRule", rrule));

            object parentId = null;
            if (apt.RecurrenceParentID != null)
            {
                parentId = apt.RecurrenceParentID;
            }
            cmd.Parameters.Add(CreateParameter("@RecurrenceParentId", parentId));

            cmd.Parameters.Add(CreateParameter("@Reminder", apt.Reminders.ToString()));
        }

        private object GetTypeFromResources(ResourceCollection resourceCollection)
        {
            if (resourceCollection[0].Type == "Type")
                return resourceCollection[0].Key;
            if (resourceCollection[1].Type == "Type")
                return resourceCollection[1].Key;

            return null;
        }

        private IEnumerable<Resource> GetAllEmployees()
        {
            var employees = new List<Resource>();
            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                conn.Open();

                DbCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = @"SELECT * FROM Employees";
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
                        var resource = new Resource("Employee", reader["EmployeeID"], name);
                        employees.Add(resource);
                    }
                }                
            }

            return employees;
        }
    }
}