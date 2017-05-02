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

namespace H_Compras.Inventarios
{
    public partial class frmReubicaciones : Constantes.frmEmpty
    {
        public DataTable tbl_Articulos = new DataTable();
        private string TipoReubicacion;

        public enum Columnas1
        { 
            Linea,
            Articulo,
            Descripcion,
            Clasificacion, 
            sOrigen,
            iOrigen,
            sDestino,
            iDestino,
            Reubicar,
            Monto,
            Dias,
            Consigna,
            Fecha
        }

        public enum Columnas2
        {
            Linea,
            Articulo,
            Descripcion,
            Clasificacion,
            sOrigen,
            iOrigen,
            sDestino1,
            iDestino1,
            viDestino1,
            rDesino1,
            sDestino2,
            iDestino2,
            viDestino2,
            rDesino2,
            //Monto,
            Dias,
            Fecha,
            Consigna,
            Monto
        }

        public enum Columnas3
        {
            Linea,
            Articulo,
            Descripcion,
            Clasificacion,
            pStock,
            pIdeal,
            Disponible,
            mStock,
            mIdeal,
            mReubicar,
            mvi,
            eStock,
            eIdeal,
            eReubicar,
            evi,
            gStock,
            gIdeal,
            gReubicar,
            gvi,
            cReubicar,
            DiasOrigen,
            DiasDestino,
            Monto,
            Consigna,
            Fecha
        }

        public enum Columnas4
        {
            Linea,
            Articulo,
            Descripcion,
            Clasificacion,
            oStock,
            oIdeal,
            cStock,
            cReubicar,
            DiasOrigen,
            Monto,
            Consigna,
            UltimaEntrada
        }

        public enum Columnas5
        {
            Linea,
            Articulo,
            Descripcion,
            Clasificacion,
            sOrigen,
            iOrigen,
            sDestino,
            iDestino,
            Reubicar,
            Monto,
            Dias,
            Consigna,
            ultimaEntrada
        }

        public frmReubicaciones()
        {
            InitializeComponent();
        }

        private void frmReubicaciones_Load(object sender, EventArgs e)
        {
            try
            {
                this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                dgvDatos.DataSource = null;
                dgvTotales.DataSource = null;
                dgvStocks.DataSource = null;
                dgvVentas.DataSource = null;

                tbl_Articulos = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetArticulos, string.Empty, string.Empty);
                ClasesSGUV.Form.ControlsForms.setDataSource(cbLinea, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Lineas, null, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "---Selecciona una línea---");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbAlmacen, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesReubicaciones, null, string.Empty), "WhsName", "WhsCode", "---Selecciona un almacén---");
                ClasesSGUV.Form.ControlsForms.Autocomplete(txtArticulo, tbl_Articulos.Copy(), "ItemCode");
                this.dgvDatos.PerformAction(UltraGridAction.Copy, true, true);

                this.SetMensaje("Listo", 5000, Color.Green, Color.Black);

                dgvDatos.DisplayLayout.Override.RowSelectorHeaderStyle = Infragistics.Win.UltraWinGrid.RowSelectorHeaderStyle.ColumnChooserButton;
                dgvDatos.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
                dgvDatos.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;

                #region Evento Grid
                foreach (Infragistics.Win.UltraWinGrid.GridKeyActionMapping ugKey in dgvDatos.KeyActionMappings)
                {
                    if (ugKey.KeyCode == Keys.Enter)
                    {
                        dgvDatos.KeyActionMappings.Remove(ugKey);
                    }
                }


                this.dgvDatos.KeyActionMappings.Add(
                   new Infragistics.Win.UltraWinGrid.GridKeyActionMapping(
                   Keys.Enter,
                   Infragistics.Win.UltraWinGrid.UltraGridAction.BelowCell,
                   0,
                   0,
                   Infragistics.Win.SpecialKeys.All,
                   0));
                #endregion

                #region Eventos ToolStrip
                nuevoToolStripButton.Click -= new EventHandler(frmReubicaciones_Load);
                nuevoToolStripButton.Click += new EventHandler(frmReubicaciones_Load);
                actualizarToolStripButton.Click -= new EventHandler(frmReubicaciones_Load);
                actualizarToolStripButton.Click += new EventHandler(frmReubicaciones_Load);

                ayudaToolStripButton.Enabled = false;
                #endregion

                txtArticulo.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                dgvDatos.DataSource = null;
                dgvStocks.DataSource = null;
                dgvTotales.DataSource = null;
                dgvVentas.DataSource = null;

                if (cbAlmacen.SelectedIndex == 0)
                {
                    this.SetMensaje("Seleccione un Almacén origen", 5000, Color.Red, Color.White);
                    return;
                }

