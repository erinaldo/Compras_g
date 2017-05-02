using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace H_Compras.ReportesVarios
{
    public partial class frmTraspasos : Constantes.frmEmpty
    {
        public string FechaInicial;
        public string FechaFinal;
        public SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog);
       
        public frmTraspasos()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try{
            gridTrapasos.Columns.Clear();
            gridTrapasos.DataSource = null;
                
            FechaInicial = dtpDesde.Value.ToString("yyyy-MM-dd");
            FechaFinal = dtpHasta.Value.ToString("yyyy-MM-dd");
            string notraspasoCadena = txtNoTraspaso.Text.ToString();
            int noTraspaso ;
            DataSet data = new DataSet();
            BindingSource masterBindingSource = new BindingSource();

            SqlCommand cmd = new SqlCommand("sp_Inventario", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            if (notraspasoCadena.Trim() != "")
            {
                noTraspaso = Convert.ToInt32(notraspasoCadena);
                cmd.Parameters.AddWithValue("@TipoConsulta", 6);
                cmd.Parameters.AddWithValue("@NoTraspaso", noTraspaso);
            }
            else
            {
                cmd.Parameters.AddWithValue("@TipoConsulta", 7);
                cmd.Parameters.AddWithValue("@FechaDesde", FechaInicial);
                cmd.Parameters.AddWithValue("@FechaHasta", FechaFinal);
            }

            cmd.CommandTimeout = 0;
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.SelectCommand.CommandTimeout = 0;
            adapter.Fill(data, "TablaDetalle");

            gridTrapasos.DataSource = data.Tables[0];

            gridTrapasos.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridTrapasos.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridTrapasos.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridTrapasos.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridTrapasos.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            gridTrapasos.Columns[6].DefaultCellStyle.Format = "N0";
            gridTrapasos.Columns[7].DefaultCellStyle.Format = "N0";
            gridTrapasos.Columns[8].DefaultCellStyle.Format = "N2";

            gridTrapasos.Columns[0].Width = 80;
            gridTrapasos.Columns[6].Width = 80;
            gridTrapasos.Columns[7].Width = 80;
            gridTrapasos.Columns[8].Width = 80;
            gridTrapasos.Columns[2].Width = 50;
            gridTrapasos.Columns[3].Width = 50;
            gridTrapasos.Columns[5].Width = 230;
            gridTrapasos.Columns[9].Width = 150;

            decimal total = (from acum in data.Tables["TablaDetalle"].AsEnumerable()
                                select acum.Field<decimal>("Total")).Sum();
            decimal totalPeso = (from acum in data.Tables["TablaDetalle"].AsEnumerable()
                             select acum.Field<decimal>("Peso")).Sum();
            txtTotal.Text = total.ToString("N0");
            txtTotalPeso.Text = totalPeso.ToString("N2")+" Kg.";
            //recorrerGrid();
            }
            catch(Exception ex){}
        }

        private void Traspasos_Load(object sender, EventArgs e)
        {
            this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);
        }
    }
}
