using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementSystem.Common.OtherModels
{
    public class SearchPSOrderForOptions
    {
        public enum PSTypeEnum
        {
            None = 0,
            Purchase = 1,
            Sales = 2,
            Other = 3
        }
        public PSTypeEnum PSType { get; set; }

        public string PSCode { get; set; }
    }
    public class SearchOptionResultPSOrder
    {
        public string PSCode { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
    }
    public class SearchOptionResultProjectTemplate
    {
        public string PTName { get; set; }
    }
}
