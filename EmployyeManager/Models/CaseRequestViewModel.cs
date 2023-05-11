using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployyeManager.Models
{
    public class CaseRequestViewModel
    {
        public int CaseID { get; set; }

        [Required]
        public int CompanyID { get; set; }

        [Required]
        public int UserID { get; set; }

        public IEnumerable<SelectListItem> CaseTypes { get; set; }

        [Required]
        public int CaseType { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public FullCalendarEvent[] LeaveAllocations { get; set; }
    }
}