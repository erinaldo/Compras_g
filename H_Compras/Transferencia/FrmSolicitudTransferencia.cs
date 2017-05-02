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
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;

namespace H_Compras.Transferencia
{
    public partial class FrmSolicitudTransferencia : Constantes.frmEmpty//Form
    {
        public DataTable tbl_Articulos = new DataTable();
        DataTable DTMaster; DataTable DTDetalle;
        TipoTransferencia oTipoMov;
        bool bCarga = false;
        ClasesSGUV.Logs log;
        public enum TipoTransferencia
        {
              SolicitudAnexo = 1,
              SolicitudConfirmada = 2
        }
        public FrmSolicitudTransferencia()
        {
            InitializeComponent();
        }

        private void FrmSolicitudTransferencia_Load(object sender, EventArgs e)
        {
            try
            {
                bCarga = true;
                tbl_Articulos = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetArticulos, string.Empty, string.Empty);
                //DataTable dtCli = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListaClientes, string.Empty, string.Empty);
                //ClasesSGUV.Form.ControlsForms.Autocomplete(txtArticulo, tbl_Articulos.Copy(), "ItemCode");
                bCarga = true;
                cboArticulo.DataSource = tbl_Articulos;
                cboArticulo.DisplayMember = "ItemCode";
                cboArticulo.ValueMember = "ItemCode";

                

                DataTable tbl_AlmacenesTemp = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesVenta, string.Empty, string.Empty);
                DataTable tbl_Almacenes = new DataTable();
                tbl_Almacenes = (from obj in tbl_AlmacenesTemp.AsEnumerable()
                                 where obj.Field<string>("Whscode") != "26" && obj.Field<string>("Whscode") != "T 197"
                                 select obj
                                ).CopyToDataTable();

                cboAlmacen.DataSource = tbl_Almacenes;
                cboAlmacen.DisplayMember = "WhsName";
                cboAlmacen.ValueMember = "Whscode";
                cboAlmacen.SelectedIndex = 0;

                cboAlmacenOrigen.DataSource = tbl_Almacenes.Copy();
                cboAlmacenOrigen.DisplayMember = "WhsName";
                cboAlmacenOrigen.ValueMember = "Whscode";
                cboAlmacenOrigen.SelectedIndex = 0;

                DataTable tbl_Vendor = ClasesSGUV.DataSource.GetSource(45, cboAlmacen.SelectedValue.ToString(), string.Empty);
        
                cboVendedor.DataSource = tbl_Vendor;
                cboVendedor.DisplayMember = "Nombre";
                cboVendedor.ValueMember = "Codigo";
                cboVendedor.SelectedIndex = -1;
                
                

                DTMaster = new DataTable();
                DTMaster.Columns.Add("nID", typeof(Int32));
                DTMaster.Columns.Add("sCodAlmacen", typeof(string));
                DTMaster.Columns.Add("nFolio", typeof(Int32));
                DTMaster.Columns.Add("nTipo", typeof(Int32));
                DTMaster.Columns.Add("sAlmacen", typeof(string));
                DTMaster.Columns.Add("dFechaSolicitud", typeof(DateTime));
                DTMaster.Columns.Add("SlpCode", typeof(string));
                
                DTMaster.Columns.Add("dFechaAlta", typeof(DateTime));
                DTMaster.Columns.Add("nUserAlta", typeof(Int32));
                DTMaster.Columns.Add("dFechaUpdate", typeof(DateTime));
                DTMaster.Columns.Add("nUserUpdate", typeof(Int32));
                DTMaster.Columns.Add("bActivo", typeof(bool));

                DTDetalle = new DataTable();
                DTDetalle.Columns.Add("nID", typeof(Int32));
                DTDetalle.Columns.Add("sCodAlmacen", typeof(string));
                DTDetalle.Columns.Add("nFolio", typeof(Int32));
                DTDetalle.Columns.Add("nTipo", typeof(Int32));
                DTDetalle.Columns.Add("CardCode", typeof(string));
                DTDetalle.Columns.Add("sAlmacenOrig", typeof(string));
                DTDetalle.Columns.Add("sDscAlmacenOrig", typeof(string));
                DTDetalle.Columns.Add("sItemCode", typeof(string));
                DTDetalle.Columns.Add("sNombre", typeof(string));
                DTDetalle.Columns.Add("nCantidad", typeof(decimal));
               // DTDetalle.Columns.Add("Surtido", typeof(decimal));
                DTDetalle.Columns.Add("nStock", typeof(decimal));
                DTDetalle.Columns.Add("nSolicitado", typeof(decimal));
                DTDetalle.Columns.Add("bAutManual", typeof(bool));
               // DTDetalle.Columns.Add("NoTraspaso", typeof(Int32));
                
                
                oTipoMov = TipoTransferencia.SolicitudAnexo;
                //oTipoMov = TipoTransferencia.SolicitudConfirmada;

