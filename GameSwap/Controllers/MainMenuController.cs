using GameSwap.Dao;
using GameSwap.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Controllers
{
    public class MainMenuController : Controller
    {
        public IActionResult Index()
        {
            Database.Init();


            if (this.UserAverageRating(HttpContext.Session.GetString("session_email")) == -1.0)
            {
                ViewBag.averageRating = "None";
            }
            else
            {
                ViewBag.averageRating = this.UserAverageRating(HttpContext.Session.GetString("session_email"));
            }


            ViewBag.unacceptedSwaps = this.NumberOfUnacceptedSwaps(HttpContext.Session.GetString("session_email"));
            ViewBag.maxDaysPassedForPendingSwap = this.GetMaxDaysPassedForPendingSwap(HttpContext.Session.GetString("session_email"));
            ViewBag.unratedSwaps = this.NumberOfUnratedSwaps(HttpContext.Session.GetString("session_email"));
            ViewBag.currentUserEmail = HttpContext.Session.GetString("session_email");
            return View();
        }


        public double UserAverageRating(string email)
        {
            UserDao userDao = new UserDao();
            double averageRating = userDao.GetUserAverageRating(email, out string error);

            return averageRating;
        }

        public int NumberOfUnacceptedSwaps(string email)
        {
            UserDao userDao = new UserDao();
            int unacceptedSwaps = userDao.GetNumberOfUnacceptedSwaps(email, out string error);

            return unacceptedSwaps;
        }

        public int NumberOfUnratedSwaps(string email)
        {
            UserDao userDao = new UserDao();
            int unratedSwaps = userDao.GetNumberOfUnratedSwaps(email, out string error);

            return unratedSwaps;
        }
        public int GetMaxDaysPassedForPendingSwap(string email)
        {
            UserDao userDao = new UserDao();
            int maxDaysPendingSwap = userDao.GetMaxDaysPassedForPendingSwap(email, out string error);

            return maxDaysPendingSwap;
        }

        public IActionResult GoMainMenu()
        {
            return Json(new { errMsg = "" });
        }
    }
}
