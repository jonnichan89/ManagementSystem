
namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for ProjectSalesOrderPage.xaml
    /// </summary>
    public partial class ProjectSalesOrderPage : Page, INavigationAware
    {
        //INavigationService navigationService,
        //IServiceProvider serviceProvider,
        //ISnackbarService snackbarService,
        //IContentDialogService contentDialogService
        public ProjectSalesOrderPage(
            ProjectSalesOrderViewModel viewModel)
        {
            InitializeComponent();

            VM = viewModel;
            DataContext = this;

        }

        [Required]
        public ProjectSalesOrderViewModel VM { get; }

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
