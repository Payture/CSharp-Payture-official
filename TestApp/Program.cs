using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPayture;
using System.Diagnostics;

namespace TestApp
{
    partial class Program
    {
        static Random Random = new Random();

        static string _host = "http://sasha:7080";
        static string _merchantKey = "elena_Test";
        static string _merchantPassword = "789555";
        static Merchant _merchant = new Merchant( _merchantKey, _merchantPassword, _host );
        static PaytureResponse response = null;

        static void Main( string[] args )
        {
            try
            {


                Console.WriteLine( "Press space for get description of commands for this console program." );
                if ( Console.ReadKey().Key == ConsoleKey.Spacebar )
                {
                    Help();
                    Console.WriteLine( "Press space for get command's list." );
                    if ( Console.ReadKey().Key == ConsoleKey.Spacebar )
                        ListCommands();
                    Console.WriteLine( "Press enter for continue." ); Console.ReadLine();
                }
               // Console.WriteLine( $"Merchant account settings: {Environment.NewLine}\tMerchantName={_merchantKey}{Environment.NewLine}\tMerchantPassword={_merchantPassword}{Environment.NewLine}\tHOST={_host}{Environment.NewLine}" );
                Console.WriteLine( "Press space for change Merchant account settings" );
                if ( Console.ReadKey().Key == ConsoleKey.Spacebar )
                {
                    ChangeMerchant();
                }

                while ( true )
                {
                    Console.WriteLine( $"{Environment.NewLine}Press backspase for exit" );
                    if ( Console.ReadKey().Key == ConsoleKey.Backspace )
                        break;
                    Router();
                }

                Console.ReadLine();
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Error occurs: {ex.Message}{Environment.NewLine}{ex.StackTrace}" );
                Console.WriteLine( "Press any key for close console." );
                Console.ReadKey();
            }
        }
        
        static void WriteResult(PaytureResponse response)
        {
            if( response != null )
                Console.WriteLine( $"{Environment.NewLine}Response Result{Environment.NewLine}{response.APIName} Success={response.Success}; Attribute=[{response.Attributes.Aggregate( "", ( a, c ) => a += $"{c.Key}={c.Value}; " )}]" );
        }
        

    }

}
