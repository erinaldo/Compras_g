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
    public partial class frmClasificacionABC : Constantes.frmEmpty
    {
        private enum Columnas
        {
            Linea, Articulo, Descripcion, Clasificacion
        }

        public frmClasificacionABC()
        {
            InitializeComponent();
        }

        private void frmClasificacionABC_Load(object sender, EventArgs e)
        {
            try
            {
                this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                txtArticulo.Clear();
                ClasesSGUV.Form.ControlsForms.setDataSource(clbLineas, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Lineas, null, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "Todas");
                ClasesSGUV.Form.ControlsForms.setDataSource(clbLineas2, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Lineas, null, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "Todas");
                ClasesSGUV.Form.ControlsForms.setDataSource(clbLineas3, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Lineas, null, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "Todas");
                DataTable tbl_Articulos = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetArticulos, string.Empty, string.Empty);
                ClasesSGUV.Form.ControlsForms.Autocomplete(txtArticulo, tbl_Articulos.Copy(), "ItemCode");
                ClasesSGUV.Form.ControlsForms.Autocomplete(txtArticulo3, tbl_Articulos.Copy(), "ItemCode");
                ClasesSGUV.Form.ControlsForms.Autocomplete(txtArticulo2, tbl_Articulos.Copy(), "ItemCode");

                clbLineas.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
                clbLineas.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);

                clbLineas2.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
                clbLineas2.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);

                clbLineas3.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
                clbLineas3.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);

                txtArticulo.Focus();
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Inventario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 12);
                        command.Parameters.AddWithValue("@ItemCode", txtArticulo.Text);
                        command.Parameters.AddWithValue("@Lineas", ClasesSGUV.Form.ControlsForms.getCadena(clbLineas, string.Empty, true, "ItmsGrpCod"));

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
            e.Layout.Bands[0].Columns[(int)Columnas.Linea].Width = 90;
            e.Layout.Bands[0].Columns[(int)Columnas.Articulo].Width = 110;
            e.Layout.Bands[0].Columns[(int)Columnas.Descripcion].Width = 250;
            e.Layout.Bands[0].Columns[(int)Columnas.Clasificacion].Width = 100;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Reportes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 5);
                        command.Parameters.AddWithValue("@ItemCode", txtArticulo2.Text);
                        command.Parameters.AddWithValue("@Lineas", ClasesSGUV.Form.ControlsForms.getCadena(clbLineas2, string.Empty, true, "ItmsGrpCod"));

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

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Reportes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 6);
                        command.Parameters.AddWithValue("@ItemCode", txtArticulo3.Text);
                        command.Parameters.AddWithValue("@Lineas", ClasesSGUV.Form.ControlsForms.getCadena(clbLineas3, string.Empty, true, "ItmsGrpCod"));

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
