using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPayture
{
    public class PaytureResponse
    {
        public PaytureCommands APIName { get; set; }
        public bool Success { get; set; }
        public string ErrCode { get; set; }
        public string RedirectURL { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public dynamic InternalElements { get; set; }


        public static PaytureResponse ErrorResponse(Transaction transaction, string message)
        {
            return new PaytureResponse
            {
                APIName = transaction.Command,
                Success = false,
                ErrCode = message
            };
        }

    }
}
