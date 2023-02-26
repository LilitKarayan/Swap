using GameSwap.Dao;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Controllers
{
    public class SwapUtil
    {
        public string RateSwap(string value)
        {
            string[] values = value.Split(" ");
            string errMsg = null;
            int rating = -1;

            try
            {
                if (string.IsNullOrEmpty(values[0]))
                {
                    errMsg = "Rating is invalid";
                }
                else
                {
                    rating = int.Parse(values[0]);
                    string date = $"{values[3]} {values[4]}";
                    SwapDao swapDao = new SwapDao();
                    errMsg = swapDao.RateSwap(values[1], values[2], rating, date);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Could not rate swaps in view swaps controller. {e.Message}");
            }

            return errMsg;
        }
    }
}
