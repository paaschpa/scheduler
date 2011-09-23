using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchedulerV2.Models;

namespace SchedulerV2.Controllers
{
    public class CalendarController : Controller
    {
        //
        // GET: /Calendar/
        public ActionResult Index(int? scheduleId)
        {
            //This gets passed to the Webservice's DataProvider to filter appointments by ScheduleID
            //Slightly obscure way and not good if Session gets packed with data.
            Session["ScheduleID"] = scheduleId.Value; 
            var schedule = Schedule.FindById(scheduleId.Value);            
            return View(schedule);
        }
    }
}
