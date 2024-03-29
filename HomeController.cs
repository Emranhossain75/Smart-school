﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartSchool.Controllers
{
    public class HomeController : Controller
    {
        // Home/Index
        public ActionResult Index()
        {
            if (Request.Cookies.Get("admin") != null)
            {
                return RedirectToAction("Home", "ERP");
            }
            else
            {
                return View();
            }
        }

        // Home/About
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        // Home/Contact
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}