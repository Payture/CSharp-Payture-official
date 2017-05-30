using CSharpPayture;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCodeTest.cs
{
    partial  class Program
    {
        static Dictionary<PaytureParams, string> allFields = new Dictionary<PaytureParams, string>
        {
            { PaytureParams.VWUserLgn, "testCustomer@test.com" },
            { PaytureParams.VWUserPsw, "pass123" },
            { PaytureParams.CardId, "" },
            { PaytureParams.IP, "127.0.0.1" },
            { PaytureParams.Amount, "100" },
            { PaytureParams.SessionId, "" },
            { PaytureParams.EMonth, "10" },
            { PaytureParams.EYear, "21" },
            { PaytureParams.CardHolder, "Test Customer" },
            { PaytureParams.SecureCode, "123" },
            { PaytureParams.PhoneNumber, "7845693211" },
            { PaytureParams.Email, "testCustomer@test.com" },
            { PaytureParams.OrderId, "" },
            { PaytureParams.SessionType, SessionType.None.ToString() },
            { PaytureParams.PAN, "4111111111111112" },
            { PaytureParams.CustomerKey, "testCustomer" },
            { PaytureParams.PaytureId, "" },
            { PaytureParams.CustomFields, "" },
            { PaytureParams.Description, "" },
            { PaytureParams.PaRes, "" },
            { PaytureParams.MD, "" },
            { PaytureParams.PayToken, "" },
            { PaytureParams.Method, "" },
            { PaytureParams.Language, "RU" },
            { PaytureParams.TemplateTag, "" },
            { PaytureParams.Url, "" },
            { PaytureParams.Total, "1" },
            { PaytureParams.Product, "Something" }
        };


        #region NEW Router
        public static void Router ( )
        {
            Console.WriteLine( "Type the command:" );
            var command = Console.ReadLine().ToUpper().Split(' ');
            if ( command.Count() < 1)
                return;
            var apiType = PaytureAPIType.api;//api type
            var sessionType = "";
            var transactionSide = "";
            var regOrNoRegCard = "";
            var cmd = command[ 0 ];  //main command
            if ( !new[] { "FIELDS", "COMMANDS", "CHANGEFIELDS", "HELP", "CHANGEMERCHANT" }.Contains( cmd ) )
            {
                var api = command[ 1 ];
                if(api == "EWALLET")
                    apiType = PaytureAPIType.vwapi;
                else if(api =="INPAY")
                    apiType = PaytureAPIType.apim;
                return;
            }
            if ( command.Count() > 2 )
            {
                if ( new[] { "I", "INIT","P", "PAY" }.Contains( cmd ) )
                {
                    sessionType = command.ElementAt( 2 ).ToUpper().Substring( 0, 1 ); //session type
                    if ( ( cmd == "PAY" || cmd == "P" ) && command.Count() >= 3 && apiType == PaytureAPIType.vwapi )
                    {
                        transactionSide = command.ElementAt( 3 ).ToUpper().Substring( 0, 1 );
                        if(transactionSide == "M" && command.Count() > 3)
                            regOrNoRegCard = command.ElementAt( 4 ).ToUpper().Substring( 0, 1 );
                    }
                    else if(cmd == "INIT" || cmd == "I")
                        { }
                    else return;
                }
                else if(cmd == "ADD")
                    transactionSide = command.ElementAt( 2 ).ToUpper().Substring( 0, 1 );
            }
            
            switch ( cmd )
            {
                case "PAY":
                    {
                        if ( apiType == PaytureAPIType.api )
                        {
                            APIPayOrBlock( PaytureCommands.Pay );
                            break;
                        }
                        switch( transactionSide )
                        {
                            case "P":
                                {
                                    PayturePayOrAdd( PaytureCommands.Pay );
                                    break;
                                }
                            case "M":
                                {
                                    var customer = GetCustomer();
                                    var data = DataForInit(sessionType == "P" ? SessionType.Pay : SessionType.Block);          
                                    
                                    if(regOrNoRegCard != "R")
                                    {
                                        var card = GetCard();
                                        response = _merchant.EWallet( PaytureCommands.Pay ).ExpandTransaction(customer, card, data, false).ProcessOperation();
                                        break;

                                    }

                                    var cardId = allFields[ PaytureParams.CardId ];
                                    var secureCode = allFields[ PaytureParams.SecureCode ];
                                    Console.WriteLine( $"CardId={cardId}; SecureCode={secureCode};" );
                                    CircleChanges( "CardId and SecureCode" );

                                    response = _merchant.EWallet( PaytureCommands.Pay ).ExpandTransaction(customer, new Card { CardId = allFields[ PaytureParams.CardId ],
                                        SecureCode = int.Parse(allFields[ PaytureParams.SecureCode ]) }, data).ProcessOperation();

                                    break;
                                }
                        }
                        break;
                    }
                case "BLOCK":
                    {
                        APIPayOrBlock( PaytureCommands.Block );
                        break;
                    }
                case "CHARGE":
                    {
                        ChargeUnblockRefundGetState( PaytureCommands.Charge, apiType );
                        break;
                    }
                case "REFUND":
                    {
                        ChargeUnblockRefundGetState(  PaytureCommands.Refund, apiType );
                        break;
                    }
                case "UNBLOCK":
                    {
                        ChargeUnblockRefundGetState(  PaytureCommands.Unblock, apiType );
                        break;
                    }
                case "GETSTATE":
                    {
                        ChargeUnblockRefundGetState(  PaytureCommands.GetState, apiType );
                        break;
                    }
                case "PAYSTATUS":
                    {
                        ChargeUnblockRefundGetState(  PaytureCommands.PayStatus, apiType );
                        break;
                    }
                case "INIT":
                    {
                        var data = DataForInit(sessionType == "P" ? SessionType.Pay : sessionType == "B"? SessionType.Block : SessionType.Add);


                        if ( apiType == PaytureAPIType.vwapi )
                        {
                            var customer = GetCustomer();
                            response = _merchant.EWallet( PaytureCommands.Init ).ExpandTransaction( customer, new Card(), data ).ProcessOperation();
                        }
                        else
                            response = _merchant.InPay( PaytureCommands.Init ).ExpandTransaction( data ).ProcessOperation();
                        var res = response;
                        Task.Run(() => Process.Start($"{res.RedirectURL}"));
                        break;
                    }
                case "ACTIVATE":
                    {
                        CustomerAndCardAPI( PaytureCommands.Activate );
                        break;
                    }
                case "REMOVE":
                    {
                        CustomerAndCardAPI(  PaytureCommands.Remove );
                        break;
                    }
                case "GETLIST":
                    {
                        CustomerAndCardAPI( PaytureCommands.GetList );
                        break;
                    }
                case "REGISTER":
                    {
                        CustomerAndCardAPI( PaytureCommands.Register );
                        break;
                    }
                case "UPDATE":
                    {
                        CustomerAndCardAPI( PaytureCommands.Update );
                        break;
                    }
                case "DELETE":
                    {
                        CustomerAndCardAPI( PaytureCommands.Delete );
                        break;
                    }
                case "CHECK":
                    {
                        CustomerAndCardAPI( PaytureCommands.Check );
                        break;
                    }
                 case "ADD":
                    {
                        switch(transactionSide)
                        {
                            case "P":
                                {
                                    PayturePayOrAdd( PaytureCommands.Add );
                                    break;
                                }
                            case "M":
                                {
                                    var customer = GetCustomer();
                                    var card = GetCard();

                                    response = _merchant.EWallet( PaytureCommands.Add ).ExpandTransaction(customer, card).ProcessOperation();    
                                    break;
                                }
                        }
                        break;
                    }
                case "FIELDS":
                    {
                        var aggrStr = allFields.Aggregate( $"{Environment.NewLine}Current value of fields:{Environment.NewLine}", ( a, c ) => a += $"\t{c.Key} = {c.Value}{Environment.NewLine}" );
                        Console.WriteLine( aggrStr );
                        break;
                    }
                case "CHANGEFIELDS":
                    {
                        CircleChanges();
                        break;
                    }
                case "COMMANDS":
                    {
                        ListCommands();
                        break;
                    }
                case "CHANGEMERCHANT":
                    {
                        ChangeMerchant();
                        break;
                    }
            }
            if ( !new[] { "FIELDS", "CHANGEFIELDS", "COMMANDS", "CHANGEMERCHANT", "HELP" }.Contains( cmd ) )
                WriteResult( response );

        }
        #endregion NEW Router
        static void GenerateOrderId()
        {
            allFields[ PaytureParams.OrderId ] = $"ORD_{Random.Next( 0, int.MaxValue )}_TEST";
        }

        static void GenerateAmount()
        {
            allFields[ PaytureParams.Amount ] =  Random.Next( 50, 100000 ).ToString();
        }

        static Card GetCard()
        {
            var card = CardFromCurrentSettings();
            var propsDataDefault = card.GetType().GetProperties().Aggregate( $"Data params:{Environment.NewLine}", ( a, c ) => a += $"\t{c.Name} = {c.GetValue( card, null )}{Environment.NewLine}" );
            Console.WriteLine( "Default settings for Card:" );
            Console.WriteLine( $@"{propsDataDefault} " );
            CircleChanges();
            return CardFromCurrentSettings();
        }
        static void PayturePayOrAdd( PaytureCommands command)
        {
            var sessionId = allFields[PaytureParams.SessionId];
            Console.WriteLine( $"SessionId: {sessionId}" );
            CircleChanges( "SessionId" );
            response = _merchant.EWallet( command ).ExpandTransaction( allFields[PaytureParams.SessionId], null ).ProcessOperation();
        }

        static Card CardFromCurrentSettings()
        {
            return new Card( allFields[ PaytureParams.PAN ], byte.Parse(allFields[ PaytureParams.EMonth ]),
               byte.Parse(allFields[ PaytureParams.EYear ]), allFields[ PaytureParams.CardHolder ], int.Parse(allFields[ PaytureParams.SecureCode ]) );
        }
        static PayInfo PayInfoFromCurrentSettings()
        {
            return new PayInfo( allFields[ PaytureParams.PAN ],
                byte.Parse( allFields[ PaytureParams.EMonth ] ),
                byte.Parse( allFields[ PaytureParams.EYear ] ),
                allFields[ PaytureParams.CardHolder ],
                int.Parse( allFields[ PaytureParams.SecureCode ] ),
                allFields[ PaytureParams.OrderId ],
                int.Parse( allFields[ PaytureParams.Amount ] ) );
        }

        static Customer CustomerFromCurrentSettings()
        {
            return new Customer( allFields[ PaytureParams.VWUserLgn ], allFields[ PaytureParams.VWUserPsw ], allFields[ PaytureParams.PhoneNumber ], allFields[ PaytureParams.Email ] );
        }
        
        static Data DataFromCurrentSettings()
        {
            return  new Data
            {
                Amount = allFields[ PaytureParams.Amount ] == null ? null : (long?) long.Parse( allFields[ PaytureParams.Amount ] ),
                IP = allFields[ PaytureParams.IP ],
                Language = allFields[ PaytureParams.Language ],
                OrderId = allFields[ PaytureParams.OrderId ],
                SessionType = allFields[ PaytureParams.SessionType ],
                TemplateTag = allFields[ PaytureParams.TemplateTag ]
            };
        }

        static PayInfo GetPayInfo()
        {
            GenerateAmount();
            GenerateOrderId();
            var payInfo = PayInfoFromCurrentSettings();
            var propsPayInfo = payInfo.GetType().GetProperties().Aggregate( $"PayInfo params:{Environment.NewLine}", ( a, c ) => a += $"\t{c.Name} = {c.GetValue( payInfo, null )}{Environment.NewLine}" );
            Console.WriteLine( "Default settings PayInfo:" );
            Console.WriteLine( $@"{propsPayInfo}{Environment.NewLine}" );
            CircleChanges();
            return PayInfoFromCurrentSettings();
        }


        static void CustomerAndCardAPI( PaytureCommands command)
        {
            var customer = GetCustomer();
            if ( command == PaytureCommands.Activate || command == PaytureCommands.Remove )
            {
                var cardId = allFields[ PaytureParams.CardId ];
                Console.WriteLine( $"CardId: {cardId}" );
                CircleChanges( "CardId" );
                response = _merchant.EWallet( command ).ExpandTransaction( customer, allFields[ PaytureParams.CardId ], command == PaytureCommands.Activate ? (long?)101 : null ).ProcessOperation();
            }
            response = _merchant.EWallet( command ).ExpandTransaction( customer ).ProcessOperation();
        }
        static Customer GetCustomer()
        {
            var customer = CustomerFromCurrentSettings();
            var propsDataDefault = customer.GetType().GetProperties().Aggregate( $"Data params:{Environment.NewLine}", ( a, c ) => a += $"\t{c.Name} = {c.GetValue( customer, null )}{Environment.NewLine}" );
            Console.WriteLine( "Default settings for Customer:" );
            Console.WriteLine( $@"{propsDataDefault} " );
            CircleChanges( "Customers fields" );

            return CustomerFromCurrentSettings();
        }
        private static void  ChargeUnblockRefundGetState( PaytureCommands command, PaytureAPIType api )
        {
            Transaction trans;
            var orderId = allFields[PaytureParams.OrderId];
            var amount = allFields[ PaytureParams.Amount ] == null ? null : (long?) long.Parse( allFields[ PaytureParams.Amount ] );
            CircleChanges();

            if ( api == PaytureAPIType.api )
                trans = _merchant.Api( command );
            else if ( api == PaytureAPIType.vwapi )
                trans = _merchant.EWallet( command );
            // else if()
            else
                trans = _merchant.InPay( command );
            response = trans.ExpandTransaction( orderId,  amount ).ProcessOperation();
        }

        private static Data DataForInit(SessionType type)
        {
            GenerateAmount();
            GenerateOrderId();
            allFields[ PaytureParams.SessionType ] = type.ToString();
            var data = DataFromCurrentSettings();
            var propsDataDefault = data.GetType().GetProperties().Aggregate( $"Data params:{Environment.NewLine}", ( a, c ) => a += $"\t{c.Name} = {c.GetValue( data, null )}{Environment.NewLine}" );
            Console.WriteLine( "Default settings for request:" );
            Console.WriteLine( $@"{propsDataDefault} " );
            CircleChanges();
            return DataFromCurrentSettings();
        }

        #region Router

        static void CircleChanges(string message = "default settings")
        {
            Console.WriteLine( $"Please enter <1> if you wanna change {message}:" );
            int val = 0;
            if(Int32.TryParse(Console.ReadLine(), out val))
            {
                if ( val == 1 )
                    while ( true )
                    {
                        Console.WriteLine( "Press Backspace if you completed changes" );
                        if ( Console.ReadKey().Key == ConsoleKey.Backspace )
                            break;
                        
                        ChangeFields();
                    }
            }
        }
        static void ChangeFields()
        {
            Console.WriteLine( "Enter your params in line separated by space, like this: key1=val1 key2=val2" );
            var line = Console.ReadLine();
            if ( String.IsNullOrEmpty( line ) )
                return;
            var splitedLine = line.Split( ' ' ).Select(n=> {
                if ( !n.Contains( "=" ) )
                    return new KeyValuePair<PaytureParams, dynamic>(PaytureParams.Unknown, null);
                var temp = n.Split( '=' );
                PaytureParams paytureParam = PaytureParams.Unknown;
                if ( !Enum.TryParse( temp[ 0 ], true, out paytureParam ) )
                    return new KeyValuePair<PaytureParams, dynamic>(PaytureParams.Unknown, null);
                return new KeyValuePair<PaytureParams, dynamic>( paytureParam, temp[ 1 ] );
            } );

            foreach(var keyVal in splitedLine)
            {
                if(allFields.ContainsKey(keyVal.Key))
                    allFields[ keyVal.Key ] = keyVal.Value;
            }

        }

        public static void ListCommands()
        {
            Console.WriteLine("Commands for help:\n" + 
                    "\tfields\t\t- list current key-value pairs that used in request to Payture server.\n" +
                    "\tchangefields\t\t- command for changing current values of  key-value pairs that used in request to Payture server.\n" + 
                    "\tcommands\t\t- list avaliable commands for this console program.\n" + 
                    "\tchangemerchant\t\t- commands for changing current merchant account settings.\n" +
                    "\thelp\t\t- commands that types this text (description of commands that you can use in this console program.).\n\n");
            Console.WriteLine("Commands for invoke PaytureAPI functions.\n" +
                    "\tpay\t-\n" +
                    "\tblock\t- only for api\n" + 
                    "\tcharge\t-\n" + 
                    "\tunblock\t-\n" +
                    "\trefund\t-\n" + 
                    "\tgetsstate\t- only for api\n" +
                    "\tpaystatus\t- for vwapi and apim\n" + 
                    "\tinit\t-\n" + 
                    "\tregister\t-\n" +
                    "\tcheck\t-\n" + 
                    "\tupdate\t-\n" + 
                    "\tdelete\t-\n" +
                    "\tadd\t-\n" +
                    "\tactivate\t-\n" +   
                    "\tsendcode\t-\n" +
                    "\tremove\t-\n"  );
        }


        static void APIPayOrBlock( PaytureCommands command )
        {
            var payInfo = GetPayInfo();
            var paytureId = allFields[PaytureParams.PaytureId];
            var custKey = allFields[PaytureParams.CustomerKey];
            var custFields = allFields[PaytureParams.CustomFields];
            var propsPayInfo = payInfo.GetType().GetProperties().Aggregate( $"PayInfo params:{Environment.NewLine}", ( a, c ) => a += $"\t{c.Name} = {c.GetValue( payInfo, null )}{Environment.NewLine}" );
            Console.WriteLine( "Additional settings for request:" );
            Console.WriteLine( $@"{propsPayInfo}{Environment.NewLine}PaytureId = {paytureId}{Environment.NewLine}CustomerKey = {custKey}{Environment.NewLine}CustomFields = {custFields}{Environment.NewLine} " );
            CircleChanges();
            response = _merchant.Api( command ).ExpandTransaction( payInfo, null, (string)allFields[PaytureParams.CustomerKey], (string)allFields[PaytureParams.PaytureId] ).ProcessOperation();
        }

        #endregion Router

        static void ChangeMerchant()
        {
            Console.WriteLine( "Type Merchant account name:" );
            _merchantKey = Console.ReadLine();
            Console.WriteLine( "Type Merchant account password" );
            _merchantPassword = Console.ReadLine();
            Console.WriteLine( "Type host name:" );
            _host = Console.ReadLine();
            Console.WriteLine( $"Merchant account settings: {Environment.NewLine}\tMerchantName={_merchantKey}{Environment.NewLine}\tMerchantPassword={_merchantPassword}{Environment.NewLine}\tHOST={_host}{Environment.NewLine}" );
            _merchant = new Merchant( _merchantKey, _merchantPassword, _host );
        }

        static void Help()
        {
            Console.WriteLine("Then console promt you 'Type command' - you can type commands for invoke PaytureAPI functions and you can types commands for help.");
            Console.WriteLine("The structure of commands for invoke PaytureAPI functions:\n\t=>Fist keyword is one of avaliable command for PaytureAPI (like pay, block for example);\n");
            Console.WriteLine("\t=>For second keyword you must state the api type, one of following:\n\t\tapi - for PaytureAPI\n\t\tinpay - for PaytureInPay\n\t\tewallet - for PaytureEWallet\n\t\tapple - for PaytureApplePay\n\t\tandroid - for PaytureAndroidPay\n");
            Console.WriteLine("\t=>Third keyword is needed for specify:\n\t\tSessionType in 'init' command (can be 'pay', 'block', 'add').");
            Console.WriteLine("\t=>Fourth keyword used for specify transaction side for 'pay' ");
            Console.WriteLine("See commands description:\n\n");
            ListCommands();
        }
    }
}
