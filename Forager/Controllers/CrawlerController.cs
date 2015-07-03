using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Forager.Filters;

namespace Forager.Controllers
{
    public class CrawlerController : ApplicationController
    {
        [InitializeSimpleMembership]
        [Authorize]
        public ActionResult Start()
        {
            Crawler.CrawlerControl.Start(100);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Stop()
        {
            Crawler.CrawlerControl.Stop();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Pause()
        {
            Crawler.CrawlerControl.Pause();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Resume()
        {
            Crawler.CrawlerControl.Resume();
            return RedirectToAction("Index", "Home");
        }
       
    }
}
