using Microsoft.Extensions.Localization;

namespace ManagementSystem.Services
{
    public class SharedResourceService(IStringLocalizerFactory factory)
    {
        private readonly IStringLocalizer _localizer = factory.Create("SharedResources", "ManagementSyStem.Data");
        public string this[string key] => _localizer[key];
    }
}
