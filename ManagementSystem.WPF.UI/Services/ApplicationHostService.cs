
namespace ManagementSystem.WPF.UI.Services
{
    /// <summary>
    /// Managed host of the application.
    /// </summary>
    public class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public ApplicationHostService(IServiceProvider serviceProvider)
        {
            // If you want, you can do something with these services at the beginning of loading the application.
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return HandleActivationAsync();
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates main window during activation.
        /// </summary>
        private Task HandleActivationAsync()
        {
            if (Application.Current.Windows.OfType<MainWindow>().Any())
            {
                return Task.CompletedTask;
            }

            IWindow mainWindow = _serviceProvider.GetRequiredService<IWindow>();
            mainWindow.Loaded += OnMainWindowLoaded;
            mainWindow?.Show();

            return Task.CompletedTask;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is not MainWindow mainWindow)
            {
                return;
            }

            _ = mainWindow.NavigationView.Navigate(typeof(DashboardPage));
        }
    }
    public interface IWindow
    {
        event RoutedEventHandler Loaded;

        void Show();
    }
    public class WindowsProviderService
    {
        private readonly IServiceProvider _serviceProvider;

        public WindowsProviderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Show<T>()
            where T : class
        {
            if (!typeof(Window).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException($"The window class should be derived from {typeof(Window)}.");
            }

            Window windowInstance =
                _serviceProvider.GetService<T>() as Window
                ?? throw new InvalidOperationException("Window is not registered as service.");
            windowInstance.Owner = Application.Current.MainWindow;
            windowInstance.Show();
        }
    }
}
