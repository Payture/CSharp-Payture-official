using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPayture
{
    public class Data : EncodeString {
        public string SessionType  { get; set; }
        public string IP  { get; set; }
        public string TemplateTag  { get; set; }
        public string Language  { get; set; }
        public string OrderId  { get; set; }
        public long? Amount  { get; set; }
        public string Url  { get; set; }
        public string Product  { get; set; }
        public Int64?  Total  { get; set; }
        public string ConfirmCode  { get; set; }
        public string CustomFields  { get; set; }
        
        public Data(SessionType sessionType, string orderId, long amount, string ip, string product, Int64? total, string url, string template, string lang )
        {
            SessionType = (sessionType == CSharpPayture.SessionType.None)  ? null : sessionType.ToString();
            OrderId = orderId;
            Amount = amount;
            IP = ip;
            TemplateTag = template;
            Language = lang;
            Url = url;
            Product = product;
            Total = total;
        }
        
        
        public Data( SessionType sessionType, string orderId, long amount, string ip, string product, Int64? total, string confirmCode,  string[] customFields, string template, string lang ) : this( sessionType, orderId, amount, ip, product, total, null, template, lang )
        {
            ConfirmCode = confirmCode;
            var resultStr = "";
            for(int j = 0; j < customFields.Count(); j++)
            {
                resultStr += $"CustomField{j}={customFields[j]};";
            }
            CustomFields = ( customFields == null ? null : resultStr );
        }
        
        public Data(){}
}
}
