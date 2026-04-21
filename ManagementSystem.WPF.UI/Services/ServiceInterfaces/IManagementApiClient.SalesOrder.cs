namespace ManagementSystem.WPF.UI.Services.ServiceInterfaces
{
    public partial interface IManagementApiClient
    {
        Task<ServiceActionResult> SearchSalesOrdersAsync(string PSCode, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetSalesOrdersAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetSalesOrderByIdAsync(int PSId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateSalesOrderAsync(PSOrder SalesOrder, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateSalesOrderAsync(PSOrder SalesOrder, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteSalesOrderAsync(int PSId, CancellationToken cancellationToken = default);
    }
}