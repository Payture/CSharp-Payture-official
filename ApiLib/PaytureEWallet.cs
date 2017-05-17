using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPayture
{
    public class PaytureEWallet : PaytureAPIBase
    {
        public PaytureEWallet( string host, string merchantKey, string merchantPassword ) : base( host, merchantKey, merchantPassword, PaytureAPIType.vwapi ) { }


        #region PaymentAPI
        public async Task<PaytureResponse> InitAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            if ( !CheckRequiredParams( paramsDic, PaytureParams.VWUserLgn, PaytureParams.VWUserPsw, PaytureParams.SessionType, PaytureParams.IP ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.Init );

            if ( paramsDic[ PaytureParams.SessionType ] != SessionType.Add && !CheckRequiredParams( paramsDic, PaytureParams.CardId, PaytureParams.OrderId, PaytureParams.Amount, PaytureParams.IP ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.Init );

            return await ProcessOperationAsync( null, EncodeArray( paramsDic ),  PaytureCommands.Init, true, false, paramsDic[ PaytureParams.SessionType ] == SessionType.Add ? SessionType.Add : SessionType.Pay );
        }

        public async Task<PaytureResponse> PayturePayAsync( string sessionId )
        {
            return await PaytureAction( sessionId, PaytureCommands.Pay );
        }
        
        private async Task<PaytureResponse> PaytureAction( string sessionId, PaytureCommands command )
        {
            return await ProcessOperationAsync( new Dictionary<PaytureParams, dynamic> { { PaytureParams.SessionId, sessionId } }, null, command, false );
        }
        public async Task<PaytureResponse> MerchantPayAsync( IDictionary<PaytureParams, dynamic> paramsDic, bool regCard = true )
        {
            if ( !CheckRequiredParams( paramsDic, PaytureParams.VWUserLgn, PaytureParams.VWUserPsw, PaytureParams.CardId, PaytureParams.OrderId, PaytureParams.Amount, PaytureParams.SecureCode, PaytureParams.IP ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.Pay );

            if ( !regCard && !CheckRequiredParams( paramsDic, PaytureParams.CardNumber, PaytureParams.CardHolder, PaytureParams.EMonth, PaytureParams.EYear ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.Pay );

            return await ProcessOperationAsync( null, EncodeArray( paramsDic ),  PaytureCommands.Pay );
        }
        
        public PaytureResponse MerchantPay( IDictionary<PaytureParams, dynamic> paramsDic, bool regCard = true )
        {
            var result = MerchantPayAsync( paramsDic, regCard );
            result.Wait();
            return result.Result;
        }

        public async Task<PaytureResponse> SendCodeAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            if ( !CheckRequiredParams( paramsDic, PaytureParams.VWUserLgn, PaytureParams.VWUserPsw ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.SendCode );
            return await ProcessOperationAsync( null, new[] { new PaytureEncode( PaytureParams.DATA, paramsDic ) },  PaytureCommands.SendCode );
        }
        public async Task<PaytureResponse> UnblockAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
             return await UnblockOrRefundAsync( paramsDic, PaytureCommands.Unblock );
        }

        private async Task<PaytureResponse> UnblockOrRefundAsync(IDictionary<PaytureParams, dynamic> paramsDic, PaytureCommands command)
        {
            if ( !CheckRequiredParams( paramsDic, PaytureParams.OrderId, PaytureParams.Amount ) )
                return PaytureResponse.ImmediateMissedParamsResponse( command );

            return await ProcessOperationAsync( paramsDic, null, command, true );
        }
        public async Task<PaytureResponse> ChargeAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            if ( !CheckRequiredParams( paramsDic, PaytureParams.OrderId ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.Charge );

            return await ProcessOperationAsync( paramsDic, null, PaytureCommands.Charge, true );
        }
        public async Task<PaytureResponse> RefundAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            return await UnblockOrRefundAsync( paramsDic, PaytureCommands.Refund );
        }
        public async Task<PaytureResponse> PayStatusAsync( string orderId )
        {
            if(String.IsNullOrEmpty(orderId))
                return PaytureResponse.ImmediateMissedParamsResponse(PaytureCommands.PayStatus);
            return await ProcessOperationAsync( null, EncodeArray( PaytureParams.OrderId, orderId ), PaytureCommands.PayStatus );
        }

        public async Task<PaytureResponse> PayStatusAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            if( !CheckRequiredParams(paramsDic, PaytureParams.OrderId) )
                return PaytureResponse.ImmediateMissedParamsResponse(PaytureCommands.PayStatus);
            return await ProcessOperationAsync( null, EncodeArray( PaytureParams.OrderId, paramsDic ), PaytureCommands.PayStatus );
        }

        public async Task<PaytureResponse> PaySubmit3DSAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            if ( !CheckRequiredParams( paramsDic, PaytureParams.MD, PaytureParams.PaRes ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.PaySubmit3DS );
            return await ProcessOperationAsync( paramsDic, null, PaytureCommands.PaySubmit3DS, false );
        }

        #endregion PaymentAPI

        #region Customer
        public async Task<PaytureResponse> RegisterCustomerAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            if(!CheckRequiredParams(paramsDic, PaytureParams.VWUserLgn, PaytureParams.VWUserPsw, PaytureParams.CardNumber, PaytureParams.Email,  PaytureParams.PhoneNumber))
                return PaytureResponse.ImmediateMissedParamsResponse(PaytureCommands.Register);
            return await ProcessOperationAsync( null, EncodeArray( PaytureParams.DATA, paramsDic ),  PaytureCommands.Register );
        }

        public async Task<PaytureResponse> UpdateCustomerAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            if(!CheckRequiredParams(paramsDic, PaytureParams.VWUserLgn, PaytureParams.VWUserPsw, PaytureParams.CardNumber, PaytureParams.Email,  PaytureParams.PhoneNumber))
                return PaytureResponse.ImmediateMissedParamsResponse(PaytureCommands.Update);
            return await ProcessOperationAsync( null, EncodeArray( PaytureParams.DATA, paramsDic ), PaytureCommands.Update );
        }
        public async Task<PaytureResponse> DeleteCustomerAsync( string customerLogin )
        {
            if ( String.IsNullOrEmpty( customerLogin ) )
                return PaytureResponse.ImmediateMissedParamsResponse( PaytureCommands.Delete );
            return await ProcessOperationAsync( null, EncodeArray( PaytureParams.VWUserLgn, customerLogin  ), PaytureCommands.Delete, true  );
        }

        public async Task<PaytureResponse> CheckCustomerAsync( string customerLogin, string customerPassword )
        {
            if( String.IsNullOrEmpty(customerLogin) && String.IsNullOrEmpty(customerPassword) )
                return PaytureResponse.ImmediateMissedParamsResponse(PaytureCommands.Check);
            return await ProcessOperationAsync( null, EncodeArray( new Dictionary<PaytureParams, dynamic> { { PaytureParams.VWUserLgn, customerLogin }, { PaytureParams.VWUserPsw, customerPassword } }), PaytureCommands.Check  );
        }

        #endregion Customer

        #region Card
        public async Task<PaytureResponse> PaytureAddCardAsync( string sessionId )
        {
            return await PaytureAction( sessionId, PaytureCommands.Add );
        }

        public async Task<PaytureResponse> MerchantAddCardAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            if( !CheckRequiredParams(paramsDic, PaytureParams.VWUserLgn, PaytureParams.VWUserPsw, PaytureParams.CardNumber, PaytureParams.EMonth, PaytureParams.EYear, PaytureParams.CardHolder, 
                PaytureParams.SecureCode, PaytureParams.PhoneNumber ) )
                return PaytureResponse.ImmediateMissedParamsResponse(PaytureCommands.Add);
            return await ProcessOperationAsync( null, EncodeArray( paramsDic ), PaytureCommands.Add );
        }

        public async Task<PaytureResponse> ActivateCardAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            if(!CheckRequiredParams(paramsDic, PaytureParams.VWUserLgn, PaytureParams.VWUserPsw, PaytureParams.CardId, PaytureParams.Amount))
                return PaytureResponse.ImmediateMissedParamsResponse(PaytureCommands.Activate);
            return await ProcessOperationAsync( null, EncodeArray( paramsDic ), PaytureCommands.Activate  );
        }

        public async Task<PaytureResponse> RemoveCardAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            if ( !CheckRequiredParams( paramsDic, PaytureParams.VWUserLgn, PaytureParams.VWUserPsw, PaytureParams.CardId ) )
                return PaytureResponse.ImmediateMissedParamsResponse(PaytureCommands.Remove);
            return await ProcessOperationAsync( null, EncodeArray( paramsDic ),  PaytureCommands.Remove );
        }

        public async Task<PaytureResponse> GetListAsync( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            if ( !CheckRequiredParams( paramsDic, PaytureParams.VWUserLgn, PaytureParams.VWUserPsw ) )
                return PaytureResponse.ImmediateMissedParamsResponse(PaytureCommands.GetList);
            return await ProcessOperationAsync( null, EncodeArray( paramsDic ) , PaytureCommands.GetList );
        }
        #endregion Card

        private PaytureEncode[] EncodeArray( IDictionary<PaytureParams, dynamic> paramsDic )
        {
            return new[] { new PaytureEncode( PaytureParams.DATA, paramsDic ) };
        }

        private PaytureEncode[] EncodeArray( PaytureParams param, dynamic val )
        {
            return new[] { new PaytureEncode( PaytureParams.DATA, new Dictionary<PaytureParams, dynamic> { { param, val} } ) };
        }
    }
}
