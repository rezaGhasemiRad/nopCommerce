using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.ExternalAuth.Yahoo.Models
{
    /// <summary>
    /// Represents plugin configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.ExternalAuth.Yahoo.ClientKeyIdentifier")]
        public string ClientId { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.Yahoo.ClientSecret")]
        public string ClientSecret { get; set; }
    }
}