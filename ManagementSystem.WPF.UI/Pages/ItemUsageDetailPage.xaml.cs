
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using ManagementSystem.WPF.UI.ViewModels.ToInherit;

namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for ItemUsageDetailPage.xaml
    /// </summary>
    public partial class ItemUsageDetailPage : Page, IViewInteractionService, IDisposable
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
        public ItemUsageDetailViewModel VM { get; }

        private DetailAction _action;
        private int _itemUsageId;
        public ItemUsageDetailPage(IManagementApiClient apiClient, IContentDialogService contentDialogService, JsonSerializerOptions jsonSerializerOptions, GlobalFunctions gFunctions)
        {
            InitializeComponent();

            VM = new ItemUsageDetailViewModel(apiClient, contentDialogService, this as IViewInteractionService, jsonSerializerOptions, gFunctions);

            DataContext = this;

            _messageDialog = new ContentDialog();
            _promptPage = new PromptPage(_messageDialog, this as IViewInteractionService);

            MessageFrame.Navigate(_promptPage);
        }

        public async Task InitializeAsync(DetailAction action, int itemUsageId)
        {
            _action = action;
            _itemUsageId = itemUsageId;

            await VM.InitializeAsync(_action, _itemUsageId);
        }

        private void ItemComboBox_OnItemHoverExit(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("mm:ss.fff") + "OnItemHoverExit");
            if (DataContext is ItemUsageDetailViewModel vm)
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

        private void ItemComboBox_QtyNUD_KeyUp(object sender, KeyEventArgs e)
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
                    VM.AddItemUsageDetailCommand.Execute(null);
                    ItemComboBox.Focus();
                    e.Handled = true;
                }
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

    }
}
