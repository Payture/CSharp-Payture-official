using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPayture
{
    public class DataBase : EncodeString
    {
        public string SessionType { get; set; }
        public string IP { get; set; }
        public string TemplateTag { get; set; }
        public string Language { get; set; }
        public string OrderId { get; set; }
        public Int64? Amount { get; set; }
        public DataBase(SessionType sessionType, string orderId, Int64 amount, string ip, string template = null, string lang = null )
        {
            SessionType = sessionType == CSharpPayture.SessionType.None ? null : sessionType.ToString();
            OrderId = orderId;
            Amount = amount;
            IP = ip;
            TemplateTag = template;
            Language = lang;
        }
        public DataBase() { }
    }
    public class Data : DataBase
    {
        public string Url { get; set; }
        public Data( SessionType sessionType, string orderId, Int64 amount, string ip, string url = null, string template = null, string lang = null ) : base( sessionType, orderId, amount, ip, template, lang )
        {
            Url = url;
        }
    }

    public class DATA : DataBase
    {
        public string ConfirmCode { get; set; }
        public string CustomFields { get; set; }
        public DATA( SessionType sessionType, string orderId, Int64 amount, string ip, string confirmCode = null,  string[] customFields = null, string template = null, string lang = null ) : base( sessionType, orderId, amount, ip, template, lang )
        {
            ConfirmCode = confirmCode;
            var i = 1;
            CustomFields = customFields == null ? null : customFields.Aggregate( "", ( a, c ) => a += $"CustomField{i++}={c};" );
        }

        public DATA() { }
    }
}
