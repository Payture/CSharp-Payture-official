using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CSharpPayture
{
    public class Transaction : RequestClient
    {
        protected Dictionary<PaytureParams, dynamic> _requestKeyValuePair = new Dictionary<PaytureParams, dynamic>();
        protected readonly PaytureAPIType _apiType = PaytureAPIType.api;
        protected SessionType _sessionType = SessionType.None;
        protected readonly Merchant _merchant;
        protected bool _expanded = false;
        public PaytureCommands Command { get; set; }
        public SessionType SessionType { get { return _sessionType; } }
        public Dictionary<PaytureParams, dynamic> RequestParams { get { return _requestKeyValuePair; } }

        public Transaction( PaytureAPIType api, PaytureCommands command, Merchant merchnat )
        {
            _apiType = api;
            Command = command;
            _merchant = merchnat;
        }

        /// <summary>
        /// Expand transaction for API Methods: Charge/UnBlock/Refund/GetState  
        /// for Ewallet and InPay Methods: Charge/UnBlock/Refund/PayStatus  
        /// </summary>
        /// <param name="orderId">Payment's identifier in Merchant system</param>
        /// <param name="amount">Payment's amount in kopec. Pass null for PayStatus and GetState commands</param>
        /// <returns></returns>
        public Transaction ExpandTransaction( string orderId, Int64? amount )
        {
            if ( _expanded )
                return this;

            if ( String.IsNullOrEmpty( orderId ) )
                return this;
            if (  _apiType == PaytureAPIType.vwapi )
            {
                if ( Command == PaytureCommands.PayStatus )
                    _requestKeyValuePair.Add( PaytureParams.DATA, $"{PaytureParams.OrderId}={orderId};" );
                else if ( Command == PaytureCommands.Refund && amount.HasValue )
                    _requestKeyValuePair.Add( PaytureParams.DATA, $"{PaytureParams.OrderId}={orderId};{PaytureParams.Amount}={amount.Value};{PaytureParams.Password}={_merchant.Password};" );
                else
                    _requestKeyValuePair.Add( PaytureParams.OrderId, orderId );
                if ( amount.HasValue )
                    _requestKeyValuePair.Add( PaytureParams.Amount, amount.Value );
            }
            else
            {
                _requestKeyValuePair.Add( PaytureParams.OrderId, orderId );
                if ( amount.HasValue )
                    _requestKeyValuePair.Add( PaytureParams.Amount, amount.Value );
            }
            if ( Command == PaytureCommands.Refund || ( _apiType != PaytureAPIType.api && ( Command == PaytureCommands.Charge || Command == PaytureCommands.Unblock ) ) )
                ExpandTransaction( true, true );
            else
                ExpandTransaction();

            _expanded = true;
            return this;
        }

        /// <summary>
        /// Expand transaction with Merchant key and password
        /// </summary>
        /// <param name="addKey">Pass false if Merchant key IS NOT NEEDED.</param>
        /// <param name="addPass">Pass true if Merchant password IS NEEDED.</param>
        /// <returns>return current expanded transaction</returns>
        protected Transaction ExpandTransaction( bool addKey = true, bool addPass = false )
        {
            if ( addKey )
                _requestKeyValuePair.Add( _apiType == PaytureAPIType.vwapi ? PaytureParams.VWID : PaytureParams.Key, _merchant.MerchantName );
            if ( addPass )
                _requestKeyValuePair.Add( PaytureParams.Password, _merchant.Password );
            return this;
        }

        
        /// <summary>
        /// Form content for request
        /// </summary>
        /// <returns></returns>
        private FormUrlEncodedContent FormContent( )
        {
            return new FormUrlEncodedContent( _requestKeyValuePair.Where( n=>n.Value != null ).Select( n => new KeyValuePair<string, string>( n.Key.ToString(), $"{n.Value}" ) ) );
        } 
        
        /// <summary>
        /// Process request to Payture server synchronously
        /// </summary>
        /// <returns>PaytureResponse - response from the Payture server.</returns>

        public async Task<PaytureResponse> ProcessOperationAsync()
        {
            if ( !_expanded )
                return PaytureResponse.ErrorResponse( this, "Params are not set" );
            if ( Command == PaytureCommands.Init )
                return await PostAsync( GetPath(), FormContent() ).ContinueWith( r => ParseResponseInternal( r, Command, SessionType ) ).ContinueWith( r => FormRedirectURL( r ) );
            return await PostAsync( GetPath(), FormContent() ).ContinueWith( r => ParseResponseInternal( r, Command, SessionType ) );
        }

        /// <summary>
        /// Process request to Payture server synchronously
        /// </summary>
        /// <returns>PaytureResponse - response from the Payture server. In case of exeption will be return PaytureResponse with exeption mesage in ErrCode field.</returns>
        public PaytureResponse ProcessOperation()
        {
            if ( !_expanded )
                return PaytureResponse.ErrorResponse( this, "Params is not setted" );
            if ( Command == PaytureCommands.Init )
            {
                try {
                    var operationResult = PostAsync( GetPath(), FormContent() ).ContinueWith( r => ParseResponseInternal( r, Command, SessionType ) ).ContinueWith( r => FormRedirectURL( r ) );
                    operationResult.Wait();
                    return operationResult.Result;
                }
                catch(Exception ex )
                {
                    return PaytureResponse.ErrorResponse( this, $"Error occurs{Environment.NewLine}Message: [{ex.Message}]{Environment.NewLine}StackTrace: {ex.StackTrace}");
                }
            }
            else
            {
                try
                {
                    var operationResult = PostAsync( GetPath(), FormContent() ).ContinueWith( r => ParseResponseInternal( r, Command, SessionType ) );
                    operationResult.Wait();
                    return operationResult.Result;
                }
                catch(Exception ex)
                {
                    return PaytureResponse.ErrorResponse( this, $"Error occurs{Environment.NewLine}Message: [{ex.Message}]{Environment.NewLine}StackTrace: {ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Form url for request
        /// </summary>
        /// <returns>url string.</returns>
        protected string GetPath()
        {
            return $"{_merchant.HOST}/{_apiType}/{Command}";
        }

        /// <summary>
        /// Helper method for PaytureCommand.Init for form Redirect URL and save it in RedirectURL field for convinience
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private PaytureResponse FormRedirectURL( Task<PaytureResponse> response )
        {
            var result = response.Result;
            var sessionId = result.Attributes[ PaytureParams.SessionId.ToString() ];
            result.RedirectURL = $"{_merchant.HOST}/{_apiType}/{( _sessionType == SessionType.Add ? PaytureCommands.Add : PaytureCommands.Pay )}?SessionId={sessionId}";
            return result;
        } 
    }
}
