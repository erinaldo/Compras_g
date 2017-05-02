using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;


namespace H_Compras.Inventarios
{
    public partial class frmTraspasos : Constantes.frmEmpty
    {
        public DataTable tbl_Articulos = new DataTable();
        string TipoFormato;
        public frmTraspasos()
        {
            InitializeComponent();
        }

        private enum Columnas1
        {
            Linea,
            Articulo,
            Descripcion,
            Clasificacion,
            oStock,
            oIdeal,
            Disponible,
            dStock,
            dIdeal,
            Transferir,
            Monto, 
            dOrigen,
            dDestino,
            Peso
        }

        private void frmTraspasos_Load(object sender, EventArgs e)
        {
            try
            {
                this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                tbl_Articulos = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetArticulos, string.Empty, string.Empty);

                ClasesSGUV.Form.ControlsForms.setDataSource(cbOrigen, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.OrigenesTraspasos, null, string.Empty), "WhsName", "WhsCode", "Origen");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbLinea, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Lineas, null, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "Todas");

                ClasesSGUV.Form.ControlsForms.Autocomplete(txtArticulo, tbl_Articulos.Copy(), "ItemCode");

                this.nuevoToolStripButton.Click -= new EventHandler(btnNuevo_Click);
                this.nuevoToolStripButton.Click += new EventHandler(btnNuevo_Click);

                #region Evento Grid
                this.dgvDatos.PerformAction(UltraGridAction.Copy, true, true);

                dgvDatos.DisplayLayout.Override.RowSelectorHeaderStyle = Infragistics.Win.UltraWinGrid.RowSelectorHeaderStyle.ColumnChooserButton;
                dgvDatos.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
                dgvDatos.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;

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

