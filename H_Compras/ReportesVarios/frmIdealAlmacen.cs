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
    public partial class frmIdealAlmacen : Constantes.frmEmpty
    {
        public frmIdealAlmacen()
        {
            InitializeComponent();
        }

        private void frmIdealAlmacen_Load(object sender, EventArgs e)
        {
            try
            {
                this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                txtArticulo.Focus();
                nuevoToolStripButton.Enabled = false;
                guardarToolStripButton.Enabled = false;
                ayudaToolStripButton.Enabled = false;
                actualizarToolStripButton.Enabled = false;

                DataTable tbl_Articulos = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetArticulos, string.Empty, string.Empty);
                ClasesSGUV.Form.ControlsForms.Autocomplete(txtArticulo, tbl_Articulos.Copy(), "ItemCode");

                ClasesSGUV.Form.ControlsForms.setDataSource(clbAlmacenes, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesTodos, null, string.Empty), "WhsName", "WhsCode", "Seleccionar todo");
                ClasesSGUV.Form.ControlsForms.setDataSource(clbLineas, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Lineas, null, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "Todas");

                clbAlmacenes.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
                clbAlmacenes.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);

                clbLineas.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
                clbLineas.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void btnConsult_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Reportes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 3);
                        command.Parameters.AddWithValue("@ItemCode", txtArticulo.Text);
                        command.Parameters.AddWithValue("@Lineas", ClasesSGUV.Form.ControlsForms.getCadena(clbLineas, string.Empty, false, "ItmsGrpCod"));
                        command.Parameters.AddWithValue("@Almacenes", ClasesSGUV.Form.ControlsForms.getCadena(clbAlmacenes, "'", false, "WhsCode"));
                        command.Parameters.AddWithValue("@Transito", rbTransito.Checked);

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

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy;

            dgvDatos.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;

            e.Layout.Bands[0].Columns[5].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[6].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[7].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[8].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[10].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;


            e.Layout.Bands[0].Columns[5].Format = "N0";
            e.Layout.Bands[0].Columns[6].Format = "N0";
            e.Layout.Bands[0].Columns[7].Format = "N0";
            e.Layout.Bands[0].Columns[8].Format = "N0";
            e.Layout.Bands[0].Columns[10].Format = "N2";

            foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Activation.NoEdit;
            }
        }

    }
}
