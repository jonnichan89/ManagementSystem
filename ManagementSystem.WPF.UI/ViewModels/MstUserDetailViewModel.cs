
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using ManagementSystem.WPF.UI.ViewModels.ToInherit;

namespace ManagementSystem.WPF.UI.ViewModels;

public partial class MstUserDetailViewModel : DetailViewModel
{
    [ObservableProperty]
    public string Title { get; private set; }

    [ObservableProperty]
    public bool UserIdIsReadOnly
    {
        get
        {
            return _action != DetailAction.Add;
        }
    }

    IContentDialogService _contentDialogService;
    IViewInteractionService _viewInteractionService;
    DetailViewModelDoAction _promptAction;
    IManagementApiClient _apiClient;
    JsonSerializerOptions _jsonOptions;
    public MstUserDetailViewModel(
        IManagementApiClient apiClient,
        IContentDialogService contentDialogService,
        IViewInteractionService viewInteractionService,
        JsonSerializerOptions jsonOptions)
    {
        _apiClient = apiClient;
        _contentDialogService = contentDialogService;
        _viewInteractionService = viewInteractionService;
        _viewInteractionService.PromptFeedback += _viewInteractionService_PromptFeedback;
        _jsonOptions = jsonOptions;

        ChangeLanguage();
    }

    public override async Task InitializeAsync(DetailAction action, object pk)
    {
        string userId = pk.ToString();

        base._action = action;

        switch (action)
        {
            case DetailAction.ViewDetail:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleView"] ?? string.Empty,
                    TranslationSource.Instance["mwUser"]);
                break;

            case DetailAction.Add:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleAdd"] ?? string.Empty,
                    TranslationSource.Instance["mwUser"]);
                Data = new MstUser()
                {
                    UserId = "",
                    UserName = "",
                    UserPassword = "",
                    Salt = "",
                    IsBlacklist = false,
                    AccessList = "",
                    CreatedAt = DateTime.Now,
                    CreatedBy = "",
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = ""
                };
                break;

            case DetailAction.Edit:
                Title = string.Format(
                    TranslationSource.Instance["detailTitleEdit"] ?? string.Empty,
                    TranslationSource.Instance["mwUser"]);

                ServiceActionResult sar = await _apiClient.GetMstUserByUserNameAsync(userId);
                if (sar.ActionIsSuccess)
                {
                    Data = (MstUser)sar.ActionResult;

                    if (string.IsNullOrWhiteSpace(Data.AccessList))
                    {
                        foreach (var userModule in _userModules)
                        {
                            userModule.CanAll = false;
                        }
                        break;
                    }
                    else
                    {
                        UserModule? dataModule;
                        UserModule[] dataModules = JsonSerializer.Deserialize<UserModule[]>(
                            Data.AccessList ?? "[]",
                            new JsonSerializerOptions()
                            {
                                PropertyNameCaseInsensitive = true
                            }
                            ) ?? Array.Empty<UserModule>();

                        foreach (var userModule in _userModules)
                        {
                            userModule.CanAll = false;

                            dataModule = dataModules.FirstOrDefault(um => um.ModuleId == userModule.ModuleId);
                            if (dataModule != null)
                            {
                                if (dataModule.CanAll)
                                    userModule.CanAll = true;
                                else
                                {
                                    userModule.CanViewList = dataModule.CanViewList;
                                    userModule.CanViewDetail = dataModule.CanViewDetail;
                                    userModule.CanAdd = dataModule.CanAdd;
                                    userModule.CanEdit = dataModule.CanEdit;
                                    userModule.CanDelete = dataModule.CanDelete;
                                }
                            }
                        }
                    }
                }
                break;