                //if (oTipoMov == TipoTransferencia.SolicitudTransferencia)
                //{
                //    lblVendedor.Visible = false;
                //    lblCliente.Visible = false;
                //    cboVendedor.SelectedIndex = -1;
                //    cboVendedor.Visible = false;
                //    cboCliente.ActiveRow = null;
                //    cboCliente.Visible = false;
 
                //}
                bCarga = false;
                exportarToolStripButton.Visible = false;
                actualizarToolStripButton.Visible = false;
                imprimirToolStripButton.Visible = false;
                nuevoToolStripButton.Click += btnNuevo_Click;
                guardarToolStripButton.Click += btnGuardar_Click;

                log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);
                bCarga = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        //private void txtArticulo_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        if (!string.IsNullOrEmpty(txtArticulo.Text))
        //        {
        //            var NomArticulo = (from item in tbl_Articulos.AsEnumerable()
        //                               where item.Field<string>("ItemCode").ToLower().Trim().Equals(txtArticulo.Text.ToLower().Trim())
        //                              select item.Field<string>("ItemName")).FirstOrDefault();
                    
        //            if(NomArticulo!=null)
        //                txtNombreArticulo.Text = NomArticulo;
        //        }
        //    }
        //}

        private void ConsultaCatalogos(string AlmacenDestino, string Vendedor, int Catalogo)
        {
            try
            {
                if (Catalogo == 1) //se trata del catalogo de vendedores
                {
 
                }
                else if (Catalogo == 2) //se trata del catalogo de clientes
                { 
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                if (oTipoMov == TipoTransferencia.SolicitudConfirmada)
                {
                    if (cboCliente.ActiveRow == null)
                    {
                        MessageBox.Show("Debe especificar un cliente", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cboCliente.Focus();
                        return;
                    }
                }
                if (cboArticulo.SelectedIndex==-1)
                {
                    MessageBox.Show("Especifique un codigo de articulo", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    cboArticulo.Focus();
                    return;
                }
                if (Convert.ToDecimal(txtCantidad.Value) == 0)
                {
                    MessageBox.Show("Especifique una cantidad", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtCantidad.Focus();
                    return;
                }
                if (cboAlmacenOrigen.SelectedIndex == -1)
                {
                    MessageBox.Show("Especifique un almacen origen", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    cboAlmacenOrigen.Focus();
                    return; 
                }

                if (cboAlmacen.SelectedValue.ToString() == cboAlmacenOrigen.Value.ToString())
                {
                    MessageBox.Show("El almacen origen no puede ser igual al almacen destino", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    cboAlmacenOrigen.Focus();
                    return;  
                }

                Cursor = Cursors.WaitCursor;
                ////Se ejecuta el proceso para cerrar las lineas que han cumplido el stock de un articulo especifico
                //try
                //{
                //    using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                //    {
                //        using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                //        {
                //            command.Connection.Open();
                //            command.CommandType = CommandType.StoredProcedure;
                //            command.Parameters.AddWithValue("@TipoConsulta", 15);
                //            command.Parameters.AddWithValue("@Articulo", cboArticulo.Value.ToString().Trim());
                            
                //            command.CommandTimeout = 0;
                //            command.ExecuteNonQuery();
                //            command.Connection.Close();
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //}

                DataSet dsValidaciones = new DataSet();

                string CodigoArticulo = cboArticulo.Value.ToString().Trim();
                int ValManual = 0;
                string Mensaje = string.Empty;
                decimal _Cantidad = Convert.ToDecimal(txtCantidad.Value);

                if (dgvDatos.DataSource != null)
                {
                    DataTable dtemp = (DataTable)dgvDatos.DataSource;
                    DataRow[] tmp = dtemp.Select("sItemCode = '" + CodigoArticulo + "'");
                    foreach (DataRow r in tmp)
                    {
                        _Cantidad += Convert.IsDBNull(r["nCantidad"]) ? 0 : Convert.ToDecimal(r["nCantidad"]);
                    }
                }

                dsValidaciones = Validaciones(cboAlmacen.SelectedValue.ToString(), CodigoArticulo, 
                                              _Cantidad, cboAlmacenOrigen.Value.ToString());                    
                bool Terminar = false;
                foreach (DataTable dtt in dsValidaciones.Tables)
                {
                    if (Terminar)
                        break;
                    Mensaje = string.Empty;
                    foreach (DataRow rw in dtt.Rows)
                    {
                        if (rw.Table.Columns.Contains("Mensaje"))
                        {
                            Mensaje = rw["Mensaje"].ToString();
                        }
                        else if (rw.Table.Columns.Contains("Validacion"))
                        {
                            ValManual = Convert.ToInt32(rw["Validacion"]);
                        }
                        if (Mensaje != "")
                        {
                            Terminar = true;
                            break;
                        }
                    }
                }

                if (Terminar)
                {
                    Cursor = Cursors.Default;
                    MessageBox.Show(Mensaje, "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
              
                string Articulo = txtNombreArticulo.Text.Trim();
                decimal Cantidad = Convert.ToDecimal(txtCantidad.Value);
                decimal Stock = 0;
                if (txtStock.Text.Trim() != string.Empty)
                    Stock = Convert.ToDecimal(txtStock.Text);
                decimal Solicitado = 0;
                if (txtSolicitado.Text.Trim() != string.Empty)
                    Solicitado = Convert.ToDecimal(txtSolicitado.Text);
                decimal IdealReubic = 0;
                if (txtReubicacion.Text.Trim() != string.Empty)
                    IdealReubic = Convert.ToDecimal(txtReubicacion.Text);

                int NumMenor = -1;
                foreach (DataRow item in DTDetalle.Rows)
                {
                    int x = Convert.IsDBNull(item["nID"]) ? -1 : Convert.ToInt32(item["nID"]);
                    if (x <= NumMenor)
                        NumMenor = x - 1;
                }


                DataRow dr = DTDetalle.NewRow();
                dr["nID"] = NumMenor;
                dr["sCodAlmacen"] = -1;
                dr["nFolio"] = -1;
                dr["nTipo"] = (int)oTipoMov;
                if(oTipoMov== TipoTransferencia.SolicitudConfirmada)
                    dr["CardCode"] = cboCliente.Value.ToString();
                dr["sAlmacenOrig"] = cboAlmacenOrigen.Value.ToString();
                dr["sDscAlmacenOrig"] = cboAlmacenOrigen.Text.ToString();
                dr["sItemCode"] = CodigoArticulo;
                dr["sNombre"] = Articulo;
                dr["nCantidad"] = Cantidad;
                dr["nStock"] = Stock;
                dr["nSolicitado"] = Solicitado;
                dr["bAutManual"] = ValManual == 1;
               
                
                DTDetalle.Rows.Add(dr);
                dgvDatos.DataSource = null;
                dgvDatos.DataSource = DTDetalle;

                cboArticulo.SelectedIndex = -1;
                txtNombreArticulo.Text = string.Empty;
                txtStock.Text = string.Empty;
                txtSolicitado.Text = string.Empty;
                txtReubicacion.Text = string.Empty;
                txtCantidad.Value = 0;
                cboCliente.SelectedRow = null;
                cboCliente.Text = string.Empty;
                cboAlmacenOrigen.SelectedIndex = -1;

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private string ConsultaArticuloEnFoliosAbiertos(string CodArt)
        {
            string _msg = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                    {
                        command.Connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 9);
                        command.Parameters.AddWithValue("@CodAlmacen", Convert.ToString(cboAlmacen.SelectedValue.ToString().Trim()));
                        command.Parameters.AddWithValue("@Tipo", (Int32)oTipoMov);
                        command.Parameters.AddWithValue("@Articulo", CodArt);
                        SqlParameter parameter = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        parameter.Direction = ParameterDirection.Output;
                        parameter.Size = 500;
                        command.Parameters.Add(parameter);
                        command.CommandTimeout = 0;

                        command.ExecuteNonQuery();
                        command.Connection.Close();
                        _msg = Convert.IsDBNull(command.Parameters["@Mensaje"].Value) ? "" : Convert.ToString(command.Parameters["@Mensaje"].Value);
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            return _msg;
        }

        private DataSet Validaciones(string AlmacenDestino, string CodArt, decimal Cantidad, string AlmacenOrigen)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                    {
                        command.Connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 9);
                        command.Parameters.AddWithValue("@CodAlmacen", Convert.ToString(cboAlmacen.SelectedValue.ToString().Trim()));
                        command.Parameters.AddWithValue("@Tipo", (Int32)oTipoMov);
                        command.Parameters.AddWithValue("@Articulo", CodArt);
                        command.Parameters.AddWithValue("@Cantidad", Cantidad);
                        command.Parameters.AddWithValue("@CodAlmacenOrig", Convert.ToString(cboAlmacenOrigen.Value.ToString().Trim()));

                        SqlParameter parameter = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        parameter.Direction = ParameterDirection.Output;
                        parameter.Size = 500;
                        command.Parameters.Add(parameter);
                        command.CommandTimeout = 0;

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return ds;//_msg;
        }

        private DataTable ValidaCantidadSolicitadaStock(string AlmacenDestino, string CodArt, decimal Cantidad, string AlmacenOrigen)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                    {
                        command.Connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 17);
                        command.Parameters.AddWithValue("@CodAlmacen", Convert.ToString(cboAlmacen.SelectedValue.ToString().Trim()));
                        command.Parameters.AddWithValue("@Tipo", (Int32)oTipoMov);
                        command.Parameters.AddWithValue("@Articulo", CodArt);
                        command.Parameters.AddWithValue("@Cantidad", Cantidad);
                        command.Parameters.AddWithValue("@CodAlmacenOrig", Convert.ToString(cboAlmacenOrigen.Value.ToString().Trim()));

                        SqlParameter parameter = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        parameter.Direction = ParameterDirection.Output;
                        parameter.Size = 500;
                        command.Parameters.Add(parameter);
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

        private DataTable ValorValidacionManual()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                    {
                        command.Connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 18);
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


        private void dgvDatos_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
        {
            try
            {
                e.DisplayPromptMsg = false;
                DialogResult result = MessageBox.Show("Esta apunto de eliminar " + e.Rows.Count().ToString() + " articulos permanentemente, no podrá recuperarlos posteriormente. ¿Seguro que desea continuar?", "Eliminación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    //dtFcrs.Rows.Clear(); dtFcrs.AcceptChanges();
                    DTDetalle = ((DataTable)dgvDatos.DataSource).Copy();

                    foreach (var item in e.Rows)
                    {
                        try
                        {
                            int ID = Convert.ToInt32(item.Cells["nID"].Value);
                            if (ID > 0)
                            {
                                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                                {
                                    using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                                    {
                                        command.CommandType = CommandType.StoredProcedure;
                                        command.Parameters.AddWithValue("@TipoConsulta", 5);
                                        command.Parameters.AddWithValue("@ID", ID);
                                        command.CommandTimeout = 0;
                                        command.Connection.Open();
                                        command.ExecuteNonQuery();
                                        command.Connection.Close();
                                    }
                                }

                                foreach (DataRow dr in DTDetalle.Rows)
                                {
                                    if (dr["nID"].ToString() == ID.ToString())
                                        dr.Delete();
                                }
                                DTDetalle.AcceptChanges();
                            }
                            else
                            {
                                foreach (DataRow dr in DTDetalle.Rows)
                                {
                                    if (dr["nID"].ToString() == ID.ToString())
                                        dr.Delete();
                                }
                                DTDetalle.AcceptChanges();

                            }

                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    e.Cancel = true;
                    e.DisplayPromptMsg = false;
                    dgvDatos.DataSource = DTDetalle;

                }
                else
                {
                    e.Cancel = true;
                    e.DisplayPromptMsg = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.Default;
                //if (oTipoMov == TipoTransferencia.SolicitudConfirmada)
                //{
                if (rbtVtaConfirm.Checked)
                {
                    if (cboVendedor.SelectedIndex == -1)
                    {
                        MessageBox.Show("Debe especificar un vendedor", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cboVendedor.Focus();
                        return;
                    }
                }
                
                //}
                if (dgvDatos.Rows.Count <= 0)
                {
                    MessageBox.Show("Debe especificar al menos un articulo", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cboCliente.Focus();
                    return;
                }

                //se inicia el objeto transacion para confirmar si todo fue correcto o realizar un Rollback para deshacer movimientos
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("TransaccionSAVE");

                    try
                    {
                        int iCaptura = -1;
                        if (lblNoFolio.Text != string.Empty)
                            iCaptura = Convert.ToInt32(lblNoFolio.Text);


                        //-------------SE INSERTA EL ENCABEZADO DE LA CAPTURA-------------
                        using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                        {
                            command.Transaction = transaction;
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@TipoConsulta", 1);
                            command.Parameters.AddWithValue("@CodAlmacen", cboAlmacen.SelectedValue.ToString());
                            if (iCaptura < 0)
                            {
                                SqlParameter parameter = new SqlParameter("@iCaptura", SqlDbType.Int);
                                parameter.Direction = ParameterDirection.Output; command.Parameters.Add(parameter);
                            }
                            else
                            {   command.Parameters.AddWithValue("@iCaptura", iCaptura); }
                            command.Parameters.AddWithValue("@Tipo", (int) oTipoMov);

                            
                            command.Parameters.AddWithValue("@Almacen", cboAlmacen.Text.ToString());
                            command.Parameters.AddWithValue("@FechaSolicitud", dtpFechaSolicitud.Value);
                            if (oTipoMov == TipoTransferencia.SolicitudConfirmada)
                            {
                            command.Parameters.AddWithValue("@CodVendedor", cboVendedor.SelectedValue);
                            //command.Parameters.AddWithValue("@CodCliente", cboCliente.ActiveRow.Cells["CardCode"].Value);
                            }
                            command.Parameters.AddWithValue("@Usuario", ClasesSGUV.Login.Id_Usuario);

                            command.ExecuteNonQuery();

                            iCaptura = Convert.IsDBNull(command.Parameters["@iCaptura"].Value) ? -1 : Convert.ToInt32(command.Parameters["@iCaptura"].Value);
                        }

                        if (iCaptura > 0)
                        {
                            //-------------SE ELIMINA EL DETALLE -------------
                            using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                            {
                                command.Transaction = transaction;
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@TipoConsulta", 2);
                                command.Parameters.AddWithValue("@CodAlmacen", cboAlmacen.SelectedValue.ToString());
                                command.Parameters.AddWithValue("@iCaptura", iCaptura);
                                command.Parameters.AddWithValue("@Tipo", (int)oTipoMov);   
                                command.ExecuteNonQuery();
                            }
                            //-------------SE INSERTA EL DETALLE -------------
                            foreach (UltraGridRow rw in dgvDatos.Rows)
                            {
                                string Cliente = Convert.IsDBNull(rw.Cells["CardCode"].Value) ? "" : Convert.ToString(rw.Cells["CardCode"].Value);
                                string CodArt = Convert.IsDBNull(rw.Cells["sItemCode"].Value) ? "" : Convert.ToString(rw.Cells["sItemCode"].Value);
                                string NombreArt = Convert.IsDBNull(rw.Cells["sNombre"].Value) ? "" : Convert.ToString(rw.Cells["sNombre"].Value);
                                decimal Cantidad = Convert.IsDBNull(rw.Cells["nCantidad"].Value) ? 0 : Convert.ToDecimal(rw.Cells["nCantidad"].Value);
                                decimal Stock = Convert.IsDBNull(rw.Cells["nStock"].Value) ? 0 : Convert.ToDecimal(rw.Cells["nStock"].Value);
                                decimal Solicitado = Convert.IsDBNull(rw.Cells["nSolicitado"].Value) ? 0 : Convert.ToDecimal(rw.Cells["nSolicitado"].Value);
                                bool AutManual = Convert.IsDBNull(rw.Cells["bAutManual"].Value) ? false : Convert.ToBoolean(rw.Cells["bAutManual"].Value);
                                string AlmacenOrig = Convert.IsDBNull(rw.Cells["sAlmacenOrig"].Value) ? "" : Convert.ToString(rw.Cells["sAlmacenOrig"].Value);

                                using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                                {
                                    command.Transaction = transaction;
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.AddWithValue("@TipoConsulta", 3);
                                    command.Parameters.AddWithValue("@CodAlmacen", cboAlmacen.SelectedValue.ToString());
                                    command.Parameters.AddWithValue("@iCaptura", iCaptura);
                                    command.Parameters.AddWithValue("@Tipo", (int)oTipoMov);
                                    command.Parameters.AddWithValue("@CodCliente", Cliente);
                                    command.Parameters.AddWithValue("@Articulo", CodArt);
                                    command.Parameters.AddWithValue("@NomArticulo", NombreArt);
                                    command.Parameters.AddWithValue("@Cantidad", Cantidad);
                                    command.Parameters.AddWithValue("@ContStock", Stock);
                                    command.Parameters.AddWithValue("@ContSolicitado", Solicitado);
                                    command.Parameters.AddWithValue("@ValidacionManual", AutManual);
                                    command.Parameters.AddWithValue("@CodAlmacenOrig", AlmacenOrig);
                                    command.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                            Cursor = Cursors.Default;
                            MessageBox.Show("Captura guardado correctamente", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblNoFolio.Text = iCaptura.ToString();
                            ConsultaCapturaID(iCaptura);

                            //if (Auxiliar <= 0)
                            //{
                            //    lblNoFolio.Text = iCaptura.ToString();
                            //    ConsultaCapturaID(iCaptura);
                            //}
                            //else
                            //{
                            //    MessageBox.Show("Folio auxiliar <" + iCaptura.ToString() + "> creado correctamente, ingrese al modulo de autorizaciones para verificar su estado");
                            //    btnNuevo.PerformClick();
                            //}
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                    catch (Exception ex)
                    {
                        Cursor = Cursors.Default;
                        MessageBox.Show(ex.Message.ToString());
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex1)
                        {
                            Cursor = Cursors.Default;
                            MessageBox.Show(ex1.Message);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(ex.Message.ToString());
            }
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
                        if (cboAlmacen.SelectedIndex == -1)
                        {
                            MessageBox.Show("Debe especificarse una sucursal", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        DTDetalle.Rows.Clear();
                        DTDetalle.AcceptChanges();
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
                    using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 4);
                        command.Parameters.AddWithValue("@CodAlmacen", Convert.ToString(cboAlmacen.SelectedValue.ToString().Trim()));
                        command.Parameters.AddWithValue("@iCaptura", iCaptura);
                        command.Parameters.AddWithValue("@Tipo", (Int32)oTipoMov);
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
                        dtpFechaSolicitud.Value = (Convert.IsDBNull(DS.Tables[0].Rows[0]["dFechaSolicitud"]) ? Convert.ToDateTime("01-01-1900") : Convert.ToDateTime(DS.Tables[0].Rows[0]["dFechaSolicitud"]));
                        //if (oTipoMov == TipoTransferencia.SolicitudConfirmada)
                        //{
                        string Vendedor = (Convert.IsDBNull(DS.Tables[0].Rows[0]["SlpCode"]) ? string.Empty : Convert.ToString(DS.Tables[0].Rows[0]["SlpCode"]));
                        if (Vendedor != string.Empty)
                            cboVendedor.SelectedValue = Vendedor;
                            //cboCliente.Value = (Convert.IsDBNull(DS.Tables[0].Rows[0]["CardCode"]) ? "" : Convert.ToString(DS.Tables[0].Rows[0]["CardCode"]));
                        //}
                        lblNoFolio.Text = DS.Tables[0].Rows[0]["nFolio"].ToString();
                        dgvDatos.DataSource = DS.Tables[1];
                    }
                    else
                    {
                        MessageBox.Show("No existe ningun folio con el numero especificado");
                        Nuevo();
                        lblNoFolio.Text = string.Empty;
                    }
                }


            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        private void dgvDatos_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Bands[0].Columns["nID"].Hidden = true;
                e.Layout.Bands[0].Columns["sCodAlmacen"].Hidden = true;
                e.Layout.Bands[0].Columns["nFolio"].Hidden = true;
                e.Layout.Bands[0].Columns["nTipo"].Hidden = true;
                e.Layout.Bands[0].Columns["CardCode"].Header.Caption = "Cliente";

                if(oTipoMov == TipoTransferencia.SolicitudConfirmada)
                    e.Layout.Bands[0].Columns["CardCode"].Hidden = false;
                else
                    e.Layout.Bands[0].Columns["CardCode"].Hidden = true;

                e.Layout.Bands[0].Columns["sAlmacenOrig"].Hidden = true;
                e.Layout.Bands[0].Columns["sDscAlmacenOrig"].Header.Caption = "Origen";
                e.Layout.Bands[0].Columns["sItemCode"].Header.Caption = "Articulo";
                e.Layout.Bands[0].Columns["sItemCode"].Width = 100;//Header.Caption = "Articulo";
                e.Layout.Bands[0].Columns["sNombre"].Header.Caption = "Nombre";
                e.Layout.Bands[0].Columns["sNombre"].Width = 250;
                e.Layout.Bands[0].Columns["nCantidad"].Header.Caption = "Cantidad";
                e.Layout.Bands[0].Columns["nCantidad"].Width = 80;

                e.Layout.Bands[0].Columns["nStock"].Hidden = true;
                e.Layout.Bands[0].Columns["nSolicitado"].Hidden = true;
                e.Layout.Bands[0].Columns["bAutManual"].Hidden = true;
                

                foreach (var item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Activation.NoEdit;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void txtArticulo_Leave(object sender, EventArgs e)
        {

        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Nuevo();
            lblNoFolio.Text = string.Empty;
            DTMaster.Rows.Clear(); DTMaster.AcceptChanges();
            DTDetalle.Rows.Clear(); DTDetalle.AcceptChanges();
            dgvDatos.DataSource = DTDetalle.Copy();
        }

        private void Nuevo()
        {
            //lblNoFolio.Text = string.Empty;
            cboArticulo.SelectedIndex = -1;
            txtNombreArticulo.Text = string.Empty;
            txtCantidad.Value = 0;
            txtStock.Text = string.Empty;
            txtSolicitado.Text = string.Empty;
            txtReubicacion.Text = string.Empty;
            dtpFechaSolicitud.Value = DateTime.Now;

            cboVendedor.SelectedIndex = -1;
            cboCliente.ActiveRow = null;
            cboCliente.Text = string.Empty;
            cboAlmacenOrigen.SelectedIndex = -1;

            DTMaster.Rows.Clear(); DTMaster.AcceptChanges();
            DTDetalle.Rows.Clear(); DTDetalle.AcceptChanges();
            dgvDatos.DataSource = DTDetalle.Copy();
        }

        private void cboArticulo_ValueChanged(object sender, EventArgs e)
        {

            try
            {
                if (!bCarga && cboArticulo.SelectedIndex != -1) {
                    string Codigo = cboArticulo.Value.ToString();

                    var NomArticulo = (from item in tbl_Articulos.AsEnumerable()
                                       where item.Field<string>("ItemCode").ToLower().Trim().Equals(Codigo.ToLower().Trim())
                                       select item.Field<string>("ItemName")).FirstOrDefault();

                    if (NomArticulo != null)
                        txtNombreArticulo.Text = NomArticulo;
                    //--------------------------------------------------------------------------------------------
                   
                    //--------------------------------------------------------------------------------------------
                    //SE CONSULTA EL ALMACEN ORIGEN POSIBLE
                    DataTable dt = new DataTable();
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                        {
                            using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@TipoConsulta", 16);
                                command.Parameters.AddWithValue("@CodAlmacen", Convert.ToString(cboAlmacen.SelectedValue.ToString().Trim()));
                                command.Parameters.AddWithValue("@Articulo", Codigo);
                                command.CommandTimeout = 0;

                                SqlDataAdapter da = new SqlDataAdapter();
                                da.SelectCommand = command;
                                da.Fill(dt);

                                if (dt.Rows.Count > 0)
                                {
                                    string CodAlmOrig = Convert.IsDBNull(dt.Rows[0]["AlmacenComprador"]) ? string.Empty : Convert.ToString(dt.Rows[0]["AlmacenComprador"]);
                                    if (CodAlmOrig != string.Empty)
                                        cboAlmacenOrigen.Value = CodAlmOrig;


                                }
                                else
                                    cboAlmacenOrigen.SelectedIndex = -1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }

                    if (cboAlmacenOrigen.SelectedIndex != -1)
                    {
                        //Se consulta el stock y el solicitado
                        DataTable tbl_stocks = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.StockAlmacenArticulo, cboArticulo.Value.ToString(), cboAlmacenOrigen.Value.ToString());
                        if (tbl_stocks.Rows.Count > 0)
                        {
                            txtStock.Text = Convert.IsDBNull(tbl_stocks.Rows[0]["OnHand"]) ? "" : Convert.ToDecimal(tbl_stocks.Rows[0]["OnHand"]).ToString("###,###,###,##0.00");
                            txtSolicitado.Text = Convert.IsDBNull(tbl_stocks.Rows[0]["OnOrder"]) ? "" : Convert.ToDecimal(tbl_stocks.Rows[0]["OnOrder"]).ToString("###,###,###,##0.00");
                        }
                        else
                        {
                            txtStock.Text = string.Empty;
                            txtSolicitado.Text = string.Empty;
                        }
                        //--------------------------------------------------------------------------------------------

                        //SE CONSULTA EL IDEAL DE REUBICACION
                        DataTable tbl_IdealReubic = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.IdealReubicacionArticulo, cboArticulo.Value.ToString(), cboAlmacenOrigen.Value.ToString());
                        if (tbl_IdealReubic.Rows.Count > 0)
                        {
                            txtReubicacion.Text = Convert.IsDBNull(tbl_IdealReubic.Rows[0]["IdealReubicacion"]) ? "" : Convert.ToDecimal(tbl_IdealReubic.Rows[0]["IdealReubicacion"]).ToString("###,###,###,##0.00");
                        }
                        else
                            txtReubicacion.Text = string.Empty;
                    }


                }
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void txtCantidad_Click(object sender, EventArgs e)
        {
            txtCantidad.SelectAll();
        }

        private void txtCantidad_Enter(object sender, EventArgs e)
        {
            txtCantidad.SelectAll();
        }

        private void FrmSolicitudVtaConfirmada_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void FrmSolicitudVtaConfirmada_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                log.Fin();
            }
            catch (Exception)
            {

            }
        }

        private void rbtSolTransfer_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtSolTransfer.Checked && !bCarga)
            {
                if (MessageBox.Show("Si cambia de tipo de solicitud se borrará toda informacion capturada, ¿Seguro que desea continuar?", "Confirmación",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (rbtSolTransfer.Checked)
                        oTipoMov = TipoTransferencia.SolicitudAnexo;

                    Nuevo();
                    cboVendedor.Enabled = false;
                    cboCliente.Enabled = false;
                }
                else
                {
                    bCarga = true;
                    rbtVtaConfirm.Checked = true;
                    bCarga = false;
                }
            }
            else if (rbtSolTransfer.Checked && bCarga)
            {
                if (rbtSolTransfer.Checked)
                    oTipoMov = TipoTransferencia.SolicitudAnexo;
                cboVendedor.Enabled = false;
                cboCliente.Enabled = false;
            }
        }

        private void rbtVtaConfirm_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtVtaConfirm.Checked && !bCarga)
            {
                if (MessageBox.Show("Si cambia de tipo de solicitud se borrará todoa informacion capturada, ¿Seguro que desea continuar?", "Confirmación",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {

                    if (rbtVtaConfirm.Checked)
                        oTipoMov = TipoTransferencia.SolicitudConfirmada;

                    Nuevo();
                    cboVendedor.Enabled = true;
                    cboCliente.Enabled = true;
                }
                else
                {
                    bCarga = true;
                    rbtSolTransfer.Checked = true;
                    bCarga = false;
                }
            }
            else if (rbtVtaConfirm.Checked && bCarga)
            {
                if (rbtVtaConfirm.Checked)
                    oTipoMov = TipoTransferencia.SolicitudConfirmada;
                cboVendedor.Enabled = false;
                cboCliente.Enabled = false;
            }


        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dtpFechaSolicitud_ValueChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void cboAlmacenOrigen_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!bCarga && cboAlmacenOrigen.SelectedIndex!=-1 && cboArticulo.SelectedIndex!=-1)
                {
                    //Se consulta el stock y el solicitado
                    DataTable tbl_stocks = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.StockAlmacenArticulo, cboArticulo.Value.ToString(), cboAlmacenOrigen.Value.ToString());
                    if (tbl_stocks.Rows.Count > 0)
                    {
                        txtStock.Text = Convert.IsDBNull(tbl_stocks.Rows[0]["OnHand"]) ? "" : Convert.ToDecimal(tbl_stocks.Rows[0]["OnHand"]).ToString("###,###,###,##0.00");
                        txtSolicitado.Text = Convert.IsDBNull(tbl_stocks.Rows[0]["OnOrder"]) ? "" : Convert.ToDecimal(tbl_stocks.Rows[0]["OnOrder"]).ToString("###,###,###,##0.00");
                    }
                    else
                    {
                        txtStock.Text = string.Empty;
                        txtSolicitado.Text = string.Empty;
                    }
                    //--------------------------------------------------------------------------------------------

                    //SE CONSULTA EL IDEAL DE REUBICACION
                    DataTable tbl_IdealReubic = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.IdealReubicacionArticulo, cboArticulo.Value.ToString(), cboAlmacenOrigen.Value.ToString());
                    if (tbl_IdealReubic.Rows.Count > 0)
                    {
                        txtReubicacion.Text = Convert.IsDBNull(tbl_IdealReubic.Rows[0]["IdealReubicacion"]) ? "" : Convert.ToDecimal(tbl_IdealReubic.Rows[0]["IdealReubicacion"]).ToString("###,###,###,##0.00");
                    }
                    else
                        txtReubicacion.Text = string.Empty;
 
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        private void cboVendedor_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!bCarga && cboVendedor.SelectedIndex != -1 && oTipoMov == TipoTransferencia.SolicitudConfirmada)
                {
                    cboVendedor.Text = string.Empty;

                    DataTable dtCli = ClasesSGUV.DataSource.GetSource(46, cboVendedor.SelectedValue.ToString(), string.Empty);
                    cboCliente.DataSource = dtCli;
                    cboCliente.DisplayMember = "CardName";
                    cboCliente.ValueMember = "CardCode";
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void cboAlmacen_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!bCarga && cboAlmacen.SelectedIndex != -1 && oTipoMov == TipoTransferencia.SolicitudConfirmada)
                {
                    cboVendedor.Text = string.Empty;
                    cboCliente.Text = string.Empty;
                    cboCliente.DataSource = null;
                    cboVendedor.DataSource = null;

                    DataTable tbl_Vendor = ClasesSGUV.DataSource.GetSource(45, cboAlmacen.SelectedValue.ToString(), string.Empty);

                    cboVendedor.DataSource = tbl_Vendor;
                    cboVendedor.DisplayMember = "Nombre";
                    cboVendedor.ValueMember = "Codigo";
                    cboVendedor.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
