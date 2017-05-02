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
    public partial class frmStockOut : Constantes.frmEmpty
    {
        Object[] valuesOut = new Object[] { };
        Datos.Connection connection = new Datos.Connection();

        public frmStockOut()
        {
            InitializeComponent();
        }

        private void frmDesabasto_Load(object sender, EventArgs e)
        {
            try
            {
                nuevoToolStripButton.Enabled = false;
                guardarToolStripButton.Enabled = false;

                dgvDatos.DataSource = null;
                dgvDetalle.DataSource = null;

                actualizarToolStripButton.Click -= new EventHandler(btnActualizar_Click);
                actualizarToolStripButton.Click += new EventHandler(btnActualizar_Click);

                DataTable tbl =
                    connection.GetDataTable("LOG",
                                            "sp_Indicadores",
                                            new string[] { },
                                            new string[] { "@TipoConsulta", "@Comprador" },
                                            ref valuesOut, 2, string.Empty);

                dgvDatos.DataSource = tbl;

                this.dgvDatos.DisplayLayout.Override.RowSelectorHeaderStyle = Infragistics.Win.UltraWinGrid.RowSelectorHeaderStyle.ColumnChooserButton;
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            this.OnLoad(e);
        }

        private void dgvDatos_AfterRowActivate(object sender, EventArgs e)
        {
            try
            {
                string itemcode = dgvDatos.ActiveRow.Cells["Artículo"].Value.ToString();

                DataTable tbl =
                    connection.GetDataTable("LOG",
                                            "sp_Reportes",
                                            new string[] { },
                                            new string[] { "@TipoConsulta", "@ItemCode" },
                                            ref valuesOut, 15, itemcode);

                dgvDetalle.DataSource = tbl;
            }
            catch (Exception)
            {
               
            }
        }

        private void dgvDetalle_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["Artículo"].Header.Fixed = true;

            e.Layout.Bands[0].Columns["transAPI"].Header.Caption = "Transferir API";
            e.Layout.Bands[0].Columns["transCOR"].Header.Caption = "Transferir COR";
            e.Layout.Bands[0].Columns["transPUE"].Header.Caption = "Transferir PUE";
            e.Layout.Bands[0].Columns["transEDOMEX"].Header.Caption = "Transferir EDOMEX";
            e.Layout.Bands[0].Columns["transAPI"].Header.Caption = "Transferir API";
            e.Layout.Bands[0].Columns["transGDL"].Header.Caption = "Transferir GDL";
            e.Layout.Bands[0].Columns["transMTY"].Header.Caption = "Transferir MTY";
            e.Layout.Bands[0].Columns["transNLAREDO"].Header.Caption = "Transferir NLAREDO";
            e.Layout.Bands[0].Columns["transSAL"].Header.Caption = "Transferir SAL";
            e.Layout.Bands[0].Columns["transTEP"].Header.Caption = "Transferir TEP";

            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                if (item.Header.Caption.Contains("Ideal") | item.Header.Caption.Contains("Stock"))
                    item.Hidden = true;

                item.Format = "N0";
                item.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            }

            e.Layout.Bands[0].Columns["Stock CEDIS"].Hidden = false;
            e.Layout.Bands[0].Columns["Artículo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["Descripción"].Width = 280;

            e.Layout.Bands[0].Columns["diasStock"].Hidden = true;

            e.Layout.Bands[0].Columns["Stock"].Format = "N0";
            e.Layout.Bands[0].Columns["Solicitado"].Format = "N0";
            e.Layout.Bands[0].Columns["diasStock"].Format = "N0";
            e.Layout.Bands[0].Columns["Límite"].Format = "N0";
            e.Layout.Bands[0].Columns["Ideal"].Format = "N0";
            e.Layout.Bands[0].Columns["Monto"].Format = "C2";

            e.Layout.Bands[0].Columns["Stock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Solicitado"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Ideal"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["diasStock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Límite"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Monto"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Fecha de arribo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            
            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
        }
    }
}
