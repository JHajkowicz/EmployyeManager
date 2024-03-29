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
    
    public partial class CaseRequest
    {
        public int CaseID { get; set; }
        public int CompanyID { get; set; }
        public int UserID { get; set; }
        public int CaseType { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual LeaveCase LeaveCase { get; set; }
        public virtual User User { get; set; }
    }
}
