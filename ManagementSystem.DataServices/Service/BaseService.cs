using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementSystem.DataServices.Service
{
    public abstract class BaseService
    {
        protected internal void ExecuteRecalItemCalc(ManagementSystemContext context, DateTime fromDate)
        {
            context.
                Database.
                ExecuteSqlInterpolated($"EXECUTE ManagementSystem.dbo.RecalItemCalc @FromDate = {fromDate:yyyy/MM/dd}");
        }
    }
}
