using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPayture
{
    public class TransactionAPI : Transaction
    {
        public TransactionAPI( PaytureCommands command, Merchant merchant ) : base ( PaytureAPIType.api, command, merchant )
        { }
        
        /// <summary>
        /// Expand transaction for API Methods: Pay/Block
        /// </summary>
        /// <param name="info"></param>
        /// <param name="customFields"></param>
        /// <param name="customerKey"></param>
        /// <param name="paytureId"></param>
        /// <returns>current expanded transaction</returns>
        public Transaction ExpandTransaction( PayInfo info, IDictionary<string, dynamic> customFields, string customerKey, string paytureId )
        {
            if ( info == null )
                return this;
            _requestKeyValuePair.Add( PaytureParams.PayInfo, info.GetPropertiesString() );
            _requestKeyValuePair.Add( PaytureParams.OrderId, info.OrderId );
            _requestKeyValuePair.Add( PaytureParams.Amount, info.Amount );
            if( customFields != null && customFields.Count() !=0 )
            {
                _requestKeyValuePair.Add( PaytureParams.CustomFields, customFields.Aggregate( "", ( a, c ) => a += $"{c.Key}={c.Value};" ) );
            }
            if ( !String.IsNullOrEmpty( customerKey ) )
                _requestKeyValuePair.Add( PaytureParams.CustomerKey, customerKey );
            if ( !String.IsNullOrEmpty( paytureId ) )
                _requestKeyValuePair.Add( PaytureParams.PaytureId, paytureId );
            ExpandTransaction();
            _expanded = true;
            return this;
        }
    }
}
