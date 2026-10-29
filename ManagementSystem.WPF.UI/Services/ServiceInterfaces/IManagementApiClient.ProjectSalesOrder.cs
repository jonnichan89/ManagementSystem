namespace ManagementSystem.WPF.UI.Services.ServiceInterfaces
{
    public partial interface IManagementApiClient
    {
        Task<ServiceActionResult> SearchProjectSalesOrdersAsync(string PSCode, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetProjectSalesOrdersAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetProjectSalesOrderByIdAsync(int PSId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateProjectSalesOrderAsync(PSOrder ProjectSalesOrder, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateProjectSalesOrderAsync(PSOrder ProjectSalesOrder, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteProjectSalesOrderAsync(int PSId, CancellationToken cancellationToken = default);
    }
}
