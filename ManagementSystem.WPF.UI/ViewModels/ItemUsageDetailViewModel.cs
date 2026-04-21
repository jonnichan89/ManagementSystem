
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;

namespace ManagementSystem.WPF.UI.ViewModels;

public partial class ItemUsageDetailViewModel : DetailViewModel
{
    [ObservableProperty]
    public string Title { get; private set; }

    [ObservableProperty]
    public bool CanEditItemUsageDate
    {
        get
        {
            return _action == DetailAction.Add;
        }
    }

    private ItemUsageDetail? _selectedItemUsageDetail;

    public ItemUsageDetail? SelectedItemUsageDetail
    {
        get { return _selectedItemUsageDetail; }
        set
        {
            ResetCommandChecked();

            _selectedItemUsageDetail = value;
            if (_selectedItemUsageDetail != null && !string.IsNullOrEmpty(_selectedItemUsageDetail.ItemName))
            {
                FilterText = _selectedItemUsageDetail.ItemName;
            }
            else
            {
                FilterText = "";
            }

            OnPropertyChanged(nameof(SelectedItemUsageDetail));
        }
    }

    [ObservableProperty]
    private ItemUsage _data;

    #region Search Item

    string _filterText;
    Predicate<object> _filterPredicate;
    ObservableRangeCollection<MstItem> _items = new();

    protected internal MstItem? _selectedItem;

    [ObservableProperty]
    private string? _previewImagePath;

    [ObservableProperty]
    private ListCollectionView _filteredView;

    [ObservableProperty]
    private int _itemSelectedIndex;

    public MstItem? SelectedItem
    {
        get { return _selectedItem; }
        set
        {
            _selectedItem = value;

            if (_selectedItem != null)
            {
                FilterText = _selectedItem.ItemName;
                PreviewImagePath = _selectedItem.ImagePath;

                if (SelectedItemUsageDetail != null)
                {
                    SelectedItemUsageDetail.ItemCode = _selectedItem.ItemCode;
                    SelectedItemUsageDetail.ItemName = _selectedItem.ItemName;
                    OnPropertyChanged(nameof(SelectedItemUsageDetail));
                    Data.OnPropertyChanged(nameof(Data.ItemUsageDetails));
                }
            }

            OnPropertyChanged(nameof(SelectedItem));
        }
    }

    public string FilterText
    {
        get { return _filterText; }
        set
        {
            _filterText = value;

            if (_filterText.Length >= 3 &&
                _items.Count > 0 &&
                (_selectedItem == null || _selectedItem.ItemName != _filterText))
            {
                var foundItem = _items.FirstOrDefault(i => i.ItemName == value);

                if (foundItem == null)
                {
                    PreviewImagePath = null;

                    if (SelectedItemUsageDetail != null)
                    {
                        SelectedItemUsageDetail.ItemCode = string.Empty;
                        SelectedItemUsageDetail.ItemName = string.Empty;
                        OnPropertyChanged(nameof(SelectedItemUsageDetail));
                    }

                    using (FilteredView.DeferRefresh())
                    {
                        FilteredView.Filter = _filterPredicate;
                        FilteredView.SortDescriptions.Add(new SortDescription("Askterisk", ListSortDirection.Descending));
                        FilteredView.SortDescriptions.Add(new SortDescription("ItemName", ListSortDirection.Ascending));
                    }
                    FilteredView.MoveCurrentTo(FilteredView.GetItemAt(0));

                    _filterText = (FilteredView.GetItemAt(0) as MstItem)?.ItemName ?? string.Empty;
                }
                else
                {
                    PreviewImagePath = foundItem.ImagePath;

                    if (SelectedItemUsageDetail != null)
                    {
                        SelectedItemUsageDetail.ItemCode = foundItem.ItemCode;
                        SelectedItemUsageDetail.ItemName = foundItem.ItemName;
                        OnPropertyChanged(nameof(SelectedItemUsageDetail));
                        Data.OnPropertyChanged(nameof(Data.ItemUsageDetails));
                    }

                    ItemSelectedIndex = FilteredView.IndexOf(foundItem);
                    _filterText = foundItem.ItemName;
                }
                OnPropertyChanged(nameof(Data));
            }
        }
    }

