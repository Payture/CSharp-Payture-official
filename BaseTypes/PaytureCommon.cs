using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPayture
{
    public enum PaytureCommands
    {
        Pay,
        Block,
        Unblock,
        Charge,
        Refund,
        GetState,
        PayStatus,
        Init,
        Add,
        Pay3DS,
        Block3DS,
        Register,
        Delete,
        Check,
        Activate,
        Remove,
        SendCode,
        GetList,
        ApplePay,
        Update,
        PaySubmit3DS,
    }

    public enum PaytureParams
    {
        VWUserLgn,
        VWUserPsw,
        CardId,
        IP,
        DATA,
        Key,
        PayInfo,
        VWID,
        Amount,
        SessionId,
        CardNumber,
        EMonth,
        EYear,
        CardHolder,
        SecureCode,
        PhoneNumber,
        Password,
        Email,
        OrderId,
        SessionType,
        Data,
        PAN,
        CustomerKey,
        PaytureId,
        CustomFields,
        Description,
        PaRes,
        MD,
        PayToken,
        Method
    }

    public enum PaytureAPIType
    {
        api,
        apim,
        vwapi
    }

    public enum ApplePayMethods
    {
        PAY,
        BLOCK
    }

    public enum SessionType
    {
        Add,
        Pay,
        Block,
        None
    }
 public class PaytureResponse
    {
        public PaytureCommands APIName { get; set; }
        public bool Success { get; set; }
        public string ErrCode { get; set; }
        public string RedirectURL { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public dynamic InternalElements { get; set; }

        public static PaytureResponse ImmediateMissedParamsResponse( PaytureCommands command, params PaytureParams[] requiredParams )
        {
            return new PaytureResponse
            {
                APIName = command,
                Success = false,
                ErrCode = $"Please check required params for {command}; Required params list: {String.Join( ",", requiredParams.Select( n => n.ToString() ) )}"
            };
        }

    }

    public class PaytureEncode
    {
        public PaytureParams Name { get; set; }
        public IDictionary<PaytureParams, dynamic> EncodeParams { get; set; }
        public string EncodeString
        {
            get
            {
                if ( EncodeParams == null || EncodeParams.Count() == 0 )
                    return null;
                return EncodeParams.Aggregate( "", ( a, c ) => { return a += $"{c.Key}={c.Value};"; } );
            }
        }
            
        public PaytureEncode( PaytureParams name, IDictionary<PaytureParams, dynamic> encodeParams )
        {
            Name = name;
            EncodeParams = encodeParams;
        }
    }
    
}
