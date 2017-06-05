
namespace CSharpPayture
{
    public class CardInfo
    {
        public string CardNumber { get; set; }
        public string CardId { get; set; }
        public string CardHolder { get; set; }
        public string ActiveStatus { get; set; }
        public bool? Expired { get; set; }
        public bool? NoCVV { get; set; }
    
        public CardInfo( string cardNumber, string cardId, string cardHolder, string activeStatus, bool? expired, bool? noCVV )
        {
            CardNumber = cardNumber;
            CardId = cardId;
            CardHolder = cardHolder;
            ActiveStatus = activeStatus;
            Expired = expired;
            NoCVV = noCVV;
        }
    }
}
