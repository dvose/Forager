using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Forager.Models
{
    public class ErrorEntitiesContext : DbContext
    {
        public ErrorEntitiesContext() : base("ForagerDb") { }
        public DbSet<Forager.Models.ErrorModel> Errors { get; set; }
    }

    [Table("Error")]
    public class ErrorModel
    {
        [Key]
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string WebPage { get; set; }
        public string ErrorStatus { get; set; }
        public string Link { get; set; }
        public int Depth { get; set; }
        public string ErrorTimeStamp { get; set; }

        public static Comparison<ErrorModel> ErrorModelByErrorType = delegate(ErrorModel e1, ErrorModel e2)
        {
            return String.Compare(e1.ErrorStatus, e2.ErrorStatus, true);
        };
        public static Comparison<ErrorModel> ErrorModelByLinkName = delegate(ErrorModel e1, ErrorModel e2)
        {
            return String.Compare(e1.Link, e2.Link, true);
        };
    }

    //Represents a list of all errors on a given page.
    public class PageError : IComparable<PageError>
    {
        public static List<PageError> CurrentReportPEs = new List<PageError>();

        public int ReportId { get; set; }
        public string Page { get; set; }
        public bool Unfold { get; set; } //Are the individual pages of this error visible (in the view)?

        private List<ErrorModel> Errors { get; set; }        
        private int MinId = Int32.MaxValue; //The lowest ID of the errors in Errors. Used for sorting.

        public PageError()
        {
            Errors = new List<ErrorModel>();
        }
        public List<ErrorModel> GetErrorList()
        {
            return Errors;
        }
        //Add an error to the list and update MinId if necessary
        public void AddError(ErrorModel em)
        {
            if(em.Id < MinId)
            {
                MinId = em.Id;
            }
            Errors.Add(em);
        }
        public int GetMinId()
        {
            return MinId;
        }
        //Sort by number of errors on the page in ascending order.
        public int CompareTo(PageError other)
        {
            if (other == null || other.Errors == null || Errors == null) return 0;
            return other.Errors.Count - Errors.Count;
        }
        //Sort by number of errors on the page in descending order.
        public static Comparison<PageError> PageErrorAscending = delegate(PageError pe1, PageError pe2)
        {
            return -pe1.CompareTo(pe2);
        };
        //Sort by Error ID. This puts webpages that were searched first at the beginning of the list. This is also the default setting.
        public static Comparison<PageError> PageErrorID = delegate(PageError pe1, PageError pe2)
        {
            /*
            int minErrorId1 = Int32.MaxValue;
            int minErrorId2 = Int32.MaxValue;
            foreach(ErrorModel em in pe1.Errors)
            {
                if(em.Id < minErrorId1)
                {
                    minErrorId1 = em.Id;
                }
            }
            foreach (ErrorModel em in pe2.Errors)
            {
                if(em.Id < minErrorId2)
                {
                    minErrorId2 = em.Id;
                }
            }
            return minErrorId1 - minErrorId2;
             * */
            return pe1.GetMinId() - pe2.GetMinId();
        };
    }
}