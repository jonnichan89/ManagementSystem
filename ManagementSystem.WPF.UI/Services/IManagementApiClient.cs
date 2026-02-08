
namespace ManagementSystem.WPF.UI.Services
{
    public interface IManagementApiClient
    {
        Task<string> DownloadItemImageFile(string imageFilePath, CancellationToken cancellationToken = default);

        Task<ServiceActionResult> GetMstUsersAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetMstUserByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateMstUserAsync(MstUser user, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateMstUserAsync(MstUser user, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteMstUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<ServiceActionResult> GetMstItemsAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetMstItemByIdAsync(int itemId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateMstItemAsync(MstItem item, Stream? imageStream = null, string imageFileName = "", CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateMstItemAsync(MstItem item, Stream? imageStream = null, string imageFileName = "", CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteMstItemAsync(int itemId, CancellationToken cancellationToken = default);


        Task<ServiceActionResult> GetMstSuppliersAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetMstSupplierByIdAsync(int supplierId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateMstSupplierAsync(MstSupplier supplier, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateMstSupplierAsync(MstSupplier supplier, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteMstSupplierAsync(int supplierId, CancellationToken cancellationToken = default);


        Task<ServiceActionResult> GetItemUsagesAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetItemUsageByIdAsync(int itemUsageId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreateItemUsageAsync(ItemUsage itemUsage, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdateItemUsageAsync(ItemUsage itemUsage, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeleteItemUsageAsync(int itemUsageId, CancellationToken cancellationToken = default);


        Task<ServiceActionResult> GetPurchaseOrdersAsync(CancellationToken cancellationToken = default);
        Task<ServiceActionResult> GetPurchaseOrderByIdAsync(int POId, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> CreatePurchaseOrderAsync(PurchaseOrder PurchaseOrder, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> UpdatePurchaseOrderAsync(PurchaseOrder PurchaseOrder, CancellationToken cancellationToken = default);
        Task<ServiceActionResult> DeletePurchaseOrderAsync(int POId, CancellationToken cancellationToken = default);


    }
}