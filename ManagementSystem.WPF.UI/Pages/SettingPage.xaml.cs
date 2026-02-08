
namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class SettingPage : Page
    {
        //INavigationService navigationService,
        //IServiceProvider serviceProvider,
        //ISnackbarService snackbarService,
        //IContentDialogService contentDialogService
        public SettingPage(
            SettingViewModel viewModel)
        {
            VM = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        [Required]
        public SettingViewModel VM { get; }

    }
}
