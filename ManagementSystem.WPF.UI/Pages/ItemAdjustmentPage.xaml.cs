
namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for ItemAdjustmentPage.xaml
    /// </summary>
    public partial class ItemAdjustmentPage : Page, INavigationAware
    {
        //INavigationService navigationService,
        //IServiceProvider serviceProvider,
        //ISnackbarService snackbarService,
        //IContentDialogService contentDialogService
        public ItemAdjustmentPage(
            ItemAdjustmentViewModel viewModel)
        {
            InitializeComponent();

            VM = viewModel;
            DataContext = this;

        }

        [Required]
        public ItemAdjustmentViewModel VM { get; }

        public Task OnNavigatedFromAsync()
        {
            return Task.CompletedTask;
        }

        public async Task OnNavigatedToAsync()
        {
            await VM.Initialize();
        }
    }
}
