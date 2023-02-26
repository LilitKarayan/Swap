using GameSwap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.DaoInterface
{
    public interface IUser
    {
        public bool RegisterUser(string email, string password, string nickname, string firstName, string lastName, string postalCode, out string error);
        public bool ValidateUser(string emailOrPhoneNumber, out string error);
        public User ValidatePassword(string emailOrPhoneNumber, string password, out string error);
        public User GetUserFullInfoByEmail(string userEmail, out string error);
        public double GetUserAverageRating(string userEmail, out string error);
        public double GetDistance(string proposerEmail, string counterpartyEmail, out string error);
        public int GetNumberOfUnacceptedSwaps(string userEmail, out string error);
        public int GetNumberOfUnratedSwaps(string userEmail, out string error);
        public int GetMaxDaysPassedForPendingSwap(string userEmail, out string error);
        public User GetRegisteredInfoByEmail(string userEmail, out string error);
        public bool UpdateUserInfoByEmail(string email, string password, string nickname, string firstName, string lastName, string postalCode, out string error);
        public string GetNickname(string proposerEmail, string counterpartyEmail, string dateTime, out string error);
        public void GetFirstNameAndEmailOfSwap(string proposerEmail, string counterpartyEmail, string dateTime, out string error, out string firstname, out string email);
    }
}
