
namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for SalesOrderPage.xaml
    /// </summary>
    public partial class SalesOrderPage : Page, INavigationAware
    {
        //INavigationService navigationService,
        //IServiceProvider serviceProvider,
        //ISnackbarService snackbarService,
        //IContentDialogService contentDialogService
        public SalesOrderPage(
            SalesOrderViewModel viewModel)
        {
            InitializeComponent();

            VM = viewModel;
            DataContext = this;

        }

        [Required]
        public SalesOrderViewModel VM { get; }

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
