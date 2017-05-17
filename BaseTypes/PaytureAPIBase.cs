using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;
using System.Web;

namespace CSharpPayture
{
    public abstract class PaytureAPIBase
    {
        public readonly string HOST = "http://sandbox.payture.com";
        public readonly string MerchantKey = "";
        public readonly string MerchantPassword = "";
        public readonly PaytureAPIType ApiType = PaytureAPIType.api;
        private HttpClient _client = new HttpClient();
        public Action<string> OnResponseReceived { get; set; }
        public Action<PaytureResponse> OnParseResponse { get; set; }
        public Action<Exception> OnExceptionOccurs { get; set; }
        public PaytureAPIBase( string host, string merhantKey, string merchantPassword, PaytureAPIType api )
        {
            ApiType = api;
            HOST = host;
            MerchantKey = merhantKey;
            MerchantPassword = merchantPassword;
        }

        protected async Task<string> PostAsync( FormUrlEncodedContent content, string path )
        {
            try
            {
                var response = await _client.PostAsync( HOST + path, content );
                var respStr = await response.Content.ReadAsStringAsync();
                OnResponseReceived( respStr );
                return respStr;
            }
            catch( Exception ex )
            {
                OnExceptionOccurs( ex );
                _client = new HttpClient();
                return null;
            }
        }
        

        protected PaytureResponse ParseXMLResponse( string body )
        {
            XElement xmlBody = XElement.Parse( body );
            var attributes = new Dictionary<string, string>();
            var attrs = xmlBody.Attributes();
            if ( attrs != null )
            { 
                foreach ( var val in attrs.Select( n => new KeyValuePair<string, string>( n.Name.LocalName, n.Value ) ) )
                {
                    attributes.Add( val.Key, val.Value );
                }
            }
            var elems = ParseXMLElement( xmlBody );
            var paytureResponse = new PaytureResponse
            {
                Success = Boolean.Parse( attributes.Where( n => n.Key == "Success" ).FirstOrDefault().Value ),
                ErrCode = attributes.Where( n => n.Key == "ErrCode" ).FirstOrDefault().Value,
                Attributes = attributes,
                InternalElements = elems
            };
            OnParseResponse( paytureResponse );
            return paytureResponse;
        }

        private dynamic ParseXMLElement( XElement element )
        {
            var elems = element.Elements().ToList();
            if ( elems == null || elems.Count() == 0 )
                return null;
            var result =   elems.Select( n =>
            {
                var internElems = ParseXMLElement( n );
                var elemAttrs = new List<dynamic>();
                foreach ( var v in n.Attributes() )
                    elemAttrs.Add( new { Name = v.Name.LocalName, Value = v.Value } );
                return new { Name = n.Name, Attributes = elemAttrs, InternalElemements = internElems };
            }).ToList();

            return result;
        }
        protected string GetPath(PaytureCommands command)
        {
            return $"/{ApiType}/{command}";
        }

        protected PaytureResponse ParseResponseInternal( Task<string> str, PaytureCommands command, SessionType sessionType = SessionType.None )
        {
            var result = str.Result;
            var paytureResponse = ParseXMLResponse( result );
            if(paytureResponse.APIName == PaytureCommands.Init && String.IsNullOrEmpty(paytureResponse.ErrCode) && sessionType != SessionType.None )
                paytureResponse.RedirectURL = $"{HOST}/{ApiType}/{sessionType}?SessionId={paytureResponse.Attributes[ "SessionId" ]}";

            paytureResponse.APIName = command;
            return paytureResponse;
        }

        protected List<KeyValuePair<PaytureParams, dynamic>> FormKeyValForEncoding( IDictionary<PaytureParams, dynamic> paramsData )
        {
            var result = new List<KeyValuePair<PaytureParams, dynamic>>();
            foreach ( var pairs in paramsData )
                result.Add( new KeyValuePair<PaytureParams, dynamic>( pairs.Key, pairs.Value ) );
            return result;
        }

        protected bool CheckRequiredParams(IDictionary<PaytureParams, dynamic> paramsData, params PaytureParams[] keyForCheck )
        {
            foreach ( var key in keyForCheck )
                if ( !paramsData.ContainsKey( key ) )
                    return false;
            return true;
        }

        private FormUrlEncodedContent FormContent( IDictionary<PaytureParams, dynamic> firstLevelEncode, IEnumerable<PaytureEncode> dataParams )
        {
            if(dataParams != null || dataParams.Count() != 0 )
                foreach ( var data in dataParams )
                    firstLevelEncode.Add( data.Name, data.EncodeString );
            return new FormUrlEncodedContent( firstLevelEncode.Select( n => new KeyValuePair<string, string>( n.Key.ToString(), $"{n.Value}" ) ) );
        }
        protected async Task<PaytureResponse> ProcessOperationAsync( IDictionary<PaytureParams, dynamic> dataEncode, IEnumerable<PaytureEncode> encodeArray, 
            PaytureCommands command,bool addMerchantKey = true, bool addMerchantPass = false, SessionType sessionType = SessionType.None )
        {
            if ( addMerchantPass )
                dataEncode.Add( PaytureParams.Password, MerchantPassword );
            if( addMerchantKey )
	            dataEncode.Add( (ApiType == PaytureAPIType.vwapi ? PaytureParams.VWID : PaytureParams.Key), MerchantKey );
            return await PostAsync( FormContent( dataEncode, encodeArray ), GetPath( command ) ).ContinueWith( r => ParseResponseInternal( r, command, sessionType ) );
        }
    }
}
