using Dapper;
using ManagementSystem.Common.OtherModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Data;

namespace ManagementSystem.DataServices.Service
{
    public class ItemCalcService
    {
        //SharedResources _resShared;
        //public ItemCalcService(SharedResources resShared) { }
        public ItemCalcService() { }

        public async Task<IEnumerable<ItemCalc>> GetItemCalcs(int pageIndex, PageFilterItemCalc? pageFilter)
        {
            List<ItemCalc> items = null;

            using (var context = new ManagementSystemContext())
            {
                if (pageFilter == null)
                {
                    pageFilter = new PageFilterItemCalc();
                }

                pageFilter.PageIndex = pageIndex;

                string query = @"
/*
DECLARE @ItemCode VARCHAR(50) = ''
DECLARE @PageIndex INT = 1
DECLARE @RecordPerPage INT = 100
*/

DECLARE @OffsetRows INT = (@PageIndex - 1) * @RecordPerPage;

IF @ItemCode <> ''
	SET @ItemCode = '%' + @ItemCode + '%';

SELECT I.*
FROM ItemCalc I
WHERE (@ItemCode = '' OR (@ItemCode <> '' AND I.ItemCode LIKE @ItemCode))
ORDER BY I.CalcAt DESC
OFFSET @OffsetRows ROWS
FETCH NEXT @RecordPerPage ROWS ONLY
";

                using (SqlConnection sqlConnection = context.Database.GetDbConnection() as SqlConnection)
                {
                    if (sqlConnection.State != ConnectionState.Open)
                    {
                        sqlConnection.Open();
                    }

                    var queryResult = await sqlConnection.QueryAsync<ItemCalc>(query, pageFilter);
                    items = queryResult.ToList();

                    sqlConnection.Close();
                }
            }
            return items;
        }

        public async Task<IEnumerable<ItemCalc>> GetAllItemCalcs(string itemCode)
        {
            using (var context = new ManagementSystemContext())
            {
                return await context.ItemCalcs.Where(p => p.ItemCode == itemCode).ToListAsync();
            }
        }

    }
}
