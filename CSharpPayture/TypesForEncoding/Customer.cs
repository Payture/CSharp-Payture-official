namespace CSharpPayture
{
    public class Customer : EncodeString
    {
        public string VWUserLgn { get; set; }
        public string VWUserPsw { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// Object contains information about customer for PaytureEwallet operations
        /// </summary>
        /// <param name="login">Required. Customer's identifier in Payture system (e-mail is recommended).</param>
        /// <param name="password">Required. Addition param for access to private Customer's information.</param>
        /// <param name="phone">Optional. Customer's phone number.</param>
        /// <param name="email">Optional. Customer's e-mail address.</param>
        public Customer( string login, string password, string phone = null, string email = null )
        {
            VWUserLgn = login;
            VWUserPsw = password;
            PhoneNumber = phone;
            Email = email;
        }
    }
}
