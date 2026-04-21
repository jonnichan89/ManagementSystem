using ManagementSystem.WPF.UI.Pages.ToInherit;
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using Microsoft.SqlServer.Server;
using System.Diagnostics.Metrics;
using Wpf.Ui.Controls;

namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for FinanceDetailPage.xaml
    /// </summary>
    public partial class FinanceDetailPage : Page, IViewInteractionService, IDisposable
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
        public FinanceDetailViewModel VM { get; }

        private DetailAction _action;
        private int _FinanceId;
        public FinanceDetailPage(IManagementApiClient apiClient, IContentDialogService contentDialogService, JsonSerializerOptions jsonSerializerOptions, GlobalFunctions gFunctions)
        {
            InitializeComponent();

            VM = new FinanceDetailViewModel(apiClient, contentDialogService, this as IViewInteractionService, jsonSerializerOptions, gFunctions);

            DataContext = this;

            _messageDialog = new ContentDialog();
            _promptPage = new PromptPage(_messageDialog, this as IViewInteractionService);

            // NumberBox IDR
            ValidateNumberFormatter1 formatterIDR = new ValidateNumberFormatter1("IDR");
            SetNumberFormatter(HeaderTotalDPPIDRNUD, formatterIDR);
            SetNumberFormatter(HeaderDPPDiscountIDRNUD, formatterIDR);
            SetNumberFormatter(HeaderTotalAmountIDRNUD, formatterIDR);

            MessageFrame.Navigate(_promptPage);
        }

        public async Task InitializeAsync(DetailAction action, int financeId)
        {
            _action = action;
            _FinanceId = financeId;

            await VM.InitializeAsync(_action, _FinanceId);
            SupplierCB_Initialize();
            CBPSOrder_Initialize();
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

        #endregion

        #region ComboBox PSOrder

        InstantComboBoxPSOrder _CBPSOrder;

        private void CBPSOrder_Initialize()
        {
            _CBPSOrder = new InstantComboBoxPSOrder(ref PSOrderComboBox, VM.CBVMPSOrder);
        }

        private void CBPSOrder_Dispose()
        {
            if (_CBPSOrder != null)
                _CBPSOrder.Dispose();
        }

        #endregion

        #region Numeric Formatter

        ValidateNumberFormatter1 formatter;
        ValidateNumberFormatter1 paidFormatter;

        private void SetNumberFormatter(NumberBox numberBox, ValidateNumberFormatter1 formatter)
        {
            numberBox.NumberFormatter = formatter;
            numberBox.InvalidateProperty(NumberBox.ValueProperty);
            numberBox.InvalidateProperty(NumberBox.TextProperty);
        }

        private void FormatNumericDisplay()
        {
            if (formatter == null)
            {
                formatter = new ValidateNumberFormatter1(VM.Data.CurrencyId);
            }
            formatter.CurrencyId = VM.Data.CurrencyId;

            // Header
            SetNumberFormatter(HeaderTotalDPPNUD, formatter);
            SetNumberFormatter(HeaderDPPDiscountNUD, formatter);
            SetNumberFormatter(HeaderTotalAmountNUD, formatter);
        }

        private void CurrencyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            FormatNumericDisplay();
        }

        private void FormatPaidNumericDisplay()
        {
            if (paidFormatter == null)
            {
                paidFormatter = new ValidateNumberFormatter1(VM.SelectedFinanceDetail.PaidCurrencyId);
            }
            paidFormatter.CurrencyId = VM.SelectedFinanceDetail.PaidCurrencyId;

            // Detail
            SetNumberFormatter(PaidCurrencyRateNUD, formatter);
            SetNumberFormatter(PaidAmountNUD, formatter);
        }

        private void PaidCurrencyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            FormatPaidNumericDisplay();
        }

        #endregion
    }
}
