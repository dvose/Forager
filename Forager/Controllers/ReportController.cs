using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Forager.Models;
using System.Diagnostics;
using Forager.ViewModels;

namespace Waggle.Controllers
{
    public class ReportController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        //Uncomment this after integration to keep from viewing without being logged in!
        //[Authorize]
        public ActionResult Show(int ReportId, int SortType = 0)
        {
            //Old code for sending a single report to the View.
            /*
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
             */
            if (PageError.CurrentReportPEs.Count == 0 || PageError.CurrentReportPEs[0].ReportId != ReportId)
            {
                ErrorEntitiesContext db = new ErrorEntitiesContext();
                List<ErrorModel> ThisReportErrors = db.Errors.SqlQuery("SELECT * FROM Error WHERE ReportId = @p0", ReportId).ToList<ErrorModel>();
                //Debug.WriteLine("ThisReportErrors has " + ThisReportErrors.Count + " entries!");
                List<PageError> PageErrors = new List<PageError>();
                foreach (ErrorModel err in ThisReportErrors) //Go through every error on this report.
                {
                    bool PageFound = false;
                    foreach (PageError pError in PageErrors) //Look through each PageError already created and see if this error is on that page.
                    {
                        if (err.WebPage == pError.Page)
                        {
                            pError.AddError(err); //Found a page for this error. Add it and continue to the next error.
                            PageFound = true;
                            break;
                        }
                    }
                    if (!PageFound) //This page was not found, create a PageError for it and add it to that PageError.
                    {
                        PageError pe = new PageError()
                        {
                            ReportId = ReportId,
                            Page = err.WebPage
                        };
                        pe.AddError(err);
                        pe.Unfold = false;
                        PageErrors.Add(pe);
                    }
                }
                if (PageErrors.Count == 0) //If the list is empty (there are no errors), add a dummy one to avoid the View page breaking
                {
                    PageErrors.Add(new PageError()
                    {
                        ReportId = ReportId,
                        Page = "Empty!"
                    });
                }
                PageError.CurrentReportPEs = PageErrors;
            }
            ReportShow rs = new ReportShow()
            {
                PageErrors = PageError.CurrentReportPEs,
                SortType = SortType
            };
            
           switch(SortType){
               case 1: //Sorted by error id. Default.
                   rs.PageErrors.Sort(PageError.PageErrorID);
                   break;
               case 2: //Sort reports by number of errors on the report (descending)
                   rs.PageErrors.Sort();
                   break;
               case 3: //Sort reports by number of errors on the report (ascending)
                   rs.PageErrors.Sort(PageError.PageErrorAscending);
                   break;
               case 4: //Sort errors by error type
                   foreach(PageError pe in rs.PageErrors)
                   {
                       pe.GetErrorList().Sort(ErrorModel.ErrorModelByErrorType);
                   }
                   break;
               case 5: //Sort errors by broken link name
                   foreach (PageError pe in rs.PageErrors)
                   {
                       pe.GetErrorList().Sort(ErrorModel.ErrorModelByLinkName);
                   }
                   break;
               default: //0 or an invalid SortType was passed. Do no sorting.
                   break;
           }
            return View(rs);
        }
        public ActionResult ToggleFold(int PEIndex, int SortType)
        {
            try
            {
                PageError.CurrentReportPEs[PEIndex].Unfold = !PageError.CurrentReportPEs[PEIndex].Unfold;
                return RedirectToAction("Show", new { ReportId = PageError.CurrentReportPEs[PEIndex].ReportId, SortType = 0 });
            }
            catch (ArgumentOutOfRangeException a)
            {
                //Redirect to reports index!
                return null;
            }
        }

    }
}
