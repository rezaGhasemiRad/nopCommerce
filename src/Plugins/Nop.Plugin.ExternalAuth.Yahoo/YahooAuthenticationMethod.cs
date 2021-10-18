using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.ExternalAuth.Yahoo
{
    /// <summary>
    /// Represents method for the authentication with Yahoo account
    /// </summary>
    public class YahooAuthenticationMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public YahooAuthenticationMethod(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
        }

        #endregion
        
        
        #region Methods
        
        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/YahooAuthentication/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return YahooAuthenticationDefaults.VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new YahooExternalAuthSettings());

            //locales

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.ExternalAuth.Yahoo.ClientKeyIdentifier"] = "Client Id",
                ["Plugins.ExternalAuth.Yahoo.ClientKeyIdentifier.Hint"] = "Enter your app ID/API key here. You can find it on your Yahoo application page.",
                ["Plugins.ExternalAuth.Yahoo.ClientSecret"] = "Client secret",
                ["Plugins.ExternalAuth.Yahoo.ClientSecret.Hint"] = "Enter your app secret here. You can find it on your Yahoo application page.",
                ["Plugins.ExternalAuth.Yahoo.Instructions"] = 
                    "<p>To configure authentication with Yahoo, please follow these steps:<br/><br/><ol>" +
                    "<li>Navigate to the <a href=\"https://developer.yahoo.com/apps/\" target =\"_blank\" > Yahoo for Developers</a> page and sign in. If you don't already have a Yahoo account, use the <b>Create an account</b> link on the login page to create one.</li>" +
                    "<li>Tap the <b>Create an App</b> in the upper right corner to create a new App ID. " +
                    "<li>Enter \"{0:s}YahooAuthentication/LoginCallback\" into the <b>Redirect URI(s)</b> field.</li>" +
                    "<li>Click on the <b>Create App</b>.</li>" +
                    "</li><li>You are now presented your apps information" +
                    "</li><li>Copy your Client ID and Client Secret below.</li></ol><br/><br/></p>"
            });
            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<YahooExternalAuthSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.ExternalAuth.Yahoo");

            await base.UninstallAsync();
        }

        #endregion

        
    }
}