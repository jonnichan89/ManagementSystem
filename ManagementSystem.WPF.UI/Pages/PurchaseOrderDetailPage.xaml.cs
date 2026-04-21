using ManagementSystem.WPF.UI.Pages.ToInherit;
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using Microsoft.SqlServer.Server;
using System.Diagnostics.Metrics;
using Wpf.Ui.Controls;

namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for PurchaseOrderDetailPage.xaml
    /// </summary>
    public partial class PurchaseOrderDetailPage : Page, IViewInteractionService, IDisposable
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
        public PurchaseOrderDetailViewModel VM { get; }

        private DetailAction _action;
        private int _POId;
        public PurchaseOrderDetailPage(IManagementApiClient apiClient, IContentDialogService contentDialogService, JsonSerializerOptions jsonSerializerOptions, GlobalFunctions gFunctions)
        {
            InitializeComponent();

            VM = new PurchaseOrderDetailViewModel(apiClient, contentDialogService, this as IViewInteractionService, jsonSerializerOptions, gFunctions);

            DataContext = this;

            _messageDialog = new ContentDialog();
            _promptPage = new PromptPage(_messageDialog, this as IViewInteractionService);

            MessageFrame.Navigate(_promptPage);
        }

        public async Task InitializeAsync(DetailAction action, int POId)
        {
            _action = action;
            _POId = POId;

            await VM.InitializeAsync(_action, _POId);
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

        #endregion

        #region Numeric Formatter

        public class ValidateNumberFormatter1 : INumberFormatter, INumberParser
        {
            public ValidateNumberFormatter1(string currencyId)
            {
                CurrencyId = currencyId;
            }

            private string _stringFormat;
            private string _currencyId;
            public string CurrencyId
            {
                set
                {
                    _currencyId = value;
                    switch (_currencyId)
                    {
                        case "IDR":
                            _stringFormat = "N0";
                            break;
                        default:
                            _stringFormat = "N2";
                            break;
                    }
                }
            }

            /// <inheritdoc />
            public string FormatDouble(double? value)
            {
                return value?.ToString(_stringFormat, GetCurrentCultureConverter()) ?? string.Empty;
            }

            /// <inheritdoc />
            public string FormatInt(int? value)
            {
                return value?.ToString(_stringFormat, GetCurrentCultureConverter()) ?? string.Empty;
            }

            /// <inheritdoc />
            public string FormatUInt(uint? value)
            {
                return value?.ToString(_stringFormat, GetCurrentCultureConverter()) ?? string.Empty;
            }

            /// <inheritdoc />
            public double? ParseDouble(string? value)
            {
                return double.TryParse(value, out double d) ? d : null;
            }

            /// <inheritdoc />
            public int? ParseInt(string? value)
            {
                return int.TryParse(value, out int i) ? i : null;
            }

            /// <inheritdoc />
            public uint? ParseUInt(string? value)
            {
                return uint.TryParse(value, out uint ui) ? ui : (uint?)null;
            }

            private static CultureInfo GetCurrentCultureConverter()
            {
                return CultureInfo.CurrentCulture;
            }
        }

        ValidateNumberFormatter1 formatter;

        private void FormatNumericDisplay()
        {
            if (formatter == null)
            {
                formatter = new ValidateNumberFormatter1(VM.Data.CurrencyId);
            }
            formatter.CurrencyId = VM.Data.CurrencyId;
            DPPNUD.NumberFormatter = formatter;
            HeaderTotalDPPNUD.NumberFormatter = formatter;
            HeaderDPPDiscountNUD.NumberFormatter = formatter;
            HeaderTotalDPPNUD.InvalidateProperty(NumberBox.ValueProperty);
            HeaderTotalDPPNUD.InvalidateProperty(NumberBox.TextProperty);
        }
        private void CurrencyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            FormatNumericDisplay();
        }

        #endregion

        private void ItemComboBox_OnItemHoverExit(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("mm:ss.fff") + "OnItemHoverExit");
            if (DataContext is PurchaseOrderDetailViewModel vm)
            {
                vm.PreviewImagePath = vm._selectedItem?.ImagePath ?? string.Empty;
            }
        }

        MstItem _lastItem;
        private void ItemComboBox_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("mm:ss.fff") + "OnPreviewMouseMove");
            var item = (sender as ComboBoxItem)?.DataContext as MstItem;
            if (item != null && _lastItem != item)
            {
                _lastItem = item;
                VM.PreviewImagePath = item.ImagePath; // Update the external image
            }
        }
        private void MyComboBox_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                // Try to directly access the UI container for the items
                // Note: This needs the visual elements to be generated first.
                comboBox.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, new Action(() =>
                {
                    if (comboBox.ItemContainerGenerator.ContainerFromItem(comboBox.SelectedItem) is ComboBoxItem item)
                    {
                        item.BringIntoView();
                    }
                }));
            }
        }

        private void ItemComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = ItemComboBox.Text;
                VM.FilterText = text;
                e.Handled = true;

                if (text != VM.FilterText)
                {
                    ItemComboBox.IsDropDownOpen = true;
                }
            }
        }

        private void ItemComboBox_NUD_KeyUp(object sender, KeyEventArgs e)
        {
            if (!ItemComboBox.IsDropDownOpen)
            {
                if (e.Key == Key.PageUp)
                {
                    if (ItemDetailDataGrid.SelectedIndex == 0)
                        ItemDetailDataGrid.SelectedIndex = ItemDetailDataGrid.Items.Count - 1;
                    else
                        ItemDetailDataGrid.SelectedIndex -= 1;

                    ItemComboBox.Focus();
                    e.Handled = true;
                }
                else if (e.Key == Key.PageDown)
                {
                    if (ItemDetailDataGrid.SelectedIndex == ItemDetailDataGrid.Items.Count - 1)
                        ItemDetailDataGrid.SelectedIndex = 0;
                    else
                        ItemDetailDataGrid.SelectedIndex += 1;

                    ItemComboBox.Focus();
                    e.Handled = true;
                }
                else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
                {
                    VM.AddItemDetailCommand.Execute(null);
                    ItemComboBox.Focus();
                    e.Handled = true;
                }
            }
        }
    }
}
