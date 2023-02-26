using GameSwap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.DaoInterface
{
    public interface ISwap
    {
        public bool ProposeSwap(string proposerEmail, string counterpartyEmail, int proposerItem, int counterpartyItem, out string error);

        public List<Swap> FindAssociatedSwaps();

        public List<SwapHistorySummary> GetSwapHistorySummary();

        public List<SwapHistory> GetSwapHistory();

        public SwapDetail GetSwapDetails(string proposerEmail, string counterpartyEmail, string dateTime);

        public List<AcceptRejectSwap> GetUnacceptedSwapsByEmail(string userEmail, out string error);

        public bool acceptOrRejectSwap(string proposalDateTime, string proposerEmail, string counterpartyEmail, string action, out string error);
    }
}
