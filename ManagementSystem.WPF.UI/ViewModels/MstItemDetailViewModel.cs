
namespace ManagementSystem.WPF.UI.ViewModels;

public partial class MstItemDetailViewModel : DetailViewModel
{
    [ObservableProperty]
    public string Title { get; private set; }

    [ObservableProperty]
    private string _imagePath;

    [ObservableProperty]
    public bool CanEditItemCode
    {
        get
        {
            return _action != DetailAction.Add;
        }
    }

    #region Search Supplier

    string _filterTextSupplier;
    Predicate<object> _filterPredicateSupplier;
    ObservableRangeCollection<MstSupplier> _suppliers = new();

    protected internal MstSupplier? _selectedSupplier;

    [ObservableProperty]
    private ListCollectionView _filteredViewSupplier;

    [ObservableProperty]
    private int _supplierSelectedIndex;

    public MstSupplier? SelectedSupplier
    {
        get { return _selectedSupplier; }
        set
        {
            _selectedSupplier = value;

            if (_selectedSupplier != null)
            {
                FilterSupplierText = _selectedSupplier.SupplierName;

                if (_selectedSupplier != null)
                {
                    _data.SupplierCode = _selectedSupplier.SupplierCode;
                    _data.SupplierName = _selectedSupplier.SupplierName;
                }
            }

            OnPropertyChanged(nameof(SelectedSupplier));
        }
    }

    public bool FilterSupplier { get; set; }
    public string FilterSupplierText
    {
        get { return _filterTextSupplier; }
        set
        {
            System.Diagnostics.Debug.WriteLine("FilterSupplierText: " + value + " - FilterSupplier: " + FilterSupplier.ToString());
            _filterTextSupplier = value;

            if (_filterTextSupplier.Length >= 3 &&
                _suppliers.Count > 0 &&
                (_selectedSupplier == null || _selectedSupplier.SupplierName != _filterTextSupplier))
            {
                var foundSupplier = _suppliers.FirstOrDefault(i => i.SupplierName == value);

                if (foundSupplier == null)
                {
                    System.Diagnostics.Debug.WriteLine("foundSupplier: false");
                    using (FilteredViewSupplier.DeferRefresh())
                    {
                        FilteredViewSupplier.Filter = _filterPredicateSupplier;
                        FilteredViewSupplier.SortDescriptions.Add(new SortDescription("Askterisk", ListSortDirection.Descending));
                        FilteredViewSupplier.SortDescriptions.Add(new SortDescription("SupplierName", ListSortDirection.Ascending));
                    }
                    if (FilterSupplier)
                    {
                        System.Diagnostics.Debug.WriteLine("MoveCurrentTo: " + ((MstSupplier)FilteredViewSupplier.GetItemAt(0)).SupplierName);
                        FilteredViewSupplier.MoveCurrentTo(FilteredViewSupplier.GetItemAt(0));

                        _filterTextSupplier = (FilteredViewSupplier.GetItemAt(0) as MstItem)?.ItemName ?? string.Empty;
                        SupplierSelectedIndex = 0;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("foundSupplier: true");
                    _data.SupplierCode = foundSupplier.SupplierCode;
                    _data.SupplierName = foundSupplier.SupplierName;

                    if (FilterSupplier)
                    {
                        SupplierSelectedIndex = FilteredViewSupplier.IndexOf(foundSupplier);
                        _filterTextSupplier = foundSupplier.SupplierName;
                    }
                }
                OnPropertyChanged(nameof(Data));
            }

            FilterSupplier = false;
        }
    }

    private async Task LoadSearchSupplierAsync()
    {
        await _gFunctions.RefreshSupplierDataAsync(false);
        _suppliers.Clear();
        _suppliers.AddRange(_gFunctions.Suppliers.ToArray());

        _filterPredicateSupplier = item =>
        {
            var item1 = (item as MstSupplier);
            item1.PutSearchToTop = (item1.SearchKey.Contains(_filterTextSupplier, StringComparison.OrdinalIgnoreCase));
            return true;
        };

        string supplierCode = _data.SupplierCode;
        _filteredViewSupplier = new ListCollectionView(_suppliers);
        OnPropertyChanged(nameof(FilteredViewSupplier));

        SelectedSupplier = _suppliers.FirstOrDefault(i => i.SupplierCode == supplierCode);

        if (_selectedSupplier != null)
        {
            _data.SupplierName = _selectedSupplier.SupplierName;
            _supplierSelectedIndex = FilteredViewSupplier.IndexOf(SelectedSupplier);
        }
        else
        {
            _data.SupplierCode = "";
            _data.SupplierName = "";
            _supplierSelectedIndex = -1;
        }

        OnPropertyChanged(nameof(SupplierSelectedIndex));
    }

    #endregion

    IContentDialogService _contentDialogService;
    IViewInteractionService _viewInteractionService;
    DetailViewModelDoAction _promptAction;
    IManagementApiClient _apiClient;
    JsonSerializerOptions _jsonOptions;
    GlobalFunctions _gFunctions;
    public MstItemDetailViewModel(
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
    }

    public override async Task InitializeAsync(DetailAction action, object pk)
    {
        int itemId = (int)pk;

        base._action = action;

        bool loadSearchSuppliers = false;

        switch (action)
        {
            case DetailAction.ViewDetail:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleView"] ?? string.Empty,
                    TranslationSource.Instance["mwMstItem"]);
                break;

            case DetailAction.Add:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleAdd"] ?? string.Empty,
                    TranslationSource.Instance["mwMstItem"]);
                Data = new MstItem()
                {
                    ItemCode = "",
                    ItemName = "",
                    ItemDescription = "",
                    ItemUnit = "",
                    IsStockControl = false,
                    IsBlacklist = false,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "",
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = ""
                };

                loadSearchSuppliers = true;

                break;

            case DetailAction.Edit:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleEdit"] ?? string.Empty,
                    TranslationSource.Instance["mwMstItem"]);

