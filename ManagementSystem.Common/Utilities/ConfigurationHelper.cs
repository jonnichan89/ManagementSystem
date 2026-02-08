using Microsoft.Extensions.Configuration;

namespace ManagementSystem.Common.Utilities
{
    public static class ConfigurationHelper
    {
        public static string? GetConnectionString(string configName)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();

            return configuration.GetConnectionString(configName);
        }
        public static IConfigurationSection GetFirstItem(string section)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();

            return configuration.GetSection(section).GetChildren().First();
        }
    }
}
