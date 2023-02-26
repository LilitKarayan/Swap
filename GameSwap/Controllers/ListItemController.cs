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
    public class ListItemController : Controller
    {
        public static string CurrentUserEmail { get; private set; }
        [BindProperty]
        public static string GameType { get; private set; }

        public ListItemController()
        {
        }

        public IActionResult Index()
        {
            Database.Init();
            
            UserDao userDao = new UserDao();

            string email = HttpContext.Session.GetString("session_email");
            int unacceptedSwaps = userDao.GetNumberOfUnacceptedSwaps(email, out _);
            int unratedSwaps = userDao.GetNumberOfUnratedSwaps(email, out _);

            ViewBag.CanListItem = unacceptedSwaps <= 5 && unratedSwaps <= 2;

            return View();
        }

        public IActionResult ListItem(string type, string nameTitle, string description, string itemCondition, int pieceCount, string platform, string media, string videoGamePlatform)
        {
            string error; 
            string email = HttpContext.Session.GetString("session_email");
            int itemNumber = -1;

            ItemDao itemDao = new ItemDao();

            switch (type)
            {
                case "BoardGame":
                case "CardGame":
                    if (nameTitle != null && itemCondition != null)
                    {
                        itemNumber = itemDao.ListGeneralItem(nameTitle, description, itemCondition, email, type, out error);
                    } 
                    else
                    {
                        error = $"Invalid data. {type} requires title and condition";
                    }
                    break;

                case "JigsawPuzzle":
                    if (nameTitle != null && itemCondition != null && pieceCount > 0)
                    {
                        itemNumber = itemDao.ListJigsaw(nameTitle, description, itemCondition, email, pieceCount, out error);
                    } 
                    else
                    {
                        error = "Invalid data. Jigsaw puzzle requires title, condition, and piece count";
                    }
                    break; 

                case "ComputerGame":
                    if (nameTitle != null && itemCondition != null && platform != null)
                    {
                        itemNumber = itemDao.ListComputerGame(nameTitle, description, itemCondition, email, platform, out error);
                    }
                    else
                    {
                        error = "Invalid data. Computer game requires title, condition, and platform";
                    }
                    break;

                case "VideoGame":
                    if (nameTitle != null && itemCondition != null && media != null && videoGamePlatform != null)
                    {
                        itemNumber = itemDao.ListVideoGame(nameTitle, description, itemCondition, email, media, videoGamePlatform, out error);
                    }
                    else
                    {
                        error = "Invalid data. Jigsaw Puzzle requires title, condition, media, and video game platform";
                    }
                    break;

                default:
                    error = "Error: invalid gametype";
                    break;
            }

            return Json(new { errMsg = error, itemNumber });
        }
    }
}
