
namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for MstItemPage.xaml
    /// </summary>
    public partial class MstItemPage : Page, INavigationAware
    {
        //INavigationService navigationService,
        //IServiceProvider serviceProvider,
        //ISnackbarService snackbarService,
        //IContentDialogService contentDialogService
        public MstItemPage(
            MstItemViewModel viewModel)
        {
            InitializeComponent();

            VM = viewModel;
            DataContext = this;

        }

        [Required]
        public MstItemViewModel VM { get; }

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
