using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Forager.ViewModels
{
    public class ReportCompare
    {
        public static ReportCompare CurrentCompare;
        public ReportShow Report1 { get; set; }
        public ReportShow Report2 { get; set; }
        public int SortType { get; set; }
        public ReportCompare()
        {
            Report1 = new ReportShow();
            Report2 = new ReportShow();
        }
    }

}