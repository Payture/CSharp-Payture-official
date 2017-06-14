
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

        /// <summary>
        /// Object that provides access to Payture API interface
        /// </summary>
        /// <param name="accountName">Merchant Key in Payture system. For taking one contact Payture support</param>
        /// <param name="password">Merchant Password in Payture system.</param>
        /// <param name="host">Url as string, which recieved Merchant request and processes it</param>
        public Merchant( string accountName, string password, string host )
        {
            _key = accountName;
            _password = password;
            _host = host;
        }

        /// <summary>
        /// Method create new payment transaction for PaytureAPI
        /// </summary>
        /// <param name="command">Command represent the corresponding api method on Payture server.</param>
        /// <returns></returns>
        public TransactionAPI Api( PaytureCommands command )
        {
            return new TransactionAPI( command, this );
        }

        /// <summary>
        /// Method create new payment transaction for PaytureInPay
        /// </summary>
        /// <param name="command">Command represent the corresponding api method on Payture server.</param>
        /// <returns></returns>
        public TransactionInPay InPay( PaytureCommands command )
        {
            return new TransactionInPay( command, this );
        }

        /// <summary>
        /// Method create new payment transaction for PaytureEwallet
        /// </summary>
        /// <param name="command">Command represent the corresponding api method on Payture server.</param>
        /// <returns></returns>
        public TransactionEWallet EWallet( PaytureCommands command )
        {
            return new TransactionEWallet( command, this );
        }

        /// <summary>
        /// Method create new payment transaction for Payture ApplePay
        /// </summary>
        /// <param name="command">Command represent the corresponding api method on Payture server.</param>
        /// <returns></returns>
        public TransactionDigitalWallet Apple( PaytureCommands command )
        {
            return new TransactionDigitalWallet( command, this, PaytureCommands.ApplePay );
        }

        /// <summary>
        /// Method create new payment transaction for Payture AndroidPay
        /// </summary>
        /// <param name="command">Command represent the corresponding api method on Payture server.</param>
        /// <returns></returns>
        public TransactionDigitalWallet Android( PaytureCommands command )
        {
            return new TransactionDigitalWallet( command, this, PaytureCommands.AndroidPay );
        }

    }
}
