using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.ExternalAuth.Yahoo.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.ExternalAuth.Yahoo.Controllers
{
    public class YahooAuthenticationController : BasePluginController
    {
        #region Feilds

        private readonly YahooExternalAuthSettings _yahooExternalAuthSettings;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        
        #endregion

        #region Ctor
        
        public YahooAuthenticationController(IPermissionService permissionService, 
            YahooExternalAuthSettings yahooExternalAuthSettings, ISettingService settingService, 
            INotificationService notificationService, ILocalizationService localizationService, 
            IAuthenticationPluginManager authenticationPluginManager, IWorkContext workContext, 
            IStoreContext storeContext, IExternalAuthenticationService externalAuthenticationService)
        {
            _permissionService = permissionService;
            _yahooExternalAuthSettings = yahooExternalAuthSettings;
            _settingService = settingService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _authenticationPluginManager = authenticationPluginManager;
            _workContext = workContext;
            _storeContext = storeContext;
            _externalAuthenticationService = externalAuthenticationService;
        }

        #endregion
        #region Methods
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                ClientId = _yahooExternalAuthSettings.ClientKeyIdentifier,
                ClientSecret = _yahooExternalAuthSettings.ClientSecret
            };

            return View("~/Plugins/ExternalAuth.Yahoo/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(
                StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //save settings
            _yahooExternalAuthSettings.ClientKeyIdentifier = model.ClientId;
            _yahooExternalAuthSettings.ClientSecret = model.ClientSecret;
            await _settingService.SaveSettingAsync(_yahooExternalAuthSettings);


            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }
        
        public async Task<IActionResult> Login(string returnUrl)
        {
            var methodIsAvailable = await _authenticationPluginManager
                .IsPluginActiveAsync(YahooAuthenticationDefaults.SystemName, await _workContext.GetCurrentCustomerAsync(), (await _storeContext.GetCurrentStoreAsync()).Id);
            if (!methodIsAvailable)
                throw new NopException("Yahoo authentication module cannot be loaded");

            if (string.IsNullOrEmpty(_yahooExternalAuthSettings.ClientKeyIdentifier) ||
                string.IsNullOrEmpty(_yahooExternalAuthSettings.ClientSecret))
            {
                throw new NopException("Yahoo authentication module not configured");
            }
            
            //configure login callback action
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginCallback", "YahooAuthentication"),
            };
            authenticationProperties.SetString(YahooAuthenticationDefaults.ErrorCallback, Url.RouteUrl("Login", new { returnUrl }));
            return Challenge(authenticationProperties, AspNet.Security.OAuth.Yahoo.YahooAuthenticationDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoginCallback([FromQuery] string code, [FromQuery] string returnUrl)
        {
            //authenticate Yahoo user
            var authenticateResult = await HttpContext.AuthenticateAsync(
                AspNet.Security.OAuth.Yahoo.YahooAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded || 
                authenticateResult.Principal == null || 
                !authenticateResult.Principal.Claims.Any())
                return RedirectToRoute("Login");

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = YahooAuthenticationDefaults.SystemName,
                AccessToken = await HttpContext.GetTokenAsync(
                    AspNet.Security.OAuth.Yahoo.YahooAuthenticationDefaults.AuthenticationScheme,
                    "access_token"),
                Email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value,
                ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
                Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
            };

            //authenticate Nop user
            return await _externalAuthenticationService.AuthenticateAsync(authenticationParameters, returnUrl);
        }
        
        #endregion
    }
}