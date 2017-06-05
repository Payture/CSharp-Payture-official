using System;

namespace CSharpPayture
{
    public class TransactionDigitalWallet : Transaction
    {
        protected PaytureCommands _specialCommand;
    
        public TransactionDigitalWallet( PaytureCommands command, Merchant merchant, PaytureCommands specialCommand ) : base ( PaytureAPIType.api, command, merchant )
        {
            _specialCommand = specialCommand;
        }

        /// <summary>
        /// Expand transaction for ApplePay and AndroidPay Methods: Pay/Block
        /// </summary>
        /// <param name="payToken">PaymentData from PayToken for current transaction.</param>
        /// <param name="orderId">Current transaction OrderId.</param>
        /// <param name="amount">Current transaction amount in kopec - pass null for Apple Pay.</param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( string payToken, string orderId, Int64? amount )
        {
            _requestKeyValuePair.Add( PaytureParams.OrderId, orderId );
            _requestKeyValuePair.Add( PaytureParams.PayToken, payToken );
            _requestKeyValuePair.Add( PaytureParams.Method, ( Command == PaytureCommands.Pay ) ? DigitalPayMethods.PAY.ToString() : DigitalPayMethods.BLOCK.ToString() );
            ExpandTransaction();
            Command =  _specialCommand;

            if( _specialCommand == PaytureCommands.AndroidPay && amount.HasValue )
                _requestKeyValuePair.Add(PaytureParams.Amount, amount.Value.ToString());

            _expanded = true;
            return this;
        }
    }
}
