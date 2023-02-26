using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Models
{
    public class Swap
    {
        public string ProposerEmail { get; set; }
        public string CounterpartyEmail { get; set; }
        public DateTime ProposalDateTime { get; set; }
        public DateTime? AcceptanceDate { get; set; }
        public string Status { get; set; }
        public int CounterpartyRating { get; set; }
        public int ProposerRating { get; set; }
        public int ProposerItem { get; set; }
        public int CounterpartyItem { get; set; }
    }
}
