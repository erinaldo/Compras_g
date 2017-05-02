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

namespace H_Compras.Inventarios.ListasPrecios
{
    public partial class frmPrecios : Constantes.frmEmpty
    {
        string ItemCode;
        DataTable tblFacturas = new DataTable();

        public frmPrecios()
        {
            InitializeComponent();
        }

        private void frmPrecios_Load(object sender, EventArgs e)
        {
            try
            {
                ClasesSGUV.Form.ControlsForms.setDataSource(cbListasPrecios, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListasPrecios, ClasesSGUV.Login.Rol, string.Empty), "ListName", "ListNum", "Lista de precios");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbLineas, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.LineasTodas, ClasesSGUV.Login.Rol, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "Línea");

                dgvDatos.KeyDown -= new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);
                dgvDatos.KeyDown += new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);

                dgvDatos.KeyPress -= new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);
                dgvDatos.KeyPress += new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);

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
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                dgvFacturas.DataSource = null;
                ItemCode = string.Empty;
            
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_ListaPrecios", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 3);
                        command.Parameters.AddWithValue("@ItmsGrpCod", cbLineas.SelectedValue);
                        command.Parameters.AddWithValue("@PriceList", cbListasPrecios.SelectedValue);
                        command.Parameters.AddWithValue("@Desde", dtpDesde.Value);
                        command.Parameters.AddWithValue("@Hasta", dtpHasta.Value);

                        DataTable tbl = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(tbl);
                        dgvDatos.DataSource = tbl;

                    }
                }

                txtFactor.Clear();
                txtDescuento.Clear();
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void btnConsultVentas_Click(object sender, EventArgs e)
        {
            try
            {
                dgvFacturas.DataSource = null;
                ItemCode = dgvDatos.ActiveRow.Cells["ItemCode"].Value.ToString();
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_ListaPrecios", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 4);
                        command.Parameters.AddWithValue("@ItemCode", ItemCode);
                        command.Parameters.AddWithValue("@Desde", dtpDesde.Value);
                        command.Parameters.AddWithValue("@Hasta", dtpHasta.Value);
                        command.Parameters.AddWithValue("@PriceList", cbListasPrecios.SelectedValue);

                        DataTable tbl = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(tbl);
                        dgvVentas.DataSource = tbl;

                    }
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void dgvVentas_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["Mes1"].Hidden = true;
            e.Layout.Bands[0].Columns["Venta"].Format = "C2";
            e.Layout.Bands[0].Columns["Venta"].Width = 90;
            e.Layout.Bands[0].Columns["Venta"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["PZ"].Format = "N0";
            e.Layout.Bands[0].Columns["PZ"].Width = 90;
            e.Layout.Bands[0].Columns["PZ"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
        }

        private void dgvDatos_AfterRowActivate(object sender, EventArgs e)
        {
            btnConsultVentas_Click(sender, e);
        }

        //string itemResp = string.Empty;
        private void dgvVentas_AfterRowActivate(object sender, EventArgs e)
        {
            try
            {
                #region Datos
                dgvFacturas.DataSource = null;

               // if (!ItemCode.Equals(itemResp))
                {
                    //itemResp = ItemCode;
                    using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("sp_ListaPrecios", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@TipoConsulta", 5);
                            command.Parameters.AddWithValue("@ItemCode", ItemCode);
                            command.Parameters.AddWithValue("@Desde", dtpDesde.Value);
                            command.Parameters.AddWithValue("@Hasta", dtpHasta.Value);
                            command.Parameters.AddWithValue("@PriceList", cbListasPrecios.SelectedValue);

                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = command;
                            tblFacturas = new DataTable();
                            da.Fill(tblFacturas);
                            //dgvFacturas.DataSource = tblFacturas;

                        }
                    }
                }
                #endregion

                string m = Convert.ToString(dgvVentas.ActiveRow.Cells["Mes"].Value);

                int mes = 0;

                switch (m)
                {
                    case "Enero": mes = 1; break;
                    case "Febrero": mes = 2; break;
                    case "Marzo": mes = 3; break;
                    case "Abril": mes = 4; break;
                    case "Mayo": mes = 5; break;
                    case "Junio": mes = 6; break;
                    case "Julio": mes = 7; break;
                    case "Agosto": mes = 8; break;
                    case "Septiembre": mes = 9; break;
                    case "Octubre": mes = 10; break;
                    case "Noviembre": mes = 11; break;
                    case "Diciembre": mes = 12; break;
                    default:
                        break;
                }

                int año = Convert.ToInt32(dgvVentas.ActiveRow.Cells["Año"].Value);

                var tbl = (from item in tblFacturas.AsEnumerable()
                          where item.Field<DateTime>("Fecha").Month == mes
                          & item.Field<DateTime>("Fecha").Year == año
                          select item);

                if (tbl.Count() > 0)
                    dgvFacturas.DataSource = tbl.CopyToDataTable();
            }
            catch (Exception)
            {
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
        //    e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
        //    e.Layout.Bands[0].Columns["ItemName"].Header.Fixed = true;
        //    e.Layout.Bands[0].Columns["Price"].Header.Fixed = true;
        //    e.Layout.Bands[0].Columns["Clasificación"].Header.Fixed = true;
        //    e.Layout.Bands[0].Columns["Factor"].Header.Fixed = true;
        //    e.Layout.Bands[0].Columns["Maximo"].Header.Fixed = true;
        //    e.Layout.Bands[0].Columns["Minimo"].Header.Fixed = true;
        //    e.Layout.Bands[0].Columns["Promedio"].Header.Fixed = true;
            if(Convert.ToInt32(cbListasPrecios.SelectedValue) != 9)
                e.Layout.Bands[0].Columns["FactorMM"].Hidden = true;
            e.Layout.Bands[0].Columns["Ap"].Hidden = true;

            e.Layout.Bands[0].Columns["FactorMM"].Format = "N3";
            e.Layout.Bands[0].Columns["FactorMM"].Width = 100;
            e.Layout.Bands[0].Columns["FactorMM"].Header.Caption = "Factor Minimo Mayoreo";
            e.Layout.Bands[0].Columns["FactorMM"].CellAppearance.TextHAlign = HAlign.Right;


            Infragistics.Win.UltraWinCalcManager.UltraCalcManager calcManager;
            calcManager = new Infragistics.Win.UltraWinCalcManager.UltraCalcManager(this.Container);
            e.Layout.Grid.CalcManager = calcManager;
            e.Layout.Bands[0].Columns["ItemName"].Width = 120;
            e.Layout.Bands[0].Columns["Price"].Width = 100;
            e.Layout.Bands[0].Columns["Price USD"].Width = 100;
            e.Layout.Bands[0].Columns["Utilidad"].Width = 100;
            e.Layout.Bands[0].Columns["Descuento"].Width = 100;
            e.Layout.Bands[0].Columns["Factor2"].Width = 100;
            e.Layout.Bands[0].Columns["PF2"].Width = 100;
            e.Layout.Bands[0].Columns["UD2"].Width = 100;
            e.Layout.Bands[0].Columns["Clasificación"].Width = 90;
            e.Layout.Bands[0].Columns["Factor3"].Width = 100;
            e.Layout.Bands[0].Columns["PF3"].Width = 100;

            e.Layout.Bands[0].Columns["Descuento"].Header.Caption = "Factor Final";
            e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Artículo";
            e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Descripción";
            e.Layout.Bands[0].Columns["Price"].Header.Caption = cbListasPrecios.Text + " ($)";
            e.Layout.Bands[0].Columns["Price USD"].Header.Caption = cbListasPrecios.Text + " (USD)";
            e.Layout.Bands[0].Columns["PF2"].Header.Caption = "Precio Final";
            e.Layout.Bands[0].Columns["Factor2"].Header.Caption = "Factor Final";
            e.Layout.Bands[0].Columns["UD2"].Header.Caption = "Utilidad Final";
            e.Layout.Bands[0].Columns["Factor3"].Header.Caption = "Factor Final c/Descuento";
            e.Layout.Bands[0].Columns["PF3"].Header.Caption = "Precuo Final c/Descuento";
            e.Layout.Bands[0].Columns["Precio final"].Header.Caption = "Precio Final";
            e.Layout.Bands[0].Columns["Utilidad con descuento"].Header.Caption = "Utilidad Final";

            e.Layout.Bands[0].Columns["Price"].Header.Appearance.BackColor = Color.FromArgb(255, 192, 0);
            e.Layout.Bands[0].Columns["Price USD"].Header.Appearance.BackColor = Color.FromArgb(255, 192, 0);
            e.Layout.Bands[0].Columns["Factor"].Header.Appearance.BackColor = Color.FromArgb(255, 192, 0);
            e.Layout.Bands[0].Columns["Utilidad"].Header.Appearance.BackColor = Color.FromArgb(255, 192, 0); 
            
            e.Layout.Bands[0].Columns["Descuento"].CellAppearance.BackColor = Color.FromName("Info");
            e.Layout.Bands[0].Columns["PF2"].CellAppearance.BackColor = Color.FromName("Info");
            e.Layout.Bands[0].Columns["Volumen"].CellAppearance.BackColor = Color.FromName("Info");
            e.Layout.Bands[0].Columns["Precio final"].Width = 100;
            e.Layout.Bands[0].Columns["Utilidad con descuento"].Width = 100;

            e.Layout.Bands[0].Columns["Descuento"].Header.Appearance.BackColor = Color.FromArgb(0, 176, 80);
            e.Layout.Bands[0].Columns["Precio final"].Header.Appearance.BackColor = Color.FromArgb(0, 176, 80);
            e.Layout.Bands[0].Columns["Utilidad con descuento"].Header.Appearance.BackColor = Color.FromArgb(0, 176, 80);
            
            e.Layout.Bands[0].Columns["Factor2"].Header.Appearance.BackColor = Color.FromArgb(47, 117, 181);
            e.Layout.Bands[0].Columns["PF2"].Header.Appearance.BackColor = Color.FromArgb(47, 117, 181);
            e.Layout.Bands[0].Columns["UD2"].Header.Appearance.BackColor = Color.FromArgb(47, 117, 181);
          
            e.Layout.Bands[0].Columns["Factor2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["PF2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["UD2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            e.Layout.Bands[0].Columns["Precio final"].Formula = "[Costo Base]*[Descuento]";
            e.Layout.Bands[0].Columns["Utilidad con descuento"].Formula = "1-([Costo Base]/[Precio final])";

            e.Layout.Bands[0].Columns["Factor2"].Formula = "[PF2]/[Costo Base]";
            e.Layout.Bands[0].Columns["UD2"].Formula = "1-([Costo Base]/[PF2])";

            e.Layout.Bands[0].Columns["Factor3"].Formula = "[PF3]/[Costo Base]";
            
            e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Precio final"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Utilidad"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Descuento"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["utilidad con descuento"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Maximo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Minimo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Promedio"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Factor3"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["PF3"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            e.Layout.Bands[0].Columns["Price"].Format = "C2";
            e.Layout.Bands[0].Columns["Price USD"].Format = "C2";
            e.Layout.Bands[0].Columns["Price USD"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            e.Layout.Bands[0].Columns["Precio final"].Format = "C2";
            e.Layout.Bands[0].Columns["Utilidad"].Format = "P2";
            e.Layout.Bands[0].Columns["Descuento"].Format = "N3";
            e.Layout.Bands[0].Columns["Utilidad con descuento"].Format = "P2";
            e.Layout.Bands[0].Columns["Aprobar"].Format = "C2";
            e.Layout.Bands[0].Columns["Factor2"].Format = "N3";
            e.Layout.Bands[0].Columns["PF2"].Format = "C2";
            e.Layout.Bands[0].Columns["UD2"].Format = "P2";
            e.Layout.Bands[0].Columns["Factor3"].Format = "N3";
            e.Layout.Bands[0].Columns["PF3"].Format = "C2";
            e.Layout.Bands[0].Columns["Volumen"].Format = "C2";

            e.Layout.Bands[0].Columns["Costo Base"].Hidden = true;
            e.Layout.Bands[0].Columns["Costo Base"].Format = "C2";
            e.Layout.Bands[0].Columns["Costo Base"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Factor"].Format = "N3";
            e.Layout.Bands[0].Columns["Factor"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            e.Layout.Bands[0].Columns["Maximo"].Format = "C2";
            e.Layout.Bands[0].Columns["Minimo"].Format = "C2";
            e.Layout.Bands[0].Columns["Promedio"].Format = "C2";

            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                item.Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
            }

            e.Layout.Bands[0].Columns["Descuento"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
            e.Layout.Bands[0].Columns["PF2"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
            e.Layout.Bands[0].Columns["Volumen"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
        }

        private void dgvDatos_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (dgvDatos.ActiveRow != null)
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_ListaPrecios", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 6);
                        command.Parameters.AddWithValue("@PriceList", cbListasPrecios.SelectedValue);
                        command.Parameters.AddWithValue("@ItemCode", dgvDatos.ActiveRow.Cells["ItemCode"].Value);
                        command.Parameters.AddWithValue("@Price", dgvDatos.ActiveRow.Cells["PF2"].Value);
                        command.Parameters.AddWithValue("@Factor", dgvDatos.ActiveRow.Cells["Descuento"].Value);
                        command.Parameters.AddWithValue("@Volumen", dgvDatos.ActiveRow.Cells["Volumen"].Value);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private void dgvDatos_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            try
            {
                e.Row.Cells["Aprobar"].Value = "Aprobar";
                e.Row.Cells["Aprobar"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                e.Row.Cells["Aprobar"].ButtonAppearance.BorderColor = Color.CadetBlue;
                e.Row.Cells["Aprobar"].Value = "Aprobar";
                e.Row.Cells["Aprobar"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                e.Row.Cells["Aprobar"].ButtonAppearance.BorderColor = Color.CadetBlue;

                if (!string.IsNullOrEmpty(e.Row.Cells["Ap"].Value.ToString()))
                {
                    e.Row.Cells["Aprobar"].Appearance.BackColor = Color.Green;
                    e.Row.Cells["Aprobar"].Appearance.BackColor2 = Color.Green;
                    e.Row.Cells["Aprobar"].ButtonAppearance.BackColor = Color.Green;
                    e.Row.Cells["Aprobar"].ButtonAppearance.BackColor2 = Color.Green;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void dgvFacturas_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["Nombre"].Width = 150;

            e.Layout.Bands[0].Columns["Precio unitario"].Format = "C2";
            e.Layout.Bands[0].Columns["Precio real"].Format = "C2";
            e.Layout.Bands[0].Columns["PZ"].Format = "N0";
            e.Layout.Bands[0].Columns["Precio unitario"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Precio real"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["PZ"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["PZ"].Width = 90;
            e.Layout.Bands[0].Columns["PZ"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
        }

        private void dgvFacturas_DoubleClickCell(object sender, Infragistics.Win.UltraWinGrid.DoubleClickCellEventArgs e)
        {
            H_Ventas.Consulta.frmFacturas form = new H_Ventas.Consulta.frmFacturas(dgvFacturas.ActiveRow.Cells["Factura"].Value.ToString());
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var item in dgvDatos.Rows)
            {
                //item.Cells["Descuento"].Value = DBNull.Value;
                item.Cells["PF3"].Value = Convert.ToDecimal(item.Cells["Price"].Value) - (Convert.ToDecimal(item.Cells["Price"].Value) * (Convert.ToDecimal(txtDescuento.Text) / 100));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var item in dgvDatos.Rows)
            {
                item.Cells["Descuento"].Value = Convert.ToDecimal(txtFactor.Text);
                item.Cells["PF2"].Value = DBNull.Value;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (var item in dgvDatos.Rows)
            {
                item.Cells["Descuento"].Value = DBNull.Value;
                item.Cells["PF2"].Value = DBNull.Value;
                item.Cells["PF3"].Value = DBNull.Value;

                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_ListaPrecios", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 6);
                        command.Parameters.AddWithValue("@PriceList", cbListasPrecios.SelectedValue);
                        command.Parameters.AddWithValue("@ItemCode", item.Cells["ItemCode"].Value);
                        command.Parameters.AddWithValue("@Price", item.Cells["PF2"].Value);
                        command.Parameters.AddWithValue("@Factor", item.Cells["Descuento"].Value);
                        command.Parameters.AddWithValue("@Volumen", item.Cells["Volumen"].Value);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            txtFactor.Clear();
            txtDescuento.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (var item in dgvDatos.Rows)
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_ListaPrecios", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 6);
                        command.Parameters.AddWithValue("@PriceList", cbListasPrecios.SelectedValue);
                        command.Parameters.AddWithValue("@ItemCode", item.Cells["ItemCode"].Value);
                        command.Parameters.AddWithValue("@Price", item.Cells["PF2"].Value);
                        command.Parameters.AddWithValue("@Factor", item.Cells["Descuento"].Value);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private void dgvDatos_BeforeCellActivate(object sender, CancelableCellEventArgs e)
        {
            if (dgvDatos.DisplayLayout.Bands[0].Columns["Descuento"].Index == e.Cell.Column.Index)
                if ((dgvDatos.ActiveRow.Cells["PF2"].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(dgvDatos.ActiveRow.Cells["PF2"].Value)) != decimal.Zero)
                {
                    MessageBox.Show("No se puede modificar factor, elimine el precio primero.");
                    e.Cancel = true;
                }

            if (dgvDatos.DisplayLayout.Bands[0].Columns["PF2"].Index == e.Cell.Column.Index)
                if ((dgvDatos.ActiveRow.Cells["Descuento"].Value == DBNull.Value ? decimal.Zero : Convert.ToDecimal(dgvDatos.ActiveRow.Cells["Descuento"].Value)) != decimal.Zero)
                {
                    MessageBox.Show("No se puede modificar precio, elimine el factor primero.");
                    e.Cancel = true;
                }
        }

        private void dgvDatos_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            //try
            //{
            //    string name = e.ColumnHeaders[0].Column.Header.Caption;

            //    MessageBox.Show( dgvDatos.DisplayLayout.Bands[0].Columns[name].Index.ToString());
            //}
            //catch (Exception)
            //{|
                
            //    throw;
            //}
        }
    }
}
