using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

namespace H_Compras.ReportesVarios
{
    public partial class frmListasPrecios : Constantes.frmEmpty
    {
        public frmListasPrecios()
        {
            InitializeComponent();
        }

        private void frmListasPrecios_Load(object sender, EventArgs e)
        {
            try
            {
                nuevoToolStripButton.Enabled = false;
                guardarToolStripButton.Enabled = false;
                ayudaToolStripButton.Enabled = false;

                this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                DataTable tbl_Articulos = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetArticulos, string.Empty, string.Empty);
                ClasesSGUV.Form.ControlsForms.Autocomplete(txtArticulo, tbl_Articulos.Copy(), "ItemCode");

                ClasesSGUV.Form.ControlsForms.setDataSource(clbLineas, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Lineas, null, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "Todas");
                clbLineas.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
                clbLineas.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);

                actualizarToolStripButton.Click -= new EventHandler(button1_Click);
                actualizarToolStripButton.Click += new EventHandler(button1_Click);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Activation.NoEdit;
                item.Width = 80;
                item.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                item.Header.FixedHeaderIndicator = FixedHeaderIndicator.None;

                if (item.Header.Caption.Contains("Cur") || string.IsNullOrEmpty(item.Header.Caption)) 
                {
                    item.Header.Caption = string.Empty;
                    item.Width = 10;
                }
            }
            e.Layout.Bands[0].Columns["ItemName"].Width = 180;

            e.Layout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy;

            e.Layout.Bands[0].Columns["ItemCode"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
            e.Layout.Bands[0].Columns["ItemName"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
            e.Layout.Bands[0].Columns["ItmsGrpNam"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

            e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
            e.Layout.Bands[0].Columns["ItemName"].Header.Fixed = true;
            e.Layout.Bands[0].Columns["OnHand"].Header.Fixed = true;
            e.Layout.Bands[0].Columns["ItmsGrpNam"].Header.Fixed = true;

            e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Artículo";
            e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Descripción";
            e.Layout.Bands[0].Columns["OnHand"].Header.Caption = "Stock";
            e.Layout.Bands[0].Columns["ItmsGrpNam"].Header.Caption = "Línea";


            e.Layout.Bands[0].Columns["Column1"].Hidden = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_ListaPrecios", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 2);
                        command.Parameters.AddWithValue("@ItemCode", txtArticulo.Text);
                        command.Parameters.AddWithValue("@Lineas", ClasesSGUV.Form.ControlsForms.getCadena(clbLineas, string.Empty, false, "ItmsGrpCod"));
                        command.Parameters.AddWithValue("@Check1", cbStock.Checked);

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        DataTable tbl = new DataTable();
                        da.Fill(tbl);

                        dgvDatos.DataSource = tbl;
                    }
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }
    }
}
