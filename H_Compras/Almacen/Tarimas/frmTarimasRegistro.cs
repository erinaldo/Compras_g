using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

using Microsoft.Expression.Encoder.Devices;
using Microsoft.Expression.Encoder.Live;
using Microsoft.Expression.Encoder;

namespace H_Compras.Almacen.Tarimas
{
    public partial class frmTarimasRegistro : Constantes.frmEmpty
    {
        private int ID;
        private List<ObjVideo> ListaDispositivos = new List<ObjVideo>();

        #region metodos
        private void GetSelectedVideoAndAudioDevices(out EncoderDevice video, out EncoderDevice audio, string ID)
        {
            video = null;
            audio = null;

            if (string.IsNullOrEmpty(ID))
            {
                MessageBox.Show("No Video and Audio capture devices have been selected.\nSelect an audio and video devices from the listboxes and try again.", "Warning");
                return;
            }

            // Get the selected video device            
            foreach (EncoderDevice edv in EncoderDevices.FindDevices(EncoderDeviceType.Video))
            {
                if (String.Compare(edv.DevicePath, ID) == 0)
                {

                    video = edv;
                    break;
                }
            }
        }

        void StopJob()
        {

        }
        #endregion

        public frmTarimasRegistro()
        {
            InitializeComponent();
        }

        private void frmTarimasRegistro_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text += " - ver. " + Application.ProductVersion;
                DataTable tblDevices = new DataTable();
                tblDevices.Columns.Add("Nombre", typeof(string));
                tblDevices.Columns.Add("ID", typeof(string));
                tblDevices.Columns.Add("Estado", typeof(string));

                foreach (EncoderDevice edv in EncoderDevices.FindDevices(EncoderDeviceType.Video))
                {
                    DataRow row = tblDevices.NewRow();
                    row["Nombre"] = edv.Name;
                    row["ID"] = edv.DevicePath;

                    tblDevices.Rows.Add(row);
                }

                ugDispositivos.DataSource = tblDevices;

                ClasesSGUV.Form.ControlsForms.setDataSource(cbAlmacen, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesVenta, null, string.Empty), "WhsName", "WhsCode", "----Almacén----");
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void btGuardar_Click(object sender, EventArgs e)
        {
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

            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = @"DECLARE @ID INT = ISNULL((Select MAX(ID) From tbl_Tarimas), 0) + 1
                                            IF NOT EXISTS (Select * From tbl_Tarimas Where NumTarima = @numTarima AND Fecha = CAST(@Fecha as DATE) AND WhsCode = @WhsCode)
                                            BEGIN INSERT INTO tbl_Tarimas(ID, NumTarima, Fecha, FechaSolicitud, WhsCode, Comment, FromWhsCode) VALUES (@ID, @numTarima, @Fecha, @FechaSolicitud, @WhsCode, @Comment, @FromWhsCode) END
                                            ELSE BEGIN UPDATE tbl_Tarimas SET UpdateDate = GETDATE(), Comment = @Comment WHERE NumTarima = @numTarima AND Fecha = CAST(@Fecha as DATE) AND WhsCode = @WhsCode END

                                             SELECT @ID ";

                    command.Parameters.AddWithValue("@numTarima", txtTarima.Text);
                    command.Parameters.AddWithValue("@Fecha", dtFecha.Value);
                    command.Parameters.AddWithValue("@FechaSolicitud", maskedTextBox1.Text);
                    command.Parameters.AddWithValue("@WhsCode", cbAlmacen.SelectedValue);
                    command.Parameters.AddWithValue("@Comment", txtComment.Text);
                    command.Parameters.AddWithValue("@FromWhsCode", ClasesSGUV.Login.Almacen);

                    connection.Open();
                    ID = Convert.ToInt32(command.ExecuteScalar());

                    btnGrabar.Enabled = true;
                }
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            if (ID <= 0) return;

            btnGrabar.Enabled = false;
            btnDetener.Enabled = true;

