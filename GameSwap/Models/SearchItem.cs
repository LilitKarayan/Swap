using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Models
{
    public class SearchItem
    {
        public int ItemNumber { get; set; }
        public string NameTitle { get; set; }
        public string Description { get; set; }
        public string ItemCondition { get; set; }
        public double Distance { get; set; }
        public string Type { get; set; }
        public string FoundIn { get; set; }
    }
}
