﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Yahoo.Infrastructure
{
    /// <summary>
    /// Represents registrar of Yahoo authentication service
    /// </summary>
    public class YahooAuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        public void Configure(AuthenticationBuilder builder)
        {
            builder.AddYahoo(AspNet.Security.OAuth.Yahoo.YahooAuthenticationDefaults.AuthenticationScheme, 
                options =>
            {
                //set credentials
                var settings = EngineContext.Current.Resolve<YahooExternalAuthSettings>();
                options.ClientId = settings.ClientKeyIdentifier;
                options.ClientSecret = settings.ClientSecret;
                foreach (var scope in YahooAuthenticationDefaults.Scopes) options.Scope.Add(scope);
                options.CallbackPath = YahooAuthenticationDefaults.CallbackPath;

                //store access and refresh tokens for the further usage
                options.SaveTokens = true;
                
                //set custom events handlers
                options.Events = new OAuthEvents
                {
                    //in case of error, redirect the user to the specified URL
                    OnRemoteFailure = context =>
                    {
                        context.HandleResponse();

                        var errorUrl = context.Properties?.GetString(YahooAuthenticationDefaults.ErrorCallback);
                        context.Response.Redirect(errorUrl);

                        return Task.FromResult(0);
                    }
                };
            });
        }
        
    }
}