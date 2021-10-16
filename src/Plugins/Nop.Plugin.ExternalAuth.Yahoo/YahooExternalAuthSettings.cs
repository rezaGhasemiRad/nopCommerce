using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalAuth.Yahoo
{
    /// <summary>
    /// Represents settings of the Yahoo authentication method
    /// </summary>
    public class YahooExternalAuthSettings : ISettings
    {
        /// <summary>
        /// Gets or sets OAuth2 client identifier
        /// </summary>
        public string ClientKeyIdentifier { get; set; }

        /// <summary>
        /// Gets or sets OAuth2 client secret
        /// </summary>
        public string ClientSecret { get; set; }
    }
}