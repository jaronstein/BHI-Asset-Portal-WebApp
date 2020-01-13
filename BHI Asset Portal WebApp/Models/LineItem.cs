using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BHI_Asset_Portal_WebApp.Models
{
    public class LineItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LineItemID { get; set; }
        public string Product { get; set; }
        [Display(Name ="Location/Community")]
        public string LocationSublocation { get; set; }
        [Display(Name ="Start Date")]
        public DateTime? StartDate { get; set; }
        [Display (Name ="End Date")]
        public DateTime? EndDate { get; set; }
        [Display (Name = "Total Impressions")]
        public int? TotalImpressions { get; set; }
        [Display (Name = "Last Updated Date")]
        public DateTime? LastUpdatedDate { get; set; }
        public Order Order { get; set; }
        public string Brand { get; set; }

        [Display(Name = "Line Item Taxonomy"), MaxLength(250)]
        public string Taxonomy { get; set; }

       
        public virtual ICollection<CreativeSet> Creative { get; set; }






    }
}