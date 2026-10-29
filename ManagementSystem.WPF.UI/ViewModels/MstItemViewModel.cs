
using ManagementSystem.Common.OtherModels;
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using ManagementSystem.WPF.UI.ViewModels.ToInherit;

namespace ManagementSystem.WPF.UI.ViewModels;

public partial class MstItemViewModel : BaseListViewModel<MstItem>
{
    private string _title = TranslationSource.Instance["mwMstItem"] ?? "Item";

    //ObservableRangeCollection<MstItem> _items = new();

    //private MstItem _selectedItem = new();
    //public MstItem SelectedItem
    //{
    //    get => _selectedItem;
    //    set
    //    {
    //        _selectedItem = value;
    //        if (_selectedItem != null)
    //        {
    //            _ = GetAllItemCalcsAsync(_selectedItem.ItemCode);
    //        }
    //        OnPropertyChanged(nameof(SelectedItem));
    //    }
    //}

    protected override void AfterSelectedItemChanged()
    {
        if (_selectedItem != null)
        {
            _ = GetAllItemCalcsAsync(_selectedItem.ItemCode);
        }
    }

    private async Task GetAllItemCalcsAsync(string itemCode)
    {
        var actionResult = await _apiClient.GetAllItemCalcsAsync(_selectedItem.ItemCode);
        if (actionResult.ActionIsSuccess)
        {
            _selectedItem.ItemCalcs = ((ItemCalc[])actionResult.ActionResult).ToList();
            _previewItemCalcs.Clear();
            _previewItemCalcs.AddRange(_selectedItem.ItemCalcs.OrderByDescending(p => p.CalcDate));
        }
    }

    private string _PreviewMode = ManagementSystem.WPF.UI.Properties.Settings.Default.MstItemPreviewMode;

    public string PreviewMode
    {
        get => _PreviewMode;
        set
        {
            ManagementSystem.WPF.UI.Properties.Settings.Default.MstItemPreviewMode = value;
            ManagementSystem.WPF.UI.Properties.Settings.Default.Save();

            SetProperty(ref _PreviewMode, value);

            switch (_PreviewMode)
            {
                case "List":
                    PreviewModeListImageMode = "Collapsed";
                    break;
                case "Image":
                    PreviewModeListImageMode = "Collapsed";
                    break;
                default:
                    PreviewModeListImageMode = "Collapsed";
                    break;
            }
        }
    }

    private string _PreviewModeListImageMode = ManagementSystem.WPF.UI.Properties.Settings.Default.MstItemPreviewListImageMode;

    public string PreviewModeListImageMode
    {
        get => _PreviewModeListImageMode;
        set
        {
            ManagementSystem.WPF.UI.Properties.Settings.Default.MstItemPreviewListImageMode = value;
            ManagementSystem.WPF.UI.Properties.Settings.Default.Save();

            SetProperty(ref _PreviewModeListImageMode, value);

            switch (_PreviewModeListImageMode)
            {
                case "Collapsed":
                    RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                    break;
                case "Visible":
                    RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
                    break;
                case "VisibleWhenSelected":
                    RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                    break;
                default:
                    RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                    break;
            }

        }
    }

    [ObservableProperty]
    private DataGridRowDetailsVisibilityMode _RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
    //(DataGridRowDetailsVisibilityMode)ManagementSystem.WPF.UI.Properties.Settings.Default.MstItemPreviewListImageMode;

    private DetailViewModelDoAction _promptAction = DetailViewModelDoAction.None;
    private ListViewModelDoAction _listAction = ListViewModelDoAction.None;

    private IManagementApiClient _apiClient;
    private IContentDialogService _contentDialogService;
    //private IViewInteractionService _viewInteractionService;
    private JsonSerializerOptions _jsonSerializerOptions;
    private GlobalFunctions _gFunctions;

    public MstItemViewModel(
        IManagementApiClient apiClient,
        IContentDialogService contentDialogService,
        //IViewInteractionService viewInteractionService,
        JsonSerializerOptions jsonSerializerOptions,
        GlobalFunctions gFunctions)
        : base()
    {
        _apiClient = apiClient;
        _contentDialogService = contentDialogService;
        //_viewInteractionService = viewInteractionService;
        //_viewInteractionService.PromptFeedback += _viewInteractionService_PromptFeedback;

        _jsonSerializerOptions = jsonSerializerOptions;
        _gFunctions = gFunctions;
    }

