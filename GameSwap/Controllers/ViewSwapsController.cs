using GameSwap.Dao;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;

namespace GameSwap.Controllers
{
    public class ViewSwapsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Swaps()
        {
            SwapDao swapDao = new SwapDao();
            return PartialView(swapDao.GetSwaps());
        }

        public IActionResult RateSwap(string value)
        {
            string[] values = value.Split(" ");
            string errMsg = null;
            
            try
            {
                if (string.IsNullOrEmpty(values[0]))
                {
                    errMsg = "Rating is invalid";
                }
                else
                {
                    int rating = int.Parse(values[0]);
                    string date = $"{values[3]} {values[4]}";
                    SwapDao swapDao = new SwapDao();
                    errMsg = swapDao.RateSwap(values[1], values[2], rating, date);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Could not rate swaps in view swaps controller. {e.Message}");
            }
           
            return Json(new { error = errMsg });
        }
    }
}
