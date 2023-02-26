using System;
using GameSwap.Dao;
using GameSwap.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameSwap.Controllers
{
    public class UpdateUserController : Controller
    {
        public static User currentUser { get; private set; }

        public IActionResult Index()
        {
            Database.Init();
            ViewBag.currentUser = currentUser;
            return View();
        }

        public IActionResult GetCurrentUserDetails()
        {
            UserDao userDao = new UserDao();
            currentUser = userDao.GetRegisteredInfoByEmail(HttpContext.Session.GetString("session_email"), out string error);
            return Json(new { errMsg = error });
        }

        public IActionResult UpdateUserInfo(string email, string password, string nickname, string firstName, string lastName, string postalCode, string phoneNumber, string type, bool ifShare, string city, string state)
        {
            PostalCodeDao postalCodeDao = new PostalCodeDao();
            UserDao userDao = new UserDao();
            PhoneDao phoneDao = new PhoneDao();

            if (postalCodeDao.ValidatePostalCode(postalCode, city, state, out string error))
            {
                if (phoneDao.ValidatePhone(phoneNumber, email, out error))
                {
                    if (userDao.UpdateUserInfoByEmail(email, password, nickname, firstName, lastName, postalCode, out error))
                    {
                        _ = phoneDao.UpdatePhone(phoneNumber, type, ifShare, email, out error);
                    }
                }

            }
            if (error == "")
            {
                HttpContext.Session.SetString("session_email", email);
                HttpContext.Session.SetString("session_firstName", firstName);
                HttpContext.Session.SetString("session_lastName", lastName);
            }
            return Json(new { errMsg = error });
        }
    }
}