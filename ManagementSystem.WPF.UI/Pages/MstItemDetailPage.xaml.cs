
using ManagementSystem.WPF.UI.Pages.ToInherit;
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;

namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for MstItemDetailPage.xaml
    /// </summary>
    public partial class MstItemDetailPage : Page, IViewInteractionService, IDisposable
    {
        public void Dispose()
        {
            if (VM != null)
                VM.Dispose();

            SupplierCB_Dispose();
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
        public MstItemDetailViewModel VM { get; }

        private DetailAction _action;
        private int _itemId;
        public MstItemDetailPage(IManagementApiClient apiClient, IContentDialogService contentDialogService, JsonSerializerOptions jsonSerializerOptions, GlobalFunctions gFunctions)
        {
            InitializeComponent();

            VM = new MstItemDetailViewModel(apiClient, contentDialogService, this as IViewInteractionService, jsonSerializerOptions, gFunctions);

            DataContext = this;

            _messageDialog = new ContentDialog();
            _promptPage = new PromptPage(_messageDialog, this as IViewInteractionService);

            MessageFrame.Navigate(_promptPage);
        }

        public async Task InitializeAsync(DetailAction action, int itemId)
        {
            _action = action;
            _itemId = itemId;

            await VM.InitializeAsync(_action, _itemId);
            SupplierCB_Initialize();
        }

        #region ComboBox Supplier

        InstantSupplierComboBox _supplierCB;

        private void SupplierCB_Initialize()
        {
            _supplierCB = new InstantSupplierComboBox(ref SupplierComboBox, VM.SupplierCBVM);
        }

        private void SupplierCB_Dispose()
        {
            if (_supplierCB != null)
                _supplierCB.Dispose();
        }
        //private void SupplierComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine("SupplierComboBox_PreviewKeyDown: " + SupplierComboBox.Text + " = " + SupplierComboBox.IsDropDownOpen.ToString() + " - " + e.Key.ToString());

        //    if (e.Key == Key.Enter || e.Key == Key.Return)
        //    {
        //        e.Handled = true;
        //    }
        //}

        //private void SupplierComboBox_KeyUp(object sender, KeyEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine("SupplierComboBox_KeyUp: " + SupplierComboBox.Text + " = " + SupplierComboBox.IsDropDownOpen.ToString() + " - " + e.Key.ToString());

        //    if (e.Key == Key.Enter || e.Key == Key.Return)
        //    {
        //        if (VM.SelectedSupplier != null && VM.SelectedSupplier.SupplierName == SupplierComboBox.Text)
        //        {
        //            // Do Nothing
        //        }
        //        else if (SupplierComboBox.Text.Length >= 3)
        //        {
        //            VM.FilterSupplierText = SupplierComboBox.Text;

        //            if (VM.SupplierSelectedIndex == -1)
        //                if (!SupplierComboBox.IsDropDownOpen)
        //                {
        //                    SupplierComboBox.IsDropDownOpen = true;
        //                    SupplierComboBox.SelectedIndex = 0;
        //                }
        //        }   
        //        e.Handled = true;
        //    }

        //}

        #endregion

    }
}
