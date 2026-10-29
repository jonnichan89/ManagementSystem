
using ManagementSystem.WPF.UI.Data.OtherModels;
using System.Text.Json.Serialization;

namespace ManagementSystem.WPF.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static string UserName { get; set; } = string.Empty;
    public static string AccessList { get; set; } = string.Empty;
    public static string AccessToken { get; set; } = string.Empty;

    public IServiceProvider ServiceProvider { get; private set; }

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
            _ = services.AddSingleton<SecureStorageService>();
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

            _ = services.AddSingleton<MstAccountPage>();
            _ = services.AddSingleton<MstAccountViewModel>();
            _ = services.AddTransient<MstAccountDetailPage>();

            _ = services.AddSingleton<ProjectTemplatePage>();
            _ = services.AddSingleton<ProjectTemplateViewModel>();
            _ = services.AddTransient<ProjectTemplateDetailPage>();

            _ = services.AddSingleton<ItemUsagePage>();
            _ = services.AddSingleton<ItemUsageViewModel>();
            _ = services.AddTransient<ItemUsageDetailPage>();

            _ = services.AddSingleton<ItemAdjustmentPage>();
            _ = services.AddSingleton<ItemAdjustmentViewModel>();
            _ = services.AddTransient<ItemAdjustmentDetailPage>();

            _ = services.AddSingleton<PurchaseOrderPage>();
            _ = services.AddSingleton<PurchaseOrderViewModel>();
            _ = services.AddTransient<PurchaseOrderDetailPage>();

            _ = services.AddSingleton<SalesOrderPage>();
            _ = services.AddSingleton<SalesOrderViewModel>();
            _ = services.AddTransient<SalesOrderDetailPage>();

            _ = services.AddSingleton<ProjectPurchaseOrderPage>();
            _ = services.AddSingleton<ProjectPurchaseOrderViewModel>();
            _ = services.AddTransient<ProjectPurchaseOrderDetailPage>();

            _ = services.AddSingleton<ProjectSalesOrderPage>();
            _ = services.AddSingleton<ProjectSalesOrderViewModel>();
            _ = services.AddTransient<ProjectSalesOrderDetailPage>();

            _ = services.AddSingleton<FinancePage>();
            _ = services.AddSingleton<FinanceViewModel>();
            _ = services.AddTransient<FinanceDetailPage>();

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

            _ = services.AddManagementApiClient(new Uri(ManagementSystem.WPF.UI.Properties.Settings.Default.APIUrl.TrimEnd('/') + "/"));

            // Configure default JsonSerializerOptions
            //PropertyNameCaseInsensitive = true,
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = null,
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

    private SplashWindow? _splash;

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        // Show lightweight splash
        try
        {

            //LoginDataResult data = JsonSerializer.Deserialize<LoginDataResult>(
            //    (actionResult.ActionResult ?? string.Empty).
            //    ToString() ?? string.Empty) ?? null;

            //App.UserName = data.UserName;
            //App.AccessList = data.AccessList;
            //App.AccessToken = data.Token;

            var gFunctions = GetRequiredService<GlobalFunctions>();
            _splash = new SplashWindow(gFunctions);
            if (_splash.ShowDialog() == false)
            {
                this.Shutdown();
                return;
            }

            //var a = new LoginWindow();
            //a.ShowDialog();

        }
        catch
        {
            // swallow splash exceptions to avoid crash on startup if rendering fails
            this.Shutdown();
            return;
        }

        try
        {
            //    //// Wait for any initial refresh the splash performs before starting host/main window.
            //    //// This keeps the splash visible until RefreshAsync completes.
            //    //if (_splash != null)
            //    //{
            //    //    try
            //    //    {
            //    //        System.Diagnostics.Debug.WriteLine("Splash Window RefreshAsync - 1 begin.");
            //    //        splashWindowResult = await _splash.RefreshAsync();
            //    //        System.Diagnostics.Debug.WriteLine("Splash Window RefreshAsync - 1 done.");
            //    //        if (splashWindowResult)
            //    //        {
            //    //            System.Diagnostics.Debug.WriteLine("Splash Window Close - 4 - Before Close.");
            //    //            _splash.Close();
            //    //        }
            //    //        System.Diagnostics.Debug.WriteLine("Splash Window Close - 4 - After Close.");
            //    //    }
            //    //    catch
            //    //    {
            //    //        // optionally log but don't block startup permanently on failure
            //    //    }
            //    //}

            //    // Close splash before showing the main window
            //    try { _splash?.Close(); } catch { }

            //    // Start host asynchronously
            //    await _host.StartAsync();

            //    // Host now started — resolve main window from DI and show it
            //    var windowObj = _host.Services.GetService(typeof(IWindow));
            //    if (windowObj is Window win)
            //    {
            //        //// Close splash before showing the main window
            //        //try { 
            //        //    System.Diagnostics.Debug.WriteLine("Splash Window Close - 1.");
            //        //    _splash?.Close(); } catch { }

            //        //if (splashWindowResult)
            //        //{
            //            // Set as application's main window and show
            //            this.MainWindow = win;
            //            win.Show();
            //        //}
            //        //else {
            //        //    this.Shutdown();
            //        //}
            //    }
            //    else
            //    {
            //        //// Close splash if main window not found
            //        //try {
            //        //    System.Diagnostics.Debug.WriteLine("Splash Window Close - 2.");
            //        //    _splash?.Close(); } catch { }
            //    }
            // Start host asynchronously
            await _host.StartAsync();

            // Host now started — resolve main window from DI and show it
            var windowObj = _host.Services.GetService(typeof(IWindow));
            if (windowObj is Window win)
            {
                // Close splash before showing the main window
                try { _splash?.Close(); } catch { }

                // Set as application's main window and show
                this.MainWindow = win;

                this.ShutdownMode = ShutdownMode.OnMainWindowClose;

                win.Show();
            }
        }
        catch (Exception)
        {
            //try {
            //    System.Diagnostics.Debug.WriteLine("Splash Window Close - 3.");
            //    _splash?.Close(); } catch { }
            this.Shutdown();
            return;
        }
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("App OnExit.");
        _host.StopAsync().Wait();

        _host.Dispose();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
    }
}
