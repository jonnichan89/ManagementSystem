namespace ManagementSystem.WPF.UI.Services.ServiceInterfaces
{
    public interface IViewInteractionService
    {
        PromptUsedFor PromptUsedFor { get; set; }
        event EventHandler<MyContentDialogButtonClickEventArgs> PromptFeedback;

        void Prompt(PromptUsedFor usedFor, string title, string message);
        void onPromptFeedback(ContentDialogButton button);

        void Close(ContentDialogResult result);

    }

    public enum PromptUsedFor
    {
        None = 0,
        QuestionBeforeSaveDetail = 1,
        MessageDetailSaveSuccess = 2,
        MessageDetailSaveFailed = 3,
        MessageAlert = 4,
        QuestionBeforeSaveDelete = 5,
        MessageDetailDeleteSuccess = 6,
        MessageDetailDeleteFailed = 7,
    }
}
