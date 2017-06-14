
namespace CSharpPayture
{
    public class Card : EncodeString
    {
        public string CardId { get; set; }
        public string CardNumber { get; set; }
        public byte? EMonth { get; set; }
        public byte? EYear { get; set; }
        public string CardHolder { get; set; }
        public int? SecureCode { get; set; }

        /// <summary>
        /// Object that contains card's information for PaytureEWallet
        /// </summary>
        /// <param name="cardNum">Card number.</param>
        /// <param name="eMonth">The expiry month of card. Takes value from 1 to 12 inclusivelly.</param>
        /// <param name="eYear">The expiry year of card. Two last digit of expired month.</param>
        /// <param name="cardHolder">Card's holder name.</param>
        /// <param name="secureCode">CVC2/CVV2. </param>
        /// <param name="cardId">Card's identifier in Payture system</param>
        public Card( string cardNum, byte eMonth, byte eYear, string cardHolder, int secureCode, string cardId = null )
        {
            CardNumber = cardNum;
            EMonth = eMonth;
            EYear = eYear;
            CardHolder = cardHolder;
            SecureCode = secureCode;
            CardId = cardId;
        }

        public Card() { }
    }
}
