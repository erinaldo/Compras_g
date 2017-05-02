using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace H_Compras.LogisticaTransportes
{
    public partial class FrmClientesReparto : Form
    {
        bool bCarga = false;
        ClasesSGUV.Logs log;
        public FrmClientesReparto()
        {
            InitializeComponent();
        }

        private void FrmClientesReparto_Load(object sender, EventArgs e)
        {
            try
            {
                int internetExplorerMajorVersion;
                internetExplorerMajorVersion = InternetExplorerBrowserEmulation.GetInternetExplorerMajorVersion();
                string version = internetExplorerMajorVersion.ToString(CultureInfo.InvariantCulture);
                lblVersionIE.Text = "Version Sugerida - Internet Explorer = v" + version;

                bCarga = true;
                cboSucursal.DataSource = ConsultaSucursales();
                cboSucursal.ValueMember = "Codigo";
                cboSucursal.DisplayMember = "Nombre";
                cboSucursal.SelectedIndex = -1;
                bCarga = false;
                log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnConsultarPuntos_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboSucursal.SelectedIndex == -1)
                {
                    MessageBox.Show("Debe especificar al menos una sucursal", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cboSucursal.Focus();
                    return;
                }
                string url_mapa = "";
                if (cboSucursal.SelectedIndex != -1 && cboVendedor.SelectedIndex == -1)
                {
                    url_mapa = "http://192.168.2.100:70/operacion/puntos?Opc=1" + "&Suc=" + cboSucursal.SelectedValue.ToString() + "&Vdr=-1";
                }
                else if (cboSucursal.SelectedIndex != -1 && cboVendedor.SelectedIndex != -1)
                {
                    url_mapa = "http://192.168.2.100:70/operacion/puntos?Opc=2" + "&Suc=" + cboSucursal.SelectedValue.ToString() + "&Vdr=" + cboVendedor.SelectedValue.ToString();
                }

                //StringBuilder sb = new StringBuilder();
                //sb.Append("<html>");
                //sb.Append("<head>");
                //sb.Append("</head");
                //sb.Append("<body>");

                ////WebBrowser webBrowser1 = (WebBrowser)ugb.Controls.Find("webbs", false).FirstOrDefault();

                //sb.Append("<iframe src =" + url_mapa + " height=" + webBrowser1.Height + " width=" + webBrowser1.Width + "></iframe>");

                //sb.Append("</body");
                //sb.Append("</html>");
                //webBrowser1.DocumentText = sb.ToString();


                Uri direccion = new Uri(url_mapa);
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.Navigate(new Uri(url_mapa)); //  .Url = direccion;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private DataTable ConsultaSucursales()
        {
            DataTable dt = new DataTable();
            
            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 14);
                        command.CommandTimeout = 0;
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return dt;
        }

        private void cboSucursal_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboSucursal.SelectedIndex != -1 && !bCarga)
                {
                    cboVendedor.DataSource = ConsultaVendedoresSucursal(Convert.ToInt32(cboSucursal.SelectedValue));
                    cboVendedor.ValueMember = "Codigo";
                    cboVendedor.DisplayMember = "Nombre";
                    cboVendedor.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private DataTable ConsultaVendedoresSucursal(int Sucursal)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 15);
                        command.Parameters.AddWithValue("@NSucursal", Sucursal);
                        command.CommandTimeout = 0;
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return dt;
        }

        private void rbt_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if ((sender as RadioButton).Checked)
                {
                    if (MessageBox.Show("Esta apunto de especificar una version a utilizar del navegador para mostrar las ubicaciones de los clientes, seguro que desea continuar?", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                    {
                        BrowserEmulationVersion version;
                        version = (BrowserEmulationVersion)Convert.ToInt32((sender as RadioButton).Tag);
                        if (InternetExplorerBrowserEmulation.GetBrowserEmulationVersion() != version)
                        {
                            // apply the new emulation version
                            if (!InternetExplorerBrowserEmulation.SetBrowserEmulationVersion(version))
                            {
                                MessageBox.Show("Error al actualizar la versión de emulación del navegador.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            else
                            {
                                MessageBox.Show("Cambio de versión aplicado, debe salir completamente del sistema y volver a iniciar para que los cambios se vean reflejados", "Informe Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //Application.Restart();
                                //Environment.Exit(-1);
                            }
                        }
                    }
                    else
                    {
                        (sender as RadioButton).Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            
        }

        private void FrmClientesReparto_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void FrmClientesReparto_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                log.Fin();
            }
            catch (Exception)
            {

            }
        }    
    }
}
