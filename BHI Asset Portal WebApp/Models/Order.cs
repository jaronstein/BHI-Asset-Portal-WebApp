using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BHI_Asset_Portal_WebApp.Models
{
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int ID { get; set; }
        
        [Required]
        [Display(Name = "Work Order Number")]
        public string BHIOrderNumber { get; set; }
        [Display(Name ="Advertiser")]
        public ApplicationUser Advertiser { get; set; }
        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "Order Name")]
        public string OrderName { get; set; }
        [Display(Name = "Sales Person")]
        public ApplicationUser SalesPerson { get; set; }
        [Display(Name = "Account Manager")]
        public ApplicationUser AccountManager { get; set; }
        [Display(Name = "Contract Start Date")]
        public DateTime? ContractStartDate { get; set; }
        [Display(Name ="Contract End Date")]
        public DateTime? ContractEndDate { get; set; }
        [Display(Name = "Last Submitted Date")]
        public DateTime? LastSubmittedDate { get; set; }
        [Display(Name = "Last Saved Date")]
        public DateTime? LastSavedDate { get; set; }
        [MaxLength(3000)]
        public string Comments { get; set; }
        public bool? Complete { get; set; }
        [Display(Name = "Jira References")]
        public string JiraReferences { get; set; }
        
        

        [MaxLength(60)]
        [Display (Name = "Submitter Name")]
        public string AgentName { get; set; }

        [MaxLength(60)]
        [Display(Name = "Sumbitter Email")]
        public string AgentEmail { get; set; }

        [MaxLength(60)]
        [Display(Name = "Agency Name")]        
        public string AgencyName { get; set; }

        [MaxLength(60)]
        [Display(Name = "Digital Sales Consultant")]
        public string SalesRepName { get; set; }

        [MaxLength(60)]
        [Display(Name = "Advertising Account Manager")]
        public string ADVAccountManagerName { get; set; }





        public virtual ICollection<LineItem> LineItems { get; set; }
    }
}