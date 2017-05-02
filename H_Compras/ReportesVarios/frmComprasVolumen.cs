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

namespace H_Compras.ReportesVarios
{
    public partial class frmComprasVolumen : Constantes.frmEmpty
    {
        DataTable Tbl_Volumen = new DataTable();
        public frmComprasVolumen()
        {
            InitializeComponent();
        }

        public void Totales(DataTable tbl)
        {
            #region Totales
            var qry1 = from item in tbl.AsEnumerable()
                       group item by new
                       {
                           Zona = item.Field<string>("Zona"),
                       } into grouped
                       select new
                       {
                           Zona = grouped.Key.Zona,
                           Piezas = grouped.Sum(ix => ix.Field<Int32>("Comprar")),
                           PiezasM = grouped.Sum(ix => ix.Field<decimal>("ComprarM")),
                           Dinero = grouped.Sum(ix => ix.Field<decimal>("Dinero")),
                           OC = string.Empty
                       };

            dgvTotales.DataSource = Constantes.Clases.ListConverter.ToDataTable(qry1);

            DataRow rowTotal = (dgvTotales.DataSource as DataTable).NewRow();
            rowTotal[0] = "TOTAL";
            rowTotal[1] = (dgvTotales.DataSource as DataTable).Compute("SUM(Piezas)", string.Empty);
            rowTotal[2] = (dgvTotales.DataSource as DataTable).Compute("SUM(PiezasM)", string.Empty);
            rowTotal[3] = (dgvTotales.DataSource as DataTable).Compute("SUM(Dinero)", string.Empty);

            (dgvTotales.DataSource as DataTable).Rows.Add(rowTotal);
            #endregion
        }

        private void CrearOC(string Zona)
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("CardCode", typeof(string));
            tbl.Columns.Add("Autorizado", typeof(decimal));
            tbl.Columns.Add("Artículo", typeof(string));
            tbl.Columns.Add("Descripción", typeof(string));
            tbl.Columns.Add("WhsCode", typeof(string));
            tbl.Columns.Add("Almacén", typeof(string));
            tbl.Columns.Add("Price", typeof(decimal));
            tbl.Columns.Add("Currency", typeof(string));
            tbl.Columns.Add("Rate", typeof(decimal));
            tbl.Columns.Add("Volumen", typeof(decimal));
            tbl.Columns.Add("BWeight1", typeof(decimal));

            foreach (DataRow item in Tbl_Volumen.Rows)
            {
                if (item.Field<string>("Zona").Equals(Zona))
                {
                    DataRow r = tbl.NewRow();
                    r["CardCode"] = item["CardCode"];
                    r["Autorizado"] = item["ComprarM"];
                    r["Artículo"] = item["ItemCode"];
                    r["Descripción"] = item["ItemName"];
                    r["WhsCode"] = item["WhsCode"];
                    r["Almacén"] = item["WhsName"];
                    r["Price"] = item["Price"];
                    r["Currency"] = item["Currency"];
                    r["Rate"] = item["Rate"];
                    r["Volumen"] = item["Volumen"];
                    r["BWeight1"] = item["BWeight1"];
                    tbl.Rows.Add(r);
                }
            }

            SDK.Documentos.frmDocumentos formulario = new SDK.Documentos.frmDocumentos(tbl, 1, cbProveedor.SelectedValue.ToString());
            formulario.MdiParent = this.MdiParent;
            formulario.Show();
        }

