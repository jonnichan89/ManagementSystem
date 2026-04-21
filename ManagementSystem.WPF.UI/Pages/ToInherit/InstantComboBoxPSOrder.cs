using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementSystem.WPF.UI.ViewModels.ToInherit;

namespace ManagementSystem.WPF.UI.Pages.ToInherit
{
    internal class InstantComboBoxPSOrder
    {
        ComboBox _comboBox;
        InstantComboBoxVMPSOrder VM;
        public InstantComboBoxPSOrder(ref ComboBox comboBox, InstantComboBoxVMPSOrder vm)
        {
            _comboBox = comboBox;
            _comboBox.PreviewKeyDown += ComboBox_PreviewKeyDown;
            _comboBox.KeyUp += ComboBox_KeyUp;
            VM = vm;
        }

        public void Dispose()
        {
            _comboBox.PreviewKeyDown -= ComboBox_PreviewKeyDown;
            _comboBox.KeyUp -= ComboBox_KeyUp;
        }

        #region ComboBox PSOrder

        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ComboBox_PreviewKeyDown: " + _comboBox.Text + " = " + _comboBox.IsDropDownOpen.ToString() + " - " + e.Key.ToString());

            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                e.Handled = true;
            }
        }

        private async void ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ComboBox_KeyUp: " + _comboBox.Text + " = " + _comboBox.IsDropDownOpen.ToString() + " - " + e.Key.ToString());

            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                if (VM.SelectedItem != null && VM.SelectedItem.PSCode == _comboBox.Text)
                {
                    // Do Nothing
                }
                else if (_comboBox.Text.Length >= 3)
                {
                    VM.FilterText = _comboBox.Text;
                    await VM.LoadSearchAsync();
                    
                    if (!_comboBox.IsDropDownOpen)
                    {
                        _comboBox.IsDropDownOpen = true;
                    }
                }
                e.Handled = true;
            }

        }

        #endregion
    }
}
