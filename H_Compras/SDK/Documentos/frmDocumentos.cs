using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace H_Compras.SDK.Documentos
{
    public partial class frmDocumentos : Constantes.frmEmpty
    {
        int IdDocument;
        Decimal IVA;
        public Decimal Rate = 1;
        int ObjType = 22;
        bool ReadOnly = false;
        string VatGroup;
        SDK.Configuracion.SDK_Configuracion_Document config;
        SDK.SDKDatos.SDK_OPOR Document;
        SDK_SAP.Clases.Document DocumentSAP;
        decimal DocKey, WtmCode, WddCode;
        public string typeDocument = "OC";

        public frmDocumentos(DataTable _details, int _idDocument, string _cardCode)
        {
            InitializeComponent();
            dgvDatos.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
            //  I  N  I  C  I  A  L  I  Z  A  R
            IdDocument = _idDocument;
            config = new Configuracion.SDK_Configuracion_Document(IdDocument, this);
            config.ModeDocument = "New";
            config.StartFill(IdDocument, _details, _cardCode);
            IVA = config.IVA1;
            VatGroup = config.VatGroup;
            this.AccessibleDescription = "SDK " + this.Text;
            this.btnCrear.Text = "Crear";
            nuevoToolStripButton.Enabled = false;
        }

        public frmDocumentos(int _idDocument)
        {
            InitializeComponent();
            dgvDatos.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
            //  I  N  I  C  I  A  L  I  Z  A  R
            //dgvDatos.DataSource = _details;
            IdDocument = _idDocument;
            config = new Configuracion.SDK_Configuracion_Document(IdDocument, this);
            config.ModeDocument = "Edit";
            config.StartEmpty();
            IVA = config.IVA1;
            this.AccessibleDescription = "SDK " + this.Text;
            this.btnCrear.Text = "Actualizar";
            this.CleanScreen();

            txtCardCode.ReadOnly = false;
            txtCardCode.Enabled = true;

            txtCardName.ReadOnly = false;
            txtCardName.Enabled = true;

            txtFolio.BackColor = Color.FromName("Info");
            txtCardCode.BackColor = Color.FromName("Info");
            txtCardName.BackColor = Color.FromName("Info");
            txtNumAtCard.BackColor = Color.FromName("Info");

            cerrarStripButton.Enabled = true;
            nuevoToolStripButton.Enabled = true;
            cerrarLíneaToolStripMenuItem.Enabled = true;

            //tooStripDuplicar.Enabled = false;
            //toolStripEliminar.Enabled = false;
        }

        public frmDocumentos(int _idDocument, bool _readOnly)
        {
            InitializeComponent();
            ReadOnly = _readOnly;
            dgvDatos.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
            //  I  N  I  C  I  A  L  I  Z  A  R
            //dgvDatos.DataSource = _details;
            IdDocument = _idDocument;
            config = new Configuracion.SDK_Configuracion_Document(IdDocument, this);
            config.ModeDocument = "Edit";
            config.StartEmpty();
            IVA = config.IVA1;
            this.AccessibleDescription = "SDK " + this.Text;
            this.btnCrear.Text = "Actualizar";
            this.CleanScreen();

            txtCardCode.ReadOnly = false;
            txtCardCode.Enabled = true;

            txtCardName.ReadOnly = false;
            txtCardName.Enabled = true;

            txtFolio.BackColor = Color.FromName("Info");
            txtCardCode.BackColor = Color.FromName("Info");
            txtCardName.BackColor = Color.FromName("Info");
            txtNumAtCard.BackColor = Color.FromName("Info");

            cerrarStripButton.Enabled = true;
            nuevoToolStripButton.Enabled = false;
            cerrarLíneaToolStripMenuItem.Enabled = true;
        }

        public frmDocumentos(int _idDocument, decimal _DocKey, int _ObjType, decimal _WtmCode, decimal _WddCode)
        {
            InitializeComponent();
            typeDocument = "Borrador";
            DocumentSAP = new SDK_SAP.Clases.Document();
            dgvDatos.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
            //  I  N  I  C  I  A  L  I  Z  A  R
            IdDocument = _idDocument;
            config = new SDK.Configuracion.SDK_Configuracion_Document(IdDocument, this);
            config.ModeDocument = "New";
            config.StartEmpty();
            IVA = config.IVA1;
            this.AccessibleDescription = "SDK " + this.Text;

            txtFolio.Text = _DocKey.ToString();
            DocKey = _DocKey;
            ObjType = _ObjType;
            WtmCode = _WtmCode;
            WddCode = _WddCode;
        }

        private void CleanScreen()
        {
            txtCardCode.Clear();
            txtCardName.Clear();
            cbMoneda.SelectedIndex = 0;
            txtMoneda.Clear();
            txtNumAtCard.Clear();
            txtFolio.Clear();
            txtStatus.Clear();
            dtpDocDate.Value = DateTime.Now;
            dtpDocDueDate.Value = DateTime.Now;

            (dgvDatos.DataSource as DataTable).Rows.Clear();
            txtComments.Clear();
            txtSubtotal.Clear();
            txtTotal.Clear();
            txtIVA.Clear();

            btnCrear.Enabled = true;
            btnCancelar.Enabled = true;

            txtFolio.Focus();
        }

        private bool Validar()
        {
            using (SqlConnection cnnection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                SqlCommand cmd = new SqlCommand("sp_SDKValidaciones", cnnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@TipoConsulta", 2));

                SqlParameter outParameter = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500);
                outParameter.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outParameter);
                cnnection.Open();
                bool validar = Convert.ToBoolean(cmd.ExecuteScalar());
                string mensaje = cmd.Parameters["@Mensaje"].Value.ToString().ToString();

                if (!string.IsNullOrEmpty(mensaje))
                {
                    this.SetMensaje(mensaje, 10000, Color.Red, Color.White);
                }

                return validar;
            }
        }

        private void frmOrdenCompra_Load(object sender, EventArgs e)
        {
            try
            {
                Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                if (!this.Validar())
                {
                    btnCrear.Enabled = false;
                    btnCancelar.Enabled = false;
                    return;
                }

                actualizarToolStripButton.Click -= new EventHandler(frmOrdenCompra_Load);
                actualizarToolStripButton.Click += new EventHandler(frmOrdenCompra_Load);

                dgvDatos.KeyDown -= new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);
                dgvDatos.KeyDown += new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);

                dgvDatos.KeyPress -= new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);
                dgvDatos.KeyPress += new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);

                this.dgvDatos.PerformAction(UltraGridAction.Copy, true, true);

                #region Evento Grid

                foreach (GridKeyActionMapping ugKey in dgvDatos.KeyActionMappings)
                {
                    if (ugKey.KeyCode == Keys.Enter)
                    {
                        dgvDatos.KeyActionMappings.Remove(ugKey);
                    }
                }

                this.dgvDatos.KeyActionMappings.Add(
                   new GridKeyActionMapping(
                   Keys.Enter,
                   UltraGridAction.BelowCell,
                   0,
                   0,
                   SpecialKeys.All,
                   0));
                #endregion

                guardarToolStripButton.Enabled = false;
               // ayudaToolStripButton.Enabled = false;
                PathHelp = "http://hntsolutions.net/manual/module_9_2_2.htm?ms=AAAAAAA%3D&st=MA%3D%3D&sct=NDEwMA%3D%3D&mw=MjU0";
                // 
                //exportarToolStripButton.Enabled = false;
                actualizarToolStripButton.Enabled = false;

                buscarStripButton.Enabled = true;
                imprimirToolStripButton.Enabled = true;

                nuevoToolStripButton.Click -= new EventHandler(btnNuevo_Click);
                nuevoToolStripButton.Click += new EventHandler(btnNuevo_Click);

                buscarStripButton.Click -= new EventHandler(btnBuscar_Click);
                buscarStripButton.Click += new EventHandler(btnBuscar_Click);

                imprimirToolStripButton.Click -= new EventHandler(btnImprimir_Click);
                imprimirToolStripButton.Click += new EventHandler(btnImprimir_Click);

                cerrarStripButton.Click -= new EventHandler(btnCerrar_Click);
                cerrarStripButton.Click += new EventHandler(btnCerrar_Click);

                dgvDatos.DisplayLayout.Override.AllowDelete = DefaultableBoolean.True;

                PathHelp = "http://hntsolutions.net/manual/module_14_2_2.htm?ms=AAAAAAA%3D&st=MA%3D%3D&sct=NDY1MQ%3D%3D&mw=MjU1";

                if (!String.IsNullOrWhiteSpace(txtFolio.Text))
                {
                    var kea = new KeyPressEventArgs(Convert.ToChar(13));
                    this.txtFolio_KeyPress(sender, kea);
                }

                dtpDocDate_ValueChanged(sender, e);
            }
            catch (Exception)
            {
                this.SetMensaje("Error: al cargar Form frm_Documentos", 5000, Color.Red, Color.White);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (btnCrear.Text == "Crear")
            {
                #region Orden de compra

                if (string.IsNullOrEmpty(txtMoneda.Text))
                {
                    this.SetMensaje("Campo MONEDA obligatorio", 5000, Color.Red, Color.White);
                    return;
                }

                #region Crear
                try
                {
                    #region Encabezado
                    DocumentSAP = new SDK_SAP.Clases.Document();
                    Cursor.Current = Cursors.WaitCursor;
                    DocumentSAP.CardCode = txtCardCode.Text;
                    DocumentSAP.Series = 15;
                    if (!String.IsNullOrWhiteSpace(txtFolio.Text))
                        DocumentSAP.DocNum = Convert.ToInt32(txtFolio.Text);
                    else
                        DocumentSAP.DocNum = 0;
                    DocumentSAP.DocRate = (double)Rate;
                    DocumentSAP.DocDate = dtpDocDate.Value;
                    DocumentSAP.DocDueDate = dtpDocDueDate.Value;
                    DocumentSAP.DocType = "dDocument_Items";
                    DocumentSAP.DocCurrency = txtMoneda.Text;

                    if (!string.IsNullOrEmpty(txtNumAtCard.Text))
                        DocumentSAP.NumAtCard = txtNumAtCard.Text;
                    if (!string.IsNullOrEmpty(txtComments.Text))
                        DocumentSAP.Comments = txtComments.Text;

                    DocumentSAP.DocTotal = Convert.ToDouble(txtTotal.Text);
                    DocumentSAP.VatSum = Convert.ToDouble(txtIVA.Text);

                    #endregion
                    foreach (var item in dgvDatos.Rows)
                    {
                        #region Detalles
                        SDK_SAP.Clases.DocumentLines line = new SDK_SAP.Clases.DocumentLines();

                        line.ItemCode = Convert.ToString(item.Cells["ItemCode"].Value);
                        line.ItemDescription = Convert.ToString(item.Cells["ItemName"].Value);
                        line.Quantity = Convert.ToDouble(item.Cells["Quantity"].Value);
                        line.Currency = Convert.ToString(item.Cells["Currency"].Value);
                        line.WarehouseCode = Convert.ToString(item.Cells["WhsCode"].Value);

                        if (item.Cells["ShipDate"].Value != DBNull.Value)
                            line.ShipDate = Convert.ToDateTime(item.Cells["ShipDate"].Value);

                        line.U_Comentario = Convert.ToString(item.Cells["U_Comentario"].Value);
                        line.U_Vendedor = Convert.ToString(item.Cells["U_Vendedor"].Value);

                        line.Price = Convert.ToDouble(item.Cells["Price"].Value);
                        line.Rate = Convert.ToDouble(item.Cells["Rate"].Value);
                        line.LineTotal = Convert.ToDouble(item.Cells["LineTotal"].Value);

                        if (item.Cells["OnHand"].Value != DBNull.Value)
                            line.OnHand = Convert.ToDouble(item.Cells["OnHand"].Value); 
                        if (item.Cells["Ideal"].Value != DBNull.Value)
                            line.Ideal = Convert.ToDouble(item.Cells["Ideal"].Value);
                        if (item.Cells["VI"].Value != DBNull.Value)
                            line.VI = Convert.ToDouble(item.Cells["VI"].Value); 
                        if (item.Cells["ABC"].Value != DBNull.Value)
                            line.ABC = Convert.ToString(item.Cells["ABC"].Value);

                        DocumentSAP.Lines.Add(line);
                        #endregion
                    }

                    if (!txtStatus.Text.Equals("Autorizado"))
                    {
                        #region Borrador
                        DocumentSAP.DocType = ObjType.ToString();
                        txtFolio.Text = DocumentSAP.SaveDraft(DocumentSAP).ToString();

                        DocumentSAP = DocumentSAP.Fill_OPOR(DocumentSAP.DocNum, ObjType, "Borrador");
                        txtFolio.Text = DocumentSAP.DocNum.ToString();

                        if (DocumentSAP.DocStatus.Equals("P"))
                            txtStatus.Text = "Pendiente";
                        else if (DocumentSAP.DocStatus.Equals("R"))
                            txtStatus.Text = "Rechazado";
                        else if (DocumentSAP.DocStatus.Equals("A"))
                            txtStatus.Text = "Autorizado";

                        typeDocument = "Borrador";

                        var kea = new KeyPressEventArgs(Convert.ToChar(13));
                        this.txtFolio_KeyPress(sender, kea);

                        this.SetMensaje("Listo", 5000, Color.Green, Color.Black);

                        #endregion
                    }
                    
                    if (txtStatus.Text.Equals("Autorizado"))
                    {
                        SDK.SDKDatos.SDK_DI sdk = new SDKDatos.SDK_DI(1);//--Orden de compra  1
                        sdk.CrearOrdenCompra(this).ToString();
                        this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
                        
                        Object[] valuesOut = new Object[] { };
                        Datos.Connection connection = new Datos.Connection();

                        connection.Ejecutar("LOG",
                                                    "sp_SDKDocumentosPrevios_test",
                                                    new string[] { },
                                                    new string[] { "@TipoConsulta", "@ObjType", "@DocNum", "@DocNumSAP" },
                                                    ref valuesOut, 17, ObjType, DocumentSAP.DocNum, txtFolio.Text);
                        
                        /*
                        #region SAP
                        SDK_SAP.DI.Connection.InitializeConnection(9);
                        SDK_SAP.DI.Connection.StartConnection();
                        SDK_SAP.DI.Documents doc = new SDK_SAP.DI.Documents(9);

                        txtFolio.Text = doc.AddDocument("ORDN", Document).ToString();

                        var kea = new KeyPressEventArgs(Convert.ToChar(13));
                        this.txtFolio_KeyPress(sender, kea);

                        SDK_SAP.DI.Connection.CloseConnection();
                        this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
                        DocumentMode = "Creado";

                        Object[] valuesOut = new Object[] { };
                        Datos.Connection connection = new Datos.Connection();

                        connection.Ejecutar("LOG",
                                                    "sp_SDKDocumentosPrevios",
                                                    new string[] { },
                                                    new string[] { "@TipoConsulta", "@ObjType", "@DocNum", "@DocNumSAP" },
                                                    ref valuesOut, 17, ObjType, Document.DocNum, txtFolio.Text);
                        #endregion
                        */
                    }
                }
                catch (Exception ex)
                {
                    this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
                }
                finally
                {
                    Cursor.Current = Cursors.Arrow;
                }

                #endregion
                #endregion
            }
            if (btnCrear.Text == "Actualizar")
            {
                #region Modificar
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    if (IdDocument == (int)SDK.Configuracion.SDK_Configuracion_Document.DocumentType.OrdenCompra)
                    {
                        SDK.SDKDatos.SDK_DI sdk = new SDKDatos.SDK_DI(1);//Orden de compra

                        sdk.ActualizarDocumento(this, DocumentSAP).ToString();
                        this.SetMensaje("OC Actualizada correctamente", 5000, Color.Green, Color.Black);
                    }
                }
                catch (Exception ex)
                {
                    this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
                }
                finally
                {
                    Cursor.Current = Cursors.Arrow;
                }
                #endregion
            }

            if (btnCrear.Text == "Autorizar")
            {
                #region Autorizar
                using (SqlConnection connection1 = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    connection1.Open();
                    SqlCommand command = new SqlCommand();

                    command.CommandText = "sp_SDKDocumentosPrevios";
                    command.Connection = connection1;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 16);
                    command.Parameters.AddWithValue("@DocNum", DocKey);
                    command.Parameters.AddWithValue("@Comments", txtComments.Text);
                    command.Parameters.AddWithValue("@WddCode", WddCode);
                    command.Parameters.AddWithValue("@userID", ClasesSGUV.Login.Id_Usuario);
                    try
                    {
                        command.ExecuteNonQuery();
                        connection1.Close();
                        MessageBox.Show("Listo", "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.SetMensaje("Listo", 5000, Color.Green, Color.Black);

                        DocumentSAP = DocumentSAP.Fill_OPOR(Convert.ToDecimal(txtFolio.Text), 22, "Borrador");
                        txtFolio.Text = DocumentSAP.DocNum.ToString();

                        if (DocumentSAP.DocStatus.Equals("P"))
                            txtStatus.Text = "Pendiente";
                        else if (DocumentSAP.DocStatus.Equals("R"))
                            txtStatus.Text = "Rechazado";
                        else if (DocumentSAP.DocStatus.Equals("A"))
                            txtStatus.Text = "Autorizado";

                        this.SetMensaje("Listo", 5000, Color.Green, Color.Black);

                        btnCrear.Enabled = false;
                    }
                    catch (Exception ex)
                    {
                        this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
                    }
                }
                #endregion
            }
        }

        private void cbMoneda_SelectionChangeCommitted(object sender, EventArgs e)
        {
            txtMoneda.Text = cbMoneda.SelectedValue.ToString();
        }

        #region Eventos menú contextual
        private void tooStripDuplicar_Click(object sender, EventArgs e)
        {
            if (dgvDatos.ActiveRow == null || dgvDatos.ActiveRow.IsAddRow)
            {
                this.SetMensaje("Debe seleccionar una fila.", 5000, Color.Red, Color.White);
            }

            UltraGridRow newRow = dgvDatos.ActiveRow;

            DataRow row = (dgvDatos.DataSource as DataTable).NewRow();
            foreach (DataColumn item in (dgvDatos.DataSource as DataTable).Columns)
            {


                row[item.ColumnName] = newRow.Cells[item.ColumnName].Value;

                //no duplicar cantidad pedniente y numero de linea
                if (item.ColumnName.Equals("OpenQty"))
                    row[item.ColumnName] = newRow.Cells["Quantity"].Value;
                if (item.ColumnName.Equals("LineNum"))
                    row[item.ColumnName] = -1;
                if (item.ColumnName.Equals("LineStatus"))
                    row[item.ColumnName] = "O";
            }
            (dgvDatos.DataSource as DataTable).Rows.Add(row);

            this.dgvDatos.Rows.Move(this.dgvDatos.Rows[this.dgvDatos.Rows.Count - 1], newRow.Index + 1);
        }

        bool deleteRow = false;
        private void toolStripEliminar_Click(object sender, EventArgs e)
        {
            deleteRow = true;

            if (dgvDatos.ActiveRow != null && !dgvDatos.ActiveRow.IsAddRow)
            {
                int _lineNum = Convert.ToInt32(dgvDatos.ActiveRow.Cells["LineNum"].Value == DBNull.Value ? -1 : dgvDatos.ActiveRow.Cells["LineNum"].Value);
                string _status = Convert.ToString(dgvDatos.ActiveRow.Cells["LineStatus"].Value);

                if (Convert.ToInt32(dgvDatos.ActiveRow.Cells["LineNum"].Value == DBNull.Value ? -1 : dgvDatos.ActiveRow.Cells["LineNum"].Value) < 0)
                    this.dgvDatos.ActiveRow.Delete();

                //else
                //    this.SetMensaje("Seleccione una línea [LineNum = -1]", 5000, Color.Green, Color.Black);
                if (_lineNum > -1 && _status.Equals("O"))
                {
                    if (this.dgvDatos.ActiveRow.Delete())
                    {
                        SDK.SDKDatos.SDK_DI sdk = new SDKDatos.SDK_DI(1);//--Orden de compra

                        sdk.EliminarLineaOrdenCompra(this, DocumentSAP, _lineNum);

                        this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
                    }
                }
            }
        }

        private void agregarFilaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dgvDatos.ActiveRow != null && !dgvDatos.ActiveRow.IsAddRow)
                this.dgvDatos.DisplayLayout.Bands[0].AddNew();
        }

        private void cerrarLíneaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvDatos.ActiveRow != null && !dgvDatos.ActiveRow.IsAddRow)
                {
                    int _lineNum = -1;
                    if (dgvDatos.ActiveRow.Cells["LineNum"].Value != DBNull.Value)
                        _lineNum = Convert.ToInt32(dgvDatos.ActiveRow.Cells["LineNum"].Value);
                    else
                        this.SetMensaje("Seleccione una línea [LineNum = -1]", 5000, Color.Green, Color.Black);

                    if (_lineNum != -1)
                    {
                        SDK.SDKDatos.SDK_DI sdk = new SDKDatos.SDK_DI(1);//--Orden de compra
                        sdk.CerrarLineaOrdenCompra(this, DocumentSAP, _lineNum);
                        this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion

        private void txtMoneda_TextChanged(object sender, EventArgs e)
        {
            lm1.Text = txtMoneda.Text;
            lm2.Text = txtMoneda.Text;
            lm3.Text = txtMoneda.Text;

            try
            {
                //if (txtMoneda.Text == "$")
                //    (dgvDatos.DataSource as DataTable).Columns["LineTotal"].Expression = "Price*Quantity*Rate";
                //if (txtMoneda.Text == "USD")
                //    (dgvDatos.DataSource as DataTable).Columns["LineTotal"].Expression = "Price*Quantity/Rate";

                /*29-04-16: CORRECCION DE BUG, AL CAMBIAR LA MONDEDA DEL DOCUMENTO SE ACTUALIZA
                 * EL CAMPO LineTotal DE ACUERDO A LA MONEDA SELECCIONADA*/
                if (config.ModeDocument != "Edit")
                    foreach (UltraGridRow item in dgvDatos.Rows)
                    {
                        if (item.Cells["Currency"].Value.ToString() == txtMoneda.Text)
                            item.Cells["Rate"].Value = 1;
                        else
                            item.Cells["Rate"].Value = config.Rate;
                    }

                (dgvDatos.DataSource as DataTable).AcceptChanges();

            }
            catch (Exception)
            {
            }
        }

        public void txtFolio_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar == (int)Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtFolio.Text))
                    {
                        SDK.Documentos.frmListadoDocumentos formulario =
                            new frmListadoDocumentos(IdDocument, txtCardCode.Text.Replace('*', '%'),
                                txtCardName.Text.Replace('*', '%'), dtpDocDate.Value, txtNumAtCard.Text.Replace('*', '%'));

                        if (formulario.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                        {
                            txtFolio.Text = formulario.DocEntry.ToString();
                            typeDocument = formulario.Tipo;
                        }
                        else
                        {
                            //this.CleanScreen();
                            return;
                        }
                    }

                    #region Documento Encontrado
                    (dgvDatos.DataSource as DataTable).Rows.Clear();

                    DocumentSAP = new SDK_SAP.Clases.Document();
                    DocumentSAP = DocumentSAP.Fill_OPOR(Convert.ToDecimal(txtFolio.Text), ObjType, typeDocument);

                    #region LLenar Form
                    this.CleanScreen();
                    txtFolio.Text = DocumentSAP.DocNum.ToString();
                    txtCardCode.Text = DocumentSAP.CardCode;
                    txtCardName.Text = DocumentSAP.CardName;
                    txtMoneda.Text = DocumentSAP.DocCurrency;
                    cbMoneda.SelectedValue = DocumentSAP.DocCurrency;
                    txtNumAtCard.Text = DocumentSAP.NumAtCard;

                    cbMoneda.Enabled = false;
                    if (DocumentSAP.DocStatus == "C")
                    {
                        txtStatus.Text = "Cerrado";
                        btnCrear.Enabled = false;
                        cerrarStripButton.Enabled = false;

                        cerrarLíneaToolStripMenuItem.Enabled = false;
                        agregarFilaToolStripMenuItem1.Enabled = false;
                        tooStripDuplicar.Enabled = false;
                    }
                    else if (DocumentSAP.DocStatus == "O")
                    {
                        txtStatus.Text = "Abierto";
                        btnCrear.Enabled = true;
                        cerrarStripButton.Enabled = true;
                        cerrarLíneaToolStripMenuItem.Enabled = true;
                        agregarFilaToolStripMenuItem1.Enabled = true;
                        tooStripDuplicar.Enabled = true;
                    }
                    else if (DocumentSAP.DocStatus == "P")
                    {
                        txtStatus.Text = "Pendiente";
                        btnCrear.Text = "Autorizar";
                        btnCrear.Enabled = true;
                        cerrarStripButton.Enabled = true;
                        cerrarLíneaToolStripMenuItem.Enabled = true;
                        agregarFilaToolStripMenuItem1.Enabled = true;
                        tooStripDuplicar.Enabled = true;
                    }
                    else if (DocumentSAP.DocStatus == "A")
                    {
                        txtStatus.Text = "Autorizado";
                        btnCrear.Text = "Crear";
                        btnCrear.Enabled = true;
                        cerrarStripButton.Enabled = false;
                        cerrarLíneaToolStripMenuItem.Enabled = false;
                        agregarFilaToolStripMenuItem1.Enabled = false;
                        tooStripDuplicar.Enabled = false;
                    }
                    else if (DocumentSAP.DocStatus == "R")
                    {
                        txtStatus.Text = "Rechazado";
                        btnCrear.Text = "Crear";
                        btnCrear.Enabled = false;
                        cerrarStripButton.Enabled = false;
                        cerrarLíneaToolStripMenuItem.Enabled = false;
                        agregarFilaToolStripMenuItem1.Enabled = false;
                        tooStripDuplicar.Enabled = false;
                    }


                    dtpDocDate.Value = DocumentSAP.DocDate;
                    dtpDocDueDate.Value = DocumentSAP.DocDueDate;
                    txtComments.Text = DocumentSAP.Comments;
                    txtSubtotal.Text = (DocumentSAP.DocTotal - DocumentSAP.VatSum).ToString("N2");
                    txtIVA.Text = DocumentSAP.VatSum.ToString("N2");
                    txtTotal.Text = DocumentSAP.DocTotal.ToString("N2");

                    #region Detalle
                    foreach (var item in DocumentSAP.Lines)
                    {
                        DataRow row = (dgvDatos.DataSource as DataTable).NewRow();
                        row["ItemCode"] = item.ItemCode;
                        row["ItemName"] = item.ItemDescription;
                        row["Quantity"] = item.Quantity;
                        row["OpenQty"] = item.OpenQty;
                        row["Price"] = item.Price;
                        row["Currency"] = item.Currency;
                        row["WhsCode"] = item.WarehouseCode;
                        row["WhsName"] = item.WarehouseCode + " | " + item.WarehouseName;
                        row["LineNum"] = item.LineNum;
                        row["Rate"] = item.Rate;
                        row["ShipDate"] = item.ShipDate;
                        row["LineTotal"] = item.LineTotal;
                        row["LineStatus"] = item.LineStatus;
                        row["U_Vendedor"] = item.U_Vendedor;
                        row["SlpCode"] = item.U_Vendedor;
                        row["U_Comentario"] = item.U_Comentario;
                        row["VolumenU"] = item.VolumenU;
                        row["PesoU"] = item.PesoU;

                        row["OnHand"] = item.OnHand;
                        row["Ideal"] = item.Ideal;

                        row["ABC"] = item.ABC;

                        (dgvDatos.DataSource as DataTable).Rows.Add(row);

                        (dgvDatos.DataSource as DataTable).AcceptChanges();
                    }
                    #endregion
                    #endregion

                    #endregion

                    #region Eliminar
                    /*
                      #region Documento Encontrado
                    (dgvDatos.DataSource as DataTable).Rows.Clear();

                    Document = new SDKDatos.SDK_OPOR();
                    Document = Document.Fill(Convert.ToDecimal(txtFolio.Text));

                    #region LLenar Form
                    this.CleanScreen();
                    txtFolio.Text = Document.DocNum.ToString();
                    txtCardCode.Text = Document.CardCode;
                    txtCardName.Text = Document.CardName;
                    txtMoneda.Text = Document.DocCur;
                    cbMoneda.SelectedValue = Document.DocCur;
                    txtNumAtCard.Text = Document.NumAtCard;

                    cbMoneda.Enabled = false;
                    if (Document.DocStatus == "C")
                    {
                        txtStatus.Text = "Cerrado";
                        btnCrear.Enabled = false;
                        cerrarStripButton.Enabled = false;

                        cerrarLíneaToolStripMenuItem.Enabled = false;
                        agregarFilaToolStripMenuItem1.Enabled = false;
                        tooStripDuplicar.Enabled = false;
                    }
                    else if (Document.DocStatus == "O")
                    {
                        txtStatus.Text = "Abierto";
                        btnCrear.Enabled = true;
                        cerrarStripButton.Enabled = true;
                        cerrarLíneaToolStripMenuItem.Enabled = true;
                        agregarFilaToolStripMenuItem1.Enabled = true;
                        tooStripDuplicar.Enabled = true;
                    }

                    dtpDocDate.Value = Document.DocDate;
                    dtpDocDueDate.Value = Document.DocDueDate;
                    txtComments.Text = Document.Comments;
                    txtSubtotal.Text = (Document.DocTotal - Document.VatSum).ToString("N2");
                    txtIVA.Text = Document.VatSum.ToString("N2");
                    txtTotal.Text = Document.DocTotal.ToString("N2");

                    #region Detalle
                    foreach (SDKDatos.SDK_POR1 item in Document.Lines)
                    {
                        DataRow row = (dgvDatos.DataSource as DataTable).NewRow();
                        row["ItemCode"] = item.ItemCode;
                        row["ItemName"] = item.Dscription;
                        row["Quantity"] = item.Quantity;
                        row["OpenQty"] = item.OpenQty;
                        row["Price"] = item.Price;
                        row["Currency"] = item.Currency;
                        row["WhsCode"] = item.Whscode;
                        row["WhsName"] = item.Whscode + " | " + item.WhsName;
                        row["LineNum"] = item.LineNum;
                        row["Rate"] = item.Rate;
                        row["ShipDate"] = item.ShipDate;
                        row["LineTotal"] = item.LineTotal;
                        row["LineStatus"] = item.LineStatus;
                        row["U_Vendedor"] = item.U_Vendedor;
                        row["SlpCode"] = item.U_Vendedor;
                        row["U_Comentario"] = item.U_Comentario;
                        row["VolumenU"] = item.VolumenU;
                        row["PesoU"] = item.PesoU;

                        (dgvDatos.DataSource as DataTable).Rows.Add(row);

                        (dgvDatos.DataSource as DataTable).AcceptChanges();
                    }
                    #endregion
                    #endregion

                    #endregion
                     */
                    #endregion

                    txtCardName.ReadOnly = true;
                    txtCardName.ReadOnly = true;
                }

                #region solo lectura;
                if (ReadOnly)
                {
                    btnCrear.Enabled = false;
                    btnCancelar.Enabled = false;
                    contextMenu.Enabled = false;

                    foreach (var item in dgvDatos.DisplayLayout.Bands[0].Columns)
                    {
                        item.CellActivation = Activation.NoEdit;
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                this.SetMensaje("Error al cargar OC: " + ex.Message, 5000, Color.Red, Color.White);
            }

        }

        #region Eventos de botones toolStrip
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            this.CleanScreen();
            txtFolio.BackColor = Color.FromName("Window");
            txtCardCode.BackColor = Color.FromName("Window");
            txtCardName.BackColor = Color.FromName("Window");
            txtNumAtCard.BackColor = Color.FromName("Window");

            txtCardCode.Clear();
            txtCardName.Clear();
            cbMoneda.SelectedIndex = 0;
            txtMoneda.Clear();
            txtNumAtCard.Clear();
            txtFolio.Clear();
            txtStatus.Clear();
            dtpDocDate.Value = DateTime.Now;
            dtpDocDueDate.Value = DateTime.Now;
            (dgvDatos.DataSource as DataTable).Rows.Clear();
            txtComments.Text = "OC Generada por Halconet.";
            txtSubtotal.Clear();
            txtIVA.Clear();
            txtTotal.Clear();

            txtCardCode.Focus();
            btnCrear.Text = "Crear";
            config.ModeDocument = "New";

            txtFolio.KeyPress -= new KeyPressEventHandler(txtFolio_KeyPress);
            dtpDocDate.KeyPress -= new KeyPressEventHandler(txtFolio_KeyPress);
            txtCardCode.KeyPress -= new KeyPressEventHandler(txtFolio_KeyPress);
            txtCardName.KeyPress -= new KeyPressEventHandler(txtFolio_KeyPress);
            this.dgvDatos.DataSource = (dgvDatos.DataSource as DataTable).Copy();

            txtCardName.ReadOnly = false;
            txtCardCode.ReadOnly = false;
            txtCardName.Enabled = true;

            agregarFilaToolStripMenuItem1.Enabled = true;
            //toolStripEliminar.Enabled = true;
            //tooStripDuplicar.Enabled = true;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (this.btnCrear.Text == "Crear")
            {
                SDK.Documentos.frmDocumentos formulario = new SDK.Documentos.frmDocumentos(1);
                formulario.MdiParent = this.MdiParent;
                formulario.Show();
                return;
            }

            txtCardCode.ReadOnly = false;
            txtCardCode.Enabled = true;

            txtCardName.ReadOnly = false;
            txtCardName.Enabled = true;
            this.CleanScreen();
            txtFolio.Focus();

            cerrarLíneaToolStripMenuItem.Enabled = true;
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                if (!typeDocument.Equals("OC"))
                {
                    this.SetMensaje("Imposible crear versión impresa.", 5000, Color.Red, Color.White);
                    return;
                }

                Constantes.Clases.LoadRPT rpt = new Constantes.Clases.LoadRPT(IdDocument);
                Constantes.frmVisor form = new Constantes.frmVisor(rpt.DocRPT);
                rpt.GenerarPDF(DocumentSAP.DocEntry.ToString());
                form.MdiParent = this.MdiParent;
                form.Show();
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
            // System.Diagnostics.Process.Start(rpt.GenerarPDF(Document.DocEntry.ToString()));
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Desea Cerrar esta orden de compra ?\r\n Numero de documento" + txtFolio.Text, "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == System.Windows.Forms.DialogResult.Yes)
            {
                SDK.SDKDatos.SDK_DI sdk = new SDKDatos.SDK_DI(1);//--Orden de compra
                sdk.CerrarOrdenCompra(this, DocumentSAP);
                this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
            }
        }
        #endregion

        private void dgvDatos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            //QUITAR
            try
            {
                if (e.Row.Cells["LineStatus"].Value.ToString() == "C")
                {
                    e.Row.Appearance.BackColor = Color.LightGray;

                    e.Row.Cells["ItemCode"].Activation = Activation.NoEdit;
                    e.Row.Cells["ItemName"].Activation = Activation.NoEdit;
                    e.Row.Cells["WhsName"].Activation = Activation.NoEdit;
                }
                else
                    if (!Convert.ToDecimal(e.Row.Cells["Quantity"].Value).Equals(
                       Convert.ToDecimal(e.Row.Cells["OpenQty"].Value)))
                    {
                        e.Row.Appearance.ForeColor = Color.Red;

                        e.Row.Cells["ItemCode"].Activation = Activation.NoEdit;
                        e.Row.Cells["ItemName"].Activation = Activation.NoEdit;
                        e.Row.Cells["WhsName"].Activation = Activation.NoEdit;
                    }

                if (config.ModeDocument != "Edit")
                    if (e.Row.Cells["Currency"].Value.ToString() == txtMoneda.Text)
                        e.Row.Cells["Rate"].Value = 1;
                    else
                        e.Row.Cells["Rate"].Value = config.Rate;
                /*29-04-16: CORRECCION DE BUG, YA SE PERMITEN CREAR DOCUMENTOS DESDE CERO*/
                //(dgvDatos.DataSource as DataTable).AcceptChanges();
            }
            catch (Exception)
            {
            }
        }

        private void txtCardCode_Leave(object sender, EventArgs e)
        {
            if (!btnCrear.Text.Equals("btnCrear"))
                using (SqlConnection cnnection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    SqlCommand cmd = new SqlCommand("sp_SDKDataSource", cnnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TipoConsulta", 7));
                    cmd.Parameters.Add(new SqlParameter("@Key", txtCardCode.Text));

                    DataTable sn = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    da.Fill(sn);

                    if (sn.Rows.Count > 0)
                    {
                        txtCardName.Text = sn.Rows[0].Field<string>("CardName");
                        cbMoneda.SelectedValue = sn.Rows[0].Field<string>("Currency");
                        txtMoneda.Text = sn.Rows[0].Field<string>("Currency") == "##" ? string.Empty : sn.Rows[0].Field<string>("Currency");
                        VatGroup = sn.Rows[0].Field<string>("VatGroup");
                        config.VatGroup = VatGroup;
                      // Rate = sn.Rows[0].Field<decimal>("Rate");
                        cbMoneda.Enabled = sn.Rows[0].Field<string>("Currency").Equals("##");

                        dtpDocDate_ValueChanged(sender, e);
                    }
                }
        }

        private void dgvDatos_SummaryValueChanged(object sender, SummaryValueChangedEventArgs e)
        {
            try
            {

                if (config.ModeDocument == "New")
                {
                    decimal total = Convert.ToDecimal(e.SummaryValue.Value);

                    txtSubtotal.Text = total.ToString("N2");

                    if (VatGroup.Equals("IVAAC16"))
                    {
                        txtIVA.Text = (total * IVA).ToString("N2");
                        txtTotal.Text = (total * (1 + IVA)).ToString("N2");
                    }
                    else
                    {
                        txtIVA.Text = (decimal.Zero).ToString("N2");
                        txtTotal.Text = total.ToString("N2");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void dgvDatos_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            if (!deleteRow)
            {
                e.Cancel = true;
                e.DisplayPromptMsg = false;
            }
            deleteRow = false;
        }

        private void frmDocumentos_Shown(object sender, EventArgs e)
        {
            if (ReadOnly)
                if (!ClasesSGUV.Forms.GetPermisoReporte(this.Name))
                {
                    MessageBox.Show("Usuario no autorizado.\r\nContacta al administrador.", "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.Close();
                }
        }

        private void dtpDocDate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
               // if (cbMoneda.SelectedValue.ToString().Equals("USD"))
                    using (SqlConnection cnnection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        SqlCommand cmd = new SqlCommand("sp_SDKDataSource", cnnection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@TipoConsulta", 16));
                        cmd.Parameters.Add(new SqlParameter("@Key", dtpDocDate.Value));

                        cnnection.Open();

                        Rate = Convert.ToDecimal(cmd.ExecuteScalar());
                    }
            }
            catch (Exception)
            {
                this.SetMensaje("No existe definición para TC", 5000, Color.Red, Color.White);
                Rate = 1;
            }
        }

        private void ultraDropDown1_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["Price"].Hidden = true;
            e.Layout.Bands[0].Columns["Currency"].Hidden = true;
            e.Layout.Bands[0].Columns["Rate"].Hidden = true;
            e.Layout.Bands[0].Columns["ManBtchNum"].Hidden = true;
            e.Layout.Bands[0].Columns["OnHand"].Hidden = true;
            e.Layout.Bands[0].Columns["Ideal"].Hidden = true;
            e.Layout.Bands[0].Columns["Clasificacion"].Hidden = true;

            e.Layout.Bands[0].Columns["ItemName"].Width = 300;
        }
    }
}