        private void LineasCompromiso_Load(object sender, EventArgs e)
        {
            try
            {
                this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                ClasesSGUV.Form.ControlsForms.setDataSource(cbProveedor, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Proveedores, null, string.Empty), "CardName", "CardCode", "---Selecciona un proveedor---");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbProveedor1, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Proveedores, null, string.Empty), "CardName", "CardCode", "---Selecciona un proveedor---");

                dgvVolumen.KeyPress -= new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);
                dgvVolumen.KeyPress += new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);

                dgvVolumen.KeyDown -= new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);
                dgvVolumen.KeyDown += new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);

                #region Evento Grid

                foreach (GridKeyActionMapping ugKey in dgvVolumen.KeyActionMappings)
                {
                    if (ugKey.KeyCode == Keys.Enter)
                    {
                        dgvVolumen.KeyActionMappings.Remove(ugKey);
                    }
                }


                this.dgvVolumen.KeyActionMappings.Add(
                   new GridKeyActionMapping(
                   Keys.Enter,
                   UltraGridAction.BelowCell,
                   0,
                   0,
                   SpecialKeys.All,
                   0));
                #endregion

                this.PathHelp = "http://hntsolutions.net/manual/module_14_2_3.htm?ms=AAAAAAA%3D&st=MA%3D%3D&sct=NDY1MQ%3D%3D&mw=MjU1";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message, "Halconet", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnConsult_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbProveedor.SelectedIndex == 0)
                {
                    this.SetMensaje("Selecciona un proveedor", 5000, Color.Red, Color.White);
                    return;
                }
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Volumen", connection))
                    {
                        command.CommandTimeout = 0;
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 2);
                        command.Parameters.AddWithValue("@CardCode", cbProveedor.SelectedValue);
                        command.Parameters.AddWithValue("@Monto", textBox1.Text == string.Empty ? "1" : textBox1.Text);
                        command.Parameters.AddWithValue("@Moneda", rbMXP.Checked == true ? "MXP" : "USD");
                        command.Parameters.AddWithValue("@TC", textBox2.Text);
                        command.Parameters.AddWithValue("@Tipo", rbDinero.Checked ? "DIN" : "PZ");
                        command.Parameters.AddWithValue("@Zonas", ClasesSGUV.Form.ControlsForms.getCadena(clbZonas, string.Empty, false, "Zona"));

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        Tbl_Volumen.Clear();
                        da.Fill(Tbl_Volumen);

                        dgvDatos.DataSource = Tbl_Volumen;
                        dgvVolumen.DataSource = Tbl_Volumen;

                        this.Totales(Tbl_Volumen);
                    }
                }
            }
            catch (Exception ex)
            {


                MessageBox.Show("Error inesperado: " + ex.Message, "Halconet", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                lbl.Text = Convert.ToDecimal(textBox1.Text).ToString("N2");
            }
            catch
                (Exception)
            {
                lbl.Text = Convert.ToDecimal("0").ToString("N0");
            }
        }

        private void dgvTotales_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[2].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            e.Layout.Bands[0].Columns[0].Width = 120;
            e.Layout.Bands[0].Columns[1].Width = 80;
            e.Layout.Bands[0].Columns[2].Width = 80;
            e.Layout.Bands[0].Columns[3].Width = 80;
            e.Layout.Bands[0].Columns[4].Width = 50;

            e.Layout.Bands[0].Columns[1].Format = "N0";
            e.Layout.Bands[0].Columns[2].Format = "N0";
            e.Layout.Bands[0].Columns[3].Format = "C0";

            e.Layout.Bands[0].Columns[1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[2].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[3].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }

        private void cbProveedor_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ClasesSGUV.Form.ControlsForms.setDataSource(clbZonas, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetZonaProveedor, cbProveedor.SelectedValue, "N"), "Zona", "Zona", string.Empty);

            #region Minimos de compra
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_Compras", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 2);
                    command.Parameters.AddWithValue("@CardCode", cbProveedor.SelectedValue);

                    DataTable tbl = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl);

                    txtMinimo.Text = tbl.Rows[0].Field<decimal>(0).ToString("C2");
                    txtUnidad.Text = tbl.Rows[0].Field<string>(1);
                }
            }
            #endregion
        }

        private void dgvTotales_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString().Equals("TOTAL"))
            {
                e.Row.Appearance.BackColor = Color.FromArgb(68, 84, 106);
                e.Row.Appearance.ForeColor = Color.White;
            }

            try
            {
                int i = e.Row.Band.Index;
                if (i == 0)
                {
                    string Val = e.Row.Cells["OC"].Value == DBNull.Value ? string.Empty : e.Row.Cells["OC"].Value.ToString();
                    if (String.IsNullOrEmpty(Val))
                    {
                        if (e.Row.Cells[0].Value.ToString().Equals("TOTAL"))
                            return;

                        e.Row.Cells["OC"].ButtonAppearance.Image = Properties.Resources.sap_logo_btn;
                        e.Row.Cells["OC"].ButtonAppearance.ImageHAlign = HAlign.Center;
                        e.Row.Cells["OC"].ButtonAppearance.ImageVAlign = VAlign.Middle;

                        e.Row.Cells["OC"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;

                        //e.Row.Cells["OC"].ButtonAppearance.BorderColor = Color.CadetBlue;
                        e.Row.Cells["OC"].Appearance.ImageBackground = Properties.Resources.sap_logo_btn;
                        e.Row.Cells["OC"].Appearance.ImageBackgroundStyle = ImageBackgroundStyle.Stretched;
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void dgvVolumen_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            Infragistics.Win.UltraWinCalcManager.UltraCalcManager calcManager;
            calcManager = new Infragistics.Win.UltraWinCalcManager.UltraCalcManager(this.Container);
            e.Layout.Grid.CalcManager = calcManager;

            e.Layout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy;

            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }

            e.Layout.Bands[0].Columns["ItmsGrpNam"].Header.Caption = "Línea";
            e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Artículo";
            e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Descripción";
            e.Layout.Bands[0].Columns["Precio de compra"].Hidden = true;
            e.Layout.Bands[0].Columns["Comprar"].Header.Caption = "Comprar (PZ)";
            e.Layout.Bands[0].Columns["ComprarM"].Header.Caption = "Comprar (Multiplo)";
            e.Layout.Bands[0].Columns["ComprarM"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
            e.Layout.Bands[0].Columns["Dinero"].Header.Caption = "Comprar (Dinero)";

            e.Layout.Bands[0].Columns["Precio de compra"].Width = 90;
            e.Layout.Bands[0].Columns["Comprar"].Width = 90;
            e.Layout.Bands[0].Columns["ComprarM"].Width = 90;
            e.Layout.Bands[0].Columns["Dinero"].Width = 90;
            e.Layout.Bands[0].Columns["VI ajustado"].Width = 90;
            e.Layout.Bands[0].Columns["Stock ajustado"].Width = 90;
            e.Layout.Bands[0].Columns["Ideal"].Width = 90;

            e.Layout.Bands[0].Columns["Precio de compra"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            //e.Layout.Bands[0].Columns["VI Anterior"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Comprar"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["ComprarM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Dinero"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Ideal"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["VI ajustado"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Stock ajustado"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Días stock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            e.Layout.Bands[0].Columns["NumInBuy"].Hidden = true;
            e.Layout.Bands[0].Columns["Price"].Hidden = true;
            e.Layout.Bands[0].Columns["WhsCode"].Hidden = true;
            e.Layout.Bands[0].Columns["BWeight1"].Hidden = true;
            e.Layout.Bands[0].Columns["Volumen"].Hidden = true;
            e.Layout.Bands[0].Columns["CardCode"].Hidden = true;
            e.Layout.Bands[0].Columns["WhsName"].Hidden = true;
            e.Layout.Bands[0].Columns["Currency"].Hidden = true;
            e.Layout.Bands[0].Columns["Rate"].Hidden = true;

            e.Layout.Bands[0].Columns["Precio de compra"].Format = "C2";
            e.Layout.Bands[0].Columns["Comprar"].Format = "N0";
            e.Layout.Bands[0].Columns["ComprarM"].Format = "N0";
            e.Layout.Bands[0].Columns["Dinero"].Format = "C2";
            e.Layout.Bands[0].Columns["Ideal"].Format = "N0";
            e.Layout.Bands[0].Columns["VI ajustado"].Format = "N2";
            e.Layout.Bands[0].Columns["Stock ajustado"].Format = "N0";
            e.Layout.Bands[0].Columns["Días stock"].Format = "N0";

            e.Layout.Bands[0].Columns["ComprarM"].CellAppearance.BackColor = Color.FromName("Info");
            e.Layout.Bands[0].Columns["Dinero"].Formula = "[ComprarM]*[Price]*[NumInBuy]";
            e.Layout.Bands[0].Columns["Comprar"].Formula = "[ComprarM]*[NumInBuy]";
        }

        private void dgvVolumen_AfterCellUpdate(object sender, CellEventArgs e)
        {
            this.Totales(Tbl_Volumen);
        }

        private void dgvVolumen_InitializeRow(object sender, InitializeRowEventArgs e)
        {

        }

        private void dgvTotales_ClickCellButton(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Index == 4)
            {
                this.CrearOC(e.Cell.Row.Cells[0].Value.ToString());
            }
        }

        private void cbProveedor1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ClasesSGUV.Form.ControlsForms.setDataSource(clbZonas1, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetZonaProveedor, cbProveedor1.SelectedValue, "N"), "Zona", "Zona", string.Empty);

            #region Minimos de compra
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_Compras", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 2);
                    command.Parameters.AddWithValue("@CardCode", cbProveedor1.SelectedValue);

                    DataTable tbl = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl);

                    txtMinimo.Text = tbl.Rows[0].Field<decimal>(0).ToString("C2");
                    txtUnidad.Text = tbl.Rows[0].Field<string>(1);
                }
            }
            #endregion
        }

        private void btnConsultar1_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbProveedor1.SelectedIndex == 0)
                {
                    this.SetMensaje("Selecciona un proveedor", 5000, Color.Red, Color.White);
                    return;
                }
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Volumen", connection))
                    {
                        command.CommandTimeout = 0;
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 4);
                        command.Parameters.AddWithValue("@CardCode", cbProveedor1.SelectedValue);
                        command.Parameters.AddWithValue("@A_1", txtA.Text);
                        command.Parameters.AddWithValue("@AA_1", txtAA.Text);
                        command.Parameters.AddWithValue("@AAA_1", txtAAA.Text);
                        //command.Parameters.AddWithValue("@Monto", textBox1.Text == string.Empty ? "1" : textBox1.Text);
                        //command.Parameters.AddWithValue("@Moneda", rbMXP.Checked == true ? "MXP" : "USD");
                        //command.Parameters.AddWithValue("@TC", textBox2.Text);
                        //command.Parameters.AddWithValue("@Tipo", rbDinero.Checked ? "DIN" : "PZ");
                        command.Parameters.AddWithValue("@Zonas", ClasesSGUV.Form.ControlsForms.getCadena(clbZonas1, string.Empty, false, "Zona"));

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        Tbl_Volumen.Clear();
                        da.Fill(Tbl_Volumen);

                        dgvDatos.DataSource = Tbl_Volumen;
                        dgvVolumen.DataSource = Tbl_Volumen;

                        this.Totales(Tbl_Volumen);
                    }
                }
            }
            catch (Exception ex)
            {


                MessageBox.Show("Error inesperado: " + ex.Message, "Halconet", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
    }
}
