
namespace ManagementSystem.WPF.UI.Helpers
{
    public static class HttpContentHelper
    {
        public static Dictionary<string, StringContent> ConvertToStringContent<T>(T obj, string prefix)
        {
            Dictionary<string, StringContent> returnValue = new Dictionary<string, StringContent>();
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                if (p.GetMethod == null) continue;
                var val = p.GetValue(obj);
                if (val == null) continue;

                string strVal;
                if (val is DateTime dt) strVal = dt.ToString("o", CultureInfo.InvariantCulture);
                else if (val is bool b) strVal = b ? "True" : "False";
                else if (val is IFormattable fmt) strVal = fmt.ToString(null, CultureInfo.InvariantCulture);
                else strVal = val.ToString() ?? string.Empty;

                returnValue.Add(p.Name, new StringContent(strVal));
            }

            return returnValue;

        }
    }
}
