//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EmployyeManager.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CompanyEmployee
    {
        public int CompanyEmployee1 { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public int CompanyID { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}