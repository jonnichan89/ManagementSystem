
using ManagementSystem.Common.OtherModels;
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace ManagementSystem.WPF;

public class GlobalFunctions
{
    private string _itemLastModified;
    private string _supplierLastModified;
    private string _accountLastModified;
    private string _projectTemplateLastModified;
    private ObservableRangeCollection<MstItem> _items = new();
    private ObservableRangeCollection<MstSupplier> _suppliers = new();
    private ObservableRangeCollection<MstAccount> _accounts = new();
    private ObservableRangeCollection<ProjectTemplate> _projectTemplates = new();
    private BitmapImage _imageError;

    public BitmapImage ImageError
    {
        get {
            if (_imageError == null)
            {
                _imageError = new BitmapImage();
                _imageError.BeginInit();
                // Load immediately to free the file
                _imageError.CacheOption = BitmapCacheOption.OnLoad;
                //_imageError.UriSource = new Uri("pack://application:,,,/Images/error.png");
                _imageError.UriSource = new Uri("pack://application:,,,/ManagementSystem.WPF.UI;component/Images/error.png", UriKind.Absolute);
                _imageError.EndInit();
            }
            return _imageError;
        }
    }
    public ObservableRangeCollection<MstItem> Items
    {
        get { return _items; }
    }

    public ObservableRangeCollection<MstSupplier> Suppliers
    {
        get { return _suppliers; }
    }

    public ObservableRangeCollection<MstAccount> Accounts
    {
        get { return _accounts; }
    }

    public ObservableRangeCollection<ProjectTemplate> ProjectTemplates
    {
        get { return _projectTemplates; }
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
        //if (!forceRefresh && Items.Count > 0)
        //{
        //    return;
        //}

        bool refreshData = false;
        var actionResult = await _apiClient.GetMstItemLastModifiedAsync();
        if (actionResult.ActionIsSuccess)
        {
            if (string.IsNullOrWhiteSpace(_itemLastModified))
            {
                refreshData = true;
            }
            else
            {
                string lastModified = (string)actionResult.ActionResult;
                if (Items.Count == 0 || string.Compare(lastModified, _itemLastModified, StringComparison.Ordinal) > 0)
                {
                    refreshData = true;
                }
            }
            _itemLastModified = (string) actionResult.ActionResult;
        }

        if (!refreshData || string.IsNullOrWhiteSpace(_itemLastModified))
        {
            return;
        }

        Items.Clear();
        var actionResultOfMstItem = await _apiClient.GetAllMstItemsAsync();
        if (actionResultOfMstItem.ActionIsSuccess)
        {
            Items.Clear(); // Clear 1x lagi karena Async bisa jadi form lain juga sedang call ini.
            Items.AddRange((IEnumerable<MstItem>)actionResultOfMstItem.ActionResult);

            await _apiClient.GetAllMstItemImagesAsync();
        }
        else
        {
            throw new Exception(
                string.Format(
                    TranslationSource.Instance["messageTitleLoadListData"] ?? string.Empty,
                    TranslationSource.Instance["mwItems"] ?? string.Empty));
        }

        return;
    }

    public async Task RefreshSupplierDataAsync(bool forceRefresh)
    {
        //if (!forceRefresh && Suppliers.Count > 0)
        //{
        //    return;
        //}

        bool refreshData = false;
        var actionResult = await _apiClient.GetMstSupplierLastModifiedAsync();
        if (actionResult.ActionIsSuccess)
        {
            if (string.IsNullOrWhiteSpace(_supplierLastModified))
            {
                refreshData = true;
            }
            else
            {
                string lastModified = (string)actionResult.ActionResult;
                if (Suppliers.Count == 0 || string.Compare(lastModified, _supplierLastModified, StringComparison.Ordinal) > 0)
                {
                    refreshData = true;
                }
            }
            _supplierLastModified = (string)actionResult.ActionResult;
        }

        if (!refreshData || string.IsNullOrWhiteSpace(_supplierLastModified))
        {
            return;
        }

        Suppliers.Clear();
        var actionResultOfMstSupplier = await _apiClient.GetMstSuppliersAsync();
        if (actionResultOfMstSupplier.ActionIsSuccess)
        {
            Suppliers.Clear(); // Clear 1x lagi karena Async bisa jadi form lain juga sedang call ini.
            Suppliers.AddRange((IEnumerable<MstSupplier>)actionResultOfMstSupplier.ActionResult);
        }
        else
        {
            throw new Exception(
                string.Format(
                    TranslationSource.Instance["messageTitleLoadListData"] ?? string.Empty,
                    TranslationSource.Instance["mwMstSupplier"] ?? string.Empty));
        }
    }

