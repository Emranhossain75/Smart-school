using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartSchool.Models;

namespace SmartSchool.Controllers
{
    public class StudentsController : Controller
    {
        // GET: Students/AddStudents // by admin
        [HttpGet]
        public ActionResult AddStudents()
        {
            if (Request.Cookies.Get("admin") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("ERP", "ERP");
            }
        }

        // POST: Students/AddStudents
        [HttpPost]
        public ActionResult AddStudents(Students student)
        {
            string fileName = Path.GetFileNameWithoutExtension(student.ImageFile.FileName);
            student.ImagePath = student.Id + fileName + System.IO.Path.GetExtension(student.ImageFile.FileName);
            fileName = "~/StudentsImage/" + student.Id + fileName + System.IO.Path.GetExtension(student.ImageFile.FileName);
            student.ImageFile.SaveAs(Server.MapPath(fileName));
            try
            {
                if (ModelState.IsValid)
                {
                    
                    if (student.addStudents())
                    {
                        ViewBag.Message = "Student info recorded successfully";
                        ModelState.Clear();
                    }
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: Students/ViewStudents // by admin
        [HttpGet]
        public ActionResult ViewStudents()
        {
            if (Request.Cookies.Get("admin") != null)
            {
                Students students = new Students();
                students.Data = students.viewStudents();
                return View(students);
            }
            else
            {
                return RedirectToAction("ERP", "ERP");
            }
        }

        // POST: Students/ViewStudents
        [HttpPost]
        public ActionResult ViewStudents(Students students)
        {
            students.Data = students.viewStudents();
            return View(students);
        }

        // GET: Students/UpdateStudentInfo // by admin
        [HttpGet]
        public ActionResult UpdateStudentInfo(string id)
        {
            if (Request.Cookies.Get("admin") != null)
            {
                Students student = new Students();
                return View(student.viewStudents().Find(smodel => smodel.Id == id));
            }
            else
            {
                return RedirectToAction("ERP", "ERP");
            }
        }

        // POST: Students/UpdateStudentInfo
        [HttpPost]
        public ActionResult UpdateStudentInfo(string id, Students student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    student.updateStudentInfo();
                    return RedirectToAction("ViewStudents");
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: Students/Register // for students
        [HttpGet]
        public ActionResult Register()
        {
            if (Request.Cookies.Get("students") != null)
            {
                return RedirectToAction("Index", "Students");
            }
            else if (Request.Cookies.Get("admin") != null)
            {
                return RedirectToAction("Home", "ERP");
            }
            else
            {
                return View();
            }
        }

        

        // POST: Students/Login
        /*[HttpPost]
        public ActionResult Login()
        {
            return View();
        }*/
    }
}