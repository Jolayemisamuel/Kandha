using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.EDMX;
using System.Text;
using NibsMVC.Models;

namespace NibsMVC.Controllers
{
    public class ChatController : Controller
    {
        //
        //// GET: /Chat/
        //NIBSEntities db = new NIBSEntities();
        //public ActionResult Index()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public string Hub(QuickSideChatModel model)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    ChatHub tb = new ChatHub();
        //    tb.Message = model.Message;
        //    tb.ReciverUserId = model.RecieverUserId;
        //    tb.SenderUserId = model.SenderUserId;
        //    tb.Time = model.Time;
        //    db.ChatHubs.Add(tb);
        //    db.SaveChanges();
        //    int MaxId = (from p in db.ChatHubs where p.ReciverUserId == model.RecieverUserId && p.SenderUserId == model.SenderUserId select p.Id).Max();
        //    sb.Append(MaxId);
        //    return sb.ToString();
        //}
        //[HttpGet]
        //public string GetReply(QuickSideChatModel model)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    var data = (from p in db.ChatHubs where p.ReciverUserId == model.RecieverUserId && p.SenderUserId == model.SenderUserId select (int?)p.Id).Max();
        //    if (data>model.LastMessId)
        //    {
        //        var mess = (from p in db.ChatHubs where p.Id == data select new { Message = p.Message,Id=p.Id }).FirstOrDefault();
        //        sb.Append(mess.Message+"^"+mess.Id);
        //    }
        //   // sb.Append(data);
        //    return sb.ToString();
        //}

    }
}
