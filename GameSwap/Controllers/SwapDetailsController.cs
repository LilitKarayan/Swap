using GameSwap.Dao;
using GameSwap.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Controllers
{
    public class SwapDetailsController : Controller
    {
        private static string proposerEmail;
        private static string counterPartyEmail;
        private static string dateTime;

        public IActionResult Index(string details)
        {
            if (!string.IsNullOrEmpty(details))
            {
                SetDetailFields(details);
            }
            return View();
        }

        private static void SetDetailFields(string details)
        {
            string[] dets = details.Split();
            proposerEmail = dets[0];
            counterPartyEmail = dets[1];
            dateTime = $"{dets[2]} {dets[3]}";
        }

        public IActionResult Details()
        {
            SwapDao swapDao = new SwapDao();
            SwapDetail detail = swapDao.GetSwapDetails(proposerEmail, counterPartyEmail, dateTime);
            return PartialView(detail);
        }

        public IActionResult UserDetail()
        {
            UserDetail detail = new UserDetail();

            UserDao userDao = new UserDao();
            detail.Nickname = userDao.GetNickname(proposerEmail, counterPartyEmail, dateTime, out string error1);
            userDao.GetFirstNameAndEmailOfSwap(proposerEmail, counterPartyEmail, dateTime, out string error2, out string firstname, out string email);

            // That means the status isn't swapped, so don't show
            if (!string.IsNullOrEmpty(firstname))
            {
                ViewBag.Status = "swapped";
            }
            else
            {
                ViewBag.Status = "not swapped";
            }

            detail.Name = firstname;
            detail.Email = email;
            detail.Distance = userDao.GetDistance(proposerEmail, counterPartyEmail, out string error3);

            PhoneDao phoneDao = new PhoneDao();
            phoneDao.GetPhoneNumberAndType(proposerEmail, counterPartyEmail, dateTime, out string phoneNumber, out string type, out string error4);
            detail.PhoneNumber = phoneNumber;
            detail.PhoneType = type;

            return PartialView(detail);
        }

        public IActionResult ProposedItemDetail()
        {
            ItemDao itemDao = new ItemDao();
            Item detail = itemDao.GetItemDetails(proposerEmail, counterPartyEmail, dateTime, "ProposerItem", out string error);
            return PartialView(detail);
        }

        public IActionResult DesiredItemDetail()
        {
            ItemDao itemDao = new ItemDao();
            Item detail = itemDao.GetItemDetails(proposerEmail, counterPartyEmail, dateTime, "CounterpartyItem", out string error);
            return PartialView(detail);
        }

        public IActionResult RateSwap(string value)
        {
            string errMsg = null;
            if (string.IsNullOrEmpty(value))
            {
                errMsg = "Rating is invalid";
            }
            else
            {
                SwapDao swapDao = new SwapDao();
                errMsg = swapDao.RateSwap(proposerEmail, counterPartyEmail, int.Parse(value), dateTime);
            }
            return Json(new { error = errMsg, rate = int.Parse(value) });
        }
    }
}
