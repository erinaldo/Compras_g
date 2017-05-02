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
using Infragistics.Win;
using Infragistics.Shared;
using Infragistics.Win.UltraWinGrid;

namespace H_Compras.Inventarios
{
    public partial class frmAnalisisObsLento : Constantes.frmEmpty
    {
        public frmAnalisisObsLento()
        {
            InitializeComponent();
        }
        DataTable tblDatos = new DataTable();
        DataTable Datos1 = new DataTable();

        public enum Columnas
        {
            ItemCode,
            ItemName,
            ItmsGrpNam,
            CardName,
            ClasificacionF,
            Accion,
            Currency,
            Price,
            OnHand,
            Total,
            Promedio,
            Planning,
            MesesIdeal,
            Horizonte,
            Ideal,
            PrimeraEntrada,
            MesesPE
        }

        public void Formato(DataGridView dgv)
        {
            dgv.Columns[(int)Columnas.ItemCode].Width = 85;
            dgv.Columns[(int)Columnas.ItemName].Width = 200;
            dgv.Columns[(int)Columnas.ItmsGrpNam].Width = 90;
            dgv.Columns[(int)Columnas.CardName].Width = 120;
            dgv.Columns[(int)Columnas.ClasificacionF].Width = 90;
            dgv.Columns[(int)Columnas.Price].Width = 90;
            dgv.Columns[(int)Columnas.OnHand].Width = 70;
            dgv.Columns[(int)Columnas.Currency].Width = 60;
            dgv.Columns[(int)Columnas.Total].Width = 90;
            dgv.Columns[(int)Columnas.Accion].Width = 90;
            dgv.Columns[(int)Columnas.Planning].Width = 90;
            dgv.Columns[(int)Columnas.MesesIdeal].Width = 80;
            dgv.Columns[(int)Columnas.Horizonte].Width = 80;

            dgv.Columns[(int)Columnas.Price].DefaultCellStyle.Format = "C2";
            dgv.Columns[(int)Columnas.OnHand].DefaultCellStyle.Format = "N0";
            dgv.Columns[(int)Columnas.Promedio].DefaultCellStyle.Format = "N2";
            dgv.Columns[(int)Columnas.Total].DefaultCellStyle.Format = "C2";

            dgv.Columns[(int)Columnas.Price].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas.OnHand].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas.Total].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas.Promedio].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgv.Columns[(int)Columnas.Planning].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas.MesesIdeal].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas.Horizonte].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            int x = 0;
            foreach (DataGridViewColumn item in dgvDetalle.Columns)
            {
                if (x > 0)
                {
                    item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    item.DefaultCellStyle.Format = "N0";
                    item.Width = 85;
                    item.ReadOnly = true;
                }
                x++;
            }
        }

        public void CargarLinea(ComboBox _cb, string _inicio)
        {
            SqlCommand command = new SqlCommand("PJ_Compras", new SqlConnection(ClasesSGUV.Propiedades.conectionSGUV));
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@TipoConsulta", 15);

            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(table);

            DataRow row = table.NewRow();
            row["Nombre"] = _inicio;
            row["Codigo"] = "0";
            table.Rows.InsertAt(row, 0);

            _cb.DataSource = table;
            _cb.DisplayMember = "Nombre";
            _cb.ValueMember = "Codigo";
        }

        public void CargarProveedores(ComboBox _cb, string _inicio)
        {
            SqlCommand command = new SqlCommand("PJ_Compras", new SqlConnection(ClasesSGUV.Propiedades.conectionSGUV));
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@TipoConsulta", 16);

            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(table);

            DataRow row = table.NewRow();
            row["Nombre"] = _inicio;
            row["Codigo"] = "0";
            table.Rows.InsertAt(row, 0);

            _cb.DataSource = table;
            _cb.DisplayMember = "Nombre";
            _cb.ValueMember = "Codigo";
        }

        public void CargarAcciones(ComboBox _cb, string _inicio)
        {
            SqlCommand command = new SqlCommand("sp_Inventario", new SqlConnection());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@TipoConsulta", 10);

            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(table);
            
            _cb.DataSource = table;
            _cb.DisplayMember = "Nombre";
            _cb.ValueMember = "Codigo";
        }

        private void frmAnalisisObsLento_Load(object sender, EventArgs e)
        {
            this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

            this.CargarLinea(cbLinea, string.Empty);
            this.CargarProveedores(cbProveedor, string.Empty);        

            cboFiltroClasificacion.SelectedIndex = 0;
            cboFiltroAccion.SelectedIndex = 0;
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                foreach (DataGridViewRow item in (sender as DataGridView).Rows)
                {
                    if (string.IsNullOrEmpty(item.Cells[(int)Columnas.ClasificacionF].Value.ToString()))
                    {
                        item.Cells[(int)Columnas.ClasificacionF].Style.BackColor = Color.Red;
                        item.Cells[(int)Columnas.ClasificacionF].Style.ForeColor = Color.White;
                    }
                    else
                    {
                        item.Cells[(int)Columnas.ClasificacionF].Style.BackColor = Color.Green;
                        item.Cells[(int)Columnas.ClasificacionF].Style.ForeColor = Color.Black;
                    }

                    if (item.Cells[(int)Columnas.Accion].Value.ToString().Equals("Descontinuado") &&
                        item.Cells[(int)Columnas.Planning].Value.ToString().Equals(string.Empty))
                    {
                        item.Cells[(int)Columnas.Accion].Style.BackColor = Color.Red;
                        item.Cells[(int)Columnas.Accion].Style.ForeColor = Color.White;
                    }

                    if (item.Cells[(int)Columnas.Accion].Value.ToString().Equals("Obsoleto") &&
                        Convert.ToDecimal(item.Cells[(int)Columnas.Promedio].Value) == decimal.Zero //&&
                        //item.Cells[(int)Columnas.P_Obsoleto].Value.ToString().Equals("N")
                        )
                    {
                        item.Cells[(int)Columnas.Accion].Style.BackColor = Color.Orange;
                        item.Cells[(int)Columnas.Accion].Style.ForeColor = Color.White;
                    }

                    //if (item.Cells[(int)Columnas.Revisado].Value.ToString().Equals("Y") )
                    //{
                    //    item.Cells[(int)Columnas.ItemCode].Style.BackColor = Color.Green;
                    //    item.Cells[(int)Columnas.ItemCode].Style.ForeColor = Color.Black;
                    //}
                }
            }
            catch (Exception) { }
        }

        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataTable tbl = new DataTable();
                using (SqlConnection connecion = new SqlConnection(ClasesSGUV.Propiedades.conectionSGUV))
                {
                    using (SqlCommand command = new SqlCommand("PJ_Compras", connecion))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 19);
                        command.Parameters.AddWithValue("@Articulo", dgvDatos.Rows[e.RowIndex].Cells[(int)Columnas.ItemCode].Value.ToString());


                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.SelectCommand.CommandTimeout = 0;


                        da.Fill(tbl);

                        dgvDetalle.DataSource = tbl;

                        foreach (DataGridViewColumn item in dgvDetalle.Columns)
                        {
                            if (item.Index > 1)
                            {
                                item.Width = 70;
                                item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            }
                            else
                            {
                                item.Width = 75;
                                item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                            }

                        }

                        dgvDetalle.Rows[0].DefaultCellStyle.Format = "N0";
                        dgvDetalle.Rows[1].DefaultCellStyle.Format = "N2";
                    }

                }
            }
            catch (Exception)
            {

            }

            if (e.RowIndex > -1)
            {
                string __articulo = (sender as DataGridView).Rows[e.RowIndex].Cells[0].Value.ToString();

                SqlCommand command = new SqlCommand("PJ_Compras", new SqlConnection(ClasesSGUV.Propiedades.conectionSGUV));
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TipoConsulta", 21);
                command.Parameters.AddWithValue("@Articulo", __articulo);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = command;
                DataTable table = new DataTable();

                da.Fill(table);
                dgvVentas.DataSource = table;
                foreach (DataGridViewColumn item in dgvVentas.Columns)
                {
                    if (item.Index > 1)
                    {
                        item.Width = 60;
                        item.DefaultCellStyle.Format = "N0";
                        item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    else
                    {
                        item.Width = 75;
                        item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                    }

                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.tblDatos.Clear();
            this.Datos1 = new DataTable();
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_Reportes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 7);
                    command.Parameters.AddWithValue("@Lineas", this.cbLinea.SelectedValue);
                    command.Parameters.AddWithValue("@Proveedores", this.cbProveedor.SelectedValue);
                    command.Parameters.AddWithValue("@ItemCode", this.txtArticulo.Text);

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = command;
                    adapter.SelectCommand.CommandTimeout = 0;
                    adapter.Fill(this.Datos1);
                    this.dgvDatos1.DataSource = this.Datos1;
                }
            }
            try
            {
                DataTable filtrados = this.Datos1.Copy();
                this.Filtra(filtrados);
            }
            catch (Exception exception)
            {
                this.SetMensaje(exception.Message, 5000, Color.Red, Color.White);
            }
            this.Cursor = Cursors.Default;
        }

        private void cbFiltro_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                string Filtro = string.Empty;
                DataView VistaFiltrada = new DataView(Datos1);
                
                //----------------FILTRO PARA CLASIFICACIONES------------------------
                if (cboFiltroClasificacion.SelectedIndex == 0) 
                {
                    if(Filtro=="")
                        Filtro += "Clasificación like '%%'";
                    else
                        Filtro += " AND Clasificación like '%%'";
                }
                else if (cboFiltroClasificacion.SelectedIndex == 1)
                {
                    if (Filtro == "")
                        Filtro += "Clasificación=''";
                    else
                        Filtro += " AND Clasificación = ''";
                }
                else { 
                    if(Filtro=="")
                        Filtro += "Clasificación='" + cboFiltroClasificacion.Text + "'";
                    else
                        Filtro += " AND Clasificación='" + cboFiltroClasificacion.Text + "'";
                } 
                    
                //-----------------FILTRO PARA ACCIONES--------------------------------
                if (cboFiltroAccion.SelectedIndex == 0){
                    if(Filtro=="")
                        Filtro += "Acción like '%%'";
                    else
                        Filtro += " AND Acción like '%%'";
                }
                else if (cboFiltroAccion.SelectedIndex == 1)
                {
                    if (Filtro == "")
                        Filtro += "Acción=''";
                    else
                        Filtro += " AND Acción=''";
                }
                else { 
                    if(Filtro == "")
                        Filtro += "Acción='" + cboFiltroAccion.Text + "'";
                    else
                        Filtro += " AND Acción='" + cboFiltroAccion.Text + "'";
                } 
                //------------------------------------------------------------------------------
                VistaFiltrada.RowFilter = Filtro;
                dgvDatos1.DataSource = VistaFiltrada.ToTable();

                DataTable __datos = new DataTable();
                __datos = dgvDatos1.DataSource as DataTable;

                //-------------SUMATORIAS PARA CLASIFICACION--------------------------------------
                DataTable tblTClasificacion = new DataTable();
                tblTClasificacion.Columns.Add("Tipo", typeof(string));
                int index = 0; string Columnas = "";
                foreach (var item in cboFiltroClasificacion.Items)
                {
                    if (index > 1)
                    {
                        tblTClasificacion.Columns.Add(item.ToString(), typeof(decimal));
                        if (Columnas != "") { Columnas += "+"; Columnas += "[" + item.ToString() + "]"; }
                        else
                            Columnas += "[" + item.ToString() + "]";
                    }
                    index++;                 
                }
                
                tblTClasificacion.Columns.Add("Sin Clasificacion", typeof(decimal));
                if (Columnas != "") { Columnas += "+" + "[" + "Sin Clasificacion" + "]"; }
                else
                    Columnas += "[" + "Sin Clasificacion" + "]";

                tblTClasificacion.Columns.Add("TOTAL", typeof(decimal), Columnas);
                DataRow row1 = tblTClasificacion.NewRow();
                DataRow row2 = tblTClasificacion.NewRow();
                int co = 0;
                foreach (DataColumn col in tblTClasificacion.Columns)
                {
                    if (co == 0)
                    {
                        row1[co] = "(PZ)"; row2[co] = "($)";
                    }
                    else if(col.ColumnName=="Sin Clasificacion")
                    {
                        row1[co] = Convert.ToDecimal(__datos.Compute("Sum([Stock])", "Clasificación=''") == DBNull.Value ? 0 : __datos.Compute("Sum([Stock])", "Clasificación=''"));
                        row2[co] = Convert.ToDecimal(__datos.Compute("Sum([Total])", "Clasificación=''") == DBNull.Value ? 0 : __datos.Compute("Sum([Total])", "Clasificación=''"));
                    }
                    else if (col.ColumnName != "TOTAL" && col.ColumnName != "Sin Clasificacion")
                    {
                        row1[co] = Convert.ToDecimal(__datos.Compute("Sum([Stock])", "Clasificación='" + col.ColumnName + "'") == DBNull.Value ? 0 : __datos.Compute("Sum([Stock])", "Clasificación='" + col.ColumnName + "'"));
                        row2[co] = Convert.ToDecimal(__datos.Compute("Sum([Total])", "Clasificación='" + col.ColumnName + "'") == DBNull.Value ? 0 : __datos.Compute("Sum([Total])", "Clasificación='" + col.ColumnName + "'"));
                    }
                    co++;
                }
                tblTClasificacion.Rows.Add(row1);
                tblTClasificacion.Rows.Add(row2);

                dgvTotal2.DataSource = tblTClasificacion;
                dgvTotal2.Rows[0].DefaultCellStyle.Format = "N0";
                dgvTotal2.Rows[1].DefaultCellStyle.Format = "C2";

                //-------------------------------------------------------------------------------------------------------------

                DataTable tblTAccion = new DataTable();
                tblTAccion.Columns.Add("Tipo", typeof(string));
                int index1 = 0; string Columnas1 = "";
                foreach (var item in cboFiltroAccion.Items)
                {
                    if (index1 > 1)
                    {
                        tblTAccion.Columns.Add(item.ToString(), typeof(decimal));
                        if (Columnas1 != "") { Columnas1 += "+"; Columnas1 += "[" + item.ToString() + "]"; }
                        else
                            Columnas1 += "[" + item.ToString() + "]";
                    }
                    index1++;
                }

                tblTAccion.Columns.Add("Sin Accion", typeof(decimal));
                if (Columnas1 != "") { Columnas1 += "+" + "[" + "Sin Accion" + "]"; }
                else
                    Columnas1 += "[" + "Sin Accion" + "]";

                tblTAccion.Columns.Add("TOTAL", typeof(decimal), Columnas1);
                DataRow row3 = tblTAccion.NewRow();
                DataRow row4 = tblTAccion.NewRow();
                int co1 = 0;
                foreach (DataColumn col in tblTAccion.Columns)
                {
                    if (co1 == 0)
                    {
                        row3[co1] = "(PZ)"; row4[co1] = "($)";
                    }
                    else if (col.ColumnName == "Sin Accion")
                    {
                        row3[co1] = Convert.ToDecimal(__datos.Compute("Sum([Stock])", "Acción=''") == DBNull.Value ? 0 : __datos.Compute("Sum([Stock])", "Acción=''"));
                        row4[co1] = Convert.ToDecimal(__datos.Compute("Sum([Total])", "Acción=''") == DBNull.Value ? 0 : __datos.Compute("Sum([Total])", "Acción=''"));
                    }
                    else if (col.ColumnName != "TOTAL" && col.ColumnName != "Sin Accion")
                    {
                        row3[co1] = Convert.ToDecimal(__datos.Compute("Sum([Stock])", "Acción='" + col.ColumnName + "'") == DBNull.Value ? 0 : __datos.Compute("Sum([Stock])", "Acción='" + col.ColumnName + "'"));
                        row4[co1] = Convert.ToDecimal(__datos.Compute("Sum([Total])", "Acción='" + col.ColumnName + "'") == DBNull.Value ? 0 : __datos.Compute("Sum([Total])", "Acción='" + col.ColumnName + "'"));
                    }
                    co1++;
                }
                tblTAccion.Rows.Add(row3);
                tblTAccion.Rows.Add(row4);

                dgvTotales.DataSource = tblTAccion;
                dgvTotales.Rows[0].DefaultCellStyle.Format = "N0";
                dgvTotales.Rows[1].DefaultCellStyle.Format = "C2";

            }
            catch (Exception ex)
            {

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            txtArticulo.Clear();
            cbLinea.SelectedIndex = 0;
            cbProveedor.SelectedIndex = 0;
            cbFiltro.SelectedIndex = 0;

            dgvDatos.DataSource = null;
            dgvTotales.DataSource = null;
            dgvDetalle.DataSource = null;


        }
        
        private void dgvDetalle_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                int x = 0;
                foreach (DataGridViewCell item in (sender as DataGridView).Rows[1].Cells)
                {
                    if (x > 0)
                        if (Convert.ToDecimal(item.Value) > decimal.Zero)
                            item.Style.BackColor = Color.Green;
                    x++;
                }
            }
            catch (Exception)
            {
                
            }
        }

        private void dgvDatos1_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                column.CellActivation = Activation.NoEdit;
                column.Hidden = true;
            }

            e.Layout.Bands[0].Columns["Artículo"].Header.Fixed = true;
            e.Layout.Bands[0].Columns["Descripción"].Header.Fixed = true;
            e.Layout.Bands[0].Columns["Línea"].Header.Fixed = true;

            e.Layout.Bands[0].Columns["Artículo"].Hidden = false;
            e.Layout.Bands[0].Columns["Descripción"].Hidden = false;
            e.Layout.Bands[0].Columns["Línea"].Hidden = false;
            e.Layout.Bands[0].Columns["ABC"].Hidden = false;
            e.Layout.Bands[0].Columns["Clasificación"].Hidden = false;
            e.Layout.Bands[0].Columns["Acción"].Hidden = false;
            e.Layout.Bands[0].Columns["Stock"].Hidden = false;
            e.Layout.Bands[0].Columns["Ideal"].Hidden = false;
            e.Layout.Bands[0].Columns["Total_MXN"].Hidden = false;
            e.Layout.Bands[0].Columns["Meses ideal"].Hidden = false;
            e.Layout.Bands[0].Columns["Vta ultimo mes"].Hidden = false;
            e.Layout.Bands[0].Columns["Promedio vta 6 meses"].Hidden = false;
            e.Layout.Bands[0].Columns["Horizonte"].Hidden = false;
            e.Layout.Bands[0].Columns["FechaEntrada"].Hidden = false;
            
            e.Layout.Bands[0].Columns["Stock"].Format = "N0";
            e.Layout.Bands[0].Columns["Ideal"].Format = "N0";
            e.Layout.Bands[0].Columns["Total_MXN"].Format = "C2";
            e.Layout.Bands[0].Columns["Promedio vta 6 meses"].Format = "N0";
            e.Layout.Bands[0].Columns["Vta ultimo mes"].Format = "N0";
            e.Layout.Bands[0].Columns["Meses ideal"].Format = "N2";
            e.Layout.Bands[0].Columns["Horizonte"].Format = "N0";

            e.Layout.Bands[0].Columns["Stock"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["Meses ideal"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["Ideal"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["Total_MXN"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["Promedio vta 6 meses"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["Horizonte"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["Vta ultimo mes"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["Meses ideal"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["FechaEntrada"].CellAppearance.TextHAlign = HAlign.Center;

            e.Layout.Bands[0].Columns["Descripción"].Hidden = false;

            e.Layout.Bands[0].Columns["Stock"].Width = 90;
            e.Layout.Bands[0].Columns["Ideal"].Width = 90;
            e.Layout.Bands[0].Columns["Total_MXN"].Width = 90;
            e.Layout.Bands[0].Columns["Promedio vta 6 meses"].Width = 90;
            e.Layout.Bands[0].Columns["Horizonte"].Width = 90;
            e.Layout.Bands[0].Columns["Vta ultimo mes"].Width = 90;
            e.Layout.Bands[0].Columns["Meses ideal"].Width = 90;
            e.Layout.Bands[0].Columns["FechaEntrada"].Width = 90;
        }

        private void btnActualizarAcciones_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvDatos1.Selected.Rows.Count > 0)
                {
                    DialogResult result = MessageBox.Show("Estas apunto de actualizar " + dgvDatos1.Selected.Rows.Count.ToString() + " registros, deseas continuar?", "Confirmation", MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Yes)
                    {
                        foreach (UltraGridRow uRow in dgvDatos1.Selected.Rows)
                        {
                            string Articulo = uRow.Cells["Artículo"].Value.ToString();
                            string Accion = cboAcciones.Text.ToString();

                            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                            {
                                using (SqlCommand command = new SqlCommand("sp_Reportes", connection))
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.AddWithValue("@TipoConsulta", 9);
                                    command.Parameters.AddWithValue("@ItemCode", Articulo);
                                    command.Parameters.AddWithValue("@Accion", Accion);
                                    command.Parameters.AddWithValue("@UserID", ClasesSGUV.Login.Id_Usuario);
                                    connection.Open();
                                    command.ExecuteNonQuery();
                                }
                            }

                            uRow.Cells["Acción"].Value = Accion == "Quitar Acción" ? string.Empty : Accion;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Seleccione una fila para poder actualizar el tipo de accion", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

            }
            catch (Exception ex)
            {
                
               
            }
        }

        private void dgvDatos1_BeforeRowActivate(object sender, RowEventArgs e)
        {
            try
            {
                DataTable tbl = new DataTable();
                using (SqlConnection connecion = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Reportes", connecion))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 8);
                        command.Parameters.AddWithValue("@ItemCode", e.Row.Cells[(int)Columnas.ItemCode].Value.ToString());

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.SelectCommand.CommandTimeout = 0;

                        da.Fill(tbl);

                        dgvDetalle.DataSource = tbl;

                        foreach (DataGridViewColumn item in dgvDetalle.Columns)
                        {
                            if (item.Index > 1)
                            {
                                item.Width = 70;
                                item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            }
                            else
                            {
                                item.Width = 75;
                                item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                            }

                        }

                        dgvDetalle.Rows[0].DefaultCellStyle.Format = "N0";
                        dgvDetalle.Rows[1].DefaultCellStyle.Format = "N2";
                    }

                }
            }
            catch (Exception)
            {

            }

            if (e.Row.Index > -1)
            {
                string __articulo = e.Row.Cells[0].Value.ToString();

                SqlCommand command = new SqlCommand("sp_HistorialVentas", new SqlConnection(Datos.Clases.Constantes.conectionLog));
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TipoConsulta", 6);
                command.Parameters.AddWithValue("@Articulo", __articulo);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = command;
                DataTable table = new DataTable();

                da.Fill(table);
                dgvVentas.DataSource = table;
                foreach (DataGridViewColumn item in dgvVentas.Columns)
                {
                    if (item.Index > 1)
                    {
                        item.Width = 60;
                        item.DefaultCellStyle.Format = "N0";
                        item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    else
                    {
                        item.Width = 75;
                        item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                    }

                }
            }

        }

        private void Filtra(DataTable filtrados)
        {
            DataTable __datos = new DataTable();
            __datos = filtrados;//dgvDatos1.DataSource as DataTable;

            //-------------SUMATORIAS PARA CLASIFICACION--------------------------------------
            DataTable tblTClasificacion = new DataTable();
            tblTClasificacion.Columns.Add("Tipo", typeof(string));
            int index = 0; string Columnas = "";
            foreach (var item in cboFiltroClasificacion.Items)
            {
                if (index > 1)
                {
                    tblTClasificacion.Columns.Add(item.ToString(), typeof(decimal));
                    if (Columnas != "") { Columnas += "+"; Columnas += "[" + item.ToString() + "]"; }
                    else
                        Columnas += "[" + item.ToString() + "]";
                }
                index++;
            }

            tblTClasificacion.Columns.Add("Sin Clasificacion", typeof(decimal));
            if (Columnas != "") { Columnas += "+" + "[" + "Sin Clasificacion" + "]"; }
            else
                Columnas += "[" + "Sin Clasificacion" + "]";

            tblTClasificacion.Columns.Add("TOTAL", typeof(decimal), Columnas);
            DataRow row1 = tblTClasificacion.NewRow();
            DataRow row2 = tblTClasificacion.NewRow();
            int co = 0;
            foreach (DataColumn col in tblTClasificacion.Columns)
            {
                if (co == 0)
                {
                    row1[co] = "(PZ)"; row2[co] = "($)";
                }
                else if (col.ColumnName == "Sin Clasificacion")
                {
                    row1[co] = Convert.ToDecimal(__datos.Compute("Sum([Stock])", "Clasificación=''") == DBNull.Value ? 0 : __datos.Compute("Sum([Stock])", "Clasificación=''"));
                    row2[co] = Convert.ToDecimal(__datos.Compute("Sum([Total_MXN])", "Clasificación=''") == DBNull.Value ? 0 : __datos.Compute("Sum([Total_MXN])", "Clasificación=''"));
                }
                else if (col.ColumnName != "TOTAL" && col.ColumnName != "Sin Clasificacion")
                {
                    row1[co] = Convert.ToDecimal(__datos.Compute("Sum([Stock])", "Clasificación='" + col.ColumnName + "'") == DBNull.Value ? 0 : __datos.Compute("Sum([Stock])", "Clasificación='" + col.ColumnName + "'"));
                    row2[co] = Convert.ToDecimal(__datos.Compute("Sum([Total_MXN])", "Clasificación='" + col.ColumnName + "'") == DBNull.Value ? 0 : __datos.Compute("Sum([Total_MXN])", "Clasificación='" + col.ColumnName + "'"));
                }
                co++;
            }
            tblTClasificacion.Rows.Add(row1);
            tblTClasificacion.Rows.Add(row2);

            dgvTotal2.DataSource = tblTClasificacion;
            dgvTotal2.Rows[0].DefaultCellStyle.Format = "N0";
            dgvTotal2.Rows[1].DefaultCellStyle.Format = "C2";

            //-------------------------------------------------------------------------------------------------------------

            DataTable tblTAccion = new DataTable();
            tblTAccion.Columns.Add("Tipo", typeof(string));
            int index1 = 0; string Columnas1 = "";
            foreach (var item in cboFiltroAccion.Items)
            {
                if (index1 > 1)
                {
                    tblTAccion.Columns.Add(item.ToString(), typeof(decimal));
                    if (Columnas1 != "") { Columnas1 += "+"; Columnas1 += "[" + item.ToString() + "]"; }
                    else
                        Columnas1 += "[" + item.ToString() + "]";
                }
                index1++;
            }

            tblTAccion.Columns.Add("Sin Accion", typeof(decimal));
            if (Columnas1 != "") { Columnas1 += "+" + "[" + "Sin Accion" + "]"; }
            else
                Columnas1 += "[" + "Sin Accion" + "]";

            tblTAccion.Columns.Add("TOTAL", typeof(decimal), Columnas1);
            DataRow row3 = tblTAccion.NewRow();
            DataRow row4 = tblTAccion.NewRow();
            int co1 = 0;
            foreach (DataColumn col in tblTAccion.Columns)
            {
                if (co1 == 0)
                {
                    row3[co1] = "(PZ)"; row4[co1] = "($)";
                }
                else if (col.ColumnName == "Sin Accion")
                {
                    row3[co1] = Convert.ToDecimal(__datos.Compute("Sum([Stock])", "Acción=''") == DBNull.Value ? 0 : __datos.Compute("Sum([Stock])", "Acción=''"));
                    row4[co1] = Convert.ToDecimal(__datos.Compute("Sum([Total_MXN])", "Acción=''") == DBNull.Value ? 0 : __datos.Compute("Sum([Total_MXN])", "Acción=''"));
                }
                else if (col.ColumnName != "TOTAL" && col.ColumnName != "Sin Accion")
                {
                    row3[co1] = Convert.ToDecimal(__datos.Compute("Sum([Stock])", "Acción='" + col.ColumnName + "'") == DBNull.Value ? 0 : __datos.Compute("Sum([Stock])", "Acción='" + col.ColumnName + "'"));
                    row4[co1] = Convert.ToDecimal(__datos.Compute("Sum([Total_MXN])", "Acción='" + col.ColumnName + "'") == DBNull.Value ? 0 : __datos.Compute("Sum([Total_MXN])", "Acción='" + col.ColumnName + "'"));
                }
                co1++;
            }
            tblTAccion.Rows.Add(row3);
            tblTAccion.Rows.Add(row4);

            dgvTotales.DataSource = tblTAccion;
            dgvTotales.Rows[0].DefaultCellStyle.Format = "N0";
            dgvTotales.Rows[1].DefaultCellStyle.Format = "C2";
        }
        
        private void dgvDatos1_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
        {
            try
            {
                int Filtrados = 0; int notFil = 0;
                DataTable dt = Datos1.Copy();
                //dt.Rows.Clear();
                DataTable filt = Datos1.Copy();
                filt.Rows.Clear();
                foreach (UltraGridRow row in this.dgvDatos1.Rows)
                {

                    if (row.IsFilteredOut)
                        Filtrados++;
                    else
                    {
                        string Consulta = "Artículo='" + row.Cells["Artículo"].Value.ToString() + "'";
                        DataRow[] dr = dt.Select(Consulta);

                        if (dr.Count() > 0)
                        {
                            foreach (DataRow item in dr)
                            {
                                DataRow dr0 = filt.NewRow();
                                for (int i = 0; i < filt.Columns.Count; i++)
                                {                                    
                                    dr0[i] = item[i];                                   
                                }                               
                                filt.Rows.Add(dr0);
                            }
                        }
                        notFil++;
                    }
                }
                Filtra(filt);
   

            }
            catch (Exception ex)
            {
                
            }
            
        }
    }
}
