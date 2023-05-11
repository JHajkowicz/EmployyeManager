using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployyeManager.Models
{
    public class InviteGenereteModel
    {        
        public int UserId { get; set; }     
        public string EmailAddressReciever { get; set; }
        public string InviteLink { get; set; }
    }
}