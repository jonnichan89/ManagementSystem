using ManagementSystem.Common.OtherModels;

namespace ManagementSystem.WPF.UI.Services.ServiceInterfaces
{
    public partial interface IManagementApiClient
    {
        Task<ServiceActionResult> GetItemAdjustmentsAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetItemAdjustmentByIdAsync(int IAId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateItemAdjustmentAsync(ItemAdjustment itemAdjustment, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateItemAdjustmentAsync(ItemAdjustment itemAdjustment, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteItemAdjustmentAsync(int IAId, CancellationToken cancellationToken = default);

    }
}