using ManagementSystem.WPF.UI.Pages.ToInherit;
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using ManagementSystem.WPF.UI.ViewModels.ToInherit;
using Microsoft.SqlServer.Server;
using System.Diagnostics.Metrics;
using Wpf.Ui.Controls;

namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for ProjectTemplateDetailPage.xaml
    /// </summary>
    public partial class ProjectTemplateDetailPage : Page, IViewInteractionService, IDisposable
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
        public ProjectTemplateDetailViewModel VM { get; }

        private DetailAction _action;
        private int _PTId;
        public ProjectTemplateDetailPage(IManagementApiClient apiClient, IContentDialogService contentDialogService, JsonSerializerOptions jsonSerializerOptions, GlobalFunctions gFunctions)
        {
            InitializeComponent();

            VM = new ProjectTemplateDetailViewModel(apiClient, contentDialogService, this as IViewInteractionService, jsonSerializerOptions, gFunctions);

            DataContext = this;

            _messageDialog = new ContentDialog();
            _promptPage = new PromptPage(_messageDialog, this as IViewInteractionService);

            MessageFrame.Navigate(_promptPage);
        }

        public async Task InitializeAsync(DetailAction action, int PTId)
        {
            _action = action;
            _PTId = PTId;

            await VM.InitializeAsync(_action, _PTId);
        }

        private void ItemComboBox_OnItemHoverExit(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("mm:ss.fff") + "OnItemHoverExit");
            if (DataContext is ProjectTemplateDetailViewModel vm)
            {
                vm.DataDetailPreviewImagePath = vm._selectedItem?.ImagePath ?? string.Empty;
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
                VM.DataDetailPreviewImagePath = item.ImagePath; // Update the external image
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
                    if (DataDetailDataGrid.SelectedIndex == 0)
                        DataDetailDataGrid.SelectedIndex = DataDetailDataGrid.Items.Count - 1;
                    else
                        DataDetailDataGrid.SelectedIndex -= 1;

                    ItemComboBox.Focus();
                    e.Handled = true;
                }
                else if (e.Key == Key.PageDown)
                {
                    if (DataDetailDataGrid.SelectedIndex == DataDetailDataGrid.Items.Count - 1)
                        DataDetailDataGrid.SelectedIndex = 0;
                    else
                        DataDetailDataGrid.SelectedIndex += 1;

                    ItemComboBox.Focus();
                    e.Handled = true;
                }
                else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
                {
                    VM.AddDataDetailCommand.Execute(null);
                    ItemComboBox.Focus();
                    e.Handled = true;
                }
            }
        }
    }
}