            default:
                break;
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
        // Example of changing language for UserModule and UserPermission
        foreach (var module in UserModules)
        {
            module.ModuleName = TranslationSource.Instance[module.ModuleId] ?? string.Empty; // Localize module name
        }
    }

    [ObservableProperty]
    private MstUser _data;

    [ObservableProperty]
    private ObservableRangeCollection<UserModule> _userModules = new()
    {
        new UserModule()
        {
            ModuleId = "mwMstUser",
            ModuleName = "",
            EnableViewList = true,
            EnableViewDetail = false,
            EnableAdd = true,
            EnableEdit = true,
            EnableDelete = true,
            IsInitializing = false,
        },
        new UserModule()
        {
            ModuleId = "mwMstSupplier",
            ModuleName = "",
            EnableViewList = true,
            EnableViewDetail = true,
            EnableAdd = true,
            EnableEdit = true,
            EnableDelete = true,
            IsInitializing = false,
        },
        new UserModule()
        {
            ModuleId = "mwMstItem",
            ModuleName = "",
            EnableViewList = true,
            EnableViewDetail = true,
            EnableAdd = true,
            EnableEdit = true,
            EnableDelete = true,
            IsInitializing = false,
        },
        new UserModule()
        {
            ModuleId = "mwMstAccount",
            ModuleName = "",
            EnableViewList = true,
            EnableViewDetail = true,
            EnableAdd = true,
            EnableEdit = true,
            EnableDelete = true,
            IsInitializing = false,
        },
        new UserModule()
        {
            ModuleId = "mwProjectTemplate",
            ModuleName = "",
            EnableViewList = true,
            EnableViewDetail = true,
            EnableAdd = true,
            EnableEdit = true,
            EnableDelete = true,
            IsInitializing = false,
        },
        new UserModule()
        {
            ModuleId = "mwItemAdjustment",
            ModuleName = "",
            EnableViewList = true,
            EnableViewDetail = true,
            EnableAdd = true,
            EnableEdit = true,
            EnableDelete = true,
            IsInitializing = false,
        },
        new UserModule()
        {
            ModuleId = "mwItemUsage",
            ModuleName = "",
            EnableViewList = true,
            EnableViewDetail = true,
            EnableAdd = true,
            EnableEdit = true,
            EnableDelete = true,
            IsInitializing = false,
        },
        new UserModule()
        {
            ModuleId = "mwPurchase",
            ModuleName = "",
            EnableViewList = true,
            EnableViewDetail = true,
            EnableAdd = true,
            EnableEdit = true,
            EnableDelete = true,
            IsInitializing = false,
        },
        new UserModule()
        {
            ModuleId = "mwSales",
            ModuleName = "",
            EnableViewList = true,
            EnableViewDetail = true,
            EnableAdd = true,
            EnableEdit = true,
            EnableDelete = true,
            IsInitializing = false,
        },
        new UserModule()
        {
            ModuleId = "mwProjectPurchaseOrder",
            ModuleName = "",
            EnableViewList = true,
            EnableViewDetail = true,
            EnableAdd = true,
            EnableEdit = true,
            EnableDelete = true,
            IsInitializing = false,
        },
        new UserModule()
        {
            ModuleId = "mwProjectSalesOrder",
            ModuleName = "",
            EnableViewList = true,
            EnableViewDetail = true,
            EnableAdd = true,
            EnableEdit = true,
            EnableDelete = true,
            IsInitializing = false,
        },
        new UserModule()
        {
            ModuleId = "mwFinancial",
            ModuleName = "",
            EnableViewList = true,
            EnableViewDetail = true,
            EnableAdd = true,
            EnableEdit = true,
            EnableDelete = true,
            IsInitializing = false,
        }
    };

    public override void TrySavePrimary()
    {
        _promptAction = DetailViewModelDoAction.SavePrimary;

        bool absoluteCannotSave = false;
        string message = "";

        if (string.IsNullOrWhiteSpace(_data.UserId))
        {
            message +=
                string.Format(
                    TranslationSource.Instance["messageFieldIsRequired"] ?? string.Empty,
                    TranslationSource.Instance["propUserId"]) +
                Environment.NewLine;

            absoluteCannotSave = true;
        }

        if (string.IsNullOrWhiteSpace(_data.UserName))
        {
            message +=
                string.Format(
                    TranslationSource.Instance["messageFieldIsRequired"] ?? string.Empty,
                    TranslationSource.Instance["propUserName"]) +
                Environment.NewLine;

            absoluteCannotSave = true;
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
            _data.AccessList = JsonSerializer.Serialize<UserModule[]>(_userModules.ToArray());
            ServiceActionResult result;
            switch (_action)
            {
                case DetailAction.Add:
                    result = await _apiClient.CreateMstUserAsync(_data);
                    break;

                case DetailAction.Edit:
                    result = await _apiClient.UpdateMstUserAsync(_data);
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
                                    _data.UserId);

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

}

public partial class UserModule : ObservableObject
{
    protected internal bool IsInitializing { get; set; } = true;

    public string ModuleId { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _canAll = false;

    [ObservableProperty]
    private bool _canViewList = false;
    [ObservableProperty]
    private bool _canViewDetail = false;
    [ObservableProperty]
    private bool _canAdd = false;
    [ObservableProperty]
    private bool _canEdit = false;
    [ObservableProperty]
    private bool _canDelete = false;

    [ObservableProperty]
    private bool _enableViewList = false;
    [ObservableProperty]
    private bool _enableViewDetail = false;
    [ObservableProperty]
    private bool _enableAdd = false;
    [ObservableProperty]
    private bool _enableEdit = false;
    [ObservableProperty]
    private bool _enableDelete = false;


    bool onCanPropertyChanged;
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (IsInitializing)
        {
            return;
        }

        base.OnPropertyChanged(e);

        if (onCanPropertyChanged) return;

        switch (e.PropertyName)
        {
            case nameof(CanAll):
                onCanPropertyChanged = true;

                if (_enableViewList) CanViewList = _canAll;
                if (_enableViewDetail) CanViewDetail = _canAll;
                if (_enableAdd) CanAdd = _canAll;
                if (_enableEdit) CanEdit = _canAll;
                if (_enableDelete) CanDelete = _canAll;

                onCanPropertyChanged = false;

                break;
            default:
                if (e.PropertyName.StartsWith("Can"))
                {
                    onCanPropertyChanged = true;

                    bool canAll = true;
                    Dictionary<string, object> canProp = ObjectHelper.CollectPropertiesStartingWith(this, "Can");
                    foreach (KeyValuePair<string, object> prop in canProp)
                    {
                        if (prop.Key == "CanAll") continue;

                        if (!(bool)prop.Value)
                        {
                            canAll = false;
                            break;
                        }
                    }

                    CanAll = canAll;
                    onCanPropertyChanged = false;
                }
                break;
        }
    }
}
