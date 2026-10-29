
using ManagementSystem.WPF.UI.ViewModels.ToInherit;

namespace ManagementSystem.WPF.UI.ViewModels;

public partial class MainWindowViewModel() : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<Control> _trayMenuItems =
    [
        new Wpf.Ui.Controls.MenuItem()
        {
            Header = "Home",
            Tag = "tray_home",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
        },
        new Wpf.Ui.Controls.MenuItem()
        {
            Header = "Settings",
            Tag = "tray_settings",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
        },
        new Separator(),
        new Wpf.Ui.Controls.MenuItem()
        {
            Header = "Close",
            Tag = "tray_close",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Dismiss24 },
        },
    ];

    [RelayCommand]
    private void ChangeEnglishLanguage()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

        TranslationSource.Instance.CurrentCulture = new CultureInfo("en-US");
    }

    [RelayCommand]
    private void ChangeBahasaLanguage()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("id-ID");
        System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("id-ID");

        TranslationSource.Instance.CurrentCulture = new CultureInfo("id-ID");
    }
}