
namespace ManagementSystem.WPF.UI.ViewModels
{
    public interface IDetailViewModel
    {
        void TrySavePrimary();
        Task<bool> SavePrimaryAsync();
        void TrySaveSecondary();
        bool SaveSecondary();
        void TryClose();
        bool Close();
    }

    public abstract partial class DetailViewModel : ViewModel, IDetailViewModel
    {
        protected internal string Title { get; private set; }
        public virtual async Task InitializeAsync(DetailAction action, object pk) { await Task.Delay(100); }
        public abstract void TrySavePrimary();
        public abstract void TrySaveSecondary();
        public abstract void TryClose();
        public virtual async Task<bool> SavePrimaryAsync() { return await Task.Run<bool>(() => false); }
        public virtual bool SaveSecondary() { return false; }
        public virtual bool Close() { return false; }

        protected internal DetailAction _action;
        public DetailViewModel() : base()
        {
        }
    }

    public enum DetailAction
    {
        ViewDetail,
        Add,
        Edit
    }

    public enum DetailViewModelDoAction
    {
        None,
        SavePrimary,
        SavePrimarySuccess,
        SaveSecondary,
        SaveSecondarySuccess,
        Close
    }

    public enum ListViewModelDoAction
    {
        None,
        Delete
    }
}
