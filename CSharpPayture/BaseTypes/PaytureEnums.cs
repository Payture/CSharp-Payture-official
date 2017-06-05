
namespace CSharpPayture
{
 public enum PaytureCommands
    {
        None,
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
        AndroidPay,
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
        Method,
        TemplateTag,
        Language,
        Product,
        Total,
        Url,
        Unknown
    }

    public enum PaytureAPIType
    {
        api,
        apim,
        vwapi
    }

    public enum DigitalPayMethods
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
}