    private async Task LoadSearchItemAsync()
    {
        await _gFunctions.RefreshItemDataAsync(false);
        _items.Clear();
        _items.AddRange(_gFunctions.Items.ToArray());

        foreach (var itemUsageDetail in Data.ItemUsageDetails)
        {
            itemUsageDetail.ItemName = _items.FirstOrDefault(i => i.ItemCode == itemUsageDetail.ItemCode)?.ItemName ?? string.Empty;
        }

        _filterPredicate = item =>
        {
            var item1 = (item as MstItem);
            item1.PutSearchToTop = (item1.SearchKey.Contains(_filterText, StringComparison.OrdinalIgnoreCase));
            return true;
        };

        FilteredView = new ListCollectionView(_items);
    }

    #endregion

    IContentDialogService _contentDialogService;
    IViewInteractionService _viewInteractionService;
    DetailViewModelDoAction _promptAction;
    IManagementApiClient _apiClient;
    JsonSerializerOptions _jsonOptions;
    private GlobalFunctions _gFunctions;

    public ItemUsageDetailViewModel(
        IManagementApiClient apiClient,
        IContentDialogService contentDialogService,
        IViewInteractionService viewInteractionService,
        JsonSerializerOptions jsonOptions,
        GlobalFunctions gFunctions)
    {
        _apiClient = apiClient;
        _contentDialogService = contentDialogService;
        _viewInteractionService = viewInteractionService;
        _viewInteractionService.PromptFeedback += _viewInteractionService_PromptFeedback;
        _jsonOptions = jsonOptions;

        ChangeLanguage();
        _gFunctions = gFunctions;
    }

    public override async Task InitializeAsync(DetailAction action, object pk)
    {
        int itemUsageId = (int)pk;

        base._action = action;

        bool loadSearchItems = false;

        switch (action)
        {
            case DetailAction.ViewDetail:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleView"] ?? string.Empty,
                    TranslationSource.Instance["mwItemUsage"]);
                break;

            case DetailAction.Add:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleAdd"] ?? string.Empty,
                    TranslationSource.Instance["mwItemUsage"]);
                Data = new ItemUsage()
                {
                    ItemUsageDate = DateTime.Today,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "",
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = ""
                };
                Data.AddItemUsageDetail();

                loadSearchItems = true;

                break;

