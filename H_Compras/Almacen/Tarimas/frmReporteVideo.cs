using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.Almacen.Tarimas
{
    public partial class frmReporteVideo : Constantes.frmEmpty
    {
        public frmReporteVideo()
        {
            InitializeComponent();
        }

        private void frmReporteVideo_Load(object sender, EventArgs e)
        {
            nuevoToolStripButton.Enabled = false;
            guardarToolStripButton.Enabled = false;
            exportarToolStripButton.Enabled = false;
            actualizarToolStripButton.Enabled = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = @"Select A.ID, A.Fecha, A.NumTarima, '''' NumTraspaso, '''' [Origen], F.WhsName [Destino]
                                                From tbl_Tarimas A 
	                                                left join tbl_TarimaVideo D ON A.Id = D.IdTarima
	                                                join [SBO-DistPJ].dbo.OWHS F on A.WhsCode = F.WhsCode
                                                Where 
	                                                A.Fecha BETWEEN CAST(@Desde AS DATE) AND CAST(@Hasta AS DATE)";

                        command.Parameters.AddWithValue("@Desde", dtDesde.Value);
                        command.Parameters.AddWithValue("@Hasta", dtHasta.Value);

                        DataTable tbl = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(tbl);

                        dgvDatos.DataSource = tbl;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }

            e.Layout.Bands[0].Columns[0].Hidden = true;
        }

        private void dgvDatos_AfterRowActivate(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = @"Select Path_Video, Nombre, HoraInicio, HoraFin, '' Rep, '' Down From tbl_tarimaVideo Where idTarima = @ID";

                        command.Parameters.AddWithValue("@ID", dgvDatos.ActiveRow.Cells["ID"].Value);

                        DataTable tbl = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(tbl);

                        dgvVideos.DataSource = tbl;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvVideos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
            e.Layout.Bands[0].Columns[0].Hidden = true;
            e.Layout.Bands[0].Columns[1].Width = 180;

            e.Layout.Bands[0].Columns[4].Header.Caption = string.Empty;
            e.Layout.Bands[0].Columns[5].Header.Caption = string.Empty;

            e.Layout.Bands[0].Columns[4].Width = 30;
            e.Layout.Bands[0].Columns[5].Width = 30;
        }

        private void dgvVideos_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            try
            {
                int i = e.Row.Band.Index;
                if (i == 0)
                {

                    e.Row.Cells["Rep"].ButtonAppearance.Image = Properties.Resources.play;
                    e.Row.Cells["Rep"].ButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    e.Row.Cells["Rep"].ButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    e.Row.Cells["Rep"].Appearance.ImageBackground = Properties.Resources.play;
                    e.Row.Cells["Rep"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    e.Row.Cells["Rep"].ButtonAppearance.BorderColor = Color.CadetBlue;

                    e.Row.Cells["Down"].ButtonAppearance.Image = Properties.Resources.Cloud_Download_Off;
                    e.Row.Cells["Down"].ButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    e.Row.Cells["Down"].ButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    e.Row.Cells["Down"].Appearance.ImageBackground = Properties.Resources.Cloud_Download_Off;
                    e.Row.Cells["Down"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    e.Row.Cells["Down"].ButtonAppearance.BorderColor = Color.CadetBlue;
                }
            }
            catch (Exception)
            {
            }
        }

        private void dgvVideos_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            try
            {
                if (e.Cell.Row.Index > -1 && e.Cell.Band.Index == 0)
                {
                    if (e.Cell.Column.Index == 4)
                    {
                        System.Diagnostics.Process.Start(dgvVideos.Rows[e.Cell.Row.Index].Cells[0].Value.ToString());
                    }
                    else if (e.Cell.Column.Index == 5)
                    {
                        DialogResult r = folderBrowserDialog1.ShowDialog();
                        if (r == System.Windows.Forms.DialogResult.OK)
                        {
                            System.IO.File.Copy(dgvVideos.Rows[e.Cell.Row.Index].Cells[0].Value.ToString(), folderBrowserDialog1.SelectedPath + "\\" + dgvVideos.Rows[e.Cell.Row.Index].Cells[1].Value.ToString());
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("El archivo no esta disponible.");
            }
        }
    }
}
