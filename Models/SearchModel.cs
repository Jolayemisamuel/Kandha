using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class SearchModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Department { get; set; }
        public string Category { get; set; }
        public string Item { get; set; }

        //List<DataRow> list { get; set; }

    }
}