                dgvDatos.KeyPress -= new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);
                dgvDatos.KeyPress += new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);

                dgvDatos.KeyDown -= new System.Windows.Forms.KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);
                dgvDatos.KeyDown += new System.Windows.Forms.KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);
                #endregion
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void cbOrigen_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ClasesSGUV.Form.ControlsForms.setDataSource(cbDestino, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.DestinosTraspasos, cbOrigen.SelectedValue, string.Empty), "WhsName", "WhsCode", "Destino");
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                dgvDatos.DataSource = null;
                dgvStocks.DataSource = null;
                dgvVentas.DataSource = null;
                dgvTotales.DataSource = null;

                if (cbOrigen.SelectedIndex == 0)
                {
                    this.SetMensaje("Seleccione un almacén origen", 5000, Color.Red, Color.White);
                    return;
                }

                if (cbDestino.SelectedIndex == 0)
                {
                    this.SetMensaje("Seleccione un almacén destino", 5000, Color.Red, Color.White);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Inventario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 13);
                        command.Parameters.AddWithValue("@Almacen_Origen", cbOrigen.SelectedValue);
                        command.Parameters.AddWithValue("@Almacen_Destino", cbDestino.SelectedValue);
                        command.Parameters.AddWithValue("@ItemCode", txtArticulo.Text);
                        command.Parameters.AddWithValue("@CardCode", cbLinea.SelectedValue);
                        command.Parameters.AddWithValue("@nameOrigen", cbOrigen.Text);
                        command.Parameters.AddWithValue("@nameDestino", cbDestino.Text);
                        command.Parameters.Add("@TipoReubicacion", SqlDbType.VarChar, 10).Direction = ParameterDirection.Output;

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        DataTable table = new DataTable();
                        da.Fill(table);

                        TipoFormato = Convert.ToString(command.Parameters["@TipoReubicacion"].Value.ToString());
                        dgvDatos.DataSource = table;

                        dgvDatos.Rows.ColumnFilters[(int)Columnas1.Linea].FilterConditions.Clear();
                        dgvDatos.Rows.ColumnFilters[(int)Columnas1.Articulo].FilterConditions.Clear();

                        if (Convert.ToInt32(cbLinea.SelectedValue) != 0)
                        {
                            Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                            myConditions.CompareValue = cbLinea.Text;
                            myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.Equals;
                            dgvDatos.Rows.ColumnFilters[(int)Columnas1.Linea].FilterConditions.Add(myConditions);
                        }
                        if (!string.IsNullOrEmpty(txtArticulo.Text))
                        {
                            Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                            myConditions.CompareValue = txtArticulo.Text;
                            myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.Equals;
                            dgvDatos.Rows.ColumnFilters[(int)Columnas1.Articulo].FilterConditions.Add(myConditions);
                        }

                        if (TipoFormato.Equals("TRA1"))
                        {
                            DataTable tbl = new DataTable();
                            tbl.Columns.Add("Total (PZ)", typeof(decimal));
                            tbl.Columns.Add("Total ($)", typeof(decimal));
                            tbl.Columns.Add("Peso (KG)", typeof(decimal));
                            tbl.Columns.Add("Volumen (ft3)", typeof(decimal));

                            DataRow row = tbl.NewRow();
                            decimal pz = decimal.Zero; decimal mm = decimal.Zero; decimal peso = decimal.Zero; decimal vol = decimal.Zero;
                            foreach (UltraGridRow item in dgvDatos.Rows)
                            {
                                if (!item.IsFilteredOut)
                                {
                                    pz += Convert.ToDecimal(item.Cells[(int)Columnas1.Transferir].Value);
                                    mm += Convert.ToDecimal(item.Cells[(int)Columnas1.Monto].Value);
                                    peso += Convert.ToDecimal(item.Cells["TP"].Value == DBNull.Value ? decimal.Zero
                                        : item.Cells["TP"].Value);
                                    vol += Convert.ToDecimal(item.Cells["Volumen"].Value);
                                }
                            }
                            row[0] = pz;
                            row[1] = mm;
                            row[2] = peso;
                            row[3] = vol;

                            tbl.Rows.Add(row);
                            dgvTotales.DataSource = tbl;

                            Infragistics.Win.UltraWinCalcManager.UltraCalcManager calcManager;
                            calcManager = new Infragistics.Win.UltraWinCalcManager.UltraCalcManager(this.Container);
                            dgvDatos.CalcManager = calcManager;
                            calcManager.ReCalc();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Activation.NoEdit;
            }

            UltraGridBand band = e.Layout.Bands[0];
            if (TipoFormato.Equals("TRA1"))
            {
                band.Columns[(int)Columnas1.Linea].Width = 90;
                band.Columns[(int)Columnas1.Articulo].Width = 100;
                band.Columns[(int)Columnas1.Descripcion].Width = 250;
                band.Columns[(int)Columnas1.Clasificacion].Width = 90;
                band.Columns[(int)Columnas1.oStock].Width = 80;
                band.Columns[(int)Columnas1.oIdeal].Width = 80;
                band.Columns[(int)Columnas1.Disponible].Width = 80;
                band.Columns[(int)Columnas1.dStock].Width = 80;
                band.Columns[(int)Columnas1.dIdeal].Width = 80;
                band.Columns[(int)Columnas1.Transferir].Width = 80;
                band.Columns[(int)Columnas1.Monto].Width = 80;
                band.Columns["P"].Width = 90;
                band.Columns["TP"].Width = 90;
                band.Columns["VolumenU"].Width = 90;
                band.Columns["Volumen"].Width = 90;

                band.Columns[(int)Columnas1.oStock].Header.Caption = "Stock " + cbOrigen.Text;
                band.Columns[(int)Columnas1.oIdeal].Header.Caption = "Ideal " + cbOrigen.Text;
                band.Columns[(int)Columnas1.Disponible].Header.Caption = "Disponible";
                band.Columns["TP"].Header.Caption = "Peso (KG)";
                band.Columns["Volumen"].Header.Caption = "Volumen (ft3)";

                band.Columns[(int)Columnas1.dStock].Header.Caption = "Stock " + cbDestino.Text;
                band.Columns[(int)Columnas1.dIdeal].Header.Caption = "Ideal " + cbDestino.Text;

                e.Layout.Bands[0].Columns["P"].Header.Caption = "% de abastecimiento";
                e.Layout.Bands[0].Columns["TP"].Format = "Peso";

                band.Columns[(int)Columnas1.dOrigen].Hidden = true;
                band.Columns[(int)Columnas1.dDestino].Hidden = true;
                band.Columns[(int)Columnas1.Peso].Hidden = true;
                band.Columns["PriceU"].Hidden = true;
                band.Columns["VolumenU"].Hidden = true;

                band.Columns[(int)Columnas1.oStock].Format = "N0";
                band.Columns[(int)Columnas1.oIdeal].Format = "N0";
                band.Columns[(int)Columnas1.Disponible].Format = "N0";
                band.Columns[(int)Columnas1.dStock].Format = "N0";
                band.Columns[(int)Columnas1.dIdeal].Format = "N0";
                band.Columns[(int)Columnas1.Transferir].Format = "N0";
                band.Columns[(int)Columnas1.Peso].Format = "N2";
                band.Columns[(int)Columnas1.Monto].Format ="C2";
                e.Layout.Bands[0].Columns["P"].Format = "P2";
                e.Layout.Bands[0].Columns["TP"].Format = "N2";
                e.Layout.Bands[0].Columns["Volumen"].Format = "N3";

                band.Columns[(int)Columnas1.oStock].CellAppearance.TextHAlign = HAlign.Right;
                band.Columns[(int)Columnas1.oIdeal].CellAppearance.TextHAlign = HAlign.Right;
                band.Columns[(int)Columnas1.Disponible].CellAppearance.TextHAlign = HAlign.Right;
                band.Columns[(int)Columnas1.dStock].CellAppearance.TextHAlign = HAlign.Right;
                band.Columns[(int)Columnas1.dIdeal].CellAppearance.TextHAlign = HAlign.Right;
                band.Columns[(int)Columnas1.Transferir].CellAppearance.TextHAlign = HAlign.Right;
                band.Columns[(int)Columnas1.Monto].CellAppearance.TextHAlign = HAlign.Right;
                band.Columns[(int)Columnas1.dDestino].CellAppearance.TextHAlign = HAlign.Right;
                band.Columns[(int)Columnas1.dOrigen].CellAppearance.TextHAlign = HAlign.Right;
                band.Columns[(int)Columnas1.Peso].CellAppearance.TextHAlign = HAlign.Right;
                e.Layout.Bands[0].Columns["P"].CellAppearance.TextHAlign = HAlign.Right;
                e.Layout.Bands[0].Columns["TP"].CellAppearance.TextHAlign = HAlign.Right;
                e.Layout.Bands[0].Columns["Volumen"].CellAppearance.TextHAlign = HAlign.Right;
                e.Layout.Bands[0].Columns["VolumenU"].CellAppearance.TextHAlign = HAlign.Right;

                band.Columns[(int)Columnas1.Transferir].CellAppearance.BackColor = Color.FromName("Info");
                band.Columns[(int)Columnas1.Transferir].CellActivation = Activation.AllowEdit;
            }

            e.Layout.Override.AllowMultiCellOperations = AllowMultiCellOperation.Copy;

            e.Layout.Bands[0].Columns["P"].Formula = "(([Stock Destino]) / [Ideal Destino])";
            e.Layout.Bands[0].Columns["TP"].Formula = "[Transferir] * [PesoU]";
            e.Layout.Bands[0].Columns["Monto"].Formula = "[Transferir] * [PriceU]";
            e.Layout.Bands[0].Columns["Volumen"].Formula = "[Transferir] * [VolumenU]";

            e.Layout.Bands[0].Columns["I"].Hidden = true;

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

        private void dgvStocks_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Bands[1].ColHeadersVisible = false;

                foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
                {
                    item.Width = 70;
                    item.Format = "N0";
                    item.CellAppearance.TextHAlign = HAlign.Right;
                    item.CellActivation = Activation.NoEdit;
                    item.Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
                }

                foreach (UltraGridColumn item in e.Layout.Bands[1].Columns)
                {
                    item.Width = 70;
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
            }
            catch (Exception) { }
        }

        private void dgvVentas_InitializeLayout(object sender, InitializeLayoutEventArgs e)
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

            }
            catch (Exception)
            {

            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            txtArticulo.Clear();
            cbOrigen.SelectedIndex = 0;
            cbDestino.SelectedIndex = 0;
            cbLinea.SelectedIndex = 0;

            dgvDatos.DataSource = null;
            dgvStocks.DataSource = null;
            dgvVentas.DataSource = null;

            txtArticulo.Focus();
        }

        private void dgvDatos_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
        {
            if (TipoFormato.Equals("TRA1"))
            {
                DataTable tbl = new DataTable();
                tbl.Columns.Add("Total (PZ)", typeof(decimal));
                tbl.Columns.Add("Total ($)", typeof(decimal));
                tbl.Columns.Add("Peso (KG)", typeof(decimal));
                tbl.Columns.Add("Volumen (ft3)", typeof(decimal));

                DataRow row = tbl.NewRow();
                decimal pz = decimal.Zero; decimal mm = decimal.Zero; decimal peso = decimal.Zero; decimal vol = decimal.Zero;

                foreach (UltraGridRow item in dgvDatos.Rows)
                {
                    if (!item.IsFilteredOut)
                    {
                        pz += Convert.ToDecimal(item.Cells[(int)Columnas1.Transferir].Value);
                        mm += Convert.ToDecimal(item.Cells[(int)Columnas1.Monto].Value);
                        peso += Convert.ToDecimal(item.Cells["TP"].Value == DBNull.Value ? decimal.Zero :
                                Convert.ToDecimal(item.Cells["TP"].Value));
                        vol += Convert.ToDecimal(item.Cells["Volumen"].Value);
                    }
                }
                row[0] = pz;
                row[1] = mm;
                row[2] = peso;
                row[3] = vol;

                tbl.Rows.Add(row);
                dgvTotales.DataSource = tbl;
            }
        }

        private void dgvTotales_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[1].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[2].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[3].CellAppearance.TextHAlign = HAlign.Right;

            e.Layout.Bands[0].Columns[0].Format = "N0";
            e.Layout.Bands[0].Columns[1].Format = "C2";
            e.Layout.Bands[0].Columns[2].Format = "N2";
            e.Layout.Bands[0].Columns[3].Format = "N2";

            e.Layout.Bands[0].Columns[0].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[1].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[2].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[3].CellActivation = Activation.NoEdit;
        }

        private void dgvDatos_AfterCellUpdate(object sender, CellEventArgs e)
        {
            try
            {
                DataTable tbl = new DataTable();
                tbl.Columns.Add("Total (PZ)", typeof(decimal));
                tbl.Columns.Add("Total ($)", typeof(decimal));
                tbl.Columns.Add("Peso (KG)", typeof(decimal));
                tbl.Columns.Add("Volumen (ft3)", typeof(decimal));

                DataRow row = tbl.NewRow();
                decimal pz = decimal.Zero; decimal mm = decimal.Zero; decimal peso = decimal.Zero; decimal vol = decimal.Zero;
                foreach (UltraGridRow item in dgvDatos.Rows)
                {
                    if (!item.IsFilteredOut)
                    {
                        pz += Convert.ToDecimal(item.Cells[(int)Columnas1.Transferir].Value);
                        mm += Convert.ToDecimal(item.Cells[(int)Columnas1.Monto].Value);
                        peso += Convert.ToDecimal(item.Cells["TP"].Value == DBNull.Value ? decimal.Zero
                            : item.Cells["TP"].Value);
                        vol += Convert.ToDecimal(item.Cells["Volumen"].Value);
                    }
                }
                row[0] = pz;
                row[1] = mm;
                row[2] = peso;
                row[3] = vol;

                tbl.Rows.Add(row);
                dgvTotales.DataSource = tbl;
            }
            catch (Exception)
            {
            }
        }

        private void dgvDatos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (Convert.ToDecimal(e.Row.Cells["I"].Value == DBNull.Value ? decimal.Zero : e.Row.Cells["I"].Value) <= 4)
            {
                e.Row.Cells[0].Appearance.BackColor = Color.Red;
                e.Row.Cells[0].Appearance.ForeColor = Color.White;
            }
        }
    }
}
