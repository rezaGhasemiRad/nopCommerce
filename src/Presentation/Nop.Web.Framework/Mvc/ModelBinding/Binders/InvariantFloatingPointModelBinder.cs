using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// Represents model binder for floating-point types
    /// </summary>
    public class InvariantFloatingPointModelBinder : IModelBinder
    {

        #region Fields

        private readonly IModelBinder _baseBinder;
        private readonly NumberStyles _supportedStyles;
        private readonly ILoggerFactory _loggerFactory;

        #endregion

        #region Ctor

        public InvariantFloatingPointModelBinder(NumberStyles supportedStyles, ILoggerFactory loggerFactory, Type type)
        {
            if (loggerFactory is null)
                throw new ArgumentNullException(nameof(loggerFactory));

            if (type is null)
                throw new ArgumentNullException(nameof(type));

            _supportedStyles = supportedStyles;
            _loggerFactory = loggerFactory;

            _baseBinder = Type.GetTypeCode(type) switch
            {
                TypeCode.Single => new FloatModelBinder(_supportedStyles, _loggerFactory),
                TypeCode.Decimal => new DecimalModelBinder(_supportedStyles, _loggerFactory),
                TypeCode.Double => new DoubleModelBinder(_supportedStyles, _loggerFactory),
                _ => new SimpleTypeModelBinder(type, loggerFactory)
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attempts to bind a model
        /// </summary>
        /// <param name="bindingContext">Model binding context</param>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext is null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            var modelState = bindingContext.ModelState;
            modelState.SetModelValue(modelName, valueProviderResult);

            var metadata = bindingContext.ModelMetadata;
            var type = metadata.UnderlyingOrModelType;

            var value = valueProviderResult.FirstValue;
            object model = null;

            //kendo uses a separator that is different for both culture variants
            value = value.Replace("/", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);

            if (type == typeof(float) && float.TryParse(value, _supportedStyles, CultureInfo.InvariantCulture, out var floatResult))
                model = floatResult;

            if (type == typeof(decimal) && decimal.TryParse(value, _supportedStyles, CultureInfo.InvariantCulture, out var decimalResult))
                model = decimalResult;

            if (type == typeof(double) && double.TryParse(value, _supportedStyles, CultureInfo.InvariantCulture, out var doubleResult))
                model = doubleResult;

            if (model is null)
                return _baseBinder.BindModelAsync(bindingContext);

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }

        #endregion
    }
}