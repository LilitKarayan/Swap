using GameSwap.Dao;
using GameSwap.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Controllers
{
    public class AcceptRejectSwapController : Controller
    {
        
        public static string currentUserEmail { get; private set; }



        public AcceptRejectSwapController()
        {
        }

        public IActionResult Index()
        {
            Database.Init();
            ViewBag.currentUserEmail = currentUserEmail;
            ViewBag.acceptRejectSwaps = this.GetUnacceptedSwaps(currentUserEmail);
            return View();
        }

        public IActionResult UnacceptedSwaps()
        {
            currentUserEmail = HttpContext.Session.GetString("session_email");
            return Json(new { errMsg = "" });
        }

        public List<AcceptRejectSwap> GetUnacceptedSwaps(string userEmail)
        {
            SwapDao swapDao = new SwapDao();
            List<AcceptRejectSwap> acceptRejectSwaps = swapDao.GetUnacceptedSwapsByEmail(userEmail, out string error);
            return acceptRejectSwaps;
        }
        

        public IActionResult acceptOrRejectSwap(string proposalDateTimeInput, string proposerEmailInput, string counterpartyEmailInput, string actionInput)
        {
            SwapDao swapDao = new SwapDao();
            _ = swapDao.acceptOrRejectSwap(proposalDateTimeInput, proposerEmailInput, counterpartyEmailInput, actionInput, out string error);
            return Json(new { errMsg = error });
        }
    }
}
