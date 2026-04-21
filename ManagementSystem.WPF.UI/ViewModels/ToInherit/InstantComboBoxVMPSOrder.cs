using ManagementSystem.Common.OtherModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementSystem.WPF.UI.ViewModels.ToInherit
{
    public partial class InstantComboBoxVMPSOrder : ObservableObject
    {
        GlobalFunctions _gFunctions;
        public InstantComboBoxVMPSOrder(GlobalFunctions gFunctions)
        {
            _gFunctions = gFunctions;
        }

        public Action<SearchOptionResultPSOrder?> SelectedItemChanged; // (DataPSOrder? selectedPSOrder);
        public Func<string> GetItemCode; // () { return ""; }
        public Action<string> SetItemName; // (string referenceName) { }

        #region Search PSOrder

        string _filterText;
        //Predicate<object> _filterPredicatePSOrder;
        protected internal ObservableRangeCollection<SearchOptionResultPSOrder> _listItem = new();

        protected internal SearchOptionResultPSOrder? _selectedItem;

        [ObservableProperty]
        private ListCollectionView _filteredListItem;

        [ObservableProperty]
        private int _selectedIndex;

        private SearchPSOrderForOptions.PSTypeEnum _PSType;
        public SearchPSOrderForOptions.PSTypeEnum PSType {
            set {
                _PSType = value;
                ClearListItem();
            }
        }

        private bool _isSelectedChanged;
        public SearchOptionResultPSOrder? SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _isSelectedChanged = true;

                _selectedItem = value;

                if (_selectedItem != null)
                {
                    SelectedItemChanged(_selectedItem);
                }

                _isSelectedChanged = false;

                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public string FilterText
        {
            get { return _filterText; }
            set
            {
                if (_isSelectedChanged) return;

                _filterText = value;

                //if (_filterText.Length >= 3 &&
                //    _listItem.Count > 0 &&
                //    (_selectedItem == null || _selectedItem.PSCode != _filterText))
                //{
                //    var foundItem = _listItem.FirstOrDefault(i => i.PSCode == value);

                //    if (foundItem == null)
                //    {
                //        //using (FilteredViewPSOrder.DeferRefresh())
                //        //{
                //        //    FilteredViewPSOrder.Filter = _filterPredicatePSOrder;
                //        //    FilteredViewPSOrder.SortDescriptions.Add(new SortDescription("PSCode", ListSortDirection.Ascending));
                //        //}

                //        //if (_references.Count(S => S.Askterisk != "") == 1)
                //        //{
                //        FilteredListItem.MoveCurrentTo(FilteredListItem.GetItemAt(0));

                //        _filterText = (FilteredListItem.GetItemAt(0) as SearchOptionResultPSOrder)?.PSCode ?? string.Empty;
                //        SelectedIndex = 0;
                //        //}
                //        //else
                //        //{
                //        //    PSOrderSelectedIndex = -1;
                //        //}
                //    }
                //    else
                //    {
                //        SelectedItemChanged(foundItem);
                //        //_data.PSOrderCode = foundPSOrder.PSOrderCode;
                //        //_data.PSOrderName = foundPSOrder.PSOrderName;

                //        _filterText = foundItem.PSCode;
                //        SelectedIndex = FilteredListItem.IndexOf(foundItem);
                //    }
                //}
                OnPropertyChanged(nameof(FilterText));
            }
        }

        protected internal async Task LoadSearchAsync()
        {
            ClearListItem();

            if (_PSType == SearchPSOrderForOptions.PSTypeEnum.None)
            {
                return;
            }

            IEnumerable<SearchOptionResultPSOrder> searchResult;
            if (_PSType == SearchPSOrderForOptions.PSTypeEnum.Purchase)
                searchResult = await _gFunctions.SearchPurchaseOrderAsync(_filterText);
            else
                searchResult = await _gFunctions.SearchSalesOrderAsync(_filterText);

            _listItem.AddRange(searchResult);

            //_filterPredicatePSOrder = item =>
            //{
            //    var item1 = (item as DataPSOrder);
            //    item1.PutSearchToTop = (item1.SearchKey.Contains(_filterPSOrderText, StringComparison.OrdinalIgnoreCase));
            //    return true;
            //};

            string referenceCode = GetItemCode(); // _data.PSOrderCode;
            FilteredListItem = new ListCollectionView(_listItem);
            OnPropertyChanged(nameof(FilteredListItem));

            SelectedItem = _listItem.FirstOrDefault(i => i.PSCode == referenceCode);

            if (_selectedItem != null)
            {
                _selectedIndex = FilteredListItem.IndexOf(SelectedItem);
            }
            else
            {
                SelectedItemChanged(null);
                //_data.PSOrderCode = "";
                //_data.PSOrderName = "";
                _selectedIndex = -1;
            }

            OnPropertyChanged(nameof(SelectedIndex));
        }

        #endregion

        private void ClearListItem()
        {
            _listItem.Clear();

            FilteredListItem = new ListCollectionView(_listItem);
            OnPropertyChanged(nameof(FilteredListItem));
        }
    }
}
