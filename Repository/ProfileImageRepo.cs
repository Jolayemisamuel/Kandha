using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;
using NibsMVC.EDMX;
namespace NibsMVC.Repository
{
    public class ProfileImageRepo
    {
        NIBSEntities entities = new NIBSEntities();
        public string getProfileImage()
        {
            int id = WebSecurity.GetUserId(WebSecurity.CurrentUserName);
            var image = (from p in entities.tblOutlets where p.OutletId == id select p.ProfileImage).SingleOrDefault();
            return image;
        }
    }
}