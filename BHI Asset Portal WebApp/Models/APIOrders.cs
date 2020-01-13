using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BHI_Asset_Portal_WebApp.Models
{
    public class APIOrders
    {
        [Required]
        public string orderName { get; set; }
        [Required]
        public string workOrderNumber { get; set; }
        [Required]
        public string customer { get; set; }
        [Required]
        public DateTime contractStartDate { get; set; }
        public DateTime contractEndDate { get; set; }
        [Required]
        public List<APIProducts> products { get; set; }



    }
    public class APIProducts
    {
        public string productName { get; set; }
        public DateTime applicableStartDate { get; set; }
        public DateTime applicableEndDate { get; set; }
        public string campaignName { get; set; }
        public string advertisingLocation { get; set; }
        public string advertisingSubLocation { get; set; }
        public string advertisingCommunity { get; set; }
    }
}