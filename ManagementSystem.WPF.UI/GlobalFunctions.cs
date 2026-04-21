
using ManagementSystem.Common.OtherModels;
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;

namespace ManagementSystem.WPF;

public class GlobalFunctions
{

    private ObservableRangeCollection<MstItem> _items = new();
    private ObservableRangeCollection<MstSupplier> _suppliers = new();

    public ObservableRangeCollection<MstItem> Items
    {
        get { return _items; }
    }

    public ObservableRangeCollection<MstSupplier> Suppliers
    {
        get { return _suppliers; }
    }

    private IManagementApiClient _apiClient;
    private JsonSerializerOptions _jsonSerializerOptions;

    public GlobalFunctions(
        IManagementApiClient apiClient,
        JsonSerializerOptions jsonSerializerOptions)
    {
        _apiClient = apiClient;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public async Task RefreshItemDataAsync(bool forceRefresh)
    {
        if (!forceRefresh && Items.Count > 0)
        {
            return;
        }

        Items.Clear();
        var actionResult = await _apiClient.GetMstItemsAsync();
        if (actionResult.ActionIsSuccess)
        {
            Items.Clear(); // Clear 1x lagi karena Async bisa jadi form lain juga sedang call ini.
            Items.AddRange((IEnumerable<MstItem>)actionResult.ActionResult);
        }
        else
        {
            throw new Exception(
                string.Format(
                    TranslationSource.Instance["messageTitleLoadListData"] ?? string.Empty,
                    TranslationSource.Instance["mwItems"] ?? string.Empty));
        }
    }

    public async Task RefreshSupplierDataAsync(bool forceRefresh)
    {
        if (!forceRefresh && Suppliers.Count > 0)
        {
            return;
        }

        Suppliers.Clear();
        var actionResult = await _apiClient.GetMstSuppliersAsync();
        if (actionResult.ActionIsSuccess)
        {
            Suppliers.Clear(); // Clear 1x lagi karena Async bisa jadi form lain juga sedang call ini.
            Suppliers.AddRange((IEnumerable<MstSupplier>)actionResult.ActionResult);
        }
        else
        {
            throw new Exception(
                string.Format(
                    TranslationSource.Instance["messageTitleLoadListData"] ?? string.Empty,
                    TranslationSource.Instance["mwMstSupplier"] ?? string.Empty));
        }
    }

    public async Task<IEnumerable<SearchOptionResultPSOrder>> SearchPurchaseOrderAsync(string poCode)
    {
        var actionResult = await _apiClient.SearchPurchaseOrdersAsync(poCode);
        if (actionResult.ActionIsSuccess)
        {
            return (IEnumerable<SearchOptionResultPSOrder>)actionResult.ActionResult;
        }
        else
        {
            throw new Exception(
                string.Format(
                    TranslationSource.Instance["messageTitleLoadListData"] ?? string.Empty,
                    TranslationSource.Instance["mwPurchaseOrder"] ?? string.Empty));
        }
    }

    public async Task<IEnumerable<SearchOptionResultPSOrder>> SearchSalesOrderAsync(string poCode)
    {
        var actionResult = await _apiClient.SearchSalesOrdersAsync(poCode);
        if (actionResult.ActionIsSuccess)
        {
            return (IEnumerable<SearchOptionResultPSOrder>)actionResult.ActionResult;
        }
        else
        {
            throw new Exception(
                string.Format(
                    TranslationSource.Instance["messageTitleLoadListData"] ?? string.Empty,
                    TranslationSource.Instance["mwSalesOrder"] ?? string.Empty));
        }
    }

}