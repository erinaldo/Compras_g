using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.ReportesVarios
{
    public partial class frmArticulosNuevos : Constantes.frmEmpty
    {
        public frmArticulosNuevos()
        {
            InitializeComponent();
        }

        private void frmArticulosNuevos_Load(object sender, EventArgs e)
        {
            try
            {
                this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);
                ClasesSGUV.Form.ControlsForms.setDataSource(clbLineas, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Lineas, null, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "Todas");

                clbLineas.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
                clbLineas.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);

                this.guardarToolStripButton.Enabled = false;
                this.actualizarToolStripButton.Enabled = false;
                this.nuevoToolStripButton.Enabled = false;
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
                        command.Parameters.AddWithValue("@TipoConsulta", 10);
                        command.Parameters.AddWithValue("@Lineas", ClasesSGUV.Form.ControlsForms.getCadena(clbLineas, string.Empty, false, "ItmsGrpCod"));

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
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
            e.Layout.Bands[0].Columns["Línea"].Header.Fixed = true;
            e.Layout.Bands[0].Columns["Proveedor"].Header.Fixed = true;
            e.Layout.Bands[0].Columns["Artículo"].Header.Fixed = true;
            e.Layout.Bands[0].Columns["Descripción"].Header.Fixed = true;

            e.Layout.Bands[0].Columns["Proveedor"].Width = 140;
            e.Layout.Bands[0].Columns["Descripción"].Width = 140;

            e.Layout.Bands[0].Columns["Promedio ultimos 4 meses"].Width = 95;
            e.Layout.Bands[0].Columns["Ideal sugerido"].Width = 95;
            e.Layout.Bands[0].Columns["Faltante"].Width = 95;

            e.Layout.Bands[0].Columns["Stock"].Format = "N0";
            e.Layout.Bands[0].Columns["Stock $"].Format = "C0";
            e.Layout.Bands[0].Columns["Ideal"].Format = "N0";
            e.Layout.Bands[0].Columns["Veces Ideal"].Format = "N2";
            e.Layout.Bands[0].Columns["Meses"].Format = "N0";

            e.Layout.Bands[0].Columns["Mes6"].Format = "N0";
            e.Layout.Bands[0].Columns["Mes5"].Format = "N0";
            e.Layout.Bands[0].Columns["Mes4"].Format = "N0";
            e.Layout.Bands[0].Columns["Mes3"].Format = "N0";
            e.Layout.Bands[0].Columns["Mes2"].Format = "N0";
            e.Layout.Bands[0].Columns["Mes1"].Format = "N0";
            e.Layout.Bands[0].Columns["Mes actual"].Format = "N0";

            e.Layout.Bands[0].Columns["Stock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Stock $"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Ideal"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Veces Ideal"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Meses"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Promedio ultimos 4 meses"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Ideal sugerido"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Faltante"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            e.Layout.Bands[0].Columns["Mes6"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Mes5"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Mes4"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Mes3"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Mes2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Mes1"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Promedio ultimos 4 meses"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Ideal sugerido"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Faltante"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Mes actual"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }

        }
    }
}
