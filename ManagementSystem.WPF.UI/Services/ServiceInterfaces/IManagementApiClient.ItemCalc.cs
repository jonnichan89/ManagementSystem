
using ManagementSystem.Common.OtherModels;

namespace ManagementSystem.WPF.UI.Services.ServiceInterfaces
{
    public partial interface IManagementApiClient
    {
        Task<ServiceActionResult<ItemCalc>> GetItemCalcsAsync(int pageIndex, PageFilterItemCalc pageFilter, CancellationToken cancellationToken = default);
        Task<ServiceActionResult<ItemCalc>> GetAllItemCalcsAsync(string itemCode, CancellationToken cancellationToken = default);
    }
}