using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;

namespace H_Compras.Logistica
{
    public partial class FrmCapturaRutas : Constantes.frmEmpty//Form
    {
        ClasesSGUV.Logs log;
        DataTable dtFcrs = new DataTable();
        Dictionary<string, string> oCodigosRuta;
        public FrmCapturaRutas()
        {
            InitializeComponent();
        }

        private void FrmCapturaRutas_Load(object sender, EventArgs e)
        {
            try
            {
                nuevoToolStripButton.Click += btnNuevo_Click;
                guardarToolStripButton.Click += btnGuardar_Click;
                exportarToolStripButton.Visible = false;
                actualizarToolStripButton.Visible = false;
                

                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 11);
                        command.Parameters.AddWithValue("@RolUsuLog", ClasesSGUV.Login.Rol);
                        command.CommandTimeout = 0;

                        DataSet ds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int Permiso = Convert.IsDBNull(ds.Tables[0].Rows[0]["PermisoFolios"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["PermisoFolios"]);
                                if (Permiso == 1)
                                    cboSucursal.Enabled = true;
                                else
                                    cboSucursal.Enabled = false;
                            }       
                        }
                        if (ds.Tables.Count > 1)
                        {
                            if (ds.Tables[1].Rows.Count > 0)
                            {
                                int Permiso = Convert.IsDBNull(ds.Tables[1].Rows[0]["PermisoCostos"]) ? 0 : Convert.ToInt32(ds.Tables[1].Rows[0]["PermisoCostos"]);
                                if (Permiso == 1)
                                {
                                    txtCostoCasetas.Enabled = true;
                                    txtCostoGasolina.Enabled = true;

                                }
                                else
                                {
                                    txtCostoCasetas.Enabled = false;
                                    txtCostoGasolina.Enabled = false;
                                }
                            }
                        }
                        
                    }

                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 20);
                        command.CommandTimeout = 0;

                        DataSet ds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(ds);

                        cboCodigoRuta.DataSource = ds.Tables[0];
                        cboCodigoRuta.ValueMember = "Codigo";
                        cboCodigoRuta.DisplayMember = "Nombre";
                        cboCodigoRuta.SelectedIndex = -1;
                    }

                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 21);
                        command.CommandTimeout = 0;

                        DataSet ds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(ds);

                        cboTipoRuta.DataSource = ds.Tables[0];
                        cboTipoRuta.ValueMember = "Codigo";
                        cboTipoRuta.DisplayMember = "Nombre";
                        //cboTipoRuta.SelectedIndex = -1;
                    }

                }

                if (ClasesSGUV.Login.Rol == (int)(ClasesSGUV.Propiedades.RolesHalcoNET.Administrador))
                {
                    cboSucursal.Enabled = true;
                }
                log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                dtFcrs = new DataTable();

                dtFcrs.Columns.Add("nID", typeof(Int32));
                dtFcrs.Columns.Add("sCodSucursal", typeof(string));
                dtFcrs.Columns.Add("nIdCaptura", typeof(Int32));
                dtFcrs.Columns.Add("nDocumento", typeof(Int32));
                dtFcrs.Columns.Add("sTipoDoc", typeof(string));
                dtFcrs.Columns.Add("mMontoMXP", typeof(decimal));
                dtFcrs.Columns.Add("mMontoUSD", typeof(decimal));
                dtFcrs.Columns.Add("sClaveCliente", typeof(string));
                dtFcrs.Columns.Add("sCliente", typeof(string));
                dtFcrs.Columns.Add("nPeso", typeof(decimal));
                dtFcrs.Columns.Add("nVolumen", typeof(decimal));
                dtFcrs.Columns.Add("nCodigoRuta", typeof(Int32));
                dtFcrs.Columns.Add("sCodigoRuta", typeof(string));
                dtFcrs.Columns.Add("iClienteReparto", typeof(Int32));
                dtFcrs.Columns.Add("nLatitud", typeof(decimal));
                dtFcrs.Columns.Add("nLongitud", typeof(decimal));
                dtFcrs.Columns.Add("UbicacionEntrega", typeof(string));
                dtFcrs.Columns.Add("Excedente", typeof(bool));

                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 8);                    
                        command.CommandTimeout = 0;

                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);
                        cboSucursal.DataSource = dt;
                        cboSucursal.ValueMember = "Codigo";
                        cboSucursal.DisplayMember = "Nombre";
                    }
                }

                DataTable dtCatalogPlacas = new DataTable();
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 1);
                        command.Parameters.AddWithValue("@IDUsu", ClasesSGUV.Login.Id_Usuario);
                        command.Parameters.AddWithValue("@RolUsuLog", ClasesSGUV.Login.Rol);
                        command.CommandTimeout = 0;

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dtCatalogPlacas);
                        
                    }
                }


                DataTable Placas = dtCatalogPlacas;

                var source = new AutoCompleteStringCollection();
                source.AddRange((from item in Placas.AsEnumerable()
                                 select item.Field<string>("PlacaNombre")).ToArray());
                txtPlaca.AutoCompleteCustomSource = source;
                txtPlaca.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
                txtPlaca.AutoCompleteSource = AutoCompleteSource.CustomSource;

                if (cboSucursal.DataSource != null)
                    cboSucursal.SelectedValue = ClasesSGUV.Login.ClaveSucursal;

                PathHelp = "http://hntsolutions.net/manual/module_10_3.htm?ms=AAAAAAA%3D&st=MA%3D%3D&sct=MzQxOA%3D%3D&mw=MjU1";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            txtPlaca.Text = string.Empty;
            txtUsuario.Text = string.Empty;
            txtNumRuta.Text = string.Empty;
            txtBuscarFolio.Text = string.Empty;
            dtpSalida.Value = DateTime.Now;
            dtpHSalida.Value = DateTime.Now;
            dtpLlegada.Value = DateTime.Now;
            dtpHLlegada.Value = DateTime.Now;
            txtObservaciones.Text = string.Empty;
            lblNoFolio.Text = string.Empty;
            txtFacturaAdd.Text = string.Empty;
            txtCostoCasetas.Value = 0;
            txtCostoGasolina.Value = 0;
            cboCodigoRuta.SelectedIndex = -1;
            cboTipoRuta.SelectedIndex = 0;

            dgvDatos.DataSource = null;
            dtFcrs.Rows.Clear();
            dtFcrs.AcceptChanges();

            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TipoConsulta", 20);
                    command.CommandTimeout = 0;

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(ds);

                    cboCodigoRuta.DataSource = ds.Tables[0];
                    cboCodigoRuta.ValueMember = "Codigo";
                    cboCodigoRuta.DisplayMember = "Nombre";
                    cboCodigoRuta.SelectedIndex = -1;
                }
            }

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Guarda())
                {
                    return;
                }
                
                Dictionary<string, string> oAPIS = new Dictionary<string, string>();

                foreach (UltraGridRow rr in dgvDatos.Rows)
                {
                    int iExc = Convert.IsDBNull(rr.Cells["Excedente"].Value) ? 0 : Convert.ToInt32(rr.Cells["Excedente"].Value);
                    string cr = Convert.IsDBNull(rr.Cells["sCodigoRuta"].Value) ? "" : Convert.ToString(rr.Cells["sCodigoRuta"].Value);
                    if (iExc == 1)
                    {
                        if (!oAPIS.ContainsKey(cr))
                            oAPIS.Add(cr, cr);
                    }
                }
                bool Procede = true;
                int Auxiliar = 0;
                if (oAPIS.Count>0)
                {
                    string APISExc = "";
                    foreach (var item in oAPIS.Values)
                    {
                        APISExc += item.ToString();
                    }
                    DialogResult result = MessageBox.Show("Las Rutas " + APISExc + " tienen exceso de visitas permitidas, "
                    + "se generará un folio auxiliar (nuevo) para autorizacion, ¿Seguro que desea continuar?", "Confirmación", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        Procede = true;
                        Auxiliar = 1;
                    }
                    else
                    {
                        Procede = false;
                    }
                }
                if (!Procede)
                    return;

                //se inicia el objeto transacion para confirmar si todo fue correcto o realizar un Rollback para deshacer movimientos
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("SampleTransaction");

                    try
                    {
                        int iCaptura = -1;
                        if (lblNoFolio.Text != string.Empty)
                            iCaptura = Convert.ToInt32(lblNoFolio.Text);


                        //-------------SE INSERTA EL ENCABEZADO DE LA CAPTURA-------------
                        using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                        {
                            command.Transaction = transaction;
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@TipoConsulta", 4);
                            if (iCaptura < 0)
                            {
                                SqlParameter parameter = new SqlParameter("@iCaptura", SqlDbType.Int);
                                parameter.Direction = ParameterDirection.Output; command.Parameters.Add(parameter);
                            }
                            else
                            {   command.Parameters.AddWithValue("@iCaptura", iCaptura); }

                            command.Parameters.AddWithValue("@Sucursal", cboSucursal.SelectedValue.ToString());
                            command.Parameters.AddWithValue("@Placas", txtPlaca.Text.Trim());
                            command.Parameters.AddWithValue("@Usuario", txtUsuario.Text.Trim());
                            command.Parameters.AddWithValue("@FechaSalida", new DateTime(dtpSalida.Value.Year, dtpSalida.Value.Month, dtpSalida.Value.Day, dtpHSalida.Value.Hour, dtpHSalida.Value.Minute, dtpHSalida.Value.Second));
                            command.Parameters.AddWithValue("@FechaLlegada", new DateTime(dtpLlegada.Value.Year, dtpLlegada.Value.Month, dtpLlegada.Value.Day, dtpHLlegada.Value.Hour, dtpHLlegada.Value.Minute, dtpHLlegada.Value.Second));
                            command.Parameters.AddWithValue("@Ruta", Convert.ToString(txtNumRuta.Text.Trim()));
                            command.Parameters.AddWithValue("@Observaciones", Convert.ToString(txtObservaciones.Text.Trim()));
                            command.Parameters.AddWithValue("@CostoCasetas", Convert.ToDecimal(txtCostoCasetas.Value));
                            command.Parameters.AddWithValue("@CostoGasolina", Convert.ToDecimal(txtCostoGasolina.Value));

                            try
                            {
                                int CodRutt = Convert.ToInt32(cboCodigoRuta.SelectedValue);
                                command.Parameters.AddWithValue("@CodigoRuta", CodRutt);
                            }
                            catch (Exception ex)
                            {
                                command.Parameters.AddWithValue("@CodigoRuta", -1);
                                command.Parameters.AddWithValue("@StrCodigoRuta", cboCodigoRuta.SelectedValue.ToString());                                
                            }

                            command.Parameters.AddWithValue("@TipoRuta", cboTipoRuta.SelectedValue.ToString());
                            command.Parameters.AddWithValue("@Auxiliar", Auxiliar);    

                            command.ExecuteNonQuery();

                            iCaptura = Convert.IsDBNull(command.Parameters["@iCaptura"].Value) ? -1 : Convert.ToInt32(command.Parameters["@iCaptura"].Value);
                        }

                        if (iCaptura > 0)
                        {
                            //-------------SE ELIMINA EL DETALLE DEL FLUJO DE EFECTIVO-------------
                            using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                            {
                                command.Transaction = transaction;
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@TipoConsulta", 3);
                                command.Parameters.AddWithValue("@Sucursal", cboSucursal.SelectedValue.ToString());
                                command.Parameters.AddWithValue("@iCaptura", iCaptura);
                                command.Parameters.AddWithValue("@Auxiliar", Auxiliar);    
                                command.ExecuteNonQuery();
                            }
                            //-------------SE INSERTA EL DETALLE DEL FLUJO DE EFECTIVO-------------
                            foreach (UltraGridRow rw in dgvDatos.Rows)
                            {
                                int NumFactura = Convert.IsDBNull(rw.Cells["nDocumento"].Value) ? -1 : Convert.ToInt32(rw.Cells["nDocumento"].Value);
                                string TipoDoc = Convert.IsDBNull(rw.Cells["sTipoDoc"].Value) ? "" : Convert.ToString(rw.Cells["sTipoDoc"].Value);
                                decimal MontoMXP = Convert.IsDBNull(rw.Cells["mMontoMXP"].Value) ? 0 : Convert.ToDecimal(rw.Cells["mMontoMXP"].Value);
                                decimal MontoUSD = Convert.IsDBNull(rw.Cells["mMontoUSD"].Value) ? 0 : Convert.ToDecimal(rw.Cells["mMontoUSD"].Value);
                                string ClaveCliente = Convert.IsDBNull(rw.Cells["sClaveCliente"].Value) ? "" : Convert.ToString(rw.Cells["sClaveCliente"].Value);
                                string Cliente = Convert.IsDBNull(rw.Cells["sCliente"].Value) ? "" : Convert.ToString(rw.Cells["sCliente"].Value);
                                decimal Peso = Convert.IsDBNull(rw.Cells["nPeso"].Value) ? 0 : Convert.ToDecimal(rw.Cells["nPeso"].Value);
                                decimal Volumen = Convert.IsDBNull(rw.Cells["nVolumen"].Value) ? 0 : Convert.ToDecimal(rw.Cells["nVolumen"].Value);

                                int NCodRuta = Convert.IsDBNull(rw.Cells["nCodigoRuta"].Value) ? 0 : Convert.ToInt32(rw.Cells["nCodigoRuta"].Value);
                                string SCodRuta = Convert.IsDBNull(rw.Cells["sCodigoRuta"].Value) ? "" : Convert.ToString(rw.Cells["sCodigoRuta"].Value);
                                int iClienteReparto = Convert.IsDBNull(rw.Cells["iClienteReparto"].Value) ? 0 : Convert.ToInt32(rw.Cells["iClienteReparto"].Value);
                                decimal Latitud = Convert.IsDBNull(rw.Cells["nLatitud"].Value) ? 0 : Convert.ToDecimal(rw.Cells["nLatitud"].Value);
                                decimal Longitud = Convert.IsDBNull(rw.Cells["nLongitud"].Value) ? 0 : Convert.ToDecimal(rw.Cells["nLongitud"].Value);
                                int Exc = 0;
                                try {   Exc = Convert.IsDBNull(rw.Cells["Excedente"].Value) ? 0 : Convert.ToInt32(rw.Cells["Excedente"].Value); } catch (Exception ex) { }
                                
                                
                                using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                                {
                                    command.Transaction = transaction;
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.AddWithValue("@TipoConsulta", 5);
                                    command.Parameters.AddWithValue("@Sucursal", cboSucursal.SelectedValue.ToString());
                                    command.Parameters.AddWithValue("@iCaptura", iCaptura);
                                    command.Parameters.AddWithValue("@Factura", NumFactura);
                                    command.Parameters.AddWithValue("@TipoDocumento", TipoDoc);
                                    command.Parameters.AddWithValue("@ClaveCliente", ClaveCliente);
                                    command.Parameters.AddWithValue("@Cliente", Cliente);
                                    command.Parameters.AddWithValue("@Peso", Peso);
                                    command.Parameters.AddWithValue("@Volumen", Volumen);
                                    command.Parameters.AddWithValue("@MontoMXP", MontoMXP);
                                    command.Parameters.AddWithValue("@MontoUSD", MontoUSD);
                                    command.Parameters.AddWithValue("@CodigoRuta", NCodRuta);
                                    command.Parameters.AddWithValue("@StrCodigoRuta", SCodRuta);
                                    command.Parameters.AddWithValue("@iClienteReparto", iClienteReparto);
                                    command.Parameters.AddWithValue("@NLatitud", Latitud);
                                    command.Parameters.AddWithValue("@NLongitud", Longitud);
                                    command.Parameters.AddWithValue("@Auxiliar", Auxiliar);
                                    command.Parameters.AddWithValue("@Exedentev", Exc);    

                                    command.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                            MessageBox.Show("Captura guardado correctamente", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (Auxiliar <= 0)
                            {
                                lblNoFolio.Text = iCaptura.ToString();
                                ConsultaCapturaID(iCaptura);
                            }
                            else
                            {
                                MessageBox.Show("Folio auxiliar <" + iCaptura.ToString() + "> creado correctamente, ingrese al modulo de autorizaciones para verificar su estado");
                                btnNuevo.PerformClick();
                            }
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex1)
                        {
                            MessageBox.Show(ex1.Message);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private bool Guarda()
        {
            bool Valido = true;
            try
            {
                if (cboSucursal.SelectedIndex == -1)
                {
                    MessageBox.Show("Debe especificarse una sucursal", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);                    
                    return false; 
                }

                if (txtPlaca.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("Debe especificar una placa", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtPlaca.Focus();
                    return false;
                }
                if (txtUsuario.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("Debe especificar un nombre de usuario", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtUsuario.Focus();
                    return false;
                }
                //if (txtNumRuta.Text.Trim() == string.Empty)
                //{
                //    MessageBox.Show("Debe especificar una ruta", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    txtNumRuta.Focus();
                //    return false;
                //}
                //if (cboCodigoRuta.SelectedIndex == -1)
                //{
                //    MessageBox.Show("Debe especificar un código de ruta", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    cboCodigoRuta.Focus();
                //    return false;
                //}

                if (cboTipoRuta.SelectedIndex == -1)
                {
                    MessageBox.Show("Debe especificar un tipo de ruta", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cboTipoRuta.Focus();
                    return false;
                }

                int tipoRuta = Convert.ToInt32(cboTipoRuta.SelectedValue);

                if (dgvDatos.Rows.Count == 0 && tipoRuta != 3)
                {
                    MessageBox.Show("Debe agregar al menos una factura para continuar", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtFacturaAdd.Focus();
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                return false;   
            }
            return Valido;
        }

        private void txtBuscarFolio_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                
                if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\r' || e.KeyChar == '\b')
                {
                    e.Handled = false;
                    if (e.KeyChar == '\r')
                    {
                        if (cboSucursal.SelectedIndex == -1)
                        {
                            MessageBox.Show("Debe especificarse una sucursal", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;  
                        }
                        dtFcrs.Rows.Clear();
                        dtFcrs.AcceptChanges();
                        ConsultaCapturaID(Convert.ToInt32(txtBuscarFolio.Text));
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void ConsultaCapturaID(int iCaptura)
        {
            try
            {
                DataSet DS = new DataSet();
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 6);
                        command.Parameters.AddWithValue("@Sucursal", Convert.ToString(cboSucursal.SelectedValue.ToString().Trim()));
                        command.Parameters.AddWithValue("@iCaptura", iCaptura);
                        command.CommandTimeout = 0;

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(DS);
                    }
                }
                if (DS.Tables.Count > 1)
                {
                    if (DS.Tables[0].Rows.Count > 0)
                    {
                        txtPlaca.Text = Convert.IsDBNull(DS.Tables[0].Rows[0]["sPlacas"]) ? string.Empty : DS.Tables[0].Rows[0]["sPlacas"].ToString();
                        txtUsuario.Text = Convert.IsDBNull(DS.Tables[0].Rows[0]["sUsuario"]) ? string.Empty : DS.Tables[0].Rows[0]["sUsuario"].ToString();
                        dtpSalida.Value = Convert.IsDBNull(DS.Tables[0].Rows[0]["dSalida"]) ? Convert.ToDateTime("01-01-1999") : Convert.ToDateTime(DS.Tables[0].Rows[0]["dSalida"]);
                        dtpHSalida.Value = Convert.IsDBNull(DS.Tables[0].Rows[0]["dSalida"]) ? Convert.ToDateTime("01-01-1999") : Convert.ToDateTime(DS.Tables[0].Rows[0]["dSalida"]);
                        dtpLlegada.Value = Convert.IsDBNull(DS.Tables[0].Rows[0]["dLlegada"]) ? Convert.ToDateTime("01-01-1999") : Convert.ToDateTime(DS.Tables[0].Rows[0]["dLlegada"]);
                        dtpHLlegada.Value = Convert.IsDBNull(DS.Tables[0].Rows[0]["dLlegada"]) ? Convert.ToDateTime("01-01-1999") : Convert.ToDateTime(DS.Tables[0].Rows[0]["dLlegada"]);
                        txtNumRuta.Text = Convert.IsDBNull(DS.Tables[0].Rows[0]["sRuta"]) ? string.Empty : DS.Tables[0].Rows[0]["sRuta"].ToString();
                        txtObservaciones.Text = Convert.IsDBNull(DS.Tables[0].Rows[0]["sObservaciones"]) ? string.Empty : DS.Tables[0].Rows[0]["sObservaciones"].ToString();

                        txtCostoCasetas.Value = Convert.IsDBNull(DS.Tables[0].Rows[0]["mCostoCaseta"]) ? 0 : Convert.ToDecimal(DS.Tables[0].Rows[0]["mCostoCaseta"]);
                        txtCostoGasolina.Value = Convert.IsDBNull(DS.Tables[0].Rows[0]["mCostoGasolina"]) ? 0 : Convert.ToDecimal(DS.Tables[0].Rows[0]["mCostoGasolina"]);

                        try
                        {
                            int CodRutt = Convert.IsDBNull(DS.Tables[0].Rows[0]["nCodigoRuta"]) ? -1 : Convert.ToInt32(DS.Tables[0].Rows[0]["nCodigoRuta"]);
                            string sCodRutt = Convert.IsDBNull(DS.Tables[0].Rows[0]["sCodigoRuta"]) ? "" : Convert.ToString(DS.Tables[0].Rows[0]["sCodigoRuta"]);

                            if (CodRutt > 0)
                            {
                                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                                {
                                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                                    {
                                        command.CommandType = CommandType.StoredProcedure;

                                        command.Parameters.AddWithValue("@TipoConsulta", 13);
                                        command.CommandTimeout = 0;

                                        DataSet ds = new DataSet();
                                        SqlDataAdapter da = new SqlDataAdapter();
                                        da.SelectCommand = command;
                                        da.Fill(ds);

                                        cboCodigoRuta.DataSource = ds.Tables[0];
                                        cboCodigoRuta.ValueMember = "Codigo";
                                        cboCodigoRuta.DisplayMember = "Nombre";
                                        cboCodigoRuta.SelectedIndex = -1;
                                    }
                                }
                                cboCodigoRuta.SelectedValue = Convert.IsDBNull(DS.Tables[0].Rows[0]["nCodigoRuta"]) ? -1 : Convert.ToInt32(DS.Tables[0].Rows[0]["nCodigoRuta"]);
 
                            }
                            else if (CodRutt <= 0)
                            {
                                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                                {
                                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                                    {
                                        command.CommandType = CommandType.StoredProcedure;

                                        command.Parameters.AddWithValue("@TipoConsulta", 20);
                                        command.CommandTimeout = 0;

                                        DataSet ds = new DataSet();
                                        SqlDataAdapter da = new SqlDataAdapter();
                                        da.SelectCommand = command;
                                        da.Fill(ds);

                                        cboCodigoRuta.DataSource = ds.Tables[0];
                                        cboCodigoRuta.ValueMember = "Codigo";
                                        cboCodigoRuta.DisplayMember = "Nombre";
                                        cboCodigoRuta.SelectedIndex = -1;
                                    }
                                }
                                cboCodigoRuta.SelectedValue = Convert.IsDBNull(DS.Tables[0].Rows[0]["sCodigoRuta"]) ? "" : Convert.ToString(DS.Tables[0].Rows[0]["sCodigoRuta"]); 
                            }
                        }
                        catch (Exception ex)
                        {
                            
                        }

                        cboTipoRuta.SelectedValue = Convert.IsDBNull(DS.Tables[0].Rows[0]["sTipoRuta"]) ? -1 : Convert.ToInt32(DS.Tables[0].Rows[0]["sTipoRuta"]);

                        lblNoFolio.Text = DS.Tables[0].Rows[0]["nIdCaptura"].ToString();

                        dtFcrs = DS.Tables[1].Copy();
                        dgvDatos.DataSource = dtFcrs;
                    }
                    else
                    {
                        MessageBox.Show("No existe ningun folio con el numero especificado");
                        txtPlaca.Text = string.Empty;
                        txtUsuario.Text = string.Empty;
                        txtNumRuta.Text = string.Empty;
                        dtpSalida.Value = DateTime.Now;
                        dtpHSalida.Value = DateTime.Now;
                        dtpLlegada.Value = DateTime.Now;
                        dtpHLlegada.Value = DateTime.Now;
                        txtObservaciones.Text = string.Empty;
                        txtCostoCasetas.Value = 0;
                        txtCostoGasolina.Value = 0;
                        lblNoFolio.Text = string.Empty;
                        txtFacturaAdd.Text = string.Empty;
                        cboCodigoRuta.SelectedIndex = -1;
                        dtFcrs.Rows.Clear();
                        dgvDatos.DataSource = null;
                    }
                }

            }
            catch (Exception EX)
            {
                throw;
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                Infragistics.Win.UltraWinGrid.UltraGridBand band = (sender as UltraGrid).DisplayLayout.Bands[0];//this.dgvFlujoEfectivo.DisplayLayout.Bands[0];
                band.Groups.Clear();
                e.Layout.Bands[0].Summaries.Clear();

                e.Layout.Bands[0].Columns["nID"].Hidden = true;

                e.Layout.Bands[0].Columns["nIdCaptura"].Hidden = true;
                e.Layout.Bands[0].Columns["sCodSucursal"].Hidden = true;
                e.Layout.Bands[0].Columns["nDocumento"].Header.Caption = "No. Factura";
                e.Layout.Bands[0].Columns["nDocumento"].Width = 80;
                e.Layout.Bands[0].Columns["sTipoDoc"].Header.VisiblePosition = 1;
                e.Layout.Bands[0].Columns["sTipoDoc"].Header.Caption = "Tipo Doc.";
                e.Layout.Bands[0].Columns["sTipoDoc"].Width = 70;
                e.Layout.Bands[0].Columns["mMontoMXP"].Header.Caption = "Monto $";
                e.Layout.Bands[0].Columns["mMontoMXP"].Format = "N2";
                e.Layout.Bands[0].Columns["mMontoMXP"].CellAppearance.TextHAlign = HAlign.Right;
                SummarySettings summary3 = band.Summaries.Add("mMontoMXP", SummaryType.Sum, band.Columns["mMontoMXP"]);
                summary3.DisplayFormat = "{0:N}"; summary3.Appearance.TextHAlign = HAlign.Right;

                e.Layout.Bands[0].Columns["mMontoUSD"].Header.Caption = "Monto USD";
                e.Layout.Bands[0].Columns["mMontoUSD"].Format = "N2";
                e.Layout.Bands[0].Columns["mMontoUSD"].CellAppearance.TextHAlign = HAlign.Right;
                SummarySettings summary4 = band.Summaries.Add("mMontoUSD", SummaryType.Sum, band.Columns["mMontoUSD"]);
                summary4.DisplayFormat = "{0:N}"; summary4.Appearance.TextHAlign = HAlign.Right;

                e.Layout.Bands[0].Columns["sClaveCliente"].Hidden = true;
                e.Layout.Bands[0].Columns["sCliente"].Header.Caption = "Cliente / Proveedor";
                e.Layout.Bands[0].Columns["sCliente"].Width = 180;
                e.Layout.Bands[0].Columns["nPeso"].Header.Caption = "Peso";
                e.Layout.Bands[0].Columns["nPeso"].Format = "N2";
                e.Layout.Bands[0].Columns["nPeso"].CellAppearance.TextHAlign = HAlign.Right;
                SummarySettings summary1 = band.Summaries.Add("nPeso", SummaryType.Sum, band.Columns["nPeso"]);
                summary1.DisplayFormat = "{0:N} kg"; summary1.Appearance.TextHAlign = HAlign.Right; 

                e.Layout.Bands[0].Columns["nVolumen"].Header.Caption = "Volumen";
                e.Layout.Bands[0].Columns["nVolumen"].Format = "N2";
                e.Layout.Bands[0].Columns["nVolumen"].CellAppearance.TextHAlign = HAlign.Right;
                SummarySettings summary2 = band.Summaries.Add("nVolumen", SummaryType.Sum, band.Columns["nVolumen"]);
                summary2.DisplayFormat = "{0:N} m3"; summary2.Appearance.TextHAlign = HAlign.Right;

                e.Layout.Bands[0].Columns["nCodigoRuta"].Hidden = true;
                e.Layout.Bands[0].Columns["sCodigoRuta"].Header.Caption = "Codigo-Ruta";
                e.Layout.Bands[0].Columns["iClienteReparto"].Hidden = true;
                e.Layout.Bands[0].Columns["nLatitud"].Hidden = true;
                e.Layout.Bands[0].Columns["nLongitud"].Hidden = true;
                //,td.nCodigoRuta, td.sCodigoRuta, td.iClienteReparto, td.nLatitud, td.nLongitud

                foreach (var item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Activation.NoEdit;
                }

                e.Layout.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed;
                e.Layout.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;
                e.Layout.Override.AllowMultiCellOperations = AllowMultiCellOperation.Copy;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnAgregarFactura_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboSucursal.SelectedIndex == -1)
                {
                    MessageBox.Show("Debe especificarse una sucursal", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (txtFacturaAdd.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("Debe especificarse un número de factura", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtFacturaAdd.Focus();
                    return;
                }

                //DataTable dt = new DataTable();
                DataSet ds = new DataSet();

                int TipoConsulta = 2;
                if (rbtFactura.Checked)
                    TipoConsulta = 2;
                else if (rbtRemision.Checked)
                    TipoConsulta = 12;

                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", TipoConsulta);
                        command.Parameters.AddWithValue("@Factura", txtFacturaAdd.Text.Trim());
                        command.CommandTimeout = 0;

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(ds);
                    }
                }
                bool Correcto = false;
                bool SeguirValidando = true;

                if (ds.Tables.Count > 0)
                {
                    if (SeguirValidando)
                    {
                        if (ds.Tables[0].Rows.Count <= 0)
                        {
                            Correcto = false;
                            SeguirValidando = false;
                            MessageBox.Show("La factura especificada no existe", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }


                    //if (SeguirValidando && ds.Tables.Count>1)
                    //{
                    //    if (ds.Tables[1].Rows.Count <= 0)
                    //    {
                    //        Correcto = false;
                    //        SeguirValidando = false;
                    //        MessageBox.Show("No se ha especificado ningun codigo de ruta para este cliente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //        return; 
                    //    } 
                    //}

                    //if (SeguirValidando && ds.Tables.Count > 2)
                    //{
                    //    if (ds.Tables[2].Rows.Count <= 0)
                    //    {
                    //        Correcto = false;
                    //        SeguirValidando = false;
                    //        MessageBox.Show("No se ha especificado ninguna coordenada de entrega para este cliente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //        return;
                    //    }
                    //}
                }
                else {
                    SeguirValidando = false;
                    Correcto = false;
                    MessageBox.Show("La factura especificada no existe", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


                int NumMenor = -1;
                foreach (DataRow item in dtFcrs.Rows)
                {
                    int x = Convert.IsDBNull(item["nID"]) ? -1 : Convert.ToInt32(item["nID"]);
                    if (x <= NumMenor)
                        NumMenor = x - 1;
                }

                string TipoVisita = Convert.IsDBNull(ds.Tables[3].Rows[0]["TipoVisita"]) ? "" : Convert.ToString(ds.Tables[3].Rows[0]["TipoVisita"]); 
                if (TipoVisita != "01") //VALIDAR PARA CLIENTES NO LOCALES
                {
                    if ((ds.Tables[1].Rows.Count > 1 || ds.Tables[2].Rows.Count > 1) || (ds.Tables[1].Rows.Count == 0 || ds.Tables[2].Rows.Count == 0))
                    {
                        H_Compras.Logistica.FrmUbicacionesClientes oForm = new
                            H_Compras.Logistica.FrmUbicacionesClientes(ds.Tables[0].Rows[0]["CardCode"].ToString(), ds.Tables[0].Rows[0]["sCliente"].ToString(),
                                                                     ds.Tables[1], ds.Tables[2]);
                        if (oForm.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                        {
                            bool Excede = false;
                            //string TipoVisita = ds.Tables[3].Rows[0]["TipoVisita"].ToString();
                            //if (TipoVisita != "01")
                            Excede = IsExcedente(ds.Tables[0].Rows[0]["CardCode"].ToString(), oForm.oCaptura.CodigoRuta.ToString());
                            //SE HACE DE LA FORMA NORMAL QUE SIEMPRE
                            DataRow dr = dtFcrs.NewRow();
                            dr["nID"] = NumMenor;
                            dr["sCodSucursal"] = cboSucursal.SelectedValue.ToString();
                            dr["nIdCaptura"] = -1;
                            dr["nDocumento"] = ds.Tables[0].Rows[0]["NumFactura"];
                            dr["sTipoDoc"] = ds.Tables[0].Rows[0]["sTipoDoc"];
                            dr["mMontoMXP"] = ds.Tables[0].Rows[0]["mMontoMXP"];
                            dr["mMontoUSD"] = ds.Tables[0].Rows[0]["mMontoUSD"];
                            dr["sClaveCliente"] = ds.Tables[0].Rows[0]["CardCode"];
                            dr["sCliente"] = ds.Tables[0].Rows[0]["sCliente"];
                            dr["nPeso"] = ds.Tables[0].Rows[0]["nPeso"];
                            dr["nVolumen"] = ds.Tables[0].Rows[0]["nVolumen"];
                            //--------------------------------------------------
                            dr["nCodigoRuta"] = "-1";//oForm.oCaptura.CodigoRuta.ToString();
                            dr["sCodigoRuta"] = Convert.IsDBNull(oForm.oCaptura.CodigoRuta) ? "" : Convert.ToString(oForm.oCaptura.CodigoRuta);
                            dr["iClienteReparto"] = Convert.IsDBNull(oForm.oCaptura.iClienteReparto) ? "-1" : Convert.ToString(oForm.oCaptura.iClienteReparto);
                            dr["nLatitud"] = Convert.IsDBNull(oForm.oCaptura.Latitud) ? "0" : Convert.ToString(oForm.oCaptura.Latitud);
                            dr["nLongitud"] = Convert.IsDBNull(oForm.oCaptura.Longitud) ? "0" : Convert.ToString(oForm.oCaptura.Longitud);
                            dr["UbicacionEntrega"] = Convert.IsDBNull(oForm.oCaptura.DescripcionEntrega) ? "" : Convert.ToString(oForm.oCaptura.DescripcionEntrega); 
                            dr["Excedente"] = Excede;

                            dtFcrs.Rows.Add(dr);

                            dgvDatos.DataSource = dtFcrs;
                            txtFacturaAdd.Text = string.Empty;
                        }
                    }
                    else
                    {
                        bool Excede = false;
                        //SE HACE DE LA FORMA NORMAL QUE SIEMPRE
                        DataRow dr = dtFcrs.NewRow();
                        dr["nID"] = NumMenor;
                        dr["sCodSucursal"] = cboSucursal.SelectedValue.ToString();
                        dr["nIdCaptura"] = -1;
                        dr["nDocumento"] = ds.Tables[0].Rows[0]["NumFactura"];//Convert.ToInt32(txtFacturasEnvio.Text);
                        dr["sTipoDoc"] = ds.Tables[0].Rows[0]["sTipoDoc"];
                        dr["mMontoMXP"] = ds.Tables[0].Rows[0]["mMontoMXP"];
                        dr["mMontoUSD"] = ds.Tables[0].Rows[0]["mMontoUSD"];
                        dr["sClaveCliente"] = ds.Tables[0].Rows[0]["CardCode"];
                        dr["sCliente"] = ds.Tables[0].Rows[0]["sCliente"];
                        dr["nPeso"] = ds.Tables[0].Rows[0]["nPeso"];
                        dr["nVolumen"] = ds.Tables[0].Rows[0]["nVolumen"];
                        //--------------------------------------------------
                        dr["nCodigoRuta"] = "-1";

                        if (ds.Tables[1].Rows.Count > 0)
                            dr["sCodigoRuta"] = ds.Tables[1].Rows[0]["CodRuta"];
                        else
                            dr["sCodigoRuta"] = "";

                        if (ds.Tables[2].Rows.Count > 0)
                        {
                            dr["iClienteReparto"] = ds.Tables[2].Rows[0]["idClienteReparto"];
                            dr["nLatitud"] = ds.Tables[2].Rows[0]["Latitud"];
                            dr["nLongitud"] = ds.Tables[2].Rows[0]["Longitud"];
                            dr["UbicacionEntrega"] = ds.Tables[2].Rows[0]["sDescripcion"];
                            dr["Excedente"] = Excede;
                        }
                        else
                        {
                            dr["iClienteReparto"] = "-1";
                            dr["nLatitud"] = "0";
                            dr["nLongitud"] = "0";
                            dr["UbicacionEntrega"] = "";
                            dr["Excedente"] = Excede;
                        }
                        dtFcrs.Rows.Add(dr);

                        dgvDatos.DataSource = dtFcrs;
                        txtFacturaAdd.Text = string.Empty;
                    }
                }
                else //Es cliente local y no importa la asociacion de codigos de ruta
                {
                    bool Excede = false;
                    //SE HACE DE LA FORMA NORMAL QUE SIEMPRE
                    DataRow dr = dtFcrs.NewRow();
                    dr["nID"] = NumMenor;
                    dr["sCodSucursal"] = cboSucursal.SelectedValue.ToString();
                    dr["nIdCaptura"] = -1;
                    dr["nDocumento"] = ds.Tables[0].Rows[0]["NumFactura"];//Convert.ToInt32(txtFacturasEnvio.Text);
                    dr["sTipoDoc"] = ds.Tables[0].Rows[0]["sTipoDoc"];
                    dr["mMontoMXP"] = ds.Tables[0].Rows[0]["mMontoMXP"];
                    dr["mMontoUSD"] = ds.Tables[0].Rows[0]["mMontoUSD"];
                    dr["sClaveCliente"] = ds.Tables[0].Rows[0]["CardCode"];
                    dr["sCliente"] = ds.Tables[0].Rows[0]["sCliente"];
                    dr["nPeso"] = ds.Tables[0].Rows[0]["nPeso"];
                    dr["nVolumen"] = ds.Tables[0].Rows[0]["nVolumen"];
                    dr["nCodigoRuta"] = "-1";
                    dr["sCodigoRuta"] = "";
                    dr["iClienteReparto"] = "-1";
                    dr["nLatitud"] = "0";
                    dr["nLongitud"] = "0";
                    dr["UbicacionEntrega"] = "";
                    dr["Excedente"] = Excede;
                    dtFcrs.Rows.Add(dr);

                    dgvDatos.DataSource = dtFcrs;
                    txtFacturaAdd.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private bool IsExcedente(string Cliente, string CodigoRuta)
        {
            bool EXED = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 29);
                        command.Parameters.AddWithValue("@ClaveCliente", Cliente);
                        command.Parameters.AddWithValue("@StrCodigoRuta", CodigoRuta);
                        command.CommandTimeout = 0;
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            int Disponibilidad = 0;
                            Disponibilidad = Convert.IsDBNull(dt.Rows[0]["Disponibilidad"]) ? 0 : Convert.ToInt32(dt.Rows[0]["Disponibilidad"]);
                            if (Disponibilidad <= 0)
                                EXED = true;
                        }

                    }
                }
            }
            catch (Exception EX)
            {
                throw;
            }

            return EXED;
        }

        private void txtPlaca_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string[] split = txtPlaca.Text.Split(' ');
                txtPlaca.Text = split[0];
            }
            catch (Exception)
            {
            }
        }

        private void dgvDatos_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            e.DisplayPromptMsg = false;
             DialogResult result = MessageBox.Show("Esta apunto de eliminar " + e.Rows.Count().ToString() + " facturas permanentemente, no podrá recuperarlos posteriormente. ¿Seguro que desea continuar?", "Eliminación", MessageBoxButtons.YesNo);
             if (result == DialogResult.Yes)
             {
                 //dtFcrs.Rows.Clear(); dtFcrs.AcceptChanges();
                 dtFcrs = ((DataTable)dgvDatos.DataSource).Copy();

                 foreach (var item in e.Rows)
                 {
                     try
                     {
                         int ID = Convert.ToInt32(item.Cells["nID"].Value);
                         if (ID > 0)
                         {
                             using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                             {
                                 using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                                 {
                                     command.CommandType = CommandType.StoredProcedure;
                                     command.Parameters.AddWithValue("@TipoConsulta", 7);
                                     command.Parameters.AddWithValue("@Factura", ID);
                                     command.CommandTimeout = 0;
                                     command.Connection.Open();
                                     command.ExecuteNonQuery();
                                     command.Connection.Close();
                                 }
                             }

                             foreach (DataRow dr in dtFcrs.Rows)
                             {
                                 if (dr["nID"].ToString() == ID.ToString())
                                     dr.Delete();
                             }
                             dtFcrs.AcceptChanges();
                         }
                         else
                         {
                             foreach (DataRow dr in dtFcrs.Rows)
                             {
                                 if (dr["nID"].ToString() == ID.ToString())
                                     dr.Delete();
                             }
                             dtFcrs.AcceptChanges();

                         }

                     }
                     catch (Exception ex)
                     {

                     }

                 }
                 e.Cancel = true;
                 e.DisplayPromptMsg = false;
                 dgvDatos.DataSource = dtFcrs;

             }
             else
             {
                 e.Cancel = true;
                 e.DisplayPromptMsg = false;
             }
        }

        private void txtFacturaAdd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\r' || e.KeyChar == '\b')
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void FrmCapturaRutas_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void FrmCapturaRutas_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                log.Fin();
            }
            catch (Exception)
            {

            }
        }

        private void txtCostoCasetas_Enter(object sender, EventArgs e)
        {
            txtCostoCasetas.SelectAll();
        }

        private void txtCostoGasolina_Enter(object sender, EventArgs e)
        {
            txtCostoGasolina.SelectAll();
        }

        private void txtCostoCasetas_Click(object sender, EventArgs e)
        {
            txtCostoCasetas.SelectAll();
        }

        private void txtCostoGasolina_Click(object sender, EventArgs e)
        {
            txtCostoGasolina.SelectAll();
        }

      

       

        //private void txtNumRuta_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\r' || e.KeyChar == '\b')
        //    {
        //        e.Handled = false;
        //    }
        //    else
        //    {
        //        e.Handled = true;
        //    }
        //}
    }

   
}
