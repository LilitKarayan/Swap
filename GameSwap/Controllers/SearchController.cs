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
    public class SearchController : Controller
    {
        public static string CurrentUserEmail { get; private set; }
        [BindProperty]
        public static string GameType { get; private set; }

        public SearchController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Results(string searchType, string searchArg)
        {
            Database.Init();
            string error;

            ItemDao itemDao = new ItemDao();
            UserDao userDao = new UserDao();
            string email = HttpContext.Session.GetString("session_email");

            List<SearchItem> searchItems = new List<SearchItem>();
            string searchTerm = "";
            bool isValid = true; 

            Debug.WriteLine(searchType, searchArg);

            switch (searchType)
            {
                case "keywordSearch":
                    searchItems = itemDao.GetItemsByKeyword(searchArg, email, out error);
                    searchTerm = "Keyword";
                    break;
                case "myPostalCode":
                    string myPostalCode = userDao.GetUserFullInfoByEmail(email, out error).PostalCode.Code;
                    searchItems = itemDao.GetItemsByPostalCode(myPostalCode, email, out error);
                    searchTerm = "My postal code";
                    break;
                case "withinMiles":
                    searchItems = itemDao.GetItemsWithinMiles(Double.Parse(searchArg), email, out error);
                    searchTerm = $"Within miles";
                    break;
                case "postalCode":
                    isValid = this.IsValidPostalCode(searchArg);
                    searchItems = itemDao.GetItemsByPostalCode(searchArg, email, out error);
                    searchTerm = "Postal code";
                    break;
                default:
                    error = "Search type not supported";
                    break;
            }

            Debug.WriteLine(error);

            return PartialView((searchTerm, searchArg, searchItems, isValid));
        }

        private bool IsValidPostalCode(string postalCode)
        {
            return postalCode.Length == 5 && postalCode.All(char.IsDigit);
        }
    }
}