    public async Task RefreshAccountDataAsync(bool forceRefresh)
    {
        //if (!forceRefresh && Accounts.Count > 0)
        //{
        //    return;
        //}

        bool refreshData = false;
        var actionResult = await _apiClient.GetMstAccountLastModifiedAsync();
        if (actionResult.ActionIsSuccess)
        {
            if (string.IsNullOrWhiteSpace(_accountLastModified))
            {
                refreshData = true;
            }
            else
            {
                string lastModified = (string)actionResult.ActionResult;
                if (Accounts.Count == 0 || string.Compare(lastModified, _accountLastModified, StringComparison.Ordinal) > 0)
                {
                    refreshData = true;
                }
            }
            _accountLastModified = (string)actionResult.ActionResult;
        }

        if (!refreshData || string.IsNullOrWhiteSpace(_accountLastModified))
        {
            return;
        }

        Accounts.Clear();
        var actionResultOfMstAccount = await _apiClient.GetMstAccountsAsync();
        if (actionResultOfMstAccount.ActionIsSuccess)
        {
            Accounts.Clear(); // Clear 1x lagi karena Async bisa jadi form lain juga sedang call ini.
            Accounts.AddRange((IEnumerable<MstAccount>)actionResultOfMstAccount.ActionResult);
        }
        else
        {
            throw new Exception(
                string.Format(
                    TranslationSource.Instance["messageTitleLoadListData"] ?? string.Empty,
                    TranslationSource.Instance["mwMstAccount"] ?? string.Empty));
        }
    }

    public async Task RefreshProjectTemplateDataAsync(bool forceRefresh)
    {
        //if (!forceRefresh && ProjectTemplates.Count > 0)
        //{
        //    return;
        //}

        bool refreshData = false;
        var actionResult = await _apiClient.GetProjectTemplateLastModifiedAsync();
        if (actionResult.ActionIsSuccess)
        {
            if (string.IsNullOrWhiteSpace(_projectTemplateLastModified))
            {
                refreshData = true;
            }
            else
            {
                string ProjectTemplateLastModified = (string)actionResult.ActionResult;
                if (ProjectTemplates.Count == 0 || string.Compare(ProjectTemplateLastModified, _projectTemplateLastModified, StringComparison.Ordinal) > 0)
                {
                    refreshData = true;
                }
            }
            _projectTemplateLastModified = (string)actionResult.ActionResult;
        }

        if (!refreshData || string.IsNullOrWhiteSpace(_projectTemplateLastModified))
        {
            return;
        }

        ProjectTemplates.Clear();
        var actionResultOfMstProjectTemplate = await _apiClient.GetAllProjectTemplatesAsync();
        if (actionResultOfMstProjectTemplate.ActionIsSuccess)
        {
            ProjectTemplates.Clear(); // Clear 1x lagi karena Async bisa jadi form lain juga sedang call ini.
            ProjectTemplates.AddRange((IEnumerable<ProjectTemplate>)actionResultOfMstProjectTemplate.ActionResult);

            await _apiClient.GetAllProjectTemplateImagesAsync();
        }
        else
        {
            throw new Exception(
                string.Format(
                    TranslationSource.Instance["messageTitleLoadListData"] ?? string.Empty,
                    TranslationSource.Instance["mwProjectTemplates"] ?? string.Empty));
        }

        return;
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