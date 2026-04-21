
using ManagementSystem.WPF.UI.Data;
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using ManagementSystem.WPF.UI.ViewModels.ToInherit;

namespace ManagementSystem.WPF.UI.ViewModels;

public partial class PurchaseOrderDetailViewModel : DetailViewModel
{
    [ObservableProperty]
    public string Title { get; private set; }

    [ObservableProperty]
    public bool CanEditPSDate
    {
        get
        {
            return _action == DetailAction.Add;
        }
    }

    private PSOrderDetail? _selectedItemDetail;

    public PSOrderDetail? SelectedItemDetail
    {
        get { return _selectedItemDetail; }
        set
        {
            ResetCommandChecked();

            _selectedItemDetail = value;
            if (_selectedItemDetail != null && !string.IsNullOrEmpty(_selectedItemDetail.ItemName))
            {
                FilterText = _selectedItemDetail.ItemName;
            }
            else
            {
                FilterText = "";
            }

            OnPropertyChanged(nameof(SelectedItemDetail));
        }
    }

    [ObservableProperty]
    private PSOrder _data;

    #region Initialize Supplier Combo Box

    private InstantSupplierComboBoxVM _supplierCBVM;
    public InstantSupplierComboBoxVM SupplierCBVM
    {
        get
        {
            return _supplierCBVM;
        }
    }

    private void InitializeSupplierCBVM()
    {
        _supplierCBVM = new InstantSupplierComboBoxVM(_gFunctions);
        _supplierCBVM.SupplierChanged = _supplierCBVM_SupplierChanged;
        _supplierCBVM.GetSupplierCode = _supplierCBVM_GetSupplierCode;
        _supplierCBVM.SetSupplierName = _supplierCBVM_SetSupplierName;
    }

    private void _supplierCBVM_SupplierChanged(MstSupplier? selectedSupplier)
    {
        if (_data == null) return;

        if (selectedSupplier != null)
        {
            _data.SupplierCode = selectedSupplier.SupplierCode;
            _data.SupplierName = selectedSupplier.SupplierName;
        }
        else
        {
            _data.SupplierCode = string.Empty;
            _data.SupplierName = string.Empty;
        }
    }

    private string _supplierCBVM_GetSupplierCode()
    {
        if (_data == null) return "";

        return _data.SupplierCode;
    }

    private void _supplierCBVM_SetSupplierName(string supplierName)
    {
        if (_data == null) return;

        _data.SupplierName = supplierName;
    }

    #endregion

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

