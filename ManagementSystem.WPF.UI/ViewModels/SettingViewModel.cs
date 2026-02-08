
namespace ManagementSystem.WPF.UI.ViewModels;

public partial class SettingViewModel() : ViewModel
{
    [RelayCommand]
    private void IsEnglishSelected(bool value)
    {
        if (!value) return;

        ManagementSystem.WPF.UI.Properties.Settings.Default.Language = "en-US";
        ManagementSystem.WPF.UI.Properties.Settings.Default.Save();

        System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

        TranslationSource.Instance.CurrentCulture = new CultureInfo("en-US");

        WeakReferenceMessenger.Default.Send(new ActionMessage(ActionMessage.ActionName.ChangeLanguage, null));
    }

    [RelayCommand]
    private void IsBahasaSelected(bool value)
    {
        if (!value) return;

        ManagementSystem.WPF.UI.Properties.Settings.Default.Language = "id-ID";
        ManagementSystem.WPF.UI.Properties.Settings.Default.Save();

        System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("id-ID");
        System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("id-ID");

        TranslationSource.Instance.CurrentCulture = new CultureInfo("id-ID");

        WeakReferenceMessenger.Default.Send(new ActionMessage(ActionMessage.ActionName.ChangeLanguage, null));
    }
}