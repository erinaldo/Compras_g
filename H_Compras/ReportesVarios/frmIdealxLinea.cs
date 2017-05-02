using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Sql;

namespace H_Compras.ReportesVarios
{
    public partial class frmIdealxLinea : Form
    {
        ClasesSGUV.Logs log;

        Datos.Connection connection = new Datos.Connection();
        Object[] valuesOut = new Object[] { };
        public frmIdealxLinea()
        {
            InitializeComponent();
        }

        private enum Columnas_Lineas
        {
            ItmsgrpCod,
            Linea,
            Stock,
            Ideal,
            Diferencia,
            Solicitado,

            StockM,
            IdealM,
            DiferenciaM,
            SolicitadoM,

            VI
        }

        private enum Columnas_Articulo
        {
            Proveedor,
            Articulo,
            Descripcion,
            ABC,
            Stock,
            Ideal,
            Diferencia,
            Solicitado,

            StockM,
            IdealM,
            DiferenciaM,
            SolicitadoM,

            VI,
            DiasStock,
            Fecha
        }

        private enum Columnas_Almacen
        {
            Articulo,
            WhsCode,
            Almacen,
            Venta_M,
            Venta_P,
            Stock,
            Solicitado,
            Ideal
        }

        private enum Columas_Cliente
        {
            Cliente,
            Nombre,
            Venta_M,
            Venta_P
        }

        private enum Columnas_Ventas
        {
            Num,
            Año,
            Articulo,
            Mes,
            Venta,
            Pz
        }

