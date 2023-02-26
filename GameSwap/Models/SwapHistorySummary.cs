using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Models
{
    public class SwapHistorySummary
    {
        public string MyRole { get; set; }
        public int Total { get; set; }
        public int Accepted { get; set; }
        public int Rejected { get; set; }
        public double RejectedPercentage { get; set; }
    }
}
