using System;
namespace GameSwap.DaoInterface
{
    public interface IPhone
    {
        public bool AddPhone(string phoneNumber, string type, bool ifShare, string email, out string error);
        public void GetPhoneNumberAndType(string proposerEmail, string counterpartyEmail, string dateTime, out string phoneNumber, out string type, out string error);
        public bool ValidatePhone(string phoneNumber, string email, out string error);
        public bool UpdatePhone(string phoneNumber, string type, bool ifShare, string email, out string error);
    }
}