                if (SelectedItemDetail != null)
                {
                    SelectedItemDetail.ItemCode = _selectedItem.ItemCode;
                    SelectedItemDetail.ItemName = _selectedItem.ItemName;
                    OnPropertyChanged(nameof(SelectedItemDetail));
                    Data.OnPropertyChanged(nameof(Data.PSOrderDetails));
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

                    if (SelectedItemDetail != null)
                    {
                        SelectedItemDetail.ItemCode = string.Empty;
                        SelectedItemDetail.ItemName = string.Empty;
                        OnPropertyChanged(nameof(SelectedItemDetail));
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

                    if (SelectedItemDetail != null)
                    {
                        SelectedItemDetail.ItemCode = foundItem.ItemCode;
                        SelectedItemDetail.ItemName = foundItem.ItemName;
                        OnPropertyChanged(nameof(SelectedItemDetail));
                        Data.OnPropertyChanged(nameof(Data.PSOrderDetails));
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

        if (_data == null) return;

        foreach (var PSOrderDetail in Data.PSOrderDetails)
        {
            PSOrderDetail.ItemName = _items.FirstOrDefault(i => i.ItemCode == PSOrderDetail.ItemCode)?.ItemName ?? string.Empty;
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

    public PurchaseOrderDetailViewModel(
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
        _gFunctions = gFunctions;

        ChangeLanguage();
        InitializeSupplierCBVM();
    }

    public override async Task InitializeAsync(DetailAction action, object pk)
    {
        int PSOrderId = (int)pk;

        base._action = action;

        bool loadSearchSuppliers = false;
        bool loadSearchItems = false;

        switch (action)
        {
            case DetailAction.ViewDetail:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleView"] ?? string.Empty,
                    TranslationSource.Instance["mwPurchaseOrder"]);
                break;

            case DetailAction.Add:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleAdd"] ?? string.Empty,
                    TranslationSource.Instance["mwPurchaseOrder"]);
                Data = new PSOrder()
                {
                    PSDate = DateTime.Today,
                    CurrencyId = "IDR",
                    CurrencyRate = 1,
                    PaidCurrencyId = "IDR",
                    PaidCurrencyRate = 1,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "",
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = ""
                };
                Data.AddPSOrderDetail();

                loadSearchSuppliers = true;
                loadSearchItems = true;

                break;

            case DetailAction.Edit:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleEdit"] ?? string.Empty,
                    TranslationSource.Instance["mwPurchaseOrder"]);

                ServiceActionResult sar = await _apiClient.GetPurchaseOrderByIdAsync(PSOrderId);
                if (sar.ActionIsSuccess)
                {
                    Data = (PSOrder)sar.ActionResult;
                }

                loadSearchSuppliers = true;
                loadSearchItems = true;

                break;

            default:
                break;
        }

        if (loadSearchSuppliers)
        {
            await _supplierCBVM.LoadSearchSupplierAsync();
        }

        if (!string.IsNullOrWhiteSpace(Data?.SupplierCode))
        {
            Data.SupplierName = (_supplierCBVM._suppliers.First(s => s.SupplierCode == Data.SupplierCode))?.SupplierName ?? string.Empty;
        }

        if (loadSearchItems)
        {
            await LoadSearchItemAsync();
        }

        if (Data?.PSOrderDetails.Count > 0)
        {
            foreach (var detail in Data.PSOrderDetails)
            {
                if (detail.ItemName != "")
                    detail.ItemName = (_items.First(i => i.ItemCode == detail.ItemCode))?.ItemName ?? string.Empty;
            }

            SelectedItemDetail = Data.PSOrderDetails[0];
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
        foreach (var detail in Data.PSOrderDetails)
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
                        TranslationSource.Instance["propQty"] + " (" + detail.ItemCode + ")") +
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
                    result = await _apiClient.CreatePurchaseOrderAsync(_data);
                    break;

                case DetailAction.Edit:
                    result = await _apiClient.UpdatePurchaseOrderAsync(_data);
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
                                    _data.PSCode);
                            break;

                        case ModelValidationType.DuplicateRecord:
                            message += Environment.NewLine +
                                string.Format(
                                    TranslationSource.Instance["messageDuplicateRecord"] ?? string.Empty,
                                    _data.PSCode);
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

    private DetailAction _PSOrderDetailAction;

    [ObservableProperty]
    private bool _AddCommandChecked;

    private void ResetCommandChecked()
    {
        AddCommandChecked = false;
    }

    [RelayCommand]
    private void AddItemDetail()
    {
        ResetCommandChecked();

        _PSOrderDetailAction = DetailAction.Add;
        AddCommandChecked = true;

        Data.AddPSOrderDetail();
        SelectedItemDetail = Data.PSOrderDetails.LastOrDefault();
    }

    [RelayCommand]
    private void EditItemDetail()
    {
        ResetCommandChecked();

        if (SelectedItemDetail != null)
        {
            _PSOrderDetailAction = DetailAction.Edit;
        }

    }

    [RelayCommand]
    private void DeleteItemDetail()
    {
        ResetCommandChecked();

        if (SelectedItemDetail != null)
        {
            Data.PSOrderDetails.Remove(SelectedItemDetail);
            SelectedItemDetail = Data.PSOrderDetails.FirstOrDefault();
        }
    }

    //[RelayCommand]
    //private void SavePSOrderDetail()
    //{
    //    if (_PSOrderDetailAction == DetailAction.Add)
    //    {
    //        Data.PSOrderDetails.Add(new PSOrderDetail()
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
    //        Data.OnPropertyChanged(nameof(Data.PSOrderDetails));

    //        SelectedPSD = Data.PSOrderDetails.LastOrDefault();
    //        return;
    //    }
    //    else if (_PSOrderDetailAction == DetailAction.Edit)
    //    {
    //        if (SelectedPSD != null)
    //        {
    //            SelectedPSD.ItemName = DataDetail.ItemName;
    //            SelectedPSD.ItemCode = DataDetail.ItemCode;
    //            SelectedPSD.Qty = DataDetail.Qty;
    //            OnPropertyChanged(nameof(Data));
    //        }
    //    }

    //    ResetCommandChecked();
    //}

    #endregion
}
