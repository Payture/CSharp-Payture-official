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
        /// <param name="customer">Customer object in wich you must specify login and password.</param>
        /// <param name="card">Card card with all fields exclude CardId.</param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( Customer customer, Card card )
        {
            if ( customer == null || card == null )
                return this;
            _requestKeyValuePair.Add( PaytureParams.DATA, customer.GetPropertiesString() + card.GetPropertiesString() );
            ExpandTransaction();
            _expanded = true;
            return this;
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
            if ( Command == PaytureCommands.Delete )
                _requestKeyValuePair.Add( PaytureParams.DATA, $"{PaytureParams.VWUserLgn}={customer.VWUserLgn};{PaytureParams.Password}={_merchant.Password}" );
            else
                _requestKeyValuePair.Add( PaytureParams.DATA, customer.GetPropertiesString() );
            ExpandTransaction();
            _expanded = true;
            return this;
        }

        /// <summary>
        /// Expand transaction for EWallet Methods: Init/Pay (Merchant side reg/noreg card) 
        /// </summary>
        /// <param name="customer">Customer object.</param>
        /// <param name="card">Card object. Specify in it:  CardId field for Init command; CardId and SecureCode for Pay on Merchant side(REGISTERED card); All fields exclude CardId for Pay on Merchant side(NO REGISTERED card).</param>
        /// <param name="data">Data object. SessionType and IP fields are required; Optional for Init: TamplateTag and Language; Optional fo Pay: ConfimCode and CustomFields.</param>
        /// <param name="regCard">Pass false in case Pay on Merchant side for NO REGISTERED CARD.</param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( Customer customer, Card card, Data data, bool regCard = true ) 
        {
            if ( customer == null || card == null || data == null )
                return this;
            _sessionType = ( SessionType )Enum.Parse( typeof( SessionType ), data.SessionType );
            var newCustom = new Customer( customer.VWUserLgn, customer.VWUserPsw );
            var newData = new Data {SessionType = data.SessionType, OrderId = data.OrderId, Amount = data.Amount, IP = data.IP, ConfirmCode = data.ConfirmCode };
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
                newData = new Data { SessionType = data.SessionType, IP = data.IP, TemplateTag = data.TemplateTag, Language = data.Language, Amount = data.Amount };
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
        /// <param name="customer">Customer object in which you must specify login and password fields.</param>
        /// <param name="cardId">Cards identifier in Payture system.</param>
        /// <param name="amount">Payment's identifier in Merchant system. For Remove pass null.</param>
        /// <param name="orderId">Payment's amount in kopec. For Activate and Remove pass null.</param>
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
    }
}
