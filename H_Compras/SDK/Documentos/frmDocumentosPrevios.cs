using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.SDK.Documentos
{
    public partial class frmDocumentosPrevios : Constantes.frmEmpty
    {
        public frmDocumentosPrevios()
        {
            InitializeComponent();
        }

        private void frmDocumentosPrevios_Load(object sender, EventArgs e)
        {
            this.nuevoToolStripButton.Enabled = false;
            this.guardarToolStripButton.Enabled = false;
            this.exportarToolStripButton.Enabled = false;
            this.actualizarToolStripButton.Enabled = false;
            this.ayudaToolStripButton.Enabled = false;

            ClasesSGUV.Form.ControlsForms.setDataSource(cbDocument, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.DocumentosSDK, null, string.Empty), "Nombre", "Code", "---Selecciona un documento---");

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_SDKDocumentosPrevios", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 3);
                        command.Parameters.AddWithValue("@Desde", dtpDesde.Value);
                        command.Parameters.AddWithValue("@Hasta", dtpHasta.Value);

                        command.Parameters.AddWithValue("@DocType", cbDocument.SelectedValue);

                        DataTable tbl = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(tbl);
                        dgvDatos.DataSource = tbl;
                    }
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje("ERROR: " + ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            if (Convert.ToInt32(cbDocument.SelectedValue) == 4)
            {
                foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn column in e.Layout.Bands[0].Columns)
                {
                    column.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                }
                e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[2].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[3].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[4].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[5].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[6].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns[7].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

                e.Layout.Bands[0].Columns["DocStatus"].Hidden = true;
                e.Layout.Bands[0].Columns["Crear"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
            }
        }

        private void dgvDatos_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            try
            {
                SDK.Documentos.frmTransferencia formulario = new SDK.Documentos.frmTransferencia(4);
                formulario.MdiParent = this.MdiParent;
                formulario.txtNumero.Text = e.Row.Cells[0].Value.ToString();
                formulario.Text = "Solicitud de traslado (Preeliminar)";
                var kea = new KeyPressEventArgs(Convert.ToChar(13));
                formulario.txtNumero_KeyPress(sender, kea);
                if (formulario.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    btnBuscar_Click(sender, e);
                }
            }
            catch (Exception)
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                #region Transferencias de stock
                SDKDatos.SDK_DI di = new SDKDatos.SDK_DI(4);//conectar

                foreach (DataRow item in (dgvDatos.DataSource as DataTable).Rows)
                {//Recorrer todos los elementos marcados del grid
                    if (item.Field<bool>("Crear"))
                    {
                        SDKDatos.SDK_OWTR document = new SDKDatos.SDK_OWTR();
                        document = document.Fill_Previo(Convert.ToDecimal(item[0]), false);
                        document.DocDate = DateTime.Now;
                        //identificar el numero de traspasos que se generarán
                        var qry = (from warehouse in document.Lines.AsEnumerable()
                                   where warehouse.LineStatus == "A"
                                   select new
                                   {
                                       Destino = warehouse.WhsCode,
                                       Origen = warehouse.FromWhsCode
                                   }).Distinct();

                        //para cada transferencia detectada
                        //gerar documento de traspaso
                        foreach (var tranferencias in qry)
                        {
                            SDKDatos.SDK_OWTR documentHijo = new SDKDatos.SDK_OWTR();

                            documentHijo.DocNum = document.DocNum;
                            documentHijo.DocDate = document.DocDate;
                            documentHijo.Filler = tranferencias.Origen;
                            documentHijo.ToWhsCode = tranferencias.Destino;

                            ////recorrer todas las filas del documento previo para 
                            ////extraer solo las correspondientes a este traspaso
                            foreach (SDKDatos.SDK_WTR1 linePreeliminar in document.Lines)
                            {
                                if (linePreeliminar.WhsCode == tranferencias.Destino
                                       && linePreeliminar.FromWhsCode == tranferencias.Origen)
                                {
                                    documentHijo.Lines.Add(linePreeliminar);
                                }
                            }

                            decimal folio = di.CrearSolicitudTraslado(documentHijo);
                            if (folio > 0)
                            {
                                document.CambiarLineStatus(document, "G", tranferencias.Origen, tranferencias.Destino);
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
            
        }

        private void dgvDatos_AfterRowActivate(object sender, EventArgs e)
        {
            try
            {
                dgvTraspaso.DataSource = null;

                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_SDKDocumentosPrevios", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 9);

                        command.Parameters.AddWithValue("@DocKey", dgvDatos.ActiveRow.Cells[0].Value);

                        DataTable tbl = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(tbl);
                        dgvTraspaso.DataSource = tbl;
                    }
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje("ERROR: " + ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void dgvSolicitudes_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                column.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
            e.Layout.Bands[0].Columns[0].Hidden = true;
        }

        private void dgvTraspaso_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                column.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
            e.Layout.Bands[0].Columns[0].Hidden = true;
        }

        private void dgvDatos_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if(!e.Row.Cells["DocStatus"].Value.ToString().Equals("A"))
            {
                e.Row.Cells["Crear"].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
        }

        private void dgvSolicitudes_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            try
            {
                SDK.Documentos.frmTransferencia formulario = new SDK.Documentos.frmTransferencia(4);
                formulario.MdiParent = this.MdiParent;
                formulario.txtNumero.Text = e.Row.Cells[0].Value.ToString();
                formulario.Text = "Solicitud de traslado";
                formulario.IsSolicitud = true;
                var kea = new KeyPressEventArgs(Convert.ToChar(13));
                formulario.txtNumero_KeyPress(sender, kea);
                if (formulario.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    btnBuscar_Click(sender, e);
                }
            }
            catch (Exception)
            {
            }
        }

        private void dgvTraspaso_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            try
            {
                SDK.Documentos.frmTransferencia formulario = new SDK.Documentos.frmTransferencia(4);
                formulario.MdiParent = this.MdiParent;
                formulario.txtNumero.Text = e.Row.Cells[0].Value.ToString();
                formulario.Text = "Transferencia de Stock";
                formulario.IsSolicitud = true;
                var kea = new KeyPressEventArgs(Convert.ToChar(13));
                formulario.txtNumero_KeyPress(sender, kea);
                if (formulario.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    btnBuscar_Click(sender, e);
                }
            }
            catch (Exception)
            {
            }
        }
    }

}