using CSharpPayture;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
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


        #region Simple promts methods
        static PaytureAPIType PromtService()
        {
            Console.WriteLine( "Type the service api type: api, ewallet or inpay:" );
            var service = Console.ReadLine().ToUpper();
            if ( service == "EWALLET" || service == "E" )
                return PaytureAPIType.vwapi;
            else if ( service == "INPAY" || service == "I" )
                return PaytureAPIType.apim;
            else if ( service == "API" || service == "A" )
                return PaytureAPIType.api;
            else
            {
                Console.WriteLine( "Illegal service. Only API, EWALLET or INPAY avaliable." );
                PromtService();
                throw new Exception();
            }
        }

        static SessionType PromtSessionType()
        {
            Console.WriteLine( "Specify The Session Type: pay, block or for ewallet only - add:)" );
            var session = Console.ReadLine().ToUpper();
            if ( session == "PAY" || session == "P" )
                return SessionType.Pay;
            else if ( session == "BLOCK" || session == "B" )
                return SessionType.Block;
            else if ( session == "ADD" || session == "A" )
                return SessionType.Add;
            else
            {
                Console.WriteLine( "Illegal Session Type. Only pay, block or add avaliable." );
                PromtSessionType();
                throw new Exception();
            }
        }

        static bool PromtForUseRegCard()
        {
            Console.WriteLine( "Use registered card?  Note: type yes/no or y/n:" );
            var regCard = Console.ReadLine().ToUpper();
            if ( regCard == "YES" || regCard == "Y" )
                return true;
            else if ( regCard == "NO" || regCard == "N" )
                return false;
            else
            {
                Console.WriteLine( "Illegal input. Type yes/no or y/n for specify necessity of using registered card." );
                PromtForUseRegCard();
                throw new Exception();
            }
        }


        static bool PromtForUseSessionId( PaytureCommands command )
        {
            Console.WriteLine( $"Use SessionId for {command} command?  Note: type yes/no or y/n:" );
            var useSessionId = Console.ReadLine().ToUpper();
            if ( useSessionId == "YES" || useSessionId == "Y" )
                return true;
            else if ( useSessionId == "NO" || useSessionId == "N" )
                return false;
            else
            {
                Console.WriteLine( $"Illegal input. Type yes/no or y/n for specify necessity of using SessionId in {command} operation." );
                PromtForUseRegCard();
                throw new Exception();
            }
        }
        #endregion Simple promts methods


        #region Router
        public static void Router ( )
        {
            Console.WriteLine( "Type the command:" );
            var cmd = Console.ReadLine().ToUpper();
            var apiType = PaytureAPIType.api;
            switch ( cmd )
            {
                case "PAY":
                    {
                        apiType = PromtService();
                        if ( apiType == PaytureAPIType.api )
                        {
                            APIPayOrBlock( PaytureCommands.Pay );
                            break;
                        }
                        
                        if( PromtForUseSessionId( PaytureCommands.Pay ) )
                        {
                            PayturePayOrAdd( PaytureCommands.Pay );
                            break;
                        }

                        //Only EWallet here
                        var customer = GetCustomer();
                        var data = DataForInit( PromtSessionType() );

                        var regCard = PromtForUseRegCard();
                        if( !regCard )
                        {
                            var card = GetCard();
                            response = _merchant.EWallet( PaytureCommands.Pay ).ExpandTransaction( customer, card, data ).ProcessOperation();
                            break;

                        }

                        var cardId = allFields[ PaytureParams.CardId ];
                        var secureCode = allFields[ PaytureParams.SecureCode ];
                        Console.WriteLine( $"CardId={cardId}; SecureCode={secureCode};" );
                        CircleChanges( "CardId and SecureCode" );

                        response = _merchant.EWallet( PaytureCommands.Pay )
                                            .ExpandTransaction(customer,  allFields[ PaytureParams.CardId ],  int.Parse(allFields[ PaytureParams.SecureCode ]),  data)
                                            .ProcessOperation();
                        break;
                    }
                case "BLOCK":
                    {
                        APIPayOrBlock( PaytureCommands.Block );
                        break;
                    }
                case "CHARGE":
                    {
                        ChargeUnblockRefundGetState(  PaytureCommands.Charge );
                        break;
                    }
                case "REFUND":
                    {
                        ChargeUnblockRefundGetState(  PaytureCommands.Refund );
                        break;
                    }
                case "UNBLOCK":
                    {
                        ChargeUnblockRefundGetState(  PaytureCommands.Unblock );
                        break;
                    }
                case "GETSTATE":
                    {
                        ChargeUnblockRefundGetState(  PaytureCommands.GetState );
                        break;
                    }
                case "PAYSTATUS":
                    {
                        ChargeUnblockRefundGetState(  PaytureCommands.PayStatus );
                        break;
                    }
                case "INIT":
                    {
                        apiType = PromtService();

                        var sessionType = PromtSessionType();
                        var data = DataForInit( sessionType );

                        if ( apiType == PaytureAPIType.vwapi )
                        {
                            var customer = GetCustomer();
                            var cardId = allFields[ PaytureParams.CardId ];
                            response = _merchant.EWallet( PaytureCommands.Init ).ExpandTransaction( customer, cardId, data ).ProcessOperation();
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
                        if ( PromtForUseSessionId( PaytureCommands.Add ) )
                        {
                            PayturePayOrAdd( PaytureCommands.Add );
                            break;
                        }

                        //EWallet add card on Merchant side
                        var customer = GetCustomer();
                        var card = GetCard();

                        response = _merchant.EWallet( PaytureCommands.Add ).ExpandTransaction(customer, card).ProcessOperation();  
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
                case "HELP":
                    {
                        Help();
                        break;
                    }
            }
            if ( !new[] { "FIELDS", "CHANGEFIELDS", "COMMANDS", "CHANGEMERCHANT", "HELP" }.Contains( cmd ) )
                WriteResult( response );

        }
        #endregion Router



        static void GenerateOrderId()
        {
            allFields[ PaytureParams.OrderId ] = $"ORD_{Random.Next( 0, int.MaxValue )}_TEST";
        }

        static void GenerateAmount()
        {
            var num = Random.Next( 50, 100000 );
            allFields[ PaytureParams.Amount ] =  num.ToString();
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
            allFields[ PaytureParams.Total ] = allFields[ PaytureParams.Amount ];
            return new Data
            {
                Amount = allFields[ PaytureParams.Amount ] == null ? null : (long?)long.Parse( allFields[ PaytureParams.Amount ] ),
                IP = allFields[ PaytureParams.IP ],
                Language = allFields[ PaytureParams.Language ],
                OrderId = allFields[ PaytureParams.OrderId ],
                SessionType = allFields[ PaytureParams.SessionType ],
                TemplateTag = allFields[ PaytureParams.TemplateTag ],
                Total = allFields[ PaytureParams.Amount ] == null ? null : (long?)long.Parse( allFields[ PaytureParams.Amount ] ),
                Product = allFields[ PaytureParams.Product ]
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
        private static void  ChargeUnblockRefundGetState( PaytureCommands command )
        {
            var api = PromtService();
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
            Console.WriteLine("Commands for help:{0}{0}" + 
                    "* fields - list current key-value pairs that used in request to Payture server.{0}{0}" +
                    "* changefields - command for changing current values of  key-value pairs that used in request to Payture server.{0}{0}" + 
                    "* commands - list avaliable commands for this console program.{0}{0}" + 
                    "* changemerchant - commands for changing current merchant account settings.{0}{0}" +
                    "* help - commands that types this text (description of commands that you can use in this console program.).{0}{0}{0}", Environment.NewLine);
            Console.WriteLine("Commands for invoke PaytureAPI functions.{0}" +
                    "* pay - use for one-stage payment. In EWALLET an INPAY api this command can be use for block funds - if you specify SessionType=Block.{0}{0}" +
                    "* block - use for block funds on Customer card. After that command the funds can be charged by Charge command or unblocked by Unblock command. This command use only for API.{0}{0}" + 
                    "* charge - write-off of funds from customer card.{0}{0}" + 
                    "* unblock - unlocking of funds on customer card.{0}{0}" +
                    "* refund - operation for refunds.{0}{0}" + 
                    "* getsstate - use for getting the actual state of payments in Payture processing system. This command use only for API.{0}{0}" +
                    "* paystatus - use for getting the actual state of payments in Payture processing system. This command use for EWALLET and INPAY.{0}{0}" + 
                    "* init - use for payment initialization, customer will be redirected on Payture payment gateway page for enter card's information.{0}{0}" + 
                    "* register - register new customer. This command use only for EWALLET.{0}{0}" +
                    "* check - check for existing customer account in Payture system. This command use only for EWALLET.{0}{0}" + 
                    "* update - This command use only for EWALLET.{0}{0}" + 
                    "* delete - delete customer account from Payture system. This command use only for EWALLET.{0}{0}" +
                    "* add - register new card in Payture system. This command use only for EWALLET.{0}{0}" +
                    "* activate - activate registered card in Payture system. This command use only for EWALLET.{0}{0}" +   
                    "* sendcode - provide additional authentication for customer payment. This command use only for EWALLET.{0}{0}" +
                    "* remove - delete card from Payture system. This command use only for EWALLET.{0}{0}", Environment.NewLine  );
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
            Console.WriteLine("\n\nThen console promt you 'Type command' - you can type commands for invoke PaytureAPI functions and you can types commands for help.");
            Console.WriteLine("After you type the command an appropriate method will be execute. If the data is not enough for execute the program promt for additional input.");
        }
    }
}
