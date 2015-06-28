using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Forager.Filters;

namespace Forager.Controllers
{
    public class ApplicationController : Controller
    {
        public ApplicationController()
        {
            ViewData["CrawlerRunning"] = Crawler.CrawlerControl.inProgress;
            ViewData["Stopping"] = Crawler.WebCrawler.shouldStop;
        }  
    }
}
