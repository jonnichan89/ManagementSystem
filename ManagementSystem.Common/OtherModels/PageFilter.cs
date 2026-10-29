using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementSystem.Common.OtherModels
{
    public class PageFilterMstItem
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public int PageIndex { get; set; } = 1;
        public int RecordPerPage { get; set; } = 100;

    }
    public class PageFilterProjectTemplate
    {
        public string PTName { get; set; }
        public int PageIndex { get; set; } = 1;
        public int RecordPerPage { get; set; } = 100;

    }
    public class PageFilterItemCalc
    {
        public string ItemCode { get; set; }
        public int PageIndex { get; set; } = 1;
        public int RecordPerPage { get; set; } = 100;
    }
}