                #region Consulta
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Inventario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 1);
                        command.Parameters.AddWithValue("@Almacen_Origen", cbAlmacen.SelectedValue);
                        command.Parameters.AddWithValue("@CardCode", cbLinea.SelectedValue);
                        command.Parameters.AddWithValue("@ItemCode", txtArticulo.Text);
                        command.Parameters.AddWithValue("@nameOrigen", cbAlmacen.Text);
                        command.Parameters.AddWithValue("@ImpoNac", rbImportacion.Checked ? "IMP" : "NAC");
                        command.Parameters.Add("@TipoReubicacion", SqlDbType.VarChar, 10).Direction = ParameterDirection.Output;

                        DataTable table = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(table);

                        TipoReubicacion = Convert.ToString(command.Parameters["@TipoReubicacion"].Value.ToString());
                        dgvDatos.DataSource = table;

                        DataTable tbl_Totales = new DataTable();
                        #region API/COR/TEP
                        if (TipoReubicacion.Equals("RE1"))
                        {
                            Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                            myConditions.CompareValue = "30";
                            myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.GreaterThan;
                            dgvDatos.Rows.ColumnFilters[(int)Columnas1.Fecha].FilterConditions.Add(myConditions);
                            dgvDatos.Rows.ColumnFilters.LogicalOperator = FilterLogicalOperator.Or;

                            tbl_Totales.Columns.Add("Cantidad Reubicable (PZ)", typeof(decimal));
                            tbl_Totales.Columns.Add("Cantidad Reubicable ($)", typeof(decimal));

                            decimal totalM = decimal.Zero;
                            decimal totalP = decimal.Zero;

                            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow item in dgvDatos.Rows.GetFilteredInNonGroupByRows())
                            {
                                totalP += Convert.ToDecimal(item.Cells[(int)Columnas1.Reubicar].Value);
                                totalM += Convert.ToDecimal(item.Cells[(int)Columnas1.Monto].Value);
                            }

                            DataRow row = tbl_Totales.NewRow();
                            row[0] = totalP;
                            row[1] = totalM;

                            tbl_Totales.Rows.Add(row);
                        }
                        #endregion

                        #region MEX/GDL
                        if (TipoReubicacion.Equals("RE2"))
                        {
                            Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                            myConditions.CompareValue = "30";
                            myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.GreaterThan;
                            dgvDatos.Rows.ColumnFilters[(int)Columnas2.Fecha].FilterConditions.Add(myConditions);

                            tbl_Totales.Columns.Add("Cantidad Reubicable (PZ)", typeof(decimal));
                            tbl_Totales.Columns.Add("Cantidad Reubicable ($)", typeof(decimal));

                            decimal totalM = decimal.Zero;
                            decimal totalP = decimal.Zero;

                            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow item in dgvDatos.Rows.GetFilteredInNonGroupByRows())
                            {
                                totalP += (item.Cells[(int)Columnas2.rDesino1].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(item.Cells[(int)Columnas2.rDesino1].Value))
                                    + (item.Cells[(int)Columnas2.rDesino2].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(item.Cells[(int)Columnas2.rDesino2].Value));
                                totalM += Convert.ToDecimal(item.Cells[(int)Columnas2.Monto].Value);
                            }

                            DataRow row = tbl_Totales.NewRow();
                            row[0] = totalP;
                            row[1] = totalM;

                            tbl_Totales.Rows.Add(row);
                        }
                        #endregion

                        #region MTY/PUE
                        if (TipoReubicacion.Equals("RE3"))
                        {
                            Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                            myConditions.CompareValue = "30";
                            myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.GreaterThan;
                            dgvDatos.Rows.ColumnFilters[(int)Columnas3.Fecha].FilterConditions.Add(myConditions);

                            tbl_Totales.Columns.Add("Cantidad Reubicable (PZ)", typeof(decimal));
                            tbl_Totales.Columns.Add("Cantidad Reubicable ($)", typeof(decimal));

                            decimal totalM = decimal.Zero;
                            decimal totalP = decimal.Zero;

                            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow item in dgvDatos.Rows.GetFilteredInNonGroupByRows())
                            {
                                totalP += Convert.ToDecimal(item.Cells[(int)Columnas3.Disponible].Value);
                                totalM += Convert.ToDecimal(item.Cells[(int)Columnas3.Monto].Value);
                            }

                            DataRow row = tbl_Totales.NewRow();
                            row[0] = totalP;
                            row[1] = totalM;

                            tbl_Totales.Rows.Add(row);
                        }
                        #endregion

                        #region Reubicaciones al CEDIS
                        if (TipoReubicacion.Equals("RE4"))
                        {
                            Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                            myConditions.CompareValue = "30";
                            myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.GreaterThan;
                            dgvDatos.Rows.ColumnFilters[(int)Columnas4.UltimaEntrada].FilterConditions.Add(myConditions);

                            tbl_Totales.Columns.Add("Cantidad Reubicable (PZ)", typeof(decimal));
                            tbl_Totales.Columns.Add("Cantidad Reubicable ($)", typeof(decimal));

                            decimal totalM = decimal.Zero;
                            decimal totalP = decimal.Zero;

                            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow item in dgvDatos.Rows.GetFilteredInNonGroupByRows())
                            {
                                totalP += (item.Cells[(int)Columnas4.cReubicar].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(item.Cells[(int)Columnas4.cReubicar].Value));
                                totalM += Convert.ToDecimal(item.Cells[(int)Columnas4.Monto].Value);
                            }

                            DataRow row = tbl_Totales.NewRow();
                            row[0] = totalP;
                            row[1] = totalM;

                            tbl_Totales.Rows.Add(row);
                        }
                        #endregion

                        #region Reubicaciones Nacional PUEBLA|MONTERREY
                        if (TipoReubicacion.Equals("RE5"))
                        {
                            Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                            myConditions.CompareValue = "30";
                            myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.GreaterThan;
                            dgvDatos.Rows.ColumnFilters[(int)Columnas5.ultimaEntrada].FilterConditions.Add(myConditions);

                            if (cbLinea.SelectedIndex > 0)
                            {
                                Infragistics.Win.UltraWinGrid.FilterCondition myConditionsLinea = new Infragistics.Win.UltraWinGrid.FilterCondition();
                                myConditionsLinea.CompareValue = cbLinea.Text;
                                myConditionsLinea.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.Equals;
                                dgvDatos.Rows.ColumnFilters[(int)Columnas5.Linea].FilterConditions.Add(myConditionsLinea);
                            }
                            tbl_Totales.Columns.Add("Cantidad Reubicable (PZ)", typeof(decimal));
                            tbl_Totales.Columns.Add("Cantidad Reubicable ($)", typeof(decimal));

                            decimal totalM = decimal.Zero;
                            decimal totalP = decimal.Zero;

                            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow item in dgvDatos.Rows.GetFilteredInNonGroupByRows())
                            {
                                totalP += (item.Cells[(int)Columnas5.Reubicar].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(item.Cells[(int)Columnas5.Reubicar].Value));
                                totalM += Convert.ToDecimal(item.Cells[(int)Columnas5.Monto].Value);
                            }

                            DataRow row = tbl_Totales.NewRow();
                            row[0] = totalP;
                            row[1] = totalM;

                            tbl_Totales.Rows.Add(row);
                        }
                        #endregion

                        dgvTotales.DataSource = tbl_Totales;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtArticulo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(txtArticulo.Text))
                {
                    var proveedor = (from item in tbl_Articulos.AsEnumerable()
                                     where item.Field<string>("ItemCode").ToLower().Trim().Equals(txtArticulo.Text.ToLower().Trim())
                                     select item.Field<Int16>("ItmsGrpCod")).FirstOrDefault();

                    cbLinea.SelectedValue = proveedor ;//== null ? string.Empty : proveedor;

                    cbProveedor_SelectionChangeCommitted(sender, e);

                    
                }
                btnBuscar_Click(sender, e);
            }
        }

        private void cbProveedor_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception)
            {
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            #region Reubicacion Almacenes pequeños
            if (TipoReubicacion == "RE1")
            {
                e.Layout.Bands[0].Columns[(int)Columnas1.Dias].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas1.Monto].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas1.Fecha].Hidden = true;

                //e.Layout.Bands[0].Columns[(int)Columnas1.sOrigen].Header.Caption = "Stock " + cbAlmacen.Text;
                //e.Layout.Bands[0].Columns[(int)Columnas1.iOrigen].Header.Caption = "Ideal " + cbAlmacen.Text;
                //e.Layout.Bands[0].Columns[(int)Columnas1.sDestino].Header.Caption = "Stock " + cbAlmacen.Text;
                //e.Layout.Bands[0].Columns[(int)Columnas1.iDestino].Header.Caption = "Ideal " + cbAlmacen.Text;

                e.Layout.Bands[0].Columns[(int)Columnas1.Linea].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas1.Articulo].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas1.Descripcion].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas1.Clasificacion].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas1.sOrigen].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas1.iOrigen].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas1.sDestino].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas1.iDestino].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas1.Reubicar].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas1.Monto].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas1.Dias].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas1.Consigna].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas1.Fecha].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

                e.Layout.Bands[0].Columns[(int)Columnas1.Linea].Width = 90;
                e.Layout.Bands[0].Columns[(int)Columnas1.Articulo].Width = 100;
                e.Layout.Bands[0].Columns[(int)Columnas1.Descripcion].Width = 200;
                e.Layout.Bands[0].Columns[(int)Columnas1.Clasificacion].Width = 60;
                e.Layout.Bands[0].Columns[(int)Columnas1.sOrigen].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas1.iOrigen].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas1.sDestino].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas1.iDestino].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas1.Reubicar].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas1.Monto].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas1.Dias].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas1.Consigna].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas1.Fecha].Width = 80;

                e.Layout.Bands[0].Columns[(int)Columnas1.sDestino].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas1.iDestino].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas1.sOrigen].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas1.iOrigen].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas1.Reubicar].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas1.Monto].Format = "C2";
                e.Layout.Bands[0].Columns[(int)Columnas1.Dias].Format = "N0";

                e.Layout.Bands[0].Columns[(int)Columnas1.sDestino].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas1.iDestino].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas1.sOrigen].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas1.iOrigen].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas1.Reubicar].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas1.Reubicar].CellAppearance.BackColor = Color.FromName("Info");
                e.Layout.Bands[0].Columns[(int)Columnas1.Monto].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas1.Dias].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas1.Consigna].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

                e.Layout.Bands[0].Columns[(int)Columnas1.Articulo].Header.Fixed = true;
                e.Layout.Bands[0].Columns[(int)Columnas1.Linea].Header.Fixed = true;
                e.Layout.Bands[0].Columns[(int)Columnas1.Descripcion].Header.Fixed = true;

                e.Layout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy;
            }
            #endregion

            #region Reubicacion MEX, GDL
            if (TipoReubicacion.Equals("RE2"))
            {
                e.Layout.Bands[0].Columns[(int)Columnas2.Linea].Width = 90;
                e.Layout.Bands[0].Columns[(int)Columnas2.Articulo].Width = 100;
                e.Layout.Bands[0].Columns[(int)Columnas2.Descripcion].Width = 220;
                e.Layout.Bands[0].Columns[(int)Columnas2.Clasificacion].Width = 60;
                e.Layout.Bands[0].Columns[(int)Columnas2.sOrigen].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas2.iOrigen].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas2.sDestino1].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas2.iDestino1].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas2.viDestino1].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas2.rDesino1].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas2.sDestino2].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas2.iDestino2].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas2.viDestino2].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas2.rDesino2].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas2.Dias].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas2.Consigna].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas2.Fecha].Width = 80;

                e.Layout.Bands[0].Columns[(int)Columnas2.Linea].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.Articulo].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.Descripcion].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.Clasificacion].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.sOrigen].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.iOrigen].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.sDestino1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.iDestino1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.viDestino1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.rDesino1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.sDestino2].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.iDestino2].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.viDestino2].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.rDesino2].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.Dias].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.Consigna].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas2.Fecha].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

                e.Layout.Bands[0].Columns[(int)Columnas2.Dias].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas2.viDestino1].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas2.viDestino2].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas2.Fecha].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas2.Dias].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas2.Monto].Hidden = true;

                e.Layout.Bands[0].Columns[(int)Columnas2.sOrigen].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas2.iOrigen].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas2.sDestino1].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas2.iDestino1].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas2.viDestino1].Format = "N2";
                e.Layout.Bands[0].Columns[(int)Columnas2.rDesino1].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas2.sDestino2].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas2.iDestino2].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas2.viDestino2].Format = "N2";
                e.Layout.Bands[0].Columns[(int)Columnas2.rDesino2].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas2.Monto].Format = "C2";

                e.Layout.Bands[0].Columns[(int)Columnas2.sOrigen].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas2.iOrigen].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas2.sDestino1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas2.iDestino1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas2.viDestino1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas2.rDesino1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas2.sDestino2].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas2.iDestino2].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas2.viDestino2].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas2.rDesino2].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas2.Monto].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas2.Consigna].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

                e.Layout.Bands[0].Columns[(int)Columnas2.rDesino1].CellAppearance.BackColor = Color.FromName("Info");
                e.Layout.Bands[0].Columns[(int)Columnas2.rDesino2].CellAppearance.BackColor = Color.FromName("Info");
            }
            #endregion

            #region Reubicacion PUE, MTY Nacional
            if (TipoReubicacion.Equals("RE3"))
            {
                e.Layout.Bands[0].Columns[(int)Columnas3.Linea].Width = 90;
                e.Layout.Bands[0].Columns[(int)Columnas3.Articulo].Width = 100;
                e.Layout.Bands[0].Columns[(int)Columnas3.Descripcion].Width = 220;
                e.Layout.Bands[0].Columns[(int)Columnas3.Clasificacion].Width = 60;
                e.Layout.Bands[0].Columns[(int)Columnas3.Disponible].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas3.mStock].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas3.mIdeal].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas3.mReubicar].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas3.eStock].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas3.eIdeal].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas3.eReubicar].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas3.gStock].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas3.gIdeal].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas3.gReubicar].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas3.cReubicar].Width = 70;
                e.Layout.Bands[0].Columns[(int)Columnas3.Fecha].Width = 70;

                foreach (var item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                }

                e.Layout.Bands[0].Columns[(int)Columnas3.mStock].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas3.mIdeal].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas3.eStock].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas3.eIdeal].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas3.gStock].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas3.gIdeal].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas3.DiasDestino].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas3.DiasOrigen].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas3.Monto].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas3.Consigna].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas3.Fecha].Hidden = true;

                e.Layout.Bands[0].Columns[(int)Columnas3.pStock].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.pIdeal].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.Disponible].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.mStock].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.mIdeal].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.mReubicar].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.eStock].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.eIdeal].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.eReubicar].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.gStock].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.gIdeal].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.gReubicar].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.cReubicar].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.DiasOrigen].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.DiasDestino].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas3.Monto].Format = "C2";
                e.Layout.Bands[0].Columns[(int)Columnas3.evi].Format = "N2";
                e.Layout.Bands[0].Columns[(int)Columnas3.mvi].Format = "N2";
                e.Layout.Bands[0].Columns[(int)Columnas3.gvi].Format = "N2";

                e.Layout.Bands[0].Columns[(int)Columnas3.pStock].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.pIdeal].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.Disponible].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.mStock].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.mIdeal].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.mReubicar].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.eStock].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.eIdeal].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.eReubicar].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.gStock].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.gIdeal].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.gReubicar].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.cReubicar].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.DiasOrigen].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.DiasDestino].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.Monto].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                //e.Layout.Bands[0].Columns[(int)Columnas3.Fecha].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.evi].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.mvi].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.gvi].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas3.Consigna].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

                e.Layout.Bands[0].Columns[(int)Columnas3.mReubicar].CellAppearance.BackColor = Color.FromName("Info");
                e.Layout.Bands[0].Columns[(int)Columnas3.eReubicar].CellAppearance.BackColor = Color.FromName("Info");
                e.Layout.Bands[0].Columns[(int)Columnas3.gReubicar].CellAppearance.BackColor = Color.FromName("Info");
                e.Layout.Bands[0].Columns[(int)Columnas3.cReubicar].CellAppearance.BackColor = Color.FromName("Info");
            }
            #endregion

            #region Reubicacion al CEDIS
            if (TipoReubicacion == "RE4")
            {
                e.Layout.Bands[0].Columns[(int)Columnas4.Monto].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas4.UltimaEntrada].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas4.DiasOrigen].Hidden = true;

                e.Layout.Bands[0].Columns[(int)Columnas4.Linea].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas4.Articulo].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas4.Descripcion].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas4.Clasificacion].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas4.oStock].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas4.oIdeal].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas4.cReubicar].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas4.cStock].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas4.Monto].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas4.UltimaEntrada].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas4.DiasOrigen].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas4.Monto].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas4.Consigna].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

                e.Layout.Bands[0].Columns[(int)Columnas4.Linea].Width = 90;
                e.Layout.Bands[0].Columns[(int)Columnas4.Articulo].Width = 100;
                e.Layout.Bands[0].Columns[(int)Columnas4.Descripcion].Width = 200;
                e.Layout.Bands[0].Columns[(int)Columnas4.Clasificacion].Width = 60;
                e.Layout.Bands[0].Columns[(int)Columnas4.oStock].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas4.oIdeal].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas4.cStock].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas4.cReubicar].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas4.DiasOrigen].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas4.Monto].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas4.Consigna].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas4.UltimaEntrada].Width = 80;

                e.Layout.Bands[0].Columns[(int)Columnas4.oStock].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas4.oIdeal].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas4.cStock].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas4.cReubicar].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas4.DiasOrigen].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas4.Monto].Format = "C2";
                e.Layout.Bands[0].Columns[(int)Columnas4.UltimaEntrada].Format = "N0";

                e.Layout.Bands[0].Columns[(int)Columnas4.oStock].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas4.oIdeal].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas4.cStock].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas4.cReubicar].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas4.DiasOrigen].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas4.Monto].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas4.UltimaEntrada].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas4.Consigna].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

                e.Layout.Bands[0].Columns[(int)Columnas4.Articulo].Header.Fixed = true;
                e.Layout.Bands[0].Columns[(int)Columnas4.Linea].Header.Fixed = true;
                e.Layout.Bands[0].Columns[(int)Columnas4.Descripcion].Header.Fixed = true;

                e.Layout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy;
            }
            #endregion

            #region Reubicacion PUEBLA|MONTERREY
            if (TipoReubicacion == "RE5")
            {
                e.Layout.Bands[0].Columns[(int)Columnas5.Dias].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas5.Monto].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Columnas5.ultimaEntrada].Hidden = true;

                e.Layout.Bands[0].Columns[(int)Columnas5.sOrigen].Header.Caption = "Stock " + cbAlmacen.Text;
                e.Layout.Bands[0].Columns[(int)Columnas5.iOrigen].Header.Caption = "Ideal " + cbAlmacen.Text;

                if (cbAlmacen.Text.Equals("PUEBLA"))
                {
                    e.Layout.Bands[0].Columns[(int)Columnas5.sDestino].Header.Caption = "Stock MONTERREY";
                    e.Layout.Bands[0].Columns[(int)Columnas5.iDestino].Header.Caption = "Ideal MONTERREY";
                }
                if (cbAlmacen.Text.Equals("MONTERREY"))
                {
                    e.Layout.Bands[0].Columns[(int)Columnas5.sDestino].Header.Caption = "Stock PUEBLA";
                    e.Layout.Bands[0].Columns[(int)Columnas5.iDestino].Header.Caption = "Ideal PUEBLA";
                }

                e.Layout.Bands[0].Columns[(int)Columnas5.Linea].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas5.Articulo].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas5.Descripcion].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas5.Clasificacion].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas5.sOrigen].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas5.iOrigen].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas5.sDestino].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas5.iDestino].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas5.Reubicar].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas5.Monto].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas5.Dias].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas5.Consigna].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas5.ultimaEntrada].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

                e.Layout.Bands[0].Columns[(int)Columnas5.Linea].Width = 90;
                e.Layout.Bands[0].Columns[(int)Columnas5.Articulo].Width = 100;
                e.Layout.Bands[0].Columns[(int)Columnas5.Descripcion].Width = 200;
                e.Layout.Bands[0].Columns[(int)Columnas5.Clasificacion].Width = 60;
                e.Layout.Bands[0].Columns[(int)Columnas5.sOrigen].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas5.iOrigen].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas5.sDestino].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas5.iDestino].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas5.Reubicar].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas5.Monto].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas5.Dias].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas5.Consigna].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas5.ultimaEntrada].Width = 80;

                e.Layout.Bands[0].Columns[(int)Columnas5.sDestino].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas5.iDestino].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas5.sOrigen].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas5.iOrigen].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas5.Reubicar].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Columnas5.Monto].Format = "C2";
                e.Layout.Bands[0].Columns[(int)Columnas5.Dias].Format = "N0";

                e.Layout.Bands[0].Columns[(int)Columnas5.sDestino].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas5.iDestino].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas5.sOrigen].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas5.iOrigen].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas5.Reubicar].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas5.Reubicar].CellAppearance.BackColor = Color.FromName("Info");
                e.Layout.Bands[0].Columns[(int)Columnas5.Monto].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas5.Dias].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Columnas5.Consigna].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

                e.Layout.Bands[0].Columns[(int)Columnas5.Articulo].Header.Fixed = true;
                e.Layout.Bands[0].Columns[(int)Columnas5.Linea].Header.Fixed = true;
                e.Layout.Bands[0].Columns[(int)Columnas5.Descripcion].Header.Fixed = true;

                e.Layout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy;
            }
            #endregion

            e.Layout.Bands[0].Columns["conIdeal"].Hidden = true;

            e.Layout.Override.AllowMultiCellOperations = AllowMultiCellOperation.Copy;
        }

        private void dgvDatos_BeforeRowActivate(object sender, Infragistics.Win.UltraWinGrid.RowEventArgs e)
        {
            try
            {
                string _item = e.Row.Cells[(int)Columnas1.Articulo].Value.ToString();

                #region Stock - Ideal
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Compras", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 3);
                        command.Parameters.AddWithValue("@ItemCode", _item);
                        command.Parameters.AddWithValue("@Zona_Comprar", "IMPORTACION USA");

                        SqlDataAdapter da = new SqlDataAdapter();
                        DataTable table = new DataTable();
                        da.SelectCommand = command;
                        da.Fill(table);


                        table.TableName = "Detalle";

                        var qryHeader = (from item in table.AsEnumerable()
                                         group item by new
                                         {
                                             ItemCode = item.Field<string>("ItemCode"),
                                             Zona = item.Field<string>("Zona")
                                         } into grouped
                                         select new
                                         {
                                             ItemCode = grouped.Key.ItemCode,
                                             Zona = grouped.Key.Zona,
                                             Ideal = grouped.Sum(ix => ix.Field<decimal>("Ideal")),
                                             Stock = grouped.Sum(ix => ix.Field<decimal>("Stock")),
                                             Solicitado = grouped.Sum(ix => ix.Field<decimal>("Solicitado"))
                                         }).ToList();

                        DataTable tbl_header = new DataTable();
                        tbl_header = Datos.Clases.ListConverter.ToDataTable(qryHeader);
                        tbl_header.TableName = "Header";

                        DataSet ds = new DataSet();
                        ds.Tables.Add(tbl_header);
                        ds.Tables.Add(table);


                        ds.Relations.Add("UNION", ds.Tables[0].Columns["Zona"], ds.Tables[1].Columns["Zona"]);

                        dgvStocks.DataSource = null;
                        dgvStocks.DataSource = ds;

                        dgvStocks.Rows.ExpandAll(true);
                    }
                }
                #endregion

                #region Ventas
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_HistorialVentas", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 1);
                        command.Parameters.AddWithValue("@Articulo", _item);
                        command.Parameters.AddWithValue("@Zona", "IMPORTACION USA");

                        SqlDataAdapter da = new SqlDataAdapter();
                        DataTable table = new DataTable();
                        da.SelectCommand = command;
                        da.Fill(table);


                        table.TableName = "Detalle";

                        var qryHeader = (from item in table.AsEnumerable()
                                         group item by new
                                         {
                                             ItemCode = item.Field<string>("Artículo"),
                                             Zona = item.Field<string>("Zona")
                                         } into grouped
                                         select new
                                         {
                                             ItemCode = grouped.Key.ItemCode,
                                             Zona = grouped.Key.Zona,
                                             M1 = grouped.Sum(ix => ix.Field<decimal>(3)),
                                             M2 = grouped.Sum(ix => ix.Field<decimal>(4)),
                                             M3 = grouped.Sum(ix => ix.Field<decimal>(5)),
                                             M4 = grouped.Sum(ix => ix.Field<decimal>(6)),
                                             M5 = grouped.Sum(ix => ix.Field<decimal>(7)),
                                             M6 = grouped.Sum(ix => ix.Field<decimal>(8)),
                                             M7 = grouped.Sum(ix => ix.Field<decimal>(9)),
                                             M8 = grouped.Sum(ix => ix.Field<decimal>(10)),
                                             M9 = grouped.Sum(ix => ix.Field<decimal>(11)),
                                             M10 = grouped.Sum(ix => ix.Field<decimal>(12)),
                                             M11 = grouped.Sum(ix => ix.Field<decimal>(13)),
                                             M12 = grouped.Sum(ix => ix.Field<decimal>(14)),
                                             MT = grouped.Sum(ix => ix.Field<decimal>(15))
                                         }).ToList();

                        DataTable tbl_header = new DataTable();
                        tbl_header = Datos.Clases.ListConverter.ToDataTable(qryHeader);
                        tbl_header.TableName = "Header";

                        DataSet ds = new DataSet();
                        ds.Tables.Add(tbl_header);
                        ds.Tables.Add(table);


                        ds.Relations.Add("UNION", ds.Tables[0].Columns["Zona"], ds.Tables[1].Columns["Zona"]);

                        dgvVentas.DataSource = null;
                        dgvVentas.DataSource = ds;

                        dgvVentas.Rows.ExpandAll(true);
                    }
                }
                #endregion
            }
            catch (Exception)
            {

            }
        }

        private void dgvStocks_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Bands[1].ColHeadersVisible = false;

                foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
                {
                    item.Width = 80;
                    item.Format = "N0";
                    item.CellAppearance.TextHAlign = HAlign.Right;
                    item.CellActivation = Activation.NoEdit;
                    item.Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
                }

                foreach (UltraGridColumn item in e.Layout.Bands[1].Columns)
                {
                    item.Width = 80;
                    item.Format = "N0";
                    item.CellAppearance.TextHAlign = HAlign.Right;
                    item.CellActivation = Activation.NoEdit;
                    item.Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
                }

                e.Layout.Bands[0].Columns[0].CellAppearance.TextHAlign = HAlign.Left;
                e.Layout.Bands[1].Columns[3].CellAppearance.TextHAlign = HAlign.Left;

                e.Layout.Bands[0].Columns[0].Header.Fixed = true;
                e.Layout.Bands[1].Columns[3].Header.Fixed = true;

                e.Layout.Bands[0].Columns[0].Header.Caption = "Artículo";
                e.Layout.Bands[1].Columns[3].Header.Caption = "Almacén";

                e.Layout.Bands[0].Columns[1].Hidden = true;
                e.Layout.Bands[1].Columns[0].Hidden = true;
                e.Layout.Bands[1].Columns[1].Hidden = true;
                e.Layout.Bands[1].Columns[2].Hidden = true;

                e.Layout.Override.AllowMultiCellOperations = AllowMultiCellOperation.Copy;
            }
            catch (Exception) { }
        }

        private void dgvVentas_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[1].ColHeadersVisible = false;

            try
            {
                foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn item in e.Layout.Bands[0].Columns)
                {
                    item.Width = 70;
                    item.Format = "N0";
                    item.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                    item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                    item.Header.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
                }

                foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn item in e.Layout.Bands[1].Columns)
                {
                    item.Width = 70;
                    item.Format = "N0";
                    item.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                    item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                    item.Header.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
                }

                e.Layout.Bands[0].Columns[0].Header.Fixed = true;
                e.Layout.Bands[0].Columns[1].Header.Fixed = true;
                e.Layout.Bands[1].Columns[1].Header.Fixed = true;

                e.Layout.Bands[0].Columns[1].Hidden = true;
                e.Layout.Bands[1].Columns[0].Hidden = true;
                e.Layout.Bands[1].Columns[2].Hidden = true;

                e.Layout.Bands[0].Columns[0].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                e.Layout.Bands[0].Columns[1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                e.Layout.Bands[1].Columns[1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

                e.Layout.Bands[0].Columns[0].Header.Caption = "Artículo";
                e.Layout.Bands[1].Columns[1].Header.Caption = "Almacén";

                e.Layout.Bands[0].Columns[2].Header.Caption = e.Layout.Bands[1].Columns[3].Header.Caption;
                e.Layout.Bands[0].Columns[3].Header.Caption = e.Layout.Bands[1].Columns[4].Header.Caption;
                e.Layout.Bands[0].Columns[4].Header.Caption = e.Layout.Bands[1].Columns[5].Header.Caption;
                e.Layout.Bands[0].Columns[5].Header.Caption = e.Layout.Bands[1].Columns[6].Header.Caption;
                e.Layout.Bands[0].Columns[6].Header.Caption = e.Layout.Bands[1].Columns[7].Header.Caption;
                e.Layout.Bands[0].Columns[7].Header.Caption = e.Layout.Bands[1].Columns[8].Header.Caption;
                e.Layout.Bands[0].Columns[8].Header.Caption = e.Layout.Bands[1].Columns[9].Header.Caption;
                e.Layout.Bands[0].Columns[9].Header.Caption = e.Layout.Bands[1].Columns[10].Header.Caption;
                e.Layout.Bands[0].Columns[10].Header.Caption = e.Layout.Bands[1].Columns[11].Header.Caption;
                e.Layout.Bands[0].Columns[11].Header.Caption = e.Layout.Bands[1].Columns[12].Header.Caption;
                e.Layout.Bands[0].Columns[12].Header.Caption = e.Layout.Bands[1].Columns[13].Header.Caption;
                e.Layout.Bands[0].Columns[13].Header.Caption = e.Layout.Bands[1].Columns[14].Header.Caption;
                e.Layout.Bands[0].Columns[14].Header.Caption = e.Layout.Bands[1].Columns[15].Header.Caption;

                e.Layout.Override.AllowMultiCellOperations = AllowMultiCellOperation.Copy;

                e.Layout.Bands[0].Columns["conIdeal"].Hidden = true; 
            }
            catch (Exception)
            {

            }
        }

        private void dgvDatos_Click(object sender, EventArgs e)
        {
            Ug = (sender as Infragistics.Win.UltraWinGrid.UltraGrid);

            Dg = null;
        }

        private void dgvTotales_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            #region Reubicacion Almacenes pequeños
            if (TipoReubicacion == "RE1" || TipoReubicacion == "RE4")
            {
                e.Layout.Bands[0].Columns[0].Width = 95;
                e.Layout.Bands[0].Columns[1].Width = 95;

                e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

                e.Layout.Bands[0].Columns[0].Format = "N0";
                e.Layout.Bands[0].Columns[1].Format = "C2";

                e.Layout.Bands[0].Columns[0].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

                e.Layout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy;
            }
            #endregion

            #region Reubicacion MEX, GDL
            if (TipoReubicacion.Equals("RE2"))
            {
                e.Layout.Bands[0].Columns[0].Width = 95;
                e.Layout.Bands[0].Columns[1].Width = 95;

                e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

                e.Layout.Bands[0].Columns[0].Format = "N0";
                e.Layout.Bands[0].Columns[1].Format = "C2";

                e.Layout.Bands[0].Columns[0].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            }
            #endregion

            #region Reubicacion PUE, MTY
            if (TipoReubicacion.Equals("RE3"))
            {
                e.Layout.Bands[0].Columns[0].Width = 95;
                e.Layout.Bands[0].Columns[1].Width = 95;

                foreach (var item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                }

                e.Layout.Bands[0].Columns[0].Format = "N0";
                e.Layout.Bands[0].Columns[1].Format = "C2";

                e.Layout.Bands[0].Columns[0].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            }
            #endregion

            #region Reubicacion AL CEDIS
            if (TipoReubicacion.Equals("RE4"))
            {
                e.Layout.Bands[0].Columns[0].Width = 95;
                e.Layout.Bands[0].Columns[1].Width = 95;

                foreach (var item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                }

                e.Layout.Bands[0].Columns[0].Format = "N0";
                e.Layout.Bands[0].Columns[1].Format = "C2";

                e.Layout.Bands[0].Columns[0].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            }
            #endregion

            #region Reubicacion NACIONAL PUEBLA|MONTERREY
            if (TipoReubicacion.Equals("RE5"))
            {
                e.Layout.Bands[0].Columns[0].Width = 95;
                e.Layout.Bands[0].Columns[1].Width = 95;

                foreach (var item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                }

                e.Layout.Bands[0].Columns[0].Format = "N0";
                e.Layout.Bands[0].Columns[1].Format = "C2";

                e.Layout.Bands[0].Columns[0].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            }
            #endregion
        }

        private void dgvDatos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (Convert.ToDecimal(e.Row.Cells["conIdeal"].Value == DBNull.Value ? decimal.Zero : e.Row.Cells["conIdeal"].Value) <= 4)
            {
                e.Row.Cells[0].Appearance.BackColor = Color.Red;
                e.Row.Cells[0].Appearance.ForeColor = Color.White;
            }
        }

        private void dgvDatos_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
        {
            DataTable tbl_Totales = new DataTable();
            #region API/COR/TEP
            if (TipoReubicacion.Equals("RE1"))
            {
                Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                myConditions.CompareValue = "30";
                myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.GreaterThan;
                dgvDatos.Rows.ColumnFilters[(int)Columnas1.Fecha].FilterConditions.Add(myConditions);

                tbl_Totales.Columns.Add("Cantidad Reubicable (PZ)", typeof(decimal));
                tbl_Totales.Columns.Add("Cantidad Reubicable ($)", typeof(decimal));

                decimal totalM = decimal.Zero;
                decimal totalP = decimal.Zero;

                foreach (Infragistics.Win.UltraWinGrid.UltraGridRow item in dgvDatos.Rows.GetFilteredInNonGroupByRows())
                {
                    totalP += Convert.ToDecimal(item.Cells[(int)Columnas1.Reubicar].Value);
                    totalM += Convert.ToDecimal(item.Cells[(int)Columnas1.Monto].Value);
                }

                DataRow row = tbl_Totales.NewRow();
                row[0] = totalP;
                row[1] = totalM;

                tbl_Totales.Rows.Add(row);
            }
            #endregion

            #region MEX/GDL
            if (TipoReubicacion.Equals("RE2"))
            {
                Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                myConditions.CompareValue = "30";
                myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.GreaterThan;
                dgvDatos.Rows.ColumnFilters[(int)Columnas2.Fecha].FilterConditions.Add(myConditions);

                tbl_Totales.Columns.Add("Cantidad Reubicable (PZ)", typeof(decimal));
                tbl_Totales.Columns.Add("Cantidad Reubicable ($)", typeof(decimal));

                decimal totalM = decimal.Zero;
                decimal totalP = decimal.Zero;

                foreach (Infragistics.Win.UltraWinGrid.UltraGridRow item in dgvDatos.Rows.GetFilteredInNonGroupByRows())
                {
                    totalP += (item.Cells[(int)Columnas2.rDesino1].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(item.Cells[(int)Columnas2.rDesino1].Value))
                        + (item.Cells[(int)Columnas2.rDesino2].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(item.Cells[(int)Columnas2.rDesino2].Value));
                    totalM += Convert.ToDecimal(item.Cells[(int)Columnas2.Monto].Value);
                }

                DataRow row = tbl_Totales.NewRow();
                row[0] = totalP;
                row[1] = totalM;

                tbl_Totales.Rows.Add(row);
            }
            #endregion

            #region MTY/PUE
            if (TipoReubicacion.Equals("RE3"))
            {
                Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                myConditions.CompareValue = "30";
                myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.GreaterThan;
                dgvDatos.Rows.ColumnFilters[(int)Columnas3.Fecha].FilterConditions.Add(myConditions);

                tbl_Totales.Columns.Add("Cantidad Reubicable (PZ)", typeof(decimal));
                tbl_Totales.Columns.Add("Cantidad Reubicable ($)", typeof(decimal));

                decimal totalM = decimal.Zero;
                decimal totalP = decimal.Zero;

                foreach (Infragistics.Win.UltraWinGrid.UltraGridRow item in dgvDatos.Rows.GetFilteredInNonGroupByRows())
                {
                    totalP += Convert.ToDecimal(item.Cells[(int)Columnas3.Disponible].Value);
                    totalM += Convert.ToDecimal(item.Cells[(int)Columnas3.Monto].Value);
                }

                DataRow row = tbl_Totales.NewRow();
                row[0] = totalP;
                row[1] = totalM;

                tbl_Totales.Rows.Add(row);
            }
            #endregion

            #region Reubicaciones al CEDIS
            if (TipoReubicacion.Equals("RE4"))
            {
                Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                myConditions.CompareValue = "30";
                myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.GreaterThan;
                dgvDatos.Rows.ColumnFilters[(int)Columnas4.UltimaEntrada].FilterConditions.Add(myConditions);

                tbl_Totales.Columns.Add("Cantidad Reubicable (PZ)", typeof(decimal));
                tbl_Totales.Columns.Add("Cantidad Reubicable ($)", typeof(decimal));

                decimal totalM = decimal.Zero;
                decimal totalP = decimal.Zero;

                foreach (Infragistics.Win.UltraWinGrid.UltraGridRow item in dgvDatos.Rows.GetFilteredInNonGroupByRows())
                {
                    totalP += (item.Cells[(int)Columnas4.cReubicar].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(item.Cells[(int)Columnas4.cReubicar].Value));
                    totalM += Convert.ToDecimal(item.Cells[(int)Columnas4.Monto].Value);
                }

                DataRow row = tbl_Totales.NewRow();
                row[0] = totalP;
                row[1] = totalM;

                tbl_Totales.Rows.Add(row);
            }
            #endregion

            #region Reubicaciones Nacional PUEBLA|MONTERREY
            if (TipoReubicacion.Equals("RE5"))
            {
                Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                myConditions.CompareValue = "30";
                myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.GreaterThan;
                dgvDatos.Rows.ColumnFilters[(int)Columnas5.ultimaEntrada].FilterConditions.Add(myConditions);

                if (cbLinea.SelectedIndex > 0)
                {
                    Infragistics.Win.UltraWinGrid.FilterCondition myConditionsLinea = new Infragistics.Win.UltraWinGrid.FilterCondition();
                    myConditionsLinea.CompareValue = cbLinea.Text;
                    myConditionsLinea.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.Equals;
                    dgvDatos.Rows.ColumnFilters[(int)Columnas5.Linea].FilterConditions.Add(myConditionsLinea);
                }
                tbl_Totales.Columns.Add("Cantidad Reubicable (PZ)", typeof(decimal));
                tbl_Totales.Columns.Add("Cantidad Reubicable ($)", typeof(decimal));

                decimal totalM = decimal.Zero;
                decimal totalP = decimal.Zero;

                foreach (Infragistics.Win.UltraWinGrid.UltraGridRow item in dgvDatos.Rows.GetFilteredInNonGroupByRows())
                {
                    totalP += (item.Cells[(int)Columnas5.Reubicar].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(item.Cells[(int)Columnas5.Reubicar].Value));
                    totalM += Convert.ToDecimal(item.Cells[(int)Columnas5.Monto].Value);
                }

                DataRow row = tbl_Totales.NewRow();
                row[0] = totalP;
                row[1] = totalM;

                tbl_Totales.Rows.Add(row);
            }
            #endregion

            dgvTotales.DataSource = tbl_Totales;
        }
    }
}
