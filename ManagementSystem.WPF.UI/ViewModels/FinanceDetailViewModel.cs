
using ManagementSystem.Common.OtherModels;
using ManagementSystem.WPF.UI.Data;
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using ManagementSystem.WPF.UI.ViewModels.ToInherit;

namespace ManagementSystem.WPF.UI.ViewModels;

public partial class FinanceDetailViewModel : DetailViewModel
{
    [ObservableProperty]
    public string Title { get; private set; }

    [ObservableProperty]
    public bool CanEditFinanceDate
    {
        get
        {
            return _action == DetailAction.Add;
        }
    }

    private FinanceDetail? _selectedFinanceDetail;

    public FinanceDetail? SelectedFinanceDetail
    {
        get { return _selectedFinanceDetail; }
        set
        {
            ResetCommandChecked();

            _selectedFinanceDetail = value;
            OnPropertyChanged(nameof(SelectedFinanceDetail));
        }
    }

    [ObservableProperty]
    private Finance _data;

    #region Data
    private void Data_ReferenceTypeChanged()
    {
        if (_data.RTMenuPurchase)
            CBVMPSOrder.PSType = SearchPSOrderForOptions.PSTypeEnum.Purchase;
        else if (_data.RTMenuSales)
            CBVMPSOrder.PSType = SearchPSOrderForOptions.PSTypeEnum.Sales;
        else if (_data.RTOther)
            CBVMPSOrder.PSType = SearchPSOrderForOptions.PSTypeEnum.Other;
    }

    #endregion

