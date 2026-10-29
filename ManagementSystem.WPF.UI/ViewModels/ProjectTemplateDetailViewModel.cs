
using ManagementSystem.WPF.UI.Data;
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using ManagementSystem.WPF.UI.ViewModels.ToInherit;

namespace ManagementSystem.WPF.UI.ViewModels;

public partial class ProjectTemplateDetailViewModel : DetailViewModel
{
    [ObservableProperty]
    public string Title { get; private set; }

    [ObservableProperty]
    public bool CanEditPTDate
    {
        get
        {
            return _action == DetailAction.Add;
        }
    }

    private ProjectTemplate? _selectedData;
    private ProjectTemplateDetail? _selectedDataDetail;

    public ProjectTemplate? SelectedData
    {
        get { return _selectedData; }
        set
        {
            _selectedData = value;
            if (_selectedData.ProjectTemplateDetails.Count > 0)
            {
                SelectedDataDetail = _selectedData.ProjectTemplateDetails[0];
            }

            OnPropertyChanged(nameof(SelectedData));
        }
    }

    public ProjectTemplateDetail? SelectedDataDetail
    {
        get { return _selectedDataDetail; }
        set
        {
            ResetDataDetailCommandChecked();

            _selectedDataDetail = value;
            if (_selectedDataDetail != null && !string.IsNullOrEmpty(_selectedDataDetail.ItemCode))
            {
                FilterText = _selectedDataDetail.ItemCode;
            }
            else
            {
                FilterText = "";
            }

            OnPropertyChanged(nameof(SelectedDataDetail));
        }
    }

    [ObservableProperty]
    private ProjectTemplate _data;

    #region Search Item

    string _filterText;
    Predicate<object> _filterPredicate;
    ObservableRangeCollection<MstItem> _items = new();

    protected internal MstItem? _selectedItem;

    [ObservableProperty]
    private string? _dataDetailPreviewImagePath;

    [ObservableProperty]
    private ListCollectionView _filteredItem;

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
                FilterText = _selectedItem.ItemCode;
                DataDetailPreviewImagePath = _selectedItem.ImagePath;