            case DetailAction.Edit:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleEdit"] ?? string.Empty,
                    TranslationSource.Instance["mwItemUsage"]);

                ServiceActionResult sar = await _apiClient.GetItemUsageByIdAsync(itemUsageId);
                if (sar.ActionIsSuccess)
                {
                    Data = (ItemUsage)sar.ActionResult;
                }

                loadSearchItems = true;

                break;

            default:
                break;
        }

        if (loadSearchItems)
        {
            await LoadSearchItemAsync();
        }

        if (Data?.ItemUsageDetails.Count > 0)
        {
            foreach (var detail in Data.ItemUsageDetails)
            {
                if (detail.ItemName != "")
                    detail.ItemName = (_items.First(i => i.ItemCode == detail.ItemCode))?.ItemName ?? string.Empty;
            }

            SelectedItemUsageDetail = Data.ItemUsageDetails[0];
        }
    }

    public override void Dispose()
    {
        _viewInteractionService.PromptFeedback -= _viewInteractionService_PromptFeedback;
        base.Dispose();
    }

    private async void _viewInteractionService_PromptFeedback(object? sender, EventArgs e)
    {
        switch (_promptAction)
        {
            case DetailViewModelDoAction.SavePrimary:
                if (e is MyContentDialogButtonClickEventArgs args)
                {
                    if (args.Button == ContentDialogButton.Primary)
                    {
                        await SavePrimaryAsync();
                    }
                }
                break;

            case DetailViewModelDoAction.SavePrimarySuccess:
                _viewInteractionService.Close(ContentDialogResult.Primary);
                break;

            default:
                break;
        }
    }

    internal override void OnActionMessageReceived(ActionMessage m)
    {
        switch (m.Name)
        {
            case ActionMessage.ActionName.ChangeLanguage:
                ChangeLanguage();
                break;

            default:
                break;
        }
    }

    private void ChangeLanguage()
    {
    }

    public override void TrySavePrimary()
    {
        _promptAction = DetailViewModelDoAction.SavePrimary;

        bool absoluteCannotSave = false;
        string message = "";

        int i = 0;
        foreach (var detail in Data.ItemUsageDetails)
        {
            if (string.IsNullOrWhiteSpace(detail.ItemCode))
            {
                message += $@"[{i}] " +
                    string.Format(
                        TranslationSource.Instance["messageFieldIsRequired"] ?? string.Empty,
                        TranslationSource.Instance["propItemCode"]) +
                    Environment.NewLine;

                absoluteCannotSave = true;
            }

            if (detail.Qty == 0)
            {
                message += $@"[{i}] " +
                    string.Format(
                        TranslationSource.Instance["messageFieldIsRequired"] ?? string.Empty,
                        TranslationSource.Instance["propItemUsageQty"] + " (" + detail.ItemCode + ")") +
                    Environment.NewLine;
                absoluteCannotSave = true;
            }
        }

        if (absoluteCannotSave)
        {
            _viewInteractionService.Prompt(
                PromptUsedFor.MessageAlert,
                string.Format(TranslationSource.Instance["messageTitleCouldNotSave"] ?? string.Empty, Title),
                message);
        }
        else if (string.IsNullOrWhiteSpace(message))
        {
            _viewInteractionService.Prompt(
                PromptUsedFor.QuestionBeforeSaveDetail,
                string.Format(TranslationSource.Instance["messageTitleQuestionBeforeSaveDetail"] ?? string.Empty, Title),
                string.Format(TranslationSource.Instance["messageQuestionBeforeSaveDetail"] ?? string.Empty, Title) + Environment.NewLine +
                    message);
        }
    }

    public override async Task<bool> SavePrimaryAsync()
    {
        try
        {
            ServiceActionResult result;
            switch (_action)
            {
                case DetailAction.Add:
                    result = await _apiClient.CreateItemUsageAsync(_data);
                    break;

                case DetailAction.Edit:
                    result = await _apiClient.UpdateItemUsageAsync(_data);
                    break;

                default:
                    return false;
            }

            if (!result.ActionIsSuccess)
            {
                string message = string.Empty;

                foreach (ModelValidation mv in result.ModelValidations)
                {
                    switch (mv.ErrorType)
                    {
                        case ModelValidationType.FieldIsRequired:
                            message += Environment.NewLine +
                                string.Format(
                                    TranslationSource.Instance["messageFieldIsRequired"] ?? string.Empty,
                                    TranslationSource.Instance["prop" + mv.PropertyName]);

                            break;

                        case ModelValidationType.PKAlreadyExists:
                            message += Environment.NewLine +
                                string.Format(
                                    TranslationSource.Instance["messagePKAlreadyExists"] ?? string.Empty,
                                    _data.ItemUsageDate);
                            break;

                        case ModelValidationType.DuplicateRecord:
                            message += Environment.NewLine +
                                string.Format(
                                    TranslationSource.Instance["messageDuplicateRecord"] ?? string.Empty,
                                    _data.ItemUsageDate);
                            break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    message += Environment.NewLine +
                        string.Format(
                            TranslationSource.Instance["messageTechnicalError"] ?? string.Empty,
                            Environment.NewLine + result.ErrorMessage);

                _viewInteractionService.Prompt(
                    PromptUsedFor.MessageDetailSaveFailed,
                    string.Format(TranslationSource.Instance["messageTitleSaveFailed"] ?? string.Empty, Title),
                    message
                );
            }
            else
            {
                _promptAction = DetailViewModelDoAction.SavePrimarySuccess;

                _viewInteractionService.Prompt(
                    PromptUsedFor.MessageDetailSaveSuccess,
                    string.Format(TranslationSource.Instance["messageTitleSaveSucceed"] ?? string.Empty, Title),
                    string.Format(TranslationSource.Instance["messageSaveDetailSucceed"] ?? string.Empty, Title)
                );
            }
        }
        catch (Exception ex)
        {
            _viewInteractionService.Prompt(
                PromptUsedFor.MessageDetailSaveFailed,
                string.Format(TranslationSource.Instance["messageTitleSaveFailed"] ?? string.Empty, Title),
                string.Format(TranslationSource.Instance["messageSaveDetailFailed"] ?? string.Empty, Title, ExceptionHelper.GetErrorMessage(ex, true))
            );
        }
        return true;
    }

    public override void TrySaveSecondary()
    {
        throw new NotImplementedException();
    }
    public override bool SaveSecondary()
    {
        return base.SaveSecondary();
    }
    public override void TryClose()
    {
        _viewInteractionService.Close(ContentDialogResult.None);
    }
    public override bool Close()
    {
        return true;
    }

    #region Detail

    private DetailAction _itemUsageDetailAction;

    [ObservableProperty]
    private bool _AddCommandChecked;

    private void ResetCommandChecked()
    {
        AddCommandChecked = false;
    }

    [RelayCommand]
    private void AddItemUsageDetail()
    {
        ResetCommandChecked();

        _itemUsageDetailAction = DetailAction.Add;
        AddCommandChecked = true;

        Data.AddItemUsageDetail();
        SelectedItemUsageDetail = Data.ItemUsageDetails.LastOrDefault();
    }

    [RelayCommand]
    private void EditItemUsageDetail()
    {
        ResetCommandChecked();

        if (SelectedItemUsageDetail != null)
        {
            _itemUsageDetailAction = DetailAction.Edit;
        }

    }

    [RelayCommand]
    private void DeleteItemUsageDetail()
    {
        ResetCommandChecked();

        if (SelectedItemUsageDetail != null)
        {
            Data.ItemUsageDetails.Remove(SelectedItemUsageDetail);
            SelectedItemUsageDetail = Data.ItemUsageDetails.FirstOrDefault();
        }
    }

    //[RelayCommand]
    //private void SaveItemUsageDetail()
    //{
    //    if (_itemUsageDetailAction == DetailAction.Add)
    //    {
    //        Data.ItemUsageDetails.Add(new ItemUsageDetail()
    //        {
    //            ItemName = DataDetail.ItemName,
    //            ItemCode = DataDetail.ItemCode,
    //            Qty = DataDetail.Qty,
    //            CreatedAt = DateTime.Now,
    //            CreatedBy = "",
    //            ModifiedAt = DateTime.Now,
    //            ModifiedBy = ""
    //        });
    //        OnPropertyChanged(nameof(Data));
    //        Data.OnPropertyChanged(nameof(Data.ItemUsageDetails));

    //        SelectedItemUsageDetail = Data.ItemUsageDetails.LastOrDefault();
    //        return;
    //    }
    //    else if (_itemUsageDetailAction == DetailAction.Edit)
    //    {
    //        if (SelectedItemUsageDetail != null)
    //        {
    //            SelectedItemUsageDetail.ItemName = DataDetail.ItemName;
    //            SelectedItemUsageDetail.ItemCode = DataDetail.ItemCode;
    //            SelectedItemUsageDetail.Qty = DataDetail.Qty;
    //            OnPropertyChanged(nameof(Data));
    //        }
    //    }

    //    ResetCommandChecked();
    //}

    #endregion
}