    public override void Dispose()
    {
        //_viewInteractionService.PromptFeedback -= _viewInteractionService_PromptFeedback;
        base.Dispose();
    }

    public async Task Initialize()
    {
        _title = TranslationSource.Instance["mwMstItem"] ?? "Item";
        _previewItems.Clear();
        SelectedItem = null;
        _previewItemCalcs.Clear();
        await RefreshListDataAsync(true);
    }

    #region Paging

    //[ObservableProperty]
    //private ObservableRangeCollection<PageIndex> _pageList = new();

    //[ObservableProperty]
    //private ObservableRangeCollection<MstItem> _previewItems = new();

    [ObservableProperty]
    private ObservableRangeCollection<ItemCalc> _previewItemCalcs = new();

    //private int _currentPage = 1;
    private PageFilterMstItem _pageFilter = new PageFilterMstItem();
    private async Task RefreshListDataAsync(bool isFirstLoad = false)
    {
        try
        {
            await _gFunctions.RefreshItemDataAsync(false);
            _items = _gFunctions.Items.ToArray();
            RefreshPreviewItems(null);
        }
        catch (Exception ex)
        {
            await _contentDialogService.ShowAlertAsync(
                string.Format(
                    TranslationSource.Instance["messageTitleLoadListData"] ?? string.Empty,
                    _title),
                ex.Message,
                TranslationSource.Instance["dialogButtonOK"] ?? string.Empty);
        }
    }

    //[RelayCommand]
    //private async Task RefreshPreviewItems(PageIndex? page)
    //{
    //    _previewItems.Clear();

    //    if (page == null)
    //    {
    //        page = new PageIndex(_currentPage, (_pageList.Count > 0 ? false : true));
    //    }

    //    int pageIndex = (page.PageValue == 0 ? page.PageListStartFrom : page.PageValue);
        
    //    _previewItems.AddRange(_items.Skip((pageIndex - 1) * _pageFilter.RecordPerPage).Take(_pageFilter.RecordPerPage));
        
    //    SelectedItem = _previewItems.FirstOrDefault();

    //    PageIndex currentPageIndex = RefreshPageList(page, page.ForceUpdatePageList);

    //    // int pageIndex = currentPageIndex?.PageValue ?? 1;
    //    _currentPage = pageIndex;

    //}

    //private PageIndex RefreshPageList(PageIndex? currentPage, bool forceUpdateList)
    //{
    //    if (currentPage == null)
    //    {
    //        currentPage = new PageIndex(1, false);
    //        forceUpdateList = true;
    //    }

    //    PageIndex returnPage = null;
    //    bool updateList = forceUpdateList;
    //    if (!forceUpdateList)
    //    {
    //        updateList = currentPage.IsPreviousPageList || currentPage.IsNextPageList;
    //    }

    //    if (updateList)
    //    {
    //        //int recordCount = 0;
    //        //ServiceActionResult apiResult = await _apiClient.GetMstItemCountAsync();
    //        //if (apiResult.ActionIsSuccess)
    //        //{
    //        //    recordCount = (int)apiResult.ActionResult;
    //        //}

    //        int recordCount = _items.Count;
    //        int maxPage = (int)Math.Ceiling((decimal)recordCount / _pageFilter.RecordPerPage);

    //        int prevPageIndex = _pageList.FirstOrDefault(p => p.IsSelected)?.PageValue ?? 1;
    //        int pageIndex = currentPage.PageValue;

    //        int fromPageIndex = _pageList.Count > 0 ? _pageList.Where(p => p.PageValue > 0).Min(p => p.PageValue) : 1;
    //        int toPageIndex = fromPageIndex - 1 + 10 > maxPage ? maxPage : fromPageIndex - 1 + 10;

    //        if (forceUpdateList)
    //        {
    //            fromPageIndex = pageIndex == 1 ?
    //                1 :
    //                pageIndex == maxPage ?
    //                (((int)Math.Ceiling((maxPage / 10.0)) - 1) * 10) + 1 :
    //                (((int)Math.Floor((maxPage / 10.0)) - 1) * 10) + 1;
    //        }
    //        else if (currentPage.IsPreviousPageList)
    //        {
    //            fromPageIndex = currentPage.PageListStartFrom;
    //        }
    //        else if (currentPage.IsNextPageList)
    //        {
    //            fromPageIndex = currentPage.PageListStartFrom;
    //        }

