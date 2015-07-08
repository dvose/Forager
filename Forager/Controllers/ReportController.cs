using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Forager.Models;
using System.Diagnostics;
using Forager.ViewModels;

namespace Forager.Controllers
{
    public class ReportController : ApplicationController
    {
        [Authorize]
        public ActionResult Index()
        {
            
            using (ReportEntitiesContext db = new ReportEntitiesContext()) {
                var reports = db.Reports.ToList();
                return View(reports); 
            }
            
            
        }
        [Authorize]
        public ActionResult ShowPDF(int ReportId) 
        {
            ReportShow reportShow = new ReportShow();
            ReportModel report = null;
            using (ReportEntitiesContext db = new ReportEntitiesContext())
            {
                report = db.Reports.Find(ReportId);
            }

            using (ErrorEntitiesContext db = new ErrorEntitiesContext())
            {
                report.Errors = db.Errors.SqlQuery("SELECT * FROM Error WHERE ReportId = @p0", ReportId).ToList<ErrorModel>();
            }
            List<PageError> pageErrors = BuildPageErrorList(ReportId);

            reportShow.Report = report;
            reportShow.PageErrors = pageErrors;
            
            return View(reportShow);
        }

        [Authorize]
        public ActionResult Show(int ReportId, int SortType = 0)
        {
            if (PageError.CurrentReportPEs.Count == 0 || PageError.CurrentReportPEs[0].ReportId != ReportId)
            {
                PageError.CurrentReportPEs = BuildPageErrorList(ReportId);
            }
            //Copy pasted because lazy
            List<ErrorModel> ThisReportErrors2;
            using (ErrorEntitiesContext db = new ErrorEntitiesContext())
            {
                ThisReportErrors2 = db.Errors.SqlQuery("SELECT * FROM Error WHERE ReportId = @p0", ReportId).ToList<ErrorModel>();
            }
            ReportModel rep = null;
            using (ReportEntitiesContext rdb = new ReportEntitiesContext())
            {
                rep = rdb.Reports.Find(ReportId);
                rep.Errors = ThisReportErrors2;
            }
            ReportShow rs = new ReportShow()
            {
                PageErrors = PageError.CurrentReportPEs,
                SortType = SortType,
                Report = rep
            };

            DoSort(SortType, rs.PageErrors);
           
            return View(rs);
        }

        [Authorize]
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
        [Authorize]
        public ActionResult BeginCompare(string ReportDropdown1, string ReportDropdown2)
        {
            int rep1 = Int32.Parse(ReportDropdown1);
            int rep2 = Int32.Parse(ReportDropdown2);
            return RedirectToAction("Compare", new { ReportId1 = rep1, ReportId2 = rep2, SortType = 0 });
        }
        [Authorize]
        public ActionResult Compare(int ReportId1, int ReportId2, int SortType)
        {
            if(ReportCompare.CurrentCompare != null && ReportCompare.CurrentCompare.Report1.Report.Id == ReportId1 && ReportCompare.CurrentCompare.Report2.Report.Id == ReportId2)
            {
                return View(ReportCompare.CurrentCompare);
            }

            ReportCompare rc = new ReportCompare();
            rc.Report1.PageErrors = BuildPageErrorList(ReportId1);
            rc.Report2.PageErrors = BuildPageErrorList(ReportId2);
            using(ReportEntitiesContext reports = new ReportEntitiesContext())
            {
                rc.Report1.Report = reports.Reports.Find(ReportId1);
                rc.Report2.Report = reports.Reports.Find(ReportId2);
                if (rc.Report1.Report == null || rc.Report2.Report == null)
                {
                    //Invalid ReportId. Redirect to index
                    return RedirectToAction("Index");
                }
            }
            if (SortType > 0)
            {
                DoSort(SortType, rc.Report1.PageErrors);
                DoSort(SortType, rc.Report2.PageErrors);
            }
            ReportCompare.CurrentCompare = rc;
            return View(rc);
        }
        [Authorize]
        public ActionResult ToggleFoldCompare(bool Rep1, int PEIndex)
        {
            try
            {
                List<PageError> PEs = Rep1 ? ReportCompare.CurrentCompare.Report1.PageErrors : ReportCompare.CurrentCompare.Report2.PageErrors;
                PEs[PEIndex].Unfold = !PEs[PEIndex].Unfold;
            }
            catch(IndexOutOfRangeException e)
            {
                //PEIndex was invalid. Don't do anything special, just redirect back to the compare page.
            }
            return RedirectToAction("Compare", new { ReportId1 = ReportCompare.CurrentCompare.Report1.Report.Id, ReportId2 = ReportCompare.CurrentCompare.Report2.Report.Id, SortType = 0 });
        }
        [Authorize]
        public ActionResult SortCompare(int SortType)
        {
            DoSort(SortType, ReportCompare.CurrentCompare.Report1.PageErrors);
            DoSort(SortType, ReportCompare.CurrentCompare.Report2.PageErrors);
            return RedirectToAction("Compare", new { ReportId1 = ReportCompare.CurrentCompare.Report1.Report.Id, ReportId2 = ReportCompare.CurrentCompare.Report2.Report.Id, SortType = 0 });
        }

        private static List<PageError> BuildPageErrorList(int ReportId)
        {
            List<PageError> PageErrors = new List<PageError>();
            List<ErrorModel> ThisReportErrors;
            using (ErrorEntitiesContext db = new ErrorEntitiesContext())
            {
                ThisReportErrors = db.Errors.SqlQuery("SELECT * FROM Error WHERE ReportId = @p0", ReportId).ToList<ErrorModel>();
            }
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
            return PageErrors;
        }
        private static void DoSort(int SortType, List<PageError> PageErrors)
        {
            switch (SortType)
            {
                case 1: //Sorted by error id. Default.
                    PageErrors.Sort(PageError.PageErrorID);
                    break;
                case 2: //Sort reports by number of errors on the report (descending)
                    PageErrors.Sort();
                    break;
                case 3: //Sort reports by number of errors on the report (ascending)
                    PageErrors.Sort(PageError.PageErrorAscending);
                    break;
                case 4: //Sort errors by error type
                    foreach (PageError pe in PageErrors)
                    {
                        pe.GetErrorList().Sort(ErrorModel.ErrorModelByErrorType);
                    }
                    break;
                case 5: //Sort errors by broken link name
                    foreach (PageError pe in PageErrors)
                    {
                        pe.GetErrorList().Sort(ErrorModel.ErrorModelByLinkName);
                    }
                    break;
                default: //0 or an invalid SortType was passed. Do no sorting.
                    break;
            }
        }
    }
}