    public CurrencyIdEnum CurrencyIdEnumValue
    {
        get
        {
            if (Data == null) return CurrencyIdEnum.IDR;

            switch (Data.CurrencyId)
            {
                case "IDR":
                    return CurrencyIdEnum.IDR;
                case "CNY":
                    return CurrencyIdEnum.CNY;
                case "USD":
                    return CurrencyIdEnum.USD;
                default:
                    return CurrencyIdEnum.IDR;
            }
        }
        set
        {
            Data.CurrencyId = value.ToString();
            OnPropertyChanged(nameof(CurrencyIdEnumValue));
            OnPropertyChanged(nameof(Data));
        }
    }

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
        return _data.SupplierCode;
    }

    private void _supplierCBVM_SetSupplierName(string supplierName)
    {
        _data.SupplierName = supplierName;
    }

    #endregion

    #region Initialize Combo Box PSOrder

    private InstantComboBoxVMPSOrder _CBVMPSOrder;
    public InstantComboBoxVMPSOrder CBVMPSOrder
    {
        get
        {
            return _CBVMPSOrder;
        }
    }

    private void InitializeCBVMPSOrder()
    {
        _CBVMPSOrder = new InstantComboBoxVMPSOrder(_gFunctions);
        _CBVMPSOrder.SelectedItemChanged = _CBVMPSOrder_SelectedItemChanged;
        _CBVMPSOrder.GetItemCode = _CBVMPSOrder_GetItemCode;
        _CBVMPSOrder.SetItemName = _CBVMPSOrder_SetItemName;
    }

    private void _CBVMPSOrder_SelectedItemChanged(SearchOptionResultPSOrder? selectedPSOrder)
    {
        if (selectedPSOrder != null)
        {
            _data.ReferenceCode = selectedPSOrder.PSCode;
            _data.SupplierCode = selectedPSOrder.SupplierCode;
            _data.SupplierName = selectedPSOrder.SupplierName;
        }
        else
        {
            _data.ReferenceCode = string.Empty;
            _data.SupplierCode = string.Empty;
            _data.SupplierName = string.Empty;
        }
    }

    private string _CBVMPSOrder_GetItemCode()
    {
        return _data.ReferenceCode;
    }

    private void _CBVMPSOrder_SetItemName(string PSOrderName)
    {
        //_data.SupplierName = PSOrderName;
    }

    #endregion

    IContentDialogService _contentDialogService;
    IViewInteractionService _viewInteractionService;
    DetailViewModelDoAction _promptAction;
    IManagementApiClient _apiClient;
    JsonSerializerOptions _jsonOptions;
    private GlobalFunctions _gFunctions;

    public FinanceDetailViewModel(
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
        InitializeCBVMPSOrder();
    }

    public override async Task InitializeAsync(DetailAction action, object pk)
    {
        int FinanceId = (int)pk;

        base._action = action;

        bool loadSearchSuppliers = false;
        bool loadSearchPSOrders = false;

        switch (action)
        {
            case DetailAction.ViewDetail:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleView"] ?? string.Empty,
                    TranslationSource.Instance["mwFinance"]);
                break;

            case DetailAction.Add:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleAdd"] ?? string.Empty,
                    TranslationSource.Instance["mwFinance"]);
                Data = new Finance()
                {
                    FinanceDate = DateTime.Today,
                    CurrencyId = "IDR",
                    PaidCurrencyId = "IDR",
                    CreatedAt = DateTime.Now,
                    CreatedBy = "",
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = ""
                };

                Data.RTMenuPurchase = true;
                Data.FTAPDebitNote = true;

                Data.AddFinanceDetail();

                loadSearchSuppliers = true;
                loadSearchPSOrders = true;

                break;

            case DetailAction.Edit:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleEdit"] ?? string.Empty,
                    TranslationSource.Instance["mwFinance"]);

                ServiceActionResult sar = await _apiClient.GetFinanceByIdAsync(FinanceId);
                if (sar.ActionIsSuccess)
                {
                    Data = (Finance)sar.ActionResult;
                }

                loadSearchSuppliers = true;
                loadSearchPSOrders = true;

                break;

            default:
                break;
        }

        _data.DelegadeReferenceTypeChanged += Data_ReferenceTypeChanged;
        //if (loadSearchPSOrders)
        //{
        //}

        if (loadSearchSuppliers)
        {
            await _supplierCBVM.LoadSearchSupplierAsync();
        }

        if (!string.IsNullOrWhiteSpace(Data?.SupplierCode))
        {
            Data.SupplierName = (_supplierCBVM._suppliers.First(s => s.SupplierCode == Data.SupplierCode))?.SupplierName ?? string.Empty;
        }

        if (Data.RTMenuPurchase)
            _CBVMPSOrder.PSType = SearchPSOrderForOptions.PSTypeEnum.Purchase;
        else if (Data.RTMenuSales)
            _CBVMPSOrder.PSType = SearchPSOrderForOptions.PSTypeEnum.Sales;

        if (loadSearchPSOrders && !string.IsNullOrWhiteSpace(Data?.ReferenceCode))
        {
            _CBVMPSOrder.FilterText = Data?.ReferenceCode;

            await _CBVMPSOrder.LoadSearchAsync();
        }

        if (Data?.FinanceDetails.Count > 0)
        {
            SelectedFinanceDetail = Data.FinanceDetails[0];
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
        foreach (var detail in Data.FinanceDetails)
        {
            if (detail.PaidAmount == 0)
            {
                message += $@"[{i}] " +
                    string.Format(
                        TranslationSource.Instance["messageFieldIsRequired"] ?? string.Empty,
                        TranslationSource.Instance["propPaidAmount"]) +
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
                    result = await _apiClient.CreateFinanceAsync(_data);
                    break;

                case DetailAction.Edit:
                    result = await _apiClient.UpdateFinanceAsync(_data);
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
                                    _data.FinanceCode);
                            break;

                        case ModelValidationType.DuplicateRecord:
                            message += Environment.NewLine +
                                string.Format(
                                    TranslationSource.Instance["messageDuplicateRecord"] ?? string.Empty,
                                    _data.FinanceCode);
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

    private DetailAction _FinanceDetailAction;

    [ObservableProperty]
    private bool _AddCommandChecked;

    private void ResetCommandChecked()
    {
        AddCommandChecked = false;
    }

    [RelayCommand]
    private void AddFinanceDetail()
    {
        ResetCommandChecked();

        _FinanceDetailAction = DetailAction.Add;
        AddCommandChecked = true;

        Data.AddFinanceDetail();
        SelectedFinanceDetail = Data.FinanceDetails.LastOrDefault();
    }

    [RelayCommand]
    private void EditFinanceDetail()
    {
        ResetCommandChecked();

        if (SelectedFinanceDetail != null)
        {
            _FinanceDetailAction = DetailAction.Edit;
        }

    }

    [RelayCommand]
    private void DeleteFinanceDetail()
    {
        ResetCommandChecked();

        if (SelectedFinanceDetail != null)
        {
            Data.FinanceDetails.Remove(SelectedFinanceDetail);
            SelectedFinanceDetail = Data.FinanceDetails.FirstOrDefault();
        }
    }

    //[RelayCommand]
    //private void SaveFinanceDetail()
    //{
    //    if (_FinanceDetailAction == DetailAction.Add)
    //    {
    //        Data.FinanceDetails.Add(new FinanceDetail()
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
    //        Data.OnPropertyChanged(nameof(Data.FinanceDetails));

    //        SelectedFinanceDetail = Data.FinanceDetails.LastOrDefault();
    //        return;
    //    }
    //    else if (_FinanceDetailAction == DetailAction.Edit)
    //    {
    //        if (SelectedFinanceDetail != null)
    //        {
    //            SelectedFinanceDetail.ItemName = DataDetail.ItemName;
    //            SelectedFinanceDetail.ItemCode = DataDetail.ItemCode;
    //            SelectedFinanceDetail.Qty = DataDetail.Qty;
    //            OnPropertyChanged(nameof(Data));
    //        }
    //    }

    //    ResetCommandChecked();
    //}

    #endregion
}
