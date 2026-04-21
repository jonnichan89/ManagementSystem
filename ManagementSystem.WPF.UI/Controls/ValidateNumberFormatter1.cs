using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementSystem.WPF.UI.Controls
{
    public class ValidateNumberFormatter1 : INumberFormatter, INumberParser
    {
        public ValidateNumberFormatter1(string currencyId)
        {
            CurrencyId = currencyId;
        }

        private string _stringFormat;
        private string _currencyId;
        public string CurrencyId
        {
            set
            {
                _currencyId = value;
                switch (_currencyId)
                {
                    case "IDR":
                        _stringFormat = "N0";
                        break;
                    default:
                        _stringFormat = "N2";
                        break;
                }
            }
        }

        public string FormatDouble(double? value)
        {
            return value?.ToString(_stringFormat, GetCurrentCultureConverter()) ?? string.Empty;
        }

        public string FormatInt(int? value)
        {
            return value?.ToString(_stringFormat, GetCurrentCultureConverter()) ?? string.Empty;
        }

        public string FormatUInt(uint? value)
        {
            return value?.ToString(_stringFormat, GetCurrentCultureConverter()) ?? string.Empty;
        }

        public double? ParseDouble(string? value)
        {
            return double.TryParse(value, out double d) ? d : null;
        }

        public int? ParseInt(string? value)
        {
            return int.TryParse(value, out int i) ? i : null;
        }

        public uint? ParseUInt(string? value)
        {
            return uint.TryParse(value, out uint ui) ? ui : (uint?)null;
        }

        private static CultureInfo GetCurrentCultureConverter()
        {
            return CultureInfo.CurrentCulture;
        }
    }

}
