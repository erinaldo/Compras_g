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
    public partial class FrmAltaActualizacionCoordenadas : Form
    {

        ClasesSGUV.Logs log;
        public FrmAltaActualizacionCoordenadas()
        {
            InitializeComponent();
        }

        private void btnAgregarGeneral_Click(object sender, EventArgs e)
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

                dgvCoordenadasCliente.DataSource = null;
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
                txtLatitud.Text = string.Empty;
                txtLongitud.Text = string.Empty;
                txtUbicacionEntrega.Text = string.Empty;
                txtIDCoordenada.Text = string.Empty;
            }
            catch (Exception ex)
            {
                throw;

            }
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
            btnAgregarGeneral.Visible = true;
            btnActualizarGeneral.Visible = false;
            cboClienteCoordenadas.ActiveRow = null;
            cboClienteCoordenadas.Text = string.Empty;
            dgvCoordenadasCliente.DataSource = null;
        }

        private void btnActualizarGeneral_Click(object sender, EventArgs e)
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

        private void dgvCoordenadasCliente_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                
                throw;
            }
        }

        private void dgvCoordenadasCliente_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            try
            {
                e.DisplayPromptMsg = false;
                DialogResult result = MessageBox.Show("Esta apunto de eliminar " + e.Rows.Count().ToString() + " registros permanentemente, no podrá recuperarlos posteriormente. ¿Seguro que desea continuar?", "Eliminación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string ClaveCliente = "";
                    foreach (var item in e.Rows)
                    {
                        int ID = Convert.ToInt32(item.Cells["idClienteReparto"].Value);
                        ClaveCliente = Convert.ToString(item.Cells["CodigoCliente"].Value);
                        using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                        {
                            using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@TipoConsulta", 41);
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

                    dgvCoordenadasCliente.DataSource = null;
                    DataTable dt = new DataTable();
                    using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@TipoConsulta", 37);
                            command.Parameters.AddWithValue("@ClaveCliente", ClaveCliente);
                            command.CommandTimeout = 0;
                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = command;
                            da.Fill(dt);
                        }
                    }
                    dgvCoordenadasCliente.DataSource = dt;
                    //Nuevo();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void FrmAltaActualizacionCoordenadas_Load(object sender, EventArgs e)
        {
            try
            {
                //Se consultan todos los clientes
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 35);
                        command.Parameters.AddWithValue("@Sucursal", ClasesSGUV.Login.Sucursal);
                        command.CommandTimeout = 0;

                        DataTable dtClientes = new DataTable();

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dtClientes);

                        cboClienteCoordenadas.DataSource = dtClientes.Copy();
                        cboClienteCoordenadas.ValueMember = "Codigo";
                        cboClienteCoordenadas.DisplayMember = "Nombre";
                        cboClienteCoordenadas.ActiveRow = null;
                    }
                }

                log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

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
                e.Layout.Bands[0].Columns["Codigo"].Width = 100;
                e.Layout.Bands[0].Columns["Nombre"].Width = 364;
                e.Layout.Bands[0].Columns["GroupName"].Hidden = true;
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

        private void FrmAltaActualizacionCoordenadas_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void FrmAltaActualizacionCoordenadas_FormClosing(object sender, FormClosingEventArgs e)
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
