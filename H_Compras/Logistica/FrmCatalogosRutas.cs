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
    public partial class FrmCatalogosRutas : Form
    {
        
        DataTable dtClientes = new DataTable();
        DataTable dtCodigosRuta = new DataTable();
        ClasesSGUV.Logs log;

        public FrmCatalogosRutas()
        {
            InitializeComponent();
        }

        private void FrmCatalogosRutas_Load(object sender, EventArgs e)
        {
            try
            {
                log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                //Se consultan todos los clientes
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 35);
                        command.Parameters.AddWithValue("@Sucursal", ClasesSGUV.Login.Sucursal);
                        command.CommandTimeout = 0;

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dtClientes);
                    }
                }

                //SE CONSULTAN TODOS LOS CODIGOS DE RUTA
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 36);
                        command.Parameters.AddWithValue("@Sucursal", ClasesSGUV.Login.Sucursal);
                        command.CommandTimeout = 0;

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dtCodigosRuta);
                    }
                }

                cboClienteCoordenadas.DataSource = dtClientes.Copy();
                cboClienteCoordenadas.ValueMember = "Codigo";
                cboClienteCoordenadas.DisplayMember = "Nombre";
                cboClienteCoordenadas.ActiveRow = null;

                cboClienteAsociacion.DataSource = dtClientes.Copy();
                cboClienteAsociacion.ValueMember = "Codigo";
                cboClienteAsociacion.DisplayMember = "Nombre";
                cboClienteAsociacion.ActiveRow = null;

                dgvCodigosRuta.DataSource = dtCodigosRuta.Copy();

                cboCodigoRutaAsociacion.DataSource = dtCodigosRuta.Copy();
                cboCodigoRutaAsociacion.ValueMember = "Codigo";
                cboCodigoRutaAsociacion.DisplayMember = "Nombre";
                cboCodigoRutaAsociacion.ActiveRow = null;


                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void cboClienteCoordenadas_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboClienteCoordenadas.ActiveRow != null)
                {
                    using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@TipoConsulta", 37);
                            command.Parameters.AddWithValue("@ClaveCliente", cboClienteCoordenadas.ActiveRow.Cells["Codigo"].Value);
                            command.CommandTimeout = 0;

                            DataTable dt = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = command;
                            da.Fill(dt);

                            dgvCoordenadasCliente.DataSource = dt;
                        }
                    }
                }


            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnAgregarCoordenada_Click(object sender, EventArgs e)
        {
            try
            {
                GuardarCoordenada();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            
        }

        private void GuardarCoordenada()
        {
            try
            {
                if (cboClienteCoordenadas.ActiveRow == null)
                {
                    MessageBox.Show("Especifique un cliente");
                    cboClienteCoordenadas.Focus();
                    return;
                }
                if (txtLatitud.Text == string.Empty)
                {
                    MessageBox.Show("Especifique una latitud");
                    txtLatitud.Focus();
                    return;
                }

                if (txtLongitud.Text == string.Empty)
                {
                    MessageBox.Show("Especifique una longitud");
                    txtLongitud.Focus();
                    return;
                }

                if (txtUbicacionEntrega.Text == string.Empty)
                {
                    MessageBox.Show("Especifique una descripción de ubicacion de entrega");
                    txtUbicacionEntrega.Focus();
                    return;
                }

                int ID = -1;
                if (txtIDCoordenada.Text != "")
                    ID = Convert.ToInt32(txtIDCoordenada.Text);


                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 25);
                        command.Parameters.AddWithValue("@ID", ID);
                        command.Parameters.AddWithValue("@ClaveCliente", cboClienteCoordenadas.ActiveRow.Cells["Codigo"].Value);
                        command.Parameters.AddWithValue("@NLatitud", txtLatitud.Text.Trim());
                        command.Parameters.AddWithValue("@NLongitud", txtLongitud.Text.Trim());
                        command.Parameters.AddWithValue("@DescripUbicacion", txtUbicacionEntrega.Text.Trim());
                        command.Parameters.AddWithValue("@IDUsu", ClasesSGUV.Login.Id_Usuario);

                        command.CommandTimeout = 0;
                        command.Connection.Open();
                        command.ExecuteNonQuery();
                        command.Connection.Close();
                    }
                }

                dgvAsociacionClientes.DataSource = null;
                DataTable dt = new DataTable();
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 37);
                        command.Parameters.AddWithValue("@ClaveCliente", cboClienteCoordenadas.ActiveRow.Cells["Codigo"].Value);
                        command.CommandTimeout = 0;
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);
                    }
                }
                dgvCoordenadasCliente.DataSource = dt;
                Nuevo();
            }
            catch (Exception ex)
            {
                throw;
                
            }
        }

        private void btnAgregarCodigoRuta_Click(object sender, EventArgs e)
        {
            try
            {
                GuardarCodigoRuta();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void GuardarCodigoRuta()
        {
            try
            {
                if (txtCodigoRuta.Text == string.Empty)
                {
                    MessageBox.Show("Especifique un codigo de ruta");
                    txtCodigoRuta.Focus();
                    return;
                }
                if (txtDefinicionRuta.Text == string.Empty)
                {
                    MessageBox.Show("Especifique una definicion para la ruta");
                    txtDefinicionRuta.Focus();
                    return;
                }
                if (txtAbreviatura.Text == string.Empty)
                {
                    MessageBox.Show("Especifique una abreviatura para la ruta");
                    txtAbreviatura.Focus();
                    return;
                }

                if (txtFrecuencia.Text == string.Empty)
                {
                    MessageBox.Show("Especifique la frecuencia con la que se visitará al cliente");
                    txtFrecuencia.Focus();
                    return;
                }

                int ID = -1;
                if (txtIdCodigoRuta.Text != "")
                    ID = Convert.ToInt32(txtIdCodigoRuta.Text);


                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 27);
                        command.Parameters.AddWithValue("@ID", ID);
                        command.Parameters.AddWithValue("@StrCodigoRuta", txtCodigoRuta.Text.Trim());
                        command.Parameters.AddWithValue("@DefRuta", txtDefinicionRuta.Text.Trim());
                        command.Parameters.AddWithValue("@AbrevRuta", txtAbreviatura.Text.Trim());
                        command.Parameters.AddWithValue("@FrecuenciaVisita", Convert.ToInt32(txtFrecuencia.Text));
                        command.Parameters.AddWithValue("@Diario", rbtDiario.Checked);
                        command.Parameters.AddWithValue("@Semanal", rbtSemanal.Checked);
                        command.Parameters.AddWithValue("@Quincenal", rbtQuincenal.Checked);
                        command.Parameters.AddWithValue("@Mensual", rbtMensual.Checked);
                        command.Parameters.AddWithValue("@IDUsu", ClasesSGUV.Login.Id_Usuario);
                        command.CommandTimeout = 0;
                        command.Connection.Open();
                        command.ExecuteNonQuery();
                        command.Connection.Close();
                    }
                }

                dgvCodigosRuta.DataSource = null;
                DataTable dt = new DataTable();
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 36);
                        command.CommandTimeout = 0;
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);
                    }
                }
                dtCodigosRuta = dt;
                dgvCodigosRuta.DataSource = dtCodigosRuta.Copy();
                cboCodigoRutaAsociacion.DataSource = dtCodigosRuta.Copy();
                Nuevo();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void btnAgregarAsociacion_Click(object sender, EventArgs e)
        {
            try
            {
                GuardarAsociacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void GuardarAsociacion()
        {
            try
            {
                if (cboClienteAsociacion.ActiveRow == null)
                {
                    MessageBox.Show("Especifique un cliente");
                    cboClienteAsociacion.Focus();
                    return;
                }
                if (cboCodigoRutaAsociacion.ActiveRow == null)
                {
                    MessageBox.Show("Especifique un codigo de ruta");
                    cboCodigoRutaAsociacion.Focus();
                    return;
                }

                //if (txtFrecuencia.Text == string.Empty)
                //{
                //    MessageBox.Show("Especifique la frecuencia con la que se visitará al cliente");
                //    txtFrecuencia.Focus();
                //    return;
                //}

                int ID = -1;
                if (txtIDAsociacion.Text != "")
                    ID = Convert.ToInt32(txtIDAsociacion.Text);


                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 22);
                        command.Parameters.AddWithValue("@ID", ID);
                        command.Parameters.AddWithValue("@ClaveCliente", cboClienteAsociacion.ActiveRow.Cells["Codigo"].Value);
                        command.Parameters.AddWithValue("@StrCodigoRuta", cboCodigoRutaAsociacion.ActiveRow.Cells["Codigo"].Value);
                        command.Parameters.AddWithValue("@IDUsu", ClasesSGUV.Login.Id_Usuario);

                        command.CommandTimeout = 0;
                        command.Connection.Open();
                        command.ExecuteNonQuery();
                        command.Connection.Close();
                    }
                }

                dgvAsociacionClientes.DataSource = null;
                DataTable dt = new DataTable();
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 38);
                        command.Parameters.AddWithValue("@ClaveCliente", cboClienteAsociacion.ActiveRow.Cells["Codigo"].Value);
                        command.CommandTimeout = 0;
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);
                    }
                }
                
                dgvAsociacionClientes.DataSource = dt;
                Nuevo();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void dgvAsociacionClientes_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            try
            {
                txtIDAsociacion.Text = dgvAsociacionClientes.Rows[e.Cell.Row.Index].Cells["nID"].Value.ToString();
                cboClienteAsociacion.Value = dgvAsociacionClientes.Rows[e.Cell.Row.Index].Cells["CardCode"].Value.ToString();
                cboCodigoRutaAsociacion.Value = dgvAsociacionClientes.Rows[e.Cell.Row.Index].Cells["CodRuta"].Value.ToString();
                txtFrecuencia.Text = dgvAsociacionClientes.Rows[e.Cell.Row.Index].Cells["nFrecuencia"].Value.ToString();
                btnAgregarGeneral.Visible = false;
                btnActualizarGeneral.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void dgvCodigosRuta_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            txtIdCodigoRuta.Text = Convert.IsDBNull(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["ID"].Value) ? "-1" : Convert.ToString(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["ID"].Value);
            txtCodigoRuta.Text = Convert.IsDBNull(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["Codigo"].Value) ? "" : Convert.ToString(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["Codigo"].Value);
            txtDefinicionRuta.Text = Convert.IsDBNull(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["Nombre"].Value) ? "" : Convert.ToString(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["Nombre"].Value);
            txtAbreviatura.Text = Convert.IsDBNull(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["Abreviatura"].Value) ? "" : Convert.ToString(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["Abreviatura"].Value);
            txtFrecuencia.Text = Convert.IsDBNull(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["nFrecuencia"].Value) ? "" : Convert.ToString(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["nFrecuencia"].Value);
            bool bDiario = Convert.IsDBNull(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["bDiario"].Value) ? false : Convert.ToBoolean(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["bDiario"].Value);
            bool bSemanal = Convert.IsDBNull(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["bSemanal"].Value) ? false : Convert.ToBoolean(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["bSemanal"].Value);
            bool bQuincenal = Convert.IsDBNull(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["bQuincenal"].Value) ? false : Convert.ToBoolean(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["bQuincenal"].Value);
            bool bMensual = Convert.IsDBNull(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["bMensual"].Value) ? false : Convert.ToBoolean(dgvCodigosRuta.Rows[e.Cell.Row.Index].Cells["bMensual"].Value);

            rbtDiario.Checked = false; rbtSemanal.Checked = false; rbtQuincenal.Checked = false; rbtSemanal.Checked = true;
            if (bDiario)
                rbtDiario.Checked = true;
            else if (bSemanal)
                rbtSemanal.Checked = true;
            else if (bQuincenal)
                rbtQuincenal.Checked = true;
            else if (bMensual)
                rbtMensual.Checked = true;

            btnActualizarGeneral.Visible = true;
            btnAgregarGeneral.Visible = false;


        }

        private void dgvCoordenadasCliente_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            txtIDCoordenada.Text = Convert.IsDBNull(dgvCoordenadasCliente.Rows[e.Cell.Row.Index].Cells["idClienteReparto"].Value) ? "-1" : Convert.ToString(dgvCoordenadasCliente.Rows[e.Cell.Row.Index].Cells["idClienteReparto"].Value);
            string CodCli = Convert.IsDBNull(dgvCoordenadasCliente.Rows[e.Cell.Row.Index].Cells["CodigoCliente"].Value) ? "-1" : Convert.ToString(dgvCoordenadasCliente.Rows[e.Cell.Row.Index].Cells["CodigoCliente"].Value);
            if (CodCli != "")
                cboClienteCoordenadas.Value = CodCli;
            else
                cboClienteCoordenadas.ActiveRow = null;
            txtLatitud.Text = Convert.IsDBNull(dgvCoordenadasCliente.Rows[e.Cell.Row.Index].Cells["Latitud"].Value) ? "-1" : Convert.ToString(dgvCoordenadasCliente.Rows[e.Cell.Row.Index].Cells["Latitud"].Value);
            txtLongitud.Text = Convert.IsDBNull(dgvCoordenadasCliente.Rows[e.Cell.Row.Index].Cells["Longitud"].Value) ? "-1" : Convert.ToString(dgvCoordenadasCliente.Rows[e.Cell.Row.Index].Cells["Longitud"].Value);
            txtUbicacionEntrega.Text = Convert.IsDBNull(dgvCoordenadasCliente.Rows[e.Cell.Row.Index].Cells["sDescripcion"].Value) ? "-1" : Convert.ToString(dgvCoordenadasCliente.Rows[e.Cell.Row.Index].Cells["sDescripcion"].Value);

            btnActualizarGeneral.Visible = true;
            btnAgregarGeneral.Visible = false;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Nuevo();
        }

        private void Nuevo()
        {
            txtIDCoordenada.Text = string.Empty;
            cboClienteCoordenadas.ActiveRow = null;
            txtLatitud.Text = string.Empty;
            txtLongitud.Text = string.Empty;
            txtUbicacionEntrega.Text = string.Empty;
            //--------------------------------------
            txtIdCodigoRuta.Text = string.Empty;
            txtCodigoRuta.Text = string.Empty;
            txtDefinicionRuta.Text = string.Empty;
            txtAbreviatura.Text = string.Empty;
            txtFrecuencia.Text = string.Empty;
            //------------------------------------
            txtIDAsociacion.Text = string.Empty;
            cboClienteAsociacion.ActiveRow = null;
            cboCodigoRutaAsociacion.ActiveRow = null;
            btnAgregarGeneral.Visible = true;
            btnActualizarGeneral.Visible = false;
        }

        private void btnAgregarGeneral_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabGeneral.SelectedTab.Key == "tbpCoordenadas")
                {
                    GuardarCoordenada();
                   // btnAgregarCoordenada.PerformClick();
                }
                else if (tabGeneral.SelectedTab.Key == "tbpCodigosRuta")
                {
                    GuardarCodigoRuta();
                   // btnAgregarCodigoRuta.PerformClick();
                }
                else if (tabGeneral.SelectedTab.Key == "tbpAsociacion")
                {
                    GuardarAsociacion();
                    //btnAgregarAsociacion.PerformClick();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            
        }

        private void btnActualizarGeneral_Click(object sender, EventArgs e)
        {
            if (tabGeneral.SelectedTab.Key == "tbpCoordenadas")
            {
                GuardarCoordenada();
                //btnAgregarCoordenada.PerformClick();
            }
            else if (tabGeneral.SelectedTab.Key == "tbpCodigosRuta")
            {
                GuardarCodigoRuta();
                //btnAgregarCodigoRuta.PerformClick();
            }
            else if (tabGeneral.SelectedTab.Key == "tbpAsociacion")
            {
                GuardarAsociacion();
                //btnAgregarAsociacion.PerformClick();
            }

        }

        private void dgvAsociacionClientes_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            try
            {
                e.DisplayPromptMsg = false;
                DialogResult result = MessageBox.Show("Esta apunto de eliminar " + e.Rows.Count().ToString() + " registros permanentemente, no podrá recuperarlos posteriormente. ¿Seguro que desea continuar?", "Eliminación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string Cliente = "";
                    foreach (var item in e.Rows)
                    {
                        int ID = Convert.ToInt32(item.Cells["nID"].Value);
                        Cliente = Convert.ToString(item.Cells["CardCode"].Value);
                        using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                        {
                            using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@TipoConsulta", 24);
                                command.Parameters.AddWithValue("@ID", ID);
                                command.Parameters.AddWithValue("@IDUsu", ClasesSGUV.Login.Id_Usuario);

                                command.CommandTimeout = 0;
                                command.Connection.Open();
                                command.ExecuteNonQuery();
                                command.Connection.Close();
                            }
                        }
                    }
                    e.Cancel = true;
                    dgvAsociacionClientes.DataSource = null;

                    DataTable dt = new DataTable();
                    using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@TipoConsulta", 38);
                            command.Parameters.AddWithValue("@ClaveCliente", Cliente);
                            command.CommandTimeout = 0;
                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = command;
                            da.Fill(dt);
                        }
                    }
                    dgvAsociacionClientes.DataSource = dt;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void dgvAsociacionClientes_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Bands[0].Columns["nID"].Hidden = true;
                e.Layout.Bands[0].Columns["CardCode"].Hidden = true;
                e.Layout.Bands[0].Columns["CodRuta"].Header.Caption = "Codigo Ruta";
                e.Layout.Bands[0].Columns["nFrecuencia"].Header.Caption = "No. Frecuencia";
                e.Layout.Bands[0].Columns["bDiario"].Hidden = true;
                e.Layout.Bands[0].Columns["bSemanal"].Hidden = true;
                e.Layout.Bands[0].Columns["bQuincenal"].Hidden = true;
                e.Layout.Bands[0].Columns["bMensual"].Hidden = true;
                e.Layout.Bands[0].Columns["sFrecuencia"].Header.Caption = "Frecuencia";
                e.Layout.Bands[0].Columns["Sucursal"].Hidden = true;
                e.Layout.Bands[0].Columns["Elegido"].Hidden = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
           
        }

        private void tabGeneral_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            try
            {
                btnAgregarGeneral.Visible = true;
                btnActualizarGeneral.Visible = false;
                if (e.Tab.Key == "tbpAsociacion")
                {
                    if (txtIDAsociacion.Text != string.Empty)
                    {
                        btnAgregarGeneral.Visible = false;
                        btnActualizarGeneral.Visible = true;
                    }
                    else
                    {
                        btnAgregarGeneral.Visible = true;
                        btnActualizarGeneral.Visible = false;
                    }
                }
                //if (e.Tab.Key == "tbpCoordenadas")
                //{
                //    if (txtIDCoordenada.Text != string.Empty)
                //        btnActualizarGeneral.Visible = true;
                //}
                //if (e.Tab.Key == "tbpCodigosRuta")
                //{
                //    if (txtIdCodigoRuta.Text != string.Empty)
                //        btnActualizarGeneral.Visible = true;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                
            }
           
            
        }

        private void dgvCodigosRuta_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            //e.Cancel = true;
            //e.DisplayPromptMsg = false;

            try
            {
                e.DisplayPromptMsg = false;
                DialogResult result = MessageBox.Show("Esta apunto de eliminar " + e.Rows.Count().ToString() + " registros permanentemente, no podrá recuperarlos posteriormente. ¿Seguro que desea continuar?", "Eliminación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    foreach (var item in e.Rows)
                    {
                        int ID = Convert.ToInt32(item.Cells["ID"].Value);
                        using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                        {
                            using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@TipoConsulta", 39);
                                command.Parameters.AddWithValue("@ID", ID);
                                command.Parameters.AddWithValue("@IDUsu", ClasesSGUV.Login.Id_Usuario);

                                command.CommandTimeout = 0;
                                command.Connection.Open();
                                command.ExecuteNonQuery();
                                command.Connection.Close();
                            }
                        }
                    }
                    e.Cancel = true;
                    dgvCodigosRuta.DataSource = null;
                    DataTable dt = new DataTable();
                    using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@TipoConsulta", 36);
                            command.CommandTimeout = 0;
                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = command;
                            da.Fill(dt);
                        }
                    }
                    dtCodigosRuta = dt;
                    dgvCodigosRuta.DataSource = dtCodigosRuta.Copy();
                    Nuevo();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        
        }

        private void dgvCoordenadasCliente_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            e.Cancel = true;
            e.DisplayPromptMsg = false;
        }

        private void dgvCodigosRuta_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Bands[0].Columns["ID"].Hidden = true;
                e.Layout.Bands[0].Columns["Codigo"].Header.Caption = "Codigo Ruta";
                e.Layout.Bands[0].Columns["Nombre"].Header.Caption = "Descripcion";
                e.Layout.Bands[0].Columns["Abreviatura"].Header.Caption = "Descripcion";
                e.Layout.Bands[0].Columns["nFrecuencia"].Header.Caption = "No. Frecuencia";
                e.Layout.Bands[0].Columns["bDiario"].Hidden = true;
                e.Layout.Bands[0].Columns["bSemanal"].Hidden = true;
                e.Layout.Bands[0].Columns["bQuincenal"].Hidden = true;
                e.Layout.Bands[0].Columns["bMensual"].Hidden = true;
                e.Layout.Bands[0].Columns["sFrecuencia"].Header.Caption = "Frecuencia";
                e.Layout.Bands[0].Columns["nUserAlta"].Hidden = true;
                e.Layout.Bands[0].Columns["dAlta"].Hidden = true;
                e.Layout.Bands[0].Columns["nUserUpdate"].Hidden = true;
                e.Layout.Bands[0].Columns["dUpdate"].Hidden = true;
                e.Layout.Bands[0].Columns["bActivo"].Hidden = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            
        }

        private void dgvCoordenadasCliente_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Bands[0].Columns["idClienteReparto"].Hidden = true;
                e.Layout.Bands[0].Columns["CodigoCliente"].Hidden = true;
                e.Layout.Bands[0].Columns["NombreCliente"].Hidden = true;
                e.Layout.Bands[0].Columns["sDescripcion"].Header.Caption = "Descripcion de Entrega";
                e.Layout.Bands[0].Columns["Latitud"].Hidden = true;
                e.Layout.Bands[0].Columns["Longitud"].Hidden = true;
                e.Layout.Bands[0].Columns["LtLn"].Header.Caption = "Coordenadas";
                e.Layout.Bands[0].Columns["FechaRegistro"].Hidden = true;
                e.Layout.Bands[0].Columns["sCodRuta"].Hidden = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            
        }

        private void cboClienteCoordenadas_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Bands[0].Columns["GroupName"].Hidden = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            
        }

        private void cboCodigoRutaAsociacion_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Bands[0].Columns["ID"].Hidden = true;
                e.Layout.Bands[0].Columns["nFrecuencia"].Header.Caption = "No. Visitas";
                e.Layout.Bands[0].Columns["bDiario"].Hidden = true;
                e.Layout.Bands[0].Columns["bSemanal"].Hidden = true;
                e.Layout.Bands[0].Columns["bQuincenal"].Hidden = true;
                e.Layout.Bands[0].Columns["bMensual"].Hidden = true;
                e.Layout.Bands[0].Columns["sFrecuencia"].Header.Caption = "Frecuencia";
                e.Layout.Bands[0].Columns["nUserAlta"].Hidden = true;
                e.Layout.Bands[0].Columns["dAlta"].Hidden = true;
                e.Layout.Bands[0].Columns["nUserUpdate"].Hidden = true;
                e.Layout.Bands[0].Columns["dUpdate"].Hidden = true;
                e.Layout.Bands[0].Columns["bActivo"].Hidden = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
           
        }

        private void cboClienteAsociacion_ValueChanged(object sender, EventArgs e)
        {
            if (cboClienteAsociacion.ActiveRow != null)
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 38);
                        command.Parameters.AddWithValue("@ClaveCliente", cboClienteAsociacion.ActiveRow.Cells["Codigo"].Value);
                        command.CommandTimeout = 0;

                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);

                        dgvAsociacionClientes.DataSource = dt;
                    }
                }
            }
        }

        private void cboClienteAsociacion_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["GroupName"].Hidden = true;
        }

        private void ultraTabPageControl2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void FrmCatalogosRutas_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void FrmCatalogosRutas_FormClosing(object sender, FormClosingEventArgs e)
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
