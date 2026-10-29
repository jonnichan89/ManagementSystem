
using ManagementSystem.Common.OtherModels;
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using ManagementSystem.WPF.UI.ViewModels.ToInherit;
using System.Diagnostics;

namespace ManagementSystem.WPF.UI.ViewModels
{
    public abstract partial class BaseListViewModel<T> : BaseViewModel
    {

        protected T[]? _items;
        protected T _selectedItem;
        public virtual T SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));

                AfterSelectedItemChanged();
            }
        }

        protected virtual void AfterSelectedItemChanged()
        { }

        public BaseListViewModel()
            : base()
        {
        }

        #region Paging

        [ObservableProperty]
        protected ObservableRangeCollection<PageIndex> _pageList = new();

        [ObservableProperty]
        protected ObservableRangeCollection<T> _previewItems = new();

        protected int _itemPerPage = 100;
        protected int _currentPage = 1;

        [RelayCommand]
        protected void RefreshPreviewItems(PageIndex? page)
        {
            _previewItems.Clear();

            if (page == null)
            {
                page = new PageIndex(_currentPage, (_pageList.Count > 0 ? false : true));
            }

            int pageIndex = RefreshPageList(page, page.ForceUpdatePageList)?.PageValue ?? 1;
            _previewItems.AddRange(_items.Skip((pageIndex - 1) * _itemPerPage).Take(_itemPerPage));

            SelectedItem = _previewItems.FirstOrDefault();

            _currentPage = pageIndex;

            AfterRefreshPreviewItems();
        }

        protected virtual void AfterRefreshPreviewItems()
        { }

        protected PageIndex? RefreshPageList(PageIndex? currentPage, bool forceUpdateList)
        {
            if (currentPage == null)
            {
                currentPage = new PageIndex(1, false);
                forceUpdateList = true;
            }

            PageIndex returnPage = null;
            bool updateList = forceUpdateList;
            if (!forceUpdateList)
            {
                updateList = currentPage.IsPreviousPageList || currentPage.IsNextPageList;
            }

            if (updateList)
            {
                int maxPage = (int)Math.Ceiling((decimal)_items.Length / _itemPerPage);

                int prevPageIndex = _pageList.FirstOrDefault(p => p.IsSelected)?.PageValue ?? 1;
                int pageIndex = currentPage.PageValue;

                int fromPageIndex = _pageList.Count > 0 ? _pageList.Where(p => p.PageValue > 0).Min(p => p.PageValue) : 1;
                int toPageIndex = fromPageIndex - 1 + 10 > maxPage ? maxPage : fromPageIndex - 1 + 10;

                if (forceUpdateList)
                {
                    fromPageIndex = pageIndex == 1 ?
                        1 :
                        pageIndex == maxPage ?
                        (((int)Math.Ceiling((maxPage / 10.0)) - 1) * 10) + 1 :
                        (((int)Math.Floor((maxPage / 10.0)) - 1) * 10) + 1;
                }
                else if (currentPage.IsPreviousPageList)
                {
                    fromPageIndex = currentPage.PageListStartFrom;
                }
                else if (currentPage.IsNextPageList)
                {
                    fromPageIndex = currentPage.PageListStartFrom;
                }

                toPageIndex = (fromPageIndex + 10 - 1 > maxPage ? maxPage : fromPageIndex + 10 - 1);

                _pageList.Clear();

                bool dotPrevious = false;
                bool dotNext = false;
                for (int i = 1; i <= maxPage; i++)
                {
                    if (i == 1)
                        _pageList.Add(new PageIndex(i, !(i >= fromPageIndex && i <= toPageIndex)));

                    else if (i >= fromPageIndex - 10 && i < fromPageIndex)
                    {
                        if (dotPrevious) continue;

                        _pageList.Add(new PageIndex(true, false, fromPageIndex - 10));
                        dotPrevious = true;
                    }

                    else if (i >= fromPageIndex && i <= toPageIndex)
                        _pageList.Add(new PageIndex(i, false));

                    else if (i > toPageIndex && i < maxPage)
                    {
                        if (dotNext) continue;

                        _pageList.Add(new PageIndex(false, true, i));
                        dotNext = true;
                    }

                    else if (i == maxPage)
                        _pageList.Add(new PageIndex(i, !(i >= fromPageIndex && i <= toPageIndex)));

                    if (i == fromPageIndex)
                    {
                        returnPage = _pageList.Last();
                        returnPage.IsSelected = true;
                    }
                }

            }
            else
            {
                int currentPageIndex = currentPage.PageValue;
                foreach (var pageItem in PageList)
                {
                    pageItem.IsSelected = (pageItem.PageValue == currentPageIndex);
                }

                returnPage = _pageList.First(p => p.IsSelected);
            }

            return returnPage;
        }

        #endregion

    }
}