        public void FormatoVentas(DataGridView dgv)
        {
            dgv.Columns[(int)Columnas_Ventas.Num].Visible = false;
            dgv.Columns[(int)Columnas_Ventas.Año].Width = 80;
            dgv.Columns[(int)Columnas_Ventas.Venta].Width = 85;
            dgv.Columns[(int)Columnas_Ventas.Pz].Width = 85;

            dgv.Columns[(int)Columnas_Ventas.Venta].DefaultCellStyle.Format = "C2";
            dgv.Columns[(int)Columnas_Ventas.Pz].DefaultCellStyle.Format = "N0";

            dgv.Columns[(int)Columnas_Ventas.Venta].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Ventas.Pz].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        public void FormatoLineas(DataGridView dgv)
        {
            dgv.Columns[(int)Columnas_Lineas.ItmsgrpCod].Visible = false;

            dgv.Columns[(int)Columnas_Lineas.Stock].Width = 70;
            dgv.Columns[(int)Columnas_Lineas.Ideal].Width = 70;
            dgv.Columns[(int)Columnas_Lineas.Diferencia].Width = 70;
            dgv.Columns[(int)Columnas_Lineas.StockM].Width = 70;
            dgv.Columns[(int)Columnas_Lineas.IdealM].Width = 70;
            dgv.Columns[(int)Columnas_Lineas.DiferenciaM].Width = 70;
            dgv.Columns[(int)Columnas_Lineas.Solicitado].Visible = false;
            dgv.Columns[(int)Columnas_Lineas.SolicitadoM].Visible = false;
            dgv.Columns[(int)Columnas_Lineas.VI].Width = 60;

            dgv.Columns[(int)Columnas_Lineas.Stock].DefaultCellStyle.Format = "N0";
            dgv.Columns[(int)Columnas_Lineas.Solicitado].DefaultCellStyle.Format = "N0";
            dgv.Columns[(int)Columnas_Lineas.Ideal].DefaultCellStyle.Format = "N0";
            dgv.Columns[(int)Columnas_Lineas.Diferencia].DefaultCellStyle.Format = "N0";
            dgv.Columns[(int)Columnas_Lineas.StockM].DefaultCellStyle.Format = "C0";
            dgv.Columns[(int)Columnas_Lineas.SolicitadoM].DefaultCellStyle.Format = "C0";
            dgv.Columns[(int)Columnas_Lineas.IdealM].DefaultCellStyle.Format = "C0";
            dgv.Columns[(int)Columnas_Lineas.DiferenciaM].DefaultCellStyle.Format = "C0";
            dgv.Columns[(int)Columnas_Lineas.VI].DefaultCellStyle.Format = "N2";

            dgv.Columns[(int)Columnas_Lineas.Stock].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Lineas.Ideal].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Lineas.Diferencia].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Lineas.StockM].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Lineas.IdealM].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Lineas.DiferenciaM].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Lineas.Solicitado].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Lineas.SolicitadoM].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Lineas.VI].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        public void FormatoArticulos(DataGridView dgv)
        {
            dgv.Columns[(int)Columnas_Articulo.Proveedor].Width = 120;
            dgv.Columns[(int)Columnas_Articulo.Stock].Width = 70;
            dgv.Columns[(int)Columnas_Articulo.ABC].Width = 50;
            dgv.Columns[(int)Columnas_Articulo.Descripcion].Width = 120;
            dgv.Columns[(int)Columnas_Articulo.Ideal].Width = 70;
            dgv.Columns[(int)Columnas_Articulo.Diferencia].Width = 70;
            dgv.Columns[(int)Columnas_Articulo.Solicitado].Width = 70;
            dgv.Columns[(int)Columnas_Articulo.SolicitadoM].Width = 70;
            dgv.Columns[(int)Columnas_Articulo.VI].Width = 70;

            dgv.Columns[(int)Columnas_Articulo.StockM].Width = 70;
            dgv.Columns[(int)Columnas_Articulo.IdealM].Width = 70;
            dgv.Columns[(int)Columnas_Articulo.DiferenciaM].Width = 70;

            dgv.Columns[(int)Columnas_Articulo.Stock].DefaultCellStyle.Format = "N0";
            dgv.Columns[(int)Columnas_Articulo.Ideal].DefaultCellStyle.Format = "N0";
            dgv.Columns[(int)Columnas_Articulo.Solicitado].DefaultCellStyle.Format = "N0";
            dgv.Columns[(int)Columnas_Articulo.Diferencia].DefaultCellStyle.Format = "N0";
            dgv.Columns[(int)Columnas_Articulo.StockM].DefaultCellStyle.Format = "C0";
            dgv.Columns[(int)Columnas_Articulo.IdealM].DefaultCellStyle.Format = "C0";
            dgv.Columns[(int)Columnas_Articulo.DiferenciaM].DefaultCellStyle.Format = "C0";
            dgv.Columns[(int)Columnas_Articulo.SolicitadoM].DefaultCellStyle.Format = "C0";

            dgv.Columns[(int)Columnas_Articulo.VI].DefaultCellStyle.Format = "N2";

            dgv.Columns[(int)Columnas_Articulo.Stock].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Articulo.Ideal].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Articulo.Solicitado].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Articulo.Diferencia].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Articulo.StockM].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Articulo.IdealM].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Articulo.DiferenciaM].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        public void FormatoAlmacenes(DataGridView dgv)
        {
            dgv.Columns[(int)Columnas_Almacen.WhsCode].Visible = false;

            dgv.Columns[(int)Columnas_Almacen.Stock].Width = 75;
            dgv.Columns[(int)Columnas_Almacen.Ideal].Width = 75;
            dgv.Columns[(int)Columnas_Almacen.Venta_P].Width = 75;
            dgv.Columns[(int)Columnas_Almacen.Venta_M].Width = 75;

            dgv.Columns[(int)Columnas_Almacen.Venta_M].DefaultCellStyle.Format = "C0";
            dgv.Columns[(int)Columnas_Almacen.Venta_P].DefaultCellStyle.Format = "N1";
            dgv.Columns[(int)Columnas_Almacen.Stock].DefaultCellStyle.Format = "N0";
            dgv.Columns[(int)Columnas_Almacen.Solicitado].DefaultCellStyle.Format = "N0";
            dgv.Columns[(int)Columnas_Almacen.Ideal].DefaultCellStyle.Format = "N0";

            dgv.Columns[(int)Columnas_Almacen.Venta_M].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Almacen.Stock].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Almacen.Solicitado].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Almacen.Ideal].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columnas_Almacen.Venta_P].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        public void FormatoClientes(DataGridView dgv)
        {
            dgv.Columns[(int)Columas_Cliente.Nombre].Width = 200;

            dgv.Columns[(int)Columas_Cliente.Venta_M].DefaultCellStyle.Format = "C0";
            dgv.Columns[(int)Columas_Cliente.Venta_P].DefaultCellStyle.Format = "N0";

            dgv.Columns[(int)Columas_Cliente.Venta_M].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[(int)Columas_Cliente.Venta_P].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void IdealxLinea_Load(object sender, EventArgs e)
        {
            this.Icon = ClasesSGUV.Propiedades.IconHalcoNET;

            log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

            this.CargarLinea(clbLinea, "Todas");
            this.CargarProveedores(clbProveedor, "Todas");
            this.CargarCompradores(clbComprador, "Todos");

            DataTable tbl_almacenes = new DataTable();
            tbl_almacenes = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.Tipos.AlmacenesVenta);

            DataRow row = tbl_almacenes.NewRow();
            row["WhsCode"] = "0";
            row["WhsName"] = "Todos";
            tbl_almacenes.Rows.InsertAt(row, 0);

            clbAlmacen.DataSource = tbl_almacenes;
            clbAlmacen.DisplayMember = "WhsName";
            clbAlmacen.ValueMember = "WhsCode";

            clbAlmacen.Click -= new EventHandler(ClasesSGUV.Forms.clb_Click);
            clbAlmacen.Click += new EventHandler(ClasesSGUV.Forms.clb_Click);

            cbLP.SelectedIndex = 1;
            cbClasificacion.SelectedIndex = 0;

            DataTable tbl_Acciones = new DataTable();

            tbl_Acciones = connection.GetDataTable("SGUV",
                                                    "PJ_IdealLinea",
                                                    new string[] { },
                                                    new string[] { "@TipoConsulta"},
                                                    ref valuesOut, 7);
         
            udpAcciones.SetDataBinding(tbl_Acciones, null);
            udpAcciones.ValueMember = "Name";
            udpAcciones.DisplayMember = "Name";
        }

        public void CargarLinea(CheckedListBox _cb, string _inicio)
        {
            SqlCommand command = new SqlCommand("PJ_Compras", new SqlConnection(ClasesSGUV.Propiedades.conectionPJ));
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@TipoConsulta", 2);
            command.Parameters.AddWithValue("@Articulo", string.Empty);
            command.Parameters.AddWithValue("@Linea", 0);
            command.Parameters.AddWithValue("@AlmacenDestino", string.Empty);
            command.Parameters.AddWithValue("@AlmacenOrigen", string.Empty);
            command.Parameters.AddWithValue("@Proveedor", 0);

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

        public void CargarProveedores(CheckedListBox _cb, string _inicio)
        {

            DataTable table = new DataTable();
            table = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.Tipos.TodosProveedores);

            DataRow row = table.NewRow();
            row["Nombre"] = _inicio;
            row["Codigo"] = "0";
            table.Rows.InsertAt(row, 0);

            _cb.DataSource = table;
            _cb.DisplayMember = "Nombre";
            _cb.ValueMember = "Codigo";
        }

        public void CargarCompradores(CheckedListBox _cb, string _inicio)
        {
            try
            {
                SqlCommand command = new SqlCommand("sp_Importacion", new SqlConnection(ClasesSGUV.Propiedades.conectionSGUV));
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TipoConsulta", 1);

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

                _cb.SelectedValue = 0;
            }
            catch (Exception)
            {
            }


        }

        private void clbSucursal_Click(object sender, EventArgs e)
        {
            if ((sender as CheckedListBox).SelectedIndex == 0)
            {
                if ((sender as CheckedListBox).CheckedIndices.Contains(0))
                {
                    for (int item = 1; item < (sender as CheckedListBox).Items.Count; item++)
                    {
                        (sender as CheckedListBox).SetItemChecked(item, false);
                    }
                }
                else
                {
                    for (int item = 1; item < (sender as CheckedListBox).Items.Count; item++)
                    {
                        (sender as CheckedListBox).SetItemChecked(item, true);
                    }
                }
            }

        }

        string Lineas;
        string Proveedores;
        string Compradores;
        string Almacenes;
        int cont = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Lineas = GetCadena(clbLinea, string.Empty).Trim(',');
                Proveedores = GetCadena(clbProveedor, "'").Trim(',');
                Compradores = GetCadena(clbComprador, "'").Trim(',');
                Almacenes = ClasesSGUV.Forms.GetCadena(clbAlmacen, "'", "WhsCode", false);
                cont = Almacenes.Split(',').Count();

                SqlCommand command = new SqlCommand("PJ_IdealLinea", new SqlConnection(ClasesSGUV.Propiedades.conectionSGUV));
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TipoConsulta", 1);
                command.Parameters.AddWithValue("@Lineas", Lineas);
                command.Parameters.AddWithValue("@Proveedores", Proveedores);
                command.Parameters.AddWithValue("@Compradores", Compradores);
                command.Parameters.AddWithValue("@Almacenes", Almacenes);
                command.Parameters.AddWithValue("@LP", cbLP.Text.Split('-')[0].Trim());
                if (cbClasificacion.SelectedIndex > 0)
                    command.Parameters.AddWithValue("@Propiedad", cbClasificacion.Text.Split('-')[0].Trim());
                command.Parameters.AddWithValue("@SinIdeal", cbSinIdeal.Checked);
                command.Parameters.AddWithValue("@Count", cont);
                command.Parameters.AddWithValue("@ConsiderarSol", cbConsideraSolicitado.Checked);


                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                adapter.Fill(table);

                dgvLineas.DataSource = table;

                DataTable _t = new DataTable();
                _t.Columns.Add("Stock", typeof(decimal));
                _t.Columns.Add("Ideal", typeof(decimal));
                _t.Columns.Add("Solicitado", typeof(decimal));
                _t.Columns.Add("Diferencia", typeof(decimal));

                if (table.Columns.Count > 0)
                {
                    DataRow row = _t.NewRow();
                    row["Stock"] = table.Compute("Sum([Stock ($)])", "");
                    row["Ideal"] = table.Compute("Sum([Ideal ($)])", "");
                    row["Solicitado"] = table.Compute("Sum([Solicitado ($)])", "");
                    row["Diferencia"] = Convert.ToDecimal(table.Compute("Sum([Diferencia ($)])", ""));

                    _t.Rows.Add(row);
                }
                dgvTotales.DataSource = _t;

                this.FormatoLineas(dgvLineas);

                dgvTotales.Columns[0].Width = 90;
                dgvTotales.Columns[1].Width = 90;
                dgvTotales.Columns[2].Width = 90;
                dgvTotales.Columns[3].Width = 90;

                dgvTotales.Columns[0].DefaultCellStyle.Format = "C0";
                dgvTotales.Columns[1].DefaultCellStyle.Format = "C0";
                dgvTotales.Columns[2].DefaultCellStyle.Format = "C0";
                dgvTotales.Columns[3].DefaultCellStyle.Format = "C0";

                dgvTotales.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvTotales.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvTotales.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvTotales.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string GetCadena(CheckedListBox cb, string _char)
        {
            StringBuilder stb = new StringBuilder();
            foreach (DataRowView item in cb.CheckedItems)
            {
                if (item["Codigo"].ToString() != "0")
                {
                    if (!cb.ToString().Equals(string.Empty))
                    {
                        stb.Append(",");
                    }
                    stb.Append(_char + item["Codigo"] + _char);
                }
            }
            if (cb.CheckedItems.Count == 0)
            {
                foreach (DataRowView item in cb.Items)
                {
                    if (item["Codigo"].ToString() != "0")
                    {
                        if (!cb.ToString().Equals(string.Empty))
                        {
                            stb.Append(",");
                        }
                        stb.Append(_char + item["Codigo"] + _char);
                    }
                }
                stb.Append(",''");
            }
            return stb.ToString();
        }

        public void Formato(string _formato, string _opcion)
        {
            dgvTotales.Columns[0].Width = 100;
            dgvTotales.Columns[1].Width = 100;
            dgvTotales.Columns[2].Width = 100;

            dgvTotales.Columns[0].DefaultCellStyle.Format = _formato;
            dgvTotales.Columns[1].DefaultCellStyle.Format = _formato;
            dgvTotales.Columns[2].DefaultCellStyle.Format = _formato;

            dgvTotales.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvTotales.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvTotales.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvTotales.Columns[0].HeaderText = "Stock (" + _opcion + ")";
            dgvTotales.Columns[1].HeaderText = "Ideal (" + _opcion + ")";
            dgvTotales.Columns[2].HeaderText = "Diferencia (" + _opcion + ")";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            rbPiezas.Checked = true;

            dgvLineas.DataSource = null;
            dgvTotales.DataSource = null;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ClasesSGUV.Exportar exp = new ClasesSGUV.Exportar();
            exp.ExportarDatos(dgvLineas, false);
        }

        private void IdealxLinea_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void IdealxLinea_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                log.Fin();
            }
            catch (Exception)
            {

            }
        }

        private void gridExceso_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dgvArticulos.DataSource = null;
                dgvAlmacenes.DataSource = null;
                dgvCliente.DataSource = null;

                SqlCommand command = new SqlCommand("PJ_IdealLinea", new SqlConnection(ClasesSGUV.Propiedades.conectionSGUV));
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TipoConsulta", 2);
                command.Parameters.AddWithValue("@Lineas", (sender as DataGridView).Rows[e.RowIndex].Cells[0].Value);
                command.Parameters.AddWithValue("@Proveedores", Proveedores);
                command.Parameters.AddWithValue("@Almacenes", Almacenes);
                command.Parameters.AddWithValue("@LP", cbLP.Text.Split('-')[0].Trim());
                command.Parameters.AddWithValue("@SinIdeal", cbSinIdeal.Checked);
                command.Parameters.AddWithValue("@Count", cont);
                if (cbClasificacion.SelectedIndex > 0)
                    command.Parameters.AddWithValue("@Propiedad", cbClasificacion.Text.Split('-')[0].Trim());
                
                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                adapter.Fill(table);

                dgvArticulos.DataSource = table;
                dgvItem.DataSource = table;

                this.FormatoArticulos(dgvArticulos);

            }
            catch (Exception)
            {

            }
        }

        string articulo = string.Empty;
        private void dgvArticulos_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        string almacen = string.Empty;
        private void dgvAlmacenes_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                almacen = (sender as DataGridView).Rows[e.RowIndex].Cells[1].Value.ToString();
                SqlCommand command = new SqlCommand("PJ_IdealLinea", new SqlConnection(ClasesSGUV.Propiedades.conectionSGUV));
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TipoConsulta", 5);
                command.Parameters.AddWithValue("@Articulo", (sender as DataGridView).Rows[e.RowIndex].Cells[0].Value);
                command.Parameters.AddWithValue("@Almacen", (sender as DataGridView).Rows[e.RowIndex].Cells[1].Value);
                command.Parameters.AddWithValue("@Proveedores", Proveedores);
                command.Parameters.AddWithValue("@Almacenes", Almacenes);

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                adapter.Fill(table);

                dgvVentas.DataSource = table;

                this.FormatoVentas(dgvVentas);
            }
            catch (Exception)
            {
            }
           
        }

        private void dgvVentas_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                SqlCommand command = new SqlCommand("PJ_IdealLinea", new SqlConnection(ClasesSGUV.Propiedades.conectionSGUV));
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TipoConsulta", 4);
                command.Parameters.AddWithValue("@Mes", (sender as DataGridView).Rows[e.RowIndex].Cells[0].Value);
                command.Parameters.AddWithValue("@Año", (sender as DataGridView).Rows[e.RowIndex].Cells[3].Value);
                command.Parameters.AddWithValue("@Articulo", (sender as DataGridView).Rows[e.RowIndex].Cells[1].Value);
                command.Parameters.AddWithValue("@Almacen", almacen);
                command.Parameters.AddWithValue("@Proveedores", Proveedores);
                command.Parameters.AddWithValue("@Almacenes", Almacenes);


                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                adapter.Fill(table);

                dgvCliente.DataSource = table;

                this.FormatoClientes(dgvCliente);
            }
            catch (Exception)
            {

            }
        }

        private void dgvArticulos_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow item in dgvArticulos.Rows)
            {
                if (Convert.ToInt32(item.Cells[(int)Columnas_Articulo.Fecha].Value) <= 4)
                {
                    item.Cells[(int)Columnas_Articulo.Articulo].Style.BackColor = Color.Red;
                    item.Cells[(int)Columnas_Articulo.Articulo].Style.ForeColor = Color.White;
                }
            }
        }

        private void dgvItem_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Proveedor].Width = 120;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Stock].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.ABC].Width = 50;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Descripcion].Width = 120;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Ideal].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Diferencia].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Solicitado].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.SolicitadoM].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.VI].Width = 70;

            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.StockM].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.IdealM].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.DiferenciaM].Width = 70;

            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Stock].Format = "N0";
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Ideal].Format = "N0";
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Solicitado].Format = "N0";
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Diferencia].Format = "N0";
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.StockM].Format = "C0";
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.IdealM].Format = "C0";
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.DiferenciaM].Format = "C0";
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.SolicitadoM].Format = "C0";

            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.VI].Format = "N2";

            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Stock].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Ideal].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Solicitado].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Diferencia].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.StockM].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.IdealM].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.DiferenciaM].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas_Articulo.Fecha].Hidden = true;

            e.Layout.Bands[0].Columns["Accion"].ValueList = udpAcciones;

            e.Layout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy;

            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }

            e.Layout.Bands[0].Columns["Accion"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {

                foreach (var item in dgvItem.DisplayLayout.Rows)
                {
                      int x  = connection.Ejecutar("SGUV",
                                                   "PJ_IdealLinea",
                                                   new string[] { },
                                                   new string[] { "@TipoConsulta", "@Accion", "@Articulo", "@Comentario" },
                                                   ref valuesOut, 6, item.Cells["Accion"].Value, item.Cells[(int)Columnas_Articulo.Articulo].Value, item.Cells["Comentarios"].Value);
                }
            }
            catch (Exception)
            {
                
            }
        }

        private void udpAcciones_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = 150;
            e.Layout.Bands[0].HeaderVisible = false;
        }

        private void dgvItem_AfterRowActivate(object sender, EventArgs e)
        {
            try
            {
                articulo = dgvItem.ActiveRow.Cells["Artículo"].Value.ToString();// (sender as DataGridView).Rows[e.RowIndex].Cells[0].Value.ToString();
                SqlCommand command = new SqlCommand("PJ_IdealLinea", new SqlConnection(ClasesSGUV.Propiedades.conectionSGUV));
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TipoConsulta", 3);
                command.Parameters.AddWithValue("@Articulo",articulo);// (sender as DataGridView).Rows[e.RowIndex].Cells[(int)Columnas_Articulo.Articulo].Value);
                command.Parameters.AddWithValue("@Proveedores", Proveedores);
                command.Parameters.AddWithValue("@Almacenes", Almacenes);

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                adapter.Fill(table);

                dgvAlmacenes.DataSource = table;

                this.FormatoAlmacenes(dgvAlmacenes);
            }
            catch (Exception)
            {
            }
        }
    }
}

