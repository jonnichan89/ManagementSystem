
namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for PurchaseOrderPage.xaml
    /// </summary>
    public partial class PurchaseOrderPage : Page, INavigationAware
    {
        //INavigationService navigationService,
        //IServiceProvider serviceProvider,
        //ISnackbarService snackbarService,
        //IContentDialogService contentDialogService
        public PurchaseOrderPage(
            PurchaseOrderViewModel viewModel)
        {
            InitializeComponent();

            VM = viewModel;
            DataContext = this;

        }

        [Required]
        public PurchaseOrderViewModel VM { get; }

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
