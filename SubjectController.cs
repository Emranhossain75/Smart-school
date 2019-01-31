using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartSchool.Models;

namespace SmartSchool.Controllers
{
    public class SubjectController : Controller
    {
        // GET: Subject/AddSubject // by admin
        [HttpGet]
        public ActionResult AddSubject()
        {
            if (Request.Cookies.Get("admin") != null)
            {
                Subject sub = new Subject();
                return View(sub);
            }
            else
            {
                return RedirectToAction("ERP", "ERP");
            }
        }

        // POST: Subject/AddSubject
        [HttpPost]
        public ActionResult AddSubject(Subject sub)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    sub.viewClassWiseSection();
                    if (sub.addSubject())
                    {
                        ViewBag.Message = "Add successfully";
                        ModelState.Clear();
                    }
                }
                return View(sub);
            }
            catch
            {
                return View(sub);
            }
        }

        // GET: Subject/ViewSubject // by admin
        [HttpGet]
        public ActionResult ViewSubject()
        {
            if (Request.Cookies.Get("admin") != null)
            {
                Subject sub = new Subject();
                sub.Data = sub.viewSubject();
                return View(sub);
            }
            else
            {
                return RedirectToAction("ERP", "ERP");
            }
        }

        // POST: Subject/ViewSubject
        [HttpPost]
        public ActionResult ViewSubject(Subject sub)
        {
            sub.Data = sub.viewSubject();
            sub.Sections = sub.viewClassWiseSection();
            return View(sub);
        }

        // Subject/ViewSubject // by admin
        public ActionResult DeleteSubject(string id)
        {
            try
            {
                Subject sub = new Subject();
                if (sub.deleteSubject(id))
                {
                    ViewBag.AlertMsg = "Delete Successfully";
                }
                return RedirectToAction("ViewSubject");
            }
            catch
            {
                return RedirectToAction("ViewSubject"); ;
            }
        }

        // GET: Subject/AssignSubject // by admin
        [HttpGet]
        public ActionResult AssignSubject()
        {
            if (Request.Cookies.Get("admin") != null)
            {
                Subject subject = new Subject();
                return View(subject);
            }
            else
            {
                return RedirectToAction("ERP", "ERP");
            }
        }

        // POST: Subject/AssignSubject
        [HttpPost]
        public ActionResult AssignSubject(Subject subject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (subject.assignSubject())
                    {
                        ViewBag.Message = "Assign successfully";
                        ModelState.Clear();
                    }
                }
                return View(subject);
            }
            catch
            {
                return View(subject);
            }
        }

        // Subject/ViewAssign // by admin
        public ActionResult ViewAssign()
        {
            if (Request.Cookies.Get("admin") != null)
            {
                Subject subject = new Subject();
                return View(subject.viewAssign());
            }
            else
            {
                return RedirectToAction("ERP", "ERP");
            }
        }

        // GET: Subject/UpdateAssign // by admin
        [HttpGet]
        public ActionResult UpdateAssign(int id)
        {
            if (Request.Cookies.Get("admin") != null)
            {
                Subject subject = new Subject();
                return View(subject.viewAssign().Find(smodel => smodel.Id == id));
            }
            else
            {
                return RedirectToAction("ERP", "ERP");
            }
        }

        // POST: Subject/UpdateAssign
        [HttpPost]
        public ActionResult UpdateAssign(int id, Subject subject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    subject.updateAssign();
                    return RedirectToAction("ViewAssign");
                }
                return View(subject);
            }
            catch
            {
                return View(subject);
            }
        }
    }
}