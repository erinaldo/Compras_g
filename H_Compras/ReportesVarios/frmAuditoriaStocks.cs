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
    public partial class frmAuditoriaStocks : Constantes.frmEmpty
    {
        Datos.Connection connection = new Datos.Connection();

        private enum DocType
        {
            Entrega = 15,
            Devolucion = 16,
            FacturaDeudores = 13,
            NCDeudores = 14,
            EMProveedor = 20,
            DMProveedor = 21,
            FacturaProveedor = 18,
            NCProveedor = 19,
            EMInventarios = 59,
            SalidaMercancias = 60,
            Transferencias = 67
        }

        private string getPath(int docType)
        {
            switch (docType)
            {
                case (int)DocType.Entrega:
                    return @"\\192.168.2.100\HalcoNET\Crystal\ODLN_I.rpt";
                case (int)DocType.Devolucion:
                    return @"\\192.168.2.100\HalcoNET\Crystal\Devolucion.rpt";
                 case (int)DocType.Transferencias:
                    return @"\\192.168.2.100\HalcoNET\Crystal\PJ_TraspasoAbr2016.rpt";
                 case (int)DocType.FacturaDeudores:
                    return @"\\192.168.2.100\HalcoNET\Crystal\OINV_DocType_I.rpt";
                 case (int)DocType.NCDeudores:
                    return @"\\192.168.2.100\HalcoNET\Crystal\ORIN_DocType_I.rpt";
                default: return string.Empty;
            }
        }
        
        public frmAuditoriaStocks()
        {
            InitializeComponent();
        }

        private void frmAuditoriaStocks_Load(object sender, EventArgs e)
        {
            Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);
            ClasesSGUV.Form.ControlsForms.setDataSource(clbAlmacenes, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesTodos, string.Empty, string.Empty), "WhsName", "WhsCode", "--Todos--");
            ClasesSGUV.Form.ControlsForms.setDataSource(clbLineas, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.LineasTodas, string.Empty, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "--Todas--");

            clbLineas.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
            clbLineas.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);

            clbAlmacenes.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
            clbAlmacenes.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);


            DataTable tbl_Articulos = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetArticulos, string.Empty, string.Empty);
            ClasesSGUV.Form.ControlsForms.Autocomplete(txtItemCode, tbl_Articulos.Copy(), "ItemCode");
            this.dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.Copy, true, true);

            nuevoToolStripButton.Enabled = false;
            guardarToolStripButton.Enabled = false;
            actualizarToolStripButton.Enabled = false;
            imprimirToolStripButton.Enabled = true;

            imprimirToolStripButton.Click -= new EventHandler(btnPrint_Click);
            imprimirToolStripButton.Click += new EventHandler(btnPrint_Click);
        }

        private void btnConsult_Click(object sender, EventArgs e)
        {
            try
            {
                string Lineas = ClasesSGUV.Form.ControlsForms.getCadena(clbLineas, string.Empty, false, "ItmsGrpCod");
                string Almacenes = ClasesSGUV.Form.ControlsForms.getCadena(clbAlmacenes, "'", false, "WhsCode");
                Object[] valuesOut = new Object[] { };

                string documentos = string.Empty;
                if (checkBox2.Checked) documentos += checkBox2.AccessibleDescription + ",";
                if (checkBox3.Checked) documentos += checkBox3.AccessibleDescription + ",";
                if (checkBox4.Checked) documentos += checkBox4.AccessibleDescription + ",";
                if (checkBox5.Checked) documentos += checkBox5.AccessibleDescription + ",";
                if (checkBox6.Checked) documentos += checkBox6.AccessibleDescription + ",";
                if (checkBox7.Checked) documentos += checkBox7.AccessibleDescription + ",";
                if (checkBox8.Checked) documentos += checkBox8.AccessibleDescription + ",";
                if (checkBox9.Checked) documentos += checkBox9.AccessibleDescription + ",";
                if (checkBox10.Checked) documentos += checkBox10.AccessibleDescription + ",";
                if (checkBox11.Checked) documentos += checkBox11.AccessibleDescription + ",";
                if (checkBox12.Checked) documentos += checkBox12.AccessibleDescription + ",";

                documentos = documentos.Trim(',');

                if (checkBox13.Checked)
                    documentos = string.Empty;

                DataSet ds =
                   connection.GetDataSet("LOG",
                                           "su_Almacen",
                                           new string[] { },
                                           new string[] { "@TipoConsulta", "@ItemCode", "@Desde", "@Hasta", "@Almacenes", "@Lineas", "@ConStock", "@Documentos" },
                                           ref valuesOut, 24, txtItemCode.Text, uccDesde.Value == null ? string.Empty : uccDesde.Value, uccHasta.Value, Almacenes, Lineas, checkBox1.Checked, documentos);

                //var header = from r in tbl.AsEnumerable()
                //             f
                DataRelation rel = new DataRelation("rel", ds.Tables[0].Columns["Artículo"], ds.Tables[1].Columns["Artículo"]);
                ds.Relations.Add(rel);
                dgvDatos.DataSource = ds;
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            dgvDatos.DisplayLayout.Bands[1].ColHeadersVisible = false;

            dgvDatos.DisplayLayout.Bands[1].Columns["Cantidad"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            dgvDatos.DisplayLayout.Bands[1].Columns["Valor trans."].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            dgvDatos.DisplayLayout.Bands[1].Columns["Cantidad Acumulada"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            dgvDatos.DisplayLayout.Bands[1].Columns["Cantidad"].Format = "N0";
            dgvDatos.DisplayLayout.Bands[1].Columns["Valor trans."].Format = "N2";
            dgvDatos.DisplayLayout.Bands[1].Columns["Cantidad Acumulada"].Format = "N0";

            dgvDatos.DisplayLayout.Bands[0].Columns["Cantidad"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            dgvDatos.DisplayLayout.Bands[0].Columns["Valor trans."].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            dgvDatos.DisplayLayout.Bands[0].Columns["Cantidad Acumulada"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            dgvDatos.DisplayLayout.Bands[0].Columns["Cantidad"].Format = "N0";
            dgvDatos.DisplayLayout.Bands[0].Columns["Valor trans."].Format = "N2";
            dgvDatos.DisplayLayout.Bands[0].Columns["Cantidad Acumulada"].Format = "N0";

            dgvDatos.DisplayLayout.Bands[0].Columns["CreatedBy"].Hidden = true;
            dgvDatos.DisplayLayout.Bands[0].Columns["TransType"].Hidden = true;

            dgvDatos.DisplayLayout.Bands[1].Columns["CreatedBy"].Hidden = true;
            dgvDatos.DisplayLayout.Bands[1].Columns["TransType"].Hidden = true;

            foreach (var band in dgvDatos.DisplayLayout.Bands)
            {
                foreach (var column in band.Columns)
                {
                    column.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                }
            }

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //string dsXML = (dgvDatos.DataSource as DataSet).GetXml();
            DataSet ds = (dgvDatos.DataSource as DataSet);
            //using (System.IO.StreamWriter fs = new System.IO.StreamWriter("c:\\pp\\datos.xml")) // XML File Path
            //{
            //    (dgvDatos.DataSource as DataSet).WriteXml(fs);


            //}

            Constantes.frmVisor form = new Constantes.frmVisor(ds, @"\\192.168.2.100\HalcoNET\Crystal\rptInformeAuditoriaStocks.rpt");
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void checkBox13_Click(object sender, EventArgs e)
        {
            if (checkBox13.Checked)
            {
                checkBox2.Enabled = false;
                checkBox3.Enabled = false;
                checkBox4.Enabled = false;
                checkBox5.Enabled = false;
                checkBox6.Enabled = false;
                checkBox7.Enabled = false;
                checkBox8.Enabled = false;
                checkBox9.Enabled = false;
                checkBox10.Enabled = false;
                checkBox11.Enabled = false;
                checkBox12.Enabled = false;
            }
            else
            {
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
                checkBox4.Enabled = true;
                checkBox5.Enabled = true;
                checkBox6.Enabled = true;
                checkBox7.Enabled = true;
                checkBox8.Enabled = true;
                checkBox9.Enabled = true;
                checkBox10.Enabled = true;
                checkBox11.Enabled = true;
                checkBox12.Enabled = true;
            }
        }

        private void dgvDatos_DoubleClickCell(object sender, Infragistics.Win.UltraWinGrid.DoubleClickCellEventArgs e)
        {
            if(e.Cell.Band.Index == 1)
            {
                //MessageBox.Show(dgvDatos.ActiveRow.Cells["CreatedBy"].Value.ToString());

                //Constantes.Clases.LoadRPT rpt = new Constantes.Clases.LoadRPT(getPath(Convert.ToInt32(dgvDatos.ActiveRow.Cells["TransType"].Value)));
                //Constantes.frmVisor form = new Constantes.frmVisor(rpt.DocRPT);
                //rpt.GenerarPDF(dgvDatos.ActiveRow.Cells["CreatedBy"].Value.ToString());
                //form.MdiParent = this.MdiParent;
                //form.Show();
            }
        }
    }
}
