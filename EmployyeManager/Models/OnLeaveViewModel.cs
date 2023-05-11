using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployyeManager.Models
{
    public class OnLeaveViewModel
    {
        public int CompanyId { get; set; }
        public List<UserWithLeaveDates> UsersOnLeave { get; set; }
        public List<UserWithLeaveDates> UsersReturnedFromLeave { get; set; }
    }
}