using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.ReportesVarios
{
    public partial class frmQryVarios : Constantes.frmEmpty
    {
        string NameRpt = string.Empty;

        public frmQryVarios()
        {
            InitializeComponent();
        }
        public frmQryVarios(string _name)
        {
            NameRpt = _name;
            InitializeComponent();
        }

        private void frmQryVarios_Load(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(NameRpt))
                {
                    DataTable tbl = new DataTable();
                    textBox1.Enabled = false;
                    tbl = ClasesSGUV.DataSource.GetQry(NameRpt);

                    textBox1.Text = tbl.Rows[0].Field<string>("qry");
                    this.Text = tbl.Rows[0].Field<string>("frmText"); 

                    button1_Click(sender, e);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand(textBox1.Text, connection))
                {
                    DataTable table = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(table);

                    dgvDatos.DataSource = table;
                }
            }
            
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                column.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
        }

    }
}