    //        toPageIndex = (fromPageIndex + 10 - 1 > maxPage ? maxPage : fromPageIndex + 10 - 1);

    //        _pageList.Clear();

    //        bool dotPrevious = false;
    //        bool dotNext = false;
    //        for (int i = 1; i <= maxPage; i++)
    //        {
    //            if (i == 1)
    //                _pageList.Add(new PageIndex(i, !(i >= fromPageIndex && i <= toPageIndex)));

    //            else if (i >= fromPageIndex - 10 && i < fromPageIndex)
    //            {
    //                if (dotPrevious) continue;

    //                _pageList.Add(new PageIndex(true, false, fromPageIndex - 10));
    //                dotPrevious = true;
    //            }

    //            else if (i >= fromPageIndex && i <= toPageIndex)
    //                _pageList.Add(new PageIndex(i, false));

    //            else if (i > toPageIndex && i < maxPage)
    //            {
    //                if (dotNext) continue;

    //                _pageList.Add(new PageIndex(false, true, i));
    //                dotNext = true;
    //            }

    //            else if (i == maxPage)
    //                _pageList.Add(new PageIndex(i, !(i >= fromPageIndex && i <= toPageIndex)));

    //            if (i == fromPageIndex)
    //            {
    //                returnPage = _pageList.Last();
    //                returnPage.IsSelected = true;
    //            }
    //        }

    //    }
    //    else
    //    {
    //        int currentPageIndex = currentPage.PageValue;
    //        foreach (var pageItem in PageList)
    //        {
    //            pageItem.IsSelected = (pageItem.PageValue == currentPageIndex);
    //        }

    //        returnPage = _pageList.First(p => p.IsSelected);
    //    }

    //    return returnPage;
    //}

    #endregion

    [RelayCommand]
    private async Task Add()
    {
        // Resolve page from command parameter (XAML StaticResource) if provided,
        // otherwise try to get it from DI (App.GetRequiredService) or fallback to new instance.
        MstItemDetailPage? detailPage;
        try
        {
            detailPage = ManagementSystem.WPF.UI.App.GetRequiredService<MstItemDetailPage>();
            await detailPage.InitializeAsync(DetailAction.Add, 0);
        }
        catch
        {
            detailPage = new MstItemDetailPage(_apiClient, _contentDialogService, _jsonSerializerOptions, _gFunctions);
            await detailPage.InitializeAsync(DetailAction.Add, 0);
        }

        detailPage.Frame = new Frame
        {
            NavigationUIVisibility = NavigationUIVisibility.Hidden,
        };

        // Use Navigate so the Frame hosts the Page correctly
        detailPage.Frame.Navigate(detailPage);

        CancellationTokenSource cts = new CancellationTokenSource();
        DetailContentDialog dcd = new DetailContentDialog(detailPage.VM)
        {
            Content = detailPage.Frame,
            Title = string.Format(
                TranslationSource.Instance["detailTitleAdd"] ?? string.Empty,
                _title),
            CloseButtonText = TranslationSource.Instance["dialogButtonCancel"] ?? string.Empty,
            PrimaryButtonText = TranslationSource.Instance["dialogButtonSave"] ?? string.Empty
        };

        ContentDialogResult dialogResult = await _contentDialogService.ShowAsync(
            dcd, cts.Token
        );

        if (dialogResult == ContentDialogResult.Primary)
        {
            await RefreshListDataAsync();
        }

        if (detailPage is IDisposable disposablePage)
        {
            disposablePage.Dispose();
        }

        detailPage = null;
        dcd = null;
    }

