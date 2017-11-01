using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using NibsMVC.Models;
namespace NibsMVC.IRepository
{
    public interface IAssignOfferInterface
    {
        List<SelectListItem> GetAllOutletList();
        string getOffer(int Id);
        bool AssignOffer(int UserId, int[] OfferCheck);
        List<AssignOfferToOutletModel> getAssignOfferList();
    }
}
