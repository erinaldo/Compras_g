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


namespace H_Compras.Reparto
{
    public partial class FrmDrawRutas : Form
    {
        bool bCarga = false;
        ClasesSGUV.Logs log;
        public FrmDrawRutas()
        {
            InitializeComponent();
        }

        private void FrmDrawRutas_Load(object sender, EventArgs e)
        {
            try
            {
                log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                int internetExplorerMajorVersion;
                internetExplorerMajorVersion = LogisticaTransportes.InternetExplorerBrowserEmulation.GetInternetExplorerMajorVersion();
                string version = internetExplorerMajorVersion.ToString(CultureInfo.InvariantCulture);
                lblVersionIE.Text = "vInternet Explorer = " + version;

                bCarga = true;
                cboSucursal.DataSource = ConsultaSucursales();
                cboSucursal.ValueMember = "Codigo";
                cboSucursal.DisplayMember = "Nombre";
                cboSucursal.SelectedIndex = -1;
                bCarga = false;
                cboSucursal.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnDrawRutas_Click(object sender, EventArgs e)
        {
            try
            {
                string url_mapa = "";

                url_mapa = "http://192.168.2.100:70/operacion/CodigoRuta?Opc=3" + "&s=" + cboSucursal.SelectedValue.ToString() + "&r=" + cboRuta.SelectedValue.ToString();

                if (!chbExplorador.Checked)
                {
                   
                    Uri direccion = new Uri(url_mapa);
                    webBrowser1.ScriptErrorsSuppressed = true;
                    webBrowser1.Navigate(new Uri(url_mapa));
                    
                    
                }
                else
                {
                    try
                    {
                        System.Diagnostics.Process.Start(url_mapa);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message.ToString()); }
 
                }

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

                        command.Parameters.AddWithValue("@TipoConsulta", 18);
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

                    cboRuta.DataSource = ConsultaRutasSucursal(cboSucursal.SelectedValue.ToString());
                    cboRuta.ValueMember = "Codigo";
                    cboRuta.DisplayMember = "Nombre";
                    //cboRuta.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private DataTable ConsultaRutasSucursal(string Sucursal)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 19);
                        command.Parameters.AddWithValue("@Sucursal", Sucursal);
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
                        LogisticaTransportes.BrowserEmulationVersion version;
                        version = (LogisticaTransportes.BrowserEmulationVersion)Convert.ToInt32((sender as RadioButton).Tag);
                        if (LogisticaTransportes.InternetExplorerBrowserEmulation.GetBrowserEmulationVersion() != version)
                        {
                            // apply the new emulation version
                            if (!LogisticaTransportes.InternetExplorerBrowserEmulation.SetBrowserEmulationVersion(version))
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
                //if ((sender as RadioButton).Checked)
                //{
                //    if (MessageBox.Show("Para poder aplicar el cambio de version se forzará a reiniciar HalcoNet, se perderán cambios que no se hayan guardado. seguro que desea continuar?", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                //    {
                //        LogisticaTransportes.BrowserEmulationVersion version;
                //        version = (LogisticaTransportes.BrowserEmulationVersion)Convert.ToInt32((sender as RadioButton).Tag);
                //        if (LogisticaTransportes.InternetExplorerBrowserEmulation.GetBrowserEmulationVersion() != version)
                //        {
                //            // apply the new emulation version
                //            if (!LogisticaTransportes.InternetExplorerBrowserEmulation.SetBrowserEmulationVersion(version))
                //            {
                //                MessageBox.Show("Error al actualizar la versión de emulación del navegador.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //            }
                //            else
                //            {
                //                Application.Restart();
                //                Environment.Exit(-1);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        (sender as RadioButton).Checked = false;
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void FrmDrawRutas_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void FrmDrawRutas_FormClosing(object sender, FormClosingEventArgs e)
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
