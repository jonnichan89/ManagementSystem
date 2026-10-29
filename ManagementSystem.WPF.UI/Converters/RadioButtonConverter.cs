
namespace ManagementSystem.WPF.UI.Converters
{
    public class RadioButtonConverter : IValueConverter
    {
        // Sets the RadioButton to checked if the ViewModel property matches the Parameter
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        // Writes the Parameter value into the ViewModel property when the user clicks the RadioButton
        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is true ? parameter?.ToString() : Binding.DoNothing;
        }
    }
}