                ServiceActionResult sar = await _apiClient.GetMstItemByIdAsync(itemId);
                if (sar.ActionIsSuccess)
                {
                    Data = (MstItem)sar.ActionResult;
                }

                loadSearchSuppliers = true;

                break;

            default:
                break;
        }

        if (loadSearchSuppliers)
        {
            await LoadSearchSupplierAsync();
        }

        if (!string.IsNullOrWhiteSpace(Data?.SupplierCode))
        {
            Data.SupplierName = (_suppliers.First(s => s.SupplierCode == Data.SupplierCode))?.SupplierName ?? string.Empty;
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

    [ObservableProperty]
    private MstItem _data;
    public override void TrySavePrimary()
    {
        _promptAction = DetailViewModelDoAction.SavePrimary;

        bool absoluteCannotSave = false;
        string message = "";

        if (string.IsNullOrWhiteSpace(_data.ItemCode))
        {
            message +=
                string.Format(
                    TranslationSource.Instance["messageFieldIsRequired"] ?? string.Empty,
                    TranslationSource.Instance["propItemCode"]) +
                Environment.NewLine;

            absoluteCannotSave = true;
        }

        if (string.IsNullOrWhiteSpace(_data.ItemName))
        {
            message +=
                string.Format(
                    TranslationSource.Instance["messageFieldIsRequired"] ?? string.Empty,
                    TranslationSource.Instance["propItemName"]) +
                Environment.NewLine;

            absoluteCannotSave = true;
        }

        if (!string.IsNullOrWhiteSpace(ImagePath))
        {
            if (System.IO.File.Exists(ImagePath))
            {
                string fileExt = System.IO.Path.GetExtension(ImagePath);
                switch (fileExt)
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                        break;

                    default:
                        message +=
                            string.Format(
                                TranslationSource.Instance["messageUnsupportedFileType"] ?? string.Empty,
                                TranslationSource.Instance["propImagePath"], Environment.NewLine + ImagePath) +
                            Environment.NewLine;

                        absoluteCannotSave = true;
                        break;
                }
            }
            else
            {
                message +=
                    string.Format(
                        TranslationSource.Instance["messageFileNotExists"] ?? string.Empty,
                        TranslationSource.Instance["propImagePath"], Environment.NewLine + ImagePath) +
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
            Stream? imageStream = null;
            string fileExt = "";
            if (!string.IsNullOrWhiteSpace(ImagePath))
            {
                imageStream = new FileStream(ImagePath, FileMode.Open);
                fileExt = System.IO.Path.GetFileName(ImagePath);
            }

            ServiceActionResult result;
            switch (_action)
            {
                case DetailAction.Add:
                    result = await _apiClient.CreateMstItemAsync(_data, imageStream, fileExt);
                    break;

                case DetailAction.Edit:
                    result = await _apiClient.UpdateMstItemAsync(_data, imageStream, fileExt);
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
                                    _data.ItemCode);

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
                _gFunctions.Items.Clear();
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

    private OpenFileDialog _openFileDialog;

    [RelayCommand]
    private void PickImageFile()
    {
        if (_openFileDialog == null)
        {
            string initialDirectory = ManagementSystem.WPF.UI.Properties.Settings.Default.LastImagePath;

            if (string.IsNullOrEmpty(initialDirectory))
            {
                initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }

            _openFileDialog = new()
            {
                InitialDirectory = initialDirectory,
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
            };
        }

        if (_openFileDialog.ShowDialog() != true)
        {
            return;
        }

        if (!File.Exists(_openFileDialog.FileName))
        {
            return;
        }

        string directoryPath = System.IO.Path.GetDirectoryName(_openFileDialog.FileName);
        if (ManagementSystem.WPF.UI.Properties.Settings.Default.LastImagePath != directoryPath)
        {
            ManagementSystem.WPF.UI.Properties.Settings.Default.LastImagePath = directoryPath;
            ManagementSystem.WPF.UI.Properties.Settings.Default.Save();

            _openFileDialog.InitialDirectory = ManagementSystem.WPF.UI.Properties.Settings.Default.LastImagePath;
        }

        ImagePath = _openFileDialog.FileName;
    }

}
