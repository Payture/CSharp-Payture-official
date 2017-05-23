using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPayture
{
    public class TransactionInPay : Transaction
    {
        public TransactionInPay( PaytureCommands command, Merchant merchant ) : base ( PaytureAPIType.apim, command, merchant )
        { }
        

        /// <summary>
        /// Expand transaction for InPay Methods: Init
        /// </summary>
        /// <param name="data"></param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( Data data )
        {
            if ( data == null )
                return this;
            _sessionType = SessionType.Pay;
            _requestKeyValuePair.Add( PaytureParams.Data, data.GetPropertiesString() );
            ExpandTransaction();
            _expanded = true;
            return this;
        }

        /// <summary>
        /// Expand transaction for InPay  Methods: Pay
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
