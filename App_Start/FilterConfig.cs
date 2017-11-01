using System.Web;
using System.Web.Mvc;

namespace NibsMVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
        //public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        //{
        //   // filters.Add(new HandleErrorAttribute());
        //    filters.Add(new HandleErrorAttribute(), 2); //by default added
        //    filters.Add(new HandleErrorAttribute
        //    {
        //        ExceptionType = typeof(System.Data.DataException),
        //        View = "DatabaseError"
        //    }, 1);
        //}
        //public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        //{
        //    filters.Add(new HandleErrorAttribute
        //    {
        //        ExceptionType = typeof(System.Data.DataException),
        //        View = "DatabaseError"
        //    });

        //    filters.Add(new HandleErrorAttribute()); //by default added
        //}
    }
}