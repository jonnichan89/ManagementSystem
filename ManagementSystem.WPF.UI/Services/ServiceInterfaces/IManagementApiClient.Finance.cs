
namespace ManagementSystem.WPF.UI.Services.ServiceInterfaces
{
    public partial interface IManagementApiClient
    {
        Task<ServiceActionResult> GetFinancesAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetFinanceByIdAsync(int accountId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateFinanceAsync(Finance account, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateFinanceAsync(Finance account, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteFinanceAsync(int accountId, CancellationToken cancellationToken = default);
    }
}