using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployyeManager.Models
{
    public class LeaveCaseAllocationsViewModel
    {
        public int LeaveCaseAllocationID { get; set; }
        public int CompanyID { get; set; }
        public int UserID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ApprovedBy { get; set; }
    }
}