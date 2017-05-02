using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.Inventarios.Conteos
{
    public partial class frmReporte1 : Constantes.frmEmpty
    {
        public frmReporte1()
        {
            InitializeComponent();
        }

        private void frmReporte1_Load(object sender, EventArgs e)
        {
            nuevoToolStripButton.Enabled = false;
            guardarToolStripButton.Enabled = false;
            actualizarToolStripButton.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 13);
                        command.Parameters.AddWithValue("@idConteo", txtFolio.Text);

                        DataTable table = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        da.Fill(table);

                        dgvDatos.DataSource = table;

                        txtAlmacen.Text = (from item in table.AsEnumerable()
                                           select item.Field<string>("WhsCode") + " - " + item.Field<string>("WhsName")).FirstOrDefault();

                        txtComentarios.Text = (from item in table.AsEnumerable()
                                               select item.Field<string>("CommentH")).FirstOrDefault();


                        txtUsuario.Text = (from item in table.AsEnumerable()
                                               select item.Field<string>("Nombre")).FirstOrDefault();

                       
                        txtFechaCreacion.Text = (from item in table.AsEnumerable()
                                                 select item.Field<DateTime>("DocDate")).FirstOrDefault().ToShortDateString();

                        foreach (DataRow item in table.Rows)
                        {
                            if (item["UpdateDate"] != DBNull.Value)
                                txtFechaCaptura.Text = item.Field<DateTime>("UpdateDate").ToShortDateString();

                            break;
                        }
                        

                        txtFechaReporte.Text = DateTime.Now.ToShortDateString();
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
            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
            e.Layout.Bands[0].Columns["Línea"].Header.Fixed = true;
            e.Layout.Bands[0].Columns["Código de artículo"].Header.Fixed = true;
            e.Layout.Bands[0].Columns["Status de código"].Header.Fixed = true;

            e.Layout.Bands[0].Columns["Línea"].Width = 80;
            e.Layout.Bands[0].Columns["Código de artículo"].Width = 90;
            e.Layout.Bands[0].Columns["Status de código"].Width = 70;
            e.Layout.Bands[0].Columns["Unidad de medida"].Width = 70;
            e.Layout.Bands[0].Columns["Descripcion de artículo"].Width = 120;
            e.Layout.Bands[0].Columns["ABC"].Width = 60;
            e.Layout.Bands[0].Columns["Inventario teorico"].Width = 80;
            e.Layout.Bands[0].Columns["Inventario fisico"].Width = 80;
            e.Layout.Bands[0].Columns["Diferencia"].Width = 80;
            e.Layout.Bands[0].Columns["Observaciones"].Width = 80;
            e.Layout.Bands[0].Columns["Costo Promedio"].Width = 80;
            e.Layout.Bands[0].Columns["Importe basado en Inv Teorico  por costo promedio"].Width = 100;
            e.Layout.Bands[0].Columns["Importe basado en Inv Fisico por  costo promedio"].Width = 100;
            e.Layout.Bands[0].Columns["Diferencia en importe por costo promedio"].Width = 100;
            e.Layout.Bands[0].Columns["Eficiencia de Importe de Articulos por Linea"].Width = 100;
            e.Layout.Bands[0].Columns["Eficiencia de articulos por línea"].Width = 100;

            e.Layout.Bands[0].Columns["IdConteo"].Hidden = true;
            e.Layout.Bands[0].Columns["CommentH"].Hidden = true;
            e.Layout.Bands[0].Columns["WhsCode"].Hidden = true;
            e.Layout.Bands[0].Columns["WhsName"].Hidden = true;
            e.Layout.Bands[0].Columns["Nombre"].Hidden = true;
            e.Layout.Bands[0].Columns["DocDate"].Hidden = true;
            e.Layout.Bands[0].Columns["UpdateDate"].Hidden = true;

            e.Layout.Bands[0].Columns["IdConteo"].Header.Caption = "Folio";
            e.Layout.Bands[0].Columns["CommentH"].Header.Caption = "Comentarios cabecera";
            e.Layout.Bands[0].Columns["WhsCode"].Header.Caption = "Código de almacén";
            e.Layout.Bands[0].Columns["WhsName"].Header.Caption = "Nombre de almacén";
            e.Layout.Bands[0].Columns["Nombre"].Header.Caption = "Usuario";
            e.Layout.Bands[0].Columns["DocDate"].Header.Caption = "Fecha de creación";
            e.Layout.Bands[0].Columns["UpdateDate"].Header.Caption = "Fecha de actualización";

            e.Layout.Bands[0].Columns["Inventario teorico"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Inventario fisico"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Diferencia"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Inventario teorico"].Format = "N0";
            e.Layout.Bands[0].Columns["Inventario fisico"].Format = "N0";
            e.Layout.Bands[0].Columns["Diferencia"].Format = "N0";

            e.Layout.Bands[0].Columns["Costo Promedio"].Format = "C1";
            e.Layout.Bands[0].Columns["Importe basado en Inv Teorico  por costo promedio"].Format = "C1";
            e.Layout.Bands[0].Columns["Importe basado en Inv Fisico por  costo promedio"].Format = "C1";
            e.Layout.Bands[0].Columns["Diferencia en importe por costo promedio"].Format = "C1";
            e.Layout.Bands[0].Columns["Costo Promedio"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Importe basado en Inv Teorico  por costo promedio"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Importe basado en Inv Fisico por  costo promedio"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Diferencia en importe por costo promedio"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            e.Layout.Bands[0].Columns["Eficiencia de Importe de Articulos por Linea"].Format = "P1";
            e.Layout.Bands[0].Columns["Eficiencia de articulos por línea"].Format = "P1";
            e.Layout.Bands[0].Columns["Eficiencia de Importe de Articulos por Linea"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Eficiencia de articulos por línea"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
    }
}