    [RelayCommand]
    private async Task Edit()
    {
        if (SelectedItem == null || SelectedItem.ItemId == 0)
        {
            await _contentDialogService.ShowAlertAsync(
                string.Format(
                    TranslationSource.Instance["messageTitleSeletedDataNullWhenEdit"] ?? string.Empty,
                    _title),
                string.Format(
                    TranslationSource.Instance["messageSeletedDataNullWhenEdit"] ?? string.Empty,
                    _title),
                TranslationSource.Instance["dialogButtonOK"] ?? string.Empty);

            return;
        }

        // Resolve page from command parameter (XAML StaticResource) if provided,
        // otherwise try to get it from DI (App.GetRequiredService) or fallback to new instance.
        MstItemDetailPage? detailPage;
        try
        {
            detailPage = ManagementSystem.WPF.UI.App.GetRequiredService<MstItemDetailPage>();
            await detailPage.InitializeAsync(DetailAction.Edit, SelectedItem.ItemId);
        }
        catch
        {
            detailPage = new MstItemDetailPage(_apiClient, _contentDialogService, _jsonSerializerOptions, _gFunctions);
            await detailPage.InitializeAsync(DetailAction.Edit, SelectedItem.ItemId);
        }

        detailPage.Frame = new Frame
        {
            NavigationUIVisibility = NavigationUIVisibility.Hidden,
        };

        // Use Navigate so the Frame hosts the Page correctly
        detailPage.Frame.Navigate(detailPage);

        CancellationTokenSource cts = new CancellationTokenSource();
        DetailContentDialog dcd = new DetailContentDialog(detailPage.VM)
        {
            Content = detailPage.Frame,
            Title = string.Format(
                TranslationSource.Instance["detailTitleEdit"] ?? string.Empty,
                _title),
            CloseButtonText = TranslationSource.Instance["dialogButtonCancel"] ?? string.Empty,
            PrimaryButtonText = TranslationSource.Instance["dialogButtonSave"] ?? string.Empty
        };

        ContentDialogResult dialogResult = await _contentDialogService.ShowAsync(
            dcd, cts.Token
        );

        if (dialogResult == ContentDialogResult.Primary)
        {
            await RefreshListDataAsync();
        }

        if (detailPage is IDisposable disposablePage)
        {
            disposablePage.Dispose();
        }

        detailPage = null;
        dcd = null;
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (SelectedItem == null || SelectedItem.ItemId == 0)
        {
            await _contentDialogService.ShowAlertAsync(
                string.Format(
                    TranslationSource.Instance["messageTitleSeletedDataNullWhenDelete"] ?? string.Empty,
                    _title),
                string.Format(
                    TranslationSource.Instance["messageSeletedDataNullWhenDelete"] ?? string.Empty,
                    _title),
                TranslationSource.Instance["dialogButtonOK"] ?? string.Empty);

            return;
        }

        _promptAction = DetailViewModelDoAction.SavePrimary;
        _listAction = ListViewModelDoAction.Delete;

        ContentDialogResult answer = await _contentDialogService.ShowSimpleDialogAsync(
            new SimpleContentDialogCreateOptions()
            {
                Title = string.Format(TranslationSource.Instance["messageTitleQuestionBeforeDelete"] ?? string.Empty, _title),
                Content = string.Format(TranslationSource.Instance["messageQuestionBeforeDelete"] ?? string.Empty, _title + " " + SelectedItem.ItemCode),
                PrimaryButtonText = TranslationSource.Instance["dialogButtonYes"] ?? string.Empty,
                CloseButtonText = TranslationSource.Instance["dialogButtonNo"] ?? string.Empty
            });

        if (answer == ContentDialogResult.Primary)
        {
            await DeleteAsync();
        }
    }

    private async Task DeleteAsync()
    {
        ServiceActionResult actionResult = await _apiClient.DeleteMstItemAsync(SelectedItem.ItemId);
        if (actionResult.ActionIsSuccess)
        {
            _gFunctions.Items.Clear();

            ContentDialogResult answer = await _contentDialogService.ShowSimpleDialogAsync(
               new SimpleContentDialogCreateOptions()
               {
                   Title = string.Format(TranslationSource.Instance["messageTitleSaveSucceed"] ?? string.Empty, _title),
                   Content = string.Format(TranslationSource.Instance["messageSaveDetailSucceed"] ?? string.Empty, _title + " " + SelectedItem.ItemCode),
                   CloseButtonText = TranslationSource.Instance["dialogButtonOK"] ?? string.Empty
               });

            await RefreshListDataAsync();
        }
        else
        {
            await _contentDialogService.ShowSimpleDialogAsync(
                new SimpleContentDialogCreateOptions()
                {
                    Title = string.Format(TranslationSource.Instance["messageTitleDeleteFailed"] ?? string.Empty, _title),
                    Content = string.Format(TranslationSource.Instance["messageDeleteFailed"] ?? string.Empty, _title + " " + SelectedItem.ItemCode) + Environment.NewLine +
                        actionResult.ErrorMessage,
                    CloseButtonText = TranslationSource.Instance["dialogButtonOK"] ?? string.Empty
                });
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await RefreshListDataAsync(false);
    }

}