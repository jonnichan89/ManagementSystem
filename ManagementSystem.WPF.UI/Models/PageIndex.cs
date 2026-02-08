
namespace ManagementSystem.WPF.UI.Models
{
    public class PageIndex : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;


        private int _pageValue;
        private string _pageText;

        private bool _forceUpdatePageList;
        private bool _isNextPageList;
        private bool _isPreviousPageList;
        private int _pageListStartFrom;

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;

                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        public int PageValue { get => _pageValue; }
        public bool ForceUpdatePageList { get => _forceUpdatePageList; }
        public bool IsNextPageList { get => _isNextPageList; }
        public bool IsPreviousPageList { get => _isPreviousPageList; }
        public int PageListStartFrom { get => _pageListStartFrom; }
        public string PageText { get => _pageText; }

        public PageIndex(int pageIndex, bool forceUpdatePageList)
        {
            _pageValue = pageIndex;
            _forceUpdatePageList = forceUpdatePageList;

            _isSelected = false;

            _pageText = _pageValue.ToString();
        }

        public PageIndex(bool isPreviousPageList, bool isNextPageList, int pageListStartFrom)
        {
            _isPreviousPageList = isPreviousPageList;
            _isNextPageList = isNextPageList;
            _pageListStartFrom = pageListStartFrom;

            _pageText = "...";
        }
    }
}
