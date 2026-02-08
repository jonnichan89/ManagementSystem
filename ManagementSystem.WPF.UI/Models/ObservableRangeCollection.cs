
namespace ManagementSystem.WPF.UI.Models
{
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            CheckReentrancy();

            foreach (var item in collection)
            {
                Items.Add(item); // Add to the underlying private List<T> without events
            }

            // Raise a single event to notify the UI that the collection has been reset/changed
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

}
