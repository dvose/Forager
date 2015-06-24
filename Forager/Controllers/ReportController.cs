using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Forager.Models;

namespace Forager.Controllers
{
    public class ReportController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Show(int ReportId)
        {
            
            ReportEntitiesContext reports = new ReportEntitiesContext();
            ReportModel r = reports.Reports.Find(ReportId);
            if (r == null)
            {
                return View("ERROR REPORT NOT FOUND");
            }
            ErrorEntitiesContext db = new ErrorEntitiesContext();
            r.Errors = new List<ErrorModel>();
            foreach(ErrorModel err in db.Errors)
            {
                if(err.ReportId == ReportId)
                {
                    r.Errors.Add(err);
                }
            }
            return View(r);
             
            /*
            ErrorEntitiesContext db = new ErrorEntitiesContext();
            List<PageError> PageErrors = new List<PageError>();
            foreach(ErrorModel err in db.Errors)
            {

            }
            return View();
             * */
        }

    }
}
