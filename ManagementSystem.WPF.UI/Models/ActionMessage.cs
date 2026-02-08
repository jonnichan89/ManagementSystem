
namespace ManagementSystem.WPF.UI.Models
{
    public class ActionMessage
    {
        public enum ActionName
        {
            ChangeLanguage
        }

        public ActionName Name { get; set; }
        public object? Data { get; set; }

        public ActionMessage(ActionName name, object? data)
        {
            Name = name;
            Data = data;
        }
    }
}
