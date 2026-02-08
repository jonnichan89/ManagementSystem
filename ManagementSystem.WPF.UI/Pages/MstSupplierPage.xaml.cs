
namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for MstSupplierPage.xaml
    /// </summary>
    public partial class MstSupplierPage : Page, INavigationAware
    {
        //INavigationService navigationService,
        //IServiceProvider serviceProvider,
        //ISnackbarService snackbarService,
        //IContentDialogService contentDialogService
        public MstSupplierPage(
            MstSupplierViewModel viewModel)
        {
            InitializeComponent();

            VM = viewModel;
            DataContext = this;

        }

        [Required]
        public MstSupplierViewModel VM { get; }

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
