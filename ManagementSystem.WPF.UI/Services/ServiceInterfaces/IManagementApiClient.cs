using ManagementSystem.Common.OtherModels;

namespace ManagementSystem.WPF.UI.Services.ServiceInterfaces
{
    public partial interface IManagementApiClient
    {
        Task<string> DownloadImageFile(string localImageFolderPath, string urlImageFilePath, int imageFileSize, CancellationToken cancellationToken = default);

        Task<ServiceActionResult> GetMstUsersAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetMstUserByUserNameAsync(string userName, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateMstUserAsync(MstUser user, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateMstUserAsync(MstUser user, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteMstUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<ServiceActionResult<MstItem>> GetMstItemsAsync(int pageIndex, PageFilterMstItem pageFilter, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetAllMstItemImagesAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult<MstItem>> GetAllMstItemsAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetMstItemCountAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetMstItemLastModifiedAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetMstItemByIdAsync(int itemId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateMstItemAsync(MstItem item, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateMstItemAsync(MstItem item, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteMstItemAsync(int itemId, CancellationToken cancellationToken = default);


        Task<ServiceActionResult> GetMstSuppliersAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetMstSupplierLastModifiedAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetMstSupplierByIdAsync(int supplierId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateMstSupplierAsync(MstSupplier supplier, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateMstSupplierAsync(MstSupplier supplier, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteMstSupplierAsync(int supplierId, CancellationToken cancellationToken = default);


        Task<ServiceActionResult> GetItemUsagesAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetItemUsageByIdAsync(int itemUsageId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateItemUsageAsync(ItemUsage itemUsage, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateItemUsageAsync(ItemUsage itemUsage, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteItemUsageAsync(int itemUsageId, CancellationToken cancellationToken = default);

    }
}