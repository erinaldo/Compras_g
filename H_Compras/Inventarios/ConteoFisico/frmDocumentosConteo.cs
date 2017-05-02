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
    public partial class frmDocumentosConteo : Constantes.frmEmpty
    {
        public frmDocumentosConteo()
        {
            InitializeComponent();
        }

        private void frmDocumentosConteo_Load(object sender, EventArgs e)
        {
            try
            {
                btnUpdate_Click(sender, e);

                this.actualizarToolStripButton.Click -= new EventHandler(btnUpdate_Click);
                this.actualizarToolStripButton.Click += new EventHandler(btnUpdate_Click);

                this.nuevoToolStripButton.Click -= new EventHandler(btnNuevo_Click);
                this.nuevoToolStripButton.Click += new EventHandler(btnNuevo_Click);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("su_Conteo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 6);
                        command.Parameters.AddWithValue("@Rol", ClasesSGUV.Login.Rol);
                        command.Parameters.AddWithValue("@UserID", ClasesSGUV.Login.Id_Usuario);

                        SqlDataAdapter da = new SqlDataAdapter(command);
                        DataTable table = new DataTable();
                        da.Fill(table);

                        dgvDatos.DataSource = table;
                    }
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["Ver"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            e.Layout.Bands[0].Columns["Ver"].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            e.Layout.Bands[0].Columns["Ver"].CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

            e.Layout.Bands[0].Columns["WhsCode"].Hidden = true;
            e.Layout.Bands[0].Columns["Autor"].Width = 130;
            e.Layout.Bands[0].Columns["Almacén"].Width = 130;

            e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[2].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[3].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[4].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[5].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
        }

        private void dgvDatos_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            e.Row.Cells["Ver"].Value = "Ver";
            e.Row.Cells["Ver"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
        }

        private void dgvDatos_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            Inventarios.Conteos.frmConteoFisico formulario 
                    = new  Conteos.frmConteoFisico(Convert.ToInt32(dgvDatos.ActiveRow.Cells["Folio"].Value));
            formulario.MdiParent = this.MdiParent;
            formulario.Show();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Inventarios.Conteos.frmConteoFisico formulario
                    = new  Conteos.frmConteoFisico(0);
            formulario.MdiParent = this.MdiParent;
            formulario.Show();
        }
    }
}
