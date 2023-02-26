using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GameSwap.Dao;
using GameSwap.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PingItWebsite.Controllers
{
    public class RegisterController : Controller
    {
        #region Variables
        public static bool _VisitedCreatedUser = false;
        #endregion


        public IActionResult Index()
        {
            Database.Init();

            return View();
        }



        public IActionResult RegisterUser(string email, string password, string nickname, string firstName, string lastName, string postalCode, string phoneNumber, string type, bool ifShare, string city, string state)
        {
            PostalCodeDao postalCodeDao = new PostalCodeDao();
            UserDao userDao = new UserDao();
            PhoneDao phoneDao = new PhoneDao();

            if (userDao.UserExists(email))
            {
                return Json(new { errMsg = "User account with that email already exists." });
            }

            if (postalCodeDao.ValidatePostalCode(postalCode, city, state, out string error))
            {
                if (phoneDao.ValidatePhone(phoneNumber, email, out error))
                {
                    if (userDao.RegisterUser(email, password, nickname, firstName, lastName, postalCode, out error))
                    {
                        _ = phoneDao.AddPhone(phoneNumber, type, ifShare, email, out error);
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