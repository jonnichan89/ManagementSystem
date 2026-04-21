namespace ManagementSystem.WPF.UI.Services.ServiceInterfaces
{
    public partial interface IManagementApiClient
    {
        Task<ServiceActionResult> SearchPurchaseOrdersAsync(string PSCode, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetPurchaseOrdersAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetPurchaseOrderByIdAsync(int PSId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreatePurchaseOrderAsync(PSOrder PurchaseOrder, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdatePurchaseOrderAsync(PSOrder PurchaseOrder, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeletePurchaseOrderAsync(int PSId, CancellationToken cancellationToken = default);
    }
}