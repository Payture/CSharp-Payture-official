
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

        public TransactionAPI Api(PaytureCommands command)
        {
            return new TransactionAPI( command, this );
        }

        public TransactionInPay InPay( PaytureCommands command )
        {
            return new TransactionInPay( command, this );
        }

        public TransactionEWallet EWallet( PaytureCommands command )
        {
            return new TransactionEWallet( command, this );
        }

        public TransactionDigitalWallet Apple( PaytureCommands command )
        {
            return new TransactionDigitalWallet( command, this, PaytureCommands.ApplePay );
        }
    
        public TransactionDigitalWallet Android( PaytureCommands command )
        {
            return new TransactionDigitalWallet( command, this, PaytureCommands.AndroidPay );
        }

    }
}
