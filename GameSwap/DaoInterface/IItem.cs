using GameSwap.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.DaoInterface
{
    public interface IItem
    {
        public Item GetItemDetails(int itemNumber, out string error);
        public bool CheckItemAvailibilityForSwap(int itemNumber, out string error);
        public List<Item> GetItemsByEmail(string userEmail, int sellerItem, out string error);
        public int ListGeneralItem(string nameTitle, string description, string itemCondition, string email, string type, out string error);
        public int ListJigsaw(string nameTitle, string description, string itemCondition, string email, int pieceCount, out string error);
        public int ListComputerGame(string nameTitle, string description, string itemCondition, string email, string platform, out string error);
        public int ListVideoGame(string nameTitle, string description, string itemCondition, string email, string media, string videoGamePlatform, out string error);

    }
}
