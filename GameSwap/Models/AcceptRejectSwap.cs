using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Models
{
    public class AcceptRejectSwap
    {
        public DateTime ProposalDateTime { get; set; }
        public string DesiredItemNameTitle { get; set; }
        public int DesiredItemNumber { get; set; }
        public string ProposerNickname { get; set; }
        public double ProposerRating { get; set; }
        public double Distance { get; set; }
        public string ProposedItemNameTitle { get; set; }
        public int ProposedItemNumber { get; set; }
        public string ProposerEmail { get; set; }
        public string CounterpartyEmail { get; set; }
        public string ProposerFirstName { get; set; }
        public string ProposerPhoneNumber { get; set; }
        public string ProposerPhoneNumberType { get; set; }
        public double ProposerPhoneNumberIfShare { get; set; }



    }
}


//< button type = "button" class= "btn btn-success" onclick = "acceptOrRejectSwap('@acceptRejectSwap.ProposalDateTime.ToString("yyyy / MM / dd HH: mm: ss")','@acceptRejectSwap.ProposerEmail','@acceptRejectSwap.CounterpartyEmail','swapped', '@acceptRejectSwap.ProposerFirstName', '@acceptRejectSwap.ProposerPhoneNumber', '@acceptRejectSwap.ProposerPhoneNumberType', '@acceptRejectSwap.ProposerPhoneNumberIfShare')" > Accept </ button >
//< button type = "button" class= "btn btn-danger" onclick = "acceptOrRejectSwap('@acceptRejectSwap.ProposalDateTime.ToString("yyyy / MM / dd HH: mm: ss")','@acceptRejectSwap.ProposerEmail','@acceptRejectSwap.CounterpartyEmail','rejected', '@acceptRejectSwap.ProposerFirstName', '@acceptRejectSwap.ProposerPhoneNumber', '@acceptRejectSwap.ProposerPhoneNumberType', '@acceptRejectSwap.ProposerPhoneNumberIfShare')" > Reject </ button >
//function acceptOrRejectSwap(proposalDateTime, proposerEmail, counterpartyEmail, action, proposerFirstName, proposerPhoneNumber, proposerPhoneNumberType, proposerPhoneNumberIfShare) {


//message = "The swap is accepted! \n Email: " + proposerEmail + " \n Name: " + proposerFirstName + " \n ";
//if (proposerPhoneNumberIfShare > 0.5)
//{
//    message += "Phone: " + proposerPhoneNumber + " (" + proposerPhoneNumberType + ")"l
//                            }
//else
//{
//    message += "No phone number availible";
//}
//alert(message);