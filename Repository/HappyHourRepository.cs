using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.EDMX;
using NibsMVC.Models;
using System.Globalization;
namespace NibsMVC.Repository
{
    public class HappyHourRepository
    {
        NIBSEntities _entities = new NIBSEntities();
        public int SaveOfferType(string Offertype)
        {
            Nibs_Offer tb = new Nibs_Offer();
            tb.OfferType = Offertype;
            _entities.Nibs_Offer.Add(tb);
            _entities.SaveChanges();
            int Max = _entities.Nibs_Offer.Max(x => x.OfferId);
            return Max;
        }
        public string SaveHappyHoursDates(HappyHoursDatesModel model)
        {
            try
            {
                var cultureSource = new CultureInfo("en-US", false);
                DateTime StartTime = DateTime.Parse(model.TimeStart, cultureSource);
                TimeSpan starttime = StartTime.TimeOfDay;
                DateTime EndTime = DateTime.Parse(model.TimeEnd, cultureSource);
                TimeSpan endtime = EndTime.TimeOfDay;
              int Max=  SaveOfferType(model.OfferType);
              Nibs_HappyHoursDates tb = new Nibs_HappyHoursDates();
              tb.Discount = model.Discount;
                DateTime enddate=(Convert.ToDateTime(model.EndDate)).Date;
                tb.EndDate = enddate;
              tb.FreeItemId = model.FreeItemId;
              tb.HHOfferType = model.HHOfferType;
              tb.OfferId = Max;
              DateTime startdate = (Convert.ToDateTime(model.StartDate)).Date;
              tb.StartDate = startdate;
              tb.TimeEnd = endtime;
              tb.TimeStart = starttime;
              tb.ItemIndex = model.ItemIndex;
              _entities.Nibs_HappyHoursDates.Add(tb);
              _entities.SaveChanges();
              return "record saved successfully..";

            }
            catch
            {
                return "something wrong try again !";
            }
        }
        public string SaveHappyHoursDays(HappyHoursDaysModel model)
        {
            try
            {
                var cultureSource = new CultureInfo("en-US", false);
                DateTime StartTime = DateTime.Parse(model.TimeStart, cultureSource);
                TimeSpan starttime = StartTime.TimeOfDay;
                DateTime EndTime = DateTime.Parse(model.TimeEnd, cultureSource);
                TimeSpan endtime = EndTime.TimeOfDay;
                int Max = SaveOfferType(model.OfferType);
                Nibs_HappyHours_Days tb = new Nibs_HappyHours_Days();
                tb.Discount = model.Discount;
                tb.EndDay = model.EndDay;
                tb.FreeItemId = model.FreeItemId;
                tb.HHOfferType = model.HHOfferType;
                tb.OfferId = Max;
                tb.StartDay = model.StartDay;
                tb.TimeEnd = endtime;
                tb.TimeStart = starttime;
                tb.ItemIndex = model.ItemIndex;
                _entities.Nibs_HappyHours_Days.Add(tb);
                _entities.SaveChanges();
                return "record saved successfully..";

            }
            catch
            {
                return "something wrong try again !";
            }
        }

        public string SaveHappyHoursDay(HappyHoursDayModel model)
        {
            try
            {
                var cultureSource = new CultureInfo("en-US", false);
                DateTime StartTime = DateTime.Parse(model.TimeStart, cultureSource);
                TimeSpan starttime = StartTime.TimeOfDay;
                DateTime EndTime = DateTime.Parse(model.TimeEnd, cultureSource);
                TimeSpan endtime = EndTime.TimeOfDay;
                int Max = SaveOfferType(model.OfferType);
                Nibs_HappyHours_Day tb = new Nibs_HappyHours_Day();
                tb.Discount = model.Discount;
                
                tb.FreeItemId = model.FreeItemId;
                tb.HHOfferType = model.HHOfferType;
                tb.OfferId = Max;
                tb.Day = model.Day;
                tb.TimeEnd = endtime;
                tb.TimeStart = starttime;
                tb.ItemIndex = model.ItemIndex;
                _entities.Nibs_HappyHours_Day.Add(tb);
                _entities.SaveChanges();
                return "record saved successfully..";

            }
            catch
            {
                return "something wrong try again !";
            }
        }
        public string SaveHappyHoursDate(HappyHoursDateModel model)
        {
            try
            {
                var cultureSource = new CultureInfo("en-US", false);
                DateTime StartTime = DateTime.Parse(model.TimeStart, cultureSource);
                TimeSpan starttime = StartTime.TimeOfDay;
                DateTime EndTime = DateTime.Parse(model.TimeEnd, cultureSource);
                TimeSpan endtime = EndTime.TimeOfDay;
                int Max = SaveOfferType(model.OfferType);
                Nibs_HappyHours_Date tb = new Nibs_HappyHours_Date();
                tb.Discount = model.Discount;
                tb.FreeItemId = model.FreeItemId;
                tb.HHOfferType = model.HHOfferType;
                tb.OfferId = Max;
                DateTime Date = (Convert.ToDateTime(model.Date)).Date;
                tb.Date = Date;
                tb.TimeEnd = endtime;
                tb.TimeStart = starttime;
                tb.ItemIndex = model.ItemIndex;
                _entities.Nibs_HappyHours_Date.Add(tb);
                _entities.SaveChanges();
                return "record saved successfully..";

            }
            catch
            {
                return "something wrong try again !";
            }
        }
    }
}