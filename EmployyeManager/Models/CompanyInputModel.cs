using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployyeManager.Models
{
    public class CompanyInputModel
    {
        [Required]
        [Display(Name = "Company Name")]
        [StringLength(50)]
        public string CompanyName { get; set; }


        [Required]
        [Display(Name = "Worker Amount")]
        [Range(1, int.MaxValue, ErrorMessage = "The Worker Amount must be a positive integer.")]
        public int WorkerAmount { get; set; }

        [Required]
        [Display(Name = "Subscription Type")]
        public int SubscriptionType { get; set; }
    }
}