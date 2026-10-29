

using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using ManagementSystem.WPF.UI.ViewModels.ToInherit;

namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for MstUserDetailPage.xaml
    /// </summary>
    public partial class MstUserDetailPage : Page, IViewInteractionService, IDisposable
    {
        public void Dispose()
        {
            if (VM != null)
                VM.Dispose();
        }

        #region IViewInteractionService Implementation

        private bool _previousPrimaryButtonVisible;

        private ContentDialog _messageDialog;
        private PromptPage _promptPage;

        public PromptUsedFor PromptUsedFor { get; set; }

        public event EventHandler<MyContentDialogButtonClickEventArgs> PromptFeedback;

        public void Prompt(PromptUsedFor usedFor, string title, string message)
        {
            PromptUsedFor = usedFor;
            _previousPrimaryButtonVisible = DetailContentDialog.IsPrimaryButtonVisible;

            _messageDialog.Title = title;
            _messageDialog.Content = message;
            _messageDialog.DefaultButton = ContentDialogButton.Close;

            switch (usedFor)
            {
                case PromptUsedFor.QuestionBeforeSaveDetail:
                    _messageDialog.CloseButtonText = TranslationSource.Instance["dialogButtonCancel"];
                    _messageDialog.PrimaryButtonText = TranslationSource.Instance["dialogButtonSave"];

                    DetailContentDialog.IsPrimaryButtonVisible = false;
                    break;

                case PromptUsedFor.MessageDetailSaveSuccess:
                case PromptUsedFor.MessageDetailSaveFailed:
                    _messageDialog.CloseButtonText = TranslationSource.Instance["dialogButtonOK"];
                    _messageDialog.PrimaryButtonText = "";

                    DetailContentDialog.IsPrimaryButtonVisible = false;
                    break;

                default:
                    DetailContentDialog.IsPrimaryButtonVisible = _previousPrimaryButtonVisible;
                    break;
            }

            InputContainer.IsEnabled = false;
            MessageFrame.Visibility = Visibility.Visible;
            MessageBox.BringIntoView();
        }

        public void onPromptFeedback(ContentDialogButton button)
        {
            DetailContentDialog.IsPrimaryButtonVisible = _previousPrimaryButtonVisible;

            InputContainer.IsEnabled = true;
            MessageFrame.Visibility = Visibility.Collapsed;
            PromptFeedback?.Invoke(this, new MyContentDialogButtonClickEventArgs(button));
        }

        public void Close(ContentDialogResult result)
        {
            DetailContentDialog.Close(result);
        }

        #endregion

        private DetailContentDialog _detailContentDialog;
        private Frame _frame;

        private DetailContentDialog DetailContentDialog
        {
            get
            {
                if (_detailContentDialog == null)
                {
                    _detailContentDialog = (DetailContentDialog)_frame.Parent;
                }
                return _detailContentDialog;
            }
        }

        public Frame Frame
        {
            get => _frame;
            set
            {
                _frame = value;
                _detailContentDialog = (DetailContentDialog)_frame.Parent;
            }
        }

        [Required]
        public MstUserDetailViewModel VM { get; }

        private DetailAction _action;
        private string _userId;
        public MstUserDetailPage(IManagementApiClient apiClient, IContentDialogService contentDialogService, JsonSerializerOptions jsonSerializerOptions)
        {
            InitializeComponent();

            VM = new MstUserDetailViewModel(apiClient, contentDialogService, this as IViewInteractionService, jsonSerializerOptions);

            DataContext = this;

            _messageDialog = new ContentDialog();
            _promptPage = new PromptPage(_messageDialog, this as IViewInteractionService);

            MessageFrame.Navigate(_promptPage);
        }

        public async Task InitializeAsync(DetailAction action, string userId)
        {
            _action = action;
            _userId = userId;

            await VM.InitializeAsync(_action, _userId);
        }

    }
}
