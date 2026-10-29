
namespace ManagementSystem.WPF.UI.Services.ServiceInterfaces
{
    public partial interface IManagementApiClient
    {
        Task<ServiceActionResult> GetMstAccountsAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetMstAccountLastModifiedAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetMstAccountByIdAsync(int accountId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateMstAccountAsync(MstAccount account, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateMstAccountAsync(MstAccount account, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteMstAccountAsync(int accountId, CancellationToken cancellationToken = default);
    }
}