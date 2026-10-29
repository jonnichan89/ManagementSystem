
using ManagementSystem.WPF.UI.Data;
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using ManagementSystem.WPF.UI.ViewModels.ToInherit;

namespace ManagementSystem.WPF.UI.ViewModels;

public partial class ProjectPurchaseOrderDetailViewModel : DetailViewModel
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

    private PSOrderPT? _selectedPTDetail;

    public PSOrderPT? SelectedPTDetail
    {
        get { return _selectedPTDetail; }
        set
        {
            ResetCommandChecked();

            _selectedPTDetail = value;
            if (_selectedPTDetail != null && !string.IsNullOrEmpty(_selectedPTDetail.PTName))
            {
                FilterText = _selectedPTDetail.PTName;
            }
            else
            {
                FilterText = "";
            }

            OnPropertyChanged(nameof(SelectedPTDetail));
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
        _supplierCBVM = new InstantSupplierComboBoxVM(_gFunctions, InstantSupplierComboBoxVM.EnumSupplierUsedIn.ProjectPurchase);
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

    #region Search PT

    string _filterText;
    Predicate<object> _filterPredicate;
    ObservableRangeCollection<ProjectTemplate> _PTs = new();

    protected internal ProjectTemplate? _selectedPT;

    [ObservableProperty]
    private string? _previewImagePath;

    [ObservableProperty]
    private ListCollectionView _filteredViewPT;

    [ObservableProperty]
    private int _PTSelectedIndex;

    public ProjectTemplate? SelectedPT
    {
        get { return _selectedPT; }
        set
        {
            _selectedPT = value;

            if (_selectedPT != null)
            {
                FilterText = _selectedPT.PTName;
                PreviewImagePath = _selectedPT.ImagePath;

                if (SelectedPTDetail != null)
                {
                    SelectedPTDetail.PTId = _selectedPT.PTId;
                    SelectedPTDetail.PTName = _selectedPT.PTName;
                    OnPropertyChanged(nameof(SelectedPTDetail));
                    Data.OnPropertyChanged(nameof(Data.PSOrderPTs));
                }
            }

            OnPropertyChanged(nameof(SelectedPT));
        }
    }

    public string FilterText
    {
        get { return _filterText; }
        set
        {
            _filterText = value;

            if (_filterText.Length >= 3 &&
                _PTs.Count > 0 &&
                (_selectedPT == null || _selectedPT.PTName != _filterText))
            {
                var foundItem = _PTs.FirstOrDefault(i => i.PTName == value);

                if (foundItem == null)
                {
                    PreviewImagePath = null;

                    if (SelectedPTDetail != null)
                    {
                        SelectedPTDetail.PTId = 0;
                        SelectedPTDetail.PTName = string.Empty;
                        OnPropertyChanged(nameof(SelectedPTDetail));
                    }

                    using (FilteredViewPT.DeferRefresh())
                    {
                        FilteredViewPT.Filter = _filterPredicate;
                        FilteredViewPT.SortDescriptions.Add(new SortDescription("Askterisk", ListSortDirection.Descending));
                        FilteredViewPT.SortDescriptions.Add(new SortDescription("PTName", ListSortDirection.Ascending));
                    }
                    FilteredViewPT.MoveCurrentTo(FilteredViewPT.GetItemAt(0));

                    _filterText = (FilteredViewPT.GetItemAt(0) as ProjectTemplate)?.PTName ?? string.Empty;
                }
                else
                {
                    PreviewImagePath = foundItem.ImagePath;

                    if (SelectedPTDetail != null)
                    {
                        SelectedPTDetail.PTId = foundItem.PTId;
                        SelectedPTDetail.PTName = foundItem.PTName;
                        OnPropertyChanged(nameof(SelectedPTDetail));
                        Data.OnPropertyChanged(nameof(Data.PSOrderPTs));
                    }

                    PTSelectedIndex = FilteredViewPT.IndexOf(foundItem);
                    _filterText = foundItem.PTName; 
                }
                OnPropertyChanged(nameof(Data));
            }
        }
    }
    
    private async Task LoadSearchProjectTemplateAsync(bool isFirstLoad = false)
    {
        await _gFunctions.RefreshProjectTemplateDataAsync(false);
        //if (!isFirstLoad && !hasNewData)
        //{
        //    return;
        //}

        _PTs.Clear();
        _PTs.AddRange(_gFunctions.ProjectTemplates.Where(p => p.IsFirstProject).ToArray());

        if (_data == null) return;

        //foreach (var PSOrderPT in Data.PSOrderPTs)
        //{
        //    PSOrderPT.PTName = _PTs.FirstOrDefault(i => i.PTId == PSOrderPT.PTId)?.PTName ?? string.Empty;
        //}

        _filterPredicate = item =>
        {
            var item1 = (item as ProjectTemplate);
            item1.PutSearchToTop = (item1.IsFirstProject && item1.SearchKey.Contains(_filterText, StringComparison.OrdinalIgnoreCase));
            return item1.IsFirstProject;
        };

        FilteredViewPT = new ListCollectionView(_PTs);
    }

    #endregion

    IContentDialogService _contentDialogService;
    IViewInteractionService _viewInteractionService;
    DetailViewModelDoAction _promptAction;
    IManagementApiClient _apiClient;
    JsonSerializerOptions _jsonOptions;
    private GlobalFunctions _gFunctions;

    public ProjectPurchaseOrderDetailViewModel(
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
        bool loadSearchPTs = false;

        switch (action)
        {
            case DetailAction.ViewDetail:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleView"] ?? string.Empty,
                    TranslationSource.Instance["mwProjectPurchaseOrder"]);
                break;

            case DetailAction.Add:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleAdd"] ?? string.Empty,
                    TranslationSource.Instance["mwProjectPurchaseOrder"]);
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
                Data.AddPSOrderPT();

                loadSearchSuppliers = true;
                loadSearchPTs = true;

                break;

            case DetailAction.Edit:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleEdit"] ?? string.Empty,
                    TranslationSource.Instance["mwProjectPurchaseOrder"]);

                ServiceActionResult sar = await _apiClient.GetProjectPurchaseOrderByIdAsync(PSOrderId);
                if (sar.ActionIsSuccess)
                {
                    Data = (PSOrder)sar.ActionResult;

                    foreach (var itemDetail in Data.PSOrderPTs)
                        itemDetail.PSOrder = Data;
                }

                loadSearchSuppliers = true;
                loadSearchPTs = true;

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

        if (loadSearchPTs)
        {
            await LoadSearchProjectTemplateAsync(true);
        }

        if (Data?.PSOrderPTs.Count > 0)
        {
            //foreach (var detail in Data.PSOrderPTs)
            //{
            //    if (detail.PTName != "")
            //        detail.PTName = (_PTs.First(i => i.ItemCode == detail.ItemCode))?.PTName ?? string.Empty;
            //}

            SelectedPTDetail = Data.PSOrderPTs[0];
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
        foreach (var detail in Data.PSOrderPTs)
        {
            if (detail.PTId == 0)
            {
                message += $@"[{i}] " +
                    string.Format(
                        TranslationSource.Instance["messageFieldIsRequired"] ?? string.Empty,
                        TranslationSource.Instance["propPTName"]) +
                    Environment.NewLine;

                absoluteCannotSave = true;
            }

            if (detail.Qty == 0)
            {
                message += $@"[{i}] " +
                    string.Format(
                        TranslationSource.Instance["messageFieldIsRequired"] ?? string.Empty,
                        TranslationSource.Instance["propQty"] + " (" + detail.PTName + ")") +
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
                    result = await _apiClient.CreateProjectPurchaseOrderAsync(_data);
                    break;

                case DetailAction.Edit:
                    result = await _apiClient.UpdateProjectPurchaseOrderAsync(_data);
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

    private DetailAction _PSOrderPTAction;

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

        _PSOrderPTAction = DetailAction.Add;
        AddCommandChecked = true;

        Data.AddPSOrderPT();
        SelectedPTDetail = Data.PSOrderPTs.LastOrDefault();
    }

    [RelayCommand]
    private void EditItemDetail()
    {
        ResetCommandChecked();

        if (SelectedPTDetail != null)
        {
            _PSOrderPTAction = DetailAction.Edit;
        }

    }

    [RelayCommand]
    private void DeleteItemDetail()
    {
        ResetCommandChecked();

        if (SelectedPTDetail != null)
        {
            Data.RemovePSOrderPT(SelectedPTDetail);
            SelectedPTDetail = Data.PSOrderPTs.FirstOrDefault();
        }
    }

    //[RelayCommand]
    //private void SavePSOrderPT()
    //{
    //    if (_PSOrderPTAction == DetailAction.Add)
    //    {
    //        Data.PSOrderPTs.Add(new PSOrderPT()
    //        {
    //            PTName = DataDetail.PTName,
    //            ItemCode = DataDetail.ItemCode,
    //            Qty = DataDetail.Qty,
    //            CreatedAt = DateTime.Now,
    //            CreatedBy = "",
    //            ModifiedAt = DateTime.Now,
    //            ModifiedBy = ""
    //        });
    //        OnPropertyChanged(nameof(Data));
    //        Data.OnPropertyChanged(nameof(Data.PSOrderPTs));

    //        SelectedPSD = Data.PSOrderPTs.LastOrDefault();
    //        return;
    //    }
    //    else if (_PSOrderPTAction == DetailAction.Edit)
    //    {
    //        if (SelectedPSD != null)
    //        {
    //            SelectedPSD.PTName = DataDetail.PTName;
    //            SelectedPSD.ItemCode = DataDetail.ItemCode;
    //            SelectedPSD.Qty = DataDetail.Qty;
    //            OnPropertyChanged(nameof(Data));
    //        }
    //    }

    //    ResetCommandChecked();
    //}

    #endregion
}
