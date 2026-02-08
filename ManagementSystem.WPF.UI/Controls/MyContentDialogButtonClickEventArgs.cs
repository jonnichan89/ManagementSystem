
namespace ManagementSystem.WPF.UI.Controls
{
    public class MyContentDialogButtonClickEventArgs : EventArgs
    {
        public ContentDialogButton Button { get; set; }
        public MyContentDialogButtonClickEventArgs(ContentDialogButton button) : base()
        {
            Button = button;
        }
    }
}
