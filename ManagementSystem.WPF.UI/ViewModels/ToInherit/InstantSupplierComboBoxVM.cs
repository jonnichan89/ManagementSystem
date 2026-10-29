using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementSystem.WPF.UI.ViewModels.ToInherit
{
    public partial class InstantSupplierComboBoxVM : ObservableObject
    {
        public enum EnumSupplierUsedIn
        {
            None = 0,
            Purchase = 1,
            Sales = 2,
            ProjectPurchase = 3,
            ProjectSales = 4,
        }

        GlobalFunctions _gFunctions;
        bool _usedInPO, _usedInSO, _usedInPPO, _usedInPSO;
        public InstantSupplierComboBoxVM(GlobalFunctions gFunctions, EnumSupplierUsedIn usedIn)
        {
            _gFunctions = gFunctions;
            switch (usedIn)
            {
                case EnumSupplierUsedIn.None:
                    break;
                case EnumSupplierUsedIn.Purchase:
                    _usedInPO = true;
                    break;
                case EnumSupplierUsedIn.Sales:
                    _usedInSO = true;
                    break;
                case EnumSupplierUsedIn.ProjectPurchase:
                    _usedInPPO = true;
                    break;
                case EnumSupplierUsedIn.ProjectSales:
                    _usedInPSO = true;
                    break;
                default:
                    break;
            }
        }

        public Action<MstSupplier?> SupplierChanged; // (MstSupplier? selectedSupplier);
        public Func<string> GetSupplierCode; // () { return ""; }
        public Action<string> SetSupplierName; // (string supplierName) { }

        #region Search Supplier

        string _filterTextSupplier;
        Predicate<object> _filterPredicateSupplier;
        protected internal ObservableRangeCollection<MstSupplier> _suppliers = new();

        protected internal MstSupplier? _selectedSupplier;

        [ObservableProperty]
        private ListCollectionView _filteredViewSupplier;

        [ObservableProperty]
        private int _supplierSelectedIndex;

        private bool _isSelectedSupplierChanged;
        public MstSupplier? SelectedSupplier
        {
            get { return _selectedSupplier; }
            set
            {
                _isSelectedSupplierChanged = true;

                _selectedSupplier = value;

                if (_selectedSupplier != null)
                {
                    SupplierChanged(_selectedSupplier);
                    //_data.SupplierCode = _selectedSupplier.SupplierCode;
                    //_data.SupplierName = _selectedSupplier.SupplierName;
                }

                _isSelectedSupplierChanged = false;

                OnPropertyChanged(nameof(SelectedSupplier));
            }
        }

        public string FilterSupplierText
        {
            get { return _filterTextSupplier; }
            set
            {
                if (_isSelectedSupplierChanged) return;

                _filterTextSupplier = value;

                if (_filterTextSupplier.Length >= 3 &&
                    _suppliers.Count > 0 &&
                    (_selectedSupplier == null || _selectedSupplier.SupplierName != _filterTextSupplier))
                {
                    var foundSupplier = _suppliers.FirstOrDefault(i => i.SupplierName == value);

                    if (foundSupplier == null)
                    {
                        using (FilteredViewSupplier.DeferRefresh())
                        {
                            FilteredViewSupplier.Filter = _filterPredicateSupplier;
                            FilteredViewSupplier.SortDescriptions.Add(new SortDescription("Askterisk", ListSortDirection.Descending));
                            FilteredViewSupplier.SortDescriptions.Add(new SortDescription("SupplierName", ListSortDirection.Ascending));
                        }

                        if (_suppliers.Count(S => S.Askterisk != "") == 1)
                        {
                            FilteredViewSupplier.MoveCurrentTo(FilteredViewSupplier.GetItemAt(0));

                            _filterTextSupplier = (FilteredViewSupplier.GetItemAt(0) as MstItem)?.ItemName ?? string.Empty;
                            SupplierSelectedIndex = 0;
                        }
                        else
                        {
                            SupplierSelectedIndex = -1;
                        }
                    }
                    else
                    {
                        SupplierChanged(foundSupplier);
                        //_data.SupplierCode = foundSupplier.SupplierCode;
                        //_data.SupplierName = foundSupplier.SupplierName;

                        _filterTextSupplier = foundSupplier.SupplierName;
                        SupplierSelectedIndex = FilteredViewSupplier.IndexOf(foundSupplier);
                    }
                    OnPropertyChanged(nameof(Data));
                }
            }
        }

        protected internal async Task LoadSearchSupplierAsync()
        {
            await _gFunctions.RefreshSupplierDataAsync(false);
            _suppliers.Clear();
            _suppliers.AddRange(_gFunctions.
                Suppliers.
                Where(p => 
                    (_usedInPO && p.UsedInPO == _usedInPO) ||
                    (_usedInSO && p.UsedInSO == _usedInSO) ||
                    (_usedInPPO && p.UsedInPPO == _usedInPPO) ||
                    (_usedInPSO && p.UsedInPSO == _usedInPSO)).
                ToArray());

            _filterPredicateSupplier = item =>
            {
                var item1 = (item as MstSupplier);
                item1.PutSearchToTop = (item1.SearchKey.Contains(_filterTextSupplier, StringComparison.OrdinalIgnoreCase));
                return true;
            };

            string supplierCode = GetSupplierCode(); // _data.SupplierCode;
            _filteredViewSupplier = new ListCollectionView(_suppliers);
            OnPropertyChanged(nameof(FilteredViewSupplier));

            SelectedSupplier = _suppliers.FirstOrDefault(i => i.SupplierCode == supplierCode);

            if (_selectedSupplier != null)
            {
                SetSupplierName(_selectedSupplier.SupplierName);
                //_data.SupplierName = _selectedSupplier.SupplierName;
                _supplierSelectedIndex = FilteredViewSupplier.IndexOf(SelectedSupplier);
            }
            else
            {
                SupplierChanged(null);
                //_data.SupplierCode = "";
                //_data.SupplierName = "";
                _supplierSelectedIndex = -1;
            }

            OnPropertyChanged(nameof(SupplierSelectedIndex));
        }

        #endregion

    }
}
