
namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for ProjectPurchaseOrderPage.xaml
    /// </summary>
    public partial class ProjectPurchaseOrderPage : Page, INavigationAware
    {
        //INavigationService navigationService,
        //IServiceProvider serviceProvider,
        //ISnackbarService snackbarService,
        //IContentDialogService contentDialogService
        public ProjectPurchaseOrderPage(
            ProjectPurchaseOrderViewModel viewModel)
        {
            InitializeComponent();

            VM = viewModel;
            DataContext = this;

        }

        [Required]
        public ProjectPurchaseOrderViewModel VM { get; }

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
