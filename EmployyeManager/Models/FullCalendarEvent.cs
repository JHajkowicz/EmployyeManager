using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployyeManager.Models
{
    public class FullCalendarEvent
    {
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public bool AllDay { get; set; }
        public string ClassName { get; set; } 
    }
}