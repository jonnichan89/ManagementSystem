
namespace ManagementSystem.WPF.UI.Services
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the typed Management API client. Provide the API base address (e.g. https://localhost:5001/).
        /// </summary>
        public static IServiceCollection AddManagementApiClient(this IServiceCollection services, Uri baseAddress)
        {
            if (baseAddress == null) throw new ArgumentNullException(nameof(baseAddress));

            services.AddHttpClient<IManagementApiClient, ManagementApiClient>(client =>
            {
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            return services;
        }
    }
}