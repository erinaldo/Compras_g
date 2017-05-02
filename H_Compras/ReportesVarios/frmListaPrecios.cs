using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace H_Compras.ReportesVarios
{
    public partial class frmListaPrecios : Constantes.frmEmpty
    {
        DataTable tbl_Articulos;
        private enum Columnas
        {
            ItemCode, ItemName, Venta, 
            PC, Currency, CB, FactorCB, Utilidad,
            MM, FactorMM, MargenMM, NFactorMM, NMargenMM,
            MTR1, FactorMTR1, MargenMTR1, NFactorMTR1, NMargenMTR1,
            MTR2, FactorMTR2, MargenMTR2, NFactorMTR2, NMargenMTR2
        }

        public frmListaPrecios()
        {
            InitializeComponent();
            tbl_Articulos = new DataTable();
        }

        private void frmListaPrecios_Load(object sender, EventArgs e)
        {
            try
            {
                this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                tbl_Articulos = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetArticulos, string.Empty, string.Empty);
                ClasesSGUV.Form.ControlsForms.Autocomplete(txtArticulo, tbl_Articulos.Copy(), "ItemCode");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbLinea, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Lineas, null, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "------");
               
                #region Evento Grid

                foreach (GridKeyActionMapping ugKey in dgvDatos.KeyActionMappings)
                {
                    if (ugKey.KeyCode == Keys.Enter)
                    {
                        dgvDatos.KeyActionMappings.Remove(ugKey);
                    }
                }


                this.dgvDatos.KeyActionMappings.Add(
                   new GridKeyActionMapping(
                   Keys.Enter,
                   UltraGridAction.BelowCell,
                   0,
                   0,
                   SpecialKeys.All,
                   0));
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eror al cargar formulario: " + this.Name + "\r\n" + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                dgvDatos.DataSource = null;
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_ListaPrecios", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 1);
                        command.Parameters.AddWithValue("@ItmsGrpCod", cbLinea.SelectedValue);
                        command.Parameters.AddWithValue("@ItemCode", txtArticulo.Text);

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;

                        DataTable table = new DataTable();
                        da.Fill(table);

                        dgvDatos.DataSource = table;
                    }
                }
                button1.Text = "Filtrar errores";
                UltraGridBand band = this.dgvDatos.DisplayLayout.Bands[0];
                band.ColumnFilters["Error"].FilterConditions.Clear();
            }
            catch (Exception ex)
            {
               MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            Infragistics.Win.UltraWinCalcManager.UltraCalcManager calcManager;
            calcManager = new Infragistics.Win.UltraWinCalcManager.UltraCalcManager(this.Container);
            e.Layout.Grid.CalcManager = calcManager;

            foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn item in e.Layout.Bands[0].Columns)
            {
                item.Format = "N2";
                item.Width = 90;
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
            e.Layout.Bands[0].Columns["Error"].Hidden = true;

            e.Layout.Bands[0].Columns[(int)Columnas.ItemCode].Header.Fixed = true;
            e.Layout.Bands[0].Columns[(int)Columnas.ItemName].Header.Fixed = true;
            e.Layout.Bands[0].Columns[(int)Columnas.Venta].Header.Fixed = true;
            e.Layout.Bands[0].Columns[(int)Columnas.PC].Header.Fixed = true;
            e.Layout.Bands[0].Columns[(int)Columnas.Currency].Header.Fixed = true;
            e.Layout.Bands[0].Columns[(int)Columnas.CB].Header.Fixed = true;
            e.Layout.Bands[0].Columns[(int)Columnas.FactorCB].Header.Fixed = true;
            e.Layout.Bands[0].Columns[(int)Columnas.Utilidad].Header.Fixed = true;

            e.Layout.Bands[0].Columns[(int)Columnas.ItemCode].Header.Caption = "Artículo";
            e.Layout.Bands[0].Columns[(int)Columnas.ItemName].Header.Caption = "Nombre";
            e.Layout.Bands[0].Columns[(int)Columnas.Venta].Header.Caption = "Promedio venta (4 meses)";
            e.Layout.Bands[0].Columns[(int)Columnas.PC].Header.Caption = "Precio de compra";
            e.Layout.Bands[0].Columns[(int)Columnas.Currency].Header.Caption = "Moneda";
            e.Layout.Bands[0].Columns[(int)Columnas.CB].Header.Caption = "Costo base";
            e.Layout.Bands[0].Columns[(int)Columnas.FactorCB].Header.Caption = "Factor Costo base";
            e.Layout.Bands[0].Columns[(int)Columnas.Utilidad].Header.Caption = "Utilidad (4 meses)";

            e.Layout.Bands[0].Columns[(int)Columnas.ItemCode].Width = 90;
            e.Layout.Bands[0].Columns[(int)Columnas.ItemName].Width = 110;
            e.Layout.Bands[0].Columns[(int)Columnas.Venta].Width = 90;
            e.Layout.Bands[0].Columns[(int)Columnas.PC].Width = 90;
            e.Layout.Bands[0].Columns[(int)Columnas.Currency].Width = 90;
            e.Layout.Bands[0].Columns[(int)Columnas.CB].Width = 90;
            e.Layout.Bands[0].Columns[(int)Columnas.FactorCB].Width = 90;
            e.Layout.Bands[0].Columns[(int)Columnas.Utilidad].Width = 90;

            e.Layout.Bands[0].Columns[(int)Columnas.Venta].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.PC].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Currency].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
            e.Layout.Bands[0].Columns[(int)Columnas.CB].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.FactorCB].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Utilidad].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            e.Layout.Bands[0].Columns["MargenMM"].Formula = "1 - ([CB] / [MM])";
            e.Layout.Bands[0].Columns["MargenMTR1"].Formula = "1 - ([CB] / [MTR1])";
            e.Layout.Bands[0].Columns["MargenMTR2"].Formula = "1 -([CB] / [MTR2])";

            e.Layout.Bands[0].Columns["NMargenMM"].Formula = "1 - ([CB] / ([MM] * [NFactorMM]))";
            e.Layout.Bands[0].Columns["NMargenMTR1"].Formula = "1 - ([CB] / ([MTR1] * [NFactorMTR1]))";
            e.Layout.Bands[0].Columns["NMargenMTR2"].Formula = "1 - ([CB] / ([MTR2] * [NFactorMTR2]))";
            
            e.Layout.Override.FormulaErrorAppearance.BackColor = Color.Red;

            #region Colores
            Color mm = Color.FromArgb(255,192,0);
            e.Layout.Bands[0].Columns["MM"].CellAppearance.BackColor = mm;
            e.Layout.Bands[0].Columns["FactorMM"].CellAppearance.BackColor = mm;
            e.Layout.Bands[0].Columns["MargenMM"].CellAppearance.BackColor = mm;
            e.Layout.Bands[0].Columns["NFactorMM"].CellAppearance.BackColor = mm;
            e.Layout.Bands[0].Columns["NMargenMM"].CellAppearance.BackColor = mm;

            Color r1 = Color.Orange;
            e.Layout.Bands[0].Columns["MTR1"].CellAppearance.BackColor = r1;
            e.Layout.Bands[0].Columns["FactorMTR1"].CellAppearance.BackColor = r1;
            e.Layout.Bands[0].Columns["MargenMTR1"].CellAppearance.BackColor = r1;
            e.Layout.Bands[0].Columns["NFactorMTR1"].CellAppearance.BackColor = r1;
            e.Layout.Bands[0].Columns["NMargenMTR1"].CellAppearance.BackColor = r1;

            Color r2 = Color.FromArgb(0,176,240);
            e.Layout.Bands[0].Columns["MTR2"].CellAppearance.BackColor = r2;
            e.Layout.Bands[0].Columns["FactorMTR2"].CellAppearance.BackColor = r2;
            e.Layout.Bands[0].Columns["MargenMTR2"].CellAppearance.BackColor = r2;
            e.Layout.Bands[0].Columns["NFactorMTR2"].CellAppearance.BackColor = r2;
            e.Layout.Bands[0].Columns["NMargenMTR2"].CellAppearance.BackColor = r2;

            //e.Layout.Bands[0].Columns["NFactorMM"].CellAppearance.BackColor = Color.FromArgb(255,242,204);
            //e.Layout.Bands[0].Columns["NFactorMTR1"].CellAppearance.BackColor = Color.FromArgb(252,228,214);
            //e.Layout.Bands[0].Columns["NFactorMTR2"].CellAppearance.BackColor = Color.FromArgb(189,215,238);

            #endregion

            #region Posicion
            e.Layout.Bands[0].Columns["MM"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["FactorMM"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["MargenMM"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["MargenMM"].Format = "P2";
            e.Layout.Bands[0].Columns["NFactorMM"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["NMargenMM"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["NMargenMM"].Format = "P2";

            e.Layout.Bands[0].Columns["MTR1"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["FactorMTR1"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["MargenMTR1"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["MargenMTR1"].Format = "P2";
            e.Layout.Bands[0].Columns["NFactorMTR1"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["NMargenMTR1"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["NMargenMTR1"].Format = "P2";

            e.Layout.Bands[0].Columns["MTR2"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["FactorMTR2"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["MargenMTR2"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["MargenMTR2"].Format = "P2";
            e.Layout.Bands[0].Columns["NFactorMTR2"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["NMargenMTR2"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["NMargenMTR2"].Format = "P2";
            #endregion

            #region Editables
           
            e.Layout.Bands[0].Columns["NFactorMM"].CellActivation = Activation.AllowEdit;
            e.Layout.Bands[0].Columns["NFactorMTR1"].CellActivation = Activation.AllowEdit;
            e.Layout.Bands[0].Columns["NFactorMTR2"].CellActivation = Activation.AllowEdit;

            #endregion

            #region ancho
            e.Layout.Bands[0].Columns[(int)Columnas.ItemCode].Width = 90;
            e.Layout.Bands[0].Columns[(int)Columnas.ItemName].Width = 120;
            e.Layout.Bands[0].Columns[(int)Columnas.Venta].Width = 80;
            e.Layout.Bands[0].Columns[(int)Columnas.PC].Width = 80;
            e.Layout.Bands[0].Columns[(int)Columnas.Currency].Width = 80;
            e.Layout.Bands[0].Columns[(int)Columnas.CB].Width = 80;
            e.Layout.Bands[0].Columns[(int)Columnas.FactorCB].Width = 80;
            e.Layout.Bands[0].Columns[(int)Columnas.Utilidad].Width = 80;

            e.Layout.Bands[0].Columns[(int)Columnas.MM].Header.Caption = "Mínimo mayoreo";
            e.Layout.Bands[0].Columns[(int)Columnas.FactorMM].Header.Caption = "Factor";
            e.Layout.Bands[0].Columns[(int)Columnas.MargenMM].Header.Caption = "Margen";
            e.Layout.Bands[0].Columns[(int)Columnas.NFactorMM].Header.Caption = "Nuevo factor";
            e.Layout.Bands[0].Columns[(int)Columnas.NMargenMM].Header.Caption = "Nuevo margen";

            e.Layout.Bands[0].Columns[(int)Columnas.MTR1].Header.Caption = "Transporte R1";
            e.Layout.Bands[0].Columns[(int)Columnas.FactorMTR1].Header.Caption = "Factor";
            e.Layout.Bands[0].Columns[(int)Columnas.MargenMTR1].Header.Caption = "Margen";
            e.Layout.Bands[0].Columns[(int)Columnas.NFactorMTR1].Header.Caption = "Nuevo factor";
            e.Layout.Bands[0].Columns[(int)Columnas.NMargenMTR1].Header.Caption = "Nuevo margen";

            e.Layout.Bands[0].Columns[(int)Columnas.MTR2].Header.Caption = "Transporte R2";
            e.Layout.Bands[0].Columns[(int)Columnas.FactorMTR2].Header.Caption = "Factor";
            e.Layout.Bands[0].Columns[(int)Columnas.MargenMTR2].Header.Caption = "Margen";
            e.Layout.Bands[0].Columns[(int)Columnas.NFactorMTR2].Header.Caption = "Nuevo factor";
            e.Layout.Bands[0].Columns[(int)Columnas.NMargenMTR2].Header.Caption = "Nuevo margen";
            //, , , , ,
            //, , , , ,
            //, , , , 
            #endregion
        }

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    this.dgvDatos.PerformAction(UltraGridAction.ExitEditMode, false,
                      false);
                    this.dgvDatos.PerformAction(UltraGridAction.AboveCell, false,
                      false);
                    e.Handled = true;
                    this.dgvDatos.PerformAction(UltraGridAction.EnterEditMode, false,
                      false);
                    break;
                case Keys.Down:
                    this.dgvDatos.PerformAction(UltraGridAction.ExitEditMode, false,
                      false);
                    this.dgvDatos.PerformAction(UltraGridAction.BelowCell, false,
                      false);
                    e.Handled = true;
                    this.dgvDatos.PerformAction(UltraGridAction.EnterEditMode, false,
                      false);
                    break;
                case Keys.Right:
                    this.dgvDatos.PerformAction(UltraGridAction.ExitEditMode, false,
                      false);
                    this.dgvDatos.PerformAction(UltraGridAction.NextCellByTab, false,
                      false);
                    e.Handled = true;
                    this.dgvDatos.PerformAction(UltraGridAction.EnterEditMode, false,
                      false);
                    break;
                case Keys.Left:
                    this.dgvDatos.PerformAction(UltraGridAction.ExitEditMode, false,
                      false);
                    this.dgvDatos.PerformAction(UltraGridAction.PrevCellByTab, false,
                      false);
                    e.Handled = true;
                    this.dgvDatos.PerformAction(UltraGridAction.EnterEditMode, false,
                      false);
                    break;
            }
        }

        private void dgvDatos_KeyPress(object sender, KeyPressEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            UltraGridCell activeCell = grid == null ? null : grid.ActiveCell;

            // if there is an active cell, its not in edit mode and can enter edit mode
            if (null != activeCell && false == activeCell.IsInEditMode && activeCell.CanEnterEditMode)
            {
                // if the character is not a control character
                if (char.IsControl(e.KeyChar) == false)
                {
                    // try to put cell into edit mode
                    grid.PerformAction(UltraGridAction.EnterEditMode);

                    // if this cell is still active and it is in edit mode...
                    if (grid.ActiveCell == activeCell && activeCell.IsInEditMode)
                    {
                        // get its editor
                        EmbeddableEditorBase editor = this.dgvDatos.ActiveCell.EditorResolved;

                        // if the editor supports selectable text
                        if (editor.SupportsSelectableText)
                        {
                            // select all the text so it can be replaced
                            editor.SelectionStart = 0;
                            editor.SelectionLength = editor.TextLength;

                            if (editor is EditorWithMask)
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

        private void dgvDatos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            decimal d_mm = decimal.Zero;
            decimal d_r1 = decimal.Zero;
            decimal d_r2 = decimal.Zero;

            d_mm = Convert.ToDecimal(e.Row.Cells["MM"].Value);
            d_r1 = Convert.ToDecimal(e.Row.Cells["MTR1"].Value);
            d_r2 = Convert.ToDecimal(e.Row.Cells["MTR2"].Value);

            if (d_r2 > d_r1 || d_r2 < d_mm)
            {
                e.Row.Cells["MTR2"].Appearance.BackColor = Color.Red;
                e.Row.Cells["Error"].Value = "Y";
            }

            if (d_r1 < d_r2 && d_r1 <= d_mm)
            {
                e.Row.Cells["MTR1"].Appearance.BackColor = Color.Red;
                e.Row.Cells["Error"].Value = "Y";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Filtrar errores")
            {
                button1.Text = "Mostrar todo";
                Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                myConditions.CompareValue = "Y";
                myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.Equals;
                dgvDatos.Rows.ColumnFilters["Error"].FilterConditions.Add(myConditions);
            }
            else
            {
                button1.Text = "Filtrar errores";
                UltraGridBand band = this.dgvDatos.DisplayLayout.Bands[0];
                band.ColumnFilters["Error"].FilterConditions.Clear();
            }
        }
    }
}
