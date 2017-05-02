using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;
using System.IO;
using System.Diagnostics;


namespace H_Compras.Logistica
{
    public partial class FrmConsultaFacturas : Form
    {
        RolOperador oTipoOperador;
        ClasesSGUV.Logs log;
        public enum RolOperador
        {
            Vendedor = 1
            ,AdministradorTotal = 2
            ,Gerentes = 3
            ,RepartoRastreo = 4
        }
        public FrmConsultaFacturas()
        {
            InitializeComponent();
            //oTipoMovimiento = oMov;
        }

        private void btnConsultarFactura_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime Inicio = new DateTime(dtpInicio.Value.Year, dtpInicio.Value.Month, dtpInicio.Value.Day, 00, 00, 00);
                DateTime Fin = new DateTime(dtpFin.Value.Year, dtpFin.Value.Month, dtpFin.Value.Day, 23, 59, 59);

                DataTable dtEncabezado = H_Compras.Logistica.csLogistica.ConsultaSolicitudesLogistica(4, Inicio, Fin, (Int32)oTipoOperador);
                DataTable dtDetalleEncabezado = H_Compras.Logistica.csLogistica.ConsultaSolicitudesLogistica(8, Inicio, Fin, (Int32)oTipoOperador);

                DataSet dsRelacion = new DataSet();
                dsRelacion.Tables.Add(dtEncabezado);
                dsRelacion.Tables.Add(dtDetalleEncabezado);

                dsRelacion.Relations.Add("SolicitudesLogistica", dsRelacion.Tables[0].Columns["Folio"], dsRelacion.Tables[1].Columns["nIdFolio"]);
                ugvDatos.DataSource = dsRelacion;
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        private void FrmConsultaFacturas_Load(object sender, EventArgs e)
        {
            log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

            GridKeyActionMapping mapping = new GridKeyActionMapping(Keys.Enter, UltraGridAction.BelowCell, (UltraGridState)0, UltraGridState.InEdit, SpecialKeys.All, (SpecialKeys)0);
            this.ugvDatos.KeyActionMappings.Add(mapping);

            dtpInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01, 00, 00, 00);
            dtpFin.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            int Permiso = H_Compras.Logistica.csLogistica.ConsultaTipoPermisosProceso(13);

            if (RolOperador.Gerentes == (RolOperador)Permiso)
            {
                oTipoOperador = RolOperador.Gerentes;
                this.Text = "Modificación/Autorización de Solicitudes - Logistica";
            }
            else if (RolOperador.AdministradorTotal == (RolOperador)Permiso)
            {
                oTipoOperador = RolOperador.AdministradorTotal;
                this.Text = "Modificación/Autorización de Solicitudes - Logistica";
            }
            else if (RolOperador.RepartoRastreo == (RolOperador)Permiso)
            {
                oTipoOperador = RolOperador.RepartoRastreo;
                this.Text = "Actualización de Solicitudes - Logistica";
            }
            else
            {
                oTipoOperador = RolOperador.Vendedor;
                this.Text = "Consulta de Solicitudes - Logistica";
            }

            
        }

        private void ugvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            Infragistics.Win.UltraWinCalcManager.UltraCalcManager calcManager;
            calcManager = new Infragistics.Win.UltraWinCalcManager.UltraCalcManager(this.Container);
            e.Layout.Grid.CalcManager = calcManager;


