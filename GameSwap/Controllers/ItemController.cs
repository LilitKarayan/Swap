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
    public class ItemController : Controller
    {
        public static Item item { get; private set; }
        public static User seller { get; private set; }
        public static string currentUserEmail { get; private set; }

        public ItemController()
        {
        }

        public IActionResult Index()
        {
            Database.Init();
            ItemDao itemDao = new ItemDao();
            item = itemDao.GetItemDetails(Int16.Parse(HttpContext.Session.GetString("session_itemNumber")), out string error);
            ViewBag.item = item;
            seller = this.GetItemSeller(item.Email);
            ViewBag.seller = seller;
            ViewBag.currentUserEmail = currentUserEmail;


            if (this.GetSellerDistance(HttpContext.Session.GetString("session_email"), item.Email) == -1.0)
            {
                ViewBag.sellerDistance = "None";
            }
                
            else
            {
                ViewBag.sellerDistance = this.GetSellerDistance(HttpContext.Session.GetString("session_email"), item.Email);
                ViewBag.distanceColor = this.GetDistanceColor(ViewBag.sellerDistance);
            }


            if (this.GetSellerAverageRating(item.Email)== -1.0)
            {
                ViewBag.sellerAverageRating = "None";
            }
            else
            {
                ViewBag.sellerAverageRating = this.GetSellerAverageRating(item.Email);
            }

            if (this.GetNumberOfUnacceptedSwaps(HttpContext.Session.GetString("session_email")) == -1.0)
            {
                ViewBag.numberOfUnacceptedSwaps = "None";
            }
            else
            {
                ViewBag.numberOfUnacceptedSwaps = this.GetNumberOfUnacceptedSwaps(HttpContext.Session.GetString("session_email"));
            }

            if (this.GetNumberOfUnratedSwaps(HttpContext.Session.GetString("session_email")) == -1.0)
            {
                ViewBag.numberOfUnratedSwaps = "None";
            }
            else
            {
                ViewBag.numberOfUnratedSwaps = this.GetNumberOfUnratedSwaps(HttpContext.Session.GetString("session_email"));
            }

            if (this.CheckItemAvailibilityForSwap(item.ItemNumber))
            {
                ViewBag.AvailibilityForSwap = true;
            }
            else
            {
                ViewBag.AvailibilityForSwap = false;
            }

            return View();

        }

        
        public IActionResult GetItemDetails(string itemNumber)
        {
            ItemDao itemDao = new ItemDao();
            item = itemDao.GetItemDetails(Int16.Parse(itemNumber), out string error);
            HttpContext.Session.SetString("session_itemNumber", Convert.ToString(item.ItemNumber));
            if (HttpContext.Session.GetString("session_email").Length > 0)
            {
                currentUserEmail = HttpContext.Session.GetString("session_email");
            }
            return Json(new { errMsg = error });
        }

        private User GetItemSeller(string sellerEmail)
        {
            UserDao userDao = new UserDao();
            seller = userDao.GetUserFullInfoByEmail(sellerEmail, out string error);
            return seller;
        }

        private double GetSellerAverageRating(string sellerEmail)
        {
            UserDao userDao = new UserDao();
            return userDao.GetUserAverageRating(sellerEmail, out string error);
        }

        private double GetSellerDistance(string proposerEmail, string counterpartyEmail)
        {
            UserDao userDao = new UserDao();
            return userDao.GetDistance(proposerEmail, counterpartyEmail, out string error);
        }

        private string GetDistanceColor(double distance)
        {
            string color = "white";
            if (distance > 0.1)
            {
                if(distance <= 25.0)
                {
                    color = "green";
                }
                else if (distance <= 50.0)
                {
                    color = "yellow";
                }
                else if (distance <= 100.0)
                {
                    color = "orange";
                }
                else
                {
                    color = "red";
                }
            }
            return color;
        }

        public int GetNumberOfUnacceptedSwaps(string userEmail)
        {
            UserDao userDao = new UserDao();
            return userDao.GetNumberOfUnacceptedSwaps(userEmail, out string error);
        }

        public int GetNumberOfUnratedSwaps(string userEmail)
        {
            UserDao userDao = new UserDao();
            return userDao.GetNumberOfUnratedSwaps(userEmail, out string error);
        }

        public bool CheckItemAvailibilityForSwap(int itemNumber)
        {
            ItemDao itemDao = new ItemDao();
            return itemDao.CheckItemAvailibilityForSwap(itemNumber, out string error);
        }
    }
}
