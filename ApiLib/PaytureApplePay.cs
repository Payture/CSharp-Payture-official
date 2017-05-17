using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPayture
{
    public class PaytureApplePay : PaytureAPIBase
    {
        public PaytureApplePay( string host, string merchantKey, string merchantPassword ) : base( host, merchantKey, merchantPassword, PaytureAPIType.api ) { }

        public async Task<PaytureResponse> ApplePayAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            return await AppleAsync( paramsDic, ApplePayMethods.PAY );
        }

        public async Task<PaytureResponse> AppleBlockAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            return await AppleAsync( paramsDic, ApplePayMethods.BLOCK );
        }

        private async Task<PaytureResponse> AppleAsync( IDictionary<PaytureParams, dynamic> paramsDic, ApplePayMethods method )
        {
            if ( !CheckRequiredParams( paramsDic, PaytureParams.PayToken, PaytureParams.OrderId ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.ApplePay );

            if ( !paramsDic.ContainsKey( PaytureParams.Method ) )
                paramsDic.Add( PaytureParams.Method, method );
            else
                paramsDic[ PaytureParams.Method ] = method;

            return await ProcessOperationAsync( paramsDic, null, PaytureCommands.ApplePay );
        }
    }
}
