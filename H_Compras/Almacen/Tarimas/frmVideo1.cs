using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DirectX.Capture;

namespace H_Compras.Almacen.Tarimas
{
    public partial class frmVideo1 : Constantes.frmEmpty
    {
        private List<Capture> captureList = null;
        private Filters filters = new Filters();
        DataTable tblTraspasos;
        private int ID;
        DateTime HoraInicio;

        public frmVideo1()
        {
            InitializeComponent();

            tblTraspasos = new DataTable();
            tblTraspasos.Columns.Add("Traspaso", typeof(Int32));
            tblTraspasos.Columns.Add("Origen", typeof(string));
            tblTraspasos.Columns.Add("Destino", typeof(string));

            dgvDatos.DataSource = tblTraspasos;
            captureList = new List<Capture>();
        }

        private void frmVideo1_Load(object sender, EventArgs e)
        {
            // nuevoToolStripButton.Enabled = false;
            guardarToolStripButton.Enabled = false;
            exportarToolStripButton.Enabled = false;
            actualizarToolStripButton.Enabled = false;

            nuevoToolStripButton.Click -= new EventHandler(btnNuevo_Click);
            nuevoToolStripButton.Click += new EventHandler(btnNuevo_Click);

            ClasesSGUV.Form.ControlsForms.setDataSource(cbAlmacen, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesVenta, null, string.Empty), "WhsName", "WhsCode", string.Empty);

        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            txtTarima.Clear();
            txtTraspaso.Clear();
            dtFecha.Value = DateTime.Now;
            dgvDatos.DataSource = null;
            btnGrabar.Enabled = false;
            btnStop.Enabled = false;
            btGuardar.Enabled = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("Select DocNum, Filler, ToWhsCode From [SBO-DistPJ].dbo.OWTR Where DocNum = @DocNum"))
                {
                    command.Connection = connection;
                    command.Parameters.AddWithValue("@DocNum", txtTraspaso.Text);

                    DataTable tbl = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl);
                    if (tbl.Rows.Count <= 0)
                    {
                        MessageBox.Show("El Número de traspaso " + txtTraspaso.Text + " no existe.");
                        return;
                    }

