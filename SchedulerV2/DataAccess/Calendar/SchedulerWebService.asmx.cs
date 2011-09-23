namespace SchedulerV2.DataAccess.Calendar
{
	using System.Collections.Generic;
	using System.Web.Script.Services;
	using System.Web.Services;
	using System.Web.SessionState;
	using Telerik.Web.UI;

	/// <summary>
	/// RadScheduler data service.
	/// </summary>
	[WebService]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ScriptService]
	public class SchedulerWebService : WebService, IRequiresSessionState
	{
		private WebServiceAppointmentController _controller;
		public const string ProviderSessionKey = "SchedulerWebServiceData";

		private WebServiceAppointmentController Controller
		{
			get
			{
				DbSchedulerProviderBase provider;
				if ((Session[ProviderSessionKey] == null))
				{                                        
					provider = new SqlSchedulerProvider();
					Session[ProviderSessionKey] = provider;
				}
				else
				{
					provider = (DbSchedulerProviderBase)Session[ProviderSessionKey];
				}

				if (_controller == null)
				{
					_controller = new WebServiceAppointmentController(provider);
				}

				return _controller;
			}
		}

		[WebMethod(EnableSession = true)]
		public IEnumerable<AppointmentData> GetAppointments(SchedulerInfo schedulerInfo)
		{
			return Controller.GetAppointments(schedulerInfo);
		}

		[WebMethod(EnableSession = true)]
		public IEnumerable<AppointmentData> InsertAppointment(SchedulerInfo schedulerInfo, AppointmentData appointmentData)
		{
			return Controller.InsertAppointment(schedulerInfo, appointmentData); 
		}

		[WebMethod(EnableSession = true)]
		public IEnumerable<AppointmentData> UpdateAppointment(SchedulerInfo schedulerInfo, AppointmentData appointmentData)
		{
            if (appointmentData.ID == null)
                return Controller.InsertAppointment(schedulerInfo, appointmentData); //Inserts Or Updates the Appointment

            return Controller.UpdateAppointment(schedulerInfo, appointmentData);
        }

		[WebMethod(EnableSession = true)]
		public IEnumerable<AppointmentData> CreateRecurrenceException(SchedulerInfo schedulerInfo, AppointmentData recurrenceExceptionData)
		{
			return Controller.CreateRecurrenceException(schedulerInfo, recurrenceExceptionData);
		}

		[WebMethod(EnableSession = true)]
		public IEnumerable<AppointmentData> RemoveRecurrenceExceptions(SchedulerInfo schedulerInfo, AppointmentData masterAppointmentData)
		{
			return Controller.RemoveRecurrenceExceptions(schedulerInfo, masterAppointmentData);
		}

		[WebMethod(EnableSession = true)]
		public IEnumerable<AppointmentData> DeleteAppointment(SchedulerInfo schedulerInfo, AppointmentData appointmentData, bool deleteSeries)
		{
            if (appointmentData.ID == null)
                return null;
			return Controller.DeleteAppointment(schedulerInfo, appointmentData, deleteSeries);
		}

		[WebMethod(EnableSession = true)]
		public IEnumerable<ResourceData> GetResources(SchedulerInfo schedulerInfo)
		{
			return Controller.GetResources(schedulerInfo);
		}

	}


}
