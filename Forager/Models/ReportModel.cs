using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Forager.Models
{
    public class ReportEntitiesContext : DbContext
    {
        public ReportEntitiesContext() : base("ForagerDb") { }
        public DbSet<Forager.Models.ReportModel> Reports { get; set; }
    }

    [Table("Report")]
    public class ReportModel
    {
        [Key]
        public int Id { get; set; }
        public string TimeStampStart { get; set;}
        public string TimeStampStop { get; set;}
        public List<ErrorModel> Errors;
    }
}