
namespace ManagementSystem.DataServices.Resources
{
    public class SharedResources
    {
        /// jangan dibuang
        /// ini dipakai untuk Controllers.Common.resx
        public SharedResources(IStringLocalizerFactory factory)
        {
            _localizer = factory.Create("SharedResources", "ManagementSystem.DataServices");
        }

        private readonly IStringLocalizer _localizer;
        public string this[string key] => _localizer[key];

    }
    //public class SharedResourceService(IStringLocalizerFactory factory)
    //{
    //    private readonly IStringLocalizer _localizer = factory.Create("SharedResources", "ManagementSystem.DataServices");
    //    public string this[string key] => _localizer[key];
    //}
}
