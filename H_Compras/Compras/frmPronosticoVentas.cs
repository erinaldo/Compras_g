using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.Compras
{
    public partial class frmPronosticoVentas : Constantes.frmEmpty
    {
        string ItemCode;
        string WhsCode;
        Infragistics.Win.UltraWinGrid.UltraGrid ventasOriginal;

        public frmPronosticoVentas(string _itemCode, string _whsCode, Infragistics.Win.UltraWinGrid.UltraGrid tbl)
        {
            InitializeComponent();

            ItemCode = _itemCode;
            WhsCode = _whsCode;
            ventasOriginal = tbl;
        }

        private void frmPronosticoVentas_Load(object sender, EventArgs e)
        {
            nuevoToolStripButton.Enabled = false;
            exportarToolStripButton.Enabled = false;
            actualizarToolStripButton.Enabled = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Ideal", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 1);
                        command.Parameters.AddWithValue("@ItemCode", ItemCode);
                        command.Parameters.AddWithValue("@WhsCode", "27");

                        DataTable table = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        
                        da.Fill(table);

                        dgvDatos.DataSource = table;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes != MessageBox.Show("Apartir de ahora el ideal se generará con el pronostico de ventas ingresado\r\n, ¿Desea continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    return;

                DateTime Fecha = Convert.ToDateTime(Convert.ToDateTime(dgvDatos.Rows[0].Cells["Mes"].Value).Year + "-" + Convert.ToDateTime(dgvDatos.Rows[0].Cells["Mes"].Value).Month + "-01");
                foreach (var item in dgvDatos.Rows[0].Cells)
                {
                    if (item.Column.Index > 2)
                    {
                        using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                        {
                            using (SqlCommand commnad = new SqlCommand("sp_Ideal", connection))
                            {
                                commnad.CommandType = CommandType.StoredProcedure;
                                connection.Open();
                                commnad.Parameters.AddWithValue("@TipoConsulta", 2);

                                commnad.Parameters.AddWithValue("@ItemCode", dgvDatos.Rows[0].Cells["ItemCode"].Value);
                                commnad.Parameters.AddWithValue("@WhsCode", "27");
                                commnad.Parameters.AddWithValue("@Mes", Fecha.Month);
                                commnad.Parameters.AddWithValue("@Año", Fecha.Year);

                                commnad.Parameters.AddWithValue("@Mes1", item.Value);
                                commnad.ExecuteNonQuery();

                                Fecha = Fecha.AddMonths(1);
                            }
                        }
                    }
                }

                this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        public string MonthName(int month)
        {
            System.Globalization.DateTimeFormatInfo dtinfo = new System.Globalization.CultureInfo("es-ES", false).DateTimeFormat;
            return dtinfo.GetMonthName(month);
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Artículo";
                e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;

                e.Layout.Bands[0].Columns["Mes"].Hidden = true;
                e.Layout.Bands[0].Columns["WhsCode"].Hidden = true;

                DateTime Fecha = Convert.ToDateTime(Convert.ToDateTime(dgvDatos.Rows[0].Cells["Mes"].Value).Year + "-" + Convert.ToDateTime(dgvDatos.Rows[0].Cells["Mes"].Value).Month + "-01");

                foreach (var item in e.Layout.Bands[0].Columns)
                {

                    if (item.Index > 2)
                    {
                        if ((Fecha.Year == DateTime.Now.Year & Fecha.Month < DateTime.Now.Month) | DateTime.Now.Year < DateTime.Now.Year)
                            item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

                        item.Header.Caption = MonthName(Fecha.Month).Substring(0,3).ToLowerInvariant() + " " + Fecha.Year;
                        item.Width = 80;
                        item.Format = "N0";
                        item.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                        Fecha = Fecha.AddMonths(1);

                        foreach (var original in ventasOriginal.DisplayLayout.Bands[0].Columns)
                        {
                           if (item.Header.Caption.ToLower().Equals(original.Header.Caption.ToLower()))
                           {
                               if (Convert.ToDecimal(dgvDatos.Rows[0].Cells[item.Index].Value == DBNull.Value ? 0 : dgvDatos.Rows[0].Cells[item.Index].Value)
                                   != Convert.ToDecimal(ventasOriginal.Rows[0].Cells[original.Index].Value == DBNull.Value ? 0 : ventasOriginal.Rows[0].Cells[original.Index].Value))
                               {
                                   item.Header.Appearance.BackColor = Color.Yellow;
                                   item.Header.Appearance.BackColor2 = Color.Yellow;
                               }
                           }
                        }

                    }

                }
            }
            catch (Exception)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes != MessageBox.Show("Se eliminará el pronostico de ventas,\r\n ¿Desea continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    return;

                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Ideal", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 3);
                        command.Parameters.AddWithValue("@ItemCode", ItemCode);
                        command.Parameters.AddWithValue("@WhsCode", "27");
                        connection.Open();
                        command.ExecuteNonQuery();

                        this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                    }
                }
            }
            catch (Exception)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.No;
            }

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvDatos.Rows.Count == 0)
            {
                dgvDatos.Rows.Band.AddNew();
                dgvDatos.Rows[0].Cells["ItemCode"].Value = ItemCode;
                dgvDatos.Rows[0].Cells["WhsCode"].Value = WhsCode;
                dgvDatos.Rows[0].Cells["Mes"].Value = DateTime.Now;
            }
               
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Este proceso puede tardar varios segundos, ¿desea continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == System.Windows.Forms.DialogResult.Yes)

                    using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("sp_Ideal", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@TipoConsulta", 5);
                            command.Parameters.AddWithValue("@ItemCode", ItemCode);

                            connection.Open();
                            command.ExecuteNonQuery();

                            this.SetMensaje("Listo.", 5000, Color.Green, Color.Black);
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
