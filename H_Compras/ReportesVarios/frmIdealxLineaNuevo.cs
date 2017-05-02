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
    public partial class frmIdealxLineaNuevo : Constantes.frmEmpty
    {
        Datos.Connection connection = new Datos.Connection();
        Object[] valuesOut = new Object[] { };
        string Almacenes = string.Empty;
        string Lineas = string.Empty;

        public frmIdealxLineaNuevo()
        {
            InitializeComponent();
        }

        private void Total()
        {
            /*
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
            */
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
                //dgvClientes.DataSource = null;

                Lineas = ClasesSGUV.Form.ControlsForms.getCadena(clbLinea, string.Empty, false, "ItmsGrpCod");
                Almacenes = ClasesSGUV.Form.ControlsForms.getCadena(clbAlmacen, "'", false, "WhsCode");

                DataTable tbl =
                            connection.GetDataTable("LOG",
                                                    "sp_Reportes",
                                                    new string[] { },
                                                    new string[] { "@TipoConsulta", "@LP", "@Lineas", "@Almacenes", "@Tipo" },
                                                    ref valuesOut, 19, cbLP.Text.Substring(0, 2), Lineas, Almacenes, rbCM.Checked ? "CM" : "LM");

                //tbl.Columns["Comentarios"].ReadOnly = false;
                //tbl.Columns["Accion"].ReadOnly = false;

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

                DataTable tbl =
                          connection.GetDataTable("LOG",
                                                  "sp_Reportes",
                                                  new string[] { },
                                                  new string[] { "@TipoConsulta", "@Lineas", "@Almacenes" },
                                                  ref valuesOut, 20, dgvDatos.ActiveRow.Cells["ItmsGrpNam"].Value, Almacenes);

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
                          connection.GetDataTable("LOG",
                                                  "sp_Reportes",
                                                  new string[] { },
                                                  new string[] { "@TipoConsulta", "@Lineas", "@Almacenes", "@WhsCode" },
                                                  ref valuesOut, 21, dgvAlmacenes.ActiveRow.Cells["Línea"].Value, Almacenes, dgvAlmacenes.ActiveRow.Cells["WhsCode"].Value);

                dgvVentas.DataSource = tbl;
            }
            catch (Exception)
            {
            }
        }

        private void dgvVentas_AfterRowActivate(object sender, EventArgs e)
        {
            /*
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
             * */
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGridLayout layout = e.Layout;
            Infragistics.Win.UltraWinGrid.UltraGridBand band = layout.Bands[0];
            e.Layout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed;
            e.Layout.Override.SummaryFooterCaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            e.Layout.Bands[0].Summaries.Clear();

            band.Columns["ItmsGrpNam"].Header.Caption = "Línea";

            band.Columns["Stock"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Ideal"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Diferencia"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Solicitado"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["A"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["B"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["C"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Remate"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Obsoleto"].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns["Artículos nuevos"].CellAppearance.TextHAlign = HAlign.Right;

            
            band.Columns["Stock"].Format = "C0";
            band.Columns["Ideal"].Format = "C0";
            band.Columns["Diferencia"].Format = "C0";
            band.Columns["Solicitado"].Format = "C0";
            band.Columns["A"].Format = "C0";
            band.Columns["B"].Format = "C0";
            band.Columns["C"].Format = "C0";
            band.Columns["Remate"].Format = "C0";
            band.Columns["Obsoleto"].Format = "C0";
            band.Columns["Artículos nuevos"].Format = "C0";

            band.Columns["ItmsGrpNam"].Header.Fixed = true;

            Infragistics.Win.UltraWinGrid.SummarySettings summary1 = band.Summaries.Add("T1", SummaryType.Sum, band.Columns["Diferencia"]);
            summary1.DisplayFormat = "{0:C1}";
            summary1.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

            Infragistics.Win.UltraWinGrid.SummarySettings summary2 = band.Summaries.Add("T2", SummaryType.Sum, band.Columns["Stock"]);
            summary2.DisplayFormat = "{0:C1}";
            summary2.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

            Infragistics.Win.UltraWinGrid.SummarySettings summary3 = band.Summaries.Add("T3", SummaryType.Sum, band.Columns["Ideal"]);
            summary3.DisplayFormat = "{0:C1}";
            summary3.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

            Infragistics.Win.UltraWinGrid.SummarySettings summary4 = band.Summaries.Add("T4", SummaryType.Sum, band.Columns["Solicitado"]);
            summary4.DisplayFormat = "{0:C1}";
            summary4.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

            Infragistics.Win.UltraWinGrid.SummarySettings summary5 = band.Summaries.Add("T5", SummaryType.Sum, band.Columns["A"]);
            summary5.DisplayFormat = "{0:C1}";
            summary5.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

            Infragistics.Win.UltraWinGrid.SummarySettings summary6 = band.Summaries.Add("T6", SummaryType.Sum, band.Columns["B"]);
            summary6.DisplayFormat = "{0:C1}";
            summary6.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

            Infragistics.Win.UltraWinGrid.SummarySettings summary7 = band.Summaries.Add("T7", SummaryType.Sum, band.Columns["C"]);
            summary7.DisplayFormat = "{0:C1}";
            summary7.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

            Infragistics.Win.UltraWinGrid.SummarySettings summary8 = band.Summaries.Add("T8", SummaryType.Sum, band.Columns["Remate"]);
            summary8.DisplayFormat = "{0:C1}";
            summary8.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

            Infragistics.Win.UltraWinGrid.SummarySettings summary9 = band.Summaries.Add("T9", SummaryType.Sum, band.Columns["Obsoleto"]);
            summary9.DisplayFormat = "{0:C1}";
            summary9.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

            Infragistics.Win.UltraWinGrid.SummarySettings summary10 = band.Summaries.Add("T10", SummaryType.Sum, band.Columns["Artículos nuevos"]);
            summary10.DisplayFormat = "{0:C1}";
            summary10.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

            foreach (var item in band.Columns)
            {
                item.CellActivation = Activation.NoEdit;
                item.Width = 110;
            }

            band.Columns["ItmsGrpNam"].Width = 95;
        }

        private void dgvDatos_SummaryValueChanged(object sender, SummaryValueChangedEventArgs e)
        {
        }

        private void dgvAlmacenes_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            
            Infragistics.Win.UltraWinGrid.UltraGridLayout layout = e.Layout;
            Infragistics.Win.UltraWinGrid.UltraGridBand band = layout.Bands[0];

            band.Columns["Línea"].Hidden = true;
            band.Columns["WhsCode"].Hidden = true;

            foreach (var item in band.Columns)
            {
                item.CellActivation = Activation.NoEdit;
                item.Width = 105;
                item.CellAppearance.TextHAlign = HAlign.Right;
                item.Format = "C0";
            }

            band.Columns[4].Format = "N0";
            band.Columns["Almacén"].Width = 90;
            band.Columns["Almacén"].CellAppearance.TextHAlign = HAlign.Left;
        }

        private void dgvVentas_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGridLayout layout = e.Layout;
            Infragistics.Win.UltraWinGrid.UltraGridBand band = layout.Bands[0];
            e.Layout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed;
            e.Layout.Override.SummaryFooterCaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            e.Layout.Bands[0].Summaries.Clear();

            band.Columns["Num"].Hidden = true;

            band.Columns[3].Format = "C2";
            band.Columns[4].Format = "N0";

            band.Columns[3].CellAppearance.TextHAlign = HAlign.Right;
            band.Columns[4].CellAppearance.TextHAlign = HAlign.Right;

            foreach (var item in band.Columns)
            {
                item.CellActivation = Activation.NoEdit;
                item.Width = 110;
            }
            band.Columns["Año"].Width = 70;

            Infragistics.Win.UltraWinGrid.SummarySettings summary1 = band.Summaries.Add("T1", SummaryType.Sum, band.Columns[3]);
            summary1.DisplayFormat = "{0:C1}";
            summary1.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            summary1.Appearance.BackColor = Color.White;
            summary1.Appearance.FontData.Bold = DefaultableBoolean.True;

            Infragistics.Win.UltraWinGrid.SummarySettings summary2 = band.Summaries.Add("T2", SummaryType.Sum, band.Columns[4]);
            summary2.DisplayFormat = "{0:N1}";
            summary2.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            summary2.Appearance.BackColor = Color.White;
            summary2.Appearance.FontData.Bold = DefaultableBoolean.True;
        }

        private void dgvClientes_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            /*
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
             * */
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnActualizar_Click(sender, e);
        }

        private void dgvAlmacenes_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                
                if (e.Row.Cells["Almacén"].Value.ToString().Equals("TOTAL"))
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
            /*
            this.Total();
             */
        }

        private void dgvDatos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            /*
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
             * */
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
