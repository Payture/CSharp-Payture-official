using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
