using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using System.Diagnostics;

namespace H_Compras.Logistica
{
    public partial class FrmAltaFacturas : Form
    {
        DataTable dtFcrs = new DataTable();
        ClasesSGUV.Logs log;

        public FrmAltaFacturas()
        {
            InitializeComponent();
        }

        private void FrmAltaFacturas_Load(object sender, EventArgs e)
        {
            try
            {
                log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                dtFcrs.Columns.Add("idRelacion", typeof(Int32));
                dtFcrs.Columns.Add("iFolio", typeof(Int32));
                dtFcrs.Columns.Add("iFactura", typeof(Int32));
                dtFcrs.Columns.Add("Cliente", typeof(string));
                dtFcrs.Columns.Add("Fecha", typeof(DateTime));
                dtFcrs.Columns.Add("Tipo", typeof(string));


                cboProveedor.DataSource = csLogistica.ConsultaCatalogo(20);
                cboProveedor.ValueMember = "Codigo";
                cboProveedor.DisplayMember = "Nombre";

            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        private void btnAgregarFactura_Click(object sender, EventArgs e)
        {
            string Tipo = ""; string Rubro = "Factura";
            if (rbtFactura.Checked)
                Tipo = "FA";
            else
            {
                Tipo = "OV";
                Rubro = "Orden de Venta";
            }
            DataTable dt = csLogistica.ExisteFactura(1, Convert.ToInt32(txtFacturasEnvio.Text), Tipo);
            if(dt.Rows.Count<=0)
            {
                MessageBox.Show("La " + Rubro + " especificada no existe", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataRow dr = dtFcrs.NewRow();
            dr["idRelacion"] = -1;
            dr["iFolio"] = -1;
            dr["iFactura"] = dt.Rows[0]["NumFactura"];//Convert.ToInt32(txtFacturasEnvio.Text);
            dr["Cliente"] = dt.Rows[0]["Cliente"].ToString();
            dr["Fecha"] = dt.Rows[0]["Fecha"];
            dr["Tipo"] = Tipo;
            dtFcrs.Rows.Add(dr);

            dgvDatos.DataSource = dtFcrs;
            txtFacturasEnvio.Text = string.Empty;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if(dgvDatos.Rows.Count < 0)
            {
                MessageBox.Show("Debe agregar antes las facturas para esta solicitud", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (cboProveedor.SelectedIndex == -1)
            {
                MessageBox.Show("Debe especificar al menos un provedor", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; 
            }
            if (txtDestino.Text == "")
            {
                MessageBox.Show("Debe especificar el destino", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; 
            }

            DialogResult result = MessageBox.Show("Esta apunto de crear una solicitud a Logistica, seguro que desdea continuar?", "Confirmación", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    int iFolio = -1;
                    if (lblNoFolio.Text != string.Empty || lblNoFolio.Text != "")
                    {
                        iFolio = Convert.ToInt32(lblNoFolio.Text);
                    }

                    iFolio = csLogistica.InsertUpdateSolicitudes(2, iFolio, dtpFechaSolicitud.Value, Convert.ToInt32(cboProveedor.SelectedValue), cboProveedor.Text, txtDestino.Text, 1);

                    if (iFolio > 0)
                    {
                        string RutaCompleta = "";//Ruta + NomArchivo;
                        DataTable dt = (DataTable)dgvDatos.DataSource;

                        foreach (DataRow item in dt.Rows)
                        {
                            int Relacion = Convert.IsDBNull(item[0]) ? -1 : Convert.ToInt32(item[0]);
                            int Factura = Convert.IsDBNull(item[2]) ? -1 : Convert.ToInt32(item[2]);
                            string Tipo = Convert.IsDBNull(item[5]) ? "" : Convert.ToString(item[5]);
                            int RelacionID = csLogistica.InsertFacturas(7, Relacion, iFolio, Factura, Tipo);
                            item[0] = RelacionID; item[1] = iFolio;
                        }

                        lblNoFolio.Text = iFolio.ToString();
                        csLogistica.UpdateRutaSolicitud(12, iFolio, RutaCompleta);
                        string CorreoNotificar = CorreoNotificar = csLogistica.ConsultaCorreosNotificar(14, 1, -1);
                        bool Notificado = Enviar("", CorreoNotificar, "", "", false);

                        Cursor = Cursors.Default;

                        if (Notificado)
                            MessageBox.Show("Solicitud de Logistica generada y notificada correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("Solicitud de Logistica generada correctamente (con error en notificación - correo)", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        btnGuardar.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                
            }           
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["idRelacion"].Hidden = true;
            e.Layout.Bands[0].Columns["iFolio"].Hidden = true;
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            //ImprimirSolicitud();

        }

        private void ImprimirSolicitud(string Folio,string Ruta, string NomArchivo)
        {
            try
            {
                if (Folio != string.Empty)
                {
                    ReportDocument doc = new ReportDocument();
                    ConnectionInfo crConnectionInfo = new ConnectionInfo();
                    TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                    Tables CrTables;

                    string _crystal = @"\\192.168.2.100\HalcoNET\Crystal\rptFacturas_Logistica.rpt";
                    doc.Load(_crystal);
                    
                    crConnectionInfo.ServerName = "192.168.2.100";
                    crConnectionInfo.DatabaseName = "PJ-Log";
                    crConnectionInfo.UserID = "sa";
                    crConnectionInfo.Password = "SAP-PJ1";
                    CrTables = doc.Database.Tables;

                    foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
                    {
                        crtableLogoninfo = CrTable.LogOnInfo;
                        crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                        CrTable.ApplyLogOnInfo(crtableLogoninfo);

                    }

                    doc.SetParameterValue(0, Convert.ToInt32(Folio));

                    string RutaCompleta = Ruta + NomArchivo;
                    if (!System.IO.Directory.Exists(Ruta))
                    {
                        System.IO.Directory.CreateDirectory(Ruta);
                    }
                    if (File.Exists(RutaCompleta))
                    {
                        DialogResult result = MessageBox.Show("¿Ya existe un archivo con el mismo nombre, desea reemplazarlo por el actual?", "Confirmación de Reemplazo", MessageBoxButtons.YesNoCancel);
                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                doc.ExportToDisk(ExportFormatType.PortableDocFormat, RutaCompleta);
                                Process prc = new Process();
                                prc.StartInfo.FileName = RutaCompleta;
                                prc.Start();
                            }
                            catch (Exception ex)
                            {
                                Cursor = Cursors.Default;
                                MessageBox.Show(ex.Message.ToString());
                            }
                        }
                    }
                    else
                    {
                        doc.ExportToDisk(ExportFormatType.PortableDocFormat, RutaCompleta);
                        Process prc = new Process();
                        prc.StartInfo.FileName = RutaCompleta;
                        prc.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
        private string RutaArch(string RutaIni)
        {
            string ruta = RutaIni;
            
            try
            {
                if (!File.Exists(RutaIni))
                {
                    return ruta;
                }
                else
                {
                    string nom = ruta;
                    bool existe = true;
                    int iFile = 1;
                    while (existe)
                    {
                        nom = Path.GetDirectoryName(ruta) + "\\" + Path.GetFileNameWithoutExtension(ruta) + "(" + iFile.ToString() + ")" + ".PDF";
                        if (!File.Exists(nom))
                        { 
                            existe = false;
                            ruta = nom;
                        }
                        else
                            iFile++;
                    }

                }
            }
            catch (Exception ex)
            {
                
                throw;
            }

            return ruta;
        }

        private void dgvDatos_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
        {
            DialogResult result = MessageBox.Show("Va a eliminar " + dgvDatos.Selected.Rows.Count.ToString() +  " facturas, seguro que desea continuar?", "Confirmation", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {
                foreach (var item in dgvDatos.Selected.Rows)
                {
                    if (item.Cells["idRelacion"].ToString() != "-1")
                    {
                        csLogistica.EliminaRelacionFactura(11, Convert.ToInt32(item.Cells["idRelacion"].Value));
                    }                                      
                }
            }
            e.DisplayPromptMsg = false;
            
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            lblNoFolio.Text = string.Empty;
            cboProveedor.SelectedIndex = -1;
            txtFacturasEnvio.Text = string.Empty;
            dgvDatos.DataSource = null;
            txtDestino.Text = string.Empty; ;
            btnGuardar.Enabled = true;
            dtFcrs.Rows.Clear();

        }

        private void FrmAltaFacturas_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void FrmAltaFacturas_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                log.Fin();
            }
            catch (Exception)
            {

            }
        }

        public bool Enviar(string _file, string _mailDestinatario, string _mailVendedor, string _vendedor, bool _solicitud)
        {
            bool enviado = false;
            try
            {
                ClasesSGUV.SendMail po = new ClasesSGUV.SendMail();
                ClasesSGUV.DatosMail oDatosCorreo = new ClasesSGUV.DatosMail();
                oDatosCorreo = oDatosCorreo.ObtenerDatos(1, "LOGIST");
                //oDatosCorreo.Asunto += " " + _vendedor;
                Dictionary<int, string> Correos = new Dictionary<int, string>();
                Dictionary<int, string> CorreosCopiaOculta = new Dictionary<int, string>();
                Dictionary<int, string> CorreosCopiar = new Dictionary<int, string>();
                Dictionary<int, string> ArchAdjunt = new Dictionary<int, string>();

                CorreosCopiaOculta.Add(1, "halconet@tractozone.com.mx");
                int x = 1;
                string[] _correos = _mailDestinatario.Split(new Char[] { ';' });
                foreach (string item in _correos)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        Correos.Add(x, item);
                        x++;
                    }
                }

                enviado = po.EnviarMail(oDatosCorreo, Correos, CorreosCopiar, CorreosCopiaOculta, ArchAdjunt);
            }
            catch (Exception ex)
            {
                return false;
            }
            
            return enviado;
        }
        

      
    }
}
