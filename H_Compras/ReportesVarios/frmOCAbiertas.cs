using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.ReportesVarios
{
    public partial class frmOCAbiertas : Constantes.frmEmpty
    {
        public frmOCAbiertas()
        {
            InitializeComponent();
        }

        public enum Columnas
        {
            Docnum, DocDate, CardCode, CardName, DocCur, DocTotal, Planeador
        }

        private void frmOCAbiertas_Load(object sender, EventArgs e)
        {
            try
            {
                this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                dgvDatos.DataSource = null;
                ClasesSGUV.Form.ControlsForms.setDataSource(clbCompradores, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.PlaneadoresCompra, null, string.Empty), "U_VLGX_PLC", "U_VLGX_PLC", "Seleccionar todo");

                nuevoToolStripButton.Click -= new EventHandler(btnNuevo_Click);
                nuevoToolStripButton.Click += new EventHandler(btnNuevo_Click);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("sp_Reportes", connection))
                    {
                        string x = ClasesSGUV.Form.ControlsForms.getCadena(clbCompradores, "'", false, "U_VLGX_PLC");
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 1);
                        command.Parameters.AddWithValue("@Compradores", x);

                        System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter();
                        da.SelectCommand = command;
                        DataTable table = new DataTable();
                        da.Fill(table);

                        dgvDatos.DataSource = table;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy;

                e.Layout.Bands[0].Columns[(int)Columnas.Docnum].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas.CardCode].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas.CardName].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas.DocCur].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas.DocTotal].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas.DocDate].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[(int)Columnas.Planeador].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

                e.Layout.Bands[0].Columns[(int)Columnas.Docnum].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas.DocCur].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas.CardCode].Width = 80;
                e.Layout.Bands[0].Columns[(int)Columnas.CardName].Width = 250;
                e.Layout.Bands[0].Columns[(int)Columnas.DocTotal].Width = 90;
                e.Layout.Bands[0].Columns[(int)Columnas.DocDate].Width = 90;
                e.Layout.Bands[0].Columns[(int)Columnas.Planeador].Width = 100;

                e.Layout.Bands[0].Columns[(int)Columnas.DocTotal].Format = "C2";
                e.Layout.Bands[0].Columns[(int)Columnas.DocTotal].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            }
            catch (Exception)
            {
            }
        }

        private void dgvDatos_DoubleClickCell(object sender, Infragistics.Win.UltraWinGrid.DoubleClickCellEventArgs e)
        {
            try
            {
                Int32 docEntry = Convert.ToInt32(dgvDatos.Rows[e.Cell.Row.Index].Cells[(int)Columnas.Docnum].Value);

                SDK.Documentos.frmDocumentos formulario = new SDK.Documentos.frmDocumentos(1);
                formulario.MdiParent = this.MdiParent;
                formulario.txtFolio.Text = docEntry.ToString();
                var kea = new KeyPressEventArgs(Convert.ToChar(13));
                formulario.txtFolio_KeyPress(sender, kea);
                formulario.Show();
            }
            catch (Exception)
            {

            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            this.frmOCAbiertas_Load(sender, e);
        }
    }
}
