using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class MovementAnalysisReport
    {
        public int id { get; set; }
        public int RawMaterialId { get; set; }
        public string RawMaterialName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}