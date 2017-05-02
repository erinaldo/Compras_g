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
    public partial class FrmConfirmacionTransferencia : Constantes.frmEmpty//Form
    {
        ClasesSGUV.Logs log;
        bool bCarga = false;
        BindingSource rEncabezado = new BindingSource();
        BindingSource rDetalle = new BindingSource();

        public FrmConfirmacionTransferencia()
        {
            InitializeComponent();
        }

        private void FrmConfirmacionTransferencia_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dtEstatus = new DataTable();
                dtEstatus.Columns.Add("Codigo", typeof(string));
                dtEstatus.Columns.Add("Nombre", typeof(string));

                DataRow dr1 = dtEstatus.NewRow();
                dr1["Codigo"] = "A"; dr1["Nombre"] = "Activo"; dtEstatus.Rows.Add(dr1);
                DataRow dr2 = dtEstatus.NewRow();
                dr2["Codigo"] = "O"; dr2["Nombre"] = "Cerrado"; dtEstatus.Rows.Add(dr2);
                DataRow dr3 = dtEstatus.NewRow();
                dr3["Codigo"] = "C"; dr3["Nombre"] = "Cancelado"; dtEstatus.Rows.Add(dr3);

                cboEstatus.DataSource = dtEstatus;
                cboEstatus.DisplayMember = "Nombre";
                cboEstatus.ValueMember = "Codigo";
                cboEstatus.SelectedIndex = 0;


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

                exportarToolStripButton.Visible = false;
                actualizarToolStripButton.Visible = false;
                imprimirToolStripButton.Visible = false;
                guardarToolStripButton.Click += btnGuardar_Click;
                buscarStripButton.Click += btnConsultarSolicitudes_Click;
                buscarStripButton.Enabled = true;
                nuevoToolStripButton.Visible = false;
                
                log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                GridKeyActionMapping mapping = new GridKeyActionMapping(Keys.Enter, UltraGridAction.BelowCell, (UltraGridState)0, UltraGridState.InEdit, SpecialKeys.All, (SpecialKeys)0);
                this.dgvArticulos.KeyActionMappings.Add(mapping);
                //guardarToolStripButton.Visible = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnConsultarSolicitudes_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                lblNoFolio.Text = string.Empty; lblID.Text = string.Empty; 
                chbAprobado.Checked = false;
                chbAprobado.Enabled = true;

                int TipoSol = 1;
                if (rbtSolTransfer.Checked)
                    TipoSol = 1;
                else if (rbtVtaConfirm.Checked)
                    TipoSol = 2;
                //------------------------------//
                //SE ACTUALIZAN DEL CAMPO CANTIDAD A SURTIDO DE LOS QUE NO SE HAYAN DETONADO EN EL EVENTO DEL GRID
                //UNA VEZ QUE SE HAN GENERADO LAS TRANSFERENCIAS
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                    {
                        command.Connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 13);
                        command.CommandTimeout = 0;
                        command.ExecuteNonQuery();
                        command.Connection.Close();
                    }
                }

                //********se consultan las solicitudes*********/
                DataSet dsFolios = new DataSet();
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 6);
                        command.Parameters.AddWithValue("@Tipo", TipoSol);
                        command.Parameters.AddWithValue("@CodAlmacen", Convert.ToString(cboAlmacen.SelectedValue.ToString().Trim()));
                        command.Parameters.AddWithValue("@Desde", dtpDesde.Value);
                        command.Parameters.AddWithValue("@Hasta", dtpHasta.Value);
                        command.CommandTimeout = 0;

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dsFolios);
                    }
                }

                dsFolios.Tables[0].TableName = "Encabezado";
                dsFolios.Tables[1].TableName = "Detalle";

                rEncabezado = new BindingSource();
                rDetalle = new BindingSource();

                DataRelation relation = new DataRelation("Relacion", dsFolios.Tables[0].Columns["PK"], dsFolios.Tables[1].Columns["PK"]);
                dsFolios.Relations.Add(relation);


                rEncabezado.DataSource = dsFolios;
                rEncabezado.DataMember = "Encabezado";
                rDetalle.DataSource = rEncabezado;
                rDetalle.DataMember = "Relacion";
                //-------------------------------------------------
                dgvFolios.DataSource = rEncabezado;
                dgvArticulos.DataSource = rDetalle;
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void dgvFolios_AfterRowActivate(object sender, EventArgs e)
        {
            try
            {
                //chbDespacho.Checked = false;
                //chbCompleto.Checked = false;
                //txtNumTraspaso.Text = string.Empty;

                lblNoFolio.Text = (sender as UltraGrid).Rows[(sender as UltraGrid).ActiveRow.Index].Cells["nFolio"].Value.ToString();
                lblID.Text = (sender as UltraGrid).Rows[(sender as UltraGrid).ActiveRow.Index].Cells["nID"].Value.ToString();
                //txtNoTraspaso.Text = (sender as UltraGrid).Rows[(sender as UltraGrid).ActiveRow.Index].Cells["NoTraspaso"].Value.ToString();
                cboEstatus.SelectedValue = (sender as UltraGrid).Rows[(sender as UltraGrid).ActiveRow.Index].Cells["sEstatus"].Value.ToString();

                //bCarga = true;
                //chbAprobado.Checked = Convert.IsDBNull((sender as UltraGrid).Rows[(sender as UltraGrid).ActiveRow.Index].Cells["bAprobado"].Value) ? false :
                //                      Convert.ToBoolean((sender as UltraGrid).Rows[(sender as UltraGrid).ActiveRow.Index].Cells["bAprobado"].Value);
                //bCarga = false;
                //if (chbAprobado.Checked)
                //{
                //    chbAprobado.Enabled = false;
                //    dgvArticulos.DisplayLayout.Bands[0].Columns["nCantidad"].CellActivation = Activation.NoEdit;
                //}
                //else
                //{
                //    chbAprobado.Enabled = true;
                //    dgvArticulos.DisplayLayout.Bands[0].Columns["nCantidad"].CellActivation = Activation.AllowEdit;
                //}

                //chbDespacho.Checked = Convert.IsDBNull((sender as UltraGrid).Rows[(sender as UltraGrid).ActiveRow.Index].Cells["bDespachado"].Value) ? false :
                //                      Convert.ToBoolean((sender as UltraGrid).Rows[(sender as UltraGrid).ActiveRow.Index].Cells["bDespachado"].Value);
                //chbCompleto.Checked = Convert.IsDBNull((sender as UltraGrid).Rows[(sender as UltraGrid).ActiveRow.Index].Cells["bCompleto"].Value) ? false :
                //                      Convert.ToBoolean((sender as UltraGrid).Rows[(sender as UltraGrid).ActiveRow.Index].Cells["bCompleto"].Value);
                //txtNumTraspaso.Text = Convert.IsDBNull((sender as UltraGrid).Rows[(sender as UltraGrid).ActiveRow.Index].Cells["nTraspaso"].Value) ? string.Empty :
                //                      Convert.ToString((sender as UltraGrid).Rows[(sender as UltraGrid).ActiveRow.Index].Cells["nTraspaso"].Value);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void dgvFolios_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Override.ExpansionIndicator = ShowExpansionIndicator.Never;
                e.Layout.Override.HeaderClickAction = HeaderClickAction.Select;
                e.Layout.Override.SelectTypeCol = SelectType.None;
                e.Layout.Override.SelectedRowAppearance.BackColor = Color.Transparent;

                e.Layout.Bands[0].Columns["nID"].Hidden = true;
                e.Layout.Bands[0].Columns["sCodAlmacen"].Hidden = true;
                e.Layout.Bands[0].Columns["nTipo"].Hidden = true;
                e.Layout.Bands[0].Columns["sAlmacen"].Hidden = true;
                e.Layout.Bands[0].Columns["PK"].Hidden = true;
                //e.Layout.Bands[0].Columns["bDespachado"].Hidden = true;
                //e.Layout.Bands[0].Columns["bCompleto"].Hidden = true;
                //e.Layout.Bands[0].Columns["nTraspaso"].Hidden = true;
                //if (rbtSolTransfer.Checked)
                //{
                //    e.Layout.Bands[0].Columns["Vendedor"].Hidden = true;
                //    e.Layout.Bands[0].Columns["Cliente"].Hidden = true;
                //}
                //else
                //{
                //e.Layout.Bands[0].Columns["Vendedor"].Hidden = false;
                //e.Layout.Bands[0].Columns["CodCliente"].Header.Caption = "Cliente";
                //e.Layout.Bands[0].Columns["Cliente"].Header.Caption = "Nombre";
                //}

                e.Layout.Bands[0].Columns["nFolio"].Header.Caption = "Folio";
                e.Layout.Bands[0].Columns["nFolio"].Header.Fixed = true;
                e.Layout.Bands[0].Columns["dFechaSolicitud"].Header.Caption = "Fecha de Solicitud";
                e.Layout.Bands[0].Columns["bAprobado"].Hidden = true;
                e.Layout.Bands[0].Columns["sEstatus"].Hidden = true;

                foreach (var item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Activation.NoEdit;
                    if (item.Key != "nFolio")
                        item.Band.Override.FixedHeaderIndicator = FixedHeaderIndicator.None;
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
           
        }

        private void dgvArticulos_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Bands[0].Columns["nID"].Hidden = true;
                e.Layout.Bands[0].Columns["sCodAlmacen"].Hidden = true;
                e.Layout.Bands[0].Columns["nFolio"].Hidden = true;
                e.Layout.Bands[0].Columns["nTipo"].Hidden = true;

                e.Layout.Bands[0].Columns["CodCliente"].Header.Caption = "Cliente";
                e.Layout.Bands[0].Columns["Cliente"].Header.Caption = "Nombre";
                if (rbtSolTransfer.Checked)
                {
                    e.Layout.Bands[0].Columns["CodCliente"].Hidden = true;
                    e.Layout.Bands[0].Columns["Cliente"].Hidden = true;
                }
                else
                {
                    e.Layout.Bands[0].Columns["CodCliente"].Hidden = false;
                    e.Layout.Bands[0].Columns["Cliente"].Hidden = false;
                }


                e.Layout.Bands[0].Columns["sItemCode"].Header.Caption = "Artículo";
                e.Layout.Bands[0].Columns["sNombre"].Header.Caption = "Nombre";
                e.Layout.Bands[0].Columns["nCantidad"].Header.Caption = "Cantidad";
                e.Layout.Bands[0].Columns["nCantidad"].Format = "N2";
                //e.Layout.Bands[0].Columns["nStock"].Hidden = true;
                e.Layout.Bands[0].Columns["nStock"].Header.Caption = "Stock Actual";
                e.Layout.Bands[0].Columns["nStock"].Format = "N2";
                e.Layout.Bands[0].Columns["nSolicitado"].Header.Caption = "Solicitado Actual";
                e.Layout.Bands[0].Columns["nSolicitado"].Format = "N2";
                //e.Layout.Bands[0].Columns["nSolicitado"].Hidden = true;
                e.Layout.Bands[0].Columns["PK"].Hidden = true;
                e.Layout.Bands[0].Columns["sAlmacenOrig"].Hidden = true;
                e.Layout.Bands[0].Columns["sDscAlmacenOrig"].Header.Caption = "Origen";

                e.Layout.Bands[0].Columns["sEstatus"].Hidden = true;
                foreach (var item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Activation.NoEdit;
                }

                e.Layout.Bands[0].Columns["nCantidad"].CellActivation = Activation.AllowEdit;
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
                if (lblNoFolio.Text == "")
                {
                    MessageBox.Show("Debe seleccionar al menos un folio de solicitud");
                    return;
                }

                //if (txtNumTraspaso.Text.Trim() == "")
                //{
                //    MessageBox.Show("Debe especificar un número de traspaso");
                //    txtNumTraspaso.Focus();
                //    return;
                //}
                //if (!chbDespacho.Checked)
                //{
                //    MessageBox.Show("Especifique si la solicitud actual ha sido despachado");
                //    chbDespacho.Focus();
                //    return;
                //}

                //if (ValidaTraspaso(Convert.ToInt32(txtNumTraspaso.Text)).Rows.Count > 0)
                //{
                if(MessageBox.Show("Se va a guardar la solicitud con un estatus de " + cboEstatus.Text + ", ¿seguro que desea continuar?", "informe"
                               , MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                        {
                            command.Connection.Open();
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@TipoConsulta", 8);
                            command.Parameters.AddWithValue("@Estatus", cboEstatus.SelectedValue.ToString());
                            //command.Parameters.AddWithValue("@Despachado", chbDespacho.Checked);
                            //command.Parameters.AddWithValue("@Completo", chbCompleto.Checked);
                            //command.Parameters.AddWithValue("@Traspaso", Convert.ToInt32(txtNumTraspaso.Text));
                            command.Parameters.AddWithValue("@ID", Convert.ToInt32(lblID.Text));
                            command.Parameters.AddWithValue("@Usuario", ClasesSGUV.Login.Id_Usuario);

                            command.ExecuteNonQuery();
                            command.Connection.Close();




                        }
                    }
                }
                MessageBox.Show("Solicitud actualizada correctamente", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnConsultarSolicitudes.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private DataTable ValidaTraspaso(int DocNum)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 7);
                        command.Parameters.AddWithValue("@DocNum", DocNum);
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

        private void dgvFolios_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            e.DisplayPromptMsg = false;
            e.Cancel = true;
        }

        private void dgvArticulos_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            e.DisplayPromptMsg = false;
            e.Cancel = true;
        }

        private void FrmConfirmacionTransferencia_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void FrmConfirmacionTransferencia_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                log.Fin();
            }
            catch (Exception)
            {

            }
        }

        private void dgvArticulos_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "nCantidad")
            {
                int id = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["nID"].Value) ? -1 : 
                    Convert.ToInt32((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["nID"].Value);
                decimal Cantidad = Convert.IsDBNull(e.Cell.Value) ? 0 : Convert.ToDecimal(e.Cell.Value);
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                    {
                        command.Connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 10);
                        command.Parameters.AddWithValue("@Cantidad", Cantidad);
                        command.Parameters.AddWithValue("@ID", id);
                        command.Parameters.AddWithValue("@Usuario", ClasesSGUV.Login.Id_Usuario);

                        command.ExecuteNonQuery();
                        command.Connection.Close();
                    }
                } 
            }
        }

        private void dgvArticulos_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                UltraGrid grid = sender as UltraGrid;
                UltraGridCell activeCell = grid == null ? null : grid.ActiveCell;

                // if there is an active cell, its not in edit mode and can enter edit mode
                if (null != activeCell && false == activeCell.IsInEditMode && activeCell.CanEnterEditMode)
                {
                    // if the character is not a control character
                    if (char.IsControl(e.KeyChar) == false)
                    {
                        // try to put cell into edit mode
                        grid.PerformAction(UltraGridAction.EnterEditMode);

                        // if this cell is still active and it is in edit mode...
                        if (grid.ActiveCell == activeCell && activeCell.IsInEditMode)
                        {
                            // get its editor
                            EmbeddableEditorBase editor = activeCell.EditorResolved;

                            // if the editor supports selectable text
                            if (editor.SupportsSelectableText)
                            {
                                // select all the text so it can be replaced
                                editor.SelectionStart = 0;
                                editor.SelectionLength = editor.TextLength;

                                if (editor is EditorWithMask)
                                {
                                    // just clear the selected text and let the grid
                                    // forward the keypress to the editor
                                    editor.SelectedText = string.Empty;
                                }
                                else
                                {
                                    // then replace the selected text with the character
                                    editor.SelectedText = new string(e.KeyChar, 1);

                                    // mark the event as handled so the grid doesn't process it
                                    e.Handled = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void dgvArticulos_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Infragistics.Win.UltraWinGrid.UltraGrid dgvDatos = (sender as Infragistics.Win.UltraWinGrid.UltraGrid);
                UltraGrid grid = sender as UltraGrid;
                UltraGridCell activeCell = grid == null ? null : grid.ActiveCell;

                switch (e.KeyCode)
                {
                    case Keys.Up:

                        dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false,
                          false);
                        dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.AboveCell, false,
                          false);
                        e.Handled = true;
                        dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode, false,
                          false);
                        break;
                    case Keys.Down:
                        dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false,
                          false);
                        dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.BelowCell, false,
                          false);
                        e.Handled = true;
                        dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode, false,
                          false);
                        break;
                    case Keys.Right:
                        if (grid.ActiveCell == activeCell /*&& !activeCell.IsInEditMode*/)
                        {
                            dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false,
                              false);
                            dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.NextCellByTab, false,
                              false);
                            e.Handled = true;
                            dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode, false,
                              false);
                        }
                        break;
                    case Keys.Left:
                        if (grid.ActiveCell == activeCell /*&& !activeCell.IsInEditMode*/)
                        {
                            dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false,
                              false);
                            dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.PrevCellByTab, false,
                              false);
                            e.Handled = true;
                            dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode, false,
                              false);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                 MessageBox.Show(ex.Message.ToString());
            }
        }

        private void chbAprobado_CheckedChanged(object sender, EventArgs e)
        {
            if (!bCarga && chbAprobado.Checked)
            {
                if (lblNoFolio.Text != string.Empty)
                {
                    if (MessageBox.Show("Esta apunto de aprobar el folio no. " + lblNoFolio.Text.Trim() + ", no podrá cambiar su estado posteriormente, ¿Seguro que desea continuar?", "Aprobación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                        {
                            using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                            {
                                command.Connection.Open();
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@TipoConsulta", 8);
                                command.Parameters.AddWithValue("@Aprobado", chbAprobado.Checked);
                                command.Parameters.AddWithValue("@ID", Convert.ToInt32(lblID.Text));
                                command.Parameters.AddWithValue("@Usuario", ClasesSGUV.Login.Id_Usuario);

                                command.ExecuteNonQuery();
                                command.Connection.Close();

                                try
                                {
                                    if (dgvFolios.ActiveRow != null)
                                    {
                                        dgvFolios.ActiveRow.Cells["bAprobado"].Value = true;
                                    }
                                    else if (dgvFolios.ActiveCell != null)
                                    {
                                        dgvFolios.Rows[dgvFolios.ActiveCell.Row.Index].Cells["bAprobado"].Value = true;
                                    }
                                    dgvArticulos.DisplayLayout.Bands[0].Columns["nCantidad"].CellActivation = Activation.NoEdit;
                                }
                                catch (Exception ex)
                                {   }
                            }
                        }

                        MessageBox.Show("Solicitud aprobada correctamente", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        chbAprobado.Enabled = false;
                    }
                    else
                        chbAprobado.Checked = false;
                }
                else
                {
                    chbAprobado.Checked = false;
                }
            }
        }

        private void btnGenerarTransferencia_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (txtNoTraspaso.Text == "")
            //    {
            //        if (lblNoFolio.Text != "")
            //        {
            //            DataSet ds = new DataSet();
            //            DataTable dt = new DataTable();
            //            DataTable dtSolicitudDetalle = new DataTable();
            //            dtSolicitudDetalle.Columns.Add("ItemCode", typeof(string));
            //            // dtSolicitudDetalle.Columns.Add("ItemName", typeof(string));
            //            //dtSolicitudDetalle.Columns.Add("WhsCode", typeof(string));
            //            dtSolicitudDetalle.Columns.Add("Quantity", typeof(decimal));
            //            //dtSolicitudDetalle.Columns.Add("WhsName", typeof(string));
            //            dtSolicitudDetalle.Columns.Add("FromWhsCode", typeof(string));
            //            dtSolicitudDetalle.Columns.Add("FromWhsName", typeof(string));

            //            foreach (UltraGridRow urow in dgvArticulos.Rows)
            //            {
            //                decimal Cantidad = Convert.IsDBNull(urow.Cells["nCantidad"].Value) ? 0 : Convert.ToDecimal(urow.Cells["nCantidad"].Value);
            //                if (Cantidad > 0)
            //                {
            //                    DataRow drw = dtSolicitudDetalle.NewRow();
            //                    drw["ItemCode"] = urow.Cells["sItemCode"].Value;
            //                    //drw["ItemName"] = urow.Cells["sNombre"].Value;
            //                    //drw["WhsCode"] = urow.Cells[""].Value;
            //                    drw["Quantity"] = urow.Cells["nCantidad"].Value;
            //                    //drw["WhsName"] = urow.Cells[""].Value;
            //                    drw["FromWhsCode"] = urow.Cells["sAlmacenOrig"].Value;
            //                    drw["FromWhsName"] = urow.Cells["sDscAlmacenOrig"].Value;
            //                    dtSolicitudDetalle.Rows.Add(drw);
            //                }
            //            }

            //            //dt.Columns.Add("ItemCode", typeof(string));


            //            //ds = (DataSet)rEncabezado.DataSource;
            //            //if (ds.Tables.Count > 0)
            //            //    dtSolicitudDetalle = ds.Tables[1];

            //            int TipoSol = 1;
            //            if (rbtSolTransfer.Checked)
            //                TipoSol = 1;
            //            else if (rbtVtaConfirm.Checked)
            //                TipoSol = 2;

            //            SDK.Documentos.frmTransferencia oForm = new SDK.Documentos.frmTransferencia(dt, 4, "", cboAlmacen.SelectedValue.ToString(), dtSolicitudDetalle, Convert.ToInt32(lblNoFolio.Text), TipoSol);
            //            oForm.ShowDialog();
            //            oForm.Close();
            //            oForm.Dispose();

            //            //SE CONSULTAN DENUEVO LAS SOLICITUDES
            //            btnConsultarSolicitudes_Click(sender, e);
            //        }
            //        else
            //        {
            //            MessageBox.Show("Debe especificar un folio para poder generar un traspaso", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("El folio actual ya ha sido generado");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message.ToString());
            //}
        }

        private void cboAlmacen_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                lblNoFolio.Text = string.Empty; lblID.Text = string.Empty; //txtNoTraspaso.Text = string.Empty;
                chbAprobado.Checked = false;
                chbAprobado.Enabled = true;
                dgvArticulos.DataSource = null;
                dgvFolios.DataSource = null;
            }
            catch (Exception ex)
            {
            }
            
        }

        private void rbtVtaConfirm_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                lblNoFolio.Text = string.Empty; lblID.Text = string.Empty; //txtNoTraspaso.Text = string.Empty;
                chbAprobado.Checked = false;
                chbAprobado.Enabled = true;
                dgvArticulos.DataSource = null;
                dgvFolios.DataSource = null;
            }
            catch (Exception ex)
            {
            }
           
        }

        private void rbtSolTransfer_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                lblNoFolio.Text = string.Empty; lblID.Text = string.Empty; //txtNoTraspaso.Text = string.Empty;
                chbAprobado.Checked = false;
                chbAprobado.Enabled = true;
                dgvArticulos.DataSource = null;
                dgvFolios.DataSource = null;
            }
            catch (Exception ex)
            {
            }
            
        }

       
    }
}
