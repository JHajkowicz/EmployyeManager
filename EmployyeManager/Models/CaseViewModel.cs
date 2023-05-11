using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployyeManager.Models
{
    public class CaseViewModel
    {
        public int CaseID { get; set; }
        public int CompanyID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CaseType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}