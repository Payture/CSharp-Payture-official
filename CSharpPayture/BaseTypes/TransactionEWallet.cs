using System;

namespace CSharpPayture
{
    public class TransactionEWallet : Transaction
    {
        public TransactionEWallet( PaytureCommands command, Merchant merchant ) : base ( PaytureAPIType.vwapi, command, merchant )
        { }

        /// <summary>
        /// Expand transaction for EWallet Methods: Add (on Merchant side)
        /// </summary>
        /// <param name="customer">Customer object in wich you must specify login and password.</param>
        /// <param name="card">Card card with all fields exclude CardId.</param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( Customer customer, Card card )
        {
            if ( customer == null || card == null )
                return this;
            var str = customer.GetPropertiesString() + card.GetPropertiesString();
            return ExpandInternal( PaytureParams.DATA, str );
        }

        /// <summary>
        /// Expand transaction for EWallet Methods: Register/Update/Delete/Check/Getlist 
        /// </summary>
        /// <param name="customer">Customer object in wich you must specify login and password; all remaining fields is optional.</param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( Customer customer )
        {
            if ( _expanded )
                return this;
            var str = "";
            if ( Command == PaytureCommands.Delete )
                str = $"{PaytureParams.VWUserLgn}={customer.VWUserLgn};{PaytureParams.Password}={_merchant.Password}";
            else
                str = customer.GetPropertiesString();
            return ExpandInternal( PaytureParams.DATA, str );
        }

        /// <summary>
        /// Expand transaction for EWallet Methods: Pay (Merchant side for NOT REGISTERED card)
        /// </summary>
        /// <param name="customer">Customer object.</param>
        /// <param name="card">Card object. Specify in it all fields exclude CardId.</param>
        /// <param name="data">Data object. SessionType and IP fields are required. Optional ConfimCode and CustomFields.</param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( Customer customer, Card card, Data data ) 
        {
            if ( customer == null || card == null || data == null )
                return this;
            _sessionType = ( SessionType )Enum.Parse( typeof( SessionType ), data.SessionType );

            card.CardId = "FreePay";
            var str = customer.GetPropertiesString() + card.GetPropertiesString() + data.GetPropertiesString(); 
            return ExpandInternal( PaytureParams.DATA, str );
        }

        /// <summary>
        /// Expand transaction for EWallet Methods: Pay (Merchant side for REGISTERED card) 
        /// </summary>
        /// <param name="customer">Customer object.</param>
        /// <param name="cardId">CardId identifier in Payture system.</param>
        /// <param name="secureCode">CVC2/CVV2.</param>
        /// <param name="data">Data object. SessionType and IP fields are required. Optional  ConfimCode and CustomFields.</param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( Customer customer, string cardId, int secureCode, Data data ) 
        {
            if ( customer == null || String.IsNullOrEmpty(cardId) || data == null )
                return this;
            _sessionType = ( SessionType )Enum.Parse( typeof( SessionType ), data.SessionType );
            var str = customer.GetPropertiesString() + $"{PaytureParams.CardId}={cardId};" + $"{PaytureParams.SecureCode}={secureCode};" +  data.GetPropertiesString()  + data.CustomFields;
            return ExpandInternal( PaytureParams.DATA, str );
        }
        /// <summary>
        /// Expand transaction for EWallet Methods: Init
        /// </summary>
        /// <param name="customer">Customer object.</param>
        /// <param name="cardId">CardId identifier in Payture system.</param>
        /// <param name="data">Data object. SessionType and IP fields are required; Optional TamplateTag and Language.</param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( Customer customer, string cardId, Data data ) 
        {
            if ( customer == null || data == null )
                return this;
            _sessionType = ( SessionType )Enum.Parse( typeof( SessionType ), data.SessionType );
            var str = customer.GetPropertiesString() + ( cardId == null ? "" : $"CardId={cardId};" ) + data.GetPropertiesString() + data.CustomFields;
            return ExpandInternal( PaytureParams.DATA, str );
        }

        /// <summary>
        /// Expand transaction for EWallet Methods: SendCode/Activate/Remove
        /// </summary>
        /// <param name="customer">Customer object in which you must specify login and password fields.</param>
        /// <param name="cardId">Cards identifier in Payture system.</param>
        /// <param name="amount">Payment's identifier in Merchant system. For Remove pass null.</param>
        /// <param name="orderId">Payment's amount in kopec. For Activate and Remove pass null.</param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( Customer customer, string cardId, Int64? amount, string orderId = null )
        {
            if ( customer == null || String.IsNullOrEmpty( cardId ) )
                return this;
            
            var str = customer.GetPropertiesString() + $"{PaytureParams.CardId}={cardId};"
                + ( amount.HasValue && Command == PaytureCommands.Activate ? $"{PaytureParams.Amount}={amount};" : "" ) + ( orderId == null ? "" : $"{PaytureParams.OrderId}={orderId};" );
            return ExpandInternal( PaytureParams.DATA, str );
        }

        /// <summary>
        /// Expand transaction for EWallet  Methods: Pay/Add (on Payture side)
        /// </summary>
        /// <param name="sessionId">Payment's identifier from Init response.</param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( string sessionId )
        {
            if ( String.IsNullOrEmpty( sessionId ) )
                return this;
            _requestKeyValuePair.Add( PaytureParams.SessionId, sessionId );
            _expanded = true;
            return this;
        }

        /// <summary>
        /// Expand transaction for PaySubmit3DS
        /// </summary>
        /// <param name="MD">Unique transaction identifier from ACS response.</param>
        /// <param name="paRes">An encrypted string with the result of 3DS Authentication.</param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( string MD, string paRes )
        {
            _requestKeyValuePair.Add( PaytureParams.MD, MD );
            _requestKeyValuePair.Add( PaytureParams.PaRes, paRes );
            _expanded = true;
            return this;
        }

        private Transaction ExpandInternal( PaytureParams field, string data )
        {
            _requestKeyValuePair.Add( field, data );
            ExpandTransaction();
            _expanded = true;
            return this;
        }
    }
}
