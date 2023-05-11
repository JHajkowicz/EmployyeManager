using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployyeManager.Models
{
    public class UserWithLeaveDates
    {       
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAdress { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}