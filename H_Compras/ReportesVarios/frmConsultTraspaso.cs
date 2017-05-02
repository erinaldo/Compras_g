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
    public partial class frmConsultTraspaso : Constantes.frmEmpty
    {
        Datos.Connection connection = new Datos.Connection();
        Object[] valuesOut = new Object[] { };

        public frmConsultTraspaso()
        {
            InitializeComponent();
        }

        private void frmConsultTraspaso_Load(object sender, EventArgs e)
        {
            this.actualizarToolStripButton.Click -= new EventHandler(btnConsultar_Click);
            this.actualizarToolStripButton.Click += new EventHandler(btnConsultar_Click);

            ClasesSGUV.Form.ControlsForms.setDataSource(clbAlmacen, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesTodos, string.Empty, string.Empty), "WhsName", "WhsCode", "--Todos--");

            clbAlmacen.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
            clbAlmacen.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);

        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                dgvDatos.DataSource = null;

               string Almacenes = ClasesSGUV.Form.ControlsForms.getCadena(clbAlmacen, "'", false, "WhsCode");

                DataTable tbl =
                            connection.GetDataTable("LOG",
                                                    "sp_Reportes",
                                                    new string[] { },
                                                    new string[] { "@TipoConsulta", "@Almacenes", "@Desde", "@Hasta"},
                                                    ref valuesOut, 18, Almacenes, dtpDesde.Value, dtpHasta.Value);



                dgvDatos.DataSource = tbl;
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }

            e.Layout.Bands[0].Columns["DocEntry"].Hidden = true;
        }

        private void dgvDatos_DoubleClickCell(object sender, Infragistics.Win.UltraWinGrid.DoubleClickCellEventArgs e)
        {
            try
            {
                SDK.Documentos.frmTransferencia forulario = new SDK.Documentos.frmTransferencia(4);
                forulario.txtNumero.Text = Convert.ToInt32(dgvDatos.ActiveRow.Cells[0].Value).ToString();
                forulario.IsSolicitud = true;
                forulario.MdiParent = this.MdiParent;
                var kea = new KeyPressEventArgs(Convert.ToChar(13));
                forulario.txtNumero_KeyPress(sender, kea);
                forulario.Show();
            }
            catch (Exception)
            {
            }
        }
    }
}
