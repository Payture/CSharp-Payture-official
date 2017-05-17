using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPayture
{
    public class PaytureInPay : PaytureAPIBase
    {
        public PaytureInPay( string host, string merchantKey, string merchantPassword ) : base( host, merchantKey, merchantPassword, PaytureAPIType.apim ) { }

        public async Task<PaytureResponse> InitAsync( IDictionary<PaytureParams, dynamic> paramDic )
        {
            return await ProcessOperationAsync( null, new[] { new PaytureEncode( PaytureParams.Data, paramDic ) }, PaytureCommands.Init, true, false, SessionType.Pay );

        }

        public async Task<PaytureResponse> PayAsync( string sessionId )
        {
            return await ProcessOperationAsync( new Dictionary<PaytureParams, dynamic> { { PaytureParams.SessionId, sessionId } }, null, PaytureCommands.Pay, false );
        }
        
        public async Task<PaytureResponse> UnblockAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            if ( !CheckRequiredParams( paramsDic, PaytureParams.OrderId, PaytureParams.Amount ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.Unblock );
            return await ProcessOperationAsync( paramsDic, null, PaytureCommands.Unblock, true );
        }
        public async Task<PaytureResponse> ChargeAsync( string orderId )
        {
            if ( String.IsNullOrEmpty( orderId ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.Charge );
            return await ProcessOperationAsync( new Dictionary<PaytureParams, dynamic>{ { PaytureParams.OrderId, orderId } }, null, PaytureCommands.Charge, true );
        }
        public async Task<PaytureResponse> RefundAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            if ( !CheckRequiredParams( paramsDic, PaytureParams.OrderId, PaytureParams.Amount ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.Refund );
            return await ProcessOperationAsync( paramsDic, null, PaytureCommands.Refund, true );
        }
        public async Task<PaytureResponse> PayStatusAsync( string orderId )
        {
            if ( String.IsNullOrEmpty( orderId ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.PayStatus );

            return await ProcessOperationAsync( new Dictionary<PaytureParams, dynamic> { { PaytureParams.OrderId, orderId } }, null, PaytureCommands.PayStatus );
        }
    }
}
