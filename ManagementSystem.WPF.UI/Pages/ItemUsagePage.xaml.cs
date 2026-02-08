
namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for ItemUsagePage.xaml
    /// </summary>
    public partial class ItemUsagePage : Page, INavigationAware
    {
        //INavigationService navigationService,
        //IServiceProvider serviceProvider,
        //ISnackbarService snackbarService,
        //IContentDialogService contentDialogService
        public ItemUsagePage(
            ItemUsageViewModel viewModel)
        {
            InitializeComponent();

            VM = viewModel;
            DataContext = this;

        }

        [Required]
        public ItemUsageViewModel VM { get; }

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
