using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPayture
{
    public class PaytureApplePay : PaytureAPIBase
    {
        public PaytureApplePay( string host, string merchantKey, string merchantPassword ) : base( host, merchantKey, merchantPassword, PaytureAPIType.api ) { }
    }
}
