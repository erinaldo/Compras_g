using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace H_Compras.ReportesVarios
{
    public partial class frmSobreStock : Constantes.frmEmpty
    {
        Datos.Connection connection = new Datos.Connection();
        Object[] valuesOut = new Object[] { };
        string Almacenes = string.Empty;
        string Lineas = string.Empty;

        public frmSobreStock()
        {
            InitializeComponent();
        }

        private void Total()
        {
            decimal AN, R, D, CE, PPC, Otro;
            AN = R = D = CE = PPC = Otro = decimal.Zero;

            foreach (var item in dgvDatos.Rows)
            {
                if (!item.IsFilteredOut)
                {
                    if (item.Cells["C"].Value.ToString().Equals("Artículo nuevo"))
                        AN += item.Cells["Diferencia ($)"].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(item.Cells["Diferencia ($)"].Value);
                    else if (item.Cells["C"].Value.ToString().Equals("Remate"))
                        R += item.Cells["Diferencia ($)"].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(item.Cells["Diferencia ($)"].Value);
                    else if (item.Cells["C"].Value.ToString().Equals("Devolución"))
                        D += item.Cells["Diferencia ($)"].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(item.Cells["Diferencia ($)"].Value);
                    else if (item.Cells["C"].Value.ToString().Equals("Compra especial"))
                        CE += item.Cells["Diferencia ($)"].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(item.Cells["Diferencia ($)"].Value);
                    else if (item.Cells["C"].Value.ToString().Equals("PPC"))
                        PPC += item.Cells["Diferencia ($)"].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(item.Cells["Diferencia ($)"].Value);
                    else
                        Otro += item.Cells["Diferencia ($)"].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(item.Cells["Diferencia ($)"].Value);
                }
            }

            txtAN.Text = AN.ToString("C0");
            txtRemate.Text = R.ToString("C0");
            txtDevolucion.Text = D.ToString("C0");
            txtCE.Text = CE.ToString("C0");
            txtPPC.Text = PPC.ToString("C0");
            txtOtros.Text = Otro.ToString("C0");
        }

        private void frmSobreStock_Load(object sender, EventArgs e)
        {
            try
            {
                this.actualizarToolStripButton.Click -= new EventHandler(btnActualizar_Click);
                this.actualizarToolStripButton.Click += new EventHandler(btnActualizar_Click);

                ClasesSGUV.Form.ControlsForms.setDataSource(clbLinea, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.LineasTodas, string.Empty, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "--Todos--");
                ClasesSGUV.Form.ControlsForms.setDataSource(clbAlmacen, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesTodos, string.Empty, string.Empty), "WhsName", "WhsCode", "--Todos--");

                clbLinea.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
                clbLinea.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);

                clbAlmacen.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
                clbAlmacen.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);

                cbLP.SelectedIndex = 1;

                DataTable tbl_Acciones = new DataTable();

                tbl_Acciones = connection.GetDataTable("SGUV",
                                                        "PJ_IdealLinea",
                                                        new string[] { },
                                                        new string[] { "@TipoConsulta" },
                                                        ref valuesOut, 7);

                udpAcciones.SetDataBinding(tbl_Acciones, null);
                udpAcciones.ValueMember = "Name";
                udpAcciones.DisplayMember = "Name";
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                dgvDatos.DataSource = null;
                dgvAlmacenes.DataSource = null;
                dgvVentas.DataSource = null;
                dgvClientes.DataSource = null;

                Lineas = ClasesSGUV.Form.ControlsForms.getCadena(clbLinea, string.Empty, false, "ItmsGrpCod");
                Almacenes = ClasesSGUV.Form.ControlsForms.getCadena(clbAlmacen, "'", false, "WhsCode");

                DataTable tbl =
                            connection.GetDataTable("LOG",
                                                    "sp_Reportes",
                                                    new string[] { },
                                                    new string[] { "@TipoConsulta", "@LP", "@Lineas", "@Almacenes", "@Tipo" },
                                                    ref valuesOut, 16, cbLP.Text.Substring(0, 2), Lineas, Almacenes, rbCM.Checked ? "CM" : "LM");

                tbl.Columns["Comentarios"].ReadOnly = false;
                tbl.Columns["Accion"].ReadOnly = false;

                dgvDatos.DataSource = tbl;

                this.Total();
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }

        }

        private void dgvDatos_AfterRowActivate(object sender, EventArgs e)
        {
            try
            {
                dgvAlmacenes.DataSource = null;
                dgvVentas.DataSource = null;
                dgvClientes.DataSource = null;

                DataTable tbl =
                          connection.GetDataTable("SGUV",
                                                  "PJ_IdealLinea",
                                                  new string[] { },
                                                  new string[] { "@TipoConsulta", "@Articulo", "@Almacenes" },
                                                  ref valuesOut, 3, dgvDatos.ActiveRow.Cells["Artículo"].Value, Almacenes);

                dgvAlmacenes.DataSource = tbl;
            }
            catch (Exception)
            {
            }
        }

        private void dgvAlmacenes_AfterRowActivate(object sender, EventArgs e)
        {
            try
            {
                dgvVentas.DataSource = null;
                DataTable tbl =
                          connection.GetDataTable("SGUV",
                                                  "PJ_IdealLinea",
                                                  new string[] { },
                                                  new string[] { "@TipoConsulta", "@Articulo", "@Almacenes", "@Almacen" },
                                                  ref valuesOut, 5, dgvAlmacenes.ActiveRow.Cells["Artículo"].Value, Almacenes, dgvAlmacenes.ActiveRow.Cells["WhsCode"].Value);

                dgvVentas.DataSource = tbl;
            }
            catch (Exception)
            {
            }
        }

        private void dgvVentas_AfterRowActivate(object sender, EventArgs e)
        {
            try
            {
                dgvClientes.DataSource = null;
                DataTable tbl =
                          connection.GetDataTable("SGUV",
                                                  "PJ_IdealLinea",
                                                  new string[] { },
                                                  new string[] { "@TipoConsulta", "@Articulo", "@Almacenes", "@Almacen", "@Mes", "@Año" },
                                                  ref valuesOut, 4, dgvVentas.ActiveRow.Cells["Artículo"].Value, Almacenes, dgvAlmacenes.ActiveRow.Cells["WhsCode"].Value, dgvVentas.ActiveRow.Cells["Num"].Value, dgvVentas.ActiveRow.Cells["Año"].Value);

                dgvClientes.DataSource = tbl;
            }
            catch (Exception)
            {
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGridLayout layout = e.Layout;
            Infragistics.Win.UltraWinGrid.UltraGridBand band = layout.Bands[0];
            e.Layout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed;
            e.Layout.Override.SummaryFooterCaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            e.Layout.Bands[0].Summaries.Clear();

            band.Columns["coment"].Header.Caption = "--";
            band.Columns["coment"].Width = 25;
            //band.Columns["RT"].Header.Caption = string.Empty;
            //band.Columns["DT"].Header.Caption = string.Empty;
            //band.Columns["CPT"].Header.Caption = string.Empty;
            //band.Columns["PPCT"].Header.Caption = string.Empty;

            //band.Columns["ANT"].Width = 1;
            //band.Columns["RT"].Width = 1;
            //band.Columns["DT"].Width = 1;
            //band.Columns["CPT"].Width = 1;
            //band.Columns["PPCT"].Width = 1;

            band.Columns["Solicitado ($)"].Hidden = true;
            band.Columns["C"].Hidden = true;


            band.Columns["Proveedor"].Width = 90;
            band.Columns["Artículo"].Width = 80;
            band.Columns["ABC"].Width = 50;
            band.Columns["Stock"].Width = 80;
            band.Columns["Ideal"].Width = 80;
            band.Columns["Diferencia"].Width = 80;
            band.Columns["Solicitado"].Width = 80;
            band.Columns["Stock ($)"].Width = 80;
            band.Columns["Ideal ($)"].Width = 80;
            band.Columns["Diferencia ($)"].Width = 80;
            band.Columns["Solicitado ($)"].Width = 80;
            band.Columns["Veces ideal"].Width = 70;
            band.Columns["Días stock"].Width = 70;
            band.Columns["Accion"].Width = 70;
            band.Columns["Comentarios"].Width = 70;

            band.Columns["Accion"].ValueList = udpAcciones;

            band.Columns["Stock"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Ideal"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Diferencia"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Solicitado"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Stock ($)"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Ideal ($)"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Diferencia ($)"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Solicitado ($)"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Veces ideal"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Días stock"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["%"].CellAppearance.TextHAlign = HAlign.Right;

            band.Columns["Stock"].Format = "N0";
            band.Columns["Ideal"].Format = "N0";
            band.Columns["Diferencia"].Format = "N0";
            band.Columns["Solicitado"].Format = "N0";
            band.Columns["Stock ($)"].Format = "C2";
            band.Columns["Ideal ($)"].Format = "C2";
            band.Columns["Diferencia ($)"].Format = "C2";
            band.Columns["Solicitado ($)"].Format = "C2";
            band.Columns["Veces ideal"].Format = "N2";
            band.Columns["Días stock"].Format = "N2";
            band.Columns["%"].Format = "P2";

            band.Columns["Proveedor"].Header.Fixed = true;
            band.Columns["Artículo"].Header.Fixed = true;
            band.Columns["ABC"].Header.Fixed = true;

            Infragistics.Win.UltraWinGrid.SummarySettings summary1 = band.Summaries.Add("T1", SummaryType.Sum, band.Columns["Diferencia ($)"]);
            summary1.DisplayFormat = "{0:C1}";
            summary1.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

            Infragistics.Win.UltraWinGrid.SummarySettings summary2 = band.Summaries.Add("T2", SummaryType.Sum, band.Columns["Stock ($)"]);
            summary2.DisplayFormat = "{0:C1}";
            summary2.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

            Infragistics.Win.UltraWinGrid.SummarySettings summary3 = band.Summaries.Add("T3", SummaryType.Sum, band.Columns["Ideal ($)"]);
            summary3.DisplayFormat = "{0:C1}";
            summary3.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

            foreach (var item in band.Columns)
            {
                item.CellActivation = Activation.NoEdit;
            }

            band.Columns["Accion"].CellActivation = Activation.AllowEdit;
            //e.Layout.Bands[0].Columns["Accion"].ValueList = udpAcciones;
        }

        private void dgvDatos_SummaryValueChanged(object sender, SummaryValueChangedEventArgs e)
        {
            //decimal val = 0;
            //string c = e.SummaryValue.Key.ToString();
            //if (c.Equals("T1"))
            //    txtAN.Text = Convert.ToDecimal(e.SummaryValue.Value.ToString()).ToString("C2");
            //if (c.Equals("T2"))
            //    txtRemate.Text = Convert.ToDecimal(e.SummaryValue.Value.ToString()).ToString("C2");
            //if (c.Equals("T3"))
            //    txtDevolucion.Text = Convert.ToDecimal(e.SummaryValue.Value.ToString()).ToString("C2");
            //if (c.Equals("T4"))
            //    txtCE.Text = Convert.ToDecimal(e.SummaryValue.Value.ToString()).ToString("C2");
            //if (c.Equals("T5"))
            //    txtPPC.Text = Convert.ToDecimal(e.SummaryValue.Value.ToString()).ToString("C2");
        }

        private void dgvAlmacenes_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGridLayout layout = e.Layout;
            Infragistics.Win.UltraWinGrid.UltraGridBand band = layout.Bands[0];

            band.Columns[1].Hidden = true;

            band.Columns[0].Width = 80;
            band.Columns[1].Width = 40;
            band.Columns[2].Width = 80;
            band.Columns[3].Width = 90;
            band.Columns[4].Width = 90;
            band.Columns[5].Width = 90;
            band.Columns[6].Width = 90;
            band.Columns[7].Width = 90;

            band.Columns[3].Format = "C2";
            band.Columns[4].Format = "N2";
            band.Columns[5].Format = "N0";
            band.Columns[6].Format = "N0";
            band.Columns[7].Format = "N0";

            band.Columns[3].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns[4].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns[5].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns[6].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns[7].CellAppearance.TextHAlign = HAlign.Right;

            foreach (var item in band.Columns)
            {
                item.CellActivation = Activation.NoEdit;
            }
        }

        private void dgvVentas_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGridLayout layout = e.Layout;
            Infragistics.Win.UltraWinGrid.UltraGridBand band = layout.Bands[0];
            e.Layout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed;
            e.Layout.Override.SummaryFooterCaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            e.Layout.Bands[0].Summaries.Clear();

            band.Columns[0].Hidden = true;

            band.Columns[1].Width = 90;
            band.Columns[2].Width = 60;
            band.Columns[3].Width = 50;
            band.Columns[4].Width = 90;
            band.Columns[5].Width = 90;

            band.Columns[4].Format = "C2";
            band.Columns[5].Format = "N0";

            band.Columns[4].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns[5].CellAppearance.TextHAlign = HAlign.Right;

            foreach (var item in band.Columns)
            {
                item.CellActivation = Activation.NoEdit;
            }

            Infragistics.Win.UltraWinGrid.SummarySettings summary1 = band.Summaries.Add("T1", SummaryType.Sum, band.Columns[4]);
            summary1.DisplayFormat = "{0:C1}";
            summary1.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            summary1.Appearance.BackColor = Color.White;
            summary1.Appearance.FontData.Bold = DefaultableBoolean.True;


            Infragistics.Win.UltraWinGrid.SummarySettings summary2 = band.Summaries.Add("T2", SummaryType.Sum, band.Columns[5]);
            summary2.DisplayFormat = "{0:N1}";
            summary2.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            summary2.Appearance.BackColor = Color.White;
            summary2.Appearance.FontData.Bold = DefaultableBoolean.True;
        }

        private void dgvClientes_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGridLayout layout = e.Layout;
            Infragistics.Win.UltraWinGrid.UltraGridBand band = layout.Bands[0];

            band.Columns[2].Format = "C2";
            band.Columns[3].Format = "N0";

            band.Columns[2].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns[3].CellAppearance.TextHAlign = HAlign.Right;

            band.Columns[2].Width = 90;
            band.Columns[3].Width = 90;

            foreach (var item in band.Columns)
            {
                item.CellActivation = Activation.NoEdit;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnActualizar_Click(sender, e);
        }

        private void dgvAlmacenes_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                if (e.Row.Cells["Almacen"].Value.ToString().Equals("TOTAL"))
                {
                    e.Row.Appearance.FontData.Bold = DefaultableBoolean.True;
                }
            }
            catch (Exception)
            {

            }
        }

        private void dgvDatos_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
        {
            this.Total();
        }

        private void dgvDatos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Comentarios"].Value = "Comentarios";
            e.Row.Cells["Comentarios"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            e.Row.Cells["Comentarios"].ButtonAppearance.BorderColor = Color.CadetBlue;

            if (!string.IsNullOrEmpty(e.Row.Cells["Coment"].Value.ToString()))
            {
                e.Row.Cells["Comentarios"].Appearance.BackColor = Color.Green;
                e.Row.Cells["Comentarios"].Appearance.BackColor2 = Color.Green;

                e.Row.Cells["Comentarios"].ButtonAppearance.BackColor = Color.Green;
                e.Row.Cells["Comentarios"].ButtonAppearance.BackColor2 = Color.Green;
            }
            if (Convert.ToString(e.Row.Cells["C"].Value).Equals("Artículo nuevo"))
            {
                e.Row.Cells["Artículo"].Appearance.BackColor = Color.FromArgb(202, 40, 147);
                e.Row.Cells["Artículo"].Appearance.ForeColor = Color.White;
            }
            decimal p = decimal.Zero;
            decimal.TryParse(e.Row.Cells["%"].Value.ToString(), out p);
            if (p >= 1)
            {
                e.Row.Cells["%"].Appearance.BackColor = Color.FromArgb(0, 176, 80);//verde
                e.Row.Cells["%"].Appearance.ForeColor = Color.Black;
            }
            else if (p < 1 & p>= (decimal)0.25)
            {
                e.Row.Cells["%"].Appearance.BackColor = Color.FromArgb(255, 255, 0); //amarillo
                e.Row.Cells["%"].Appearance.ForeColor = Color.Black;
            }
            else if (p < (decimal)0.25)
            {
                e.Row.Cells["%"].Appearance.BackColor = Color.FromArgb(255, 0, 0);//rojo
                e.Row.Cells["%"].Appearance.ForeColor = Color.White;
            }
        }

        private void dgvDatos_ClickCellButton(object sender, CellEventArgs e)
        {
            try
            {
                frmComentarios formularios = new frmComentarios(dgvDatos.ActiveRow.Cells["Artículo"].Value.ToString(),
                    dgvDatos.ActiveRow.Cells["Descripción"].Value.ToString());
                formularios.MdiParent = this.MdiParent;
                formularios.Show();
            }
            catch (Exception)
            {

            }
        }

        private void udpAcciones_AfterCloseUp(object sender, DropDownEventArgs e)
        {
        }

        private void dgvDatos_AfterCellListCloseUp(object sender, CellEventArgs e)
        {
            //int x = connection.Ejecutar("SGUV",
            //                                       "PJ_IdealLinea",
            //                                       new string[] { },
            //                                       new string[] { "@TipoConsulta", "@Accion", "@Articulo", "@Comentario" },
            //                                       ref valuesOut, 6, udpAcciones.SelectedRow.Cells["Name"].Value, dgvDatos.ActiveRow.Cells["Artículo"].Value, dgvDatos.ActiveRow.Cells["Comentarios"].Value);
   
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var item in dgvDatos.DisplayLayout.Rows)
                {
                   // if (item.Cells["Artículo"].Value.ToString().Equals("006"))
                    {
                        int x = connection.Ejecutar("SGUV",
                                                     "PJ_IdealLinea",
                                                     new string[] { },
                                                     new string[] { "@TipoConsulta", "@Accion", "@Articulo", "@Comentario" },
                                                     ref valuesOut, 6, item.Cells["Accion"].Value, item.Cells["Artículo"].Value, item.Cells["Comentarios"].Value);
                    }
                }

                this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
            }
            catch (Exception)
            {

            }
        }
    }
}
