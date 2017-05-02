using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace H_Compras.Compras 
{
    public partial class frmIndicadores : Constantes.frmEmpty
    {
        DataSet DataPendientes = new DataSet();
        DataSet DataStockOut = new DataSet();
        DataSet DataDesabasto = new DataSet();
        DataSet DataSobrestock = new DataSet();
        DataSet DataArribos = new DataSet();
        decimal itemsComprador = decimal.Zero;

        public frmIndicadores()
        {
            InitializeComponent();
        }

        private void GetItems() 
        {
            using (SqlConnection connection = new  SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_Indicadores", connection))
                {
                    string com = cbCompradores.SelectedValue.ToString();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 0;
                    command.Parameters.AddWithValue("@TipoConsulta", 6);
                    command.Parameters.AddWithValue("@Comprador",  cbCompradores.SelectedValue.ToString());

                    connection.Open();

                   itemsComprador = Convert.ToDecimal(command.ExecuteScalar());
                }
            }
        }

        private void frmIndicadores_Load(object sender, EventArgs e)
        {
            try
            {
                this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                if (ClasesSGUV.Login.Rol == (int)ClasesSGUV.Propiedades.RolesHalcoNET.Administrador)
                    pictureBox1.Image = Properties.Resources.small_load;

                nuevoToolStripButton.Enabled = false;
                guardarToolStripButton.Enabled = false;
                exportarToolStripButton.Enabled = false;
                ayudaToolStripButton.Enabled = false;

                Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);
                dgvPendientes.DataSource = null;
                ClasesSGUV.Form.ControlsForms.setDataSource(cbCompradores, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.PlaneadoresCompra, null, string.Empty), "U_VLGX_PLC", "U_VLGX_PLC", "Todos");

                PathHelp = "http://hntsolutions.net/manual/module_14_4.htm?ms=AAAAAAA%3D&st=MA%3D%3D&sct=NDY1MQ%3D%3D&mw=MjU1";
            }
            catch (Exception)
            {

            }
                
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DataPendientes = null;
                DataPendientes = new DataSet();
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Indicadores", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 0;
                        command.Parameters.AddWithValue("@TipoConsulta", 1);
                        command.Parameters.AddWithValue("@Comprador", cbCompradores.SelectedValue);

                        connection.Open();
                        //connection.BeginTransaction();
                        //command.Transaction = new SqlTransaction();
                        SqlDataReader reader = command.ExecuteReader();
                        DataTable tbl = new DataTable();
                        tbl.Load(reader);

                        //DataTable tbl = new DataTable();
                        //SqlDataAdapter da = new SqlDataAdapter();
                        //da.SelectCommand = command;
                        //da.Fill(tbl);

                        //dgvPendientes.DataSource = tbl;

                        var qry = from item in tbl.AsEnumerable()
                                  group item by new
                                  {
                                      Descripcion = "Pendientes de compra"
                                  } into grouped
                                  select new
                                  {
                                      Descripcion = grouped.Key.Descripcion,
                                      Piezas = grouped.Sum(ix => ix.Field<decimal>("PZ")),
                                      MXP = grouped.Sum(ix => ix.Field<decimal>("MXP")),
                                      USD = grouped.Sum(ix => ix.Field<decimal>("USD"))
                                  };

                        DataTable tbl2 = new DataTable("details");
                        tbl2 = Constantes.Clases.ListConverter.ToDataTable(qry);
                        dgvPendientes.DataSource = tbl2;

                        var qry1 = from item in tbl.AsEnumerable()
                                   group item by new
                                   {
                                       Proveedor = item.Field<string>("S/N"),
                                       Nombre = item.Field<string>("CardName")
                                   } into grouped
                                   select new
                                   {
                                       Proveedor = grouped.Key.Proveedor,
                                       Nombre = grouped.Key.Nombre,
                                       Piezas = grouped.Sum(ix => ix.Field<decimal>("PZ")),
                                       MXP = grouped.Sum(ix => ix.Field<decimal>("MXP")),
                                       USD = grouped.Sum(ix => ix.Field<decimal>("USD"))
                                   };

                        DataTable tbl1 = new DataTable("header");
                        tbl1 = Constantes.Clases.ListConverter.ToDataTable(qry1);

                        DataPendientes.Tables.Add(tbl1);
                        DataPendientes.Tables.Add(tbl);

                        DataRelation relation = new DataRelation("relation", tbl1.Columns["Proveedor"], tbl.Columns["S/N"]);
                        DataPendientes.Relations.Add(relation);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                this.GetItems();
                btnConsultar.Enabled = false;
                pictureBox1.Visible = true;

                bgPendientes.RunWorkerAsync();
                bgStockout.RunWorkerAsync();
                bgDesabasto.RunWorkerAsync();
                bgSobrestock.RunWorkerAsync();
                bgArribos.RunWorkerAsync();
            }
            catch (Exception)
            {
                btnConsultar.Enabled = true;
                pictureBox1.Visible = false;
            }
            finally
            {
               
            }
        }

        private void bgStockout_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DataStockOut = null;
                DataStockOut = new DataSet();
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Indicadores", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 0;
                        command.Parameters.AddWithValue("@TipoConsulta", 2);
                        command.Parameters.AddWithValue("@Comprador", cbCompradores.SelectedValue);

                        DataTable tbl = new DataTable("Details");
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(tbl);

                        DataTable tbl1 = new DataTable("Header");

                        var qry1 = from item in tbl.AsEnumerable()
                                   group item by new
                                   {
                                       Descripcion = "Partidas en stockout",
                                   } into grouped
                                   select new
                                   {
                                       Descripcion = grouped.Key.Descripcion,
                                       Items = itemsComprador,
                                       StockOut = grouped.Count(),
                                       P = itemsComprador > decimal.Zero ? grouped.Count() / itemsComprador : decimal.Zero
                                   };

                        tbl1 = Constantes.Clases.ListConverter.ToDataTable(qry1);

                        dgvStockout.DataSource = tbl1;

                        DataStockOut.Tables.Add(tbl);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!bgPendientes.IsBusy)
                if (!bgDesabasto.IsBusy)
                    if (!bgSobrestock.IsBusy)
                        if (!bgStockout.IsBusy)
                            if (!bgArribos.IsBusy)
                            {
                                btnConsultar.Enabled = true;
                                pictureBox1.Visible = false;
                            }
        }

        private void bgDesabasto_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DataDesabasto = null;
                DataDesabasto = new DataSet();

                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Indicadores", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 0;
                        command.Parameters.AddWithValue("@TipoConsulta", 3);
                        command.Parameters.AddWithValue("@Comprador", cbCompradores.SelectedValue);

                        DataTable tbl = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(tbl);

                        DataTable tbl1 = new DataTable("Header");
                        
                        var qry1 = from item in tbl.AsEnumerable()
                                   group item by new
                                   {
                                       Descripcion = "Partidas en desabasto",
                                   } into grouped
                                   select new
                                   {
                                       Descripcion = grouped.Key.Descripcion,
                                       Items = itemsComprador,
                                       Num = grouped.Count(),
                                       Monto = grouped.Sum(i => i.Field<decimal>("Monto")),
                                       P = itemsComprador > decimal.Zero ? grouped.Count() / itemsComprador : decimal.Zero
                                   };
              
                        tbl1 = Constantes.Clases.ListConverter.ToDataTable(qry1);

                        dgvDesabasto.DataSource = tbl1;

                        DataDesabasto.Tables.Add(tbl);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void bgSobrestock_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DataSobrestock = null;
                DataSobrestock = new DataSet();

                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Indicadores", connection))
                    {
                        string com = cbCompradores.SelectedValue.ToString();
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 0;
                        command.Parameters.AddWithValue("@TipoConsulta", 4);
                        command.Parameters.AddWithValue("@Comprador", com);

                        DataTable tbl = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(tbl);
                        
                        DataTable tbl1 = new DataTable("Header");

                        var qry1 = from item in tbl.AsEnumerable()
                                   group item by new
                                   {
                                       Descripcion = "Partidas en sobrestock",
                                   } into grouped
                                   select new
                                   {
                                       Descripcion = grouped.Key.Descripcion,
                                       Items = itemsComprador,
                                       Num = grouped.Count(),
                                       Monto = grouped.Sum(i => i.Field<decimal>("Monto")),
                                       P = itemsComprador > decimal.Zero ? grouped.Count() / itemsComprador : decimal.Zero
                                   };

                        tbl1 = Constantes.Clases.ListConverter.ToDataTable(qry1);

                        dgvSobrestock.DataSource = tbl1;

                        DataSobrestock.Tables.Add(tbl);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void dgv_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[1].Header.Caption = "Total items";

            e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[2].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[3].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[4].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            e.Layout.Bands[0].Columns[0].Width = 130;
            e.Layout.Bands[0].Columns[1].Width = 60;
            e.Layout.Bands[0].Columns[2].Width = 60;
            e.Layout.Bands[0].Columns[4].Width = 60;
            e.Layout.Bands[0].Columns[3].Width = 80;

            e.Layout.Bands[0].Columns[1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
            e.Layout.Bands[0].Columns[2].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
            e.Layout.Bands[0].Columns[3].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
            e.Layout.Bands[0].Columns[4].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

            e.Layout.Bands[0].Columns[1].Format = "N0";
            e.Layout.Bands[0].Columns[2].Format = "N0";
            e.Layout.Bands[0].Columns[4].Format = "P2";
            e.Layout.Bands[0].Columns[3].Format = "C0";

            e.Layout.Bands[0].Columns[4].Header.Caption = "%";
        }

        private void dgvPendientes_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[2].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            e.Layout.Bands[0].Columns[0].Width = 100;
            e.Layout.Bands[0].Columns[1].Width = 80;
            e.Layout.Bands[0].Columns[2].Width = 80;
            e.Layout.Bands[0].Columns[3].Width = 80;

            e.Layout.Bands[0].Columns[1].Format = "N0";
            e.Layout.Bands[0].Columns[2].Format = "C2";
            e.Layout.Bands[0].Columns[3].Format = "C2";

            e.Layout.Bands[0].Columns[1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[2].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[3].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            //e.Layout.Bands[0].Columns[0].ValueList = ultraGrid1;
        }

        private void ultraDropDownPendientes_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            //e.Layout.Bands[0].Columns[3].Width = 50;
            //e.Layout.Bands[0].Columns[1].Width = 250;
            //e.Layout.Bands[0].Columns[0].Width = 50;

            //e.Layout.Bands[0].Columns[3].Hidden = true;
            //e.Layout.Bands[0].Columns[7].Hidden = true;
            //e.Layout.Bands[0].Columns[8].Hidden = true;
            //e.Layout.Bands[0].Columns[9].Hidden = true;

            //e.Layout.Bands[0].Columns[7].Hidden = true;
            //e.Layout.Bands[0].Columns[8].Hidden = true;
            //e.Layout.Bands[0].Columns[9].Hidden = true;

            //e.Layout.Bands[0].Columns[0].ValueList = ultraDropDown2;

        }
        
        #region Detalles
        private void btn1_Click(object sender, EventArgs e)
        {
            try
            {
                frmDetalleIndicadores formulario = new frmDetalleIndicadores(DataPendientes, "PENDIENTES");
                formulario.MdiParent = this.MdiParent;
                formulario.Show();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            try
            {
                frmDetalleIndicadores formulario = new frmDetalleIndicadores(DataStockOut, "STOCKOUT");
                formulario.MdiParent = this.MdiParent;
                formulario.Show();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            try
            {
                frmDetalleIndicadores formulario = new frmDetalleIndicadores(DataDesabasto, "DESABASTO");
                formulario.MdiParent = this.MdiParent;
                formulario.Show();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            try
            {
                frmDetalleIndicadores formulario = new frmDetalleIndicadores(DataSobrestock, "SOBRESTOCK");
                formulario.MdiParent = this.MdiParent;
                formulario.Show();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
      
        private void btnArribos_Click(object sender, EventArgs e)
        {
            try
            {
                frmDetalleIndicadores formulario = new frmDetalleIndicadores(DataArribos, "ARRIBOS");
                formulario.MdiParent = this.MdiParent;
                formulario.Show();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }  
        #endregion

        private void bgArribos_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DataArribos = null;
                DataArribos = new DataSet();

                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Indicadores", connection))
                    {
                        string com = cbCompradores.SelectedValue.ToString();
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 0;
                        command.Parameters.AddWithValue("@TipoConsulta", 5);
                        command.Parameters.AddWithValue("@Comprador", com);

                        DataTable tbl = new DataTable("Details");
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(tbl);

                        DataTable tbl1 = new DataTable();
                        DataTable tbl2 = new DataTable("Header");

                        var qry1 = (from item in tbl.AsEnumerable()
                                    select new
                                    {
                                        DocNum = item.Field<Int32>("DocNum"),
                                        Completas = item.Field<string>("Estatus") == "COMPLETA" ? 1 : 0,
                                        Parciales = item.Field<string>("Estatus") == "PARCIAL" ? 1 : 0
                                    }).Distinct();

                        var qryFinal = from item in qry1
                                       group item by new
                                       {
                                           Descripcion = "Arribos"
                                       } into grouped
                                       select new
                                       {
                                           Descripcion = grouped.Key.Descripcion,
                                           Completas = grouped.Sum(ix => ix.Completas),
                                           Parciales = grouped.Sum(ix => ix.Parciales)
                                       };

                        var qryHeader = (from item in tbl.AsEnumerable()
                                         select new
                                         {
                                             DocNum = item.Field<Int32>("DocNum"),
                                             Proveedor = item.Field<string>("CardName"),
                                             Estatus = item.Field<string>("Estatus"),
                                             Vencido = item.Field<decimal>("Vencidos")
                                         }).Distinct();

                        tbl1 = Constantes.Clases.ListConverter.ToDataTable(qryFinal);
                        tbl2 = Constantes.Clases.ListConverter.ToDataTable(qryHeader);

                        dgvArribos.DataSource = tbl1;

                        DataArribos.Tables.Add(tbl2);
                        DataArribos.Tables.Add(tbl);

                        DataRelation relation = new DataRelation("relation1", tbl2.Columns["DocNum"], tbl.Columns["DocNum"]);
                        DataArribos.Relations.Add(relation);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void dgvArribos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[2].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            e.Layout.Bands[0].Columns[0].Width = 130;
            e.Layout.Bands[0].Columns[1].Width = 100;
            e.Layout.Bands[0].Columns[2].Width = 100;

            e.Layout.Bands[0].Columns[1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
            e.Layout.Bands[0].Columns[2].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
        }

        private void dgvStockout_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[1].Header.Caption = "Total items";
            e.Layout.Bands[0].Columns[3].Header.Caption = "%";

            e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[2].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[3].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            e.Layout.Bands[0].Columns[0].Width = 130;
            e.Layout.Bands[0].Columns[1].Width = 70;
            e.Layout.Bands[0].Columns[2].Width = 70;
            e.Layout.Bands[0].Columns[3].Width = 70;

            e.Layout.Bands[0].Columns[1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[2].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[3].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            e.Layout.Bands[0].Columns[1].Format = "N0";
            e.Layout.Bands[0].Columns[2].Format = "N0";
            e.Layout.Bands[0].Columns[3].Format = "P2";
        }
    }
}
