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
    public partial class frmConfiguracion : Constantes.frmEmpty
    {
        private Capture capture = null;
        private Filters filters = new Filters();

        public frmConfiguracion()
        {
            InitializeComponent();
        }

        private void frmConfiguracion_Load(object sender, EventArgs e)
        {
            nuevoToolStripButton.Enabled = false;
            guardarToolStripButton.Enabled = false;
            exportarToolStripButton.Enabled = false;
            actualizarToolStripButton.Enabled = false;

            #region Cargar dispositivos
            Filter videoDevice = null;
            Filter f;

            if (capture != null)
                videoDevice = capture.VideoDevice;
            cbDispositivos.Items.Clear();
            cbDispositivos.Items.Add("None");

            DataTable devices = new DataTable();
            devices.Columns.Add("Name", typeof(string));
            devices.Columns.Add("ID", typeof(string));

            for (int c = 0; c < filters.VideoInputDevices.Count; c++)
            {
                f = filters.VideoInputDevices[c];

                DataRow row = devices.NewRow();
                row["Name"] = f.Name;
                row["ID"] = f.MonikerString;
                devices.Rows.Add(row);
            }

            cbDispositivos.DataSource = devices;
            cbDispositivos.DisplayMember = "Name";
            cbDispositivos.ValueMember = "ID";

            cbDispositivos.Enabled = (filters.VideoInputDevices.Count > 0);

            cbDispositivos.SelectedIndex = 0;
            #endregion

            #region sizes
            try
            {
                cbResolucion.Items.Clear();
                //Size frameSize = capture.FrameSize;
                cbResolucion.Items.Add("160 x 120");
                cbResolucion.Items.Add("320 x 240");
                cbResolucion.Items.Add("640 x 480");
                cbResolucion.Items.Add("1024 x 768");
                cbResolucion.SelectedIndex = 0;
            }
            catch { }
            #endregion

            #region llenar grid
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

                    ultraGrid1.DataSource = table;

                }
            }
            #endregion
        }

        private void button2_Click(object sender, EventArgs e)
        {
            #region Señal
            try
            {
                if (capture == null)
                    throw new ApplicationException("Please select a video and/or audio device.");
                if (!capture.Cued)
                {
                    //capture.VideoCompressor = Filters.VideoCompressors[0];
                    //capture.AudioCompressor = Filters.AudioCompressors[0];

                    // capture.FrameRate = 29.997;                 // NTSC

                    //capture.Filename = @"c:\pp\pe.mp4";
                }
                capture.Cue();

                MessageBox.Show("Listo");

                capture.PreviewWindow = panelVideo;

                capture.Start();
                System.Threading.Thread.Sleep(1000);
                capture.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.ToString());
            }
            #endregion
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (capture == null)
                    throw new ApplicationException("Please select a video and/or audio device.");
                capture.Stop();
                capture.PreviewWindow = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "Insert into tbl_VideoConfig (Dispositivo, Resolucion, Compressor, Carpeta, PC, NameID) values (@Dispositivo, @Resolucion, @Compressor, @Carpeta, @PC, @NameID)";
                    command.Parameters.AddWithValue("@Dispositivo", cbDispositivos.SelectedValue);
                    command.Parameters.AddWithValue("@Resolucion", cbResolucion.Text);
                    command.Parameters.AddWithValue("@Compressor", cbCompresors.Text);
                    command.Parameters.AddWithValue("@Carpeta", txtPath.Text);
                    command.Parameters.AddWithValue("@PC", Environment.MachineName);
                    command.Parameters.AddWithValue("@NameID", cbDispositivos.Text);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            #region llenar grid
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

                    ultraGrid1.DataSource = table;

                }
            }
            #endregion
        }

        private void ultraGrid1_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
        }

        private void cbDispositivos_SelectionChangeCommitted(object sender, EventArgs e)
        {
            #region Seleccionar dispositivo
            try
            {

                Filter videoDevice = null;
                Filter audioDevice = null;
                if (capture != null)
                {
                    videoDevice = capture.VideoDevice;
                    audioDevice = capture.AudioDevice;
                    capture.Dispose();
                    capture = null;
                }

                // Get new video device
                foreach (Filter item in filters.VideoInputDevices)
                {
                    string aux = cbDispositivos.SelectedValue.ToString();//.Substring(0, cbDispositivos.Text.Length - 2);
                    if (item.MonikerString.Equals(aux))
                        videoDevice = item;
                }

                // Create capture object
                if ((videoDevice != null) || (audioDevice != null))
                {
                    capture = new Capture(videoDevice, audioDevice);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dispositivo de video no soportado.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
            #endregion

            cbCompresors.Items.Clear();

            foreach (Filter item in filters.VideoCompressors)
            {
                cbCompresors.Items.Add(item.Name);
            }
        }

        private void cbResolucion_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                // Disable preview to avoid additional flashes (optional)
                bool preview = (capture.PreviewWindow != null);
                capture.PreviewWindow = null;

                // Update the frame size
                //MenuItem m = sender as MenuItem;
                string[] s = cbResolucion.Text.Split('x');
                Size size = new Size(int.Parse(s[0]), int.Parse(s[1]));
                capture.FrameSize = size;

                // Restore previous preview setting
                capture.PreviewWindow = (preview ? panelVideo : null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Frame size not supported.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void cbCompresors_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                // Disable preview to avoid additional flashes (optional)
                bool preview = (capture.PreviewWindow != null);
                capture.PreviewWindow = null;

                // Update the frame size
                //MenuItem m = sender as MenuItem;
                capture.VideoCompressor = filters.VideoCompressors[cbCompresors.SelectedIndex];

                // Restore previous preview setting
                capture.PreviewWindow = (preview ? panelVideo : null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Frame size not supported.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            if (panel1.Dock != DockStyle.Fill)
                panel1.Dock = DockStyle.Fill;
            else
                panel1.Dock = DockStyle.None;
        }
    }
}
