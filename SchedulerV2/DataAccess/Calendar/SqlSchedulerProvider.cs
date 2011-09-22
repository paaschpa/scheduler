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
        private IDictionary<int, Resource> _managers;

        private IDictionary<int, Resource> Managers
        {
            get
            {
                if (_managers == null)
                {
                    _managers = new Dictionary<int, Resource>();
                    foreach (Resource manager in LoadManagers())
                    {
                        _managers.Add((int)manager.Key, manager);
                    }
                }

                return _managers;
            }
        }

        public override IEnumerable<Appointment> GetAppointments(RadScheduler owner)
        {
            List<Appointment> appointments = new List<Appointment>();

            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                DbCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT [ID], [Subject], [Start], [End], [RecurrenceRule], [RecurrenceParentId] FROM [Appointments]";
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
                        else
                            if (apt.RecurrenceRule != string.Empty)
                            {
                                apt.RecurrenceState = RecurrenceState.Master;
                            }

                        //LoadResources(apt);
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
                               {new ResourceType("Type"), new[] {
                                                                    new Resource("Type", 1, "Standard"), 
                                                                    new Resource("Type", 2, "Vacation"),
                                                                    new Resource("Type", 3, "Borrowed")
                                                                } 
                               },                                
                               {new ResourceType("User"), new[]
                                                              {
                                                                  new Resource("User", 1, "TestUser")
                                                              } 
                               }
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
                        @"	INSERT	INTO [Appointments]
        									([ScheduleID], [TypeID], [Subject], [Start], [End],
        									[RecurrenceRule], [RecurrenceParentID])
        							VALUES	(@ScheduleID, @TypeID, @Subject, @Start, @End, @RecurrenceRule, @RecurrenceParentID)";

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

                    //FillClassStudents(appointmentToInsert, cmd, identity);

                    tran.Commit();
                }
            }        
        }

        private IEnumerable<Resource> LoadManagers()
        {
            List<Resource> resources = new List<Resource>();

            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                DbCommand cmd = DbFactory.CreateCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT [UserID], [UserName], [Phone] FROM [Users]";

                conn.Open();
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Resource res = new Resource();
                        res.Type = "Manager";
                        res.Key = reader["UserID"];
                        res.Text = Convert.ToString(reader["UserName"]);
                        //res.Attributes["Phone"] = Convert.ToString(reader["Phone"]);
                        resources.Add(res);
                    }
                }
            }

            return resources;
        }

        private void PopulateAppointmentParameters(DbCommand cmd, Appointment apt)
        {
            cmd.Parameters.Add(CreateParameter("@ScheduleID", apt.Attributes["ScheduleID"]));
            cmd.Parameters.Add(CreateParameter("@TypeID", apt.Resources[0].Key));
            cmd.Parameters.Add(CreateParameter("@Subject", apt.Subject));
            cmd.Parameters.Add(CreateParameter("@Start", apt.Start));
            cmd.Parameters.Add(CreateParameter("@End", apt.End));

            //Resource teacher = apt.Resources.GetResourceByType("Teacher");
            //object teacherId = null;
            //if (teacher != null)
            //{
            //    teacherId = teacher.Key;
            //}
            //cmd.Parameters.Add(CreateParameter("@TeacherID", teacherId));

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
    }
}