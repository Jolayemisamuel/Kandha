using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class QuickSideBarModel
    {
        public List<QuickSideBarManagerModel> getmanagerlist { get; set; }
        public List<QuickSideBarOperatorModel> getoperatorlist { get; set; }
    }
    public class QuickSideBarManagerModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string OutletName { get; set; }
    }
    public class QuickSideBarOperatorModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string OutletName { get; set; }
    }
    public class QuickSideChatModel
    {
        public int Id { get; set; }
        public int SenderUserId { get; set; }
        public int RecieverUserId { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
        public int LastMessId { get; set; }
    }
}