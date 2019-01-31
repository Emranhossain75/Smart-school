using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartSchool.Controllers
{
    public class AttendanceController : Controller
    {
        // GET: Attendance/TakeAttendance // by teacher
        [HttpGet]
        public ActionResult TakeAttendance()
        {
            return View();
        }

        // POST: Attendance/TakeAttendance
        /*[HttpPost]
        public ActionResult TakeAttendance()
        {
            return View();
        }*/

        // GET: Attendance/ViewAttendance // by teacher
        [HttpGet]
        public ActionResult ViewAttendance()
        {
            return View();
        }

        // POST: Attendance/ViewAttendance
        /*[HttpPost]
        public ActionResult ViewAttendance()
        {
            return View();
        }*/
    }
}