
namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for ProjectTemplatePage.xaml
    /// </summary>
    public partial class ProjectTemplatePage : Page, INavigationAware
    {
        //INavigationService navigationService,
        //IServiceProvider serviceProvider,
        //ISnackbarService snackbarService,
        //IContentDialogService contentDialogService
        public ProjectTemplatePage(
            ProjectTemplateViewModel viewModel)
        {
            InitializeComponent();

            VM = viewModel;
            DataContext = this;

        }

        [Required]
        public ProjectTemplateViewModel VM { get; }

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
