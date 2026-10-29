using ManagementSystem.Common.OtherModels;
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using System.Diagnostics;

namespace ManagementSystem.WPF.UI.ViewModels.ToInherit
{
    public abstract partial class BaseViewModel : ObservableObject, INavigationAware, IDisposable
    {
        public BaseViewModel()
        {
            // Example of subscribing to an observable (if needed)
            //_subscription = someObservable.Subscribe(value => { /* Handle value */ });

            WeakReferenceMessenger.Default.Register<ActionMessage>(this, (r, m) =>
            {
                OnActionMessageReceived(m);
            });
        }

        internal virtual void OnActionMessageReceived(ActionMessage m) { }

        /// <inheritdoc />
        public virtual Task OnNavigatedToAsync()
        {
            OnNavigatedTo();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the event that is fired after the component is navigated to.
        /// </summary>
        // ReSharper disable once MemberCanBeProtected.Global
        public virtual void OnNavigatedTo() { }

        /// <inheritdoc />
        public virtual Task OnNavigatedFromAsync()
        {
            OnNavigatedFrom();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the event that is fired before the component is navigated from.
        /// </summary>
        // ReSharper disable once MemberCanBeProtected.Global
        public virtual void OnNavigatedFrom() { }

        public virtual void Dispose()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Dispose();
            }
        }
    }
}
