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
    public class MyItemsController : Controller
    {
        public static string CurrentUserEmail { get; private set; }
        [BindProperty]
        public static string GameType { get; private set; }

        public MyItemsController()
        {
        }

        public IActionResult Index()
        {
            Database.Init();

            ItemDao itemDao = new ItemDao();
            string email = HttpContext.Session.GetString("session_email");

            ViewBag.itemCounts = itemDao.GetItemCountsForUserByEmail(email, out _);
            ViewBag.items = itemDao.GetAllUnswappedItemsByEmail(email, out _);

            return View();
        }
    }
}
