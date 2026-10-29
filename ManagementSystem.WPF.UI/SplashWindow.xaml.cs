using Microsoft.Identity.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ManagementSystem.WPF.UI;

/// <summary>
/// Interaction logic for SplashWindow.xaml
/// </summary>
public partial class SplashWindow : Window
{
    private GlobalFunctions _gFunctions;
    private int _isRefreshing;

    public SplashWindow(GlobalFunctions gFunctions)
    {
        InitializeComponent();
        _gFunctions = gFunctions;

        // Hook lifecycle events to trigger refreshes when the splash is shown or activated
        Loaded += SplashWindow_Loaded;
        Activated += SplashWindow_Activated;
        Unloaded += SplashWindow_Unloaded;
        Closed += SplashWindow_Closed;
    }

    private void SplashWindow_Closed(object? sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Splash Window Closed.");
    }

    private void SplashWindow_Unloaded(object? sender, RoutedEventArgs e)
    {
        // detach handlers to avoid leaks if window is reused
        Loaded -= SplashWindow_Loaded;
        Activated -= SplashWindow_Activated;
        Unloaded -= SplashWindow_Unloaded;
        Closed -= SplashWindow_Closed;
    }

    private void SplashWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // safe fire-and-forget for UI-driven activation; App will explicitly await RefreshAsync when needed
        _ = EnsureRefreshedAsync();
    }

    private void SplashWindow_Activated(object? sender, EventArgs e)
    {
        _ = EnsureRefreshedAsync();
    }

    /// <summary>
    /// Public entry to request a refresh and wait for completion.
    /// </summary>
    public async Task<bool> RefreshAsync()
    {
            System.Diagnostics.Debug.WriteLine("Splash Window await RefreshAsync begin.");
        return await EnsureRefreshedAsync();
            System.Diagnostics.Debug.WriteLine("Splash Window await RefreshAsync done.");
    }

    /// <summary>
    /// Ensures only one refresh runs at a time. Calls GlobalFunctions refresh methods concurrently.
    /// </summary>
    private async Task<bool> EnsureRefreshedAsync()
    {
        // simple concurrency guard: return if a refresh is already running
        if (Interlocked.Exchange(ref _isRefreshing, 1) == 1)
            return false;

        try
        {
            UpdateStatus("Refreshing items...");
            // run the main refresh tasks in parallel
            await Task.WhenAll(
                _gFunctions.RefreshItemDataAsync(true),
                _gFunctions.RefreshSupplierDataAsync(true),
                _gFunctions.RefreshAccountDataAsync(true),
                _gFunctions.RefreshProjectTemplateDataAsync(true)
            );

            System.Diagnostics.Debug.WriteLine("Splash Window Task done.");
            UpdateStatus("Ready");
        }
        catch (Exception ex)
        {
            // show a concise status on failure; consider logging the exception elsewhere
            System.Diagnostics.Debug.WriteLine("Splash Window Task failed.");
            UpdateStatus("Refresh failed");
        }
        finally
        {
            Interlocked.Exchange(ref _isRefreshing, 0);
            System.Diagnostics.Debug.WriteLine("Splash Window set DialogResult.");
            this.DialogResult = true;
            this.Close();
        }

        return true;
    }

    /// <summary>
    /// Update status text shown on the splash.
    /// Safe to call from UI thread.
    /// </summary>
    public void UpdateStatus(string text)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(() => UpdateStatus(text));
            return;
        }

        StatusText.Text = text ?? string.Empty;
    }
}