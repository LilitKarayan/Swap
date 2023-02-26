using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Models
{
    public class Item
    {
        public int ItemNumber { get; set; }
        public string NameTitle { get; set; }
        public string Description { get; set; }
        public string ItemCondition { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public int PieceCount { get; set; }
        public string Platform { get; set; }
        public string Media { get; set; }
        public string VideoGamePlatform { get; set; }
    }
}
