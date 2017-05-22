using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPayture
{
    public class TransactionEWallet : Transaction
    {
        public TransactionEWallet( PaytureCommands command, Merchant merchant ) : base ( PaytureAPIType.vwapi, command, merchant )
        { }

        /// <summary>
        /// Expand transaction for EWallet Methods: Add (on Merchant side)
        /// </summary>
        /// <param name="data"></param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( Customer data, Card card )
        {
            if ( data == null || card == null )
                return this;
            _requestKeyValuePair.Add( PaytureParams.DATA, data.GetPropertiesString() + card.GetPropertiesString() );
            ExpandTransaction();
            _expanded = true;
            return this;
        }

        /// <summary>
        /// Expand transaction for EWallet Methods: Register/Update/Delete/Check/Getlist 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( Customer data )
        {
            if ( _expanded )
                return this;
            if ( Command == PaytureCommands.Delete )
                _requestKeyValuePair.Add( PaytureParams.DATA, $"{PaytureParams.VWUserLgn}={data.VWUserLgn};{PaytureParams.Password}={_merchant.Password}" );
            else
                _requestKeyValuePair.Add( PaytureParams.DATA, data.GetPropertiesString() );
            ExpandTransaction();
            _expanded = true;
            return this;
        }

        /// <summary>
        /// Expand transaction for EWallet Methods: Init/Pay (Merchant side reg/noreg card) 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( Customer customer, Card card, DATA data, bool regCard = true ) 
        {
            if ( customer == null || card == null || data == null )
                return this;
            _sessionType = ( SessionType )Enum.Parse( typeof( SessionType ), data.SessionType );
            var newCustom = new Customer( customer.VWUserLgn, customer.VWUserPsw );
            var newData = new DATA {SessionType = data.SessionType, OrderId = data.OrderId, Amount = data.Amount, IP = data.IP, ConfirmCode = data.ConfirmCode };
            var newCard = new Card();
            if ( regCard )
            {
                newCard = new Card { SecureCode = card.SecureCode, CardId = card.CardId };
            }
            else
            {
                card.CardId = "FreePay";
            }
            if(Command == PaytureCommands.Init)
            {
                newCustom.PhoneNumber = customer.PhoneNumber;
                newCustom.Email = customer.Email;
                newData = new DATA { SessionType = data.SessionType, IP = data.IP, TemplateTag = data.TemplateTag, Language = data.Language, Amount = data.Amount };
                if( data.SessionType != SessionType.Add.ToString() )
                {
                    newCard = new Card { CardId = card.CardId };
                    newData.OrderId = data.OrderId;
                }

            }
            var str = newCustom.GetPropertiesString() + card.GetPropertiesString() + newData.GetPropertiesString() + data.CustomFields;
            _requestKeyValuePair.Add( PaytureParams.DATA, str );

            ExpandTransaction();
            _expanded = true;
            return this;
        }

        /// <summary>
        /// Expand transaction for EWallet Methods: SendCode/Activate/Remove
        /// </summary>
        /// <param name="data"></param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( Customer customer, string cardId, Int64? amount, string orderId = null )
        {
            if ( customer == null || String.IsNullOrEmpty( cardId ) )
                return this;

            _requestKeyValuePair.Add( PaytureParams.DATA, customer.GetPropertiesString() + $"{PaytureParams.CardId}={cardId};"  
                + ( amount.HasValue && Command == PaytureCommands.Activate ?  $"{PaytureParams.Amount}={amount};" : "" ) + (orderId == null ? "" : $"{PaytureParams.OrderId}={orderId};"));

            ExpandTransaction();
            _expanded = true;
            return this;
        }

        /// <summary>
        /// Expand transaction for EWallet  Methods: Pay/Add (on Payture side)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="amount"></param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( string sessionId )
        {
            if ( String.IsNullOrEmpty( sessionId ) )
                return this;
            _requestKeyValuePair.Add( PaytureParams.SessionId, sessionId );
            _expanded = true;
            return this;
        }
    }
}
