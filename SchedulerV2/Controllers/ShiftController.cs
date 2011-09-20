using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchedulerV2.Models;

namespace SchedulerV2.Controllers
{
    public class ShiftController : Controller
    {
     
        public ActionResult Create(int locationId)
        {
            var shift = new Shift() { LocationID = locationId};
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(Shift shift)
        {
            Shift.Create(shift);
            return View(shift);
        }
    }
}
