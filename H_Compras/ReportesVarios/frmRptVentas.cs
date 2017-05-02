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
    public partial class frmRptVentas : Constantes.frmEmpty
    {
        public DataTable tbl_Articulos = new DataTable();

        private enum Columnas
        {
            Linea,
            Articulo,
            Descripcion,
            Almacen,
            Mes1,
            Mes2,
            Mes3,
            Mes4,
            Mes5,
            Mes6,
            Mes7,
            Mes8,
            Mes9,
            Mes10,
            Mes11,
            Mes12,
            MesC,
            Total,
            Promedio1,
            Promedio2,
            Stock,
            Solicitado
        }

        public frmRptVentas()
        {
            InitializeComponent();
        }

        private void frmRptVentas_Load(object sender, EventArgs e)
        {
            try
            {
                //this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                txtArticulo.Clear();

                ClasesSGUV.Form.ControlsForms.setDataSource(clbAlmacenes, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesVenta, null, string.Empty), "WhsName", "WhsCode", "Seleccionar todo");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbLinea, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Lineas, null, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "Todas");
                tbl_Articulos = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetArticulos, string.Empty, string.Empty);
                ClasesSGUV.Form.ControlsForms.Autocomplete(txtArticulo, tbl_Articulos.Copy(), "ItemCode");

                DataTable tbl = clbAlmacenes.DataSource as DataTable;

                //DataRow row = tbl.NewRow();
                //row[0] = "26";
                //row[1] = "ALMACEN RACSA";
                //tbl.Rows.InsertAt(row, 1);

                DataRow row1 = tbl.NewRow();
                row1[0] = "--";
                row1[1] = "Almacenes PJ";
                tbl.Rows.InsertAt(row1, 1);

                clbAlmacenes.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
                clbAlmacenes.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("sp_HistorialVentas", connection))
                    {
                        string x = ClasesSGUV.Form.ControlsForms.getCadena(clbAlmacenes, "'", false, "WhsCode");
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 2);
                        command.Parameters.AddWithValue("@Almacen", ClasesSGUV.Form.ControlsForms.getCadena(clbAlmacenes,"'", false, "WhsCode"));
                        command.Parameters.AddWithValue("@Linea", cbLinea.SelectedValue);
                        command.Parameters.AddWithValue("@Articulo", txtArticulo.Text);
                        command.Parameters.AddWithValue("@Group", cbAgrupar.Checked);

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
            Infragistics.Win.UltraWinCalcManager.UltraCalcManager calcManager;
            calcManager = new Infragistics.Win.UltraWinCalcManager.UltraCalcManager(this.Container);
            e.Layout.Grid.CalcManager = calcManager;

            Infragistics.Win.UltraWinGrid.UltraGridLayout layout = e.Layout;
            Infragistics.Win.UltraWinGrid.UltraGridBand band = layout.Bands[0];
            e.Layout.Bands[0].Summaries.Clear();
            
            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                item.Header.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;

                if(item.Index >= (int)Columnas.Mes1 && item.Index <= (int)Columnas.MesC)
                {
                    Infragistics.Win.UltraWinGrid.SummarySettings summary1 = band.Summaries.Add("sum" + item.Index, Infragistics.Win.UltraWinGrid.SummaryType.Sum, band.Columns[item.Index]);

                    summary1.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    summary1.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
                    summary1.DisplayFormat = "{0:N0}";
                    summary1.Formula = "SUM([" + item.Header.Caption + "])";
                }
            }

            e.Layout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy;
            
            e.Layout.Bands[0].Columns[(int)Columnas.Linea].Header.Fixed = true;
            e.Layout.Bands[0].Columns[(int)Columnas.Articulo].Header.Fixed = true;
            e.Layout.Bands[0].Columns[(int)Columnas.Descripcion].Header.Fixed = true;
            e.Layout.Bands[0].Columns[(int)Columnas.Almacen].Header.Fixed = true;

            e.Layout.Bands[0].Columns[(int)Columnas.Mes1].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes2].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes3].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes4].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes5].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes6].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes7].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes8].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes9].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes10].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes11].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes12].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas.MesC].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas.Stock].Width = 70;
            e.Layout.Bands[0].Columns[(int)Columnas.Solicitado].Width = 70;

            e.Layout.Bands[0].Columns[(int)Columnas.Linea].Width = 80;
            e.Layout.Bands[0].Columns[(int)Columnas.Articulo].Width = 90;
            e.Layout.Bands[0].Columns[(int)Columnas.Descripcion].Width = 120;
            e.Layout.Bands[0].Columns[(int)Columnas.Almacen].Width = 90;
            e.Layout.Bands[0].Columns[(int)Columnas.Total].Width = 100;
            e.Layout.Bands[0].Columns[(int)Columnas.Promedio1].Width = 100;
            e.Layout.Bands[0].Columns[(int)Columnas.Promedio2].Width = 100;

            e.Layout.Bands[0].Columns[(int)Columnas.Total].Format = "N0";
            e.Layout.Bands[0].Columns[(int)Columnas.Promedio1].Format = "N0";
            e.Layout.Bands[0].Columns[(int)Columnas.Promedio2].Format = "N0";
            e.Layout.Bands[0].Columns[(int)Columnas.Stock].Format = "N0";
            e.Layout.Bands[0].Columns[(int)Columnas.Solicitado].Format = "N0";

            e.Layout.Bands[0].Columns[(int)Columnas.Mes1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes2].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes3].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes4].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes5].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes6].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes7].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes8].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes9].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes10].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes11].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Mes12].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.MesC].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            e.Layout.Bands[0].Columns[(int)Columnas.Total].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Promedio1].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Promedio2].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Stock].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[(int)Columnas.Solicitado].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            //e.Layout.Bands[0].Columns[(int)Columnas.Stock].Hidden = cbAgrupar.Checked;
            //e.Layout.Bands[0].Columns[(int)Columnas.Solicitado].Hidden = cbAgrupar.Checked;
        }

        private void txtArticulo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(txtArticulo.Text))
                {
                    button1_Click(sender, e);
                }
            }
        }
    }
}
