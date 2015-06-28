﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Forager.Filters;

namespace Forager.Controllers
{
    public class HomeController : Controller
    {
         [InitializeSimpleMembership]
        [Authorize]
        public ActionResult Index()
        {
            ViewData["CrawlerRunning"] = Crawler.CrawlerControl.inProgress;
            ViewData["Stopping"] = Crawler.WebCrawler.shouldStop;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
