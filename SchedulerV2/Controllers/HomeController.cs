using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchedulerV2.Models;
using Telerik.Web.Mvc;

namespace SchedulerV2.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View(Employee.ListAllDOs());
        }

        [GridAction]
        public ActionResult GetSchedules(int? locationId)
        {
            return View(new GridModel<Schedule> { Data = Schedule.ListScheduleForLocation(locationId.Value)});
        }
    }
}
