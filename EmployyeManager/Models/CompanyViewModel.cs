using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployyeManager.Models
{
    public class CompanyViewModel
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int WorkerAmount { get; set; }      
        public int OwnerID { get; set; }
        public List<User> Users { get; set; }
        public List<Role> Roles { get; set; }

        public List<CompanyEmployee> CompanyEmployees { get; set; }
    }
}