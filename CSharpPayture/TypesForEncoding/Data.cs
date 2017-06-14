using System;
using System.Collections.Generic;
using System.Linq;

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
        
        /// <summary>
        /// Object contains payment's params
        /// </summary>
        /// <param name="sessionType">Session type - indicate the kind of operation (Pay/Block/Add).</param>
        /// <param name="orderId">Payment identifier in Merchant service system.</param>
        /// <param name="amount">Amount of payment kopecs.</param>
        /// <param name="ip">Customer's IP address.</param>
        /// <param name="product">The Product's name.</param>
        /// <param name="total">Total amount of current payment.</param>
        /// <param name="url">The address to which Customer will be return after completion of payment.</param>
        /// <param name="template">Used tamplate for payment page.</param>
        /// <param name="lang">Tamplate language.</param>
        public Data( SessionType sessionType, string orderId, long amount, string ip, string product, Int64? total, string url, string template, string lang ) : this( sessionType, orderId, amount, ip )
        {
            TemplateTag = template;
            Language = lang;
            Url = url;
            Product = product;
            Total = total;
        }
        
        /// <summary>
        /// Object contains payment's params
        /// </summary>
        /// <param name="sessionType">Session type - indicate the kind of operation (Pay/Block/Add).</param>
        /// <param name="orderId">Payment identifier in Merchant service system.</param>
        /// <param name="amount">Amount of payment kopecs.</param>
        /// <param name="ip">Customer's IP address.</param>
        /// <param name="product">The Product's name.</param>
        /// <param name="total">Total amount of current payment.</param>
        /// <param name="confirmCode">From SMS. Required in case of confirm request for current OrderId, pass null otherwise.</param>
        /// <param name="customFields">Addition fields for procession transaction. Internal will be concatenate into simple string, like this: key1=value1;key2=value2;</param>
        /// <param name="template">Used tamplate for payment page.</param>
        /// <param name="lang">Tamplate language.</param>
        public Data( SessionType sessionType, string orderId, long amount, string ip, string product, Int64? total, string confirmCode,  IDictionary<string, string> customFields, string template, string lang ) : this( sessionType, orderId, amount, ip, product, total, null, template, lang )
        {
            ConfirmCode = confirmCode;
            var resultStr = "";
            foreach(var custPair in customFields )
                resultStr += $"{custPair.Key}={custPair.Value};";

            CustomFields = ( customFields == null ? null : resultStr );
        }
        

        /// <summary>
        /// Object contains payment's params
        /// </summary>
        /// <param name="sessionType">Session type - indicate the kind of operation (Pay/Block/Add).</param>
        /// <param name="orderId">Payment identifier in Merchant service system.</param>
        /// <param name="amount">Amount of payment kopecs.</param>
        /// <param name="ip">Customer's IP adress.</param>
        public Data( SessionType sessionType, string orderId, long amount, string ip )
        {
            SessionType = ( sessionType == CSharpPayture.SessionType.None )  ? null : sessionType.ToString();
            OrderId = orderId;
            Amount = amount;
            IP = ip;
        }
        public Data(){}
    }
}
