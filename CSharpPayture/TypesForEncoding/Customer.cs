namespace CSharpPayture
{
    public class Customer : EncodeString
    {
        public string VWUserLgn { get; set; }
        public string VWUserPsw { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Customer( string login, string password, string phone = null, string email = null )
        {
            VWUserLgn = login;
            VWUserPsw = password;
            PhoneNumber = phone;
            Email = email;
        }
    }
}
