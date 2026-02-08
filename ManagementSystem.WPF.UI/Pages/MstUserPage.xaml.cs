
namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for MstUserPage.xaml
    /// </summary>
    public partial class MstUserPage : Page, INavigationAware
    {
        //INavigationService navigationService,
        //IServiceProvider serviceProvider,
        //ISnackbarService snackbarService,
        //IContentDialogService contentDialogService
        public MstUserPage(
            MstUserViewModel viewModel)
        {
            InitializeComponent();

            VM = viewModel;
            DataContext = this;

        }

        [Required]
        public MstUserViewModel VM { get; }

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
