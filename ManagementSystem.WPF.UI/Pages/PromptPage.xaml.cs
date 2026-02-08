
namespace ManagementSystem.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for PromptPage.xaml
    /// </summary>
    public partial class PromptPage : Page
    {
        ContentDialog _contentDialog;
        IViewInteractionService _interactionService;

        public PromptPage(ContentDialog cd, IViewInteractionService vis)
        {
            InitializeComponent();

            _interactionService = vis;

            _contentDialog = cd;
            _contentDialog.SetValue(ContentDialog.TemplateButtonCommandProperty, new Wpf.Ui.Input.RelayCommand<ContentDialogButton>(onTemplateButtonCommandExecute));

            this.DataContext = _contentDialog;
        }

        private void onTemplateButtonCommandExecute(ContentDialogButton button)
        {
            _interactionService.onPromptFeedback(button);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == "IsVisible" && IsVisible)
            {
                if (!_contentDialog.IsPrimaryButtonEnabled)
                {
                    PrimaryColumn.Width = new GridLength(0);
                    FirstSpacer.Width = new GridLength(0);
                }
                else
                {
                    PrimaryColumn.Width = new GridLength(1, GridUnitType.Star);
                    FirstSpacer.Width = new GridLength(8);
                }

                if (!_contentDialog.IsSecondaryButtonEnabled)
                {
                    SecondaryColumn.Width = new GridLength(0);
                    SecondSpacer.Width = new GridLength(0);
                }
                else
                {
                    SecondaryColumn.Width = new GridLength(1, GridUnitType.Star);
                    SecondSpacer.Width = new GridLength(8);
                }
            }
        }
    }
}
