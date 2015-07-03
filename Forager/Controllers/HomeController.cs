using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Forager.Filters;

namespace Forager.Controllers
{
    public class HomeController : ApplicationController
    {
         [InitializeSimpleMembership]
        [Authorize]
        public ActionResult Index()
        {
            ViewData["CrawlerPaused"] = Crawler.CrawlerControl.isPaused;
            return View();
        }

        public ActionResult LiveUpdate()
        {
            ViewData["update"] = Crawler.WebCrawler.liveUpdate;
            return PartialView("_LiveUpdate");
        }

        public ActionResult Updates()
        {
            return View();
        }
    }
}
