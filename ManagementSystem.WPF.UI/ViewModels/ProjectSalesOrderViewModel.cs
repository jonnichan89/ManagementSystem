using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using ManagementSystem.WPF.UI.ViewModels.ToInherit;

namespace ManagementSystem.WPF.UI.ViewModels;

public partial class ProjectSalesOrderViewModel : BaseListViewModel<PSOrder>
{
    private string _title = TranslationSource.Instance["mwProjectSalesOrder"] ?? "Project Sales Order";

    //private PSOrder[]? _items;

    [ObservableProperty]
    private ObservableCollection<PSOrderDetail>? _PSDetails = null;

    //private PSOrder _selectedItem;
    //public PSOrder SelectedItem
    //{
    //    get => _selectedItem;
    //    set
    //    {
    //        SetProperty(ref _selectedItem, value);
    //        if (_selectedItem != null)
    //        {
    //            PSDetails = _selectedItem.PSOrderDetails;
    //        }
    //    }
    //}

    protected override void AfterSelectedItemChanged()
    {
        if (_selectedItem != null)
        {
            PSDetails = _selectedItem.PSOrderDetails;
        }
    }

    private DetailViewModelDoAction _promptAction = DetailViewModelDoAction.None;
    private ListViewModelDoAction _listAction = ListViewModelDoAction.None;

    private IManagementApiClient _apiClient;
    private IContentDialogService _contentDialogService;
    //private IViewInteractionService _viewInteractionService;
    private JsonSerializerOptions _jsonSerializerOptions;
    private GlobalFunctions _gFunctions;

    public ProjectSalesOrderViewModel(
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
        _title = TranslationSource.Instance["mwProjectSalesOrder"] ?? "Project Sales Order";
        _previewItems.Clear();
        await RefreshListDataAsync();

        if (_previewItems.Count > 0)
        {
            if (SelectedItem == null)
            {
                SelectedItem = _previewItems[0];
            }
            else
            {
                int lastPSOrderId = SelectedItem?.PSId ?? 0;
                SelectedItem = _previewItems.FirstOrDefault(iu => iu.PSId == lastPSOrderId) ?? _previewItems[0];
            }
        }

        await _gFunctions.RefreshItemDataAsync(false);
    }

    #region Paging

    //[ObservableProperty]
    //private ObservableRangeCollection<PageIndex> _pageList = new();

    //[ObservableProperty]
    //private ObservableRangeCollection<PSOrder> _previewItems = new();

    //private int _itemPerPage = 50;

    private async Task RefreshListDataAsync()
    {
        var actionResult = await _apiClient.GetProjectSalesOrdersAsync();
        if (actionResult.ActionIsSuccess)
        {
            _items = (PSOrder[])actionResult.ActionResult;
            RefreshPreviewItems(null);
        }
        else
        {
            await _contentDialogService.ShowAlertAsync(
                string.Format(
                    TranslationSource.Instance["messageTitleLoadListData"] ?? string.Empty,
                    _title),
                actionResult.ErrorMessage,
                TranslationSource.Instance["dialogButtonOK"] ?? string.Empty);
        }
    }

    //[RelayCommand]
    //private void RefreshPreviewItems(PageIndex? page)
    //{
    //    _previewItems.Clear();

    //    int pageIndex = RefreshPageList(page, page?.ForceUpdatePageList ?? true)?.PageValue ?? 1;
    //    _previewItems.AddRange(_items.Skip((pageIndex - 1) * _itemPerPage).Take(_itemPerPage));

    //}

    //private PageIndex? RefreshPageList(PageIndex? currentPage, bool forceUpdateList)
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
    //        int maxPage = (int)Math.Ceiling((decimal)_items.Length / _itemPerPage);

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
        ProjectSalesOrderDetailPage? detailPage;
        try
        {
            detailPage = ManagementSystem.WPF.UI.App.GetRequiredService<ProjectSalesOrderDetailPage>();
            await detailPage.InitializeAsync(DetailAction.Add, 0);
        }
        catch
        {
            detailPage = new ProjectSalesOrderDetailPage(_apiClient, _contentDialogService, _jsonSerializerOptions, _gFunctions);
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
            SelectedItem = null;
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
        if (SelectedItem == null || SelectedItem.PSId == 0)
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
        ProjectSalesOrderDetailPage? detailPage;
        try
        {
            detailPage = ManagementSystem.WPF.UI.App.GetRequiredService<ProjectSalesOrderDetailPage>();
            await detailPage.InitializeAsync(DetailAction.Edit, SelectedItem.PSId);
        }
        catch
        {
            detailPage = new ProjectSalesOrderDetailPage(_apiClient, _contentDialogService, _jsonSerializerOptions, _gFunctions);
            await detailPage.InitializeAsync(DetailAction.Edit, SelectedItem.PSId);
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
        if (SelectedItem.PSId == 0)
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
                Content = string.Format(TranslationSource.Instance["messageQuestionBeforeDelete"] ?? string.Empty, _title + " " + SelectedItem.PSCode),
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
        ServiceActionResult actionResult = await _apiClient.DeleteProjectSalesOrderAsync(SelectedItem.PSId);
        if (actionResult.ActionIsSuccess)
        {
            ContentDialogResult answer = await _contentDialogService.ShowSimpleDialogAsync(
               new SimpleContentDialogCreateOptions()
               {
                   Title = string.Format(TranslationSource.Instance["messageTitleDelete"] ?? string.Empty, _title),
                   Content = string.Format(TranslationSource.Instance["messageDeleteSucceed"] ?? string.Empty, _title + " " + SelectedItem.PSCode),
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
                    Content = string.Format(TranslationSource.Instance["messageDeleteFailed"] ?? string.Empty, _title + " " + SelectedItem.PSCode) + Environment.NewLine +
                        actionResult.ErrorMessage,
                    CloseButtonText = TranslationSource.Instance["dialogButtonOK"] ?? string.Empty
                });
        }
    }

}
