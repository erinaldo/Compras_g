using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.Inventarios.ConteoFisico
{
    public partial class frmPreeliminarConteo : Constantes.frmEmpty
    {
        public frmPreeliminarConteo()
        {
            InitializeComponent();
        }

        private void frmPreeliminarConteo_Load(object sender, EventArgs e)
        {
            try
            {
                this.nuevoToolStripButton.Enabled = false;
                this.guardarToolStripButton.Enabled = false;

                Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                ClasesSGUV.Form.ControlsForms.setDataSource(cbAlmacenes, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesVenta, null, string.Empty), "WhsName", "WhsCode", "---Selecciona un almacén---");
                ClasesSGUV.Form.ControlsForms.setDataSource(clbLinea, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Lineas, null, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "---Todas---");

                clbLinea.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
                clbLinea.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cbAlmacenes.SelectedIndex == 0)
            {
                this.SetMensaje("Selecciona un almacén", 5000, Color.Red, Color.White);
                return;
            }

            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    string f = ClasesSGUV.Form.ControlsForms.getCadena(clbLinea, string.Empty, false, "ItmsGrpCod");
                    //command.Parameters.AddWithValue("@TipoConsulta", 4);
                    //command.Parameters.AddWithValue("@WhsCode", cbAlmacenes.SelectedValue);
                    //command.Parameters.AddWithValue("@Lineas", ClasesSGUV.Form.ControlsForms.getCadena(clbLinea, string.Empty, false, "ItmsGrpCod"));
                    
                    //if (rbLinea.Checked)
                    //    command.Parameters.AddWithValue("@Orden", "01");
                    //if (rbUbicacion.Checked)
                    //    command.Parameters.AddWithValue("@Orden", "02");


                    DataTable tbl = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(tbl);

                    dgvDatos.DataSource = tbl;
                }
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[1].Width = 300;

            e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[2].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[3].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

        }
    }
}
