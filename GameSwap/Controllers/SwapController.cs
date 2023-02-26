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
    public class SwapController : Controller
    {
        public static Swap swap { get; private set; }
        public static double sellerDistance { get; private set; }
        public static string counterpartyItemTitle { get; private set; }
        public static int itemNumber { get; private set; }
        public static string counterpartyEmail { get; private set; }



        public SwapController()
        {
        }

        public IActionResult Index()
        {
            Database.Init();
            ViewBag.counterpartyItemNumber = itemNumber;
            ViewBag.counterpartyItemTitle = counterpartyItemTitle;
            ViewBag.counterpartyEmail = counterpartyEmail;
            ViewBag.currentUserEmail = HttpContext.Session.GetString("session_email");
            ViewBag.sellerItem = Int16.Parse(HttpContext.Session.GetString("session_itemNumber"));

            ViewBag.items=this.GetItemsByEmail(ViewBag.currentUserEmail, ViewBag.sellerItem);
            ViewBag.sellerDistance = sellerDistance;
            return View();
        }

        public IActionResult StartSwap(string itemNumberInput, double sellerDistanceInput, string itemTitleInput, string counterpartyEmailInput)
        {
            counterpartyEmail = counterpartyEmailInput;
            counterpartyItemTitle = itemTitleInput;
            itemNumber = Int16.Parse(itemNumberInput);
            sellerDistance = sellerDistanceInput;
            return Json(new { errMsg = "" });
        }

        public IActionResult ProposeSwap(string proposerEmailInput, string counterpartyEmailInput, string proposerItemInput, string counterpartyItemInput)
        {
            SwapDao swapDao = new SwapDao();
            _ = swapDao.ProposeSwap(proposerEmailInput, counterpartyEmailInput, Int16.Parse(proposerItemInput), Int16.Parse(counterpartyItemInput), out string error);
            return Json(new { errMsg = error });
        }

        

        public List<Item> GetItemsByEmail(string userEmail, int sellerItem)
        {
            ItemDao itemDao = new ItemDao();
            List<Item> items =  itemDao.GetItemsByEmail(userEmail, sellerItem, out string error);
            return items;
        }
    }
}
