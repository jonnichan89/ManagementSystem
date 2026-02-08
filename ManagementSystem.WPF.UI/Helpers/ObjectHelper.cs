
namespace ManagementSystem.WPF.UI.Helpers
{
    public static class ObjectHelper
    {
        public static Dictionary<string, object> CollectPropertiesStartingWith(object obj, string prefix)
        {
            // Use a Dictionary to store the name and value of the matching properties
            var matchingProperties = new Dictionary<string, object>();

            // Get the Type of the object
            Type type = obj.GetType();

            // Get all public properties and filter them
            IEnumerable<PropertyInfo> properties = type.GetProperties()
                .Where(p => p.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)); // Use OrdinalIgnoreCase for case-insensitivity

            // Iterate through the filtered properties and get their values
            foreach (PropertyInfo prop in properties)
            {
                // Get the value of the property for the given object instance
                object value = prop.GetValue(obj, null);
                matchingProperties.Add(prop.Name, value);
            }

            return matchingProperties;
        }
    }
}
