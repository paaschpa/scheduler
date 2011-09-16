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
            var schedule = Schedule.FindById(scheduleId.Value);
            return View(schedule);
        }
    }
}
