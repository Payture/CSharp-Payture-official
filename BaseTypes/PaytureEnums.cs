﻿using System;
using System.Collections.Generic;
using System.Linq;
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
}