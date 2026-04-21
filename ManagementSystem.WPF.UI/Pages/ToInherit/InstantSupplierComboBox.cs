using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementSystem.WPF.UI.ViewModels.ToInherit;

namespace ManagementSystem.WPF.UI.Pages.ToInherit
{
    internal class InstantSupplierComboBox
    {
        ComboBox _supplierComboBox;
        InstantSupplierComboBoxVM VM;
        public InstantSupplierComboBox(ref ComboBox supplierComboBox, InstantSupplierComboBoxVM vm)
        {
            _supplierComboBox = supplierComboBox;
            _supplierComboBox.PreviewKeyDown += SupplierComboBox_PreviewKeyDown;
            _supplierComboBox.KeyUp += SupplierComboBox_KeyUp;
            VM = vm;
        }

        public void Dispose()
        {
            _supplierComboBox.PreviewKeyDown -= SupplierComboBox_PreviewKeyDown;
            _supplierComboBox.KeyUp -= SupplierComboBox_KeyUp;
        }

        #region ComboBox Supplier

        private void SupplierComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SupplierComboBox_PreviewKeyDown: " + _supplierComboBox.Text + " = " + _supplierComboBox.IsDropDownOpen.ToString() + " - " + e.Key.ToString());

            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                e.Handled = true;
            }
        }

        private void SupplierComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SupplierComboBox_KeyUp: " + _supplierComboBox.Text + " = " + _supplierComboBox.IsDropDownOpen.ToString() + " - " + e.Key.ToString());

            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                if (VM.SelectedSupplier != null && VM.SelectedSupplier.SupplierName == _supplierComboBox.Text)
                {
                    // Do Nothing
                }
                else if (_supplierComboBox.Text.Length >= 3)
                {
                    VM.FilterSupplierText = _supplierComboBox.Text;

                    if (VM.SupplierSelectedIndex == -1)
                        if (!_supplierComboBox.IsDropDownOpen)
                        {
                            _supplierComboBox.IsDropDownOpen = true;
                            _supplierComboBox.SelectedIndex = 0;
                        }
                }
                e.Handled = true;
            }

        }

        #endregion
    }
}
