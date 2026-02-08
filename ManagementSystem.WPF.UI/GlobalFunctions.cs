
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

}