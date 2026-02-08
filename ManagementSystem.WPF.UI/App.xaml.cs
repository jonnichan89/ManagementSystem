
using System.Text.Json.Serialization;

namespace ManagementSystem.WPF.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static readonly IHost _host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(c =>
    {
        _ = c.SetBasePath(AppContext.BaseDirectory);
    })
    .ConfigureServices(
        (_1, services) =>
        {
            _ = services.AddNavigationViewPageProvider();

            // App Host
            _ = services.AddHostedService<ApplicationHostService>();

            // Main window container with navigation
            _ = services.AddSingleton<IWindow, MainWindow>();
            _ = services.AddSingleton<MainWindowViewModel>();
            _ = services.AddSingleton<INavigationService, Wpf.Ui.NavigationService>();
            _ = services.AddSingleton<ISnackbarService, SnackbarService>();
            _ = services.AddSingleton<IContentDialogService, ContentDialogService>();
            _ = services.AddSingleton<WindowsProviderService>();
            _ = services.AddSingleton<GlobalFunctions>();

            // Top-level pages
            _ = services.AddSingleton<DashboardPage>();
            //_ = services.AddSingleton<DashboardViewModel>();
            _ = services.AddSingleton<MstUserPage>();
            _ = services.AddSingleton<MstUserViewModel>();
            _ = services.AddTransient<MstUserDetailPage>();

            _ = services.AddSingleton<MstItemPage>();
            _ = services.AddSingleton<MstItemViewModel>();
            _ = services.AddTransient<MstItemDetailPage>();

            _ = services.AddSingleton<MstSupplierPage>();
            _ = services.AddSingleton<MstSupplierViewModel>();
            _ = services.AddTransient<MstSupplierDetailPage>();

            _ = services.AddSingleton<ItemUsagePage>();
            _ = services.AddSingleton<ItemUsageViewModel>();
            _ = services.AddTransient<ItemUsageDetailPage>();

            _ = services.AddSingleton<PurchaseOrderPage>();
            _ = services.AddSingleton<PurchaseOrderViewModel>();
            _ = services.AddTransient<PurchaseOrderDetailPage>();

            _ = services.AddSingleton<SettingPage>();
            _ = services.AddSingleton<SettingViewModel>();

            //// All other pages and view models
            //_ = services.AddTransientFromNamespace("Wpf.Ui.Gallery.Views", GalleryAssembly.Asssembly);
            //_ = services.AddTransientFromNamespace(
            //    "Wpf.Ui.Gallery.ViewModels",
            //    GalleryAssembly.Asssembly
            //);

            _ = services.AddStringLocalizer(b =>
            {
                b.FromResource<StringResource>(new("en-US"));
                b.FromResource<StringResource>(new("id-ID"));
            });

            _ = services.AddManagementApiClient(new Uri("http://localhost/ManagementSystem/"));

            // Configure default JsonSerializerOptions
            //PropertyNameCaseInsensitive = true,
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = null,
            };

            // Register the options instance in the DI container
            services.AddSingleton(jsonOptions);
        }
    )
    .Build();

    public static T GetRequiredService<T>()
        where T : class
    {
        return _host.Services.GetRequiredService<T>();
    }
    private void OnStartup(object sender, StartupEventArgs e)
    {
        _host.Start();
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        _host.StopAsync().Wait();

        _host.Dispose();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
    }
}