                if (SelectedDataDetail != null)
                {
                    SelectedDataDetail.ItemCode = _selectedItem.ItemCode;
                    SelectedDataDetail.ItemName = _selectedItem.ItemName;
                    SelectedDataDetail.Unit = _selectedItem.ItemUnit;
                    OnPropertyChanged(nameof(SelectedDataDetail));
                    SelectedData.OnPropertyChanged(nameof(SelectedData.ProjectTemplateDetails));
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
                (_selectedItem == null || _selectedItem.ItemCode != _filterText))
            {
                var foundItem = _items.FirstOrDefault(i => i.ItemCode == value);

                if (foundItem == null)
                {
                    DataDetailPreviewImagePath = null;

                    if (SelectedDataDetail != null)
                    {
                        SelectedDataDetail.ItemCode = string.Empty;
                        SelectedDataDetail.ItemName = string.Empty;
                        SelectedDataDetail.Unit = string.Empty;
                        OnPropertyChanged(nameof(SelectedDataDetail));
                    }

                    using (FilteredItem.DeferRefresh())
                    {
                        FilteredItem.Filter = _filterPredicate;
                        FilteredItem.SortDescriptions.Add(new SortDescription("Askterisk", ListSortDirection.Descending));
                        FilteredItem.SortDescriptions.Add(new SortDescription("ItemCode", ListSortDirection.Ascending));
                    }
                    FilteredItem.MoveCurrentTo(FilteredItem.GetItemAt(0));

                    _filterText = (FilteredItem.GetItemAt(0) as MstItem)?.ItemCode ?? string.Empty;
                }
                else
                {
                    DataDetailPreviewImagePath = foundItem.ImagePath;

                    if (SelectedDataDetail != null)
                    {
                        SelectedDataDetail.ItemCode = foundItem.ItemCode;
                        SelectedDataDetail.ItemCode = foundItem.ItemCode;
                        OnPropertyChanged(nameof(SelectedDataDetail));
                        SelectedData.OnPropertyChanged(nameof(SelectedData.ProjectTemplateDetails));
                    }

                    ItemSelectedIndex = FilteredItem.IndexOf(foundItem);
                    _filterText = foundItem.ItemCode;
                }
                OnPropertyChanged(nameof(SelectedDataDetail));
            }
        }
    }

    private async Task LoadSearchItemAsync(bool isFirstLoad = false)
    {
        await _gFunctions.RefreshItemDataAsync(false);
        //if (!isFirstLoad && !hasNewData)
        //{
        //    return;
        //}

        _items.Clear();
        _items.AddRange(_gFunctions.Items.ToArray());

        if (_selectedData == null) return;

        foreach (var projectTemplateDetail in SelectedData.ProjectTemplateDetails)
        {
            projectTemplateDetail.ItemCode = _items.FirstOrDefault(i => i.ItemCode == projectTemplateDetail.ItemCode)?.ItemCode ?? string.Empty;
        }

        _filterPredicate = item =>
        {
            var item1 = (item as MstItem);
            item1.PutSearchToTop = (item1.SearchKey.Contains(_filterText, StringComparison.OrdinalIgnoreCase));
            return true;
        };

        FilteredItem = new ListCollectionView(_items);
    }

    #endregion

    IContentDialogService _contentDialogService;
    IViewInteractionService _viewInteractionService;
    DetailViewModelDoAction _promptAction;
    IManagementApiClient _apiClient;
    JsonSerializerOptions _jsonOptions;
    private GlobalFunctions _gFunctions;

    public ProjectTemplateDetailViewModel(
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
        int ProjectTemplateId = (int)pk;

        base._action = action;

        bool loadSearchItems = false;

        switch (action)
        {
            case DetailAction.ViewDetail:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleView"] ?? string.Empty,
                    TranslationSource.Instance["mwProjectTemplate"]);
                break;

            case DetailAction.Add:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleAdd"] ?? string.Empty,
                    TranslationSource.Instance["mwProjectTemplate"]);
                Data = new ProjectTemplate()
                {
                    PTName = "MainProject",
                    Note = "",
                    ImagePath = "",
                    ImagePathToUpload = "",
                    ParentPTId = null,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "",
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = ""
                };

                Data.AddProjectTemplate();

                SelectedData = Data.ProjectTemplates.First();
                SelectedData.AddProjectTemplateDetail();

                loadSearchItems = true;

                break;

            case DetailAction.Edit:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleEdit"] ?? string.Empty,
                    TranslationSource.Instance["mwProjectTemplate"]);

                ServiceActionResult sar = await _apiClient.GetProjectTemplateByIdAsync(ProjectTemplateId);
                if (sar.ActionIsSuccess)
                {
                    Data = (ProjectTemplate)sar.ActionResult;
                    if (Data.ProjectTemplates.Count > 0)
                        SelectedData = Data.ProjectTemplates.First();
                }

                loadSearchItems = true;

                break;

            default:
                break;
        }


        if (loadSearchItems)
        {
            await LoadSearchItemAsync(true);
        }

        //if (Data?.ProjectTemplateDetails.Count > 0)
        //{
        foreach (var data in Data.ProjectTemplates)
            foreach (var dataDetail in data.ProjectTemplateDetails)
            {
                if (SelectedDataDetail == null)
                    SelectedDataDetail = dataDetail;

                if (dataDetail.ItemCode != "")
                    dataDetail.ItemCode = (_items.First(i => i.ItemCode == dataDetail.ItemCode))?.ItemCode ?? string.Empty;
            }

        //}
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

        byte PTNo = 0;
        int PTDId = 0;
        foreach (var data in Data.ProjectTemplates)
        {
            PTNo++;
            data.PTNo = PTNo;

            PTDId = 0;
            foreach (var dataDetail in data.ProjectTemplateDetails)
            {
                PTDId++;
                dataDetail.PTId = data.PTId;
                dataDetail.PTDId = PTDId;

                if (string.IsNullOrWhiteSpace(dataDetail.ItemCode))
                {
                    message += $@"[{PTDId}] " +
                        string.Format(
                            TranslationSource.Instance["messageFieldIsRequired"] ?? string.Empty,
                            TranslationSource.Instance["propItemCode"]) +
                        Environment.NewLine;

                    absoluteCannotSave = true;
                }

                if (dataDetail.Qty == 0)
                {
                    message += $@"[{PTDId}] " +
                        string.Format(
                            TranslationSource.Instance["messageFieldIsRequired"] ?? string.Empty,
                            TranslationSource.Instance["propQty"] + " (" + dataDetail.ItemCode + ")") +
                        Environment.NewLine;
                    absoluteCannotSave = true;
                }
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
                    result = await _apiClient.CreateProjectTemplateAsync(_data);
                    break;

                case DetailAction.Edit:
                    result = await _apiClient.UpdateProjectTemplateAsync(_data);
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
                                    _data.PTName);
                            break;

                        case ModelValidationType.DuplicateRecord:
                            message += Environment.NewLine +
                                string.Format(
                                    TranslationSource.Instance["messageDuplicateRecord"] ?? string.Empty,
                                    _data.PTName);
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


    #region Data

    private DetailAction _ProjectTemplateDataAction;

    [ObservableProperty]
    private bool _AddDataCommandChecked;

    private void ResetDataCommandChecked()
    {
        _ProjectTemplateDataAction = DetailAction.None;
        AddDataCommandChecked = false;
    }

    [RelayCommand]
    private void AddData()
    {
        ResetDataCommandChecked();

        _ProjectTemplateDataAction = DetailAction.Add;
        AddDataCommandChecked = true;

        Data.AddProjectTemplate();
        SelectedData = Data.ProjectTemplates.LastOrDefault();
    }

    [RelayCommand]
    private void EditData()
    {
        ResetDataCommandChecked();

        if (SelectedData != null)
        {
            _ProjectTemplateDataAction = DetailAction.Edit;
        }

    }

    [RelayCommand]
    private void DeleteData()
    {
        ResetDataCommandChecked();

        if (SelectedData != null)
        {
            if (Data.ProjectTemplates.Count == 1)
            {
                _viewInteractionService.Prompt(
                    PromptUsedFor.MessageAlert,
                    string.Format(TranslationSource.Instance["messageTitleDelete"] ?? string.Empty, TranslationSource.Instance["mwProjectTemplate"]),
                    string.Format(TranslationSource.Instance["messageTitleLastItemCouldNotBeDeleted"] ?? string.Empty, TranslationSource.Instance["mwProjectTemplate"]));
            }
            else
            {
                SelectedData.ProjectTemplates.Remove(SelectedData);
                SelectedData = SelectedData.ProjectTemplates.FirstOrDefault();
            }
        }
    }

    private OpenFileDialog _openFileDialog;

    [RelayCommand]
    private void DataPickImageFile()
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

        SelectedData.ImagePath = _openFileDialog.FileName;
        SelectedData.ImagePathToUpload = _openFileDialog.FileName;
    }

    #endregion

    #region Detail

    private DetailAction _ProjectTemplateDetailAction;

    [ObservableProperty]
    private bool _AddDataDetailCommandChecked;

    private void ResetDataDetailCommandChecked()
    {
        AddDataDetailCommandChecked = false;
    }

    [RelayCommand]
    private void AddDataDetail()
    {
        ResetDataDetailCommandChecked();

        _ProjectTemplateDetailAction = DetailAction.Add;
        AddDataDetailCommandChecked = true;

        SelectedData.AddProjectTemplateDetail();
        SelectedDataDetail = SelectedData.ProjectTemplateDetails.LastOrDefault();
    }

    [RelayCommand]
    private void EditDataDetail()
    {
        ResetDataDetailCommandChecked();

        if (SelectedDataDetail != null)
        {
            _ProjectTemplateDetailAction = DetailAction.Edit;
        }

    }

    [RelayCommand]
    private void DeleteDataDetail()
    {
        ResetDataDetailCommandChecked();

        if (SelectedDataDetail != null)
        {
            SelectedData.ProjectTemplateDetails.Remove(SelectedDataDetail);
            SelectedDataDetail = SelectedData.ProjectTemplateDetails.FirstOrDefault();
        }
    }

    #endregion
}
