using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPayture
{
    public class Merchant
    {
        #region private fields
        private readonly string _key;
        private readonly string _password;
        private readonly string _host;
        #endregion private fields

        #region properties
        public string MerchantName { get { return _key; } }
        public string Password { get { return _password; } }
        public string HOST { get { return _host; } }

        #endregion properties
        public Merchant( string accountName, string password, string host )
        {
            _key = accountName;
            _password = password;
            _host = host;
        }

        public APITransaction Api(PaytureCommands command)
        {
            return new APITransaction( command, this );
        }

        public TransactionInPay InPay( PaytureCommands command )
        {
            return new TransactionInPay( command, this );
        }

        public TransactionEWallet EWallet( PaytureCommands command )
        {
            return new TransactionEWallet( command, this );
        }

      /*  public PaytureAPI GETPaytureApplePay( PaytureCommands command )
        {
            return new PaytureAPI( this, PaytureAPIType.api );
        }*/

    }
}