                    DataRow row = tblTraspasos.NewRow();
                    row["Traspaso"] = tbl.Rows[0].Field<Int32>("DocNum");
                    row["Origen"] = tbl.Rows[0].Field<string>("Filler");
                    row["Destino"] = tbl.Rows[0].Field<string>("ToWhsCode");
                    tblTraspasos.Rows.Add(row);
                    txtTraspaso.Text = string.Empty;
                }
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            captureList.Clear();
            if (string.IsNullOrEmpty(txtTarima.Text))
            {
                MessageBox.Show("Ingresa el número de tarima");
                return;
            }

            btnGrabar.Enabled = false;
            btnStop.Enabled = true;

            #region Dispositivos configurados
            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "Select * From tbl_VideoConfig Where PC =  @PC";
                    command.Parameters.AddWithValue("@PC", Environment.MachineName);
                    DataTable table = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(table);
                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("No tiene dispositivos configurados");
                        return;
                    }

                    foreach (DataRow deviceItem in table.Rows)
                    {
                        #region Seleccionar dispositivo
                        Capture captureD = null;
                        try
                        {

                            Filter videoDevice = null;
                            Filter audioDevice = null;
                            if (captureD != null)
                            {
                                videoDevice = captureD.VideoDevice;
                                audioDevice = captureD.AudioDevice;
                                captureD.Dispose();
                                captureD = null;
                            }

                            foreach (Filter item in filters.VideoInputDevices)
                            {
                                if (item.MonikerString.Equals(deviceItem["Dispositivo"].ToString()))
                                    videoDevice = item;
                            }

                            if ((videoDevice != null) || (audioDevice != null))
                            {
                                captureD = new Capture(videoDevice, audioDevice);
                            }

                            foreach (Filter item in filters.VideoCompressors)
                            {
                                if (item.Name.Equals(deviceItem["Compressor"].ToString()))
                                    captureD.VideoCompressor = item;
                            }
                            string name = DateTime.Now.ToShortDateString().Replace("/", "") + " " + DateTime.Now.ToShortTimeString();
                            Random rand = new Random();
                            name = name.Replace(".", "").Replace(" ", "").Replace(":", "") + (rand.NextDouble() * 100).ToString("N0");
                            captureD.Filename = deviceItem["Carpeta"].ToString() + name + ".mp4";

                            captureList.Add(captureD);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Dispositivo de video no soportado.\n\n" + ex.Message + "\n\n" + ex.ToString());
                        }
                        #endregion
                    }

                    foreach (Capture item in captureList)
                    {
                        try
                        {
                            if (item == null)
                                continue;
                            if (!item.Cued) { }
                            item.Cue();

                            item.Start();
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
            #endregion

            HoraInicio = DateTime.Now;
        }

        private void btGuardar_Click(object sender, EventArgs e)
        {
            btnGrabar.Enabled = true;

            if (string.IsNullOrEmpty(txtTarima.Text))
            {
                MessageBox.Show("Ingresa el número de tarima");
                return;
            }

            if (string.IsNullOrEmpty(maskedTextBox1.Text))
            {
                MessageBox.Show("Ingresa fecha de solicitud");
                return;
            }

            if (cbAlmacen.SelectedIndex == 0)
            {
                MessageBox.Show("Selecciona un almacén");
                return;
            }

            //if (dgvDatos.Rows.Count == 0)
            //{
            //    MessageBox.Show("Ingresa al menos un número de trapaso");
            //    return;
            //}

            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = @"DECLARE @ID INT = ISNULL((Select MAX(ID) From tbl_Tarimas), 0) + 1
                                            IF NOT EXISTS (Select * From tbl_Tarimas Where NumTarima = @numTarima AND Fecha = CAST(@Fecha as DATE) AND WhsCode = @WhsCode)
                                            BEGIN INSERT INTO tbl_Tarimas(ID, NumTarima, Fecha, FechaSolicitud, WhsCode, Comment) VALUES (@ID, @numTarima, @Fecha, @FechaSolicitud, @WhsCode, @Comment) END
                                            ELSE BEGIN UPDATE tbl_Tarimas SET UpdateDate = GETDATE(), Comment = @Comment WHERE NumTarima = @numTarima AND Fecha = CAST(@Fecha as DATE) AND WhsCode = @WhsCode END ";

                    command.Parameters.AddWithValue("@numTarima", txtTarima.Text);
                    command.Parameters.AddWithValue("@Fecha", dtFecha.Value);
                    command.Parameters.AddWithValue("@FechaSolicitud", maskedTextBox1.Text);
                    command.Parameters.AddWithValue("@WhsCode", cbAlmacen.SelectedValue);
                    command.Parameters.AddWithValue("@Comment", txtComment.Text);

                    connection.Open();
                    ID = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            /*
            foreach (DataRow item in tblTraspasos.Rows)
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;

                        command.CommandText = @"DECLARE @ID INT = ISNULL((Select MAX(ID) From tbl_TarimaTraspaso), 0) + 1
                                            IF NOT EXISTS (Select * From tbl_TarimaTraspaso Where NumTraspaso = @traspaso AND IdTarima = @idTarima)
                                            BEGIN INSERT INTO tbl_TarimaTraspaso(ID, idTarima, NumTraspaso) VALUES (@ID, @idTarima, @traspaso) END";
                        command.Parameters.AddWithValue("@traspaso", item.Field<Int32>("Traspaso"));
                        command.Parameters.AddWithValue("@idTarima", ID);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }*/
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnGrabar.Enabled = true;
            btnStop.Enabled = false;

            foreach (Capture item in captureList)
            {
                try
                {
                    if (item == null)
                        continue;

                    item.Stop();
                }
                catch (Exception)
                {
                }

                try
                {
                    using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;

                            command.CommandText = @"DECLARE @ID INT = ISNULL((Select MAX(ID) From tbl_tarimaVideo), 0) + 1
                                            INSERT INTO tbl_TarimaVideo(ID, idTarima, Path_Video, Nombre, HoraFin, HoraInicio) 
                                            VALUES (@ID, @idTarima, @Path, @Nombre, GETDATE(), @Inicio);";
                            command.Parameters.AddWithValue("@idTarima", ID);
                            command.Parameters.AddWithValue("@Path", item.Filename);
                            command.Parameters.AddWithValue("@Inicio", HoraInicio);
                            command.Parameters.AddWithValue("@Nombre", System.IO.Path.GetFileName(item.Filename));

                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            frmConfiguracion form = new frmConfiguracion();
            form.ShowDialog();
        }

        private void dgvDatos_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
        {
            try
            {
                foreach (Infragistics.Win.UltraWinGrid.UltraGridRow item in e.Rows)
                {
                    using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;

                            command.CommandText = @"Delete From tbl_TarimaTraspaso Where IdTarima = @id AND NumTraspaso = @traspaso";
                            command.Parameters.AddWithValue("@id", ID);
                            command.Parameters.AddWithValue("@traspaso", item.Cells["Traspaso"].Value);

                            connection.Open();

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
        }

        private void txtTarima_Leave(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;

//                        command.CommandText = @" Select NumTraspaso, Filler, ToWhsCode, C.ID
//                                                    From tbl_TarimaTraspaso A 
//                                                        Join [SBO-DistPJ].dbo.OWTR B on A.NumTraspaso = B.DocNum 
//                                                        Join tbl_Tarimas C on C.Id = A.IdTarima
//                                                    Where C.NumTarima = @Tarima AND Fecha  = CAST(@Fecha AS DATE)";
                        command.CommandText = @"Select * From tbl_Tarimas Where NumTarima = @Tarima AND Fecha = CAST(@Fecha AS DATE)";
                        command.Parameters.AddWithValue("@Tarima", txtTarima.Text);
                        command.Parameters.AddWithValue("@Fecha", dtFecha.Value);

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        DataTable tbl = new DataTable();
                        da.Fill(tbl);

                        if (tbl.Rows.Count > 0)
                        {
                            btnGrabar.Enabled = true;
                            btnStop.Enabled = false;
                            cbAlmacen.SelectedValue = tbl.Rows[0]["Whscode"];
                            txtComment.Text = tbl.Rows[0]["Comment"].ToString();
                            maskedTextBox1.Text = tbl.Rows[0]["FechaSolicitud"].ToString();

                            /*foreach (DataRow item in tbl.Rows)
                            {
                                DataRow row = tblTraspasos.NewRow();
                                row["Traspaso"] = item["NumTraspaso"];
                                row["Origen"] = item["Filler"];
                                row["Destino"] = item["ToWhsCode"];
                                ID = Convert.ToInt32(item["ID"]);
                                tblTraspasos.Rows.Add(row);
                            }*/
                        }
                        else
                        {
                            //btnGrabar.Enabled = true;
                            btnStop.Enabled = false;
                            tblTraspasos.Rows.Clear();
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void maskedTextBox1_Validating(object sender, CancelEventArgs e)
        {
            DateTime fecha;

            try
            {
                fecha = Convert.ToDateTime((sender as MaskedTextBox).Text);
            }
            catch (Exception)
            {
                this.SetMensaje("Fecha es incorrecta", 5000, Color.Red, Color.White);
                (sender as MaskedTextBox).Clear();
                e.Cancel = true;
            }




        }
    }
}
