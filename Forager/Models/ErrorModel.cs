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
        public ErrorEntitiesContext() : base("ForagerDb"){}
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
    }

    //Represents a list of all errors on a given page.
    public class PageError
    {
        public int ReportId { get; set; }
        public string Page { get; set; }
        public List<ErrorModel> Errors { get; set; }
    }
}