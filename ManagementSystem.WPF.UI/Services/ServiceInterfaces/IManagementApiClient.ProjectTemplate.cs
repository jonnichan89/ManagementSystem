using ManagementSystem.Common.OtherModels;

namespace ManagementSystem.WPF.UI.Services.ServiceInterfaces
{
    public partial interface IManagementApiClient
    {
        //Task<ServiceActionResult> SearchProjectTemplatesAsync(string PTCode, CancellationToken cancellationToken = default);
        Task<ServiceActionResult<ProjectTemplate>> GetProjectTemplatesAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetAllProjectTemplateImagesAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult<ProjectTemplate>> GetAllProjectTemplatesAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetProjectTemplateByIdAsync(int PTId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetProjectTemplateLastModifiedAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateProjectTemplateAsync(ProjectTemplate ProjectTemplate, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateProjectTemplateAsync(ProjectTemplate ProjectTemplate, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteProjectTemplateAsync(int PTId, CancellationToken cancellationToken = default);
    }
}