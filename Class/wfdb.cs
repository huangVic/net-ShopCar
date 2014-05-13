using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopCar.Class
{
    public class wfdb
    {


        public string tos(object data)
        {
            if (Convert.IsDBNull(data))
            {
                return "";
            }
            else
            {
                return data.ToString().Trim();
            }
        
        }


        public int toi(object data)
        {
            if (Convert.IsDBNull(data))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(data);
            }

        }


    }
}