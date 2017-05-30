using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPayture
{
 public class PayInfo : EncodeString
    {
        public string PAN { get; set; }
        public byte EMonth { get; set; }
        public byte EYear { get; set; }
        public string CardHolder { get; set; }
        public int SecureCode { get; set; }
        public string OrderId { get; set; }
        public Int64 Amount { get; set; }
        public PayInfo( string pan, byte eMonth, byte eYear, string cardHolder, int secureCode, string ordId, Int64 amount )
        {
            PAN = pan;
            EMonth = eMonth;
            EYear = eYear;
            CardHolder = cardHolder;
            SecureCode = secureCode;
            OrderId = ordId;
            Amount = amount;
        }
    }
}