            e.Layout.Bands[0].Columns["Envio"].Width = 70;
            e.Layout.Bands[1].Columns["DocNum"].Width = 70;
            e.Layout.Bands[0].Columns["CostoEnvio"].Format = "C2";
            e.Layout.Bands[0].Columns["CostoVenta"].Format = "C2";
            e.Layout.Bands[0].Columns["CostoVenta"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["CostoVSVenta"].Formula = "if([CostoVenta]=0,0,(([CostoEnvio] /[CostoVenta])))";
            e.Layout.Bands[0].Columns["CostoVSVenta"].Format = "P2";
            e.Layout.Bands[0].Columns["CostoVSVenta"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[1].Columns["nIdFolio"].Hidden = true;
            e.Layout.Bands[1].Columns["DocNum"].Header.Caption = "FAC/OV";
            e.Layout.Bands[1].Columns["CardName"].Header.Caption = "Cliente";
            e.Layout.Bands[1].Columns["DocTotal"].Header.Caption = "Monto";
            e.Layout.Bands[1].Columns["DocTotal"].Format = "C2";
            e.Layout.Bands[0].Columns["Cubicaje"].Format = "N2";
            e.Layout.Bands[0].Columns["Cubicaje"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["Tipo Entrega"].Width = 70;
            e.Layout.Bands[0].Columns["No.Rastreo"].Header.Caption = "No. Rastreo";
            e.Layout.Bands[0].Columns["No.Rastreo"].Width = 70;
            e.Layout.Bands[0].Columns["Cancelar"].Hidden = true; //.Width = 60;
            e.Layout.Bands[0].Columns["Notificado"].Width = 70;

            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Activation.NoEdit;
            }
            e.Layout.Bands[0].Columns["Envio"].CellActivation = Activation.AllowEdit;
            foreach (var item in e.Layout.Bands[1].Columns)
            {
                item.CellActivation = Activation.NoEdit;
            }

            if (oTipoOperador == RolOperador.Vendedor)
            {
                e.Layout.Bands[0].Columns["Envio"].Hidden = true;
                //e.Layout.Bands[0].Columns["CostoEnvio"].Hidden = true;
                //e.Layout.Bands[0].Columns["Compañia"].Hidden = true;
                //e.Layout.Bands[0].Columns["No.Rastreo"].Hidden = true;
                e.Layout.Bands[0].Columns["Procesar"].Hidden = true;
                e.Layout.Bands[0].Columns["Autorizar"].Hidden = true;
                e.Layout.Bands[0].Columns["Cancelar"].Hidden = true;
                //e.Layout.Bands[0].Columns["CostoVSVenta"].Hidden = true;
                e.Layout.Bands[0].Columns["Correo"].Hidden = true;
                e.Layout.Bands[0].Columns["Notificado"].Hidden = true;
                e.Layout.Bands[0].Columns["sRutaPDF"].Hidden = true;
            }
            else if (oTipoOperador == RolOperador.AdministradorTotal)
            {
                e.Layout.Bands[0].Columns["Envio"].Header.Caption = "";
                e.Layout.Bands[0].Columns["Procesar"].Hidden = true;
                e.Layout.Bands[0].Columns["Autorizar"].Hidden = true;
                e.Layout.Bands[0].Columns["Cancelar"].Hidden = true;                
                e.Layout.Bands[0].Columns["Correo"].Hidden = false;
                e.Layout.Bands[0].Columns["CostoEnvio"].CellActivation = Activation.AllowEdit;
                e.Layout.Bands[0].Columns["Compañia"].CellActivation = Activation.AllowEdit;
                e.Layout.Bands[0].Columns["No.Rastreo"].CellActivation = Activation.AllowEdit;
                e.Layout.Bands[0].Columns["sRutaPDF"].Hidden = true;

                btnEnviarCorreo.Visible = true;
                btnAutorizar.Visible = true;
                btnRechazar.Visible = true;
            }
            else if (oTipoOperador == RolOperador.Gerentes)
            {
                //e.Layout.Bands[0].Columns["Envio"].Hidden = true;
                e.Layout.Bands[0].Columns["Envio"].Header.Caption = "";
                e.Layout.Bands[0].Columns["Procesar"].Hidden = true;
                e.Layout.Bands[0].Columns["Autorizar"].Hidden = true;
                e.Layout.Bands[0].Columns["Cancelar"].Hidden = true;
                e.Layout.Bands[0].Columns["Correo"].Hidden = true;
                e.Layout.Bands[0].Columns["Notificado"].Hidden = true;
                e.Layout.Bands[0].Columns["sRutaPDF"].Hidden = true;
                
                btnAutorizar.Visible = true;
                btnRechazar.Visible = true;
                //btnEnviarCorreo.Visible = true;
            }
            else if (oTipoOperador == RolOperador.RepartoRastreo)
            {
                e.Layout.Bands[0].Columns["Envio"].Hidden = true;
                e.Layout.Bands[0].Columns["Procesar"].Hidden = true;
                e.Layout.Bands[0].Columns["Autorizar"].Hidden = true;
                e.Layout.Bands[0].Columns["Cancelar"].Hidden = true;
                e.Layout.Bands[0].Columns["Correo"].Hidden = true;
                e.Layout.Bands[0].Columns["CostoEnvio"].CellActivation = Activation.NoEdit;
                e.Layout.Bands[0].Columns["Compañia"].CellActivation = Activation.NoEdit;
                e.Layout.Bands[0].Columns["No.Rastreo"].CellActivation = Activation.AllowEdit;
                e.Layout.Bands[0].Columns["sRutaPDF"].Hidden = true;

                btnEnviarCorreo.Visible = false;
                //btnRechazar.Visible = true;
            }

        }

        private void ugvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGrid dgvDatos = (sender as Infragistics.Win.UltraWinGrid.UltraGrid);
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
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false,
                      false);
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.NextCellByTab, false,
                      false);
                    e.Handled = true;
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode, false,
                      false);
                    break;
                case Keys.Left:
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false,
                      false);
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.PrevCellByTab, false,
                      false);
                    e.Handled = true;
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode, false,
                      false);
                    break;
            }
        }

        private void ugvDatos_KeyPress(object sender, KeyPressEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGrid grid = (sender as Infragistics.Win.UltraWinGrid.UltraGrid);

            Infragistics.Win.UltraWinGrid.UltraGridCell activeCell = grid == null ? null : grid.ActiveCell;

            // if there is an active cell, its not in edit mode and can enter edit mode
            if (null != activeCell && false == activeCell.IsInEditMode && activeCell.CanEnterEditMode)
            {
                // if the character is not a control character
                if (char.IsControl(e.KeyChar) == false)
                {
                    // try to put cell into edit mode
                    grid.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode);

                    // if this cell is still active and it is in edit mode...
                    if (grid.ActiveCell == activeCell && activeCell.IsInEditMode)
                    {
                        // get its editor
                        Infragistics.Win.EmbeddableEditorBase editor = grid.ActiveCell.EditorResolved;

                        // if the editor supports selectable text
                        if (editor.SupportsSelectableText)
                        {
                            // select all the text so it can be replaced
                            editor.SelectionStart = 0;
                            editor.SelectionLength = editor.TextLength;

                            if (editor is Infragistics.Win.EditorWithMask)
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

       
        private void ugvDatos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if(e.Row.Band.Index ==0)
            { 
                e.Row.Cells["Procesar"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                e.Row.Cells["Procesar"].Value = "Procesar/Guardar";

                e.Row.Cells["Autorizar"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                e.Row.Cells["Autorizar"].Value = "Autorizar";

                e.Row.Cells["Cancelar"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                e.Row.Cells["Cancelar"].Value = "Cancelar";
            }
        }

        private void ugvDatos_ClickCellButton(object sender, CellEventArgs e)
        {
            //if (e.Cell.Band.Index == 0 && e.Cell.Column.Key == "Procesar")
            //{

            //    int Folio = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Folio"].Value) ? -1 :
            //        Convert.ToInt32((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Folio"].Value);

            //    decimal CE = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["CostoEnvio"].Value) ? 0 :
            //        Convert.ToDecimal((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["CostoEnvio"].Value);

            //    string Company = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Compañia"].Value) ? "" :
            //        Convert.ToString((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Compañia"].Value);

            //    string NoRastreo = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["No.Rastreo"].Value) ? "" : 
            //        Convert.ToString((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["No.Rastreo"].Value);

            //    if (CE == 0)
            //    {
            //        MessageBox.Show("Debe especificar el costo de envío para la solicitud actual", "Información de Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }

            //    //----------------------------------------------------------------------------------------------------------------------
            //    //----------------------------------------------------------------------------------------------------------------------
            //    string _Porciento = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["CostoVSVenta"].Text) ? "" :
            //        Convert.ToString((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["CostoVSVenta"].Text);

            //    string PorcientoS = "0";
            //    if (_Porciento != "")
            //    {
            //        String[] substrings = _Porciento.Split('%');
            //        PorcientoS = substrings[0].ToString().Trim();
            //    }
                
            //    decimal Porciento = Convert.ToDecimal(PorcientoS);

            //    DataTable dtFolioAct = csLogistica.UpdateSolicitudFactura(3, Folio, CE, Company, NoRastreo, Porciento);
            //    if (dtFolioAct.Rows.Count > 0)
            //        (sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Estatus"].Value = Convert.IsDBNull(dtFolioAct.Rows[0]["Estatus"]) ? "" : Convert.ToString(dtFolioAct.Rows[0]["Estatus"]); 





            //}
            if (e.Cell.Band.Index == 0 && e.Cell.Column.Key == "Cancelar")
            {
                int Folio = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Folio"].Value) ? -1 :
                    Convert.ToInt32((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Folio"].Value);

                DataTable dtFolioAct = H_Compras.Logistica.csLogistica.UpdateEstatusSolicitudLogistica(6, Folio);

                if (dtFolioAct.Rows.Count > 0)
                    (sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Estatus"].Value = Convert.IsDBNull(dtFolioAct.Rows[0]["Estatus"]) ? "" : Convert.ToString(dtFolioAct.Rows[0]["Estatus"]); 
                
            }

            //if (e.Cell.Band.Index == 0 && e.Cell.Column.Key == "Autorizar")
            //{
            //    int Folio = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Folio"].Value) ? -1 :
            //        Convert.ToInt32((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Folio"].Value);
                
            //    //DialogResult result = MessageBox.Show("¿Está apunto de eliminar el archivo adjunto actual, seguro que desea continuar?", "Confirmación de Eliminación", MessageBoxButtons.YesNoCancel);
            //    //if (result == DialogResult.Yes)
            //    //{
            //    //}
            //    DataTable dtFolioAct = csLogistica.UpdateEstatusSolicitudLogistica(9, Folio);

            //    if (dtFolioAct.Rows.Count > 0)
            //        (sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Estatus"].Value = Convert.IsDBNull(dtFolioAct.Rows[0]["Estatus"]) ? "" : Convert.ToString(dtFolioAct.Rows[0]["Estatus"]); 
            //}
        }

        private void ugvDatos_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            if (e.Cell.Column.Key == "CostoEnvio")
            {
                decimal val = Convert.ToDecimal(e.Cell.Text);
                if (val <= 0)
                {
                    MessageBox.Show("El costo de envío debe ser mayor a cero");
                    e.Cancel = true;
                }
            }

            //if (e.Cell.Column.Key == "Envio")
            //{
            //    string _Estatus = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Estatus"].Value) ? "" :
            //        Convert.ToString((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Estatus"].Value);
            //    if (_Estatus != "EN PROCESO DE AUTORIZACION")
            //    {
            //        e.Cancel = true; 
            //    }
            //}

            
        }

        private void ugvDatos_AfterCellUpdate(object sender, CellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Key == "CostoEnvio" || e.Cell.Column.Key == "Compañia" || e.Cell.Column.Key == "No.Rastreo")
                {
                    if (e.Cell.Column.Key == "CostoEnvio")
                    {

                    }

                    int Folio = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Folio"].Value) ? -1 :
                            Convert.ToInt32((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Folio"].Value);

                    decimal CE = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["CostoEnvio"].Value) ? 0 :
                        Convert.ToDecimal((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["CostoEnvio"].Value);

                    string Company = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Compañia"].Value) ? "" :
                        Convert.ToString((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Compañia"].Value);

                    string NoRastreo = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["No.Rastreo"].Value) ? "" :
                        Convert.ToString((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["No.Rastreo"].Value);

                    decimal CostoVenta = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["CostoVenta"].Value) ? 0 :
                        Convert.ToDecimal((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["CostoVenta"].Value);

                    if (CE == 0 && e.Cell.Column.Key == "CostoEnvio")
                    {
                        MessageBox.Show("Debe especificar el costo de envío para la solicitud actual", "Información de Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    //----------------------------------------------------------------------------------------------------------------------
                    //----------------------------------------------------------------------------------------------------------------------

                    decimal Porciento = 0;
                    if (CostoVenta > 0)
                        Porciento = ((CE / CostoVenta) * 100); //Convert.ToDecimal(PorcientoS);

                    DataTable dtFolioAct = H_Compras.Logistica.csLogistica.UpdateSolicitudFactura(3, Folio, CE, Company, NoRastreo, Porciento, e.Cell.Column.Key.ToString());
                    if (dtFolioAct.Rows.Count > 0)
                        (sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Estatus"].Value = Convert.IsDBNull(dtFolioAct.Rows[0]["Estatus"]) ? "" : Convert.ToString(dtFolioAct.Rows[0]["Estatus"]);

                    string Estatus = dtFolioAct.Rows[0]["Estatus"].ToString();
                    if (Estatus == "AUTORIZADO")
                    {
                        DialogResult result = MessageBox.Show("El estatus ha cambiado a autorizado ¿desea enviar la notificación?, sí para continuar, No para cancelar y probar con otro valor.", "Confirmación", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            string CorreoNotificar = CorreoNotificar = H_Compras.Logistica.csLogistica.ConsultaCorreosNotificar(14, 2, Folio);
                            Enviar("", CorreoNotificar, "", "", false, "Autorización de Solicitud", true, "Una solicitud ha sido autorizada por el área de logistica, ingrese a su halconet para ver mas detalles", true);
                            H_Compras.Logistica.csLogistica.UpdateEstatusSolicitudLogistica(10, Folio);
                            (sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Notificado"].Value = "SI";
                            MessageBox.Show("Notificación realizada correctamente", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (Estatus == "EN PROCESO DE AUTORIZACION")
                    {
                        DialogResult result = MessageBox.Show("El estatus ha cambiado a proceso de autorizacion ¿desea enviar la notificación?, sí para continuar, No para cancelar y probar con otro valor.", "Confirmación", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            string CorreoNotificar = CorreoNotificar = H_Compras.Logistica.csLogistica.ConsultaCorreosNotificar(14, 3, Folio);
                            Enviar("", CorreoNotificar, "", "", false, "", false, "", false);
                            H_Compras.Logistica.csLogistica.UpdateEstatusSolicitudLogistica(10, Folio);
                            (sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Notificado"].Value = "SI";

                            MessageBox.Show("Notificación realizada correctamente", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            
        }

        private void btnAutorizar_Click(object sender, EventArgs e)
        {
            try
            {
                int nAutorizaciones = 0;
                foreach (UltraGridRow item in ugvDatos.Rows)
                {
                    string Text = item.Cells["Envio"].Value.ToString();
                    if (Text == "True")
                    {
                        nAutorizaciones = nAutorizaciones + 1;
                    }                    
                }

                if (nAutorizaciones == 0)
                {
                    MessageBox.Show("Debe seleccionar al menos un registro a autorizar");
                    return;
                }

                DialogResult result = MessageBox.Show("Esta apunto de autorizar " + nAutorizaciones.ToString() + " solicitud(es) seleccionada(s), seguro que desdea continuar?", "Confirmación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Cursor = Cursors.WaitCursor;
                    foreach (UltraGridRow item in ugvDatos.Rows)
                    {
                        string Text = item.Cells["Envio"].Value.ToString();
                        if (Text == "True")
                        {
                            int Folio = Convert.ToInt32(item.Cells["Folio"].Value);
                            DataTable dtFolioAct = H_Compras.Logistica.csLogistica.UpdateEstatusSolicitudLogistica(9, Folio);
                            if (dtFolioAct.Rows.Count > 0)
                                item.Cells["Estatus"].Value = Convert.IsDBNull(dtFolioAct.Rows[0]["Estatus"]) ? "" : Convert.ToString(dtFolioAct.Rows[0]["Estatus"]);

                            string CorreoNotificar="";

                            if(oTipoOperador == RolOperador.Gerentes)
                                CorreoNotificar = H_Compras.Logistica.csLogistica.ConsultaCorreosNotificar(14, 4, Folio);
                            else
                                CorreoNotificar = H_Compras.Logistica.csLogistica.ConsultaCorreosNotificar(14, 2, Folio);

                            Enviar("", CorreoNotificar, "", "", false, "Autorización de Solicitud", true, "Una solicitud ha sido autorizada por el área de logistica, ingrese a su halconet para ver mas detalles", true);
                        }
                    }
                    Cursor = Cursors.Default;
                    MessageBox.Show(nAutorizaciones.ToString() + " Solicitud(es) autorizadas", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                foreach (UltraGridRow item in ugvDatos.Rows)
                {
                    item.Cells["Envio"].Value = false;
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public bool Enviar(string _file, string _mailDestinatario, string _mailVendedor, string _vendedor, bool _solicitud, 
            string Asunto, bool bAsunto, string Mensaje, bool bMensaje)
        {
            bool enviado = false;
            try
            {
                ClasesSGUV.SendMail po = new ClasesSGUV.SendMail();
                ClasesSGUV.DatosMail oDatosCorreo = new ClasesSGUV.DatosMail();
                oDatosCorreo = oDatosCorreo.ObtenerDatos(1, "LOGIST");
                if (!bAsunto)
                    oDatosCorreo.Asunto += " " + _vendedor;
                else
                    oDatosCorreo.Asunto = Asunto;

                if (bMensaje)
                    oDatosCorreo.Cuerpo = Mensaje;

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

                if (_mailVendedor != "")
                    Correos.Add(x, _mailVendedor);
                //Correos.Add(1, "pedro.juarez@tractozone.com.mx");


                enviado = po.EnviarMail(oDatosCorreo, Correos, CorreosCopiar, CorreosCopiaOculta, ArchAdjunt);

            }
            catch (Exception ex)
            {
                throw;
            }
            
            return enviado;
        }

        private void btnEnviarCorreo_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (UltraGridRow item in ugvDatos.Rows)
                {
                    string Text = item.Cells["Envio"].Value.ToString();
                    if (Text == "True")
                    {
                        string Ruta = item.Cells["sRutaPDF"].Value.ToString();
                        int Folio = Convert.ToInt32(item.Cells["Folio"].Value);
                        string Correos = /*"pedro.juarez@tractozone.com.mx";*/item.Cells["Correo"].Value.ToString();
                        if (Correos != "")
                        {
                            Cursor = Cursors.WaitCursor;
                            string Vendor = item.Cells["Vendedor"].Value.ToString();
                            if (Enviar(Ruta, Correos, "", Vendor, false, "", false, "", false))
                            {
                                H_Compras.Logistica.csLogistica.UpdateEstatusSolicitudLogistica(10, Folio);
                                item.Cells["Notificado"].Value = "SI";
                            }
                            Cursor = Cursors.Default;
                        }
                        else
                        {
                            MessageBox.Show("No existe correo destinatario, verificar con el adminstrador del sistema", "Informacion incompleta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        
                    }
                }
                foreach (UltraGridRow item in ugvDatos.Rows)
                {
                    item.Cells["Envio"].Value = false;                    
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void ugvDatos_ClickCell(object sender, ClickCellEventArgs e)
        {
            if (e.Cell.Column.Key == "Envio")
            {
                string _Estatus = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Estatus"].Value) ? "" :
                    Convert.ToString((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Estatus"].Value);
                if (_Estatus != "EN PROCESO DE AUTORIZACION")
                {
                    e.Cell.Value = false;
                }
 
            }
        }

        private void ugvDatos_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                string Estatus = ugvDatos.Rows[e.Cell.Row.Index].Cells["Estatus"].Value.ToString();
                if (Estatus == "AUTORIZADO" || Estatus == "CANCELADA")
                {
                    string Ruta = ugvDatos.Rows[e.Cell.Row.Index].Cells["sRutaPDF"].Value.ToString();
                
                    string Folio = ugvDatos.Rows[e.Cell.Row.Index].Cells["Folio"].Value.ToString();
                    Ruta = ""; Ruta = Path.GetTempPath(); //"\\\\192.168.2.98\\Digitalización\\Solicitud_Logistica\\";
                    string NomArchivo = ""; NomArchivo = "Solicitud_" + Folio + ".PDF";
                    H_Compras.Logistica.csLogistica.ImprimirSolicitud(Folio, Ruta, NomArchivo);
                    string RutaCompleta = Ruta + NomArchivo;
                    H_Compras.Logistica.csLogistica.UpdateRutaSolicitud(12, Convert.ToInt32(Folio), RutaCompleta);
                    ugvDatos.Rows[e.Cell.Row.Index].Cells["sRutaPDF"].Value = RutaCompleta;
                }
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(ex.Message.ToString());
            }
            //string Estatus = ugvDatos.Rows[e.Cell.Row.Index].Cells["Estatus"].Value.ToString();
            //if (Estatus == "AUTORIZADO" || Estatus == "CANCELADA")
            //{
            //    string Ruta = ugvDatos.Rows[e.Cell.Row.Index].Cells["sRutaPDF"].Value.ToString();
                
            //    string Folio = ugvDatos.Rows[e.Cell.Row.Index].Cells["Folio"].Value.ToString();
            //    Ruta = ""; Ruta = "\\\\192.168.2.98\\Digitalización\\Solicitud_Logistica\\";
            //    string NomArchivo = ""; NomArchivo = "Solicitud_" + Folio + ".PDF";
            //    csLogistica.ImprimirSolicitud(Folio, Ruta, NomArchivo);
            //    string RutaCompleta = Ruta + NomArchivo;
            //    csLogistica.UpdateRutaSolicitud(12, Convert.ToInt32(Folio), RutaCompleta);
            //    ugvDatos.Rows[e.Cell.Row.Index].Cells["sRutaPDF"].Value = RutaCompleta;

            //    //if (Ruta != string.Empty)
            //    //{
            //        //if (File.Exists(Ruta))
            //        //{
            //        //    Process prc = new Process();
            //        //    prc.StartInfo.FileName = Ruta;
            //        //    prc.Start();
            //        //}
            //        //else
            //        //{
            //        //    string Folio = ugvDatos.Rows[e.Cell.Row.Index].Cells["Folio"].Value.ToString();
            //        //    Ruta = ""; Ruta = "\\\\192.168.2.98\\Digitalización\\Solicitud_Logistica\\";
            //        //    string NomArchivo = ""; NomArchivo = "Solicitud_" + Folio + ".PDF";
            //        //    csLogistica.ImprimirSolicitud(Folio, Ruta, NomArchivo);
            //        //    csLogistica.UpdateRutaSolicitud(12, Convert.ToInt32(Folio), RutaCompleta);
            //        //    ugvDatos.Rows[e.Cell.Row.Index].Cells["sRutaPDF"].Value = RutaCompleta;
            //        //}


            //    //}
            //    //else
            //    //{
            //    //    string Folio = ugvDatos.Rows[e.Cell.Row.Index].Cells["Folio"].Value.ToString();
            //    //    Ruta = ""; Ruta = "\\\\192.168.2.98\\Digitalización\\Solicitud_Logistica\\";
            //    //    string NomArchivo = ""; NomArchivo = "Solicitud_" + Folio + ".PDF";
            //    //    csLogistica.ImprimirSolicitud(Folio, Ruta, NomArchivo);
            //    //    string RutaCompleta = Ruta + NomArchivo;
            //    //    csLogistica.UpdateRutaSolicitud(12, Convert.ToInt32(Folio), RutaCompleta);
            //    //    ugvDatos.Rows[e.Cell.Row.Index].Cells["sRutaPDF"].Value = RutaCompleta;
            //    //}
            //}
        }

        private void FrmConsultaFacturas_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void FrmConsultaFacturas_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                log.Fin();
            }
            catch (Exception)
            {

            }
        }

        private void btnRechazar_Click(object sender, EventArgs e)
        {
            try
            {
                int nAutorizaciones = 0;
                foreach (UltraGridRow item in ugvDatos.Rows)
                {
                    string Text = item.Cells["Envio"].Value.ToString();
                    if (Text == "True")
                    {
                        nAutorizaciones = nAutorizaciones + 1;
                    }                    
                }

                if (nAutorizaciones == 0)
                {
                    MessageBox.Show("Debe seleccionar al menos un registro a rechazar");
                    return;
                }

                DialogResult result = MessageBox.Show("Esta apunto de rechazar " + nAutorizaciones.ToString() + " solicitud(es) seleccionada(s), seguro que desdea continuar?", "Confirmación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Cursor = Cursors.WaitCursor;
                    foreach (UltraGridRow item in ugvDatos.Rows)
                    {
                        string Text = item.Cells["Envio"].Value.ToString();
                        if (Text == "True")
                        {
                            int Folio = Convert.ToInt32(item.Cells["Folio"].Value);
                            DataTable dtFolioAct = H_Compras.Logistica.csLogistica.UpdateEstatusSolicitudLogistica(15, Folio);
                            if (dtFolioAct.Rows.Count > 0)
                                item.Cells["Estatus"].Value = Convert.IsDBNull(dtFolioAct.Rows[0]["Estatus"]) ? "" : Convert.ToString(dtFolioAct.Rows[0]["Estatus"]);

                            string CorreoNotificar="";

                            if(oTipoOperador == RolOperador.Gerentes)
                                CorreoNotificar = H_Compras.Logistica.csLogistica.ConsultaCorreosNotificar(14, 5, Folio);
                            else
                                CorreoNotificar = H_Compras.Logistica.csLogistica.ConsultaCorreosNotificar(14, 2, Folio);

                            Enviar("", CorreoNotificar, "", "", false, "Rechazo de Solicitud", true, "Una solicitud ha sido rechazada por el área de logistica, ingrese a su halconet para ver mas detalles", true);
                        }
                    }
                    Cursor = Cursors.Default;
                    MessageBox.Show(nAutorizaciones.ToString() + " Solicitud(es) rechazadas", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                foreach (UltraGridRow item in ugvDatos.Rows)
                {
                    item.Cells["Envio"].Value = false;
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(ex.Message.ToString());
            }
        }

      

       
    }
}
