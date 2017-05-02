using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace H_Compras.Compras.Pedido
{
    public partial class frmCompras : Constantes.frmEmpty
    {
        public DataTable tbl_Articulos = new DataTable();
        public DataTable tbl_Almacenes = new DataTable();
        public string ALMACENES;
        public DataTable TBL_ALMACENES = new DataTable();
        DataTable TBL_ALMACENES_INDEPENDIENTES = new DataTable();
        private decimal MinCompra = decimal.Zero;
        private string UnidadMinimo = string.Empty;
        private bool _isDrag = false;
        private string QryGroup56 = string.Empty;
        private int TipoConsulta;
        string Proveedor;

        Dictionary<string, class_Reubicable> dicReubicables = new Dictionary<string,class_Reubicable>();

        string sp_Name = "sp_Compras";

        public enum ColumnasGrid
        {
            Linea,
            Articulo,
            Descripcion,
            Clasificacion,
            Ideal,
            IdealOK,
            Stock,
            Solicitado,
            Total,
            Faltante,
            sCEDIS,
            CEDIS,
            Autorizado, 
            NumInBuy,
            Multiplo,
            WhsCode,
            Almacen,
            Price,
            Entrada,
            HP,
            Currency,
            Rate,
            VoluU,
            PesoU,
            Reubicable,
            conPronostico,
            
            VI,
            TotalPrice,
            TransPrice,
            DiasStock,
            Peso,
            Volumen
        }

        public frmCompras()
        {
            InitializeComponent();
        }

        public void Formato()
        {
            UltraGridColumn column1 = dgvDatos.DisplayLayout.Bands[0].Columns[(int)ColumnasGrid.Linea];
            column1.Width = 70;


            UltraGridColumn column2 = dgvDatos.DisplayLayout.Bands[0].Columns[(int)ColumnasGrid.Articulo];
            column2.Width = 90;


            UltraGridColumn column3 = dgvDatos.DisplayLayout.Bands[0].Columns[(int)ColumnasGrid.Descripcion];
            column3.Width = 220;
        }

        public void Totales()
        {
            DataTable tblTotales = new DataTable();
            tblTotales.Columns.Add("Mínimo de compra", typeof(decimal));
            tblTotales.Columns.Add("Unidad", typeof(string));
            tblTotales.Columns.Add("Comprar (PZ)", typeof(decimal));
            tblTotales.Columns.Add("Comprar ($)", typeof(decimal));
            tblTotales.Columns.Add("Transferir (PZ)", typeof(decimal));
            tblTotales.Columns.Add("Transferir ($)", typeof(decimal));

            DataRow rowTotal = tblTotales.NewRow();
            rowTotal[0] = MinCompra;
            rowTotal[1] = UnidadMinimo;
            rowTotal[2] = Convert.ToDecimal((dgvDatos.DataSource as DataTable).Compute("SUM(Autorizado)", string.Empty));
            rowTotal[3] = Convert.ToDecimal((dgvDatos.DataSource as DataTable).Compute("SUM(TotalPrice)", string.Empty));

            rowTotal[4] = Convert.ToDecimal(
                (dgvDatos.DataSource as DataTable).Compute("SUM([Transferir del CEDIS])", string.Empty) == DBNull.Value ? decimal.Zero :
                 (dgvDatos.DataSource as DataTable).Compute("SUM([Transferir del CEDIS])", string.Empty));
            rowTotal[5] = Convert.ToDecimal(
                (dgvDatos.DataSource as DataTable).Compute("SUM(TransPrice)", string.Empty) == DBNull.Value ? decimal.Zero :
                (dgvDatos.DataSource as DataTable).Compute("SUM(TransPrice)", string.Empty)
                    );

            tblTotales.Rows.Add(rowTotal);

            dgvTotales.DataSource = tblTotales;
            dgvTotales.Columns[0].HeaderText = "Mínimo compra";

            if (UnidadMinimo.ToLower().Equals("pz"))
                dgvTotales.Columns[0].DefaultCellStyle.Format = "N0";
            else
                dgvTotales.Columns[0].DefaultCellStyle.Format = "C2";

            dgvTotales.Columns[2].DefaultCellStyle.Format = "N0";
            dgvTotales.Columns[3].DefaultCellStyle.Format = "C2";
            dgvTotales.Columns[4].DefaultCellStyle.Format = "N0";
            dgvTotales.Columns[5].DefaultCellStyle.Format = "C2";

            dgvTotales.Columns[0].Width = 80;
            dgvTotales.Columns[1].Width = 80;
            dgvTotales.Columns[2].Width = 80;
            dgvTotales.Columns[3].Width = 80;
            dgvTotales.Columns[4].Width = 80;
            dgvTotales.Columns[5].Width = 80;

            dgvTotales.Columns[0].DefaultCellStyle.Alignment =
            dgvTotales.Columns[1].DefaultCellStyle.Alignment =
            dgvTotales.Columns[2].DefaultCellStyle.Alignment =
            dgvTotales.Columns[3].DefaultCellStyle.Alignment =
            dgvTotales.Columns[4].DefaultCellStyle.Alignment =
            dgvTotales.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void Compras_Load(object sender, EventArgs e)
        {
            try
            {
                Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                #region Limpiar Controles
                txtArticulo.Clear();
                cbProveedor.DataSource = null;
                cbZona.DataSource = null;

                txtHorizonte.Clear();
                txtHorizonte.Enabled = false;
                cbHorizonte.Checked = false;

                dgvDatos.DataSource = null;
                dgvTotales.DataSource = null;
                dgvStocks.DataSource = null;
                dgvVentas.DataSource = null;
                #endregion

                ClasesSGUV.Form.ControlsForms.setDataSource(cbProveedor, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Proveedores, null, string.Empty), "CardName", "CardCode", "---Selecciona un proveedor---");
                tbl_Articulos = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetArticulos, string.Empty, string.Empty);
                ClasesSGUV.Form.ControlsForms.Autocomplete(txtArticulo, tbl_Articulos.Copy(), "ItemCode");
                this.dgvDatos.PerformAction(UltraGridAction.Copy, true, true);
                lbIndependientes.AllowDrop = true;

                dgvDatos.DisplayLayout.Override.RowSelectorHeaderStyle = RowSelectorHeaderStyle.ColumnChooserButton;
                dgvDatos.DisplayLayout.Override.RowSelectors = DefaultableBoolean.True;
                dgvDatos.DisplayLayout.Override.AllowRowFiltering = DefaultableBoolean.True;

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

                #region Eventos ToolStrip
                nuevoToolStripButton.Click -= new EventHandler(Compras_Load);
                nuevoToolStripButton.Click += new EventHandler(Compras_Load);
                actualizarToolStripButton.Click -= new EventHandler(Compras_Load);
                actualizarToolStripButton.Click += new EventHandler(Compras_Load);

                ayudaToolStripButton.Enabled = false;
                #endregion

                txtArticulo.Focus();
                PathHelp = "http://hntsolutions.net/manual/module_14_2_1.htm?ms=AAAAAAA%3D&st=MA%3D%3D&sct=NDQzMg%3D%3D&mw=MjU1";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                #region Eliminar Almacen de lbAlmacenes
                var qry = (from item in TBL_ALMACENES.AsEnumerable()
                           where item.Field<bool>("Obligatorio")
                           && item.Field<string>("Zona").Equals(cbZona.SelectedValue.ToString())
                           select item.Field<string>("WhsCode")).ToList();

                if (lbAlmacenes.SelectedIndex != -1)
                {
                    if (lbAlmacenes.Text.Length > 2)
                        if (!qry.Contains(lbAlmacenes.Text.Substring(0, 2)))
                        {
                            #region Regresar Almacen independiente a lbIndentendiente
                            foreach (DataRow item in TBL_ALMACENES_INDEPENDIENTES.Rows)
                            {
                                if (item.Field<bool>("Independiente"))
                                    if (item.Field<string>("WhsCode").Equals(lbAlmacenes.Text.Substring(0, 2)))
                                        lbIndependientes.Items.Add(item.Field<string>("WhsCode") + " - " + item.Field<string>("WhsName"));
                            }
                            #endregion

                            lbAlmacenes.Items.RemoveAt(lbAlmacenes.SelectedIndex);
                            lbAlmacenes.SelectedIndex = 0;
                        }
                        else
                        {
                            MessageBox.Show("El almacén [" + lbAlmacenes.Text + "]" + " no se puede eliminar de esta zona", "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                }
                #endregion
            }
            catch (Exception)
            {
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                dicReubicables.Clear();

                Proveedor = cbProveedor.SelectedValue.ToString();
                dgvDatos.DataSource = null;
                dgvStocks.DataSource = null;
                dgvVentas.DataSource = null;
                dgvTotales.DataSource = null;

                TipoConsulta = ((DataRowView)cbZona.SelectedItem).Row.Field<Int32>("TipoConsulta");
                if (string.IsNullOrEmpty(txtHorizonte.Text))
                {
                    this.SetMensaje("Ingrese un Horizonte de planeación", 5000, Color.Red, Color.White);
                    return;
                }
                ALMACENES = string.Empty;

                foreach (var listBoxItem in lbAlmacenes.Items)
                {
                    if (listBoxItem.ToString().Length >= 2)
                        ALMACENES += "'" + listBoxItem.ToString().Substring(0, 2) + "',";
                }

                #region Datos Reporte
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand(sp_Name, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        if (string.IsNullOrEmpty(txtArticulo.Text))
                            QryGroup56 = "N";

                        command.Parameters.AddWithValue("@TipoConsulta", TipoConsulta);
                        command.Parameters.AddWithValue("@Almacenes", ALMACENES.Trim(','));
                        command.Parameters.AddWithValue("@ItemCode", txtArticulo.Text);
                        command.Parameters.AddWithValue("@Zona_Comprar", cbZona.SelectedValue);
                        command.Parameters.AddWithValue("@CardCode", cbProveedor.SelectedValue);
                        command.Parameters.AddWithValue("@Propiedad", QryGroup56);

                        if (cbHorizonte.Checked)
                            command.Parameters.AddWithValue("@Horizonte", txtHorizonte.Text);

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;

                        DataTable table = new DataTable();
                        da.Fill(table);

                        if (table.Rows.Count <= 0)
                        {
                            this.SetMensaje("ERROR: EL PROVEEDOR NO TIENE ARTÍCULOS ASIGNADOS", 5000, Color.Red, Color.White);
                            return;
                        }
                        table.Columns.Add("Veces Ideal", typeof(decimal), "([Stock + Solicitado]+(Autorizado*NumInBuy))/IIF(Ideal=0,1,Ideal)");
                        table.Columns.Add("TotalPrice", typeof(decimal), "((Autorizado*NumInBuy)*Price)");
                        table.Columns.Add("TransPrice", typeof(decimal), "[Transferir del CEDIS] * [Price]");
                        table.Columns.Add("DiasStock", typeof(decimal), "[Veces Ideal]*Horizonte");
                        table.Columns.Add("Peso", typeof(decimal), "Autorizado*NumInBuy*BWeight1");
                        table.Columns.Add("Volumen", typeof(decimal), "Autorizado*NumInBuy*VolumenU");

                        dgvDatos.DataSource = table;

                        this.Formato();
                    }
                }
                #endregion

                #region Minimos de compra
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand(sp_Name, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 2);
                        command.Parameters.AddWithValue("@CardCode", cbProveedor.SelectedValue);

                        DataTable tbl = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(tbl);

                        MinCompra = tbl.Rows[0].Field<decimal>(0);
                        UnidadMinimo = tbl.Rows[0].Field<string>(1);
                    }
                }
                #endregion

                this.Totales();

                #region cantidades reublicables

                //foreach (UltraGridRow item in dgvDatos.Rows)
                //{
                    this.getReubicables("");

                //    item.Cells[(int)ColumnasGrid.Reubicable].Value 
                //        = this.sumReubicables(dicReubicables[item.Cells[(int)ColumnasGrid.Articulo].Value.ToString()].sourdeReubicale);
                //}
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbLinea_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtArticulo.Text))
                    QryGroup56 = "N";

                ClasesSGUV.Form.ControlsForms.setDataSource(cbZona, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetZonaProveedor, cbProveedor.SelectedValue, QryGroup56), "Zona", "Zona", string.Empty);

                cbZona_SelectionChangeCommitted(sender, e);


                #region Todos los Almacenes
                TBL_ALMACENES.Clear();
                TBL_ALMACENES = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesProveedor, cbProveedor.SelectedValue, string.Empty);
                #endregion

                #region Horizonte
                DataRowView row = cbProveedor.SelectedItem as DataRowView;
                txtHorizonte.Text = row["HP"].ToString();
                #endregion

                #region Editar Historial de ventas
                DataRowView row1 = cbProveedor.SelectedItem as DataRowView;
                int grupo = Convert.ToInt32(row1["GroupCode"] != DBNull.Value ? row1["GroupCode"] : 0 );
                if (grupo == 113 | grupo == 112)
                    contextMenu.Enabled = true;
                else
                    contextMenu.Enabled = false;
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                                     select item.Field<string>("CardCode")).FirstOrDefault();
                    QryGroup56 = "N";
                    //QryGroup56 = (from item in tbl_Articulos.AsEnumerable()
                    //              where item.Field<string>("ItemCode").ToLower().Trim().Equals(txtArticulo.Text.ToLower().Trim())
                    //              select item.Field<string>("QryGroup56")).FirstOrDefault();

                    cbProveedor.SelectedValue = proveedor == null ? string.Empty : proveedor;


                    cbLinea_SelectionChangeCommitted(sender, e);
                }

                button4_Click(sender, e);
            }
        }

        private void cbZona_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                lbAlmacenes.Items.Clear();

                string _zona = cbZona.SelectedValue.ToString();

                DataTable tbl_almacenes_zonas = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesZonaProveedor, cbProveedor.SelectedValue, _zona);

                foreach (DataRow item in tbl_almacenes_zonas.Rows)
                {
                    lbAlmacenes.Items.Add(item.Field<string>("WhsCode") + " - " + item.Field<string>("WhsName"));
                }

                tbl_Almacenes = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesZona, _zona, string.Empty);

                #region Almacenes independientes
                TBL_ALMACENES_INDEPENDIENTES.Clear();
                TBL_ALMACENES_INDEPENDIENTES = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesIndependientesProveedor, cbProveedor.SelectedValue, string.Empty);
                lbIndependientes.Items.Clear();
                foreach (DataRow item in TBL_ALMACENES_INDEPENDIENTES.Rows)
                {
                    lbIndependientes.Items.Add(item.Field<string>("WhsCode") + " - " + item.Field<string>("WhsName"));
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            #region Ancho
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Linea].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Articulo].Width = 90;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Descripcion].Width = 180;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Clasificacion].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Ideal].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.IdealOK].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Stock].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Solicitado].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Faltante].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Autorizado].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Multiplo].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Almacen].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.VI].Width = 70;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TotalPrice].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Total].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.CEDIS].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.sCEDIS].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TransPrice].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.DiasStock].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.HP].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Currency].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Peso].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Volumen].Width = 80;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Reubicable].Width = 80;
            //e.Layout.Bands[0].Columns[(int)ColumnasGrid.].Width = 80;
            #endregion

            #region Formulas
            //Infragistics.Win.UltraWinCalcManager.UltraCalcManager calcManager;
            //calcManager = new Infragistics.Win.UltraWinCalcManager.UltraCalcManager(this.Container);
            //e.Layout.Grid.CalcManager = calcManager;

            //e.Layout.Bands[0].Columns[(int)ColumnasGrid.TransPrice].Formula = "[Transferir del CEDIS] * [Price]";
            //e.Layout.Override.FormulaErrorAppearance.BackColor = Color.Red;
            #endregion

            #region Hidden
            //e.Layout.Bands[0].Columns[(int)ColumnasGrid.Price].Hidden = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.WhsCode].Hidden = true;
            //e.Layout.Bands[0].Columns[(int)ColumnasGrid.Total].Hidden = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Almacen].Hidden = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Ideal].Hidden = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Entrada].Hidden = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TotalPrice].Hidden = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TransPrice].Hidden = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.HP].Hidden = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Currency].Hidden = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Rate].Hidden = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.PesoU].Hidden = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.conPronostico].Hidden = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.VoluU].Hidden = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.CEDIS].Hidden = true;//TipoConsulta == 5;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.sCEDIS].Hidden = true;//TipoConsulta == 5;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Reubicable].Hidden = TipoConsulta == 5;
            #endregion

            #region CellActivation
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Linea].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.CEDIS].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Articulo].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Descripcion].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Clasificacion].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Ideal].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.IdealOK].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Stock].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Faltante].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Multiplo].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TotalPrice].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.VI].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Almacen].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Total].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Solicitado].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.sCEDIS].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TransPrice].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.DiasStock].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.HP].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.NumInBuy].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Volumen].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Peso].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Reubicable].CellActivation = Activation.NoEdit;
            #endregion

            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Autorizado].CellAppearance.BackColor = Color.FromName("Info");

            #region Format
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Multiplo].Format = "N0";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.CEDIS].Format = "N0";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.sCEDIS].Format = "N0";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Stock].Format = "N0";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Faltante].Format = "N0";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Autorizado].Format = "N0";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Multiplo].Format = "N0";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Solicitado].Format = "N0";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Total].Format = "N0";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.VI].Format = "N2";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Price].Format = "C2";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TotalPrice].Format = "C2";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TransPrice].Format = "C2";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.HP].Format = "N0";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.DiasStock].Format = "N0";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.NumInBuy].Format = "N0";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Peso].Format = "N2";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Volumen].Format = "N2";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Reubicable].Format = "N0";
            #endregion

            #region CellApparence
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Ideal].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.IdealOK].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.CEDIS].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.sCEDIS].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Stock].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Faltante].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Autorizado].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Multiplo].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Total].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Solicitado].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Price].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.VI].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TotalPrice].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TransPrice].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.DiasStock].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.HP].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.NumInBuy].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Volumen].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Peso].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Reubicable].CellAppearance.TextHAlign = HAlign.Right;
            #endregion

            #region Fixed Header
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Articulo].Header.Fixed = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Linea].Header.Fixed = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Descripcion].Header.Fixed = true;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Clasificacion].Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Ideal].Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.IdealOK].Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Stock].Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Faltante].Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Autorizado].Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Multiplo].Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.VI].Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TotalPrice].Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TransPrice].Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.NumInBuy].Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
            #endregion

            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Autorizado].Header.Caption = "Autorizado a comprar";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.VI].Header.Caption = "Veces Ideal";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.IdealOK].Header.Caption = "Ideal";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TotalPrice].Header.Caption = "Monto Total";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TransPrice].Header.Caption = "Transferir Total";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.DiasStock].Header.Caption = "Días de stock";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.NumInBuy].Header.Caption = "Multiplo";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.Price].Header.Caption = "Precio unitario";

            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TotalPrice].Header.ToolTipText = "Autorizado * Precio de compra";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.TransPrice].Header.ToolTipText = "Transferir del CEDIS * Precio de compra";
            e.Layout.Bands[0].Columns[(int)ColumnasGrid.DiasStock].Header.ToolTipText = "Veces ideal * Horizonte de planeación";

            e.Layout.Override.AllowMultiCellOperations = AllowMultiCellOperation.Copy;
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

        private void dgvDatos_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        {
            try
            {
                this.Totales();
            }
            catch (Exception)
            {

            }
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

        private void ultraGrid_Click(object sender, EventArgs e)
        {
            Ug = (sender as UltraGrid);

            Dg = null;
        }

        private void lbAlmacenes_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string str = (string)e.Data.GetData(DataFormats.StringFormat);

                _isDrag = false;

                foreach (DataRow item in TBL_ALMACENES.Rows)
                {
                    if (item.Field<string>("Zona").Equals(cbZona.SelectedValue.ToString()))
                        if (item.Field<string>("WhsCode").Equals(str.Substring(0, 2)))
                        {
                            _isDrag = true;
                            break;
                        }
                }

                if(!_isDrag)
                    MessageBox.Show("Este almacén no se puede agregar a la zona [" + cbZona.SelectedValue + "]", "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                foreach (string item in lbAlmacenes.Items)
                {
                    if (item.Equals(str))
                    {
                        MessageBox.Show("El almacén [" + item + "] ya existe en la zona [" + cbZona.SelectedValue + "]", "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        _isDrag = false;
                        break;
                    }
                }

                if (_isDrag)
                    (sender as ListBox).Items.Add(str);
            }
        }

        private void lbAlmacenes_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void lbIndependientes_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void lbIndependientes_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (lbIndependientes.Items.Count == 0)
                    return;

                int index = lbIndependientes.IndexFromPoint(e.X, e.Y);
                string s = lbIndependientes.Items[index].ToString();
                DragDropEffects dde1 = DoDragDrop(s, DragDropEffects.All);

                if (_isDrag)
                    if (dde1 == DragDropEffects.All)
                    {
                        lbIndependientes.Items.RemoveAt(lbIndependientes.IndexFromPoint(e.X, e.Y));
                    }
            }
            catch (Exception) { }
        }

        string res;
        private void dgvDatos_BeforeRowActivate(object sender, RowEventArgs e)
        {
            res = string.Empty;
            try
            {
                string _item = e.Row.Cells[(int)ColumnasGrid.Articulo].Value.ToString();

                #region Stock - Ideal
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand(sp_Name, connection))
                    {
                       command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 3);
                        command.Parameters.AddWithValue("@ItemCode", _item);
                        command.Parameters.AddWithValue("@Zona_Comprar", cbZona.SelectedValue);
                        command.Parameters.AddWithValue("@Almacenes", ALMACENES.Trim(','));


                        SqlDataAdapter da = new SqlDataAdapter();
                        DataTable table = new DataTable();
                        da.SelectCommand = command;
                        da.Fill(table);


                        table.TableName = "Detalle";

                        var qryHeader = (from item in table.AsEnumerable()
                                         where item.Field<string>("WhsCode") != "27"
                                        group item by new 
                                        {
                                            ItemCode = item.Field<string>("ItemCode"),
                                            Zona = item.Field<string>("Zona")
                                        }into grouped
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
                      

                        ds.Relations.Add("UNION", ds.Tables[0].Columns["Zona"],ds.Tables[1].Columns["Zona"]);

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
                        command.Parameters.AddWithValue("@TipoConsulta", 7);
                        command.Parameters.AddWithValue("@Articulo", _item);
                        command.Parameters.AddWithValue("@Zona", cbZona.SelectedValue);
                        command.Parameters.AddWithValue("@Almacen", ALMACENES.Trim(','));

                        SqlParameter _result = new SqlParameter("@Result", SqlDbType.Char, 1);
                        _result.Direction = ParameterDirection.Output;
                        command.Parameters.Add(_result);

                        SqlDataAdapter da = new SqlDataAdapter();
                        DataTable table = new DataTable();
                        da.SelectCommand = command;
                        da.Fill(table);

                        res = command.Parameters["@Result"].Value.ToString();

                        #region ventas original
                        if (res.Equals("N"))
                        {
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
                        #endregion
                        #region Ventas pronostico
                        else
                        {
                            dgvVentas.DataSource = table;
                            dgvVentas.Rows.ExpandAll(true);
                        }
                        #endregion
                    }
                }
                #endregion

                editarPronosticoToolStripMenuItem_Click(sender, e);
            }
            catch (Exception)
            {
                
            }
        }

        public string MonthName(int month)
        {
            System.Globalization.DateTimeFormatInfo dtinfo = new System.Globalization.CultureInfo("es-ES", false).DateTimeFormat;
            return dtinfo.GetMonthName(month);
        }

        private void dgvStocks_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
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

                e.Layout.Bands[1].ColHeadersVisible = false;

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
            try
            {
                #region Ventas original
                if (res.Equals("N"))
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

                    e.Layout.Bands[0].Columns[0].Header.Fixed = true;
                    e.Layout.Bands[0].Columns[1].Header.Fixed = true;
                    e.Layout.Bands[1].Columns[1].Header.Fixed = true;

                    e.Layout.Bands[1].Columns[0].Hidden = true;
                    e.Layout.Bands[1].Columns[2].Hidden = true;
                    e.Layout.Bands[0].Columns[1].Hidden = true;

                    e.Layout.Bands[0].Columns[0].CellAppearance.TextHAlign = HAlign.Left;
                    e.Layout.Bands[0].Columns[1].CellAppearance.TextHAlign = HAlign.Left;
                    e.Layout.Bands[1].Columns[1].CellAppearance.TextHAlign = HAlign.Left;

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
                #endregion
                #region Pronosticos
                else
                {
                    e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Artículo";

                    e.Layout.Bands[0].Columns["Mes"].Hidden = true;
                    e.Layout.Bands[0].Columns["WhsCode"].Hidden = true;

                    DateTime Fecha = Convert.ToDateTime(Convert.ToDateTime(dgvVentas.Rows[0].Cells["Mes"].Value).Year + "-" + Convert.ToDateTime(dgvVentas.Rows[0].Cells["Mes"].Value).Month + "-01");

                    foreach (var item in e.Layout.Bands[0].Columns)
                    {
                        item.Header.FixedHeaderIndicator = FixedHeaderIndicator.None;

                        if (item.Index > 2)
                        {
                            item.Header.Caption = MonthName(Fecha.Month).Substring(0, 3).ToLowerInvariant() + " " + Fecha.Year;
                            item.Width = 80;
                            item.Format = "N0";
                            item.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                            Fecha = Fecha.AddMonths(1);

                            foreach (var original in ugPronostico.DisplayLayout.Bands[0].Columns)
                            {
                                if (item.Header.Caption.ToLower().Equals(original.Header.Caption.ToLower()))
                                {
                                    if (Convert.ToDecimal(dgvVentas.Rows[0].Cells[item.Index].Value == DBNull.Value ? 0 : dgvVentas.Rows[0].Cells[item.Index].Value)
                                        != Convert.ToDecimal(ugPronostico.Rows[0].Cells[original.Index].Value == DBNull.Value ? 0 : ugPronostico.Rows[0].Cells[original.Index].Value))
                                    {
                                        item.Header.Appearance.BackColor = Color.Yellow;
                                        item.Header.Appearance.BackColor2 = Color.Yellow;
                                    }
                                }
                            }

                        }

                    }
                }
                #endregion
            }
            catch (Exception)
            {
            }
        }

        private void cbHorizonte_Click(object sender, EventArgs e)
        {
            txtHorizonte.Enabled = cbHorizonte.Checked;
        }

        private void dgvDatos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                if (Convert.ToDecimal(e.Row.Cells[(int)ColumnasGrid.Entrada].Value) <= 4)
                {
                    e.Row.Cells[(int)ColumnasGrid.Articulo].Appearance.BackColor = Color.Red;
                    e.Row.Cells[(int)ColumnasGrid.Articulo].Appearance.ForeColor = Color.White;

                    e.Row.Cells[(int)ColumnasGrid.Articulo].SelectedAppearance.BackColor = Color.Red;
                    e.Row.Cells[(int)ColumnasGrid.Articulo].SelectedAppearance.ForeColor = Color.White;
                }
              
                if (Convert.ToString(e.Row.Cells["conPronostico"].Value).Equals("Y"))
                {
                    e.Row.Cells[(int)ColumnasGrid.IdealOK].Appearance.BackColor = Color.Yellow;
                    e.Row.Cells[(int)ColumnasGrid.IdealOK].Appearance.ForeColor = Color.Black;

                    e.Row.Cells[(int)ColumnasGrid.IdealOK].SelectedAppearance.BackColor = Color.Yellow;
                    e.Row.Cells[(int)ColumnasGrid.IdealOK].SelectedAppearance.ForeColor = Color.Black;
                }
            }
            catch (Exception)
            {
                
            }
        }

        private void btnSDK_Click(object sender, EventArgs e)
        {
            try
            {
                SDK.Documentos.frmDocumentos formulario = new SDK.Documentos.frmDocumentos(dgvDatos.DataSource as DataTable, 1, Proveedor);
                formulario.MdiParent = this.MdiParent;
                formulario.Show();
            }
            catch (Exception ex)
            {
                SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void dgvDatos_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            try
            {
                if ((int)ColumnasGrid.Reubicable== e.Cell.Column.Index)
                {
                    frmPedidoReubicaciones form = 
                        new frmPedidoReubicaciones(
                            dicReubicables[dgvDatos.Rows[e.Cell.Row.Index].Cells[(int)ColumnasGrid.Articulo].Value.ToString()].sourdeReubicale
                        );
                    if (form.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                    {
                        dicReubicables[dgvDatos.Rows[e.Cell.Row.Index].Cells[(int)ColumnasGrid.Articulo].Value.ToString()].sourdeReubicale = form.Tbl_Reubicaicones;

                        dgvDatos.Rows[e.Cell.Row.Index].Cells[(int)ColumnasGrid.Reubicable].Value = this.sumReubicables(form.Tbl_Reubicaicones);

                        if (true)
                        {
                            e.Cell.Appearance.BackColor = Color.Green;
                            e.Cell.Row.RowSelectorAppearance.BackColor = Color.Green;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void getReubicables(string _ToWhsCode, string _ItemCode)
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand(sp_Name, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 7);
                    command.Parameters.AddWithValue("@Almacen_Comprador", _ToWhsCode);
                    command.Parameters.AddWithValue("@ItemCode", _ItemCode);
                    command.Parameters.AddWithValue("@CardCode", cbProveedor.SelectedValue);
                    command.Parameters.AddWithValue("@Zona_Comprar", cbZona.Text);

                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;

                    DataTable tbl = new DataTable();
                    da.Fill(tbl);

                    class_Reubicable fila = new class_Reubicable();
                    fila.ItemCode = _ItemCode;
                    fila.sourdeReubicale = tbl;

                    dicReubicables.Add(_ItemCode, fila);
                }
            }
        }

        private void getReubicables(string _ToWhsCode)
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand(sp_Name, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 7);
                    command.Parameters.AddWithValue("@Almacen_Comprador", _ToWhsCode);
                    command.Parameters.AddWithValue("@CardCode", cbProveedor.SelectedValue);
                    command.Parameters.AddWithValue("@Zona_Comprar", cbZona.Text);

                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;

                    DataTable tbl = new DataTable();
                    da.Fill(tbl);

                    foreach (DataRow item in (dgvDatos.DataSource as DataTable).Rows)
                    {
                        var qry = from reu in tbl.AsEnumerable()
                                  where reu.Field<string>("ItemCode") == item.Field<string>("Artículo")
                                  select reu;

                        class_Reubicable fila = new class_Reubicable();
                        fila.ItemCode = item.Field<string>("Artículo");
                        if (qry.Count() > 0)
                        {
                            DataTable itemsReublicables = qry.CopyToDataTable();
                            fila.sourdeReubicale = itemsReublicables;
                            item["Reubicar"] = this.sumReubicables(itemsReublicables);
                        }

                        else
                        {
                            DataTable itemsReublicables = tbl.Copy();
                            itemsReublicables.Clear();
                            fila.sourdeReubicale = itemsReublicables;
                        }
                        dicReubicables.Add(item.Field<string>("Artículo"), fila);

                        
                    }
                }
            }
        }

        private decimal sumReubicables(DataTable tbl)
        {
            return Convert.ToDecimal(tbl.Compute("SUM(QtyOK)", string.Empty) == DBNull.Value ?
                decimal.Zero : tbl.Compute("SUM(QtyOK)", string.Empty));
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                if ((from item in (dgvDatos.DataSource as DataTable).AsEnumerable()
                     where item.Field<decimal>("Autorizado") > decimal.Zero
                     select item).Count() <= 0)
                    return;

                DataTable tbl_compras = (from item in (dgvDatos.DataSource as DataTable).AsEnumerable()
                                         where item.Field<decimal>("Autorizado") > decimal.Zero
                                         select item).CopyToDataTable();

                DataTable tbl_Transferencia = new DataTable();
                tbl_Transferencia.Columns.Add("ItemCode", typeof(string));
                tbl_Transferencia.Columns.Add("ItemName", typeof(string));
                tbl_Transferencia.Columns.Add("WhsCode", typeof(string));
                tbl_Transferencia.Columns.Add("WhsName", typeof(string));
                tbl_Transferencia.Columns.Add("FromWhsCode", typeof(string));
                tbl_Transferencia.Columns.Add("FromWhsName", typeof(string));
                tbl_Transferencia.Columns.Add("Cantidad", typeof(decimal));
                tbl_Transferencia.Columns.Add("Peso", typeof(decimal));
                tbl_Transferencia.Columns.Add("Volumen", typeof(decimal));
                tbl_Transferencia.Columns.Add("U_Tarima", typeof(string));
                tbl_Transferencia.Columns.Add("U_TipoAlm", typeof(string));
                tbl_Transferencia.Columns.Add("U_TipoAlmName", typeof(string));


                foreach (DataRow item in tbl_compras.AsEnumerable())
                {
                    foreach (DataRow reu in dicReubicables[item.Field<string>("Artículo")].sourdeReubicale.Rows)
                    {
                        if (reu.Field<decimal>("QtyOK") > 0)
                        {
                            DataRow row = tbl_Transferencia.NewRow();
                            row["ItemCode"] = reu["ItemCode"];
                            row["ItemName"] = reu["ItemName"];

                            row["WhsCode"] = reu["ToWhsCode"]; //destino
                            row["WhsName"] = reu["DestinoNombre"];

                            row["FromWhsCode"] = reu["WhsCode"];
                            row["FromWhsName"] = reu["WhsName"];

                            row["Cantidad"] = reu["QtyOK"];

                            row["Peso"] = reu["BWeight1"];
                            row["Volumen"] = reu["Volumen"];


                            tbl_Transferencia.Rows.Add(row);
                        }
                    }
                }
                // SDK.Documentos.frmDocumentos formulario = new SDK.Documentos.frmDocumentos(dgvDatos.DataSource as DataTable, 1, Proveedor);
                string ToWhsCode = (from item in tbl_Transferencia.AsEnumerable()
                                    select item.Field<string>("WhsCode")).FirstOrDefault();
                string Filler = (from item in tbl_Transferencia.AsEnumerable()
                                 select item.Field<string>("FromWhsCode")).FirstOrDefault();

                SDK.Documentos.frmTransferencia formulario = new SDK.Documentos.frmTransferencia(tbl_Transferencia, 4, Filler, ToWhsCode); //new SDK.Documentos.frmTransferencia(dgvDatos.DataSource as DataTable, 1, Proveedor);
                formulario.MdiParent = this.MdiParent;
                formulario.Show();
            }
            catch (Exception ex)
            {
                SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void editarPronosticoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Ideal", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 4);
                        command.Parameters.AddWithValue("@ItemCode", dgvDatos.ActiveRow.Cells["Artículo"].Value.ToString());
                        command.Parameters.AddWithValue("@WhsCode", "27");

                        //SqlParameter _result = new SqlParameter("@Result", SqlDbType.Char, 1);
                        //_result.Direction = ParameterDirection.Output;
                        //command.Parameters.Add(_result);

                        SqlDataAdapter da = new SqlDataAdapter();
                        DataTable table = new DataTable();
                        da.SelectCommand = command;
                        da.Fill(table);

                        if(table.Rows.Count == 0)
                        {
                            DataRow row = table.NewRow();
                            row["Mes"] = DateTime.Now;

                            table.Rows.Add(row);
                        }

                        //var qryHeader = (from item in table.AsEnumerable()
                        //                     group item by new
                        //                     {
                        //                         ItemCode = item.Field<string>("Artículo"),
                        //                         Zona = item.Field<string>("Zona")
                        //                     } into grouped
                        //                     select new
                        //                     {
                        //                         ItemCode = grouped.Key.ItemCode,
                        //                         Zona = grouped.Key.Zona,
                        //                         M1 = grouped.Sum(ix => ix.Field<decimal>(3)),
                        //                         M2 = grouped.Sum(ix => ix.Field<decimal>(4)),
                        //                         M3 = grouped.Sum(ix => ix.Field<decimal>(5)),
                        //                         M4 = grouped.Sum(ix => ix.Field<decimal>(6)),
                        //                         M5 = grouped.Sum(ix => ix.Field<decimal>(7)),
                        //                         M6 = grouped.Sum(ix => ix.Field<decimal>(8)),
                        //                         M7 = grouped.Sum(ix => ix.Field<decimal>(9)),
                        //                         M8 = grouped.Sum(ix => ix.Field<decimal>(10)),
                        //                         M9 = grouped.Sum(ix => ix.Field<decimal>(11)),
                        //                         M10 = grouped.Sum(ix => ix.Field<decimal>(12)),
                        //                         M11 = grouped.Sum(ix => ix.Field<decimal>(13)),
                        //                         M12 = grouped.Sum(ix => ix.Field<decimal>(14)),
                        //                         MT = grouped.Sum(ix => ix.Field<decimal>(15))
                        //                     }).ToList();

                        //DataTable tbl_header = new DataTable();
                        //tbl_header = Datos.Clases.ListConverter.ToDataTable(qryHeader);
                        //tbl_header.TableName = "Header";

                        //DataSet ds = new DataSet();
                        //ds.Tables.Add(tbl_header);
                        //ds.Tables.Add(table);


                        //ds.Relations.Add("UNION", ds.Tables[0].Columns["Zona"], ds.Tables[1].Columns["Zona"]);

                        ugPronostico.DataSource = null;
                        ugPronostico.DataSource = table;

                        //ugPronostico.DataSource = tbl_header;

                        frmPronosticoVentas formulario = new frmPronosticoVentas(dgvDatos.ActiveRow.Cells["Artículo"].Value.ToString(), dgvDatos.ActiveRow.Cells["WhsCode"].Value.ToString(), ugPronostico);
                        formulario.ShowDialog();
                    }
                }
            }
            catch (Exception)
            {

            }

        }

        private void ugPronostico_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            //foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
            //{
            //    item.Width = 70;
            //    item.Format = "N0";
            //    item.CellAppearance.TextHAlign = HAlign.Right;
            //    item.CellActivation = Activation.NoEdit;
            //    item.Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
            //}

            //e.Layout.Bands[0].Columns[0].Header.Fixed = true;
            //e.Layout.Bands[0].Columns[1].Header.Fixed = true;

            //e.Layout.Bands[0].Columns[1].Hidden = true;

            //e.Layout.Bands[0].Columns[0].CellAppearance.TextHAlign = HAlign.Left;
            //e.Layout.Bands[0].Columns[1].CellAppearance.TextHAlign = HAlign.Left;

            //e.Layout.Bands[0].Columns[0].Header.Caption = "Artículo";

            //e.Layout.Bands[0].Columns[2].Header.Caption = e.Layout.Bands[1].Columns[3].Header.Caption;
            //e.Layout.Bands[0].Columns[3].Header.Caption = e.Layout.Bands[1].Columns[4].Header.Caption;
            //e.Layout.Bands[0].Columns[4].Header.Caption = e.Layout.Bands[1].Columns[5].Header.Caption;
            //e.Layout.Bands[0].Columns[5].Header.Caption = e.Layout.Bands[1].Columns[6].Header.Caption;
            //e.Layout.Bands[0].Columns[6].Header.Caption = e.Layout.Bands[1].Columns[7].Header.Caption;
            //e.Layout.Bands[0].Columns[7].Header.Caption = e.Layout.Bands[1].Columns[8].Header.Caption;
            //e.Layout.Bands[0].Columns[8].Header.Caption = e.Layout.Bands[1].Columns[9].Header.Caption;
            //e.Layout.Bands[0].Columns[9].Header.Caption = e.Layout.Bands[1].Columns[10].Header.Caption;
            //e.Layout.Bands[0].Columns[10].Header.Caption = e.Layout.Bands[1].Columns[11].Header.Caption;
            //e.Layout.Bands[0].Columns[11].Header.Caption = e.Layout.Bands[1].Columns[12].Header.Caption;
            //e.Layout.Bands[0].Columns[12].Header.Caption = e.Layout.Bands[1].Columns[13].Header.Caption;
            //e.Layout.Bands[0].Columns[13].Header.Caption = e.Layout.Bands[1].Columns[14].Header.Caption;
            //e.Layout.Bands[0].Columns[14].Header.Caption = e.Layout.Bands[1].Columns[15].Header.Caption;

            DateTime Fecha = Convert.ToDateTime(Convert.ToDateTime(ugPronostico.Rows[0].Cells["Mes"].Value).Year + "-" + Convert.ToDateTime(ugPronostico.Rows[0].Cells["Mes"].Value).Month + "-01");

            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Activation.NoEdit;

                if (item.Index > 2)
                {
                    item.Header.Caption = MonthName(Fecha.Month).Substring(0, 3) + " " + Fecha.Year;
                    item.Width = 80;
                    item.Format = "N0";
                    item.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                    Fecha = Fecha.AddMonths(1);
                }

            }
        }
    }
}