using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchedulerV2.Models;

namespace SchedulerV2.Controllers
{
    public class ScheduleController : Controller
    {
        public ActionResult Create(int? locationId)
        {
            var schedule = new Schedule();
            schedule.CreatedByUserID = Employee.FindByUserName(User.Identity.Name).EmployeeID;
            if (locationId.HasValue) schedule.Location = Location.FindById(locationId.Value);
          
            return View(schedule);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(Schedule schedule)
        {
            Schedule.Create(schedule);
            return View(schedule);
        }

    }
}
