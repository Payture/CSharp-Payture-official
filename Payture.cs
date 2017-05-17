using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPayture
{
    public class Payture
    {
        public PaytureAPI Api { get; set; }
        public PaytureInPay InPay { get; set; }
        public PaytureEWallet EWallet { get; set; }
        public PaytureApplePay ApplePay { get; set; }
    }
}