            try
            {
                ListaDispositivos = new List<ObjVideo>();

                foreach (var item in ugDispositivos.Rows)
                {
                    item.Cells["Estado"].Value = "Grabando...";
                    ObjVideo dispositivo = new ObjVideo();

                    EncoderDevice video = null;
                    EncoderDevice audio = null;

                    GetSelectedVideoAndAudioDevices(out video, out audio, item.Cells["ID"].Value.ToString());

                    if (video == null)
                    {
                        return;
                    }

                    dispositivo.Job = new LiveJob();

                    if (!dispositivo.BStartedRecording)
                        if (video != null)
                        {
                            dispositivo.DeviceSource = dispositivo.Job.AddDeviceSource(video, audio);

                            dispositivo.DeviceSource.PickBestVideoFormat(new Size(1280, 720), 1);
                            SourceProperties sp = dispositivo.DeviceSource.SourcePropertiesSnapshot();
                            dispositivo.Job.OutputFormat.VideoProfile.Size = new Size(sp.Size.Width, sp.Size.Height);
                            dispositivo.Job.ActivateSource(dispositivo.DeviceSource);
                        }
                        else
                        {
                            MessageBox.Show("No Video/Audio capture devices have been found.", "Warning");
                        }

                    if (dispositivo.BStartedRecording)
                    {
                        dispositivo.Job.StopEncoding();
                        dispositivo.BStartedRecording = false;
                        video = null;
                        audio = null;
                    }
                    else
                    {
                        FileArchivePublishFormat fileOut = new FileArchivePublishFormat();
                        Random rnd = new Random();
                        dispositivo.FileName = "C:\\Video\\CAM{0:yyyyMMdd_hhmmss}-" + rnd.Next(10) + ".wmv";

                        fileOut.OutputFileName = String.Format(dispositivo.FileName, DateTime.Now);

                        dispositivo.FileName = fileOut.OutputFileName;

                        dispositivo.Job.PublishFormats.Add(fileOut);
                        dispositivo.Job.StartEncoding();

                        dispositivo.BStartedRecording = true;
                    }

                    dispositivo.Id = item.Cells["ID"].Value.ToString();
                    ListaDispositivos.Add(dispositivo);
                }
            }
            catch (Exception ex)
            {
                btnDetener.Enabled = true;
                btnGrabar.Enabled = false;
                ListaDispositivos.Clear();
                ListaDispositivos = null;

                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }

        }

        private void ugDispositivos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["Nombre"].Width = 160;
            e.Layout.Bands[0].Columns["ID"].Hidden = true;
        }

        private void btnDetener_Click(object sender, EventArgs e)
        {
            foreach (var item in ListaDispositivos)
            {
                #region Stop
                item.Job.StopEncoding();
                item.BStartedRecording = false;

                if (item.Job != null)
                {
                    if (item.BStartedRecording)
                    {
                    }

                    item.Job.StopEncoding();
                    item.Job.RemoveDeviceSource(item.DeviceSource);

                    item.DeviceSource.PreviewWindow = null;
                    item.DeviceSource = null;
                }
                foreach (var disp in ugDispositivos.Rows)
                {
                    if (disp.Cells["ID"].Value.ToString() == item.Id)
                        disp.Cells["Estado"].Value = "Detenido.";
                }
                #endregion 
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
                            command.Parameters.AddWithValue("@Path", item.FileName);
                            command.Parameters.AddWithValue("@Inicio", DateTime.Now);
                            command.Parameters.AddWithValue("@Nombre", System.IO.Path.GetFileName(item.FileName));

                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            ListaDispositivos.Clear();
            ListaDispositivos = null;

            btnGrabar.Enabled = true;
            btnDetener.Enabled = false;
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
                            btnDetener.Enabled = false;
                            ID = Convert.ToInt32(tbl.Rows[0]["ID"]);
                            cbAlmacen.SelectedValue = tbl.Rows[0]["Whscode"];
                            txtComment.Text = tbl.Rows[0]["Comment"].ToString();
                            maskedTextBox1.Text = tbl.Rows[0]["FechaSolicitud"].ToString();
                        }
                        else
                        {
                            btnDetener.Enabled = false;
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class ObjVideo
    {
        string _fileName;

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private LiveJob _job;

        public LiveJob Job
        {
            get { return _job; }
            set { _job = value; }
        }
        private bool _bStartedRecording = false;

        public bool BStartedRecording
        {
            get { return _bStartedRecording; }
            set { _bStartedRecording = value; }
        }
        private LiveDeviceSource _deviceSource;

        public LiveDeviceSource DeviceSource
        {
            get { return _deviceSource; }
            set { _deviceSource = value; }
        }

    }
}
