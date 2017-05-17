using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace CSharpPayture
{
    public class PaytureAPI : PaytureAPIBase
    {
        
        public PaytureAPI( string host, string merhantKey, string merchantPassword ) : base ( host, merhantKey, merchantPassword, PaytureAPIType.api )
        {
        }
        
        public async Task<PaytureResponse> PayAsync( IDictionary<PaytureParams, dynamic> paramsDic, IDictionary<PaytureParams, dynamic> customFields )
        {
            return await PayOrBlock( paramsDic, PaytureCommands.Pay, customFields );
        }
        
        private async Task<PaytureResponse> PayOrBlock( IDictionary<PaytureParams, dynamic> paramsDic, PaytureCommands command, IDictionary<PaytureParams, dynamic> customFields )
        {
            var encodeArray = new List<PaytureEncode>();
            if(customFields != null || customFields.Count() != 0 )
                encodeArray.Add( new PaytureEncode( PaytureParams.CustomFields, customFields ) );
            var payInfoDic = new Dictionary<PaytureParams, dynamic>();
            var dataEncode = new Dictionary<PaytureParams, dynamic>();
            foreach(var keyVal in paramsDic )
            {
                if ( ( new[] { PaytureParams.PAN, PaytureParams.EMonth, PaytureParams.EYear, PaytureParams.SecureCode, PaytureParams.CardHolder, PaytureParams.OrderId, PaytureParams.Amount } ).Contains( keyVal.Key ) )
                    payInfoDic.Add( keyVal.Key, keyVal.Value );
                if ( !( new[] {  PaytureParams.PAN, PaytureParams.EMonth, PaytureParams.EYear, PaytureParams.SecureCode, PaytureParams.CardHolder, } ).Contains( keyVal.Key ) )
                    dataEncode.Add( keyVal.Key, keyVal.Value );
            }
            encodeArray.Add( new PaytureEncode( PaytureParams.PayInfo, payInfoDic ) );
            return await ProcessOperationAsync( dataEncode, encodeArray, command );
        }
        
        public async Task<PaytureResponse> BlockAsync( IDictionary<PaytureParams, dynamic> paramsDic, IDictionary<PaytureParams, dynamic> customFields )
        {
            return await PayOrBlock( paramsDic, PaytureCommands.Block, customFields );
        }
        public async Task<PaytureResponse> UnblockAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            if ( !CheckRequiredParams( paramsDic, PaytureParams.OrderId, PaytureParams.Amount ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.Unblock );

            return await ProcessOperationAsync( paramsDic, null, PaytureCommands.Unblock );
        }
        public async Task<PaytureResponse> ChargeAsync( string orderId )
        {
            if ( String.IsNullOrEmpty( orderId ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.Charge );

            return await ProcessOperationAsync( new Dictionary<PaytureParams, dynamic> { { PaytureParams.OrderId, orderId } }, null, PaytureCommands.Charge );
        }
        public async Task<PaytureResponse> RefundAsync(IDictionary<PaytureParams, dynamic> paramsDic)
        {
            if ( !CheckRequiredParams( paramsDic, PaytureParams.OrderId, PaytureParams.Amount ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.Refund );

            return await ProcessOperationAsync( paramsDic, null, PaytureCommands.Refund, true );
        }
        public async Task<PaytureResponse> GetStateAsync( string orderId )
        {
            if ( String.IsNullOrEmpty( orderId ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.GetState );

            return await ProcessOperationAsync( new Dictionary<PaytureParams, dynamic> { { PaytureParams.OrderId, orderId } }, null,  PaytureCommands.GetState );
        }

        public async Task<PaytureResponse> Pay3DSAsync(IDictionary<PaytureParams, dynamic> paramsDic )
        {
            return await Pay3DSOrBlock3DSAsync( paramsDic, PaytureCommands.Pay3DS );
        }

        public async Task<PaytureResponse> Block3DSAsync(IDictionary<PaytureParams, dynamic> paramsDic )
        {
            return await Pay3DSOrBlock3DSAsync( paramsDic, PaytureCommands.Pay3DS );
        }
        public async Task<PaytureResponse> Pay3DSOrBlock3DSAsync(IDictionary<PaytureParams, dynamic> paramsDic, PaytureCommands command )
        {
            if ( !CheckRequiredParams( paramsDic, PaytureParams.OrderId, PaytureParams.PaRes ) )
                return PaytureResponse.ImmediateMissedParamsResponse( command );
            return await ProcessOperationAsync( paramsDic, null, command );
        }
    }
}
