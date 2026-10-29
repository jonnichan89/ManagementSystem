
using ManagementSystem.WPF.UI.Data;

namespace ManagementSystem.WPF.UI.Services.ServiceInterfaces
{
    public partial interface IManagementApiClient
    {
        Task<ServiceActionResult> LoginAsync(LoginUser user, CancellationToken cancellationToken = default);
    }
}