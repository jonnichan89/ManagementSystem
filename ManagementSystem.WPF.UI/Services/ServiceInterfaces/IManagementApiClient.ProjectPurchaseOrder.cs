namespace ManagementSystem.WPF.UI.Services.ServiceInterfaces
{
    public partial interface IManagementApiClient
    {
        Task<ServiceActionResult> SearchProjectPurchaseOrdersAsync(string PSCode, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetProjectPurchaseOrdersAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetProjectPurchaseOrderByIdAsync(int PSId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateProjectPurchaseOrderAsync(PSOrder ProjectPurchaseOrder, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateProjectPurchaseOrderAsync(PSOrder ProjectPurchaseOrder, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteProjectPurchaseOrderAsync(int PSId, CancellationToken cancellationToken = default);
    }
}
