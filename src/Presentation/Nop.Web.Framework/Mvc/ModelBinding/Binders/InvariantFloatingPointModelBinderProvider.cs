using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// Represents a model binder provider for binding floating-point types
    /// </summary>
    public class InvariantFloatingPointModelBinderProvider : IModelBinderProvider
    {
        private const NumberStyles SUPPORTED_STYLES = NumberStyles.Float | NumberStyles.AllowThousands;

        /// <summary>
        /// Creates a model binder
        /// </summary>
        /// <param name="context">Context object</param>
        /// <returns>Instance of model binder for floating-point types</returns>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var modelType = context.Metadata.UnderlyingOrModelType;
            var loggerFactory = EngineContext.Current.Resolve<ILoggerFactory>();

            if (new[] { typeof(float), typeof(decimal), typeof(double) }.Contains(modelType))
            {
                return new InvariantFloatingPointModelBinder(SUPPORTED_STYLES, loggerFactory, modelType);
            }

            return null;
        }
    }
}