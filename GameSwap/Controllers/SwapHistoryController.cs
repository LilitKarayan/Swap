using GameSwap.Dao;
using GameSwap.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GameSwap.Controllers
{
    public class SwapHistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Summary()
        {
            SwapDao swapDao = new SwapDao();
            List<SwapHistorySummary> history = swapDao.GetSwapHistorySummary();
            return PartialView(history);
        }

        public IActionResult History()
        {
            SwapDao swapDao = new SwapDao();
            List<SwapHistory> history = swapDao.GetSwapHistory();
            return PartialView(history);
        }

        public IActionResult RateSwap(string value)
        {
            string[] values = value.Split(" ");
            string errMsg = null;
            int rating = -1;
            
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
            return Json(new { error = errMsg, rate = rating });
        }
    }
}
