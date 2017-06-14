using System;

namespace CSharpPayture
{
 public class PayInfo : EncodeString
    {
        private string _emonth;
        private string _eyear;
        private string _cvv;

        public string PAN { get; set; }
        public string CardHolder { get; set; }
        public string OrderId { get; set; }
        public Int64 Amount { get; set; }

        public string SecureCode
        {
            get { return _cvv; }
            set
            {
                if ( System.Text.RegularExpressions.Regex.Match( value, "^\\d{3,4}$" ).Success )
                    _cvv = value;
                else
                    throw new ArgumentException( "Invalid value, only digit allowed.", "SecureCode" );
            }
        }
        public string EMonth
        {
            get
            {
                return _emonth;
            }
            set
            {
                if ( System.Text.RegularExpressions.Regex.Match( value, "^\\d{1,2}$" ).Success )
                {
                    var eMonth = byte.Parse( value );
                    if ( eMonth >= 1 || eMonth <= 12 )
                        _emonth = value;
                    else
                        throw new ArgumentException( "Invalid value. It must be from 1 to 12 inclusive.", "EMonth" );
                }
                else
                    throw new ArgumentException( "Invalid value. It must be from 1 to 12 inclusive.", "EMonth" );
            }
        }
        public string EYear
        {
            get
            {
                return _eyear;
            }
            set
            {
                if ( System.Text.RegularExpressions.Regex.Match( value, "^\\d{1,2}$" ).Success )
                {
                    var eMonth = byte.Parse( value );
                    if ( eMonth >= 17 || eMonth <= 99 )
                        _emonth = value;
                    else
                        throw new ArgumentException( "Invalid value. It must be greater or equal then 17 and consist of two digit.", "EYear" );
                }
                if ( System.Text.RegularExpressions.Regex.Match( value, "^\\d{1,4}$" ).Success )
                {
                    var eyear = int.Parse( value );
                    if ( eyear >= 2017 || eyear <= 2099 )
                        _eyear = value;
                    else
                        throw new ArgumentException( "Invalid value. It must be greater or equal then 2017.", "EYear" );
                }
                else
                    throw new ArgumentException( "Invalid value. It must be greater or equal then 17 and consist of two digit.", "EYear" );
            }
        }

        /// <summary>
        /// Object that contains required params for transaction processing
        /// </summary>
        /// <param name="pan">Card's number.</param>
        /// <param name="eMonth">The expiry month of card.</param>
        /// <param name="eYear">The expiry year of card.</param>
        /// <param name="cardHolder">Card's holder name.</param>
        /// <param name="secureCode">CVC2/CVV2.</param>
        /// <param name="ordId">Payment identifier in Merchant service system.</param>
        /// <param name="amount">Amount of payment kopecs.</param>
        public PayInfo( string pan, string eMonth, string eYear, string cardHolder, string secureCode, string ordId, Int64 amount )
        {
            PAN = pan;
            EMonth = eMonth;
            EYear = eYear;
            CardHolder = cardHolder;
            SecureCode = secureCode;
            OrderId = ordId;
            Amount = amount;
        }

        /// <summary>
        /// Object that contains required params for transaction processing
        /// </summary>
        /// <param name="pan">Card's number.</param>
        /// <param name="eMonth">The expiry month of card.</param>
        /// <param name="eYear">The expiry year of card.</param>
        /// <param name="cardHolder">Card's holder name.</param>
        /// <param name="secureCode">CVC2/CVV2.</param>
        /// <param name="ordId">Payment identifier in Merchant service system.</param>
        /// <param name="amount">Amount of payment kopecs.</param>
        public PayInfo( string pan, byte eMonth, byte eYear, string cardHolder, int secureCode, string ordId, Int64 amount ) : this ( pan, eMonth.ToString(), eYear.ToString(), cardHolder, secureCode.ToString(), ordId, amount )
        { }
    }
}
