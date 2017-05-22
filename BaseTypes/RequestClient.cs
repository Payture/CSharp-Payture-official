﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CSharpPayture
{
    public abstract class RequestClient
    {
        protected HttpClient _client = new HttpClient();


        /// <summary>
        /// Method sends data in encoded url to the Payture server
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns>response string from the Payture server</returns>
        protected async Task<string> PostAsync( string url, FormUrlEncodedContent content )
        {
            try
            {
                var response = await _client.PostAsync( url, content );
                var respStr = await response.Content.ReadAsStringAsync();
                //OnResponseReceived( respStr );
                return respStr;
            }
            catch( Exception ex )
            {
               // OnExceptionOccurs( ex );
                _client = new HttpClient();
                return null;
            }
        }

        /// <summary>
        /// Helper method for parsing received response (that in XML format)
        /// </summary>
        /// <param name="body"></param>
        /// <returns>response object</returns>
        protected PaytureResponse ParseXMLResponse( string body )
        {
            XElement xmlBody = XElement.Parse( body );
            var attributes = new Dictionary<string, string>();
            var attrs = xmlBody.Attributes();
            if ( attrs != null )
            { 
                foreach ( var val in attrs.Select( n => new KeyValuePair<string, string>( n.Name.LocalName, n.Value ) ) )
                {
                    attributes.Add( val.Key, val.Value );
                }
            }
            var elems = ParseXMLElement( xmlBody );
            var paytureResponse = new PaytureResponse
            {
                Success = Boolean.Parse( attributes.Where( n => n.Key == "Success" ).FirstOrDefault().Value ),
                ErrCode = attributes.Where( n => n.Key == "ErrCode" ).FirstOrDefault().Value,
                Attributes = attributes,
                InternalElements = elems
            };
           // OnParseResponse( paytureResponse );
            return paytureResponse;
        }


        private dynamic ParseXMLElement( XElement element )
        {
            var elems = element.Elements().ToList();
            if ( elems == null || elems.Count() == 0 )
                return null;
            var result =   elems.Select( n =>
            {
                var internElems = ParseXMLElement( n );
                var elemAttrs = new List<dynamic>();
                foreach ( var v in n.Attributes() )
                    elemAttrs.Add( new { Name = v.Name.LocalName, Value = v.Value } );
                return new { Name = n.Name, Attributes = elemAttrs, InternalElemements = internElems };
            }).ToList();

            return result;
        }

        protected PaytureResponse ParseResponseInternal( Task<string> str, PaytureCommands command, SessionType sessionType = SessionType.None )
        {
            var result = str.Result;
            var paytureResponse = ParseXMLResponse( result );
            paytureResponse.APIName = command;
            return paytureResponse;
        }
    }
}
