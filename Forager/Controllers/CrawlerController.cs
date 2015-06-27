using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Forager.Filters;

namespace Forager.Controllers
{
    public class CrawlerController : Controller
    {
        [InitializeSimpleMembership]
        [Authorize]
        public ActionResult Start()
        {
            Crawler.CrawlerControl.Start(15);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Stop()
        {
            Crawler.CrawlerControl.Stop();
            return RedirectToAction("Index", "Home");
        }
       
    }
}
