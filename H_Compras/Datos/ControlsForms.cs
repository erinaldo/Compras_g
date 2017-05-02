using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace H_Compras.Datos.Clases
{
    public static class ControlsForms_
    {
        //[System.ComponentModel.Bindable(true)]
        public static void setDataSource(ComboBox _control, DataTable _source, string _displayMember, string _valueMember, string _header)
        {
            if (!string.IsNullOrEmpty(_header))
            {
                DataRow _newRow = _source.NewRow();
                _newRow[_valueMember] = "0";
                _newRow[_displayMember] = _header;

                _source.Rows.InsertAt(_newRow, 0);
            }
            _control.DataSource = _source;
            _control.DisplayMember = _displayMember;
            _control.ValueMember = _valueMember;

        }

        public static void setDataSource(ListBox _control, DataTable _source, string _displayMember, string _valueMember, string _header)
        {


            if (!string.IsNullOrEmpty(_header))
            {
                DataRow _newRow = _source.NewRow();
                _newRow[_valueMember] = "0";
                _newRow[_displayMember] = _header;

                _source.Rows.InsertAt(_newRow, 0);
            }
            _control.DataSource = _source;
            _control.DisplayMember = _displayMember;
            _control.ValueMember = _valueMember;

        }

        public static void Autocomplete(TextBox txt, DataTable _source, string _name)
        {
            var source = new AutoCompleteStringCollection();
            source.AddRange((from item in _source.AsEnumerable()
                             select item.Field<string>(_name)).ToArray());

            txt.AutoCompleteCustomSource = source;
            txt.AutoCompleteMode = AutoCompleteMode.Suggest;
            txt.AutoCompleteSource = AutoCompleteSource.CustomSource;

        }

        #region Eventos Coltroles
        public static void UltraGrid_KeyDown(object sender, KeyEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGrid dgvDatos = (sender as Infragistics.Win.UltraWinGrid.UltraGrid);
            switch (e.KeyCode)
            {
                case Keys.Up:
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false,
                      false);
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.AboveCell, false,
                      false);
                    e.Handled = true;
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode, false,
                      false);
                    break;
                case Keys.Down:
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false,
                      false);
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.BelowCell, false,
                      false);
                    e.Handled = true;
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode, false,
                      false);
                    break;
                case Keys.Right:
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false,
                      false);
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.NextCellByTab, false,
                      false);
                    e.Handled = true;
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode, false,
                      false);
                    break;
                case Keys.Left:
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false,
                      false);
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.PrevCellByTab, false,
                      false);
                    e.Handled = true;
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode, false,
                      false);
                    break;
            }
        }

        public static void UltraGrid_KeyPress(object sender, KeyPressEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGrid grid = (sender as Infragistics.Win.UltraWinGrid.UltraGrid);

            Infragistics.Win.UltraWinGrid.UltraGridCell activeCell = grid == null ? null : grid.ActiveCell;

            // if there is an active cell, its not in edit mode and can enter edit mode
            if (null != activeCell && false == activeCell.IsInEditMode && activeCell.CanEnterEditMode)
            {
                // if the character is not a control character
                if (char.IsControl(e.KeyChar) == false)
                {
                    // try to put cell into edit mode
                    grid.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode);

                    // if this cell is still active and it is in edit mode...
                    if (grid.ActiveCell == activeCell && activeCell.IsInEditMode)
                    {
                        // get its editor
                        Infragistics.Win.EmbeddableEditorBase editor = grid.ActiveCell.EditorResolved;

                        // if the editor supports selectable text
                        if (editor.SupportsSelectableText)
                        {
                            // select all the text so it can be replaced
                            editor.SelectionStart = 0;
                            editor.SelectionLength = editor.TextLength;

                            if (editor is Infragistics.Win.EditorWithMask)
                            {
                                // just clear the selected text and let the grid
                                // forward the keypress to the editor
                                editor.SelectedText = string.Empty;
                            }
                            else
                            {
                                // then replace the selected text with the character
                                editor.SelectedText = new string(e.KeyChar, 1);

                                // mark the event as handled so the grid doesn't process it
                                e.Handled = true;
                            }
                        }
                    }
                }
            }
        }

        public static void clbBox_Click(object sender, EventArgs e)
        {
            if (((CheckedListBox)sender).SelectedIndex == 0)
            {
                if (((CheckedListBox)sender).CheckedIndices.Contains(0))
                {
                    for (int item = 1; item < ((CheckedListBox)sender).Items.Count; item++)
                    {
                        ((CheckedListBox)sender).SetItemChecked(item, false);
                    }
                }
                else
                {
                    for (int item = 1; item < ((CheckedListBox)sender).Items.Count; item++)
                    {
                        ((CheckedListBox)sender).SetItemChecked(item, true);
                    }
                }
            }

        }

        public static string getCadena(CheckedListBox clb, string separador, bool vacia, string columName)
        {
            StringBuilder stb = new StringBuilder();
            foreach (DataRowView item in clb.CheckedItems)
            {
                if (item[columName].ToString() != "0")
                {
                    if (!clb.ToString().Equals(string.Empty))
                    {
                        stb.Append(",");
                    }
                    stb.Append(separador + item[columName] + separador);
                }
            }
            if (clb.CheckedItems.Count == 0)
            {
                foreach (DataRowView item in clb.Items)
                {
                    if (item[columName].ToString() != "0")
                    {
                        if (!clb.ToString().Equals(string.Empty))
                        {
                            stb.Append(",");
                        }
                        stb.Append(separador + item[columName] + separador);
                    }
                }
                if (vacia)
                    return string.Empty;
            }

            return stb.ToString().Trim(',');
        }
        #endregion
    }
}
