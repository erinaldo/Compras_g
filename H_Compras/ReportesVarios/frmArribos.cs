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

namespace H_Compras.ReportesVarios
{
    public partial class frmArribos : Form
    {
        private int Vendedor;
        string Memo;
        ClasesSGUV.Logs log;

        Datos.Connection connection = new Datos.Connection();
        Object[] valuesOut = new Object[] { };

        DataTable Tbl_Datos = new DataTable();

        public frmArribos(int _slpcode)
        {
            InitializeComponent();
            log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription,0);
            Vendedor = _slpcode;
        }

        private string GetMemo()
        {
            string qry = "select Memo from OSLP Where SlpCode  = @Vendedor";
            try
            {
                using (SqlConnection conn = new SqlConnection(ClasesSGUV.Propiedades.conectionPJ))
                {
                    using (SqlCommand command = new SqlCommand(qry, conn))
                    {
                        conn.Open();

                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@Vendedor", Vendedor);

                        string memo = Convert.ToString(command.ExecuteScalar());
                        return memo;
                    }

                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public DataTable ListaArticulos()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ClasesSGUV.Propiedades.conectionSGUV;
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "PJ_ReparticionStock";

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 1);
                    command.Parameters.AddWithValue("@Articulo", string.Empty);
                    command.Parameters.AddWithValue("@CantiadOK", decimal.Zero);
                    command.Parameters.AddWithValue("@Incremento", decimal.Zero);

                    command.CommandTimeout = 0;

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = command;
                    adapter.SelectCommand.CommandTimeout = 0;
                    adapter.Fill(Articul);

                    return Articul;
                }
            }
        }

        public static AutoCompleteStringCollection Autocomplete(DataTable _t, string _column)
        {
            DataTable dt = _t;

            AutoCompleteStringCollection coleccion = new AutoCompleteStringCollection();

            foreach (DataRow row in dt.Rows)
            {
                coleccion.Add(Convert.ToString(row[_column]));
            }

            return coleccion;
        }
        DataTable Articul = new DataTable();
        private void Arribos_Load(object sender, EventArgs e)
        {
            this.Icon = ClasesSGUV.Propiedades.IconHalcoNET;

            Memo = GetMemo();

            SqlCommand command = new SqlCommand("PJ_NotificacionStock", new SqlConnection(ClasesSGUV.Propiedades.conectionSGUV));
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@TipoConsulta", 5);
            command.Parameters.AddWithValue("@Comprador", string.Empty);
            command.Parameters.AddWithValue("@Linea", string.Empty);
            command.Parameters.AddWithValue("@Almacen", Memo);
            command.Parameters.AddWithValue("@Rol", ClasesSGUV.Login.Rol);

            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(table);

            Tbl_Datos = table.Copy();

            this.ListaArticulos();

            txtArticulo.AutoCompleteCustomSource = Autocomplete(Articul, "ItemCode");
            txtArticulo.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtArticulo.AutoCompleteSource = AutoCompleteSource.CustomSource;

            DataTable tbl = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesTodos, string.Empty, string.Empty);

            ultraDropDown1.SetDataBinding(tbl, null);
            ultraDropDown1.ValueMember = "WhsCode";
            ultraDropDown1.DisplayMember = "WhsName";
        }

        private void Arribos_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();   
            }
            catch (Exception)
            {
            }
        }

        private void Arribos_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                log.Fin();
            }
            catch (Exception)
            {
            }
        }

        private void txtArticulo_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    var qry = from item in Datos.AsEnumerable()
            //              where item.Field<string>("Artículo").ToLower().Contains(txtArticulo.Text.ToLower())
            //              select item;

            //    dgvStock.DataSource = qry.CopyToDataTable();
            //}
            //catch (Exception)
            //{
            //    dgvStock.DataSource = null;
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            toolMsj.BackColor = Color.FromName("Control");
            toolMsj.ForeColor = Color.Black;

            toolMsj.Text = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(txtArticulo.Text))
                {
                    var qry = from item in Tbl_Datos.AsEnumerable()
                              where item.Field<string>("Artículo").ToLower().Equals(txtArticulo.Text.ToLower())
                              select item;

                    dgvStock.DataSource = qry.CopyToDataTable();

                    dgvDatos.DataSource = qry.CopyToDataTable();
                }
                else
                {
                    var qry = from item in Tbl_Datos.AsEnumerable()
                              //where item.Field<string>("Artículo").ToLower().Equals(txtArticulo.Text.ToLower())
                              select item;

                    dgvStock.DataSource = qry.CopyToDataTable();
                    
                    dgvDatos.DataSource = qry.CopyToDataTable();
                }
            }
            catch (Exception)
            {
                toolMsj.Text = "No hay pedidos recientes para este artículo.";
                toolMsj.BackColor = Color.Red;
                toolMsj.ForeColor = Color.White;

                dgvStock.DataSource = null;
            }
        }

        private void txtArticulo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void dgvStock_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (ClasesSGUV.Login.Rol == (int)ClasesSGUV.Propiedades.RolesHalcoNET.Administrador
                    | ClasesSGUV.Login.Rol == (int)ClasesSGUV.Propiedades.RolesHalcoNET.GerenteCompras
                    | ClasesSGUV.Login.Rol == (int)ClasesSGUV.Propiedades.RolesHalcoNET.Zulma)
                {
                    Int32 docEntry = Convert.ToInt32(dgvStock.Rows[e.RowIndex].Cells[0].Value);

                    SDK.Documentos.frmDocumentos formulario = new SDK.Documentos.frmDocumentos(1);
                    formulario.typeDocument = "OC";
                    formulario.MdiParent = this.MdiParent;
                    formulario.txtFolio.Text = docEntry.ToString();
                    var kea = new KeyPressEventArgs(Convert.ToChar(13));
                    formulario.txtFolio_KeyPress(sender, kea);
                    formulario.Show();
                }
            }
            catch (Exception)
            {

            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
             Infragistics.Win.UltraWinGrid.UltraGridLayout layout = e.Layout;
            Infragistics.Win.UltraWinGrid.UltraGridBand band = layout.Bands[0];
            e.Layout.Bands[0].Columns["Lugar de arribo"].ValueList = ultraDropDown1;
            
            foreach (var item in band.Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }

            band.Columns["Fecha de arribo"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
            band.Columns["Lugar de arribo"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
                
            band.Columns["DocEntry"].Hidden = true;
            band.Columns["LineNum"].Hidden = true;
        }

        private void dgvDatos_AfterRowUpdate(object sender, Infragistics.Win.UltraWinGrid.RowEventArgs e)
        {
            try
            {
                connection.Ejecutar("LOG",
                             "sp_Reportes",
                             new string[] { },
                             new string[] { "@TipoConsulta", "@Fecha", "@WhsCode", "@DocEntry", "@LineNum", "@tipoDoc"},
                             ref valuesOut, 17, dgvDatos.ActiveRow.Cells["Fecha de arribo"].Value, dgvDatos.ActiveRow.Cells["Lugar de arribo"].Value, dgvDatos.ActiveRow.Cells["DocEntry"].Value, dgvDatos.ActiveRow.Cells["LineNum"].Value, dgvDatos.ActiveRow.Cells["Tipo"].Value);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
