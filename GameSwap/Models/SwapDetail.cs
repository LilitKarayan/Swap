using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Models
{
    public class SwapDetail
    {
        public DateTime Proposed { get; set; }
        public DateTime? AcceptedRejected { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public int? RatingLeft { get; set; }
    }
}
