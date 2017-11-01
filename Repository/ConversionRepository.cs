using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NibsMVC.Repository
{
    public class ConversionRepository
    {
        public decimal ConvertValues(string Unit, decimal Qty)
        {
            decimal Quantity = 0;
            if (Unit == "Kgs")
            {
                Quantity = Convert.ToDecimal(Qty) * 1000;

            }
            else if (Unit == "Ltr")
            {
                Quantity = Convert.ToDecimal(Qty) * 1000;

            }
            else if (Unit == "Gms")
            {
                Quantity = Convert.ToDecimal(Qty);

            }
            else if (Unit == "ML")
            {
                Quantity = Convert.ToDecimal(Qty);

            }
            else if (Unit == "Piece")
            {
                Quantity = Convert.ToDecimal(Qty);

            }
            else
                Quantity = Convert.ToDecimal(Qty);
            return Quantity;
        }
        public string Type(string Type)
        {
            string val = string.Empty;
            if (Type == "Kgs")
            {
                val = "Gms";
            }
            else if (Type == "Ltr")
            {
                val = "ML";
            }
            else if (Type == "Gms")
            {
                val = "Gms";
            }
            else if (Type == "ML")
            {
                val = "ML";
            }
            else if (Type == "Piece")
            {
                val = "Piece";
            }
            else
                val = Type;

            return val;
        }

        // retrun convert values
        public decimal ReturnConvertValues(string Unit, decimal Qty)
        {
            decimal Quantity = 0;
            if (Unit == "Gms")
            {
                Quantity = Convert.ToDecimal(Qty) / 1000;

            }
            else if (Unit == "ML")
            {
                Quantity = Convert.ToDecimal(Qty) / 1000;

            }
            else if (Unit == "Kgs")
            {
                Quantity = Convert.ToDecimal(Qty);

            }
            else if (Unit == "Ltr")
            {
                Quantity = Convert.ToDecimal(Qty);

            }
            else if (Unit == "Piece")
            {
                Quantity = Convert.ToDecimal(Qty);

            }
            else
                Quantity = Convert.ToDecimal(Qty);

            return Quantity;
        }
        public string ReturnType(string Type)
        {
            string val = string.Empty;
            if (Type == "Gms")
            {
                val = "Kgs";
            }
            else if (Type == "ML")
            {
                val = "Ltr";
            }
            else if (Type == "Kgs")
            {
                val = "Gms";
            }
            else if (Type == "Ltr")
            {
                val = "ML";
            }
            else if (Type == "Piece")
            {
                val = "Piece";
            }
            else
                val = Type;
            return val;
        }

       
    }
}