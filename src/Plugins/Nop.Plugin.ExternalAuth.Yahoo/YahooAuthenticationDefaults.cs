using System;
using System.Collections.Generic;
using iTextSharp.text;

namespace Nop.Plugin.ExternalAuth.Yahoo
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public class YahooAuthenticationDefaults
    {
        /// <summary>
        /// Gets a name of the view component to display login button
        /// </summary>
        public const string VIEW_COMPONENT_NAME = "YahooAuthentication";

        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public static string SystemName = "ExternalAuth.Yahoo";

        /// <summary>
        /// Gets a name of error callback method
        /// </summary>
        public static string ErrorCallback = "ErrorCallback";

        /// <summary>
        /// Path for redirect after successfule login in yahoo
        /// </summary>
        public static string CallbackPath = "/YahooAuthentication/LoginCallback";


        /// <summary>
        /// Scopes requesting from Yahoo oauth provider
        /// </summary>
        public static List<string> Scopes = new() { "profile", "openid", "email"};
        
        /// <summary>
        /// Gets the base URL of onboarding services
        /// </summary>
        public static string ServiceUrl => "https://www.nopcommerce.com/";
    }
}