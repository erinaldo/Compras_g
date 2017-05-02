using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.Inventarios.ListasPrecios
{
    public partial class frmReporteLP : Constantes.frmEmpty
    {
        public frmReporteLP()
        {
            InitializeComponent();
        }

        private void frmReporteLP_Load(object sender, EventArgs e)
        {
            try
            {
               // ClasesSGUV.Form.ControlsForms.setDataSource(cbListasPrecios, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListasPrecios, ClasesSGUV.Login.Rol, string.Empty), "ListName", "ListNum", "Lista de precios");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbLineas, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.LineasTodas, ClasesSGUV.Login.Rol, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "Línea");

            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_ListaPrecios", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 11);
                        command.Parameters.AddWithValue("@ItmsGrpCod", cbLineas.SelectedValue);

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
            foreach (var item in dgvDatos.DisplayLayout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }

            //e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Artículo";
            //e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Descripción";
            //e.Layout.Bands[0].Columns["ItmsGrpNam"].Header.Caption = "Línea";
            //e.Layout.Bands[0].Columns["ListName"].Header.Caption = "Lista de precios";
            //e.Layout.Bands[0].Columns["PriceList"].Hidden = true;

            //e.Layout.Bands[0].Columns["Precio con Descuento"].Header.Caption = "Precio Final";
            //e.Layout.Bands[0].Columns["Factor con Descuento"].Header.Caption = "Factor Final";

            //e.Layout.Bands[0].Columns["Precio Original"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            //e.Layout.Bands[0].Columns["Precio con Descuento"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            //e.Layout.Bands[0].Columns["Factor original"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            //e.Layout.Bands[0].Columns["Factor con Descuento"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            //e.Layout.Bands[0].Columns["Precio Original"].Format = "C4";
            //e.Layout.Bands[0].Columns["Precio con Descuento"].Format = "C4";
            //e.Layout.Bands[0].Columns["Factor original"].Format = "N4";
            //e.Layout.Bands[0].Columns["Factor con Descuento"].Format = "N4";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Se actualizarán los factores de la línea: " + cbLineas.Text + "\r\n¿Desea continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                    return;

                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    connection.Open();

                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;

                    transaction = connection.BeginTransaction("SampleTransaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    try
                    {
                        command.CommandText = "sp_ListaPrecios";
                        command.CommandType = CommandType.StoredProcedure;
                        //foreach (var item in dgvDatos.Rows)
                        //{
                        //    command.Parameters.Clear();

                        command.Parameters.AddWithValue("@TipoConsulta", 12);
                        command.Parameters.AddWithValue("@ItmsGrpCod", cbLineas.SelectedValue);
                        //command.Parameters.AddWithValue("@ItemCode", item.Cells["ItemCode"].Value);
                        //command.Parameters.AddWithValue("@ItemCode", item.Cells["ItemCode"].Value);
                        //command.Parameters.AddWithValue("@PriceList", item.Cells["PriceList"].Value);
                        //command.Parameters.AddWithValue("@Factor", item.Cells["Factor con Descuento"].Value);

                        command.ExecuteNonQuery();
                        // }

                        transaction.Commit();
                        MessageBox.Show("Listo", "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                        Console.WriteLine("  Message: {0}", ex.Message);
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                            Console.WriteLine("  Message: {0}", ex2.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }
    }
}
