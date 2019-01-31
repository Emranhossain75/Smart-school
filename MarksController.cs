using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartSchool.Controllers
{
    public class MarksController : Controller
    {
        // GET: Marks/EntryMarks // by teacher
        [HttpGet]
        public ActionResult EntryMarks()
        {
            return View();
        }

        // POST: Marks/EntryMarks
        /*[HttpPost]
        public ActionResult EntryMarks()
        {
            return View();
        }*/

        // GET: Marks/ViewMarks // by teacher
        [HttpGet]
        public ActionResult ViewMarks()
        {
            return View();
        }

        // POST: Marks/ViewMarks
        /*[HttpPost]
        public ActionResult ViewMarks()
        {
            return View();
        }*/
    }
}