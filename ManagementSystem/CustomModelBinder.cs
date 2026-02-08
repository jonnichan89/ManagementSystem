using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ManagementSystem
{
    public class EmptyStringModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(string))
            {
                return new EmptyStringModelBinder();
            }
            return null;
        }
    }
    public class EmptyStringModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            bindingContext.Result = ModelBindingResult.Success(valueProviderResult.FirstValue ?? string.Empty);
            return Task.CompletedTask;
        }
    }
}
