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
    public partial class frmCreditMemo : Constantes.frmEmpty
    {
        private enum Documento
        {
            CreditMemo = 14,
            Devolucion = 16,
            PurchaseOrder = 18
        }
        public int DocumentType;

        int IdDocument;
        Decimal IVA;
        string VatGroup;
        SDK.Configuracion.SDK_Configuracion_CreditMemo config;
        SDK_SAP.Clases.Document Document;
        DataTable tbl_Lotes = new DataTable();
        string DocumentMode;/*Nuevo|Registrar|Creado|Consulta*/
        private decimal DocKey; private int ObjType; private decimal WtmCode; private decimal WddCode;
        private string NumAtCard;

        public frmCreditMemo(int _idDocument)
        {
            InitializeComponent();

            Document = new SDK_SAP.Clases.Document();
            dgvDatos.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
            //  I  N  I  C  I  A  L  I  Z  A  R
            IdDocument = _idDocument;
            config = new SDK.Configuracion.SDK_Configuracion_CreditMemo(IdDocument, this);
            config.ModeDocument = "New";
            DocumentMode = "Nuevo";
            config.StartEmpty();
            ObjType = config.ObjType;

            IVA = config.IVA1;
            this.AccessibleDescription = "SDK " + this.Text;
            txtTC.Text = config.Rate.ToString("N2");
        }

        public frmCreditMemo(int _idDocument, decimal _DocKey, int _ObjType, decimal _WtmCode, decimal _WddCode)
        {
            InitializeComponent();

            Document = new SDK_SAP.Clases.Document();
            dgvDatos.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
            //  I  N  I  C  I  A  L  I  Z  A  R
            IdDocument = _idDocument;
            config = new SDK.Configuracion.SDK_Configuracion_CreditMemo(IdDocument, this);
            config.ModeDocument = "New";
            DocumentMode = "Consulta";
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
            txtTC.Clear();
            txtFolio.Clear();
            txtStatus.Clear();

            cbIndicador.SelectedIndex = 0;
            cbMetPago.SelectedIndex = 0;
            cbMotivo.SelectedIndex = 0;

            (dgvDatos.DataSource as DataTable).Rows.Clear();
            txtComments.Text = "Generado por Halconet.";
            txtTotal.Clear();
            txtSubtotal.Clear();
            txtIVA.Clear();
            dtpDocDate.Value = DateTime.Now;
            dtpDocDueDate.Value = DateTime.Now;
        }

        private void Formulario_Modo()
        {
            switch (DocumentMode)
            {
                case "Nuevo":
                    {
                        //buscarStripButton.Enabled = true;
                        guardarToolStripButton.Enabled = false;
                        exportarToolStripButton.Enabled = false;
                        actualizarToolStripButton.Enabled = false;
                        tooStripDuplicar.Enabled = false;
                        btnCrear.Enabled = true;
                        btnCopiarDe.Enabled = true;
                        txtFolio.ReadOnly = true;
                    } break;
                case "Registrar":
                    {
                        //buscarStripButton.Enabled = false;
                        guardarToolStripButton.Enabled = false;
                        exportarToolStripButton.Enabled = false;
                        actualizarToolStripButton.Enabled = false;
                        tooStripDuplicar.Enabled = false;
                        btnCrear.Enabled = true;
                        btnCopiarDe.Enabled = true;
                        txtFolio.ReadOnly = true;
                    } break;
                case "Creado":
                    {
                        //buscarStripButton.Enabled = false;
                        guardarToolStripButton.Enabled = false;
                        exportarToolStripButton.Enabled = false;
                        actualizarToolStripButton.Enabled = false;
                        tooStripDuplicar.Enabled = false;
                        btnCrear.Enabled = false;
                        btnCopiarDe.Enabled = false;
                        txtFolio.ReadOnly = true;
                    } break;
                case "Consulta":
                    {
                        //buscarStripButton.Enabled = false;
                        guardarToolStripButton.Enabled = false;
                        exportarToolStripButton.Enabled = false;
                        actualizarToolStripButton.Enabled = false;
                        tooStripDuplicar.Enabled = false;
                        btnCrear.Enabled = false;
                        txtFolio.ReadOnly = false;
                        btnCopiarDe.Enabled = false;

                        txtFolio.BackColor = Color.FromName("Info");
                        txtCardCode.BackColor = Color.FromName("Info");
                        txtCardName.BackColor = Color.FromName("Info");
                    } break;
            }
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
                if (IdDocument == 8)
                    DocumentType = (int)Documento.CreditMemo;
                if (IdDocument == 9)
                    DocumentType = (int)Documento.Devolucion;
                if (IdDocument == 1)
                    DocumentType = (int)Documento.PurchaseOrder;
                

                Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                actualizarToolStripButton.Click -= new EventHandler(frmOrdenCompra_Load);
                actualizarToolStripButton.Click += new EventHandler(frmOrdenCompra_Load);

                dgvDatos.KeyDown -= new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);
                dgvDatos.KeyDown += new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);

                dgvDatos.KeyPress -= new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);
                dgvDatos.KeyPress += new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);

                this.dgvDatos.PerformAction(UltraGridAction.Copy, true, true);
                this.dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.DeleteRows);

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


                nuevoToolStripButton.Click -= new EventHandler(btnNuevo_Click);
                nuevoToolStripButton.Click += new EventHandler(btnNuevo_Click);

                buscarStripButton.Click -= new EventHandler(btnBuscar_Click);
                buscarStripButton.Click += new EventHandler(btnBuscar_Click);

                imprimirToolStripButton.Click -= new EventHandler(btnImprimir_Click);
                imprimirToolStripButton.Click += new EventHandler(btnImprimir_Click);

                cerrarStripButton.Click -= new EventHandler(btnCerrar_Click);
                cerrarStripButton.Click += new EventHandler(btnCerrar_Click);

                this.Formulario_Modo();

                ClasesSGUV.Form.ControlsForms.setDataSource(cbMetPago, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.DocMetodosPago, string.Empty, string.Empty), "Name", "Code", "");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbMotivo, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.DocMotivo, string.Empty, string.Empty), "Name", "Code", "");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbIndicador, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.DocIndicator, string.Empty, string.Empty), "Name", "Code", "");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbSeries, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.DocSeries, ObjType, string.Empty), "Name", "Code", "");

                if (!String.IsNullOrWhiteSpace(txtFolio.Text))
                {
                    var kea = new KeyPressEventArgs(Convert.ToChar(13));
                    this.txtFolio_KeyPress(sender, kea);
                    cbSeries.Enabled = false;
                }
                else
                {
                    if (DocumentType == (int)Documento.CreditMemo)
                    {
                        cbMetPago.SelectedValue = "99";
                        Datos.Connection connection = new Datos.Connection();
                        Object[] valuesOut = new Object[] { };
                        DataTable tbl =
                            connection.GetDataTable("LOG",
                                                    "sp_SDKDataSource",
                                                    new string[] { },
                                                    new string[] { "@TipoConsulta", "@ObjType", "@Key" },
                                                    ref valuesOut, 17, ObjType, ClasesSGUV.Login.Id_Usuario);

                        if (tbl.Rows.Count > 0)
                            cbSeries.SelectedValue = tbl.Rows[0]["Serie"];

                        btnCopiarDe.Text = "Copiar de Factura";
                    }
                    if (DocumentType == (int)Documento.Devolucion)
                    {
                        Datos.Connection connection = new Datos.Connection();
                        Object[] valuesOut = new Object[] { };
                        DataTable tbl =
                            connection.GetDataTable("LOG",
                                                    "sp_SDKDataSource",
                                                    new string[] { },
                                                    new string[] { "@TipoConsulta", "@ObjType", "@Key" },
                                                    ref valuesOut, 17, ObjType, ClasesSGUV.Login.Id_Usuario);

                        if (tbl.Rows.Count > 0)
                            cbSeries.SelectedValue = tbl.Rows[0]["Serie"];

                        btnCopiarDe.Text = "Copiar de Entrega";
                    }
                }

                buscarStripButton.Enabled = true;
                imprimirToolStripButton.Enabled = true;
            }
            catch (Exception)
            {
                this.SetMensaje("Error: al cargar Form frm_Documentos", 5000, Color.Red, Color.White);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Los cambios hechos se perderán\r\n¿Desea continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            this.Close();
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (btnCrear.Text == "Crear")
            {
                #region nota de credito
                if (DocumentType == (int)Documento.CreditMemo)
                {
                    if (string.IsNullOrEmpty(txtMoneda.Text))
                    {
                        this.SetMensaje("Campo MONEDA obligatorio", 5000, Color.Red, Color.White);
                        return;
                    }

                    if (cbIndicador.SelectedIndex == 0)
                    {
                        this.SetMensaje("Campo INDICADOR obligatorio", 5000, Color.Red, Color.White);
                        return;
                    }
                    if (cbMetPago.SelectedIndex == 0)
                    {
                        this.SetMensaje("Campo METODO DE PAGO obligatorio", 5000, Color.Red, Color.White);
                        return;
                    }
                    if (cbMotivo.SelectedIndex == 0)
                    {
                        this.SetMensaje("Campo MOTIVO obligatorio", 5000, Color.Red, Color.White);
                        return;
                    }
                    if (cbSeries.SelectedIndex == 0)
                    {
                        this.SetMensaje("Campo SERIES obligatorio", 5000, Color.Red, Color.White);
                        return;
                    }
                    #region Crear
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        //Document = new SDK_SAP.Clases.Document();
                        Document.Lines.Clear();
                        Document.DocType = "dDocument_Items";
                        Document.Comments = txtComments.Text;
                        Document.CardCode = txtCardCode.Text;
                        Document.DocDate = dtpDocDate.Value;
                        Document.DocDueDate = dtpDocDueDate.Value;
                        Document.DocCurrency = txtMoneda.Text;
                        Document.DocTotal = Convert.ToDouble(txtTotal.Text != string.Empty ? txtTotal.Text : "0");
                        Document.VatSum = Convert.ToDouble(txtIVA.Text != string.Empty ? txtIVA.Text : "0");
                        Document.DocRate = txtMoneda.Text == "USD" ? Convert.ToDouble(txtTC.Text) : (double)1;
                        Document.U_MPAGO2 = cbMetPago.SelectedValue.ToString();
                        Document.U_Motivo = cbMotivo.SelectedValue.ToString();
                        Document.Indicator = cbIndicador.SelectedValue.ToString();
                        Document.Series = Convert.ToInt32(cbSeries.SelectedValue);
                        Document.NumAtCard = NumAtCard;

                        if (Document.Indicator.Equals("CF"))
                            Document.EDocGenerationType = "N";
                        else
                            Document.EDocGenerationType = "G";

                        foreach (var item in dgvDatos.Rows)
                        {
                            SDK_SAP.Clases.DocumentLines line = new SDK_SAP.Clases.DocumentLines();

                            line.ItemCode = Convert.ToString(item.Cells["ItemCode"].Value);
                            line.ItemDescription = Convert.ToString(item.Cells["ItemName"].Value);
                            line.Quantity = Convert.ToDouble(item.Cells["Quantity"].Value);
                            line.Currency = Convert.ToString(item.Cells["Currency"].Value);
                            line.WarehouseCode = Convert.ToString(item.Cells["WhsCode"].Value);
                            line.Price = Convert.ToDouble(item.Cells["Price"].Value);
                            line.Rate = Convert.ToDouble(item.Cells["Rate"].Value);
                            line.U_Comentario = Convert.ToString(item.Cells["U_Comentario"].Value);
                            line.ManBtchNum = Convert.ToString(item.Cells["ManBtchNum"].Value);
                            line.LineTotal = Convert.ToDouble(item.Cells["LineTotal"].Value);
                            line.AccountCode = Convert.ToString(item.Cells["AccountCode"].Value);
                            line.TaxCode = Convert.ToString(item.Cells["TaxCode"].Value);

                            line.BaseEntry = (int?)(item.Cells["BaseEntry"].Value != DBNull.Value ? item.Cells["BaseEntry"].Value : null);
                            line.BaseLine = (int?)(item.Cells["BaseLine"].Value != DBNull.Value ? item.Cells["BaseLine"].Value : null);
                            line.BaseType = (int?)(item.Cells["BaseType"].Value != DBNull.Value ? item.Cells["BaseType"].Value : null);

                            #region Lotes
                            if (line.ManBtchNum.Equals("Y"))
                            {
                                SDK_SAP.Clases.Lotes objLote = new SDK_SAP.Clases.Lotes();
                                DataTable tbl_Lot = objLote.LotesDocumentoBase(line.Quantity, (int)line.BaseType, (int)line.BaseEntry, (int)line.BaseLine);

                                foreach (DataRow itemLote in tbl_Lot.Rows)
                                {
                                    SDK_SAP.Clases.Lotes oLot = new SDK_SAP.Clases.Lotes();
                                    oLot.Lote = itemLote.Field<string>("BatchNum");
                                    oLot.Cantidad = (double)itemLote.Field<decimal>("Quantity");
                                    line.LotesList.Add(oLot);
                                }
                            }
                            #endregion

                            Document.Lines.Add(line);
                        }

                        bool finalizar = true;

                        if (!string.IsNullOrEmpty(txtFolio.Text))
                        {
                            #region Lotes
                            bool viewLotes = false;
                            bool LotesOk = true;

                            viewLotes = false;
                            finalizar = true;
                            //foreach (var item in Document.Lines)
                            //{
                            //    if (item.ManBtchNum.Equals("Y"))
                            //    {
                            //        if (item.Quantity != item.LotesList.Sum(lote_item => lote_item.Cantidad))
                            //        {
                            //            viewLotes = true;
                            //        }
                            //        foreach (var lote in item.LotesList)
                            //        {
                            //            if (!string.IsNullOrEmpty(lote.Pedimento))
                            //                viewLotes = true;
                            //            if (!string.IsNullOrEmpty(lote.Aduana))
                            //                viewLotes = true;
                            //        }
                            //    }
                            //}

                            //if (viewLotes)
                            //{
                            //    frmLotes form = new frmLotes(Document, tbl_Lotes);
                            //    if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                            //        return;
                            //    else
                            //        tbl_Lotes = form.Tbl_lotes;
                            //}

                            foreach (var item in Document.Lines)
                            {
                                if (item.ManBtchNum.Equals("Y"))
                                {
                                    if (item.Quantity != item.LotesList.Sum(lote_item => lote_item.Cantidad))
                                    {
                                        finalizar = false;
                                    }
                                }
                            }

                            //foreach (var item in Document.Lines)
                            //{
                            //    if (item.ManBtchNum.Equals("Y"))
                            //    {
                            //        foreach (var lote in item.LotesList)
                            //        {
                            //            if (string.IsNullOrEmpty(lote.Pedimento))
                            //                LotesOk = false;
                            //            if (string.IsNullOrEmpty(lote.Aduana))
                            //                LotesOk = false;
                            //        }
                            //    }
                            //}

                            if (!LotesOk)
                            {
                                this.SetMensaje("Falta ingresar informacion Aduana|Pedimento", 5000, Color.Red, Color.White);
                                return;
                            }


                            #endregion
                        }

                        if (finalizar)
                        {
                            if (!(Document.DocStatus == null ? "" : Document.DocStatus).Equals("A"))
                            {
                                Document.DocType = ObjType.ToString();
                                SDK_SAP.DI.Documents doc = new SDK_SAP.DI.Documents(6);
                                txtFolio.Text = Document.SaveDraft(Document).ToString();

                                Document = Document.Fill(Document.DocNum, 14, "Borrador");
                                txtFolio.Text = Document.DocNum.ToString();

                                if (Document.DocStatus.Equals("P"))
                                    txtStatus.Text = "Pendiente";
                                else if (Document.DocStatus.Equals("R"))
                                    txtStatus.Text = "Rechazado";
                                else if (Document.DocStatus.Equals("A"))
                                    txtStatus.Text = "Autorizado";

                                this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
                                DocumentMode = "Creado";
                            }
                            else if (Document.DocStatus.Equals("A"))
                            {
                                SDK_SAP.DI.Connection.InitializeConnection(8);
                                SDK_SAP.DI.Connection.StartConnection();
                                SDK_SAP.DI.Documents doc = new SDK_SAP.DI.Documents(8);

                                txtFolio.Text = doc.AddDocument("ORIN", Document).ToString();

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
                            }
                            this.Formulario_Modo();
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
                #endregion

                #region Devolucion
                if (DocumentType == (int)Documento.Devolucion)
                {
                    if (string.IsNullOrEmpty(txtMoneda.Text))
                    {
                        this.SetMensaje("Campo MONEDA obligatorio", 5000, Color.Red, Color.White);
                        return;
                    }

                    if (cbSeries.SelectedIndex == 0)
                    {
                        this.SetMensaje("Campo SERIES obligatorio", 5000, Color.Red, Color.White);
                        return;
                    }
                    #region Crear
                    try
                    {
                        #region Encabezado
                        Cursor.Current = Cursors.WaitCursor;
                        Document.Lines.Clear();
                        Document.DocType = "dDocument_Items";
                        Document.Comments = txtComments.Text;
                        Document.CardCode = txtCardCode.Text;
                        Document.DocDate = dtpDocDate.Value;
                        Document.DocDueDate = dtpDocDueDate.Value;
                        Document.DocCurrency = txtMoneda.Text;
                        Document.DocTotal = Convert.ToDouble(txtTotal.Text != string.Empty ? txtTotal.Text : "0");
                        Document.VatSum = Convert.ToDouble(txtIVA.Text != string.Empty ? txtIVA.Text : "0");
                        Document.DocRate = txtMoneda.Text == "USD" ? Convert.ToDouble(txtTC.Text) : (double)1;
                   
                        Document.Series = Convert.ToInt32(cbSeries.SelectedValue);
                        Document.NumAtCard = NumAtCard;
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
                            line.Price = Convert.ToDouble(item.Cells["Price"].Value);
                            line.Rate = Convert.ToDouble(item.Cells["Rate"].Value);
                            line.U_Comentario = Convert.ToString(item.Cells["U_Comentario"].Value);
                            line.ManBtchNum = Convert.ToString(item.Cells["ManBtchNum"].Value);
                            line.LineTotal = Convert.ToDouble(item.Cells["LineTotal"].Value);
                            line.TaxCode = Convert.ToString(item.Cells["TaxCode"].Value);

                            line.BaseEntry = (int?)(item.Cells["BaseEntry"].Value != DBNull.Value ? item.Cells["BaseEntry"].Value : null);
                            line.BaseLine = (int?)(item.Cells["BaseLine"].Value != DBNull.Value ? item.Cells["BaseLine"].Value : null);
                            line.BaseType = (int?)(item.Cells["BaseType"].Value != DBNull.Value ? item.Cells["BaseType"].Value : null);

                            #region Lotes
                            if (line.ManBtchNum.Equals("Y"))
                            {
                                SDK_SAP.Clases.Lotes objLote = new SDK_SAP.Clases.Lotes();
                                DataTable tbl_Lot = objLote.LotesDocumentoBase(line.Quantity, (int)line.BaseType, (int)line.BaseEntry, (int)line.BaseLine);

                                foreach (DataRow itemLote in tbl_Lot.Rows)
                                {
                                    SDK_SAP.Clases.Lotes oLot = new SDK_SAP.Clases.Lotes();
                                    oLot.Lote = itemLote.Field<string>("BatchNum");
                                    oLot.Cantidad = (double)itemLote.Field<decimal>("Quantity");
                                    line.LotesList.Add(oLot);
                                }
                            }
                            #endregion

                            Document.Lines.Add(line);
                            #endregion
                        }

                        bool finalizar = true;

                        if (!string.IsNullOrEmpty(txtFolio.Text))
                        {
                            #region Lotes
                            bool LotesOk = true;

                            finalizar = true;
                            foreach (var item in Document.Lines)
                            {
                                if (item.ManBtchNum.Equals("Y"))
                                {
                                    if (item.Quantity != item.LotesList.Sum(lote_item => lote_item.Cantidad))
                                    {
                                        finalizar = false;
                                    }
                                }
                            }
                            if (!LotesOk)
                            {
                                this.SetMensaje("Falta ingresar informacion Aduana|Pedimento", 5000, Color.Red, Color.White);
                                return;
                            }
                            #endregion
                        }

                        if (finalizar)
                        {
                            if (!(Document.DocStatus == null ? "" : Document.DocStatus).Equals("A"))
                            {
                                Document.DocType = ObjType.ToString();
                                SDK_SAP.DI.Documents doc = new SDK_SAP.DI.Documents(6);
                                txtFolio.Text = Document.SaveDraft(Document).ToString();

                                Document = Document.Fill(Document.DocNum, 16, "Borrador");
                                txtFolio.Text = Document.DocNum.ToString();

                                //if (Document.DocStatus.Equals("P"))
                                //    txtStatus.Text = "Pendiente";
                                //else if (Document.DocStatus.Equals("R"))
                                //    txtStatus.Text = "Rechazado";
                                //else if (Document.DocStatus.Equals("A"))
                                //    txtStatus.Text = "Autorizado";

                                var kea = new KeyPressEventArgs(Convert.ToChar(13));
                                this.txtFolio_KeyPress(sender, kea);

                                btnCrear_Click(sender, e);

                                this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
                                DocumentMode = "Creado";
                            }
                            else if ((Document.DocStatus == null ? "" : Document.DocStatus).Equals("A"))
                            {
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
                            }
                            this.Formulario_Modo();
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
                }
                #endregion
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

                        Document = Document.Fill(Document.DocNum, DocumentType, "Borrador");
                        txtFolio.Text = Document.DocNum.ToString();

                        if (Document.DocStatus.Equals("P"))
                            txtStatus.Text = "Pendiente";
                        else if (Document.DocStatus.Equals("R"))
                            txtStatus.Text = "Rechazado";
                        else if (Document.DocStatus.Equals("A"))
                            txtStatus.Text = "Autorizado";

                        this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
                        DocumentMode = "Creado";
                    }
                    catch (Exception ex)
                    {
                        this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
                    }
                }
                #endregion
            }

            if (btnCrear.Text == "Actualizar")
            {
                if (DocumentType == (int)Documento.Devolucion)
                {
                    try
                    {
                        SDK_SAP.DI.Connection.InitializeConnection(9);
                        SDK_SAP.DI.Connection.StartConnection();
                        SDK_SAP.DI.Documents doc = new SDK_SAP.DI.Documents(9);

                        Document.Comments = txtComments.Text;

                        txtFolio.Text = doc.UpdateDocument("ORDN", Document).ToString();

                        this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
                    }
                    catch (Exception ex)
                    {
                        this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
                    }


                }
            }
             
        }

        private void cbMoneda_SelectionChangeCommitted(object sender, EventArgs e)
        {
            txtMoneda.Text = cbMoneda.SelectedValue.ToString();
            if (txtMoneda.Text.Equals("USD"))
                txtTC.Text = config.Rate.ToString("N4");
            else
                txtTC.Text = ((double)1).ToString("N4");
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
                if (item.ColumnName.Equals("LineNum"))
                    row[item.ColumnName] = -1;
                if (item.ColumnName.Equals("LineStatus"))
                    row[item.ColumnName] = "O";

                if (item.ColumnName.Equals("BaseEntry"))
                    row[item.ColumnName] = DBNull.Value;
                if (item.ColumnName.Equals("BaseLine"))
                    row[item.ColumnName] = DBNull.Value;
                if (item.ColumnName.Equals("BaseType"))
                    row[item.ColumnName] = DBNull.Value;
            }
            (dgvDatos.DataSource as DataTable).Rows.Add(row);

            this.dgvDatos.Rows.Move(this.dgvDatos.Rows[this.dgvDatos.Rows.Count - 1], newRow.Index + 1);
        }

        bool deleteRow = false;
        //pendiente
        private void toolStripEliminar_Click(object sender, EventArgs e)
        {
            if (DocumentMode.Equals("Nuevo"))
                return;
            deleteRow = true;

            if (dgvDatos.ActiveRow != null && !dgvDatos.ActiveRow.IsAddRow)
            {
                int _lineNum = Convert.ToInt32(dgvDatos.ActiveRow.Cells["LineNum"].Value == DBNull.Value ? -1 : dgvDatos.ActiveRow.Cells["LineNum"].Value);
                string _status = Convert.ToString(dgvDatos.ActiveRow.Cells["LineStatus"].Value);

                if (Convert.ToInt32(dgvDatos.ActiveRow.Cells["LineNum"].Value == DBNull.Value ? -1 : dgvDatos.ActiveRow.Cells["LineNum"].Value) < 0)
                    this.dgvDatos.ActiveRow.Delete();

                if (_lineNum > -1 && _status.Equals("O"))
                {
                    if (this.dgvDatos.ActiveRow.Delete())
                    {
                        //SDK.SDKDatos.SDK_DI sdk = new SDKDatos.SDK_DI(1);//--Orden de compra

                        //--sdk.EliminarLineaOrdenCompra(this, Document, _lineNum);

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

        //pendiente
        private void cerrarLíneaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DocumentMode.Equals("Nuevo"))
                    return;

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
                        //--sdk.CerrarLineaOrdenCompra(this, Document, _lineNum);
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
            lm4.Text = txtMoneda.Text;
            lm5.Text = txtMoneda.Text;

            try
            {
                if (txtMoneda.Text == "$")
                    (dgvDatos.DataSource as DataTable).Columns["LineTotal"].Expression = "Price*Quantity*Rate";
                if (txtMoneda.Text == "USD")
                    (dgvDatos.DataSource as DataTable).Columns["LineTotal"].Expression = "Price*Quantity/Rate";

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

        string sType = "Borrador";
        public void txtFolio_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                sType = "Borrador";
                switch (DocumentMode)
                {
                    #region Nuevo
                    case "Nuevo":
                        {
                            #region Nuevo
                            if ((int)e.KeyChar == (int)Keys.Enter)
                            {
                                if (string.IsNullOrEmpty(txtFolio.Text))
                                {
                                    SDK.Documentos.frmListadoDocumentos formulario =
                                        new frmListadoDocumentos(-5, txtCardCode.Text.Replace('*', '%'),
                                            txtCardName.Text.Replace('*', '%'), dtpDocDate.Value, txtTC.Text.Replace('*', '%'));

                                    if (formulario.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                                    {
                                        txtCardCode.Text = formulario.Row["CardCode"].ToString();
                                        txtCardCode_Leave(sender, e);
                                        cbMoneda_SelectionChangeCommitted(sender, e);
                                    }
                                    else
                                    {
                                        return;
                                    }

                                }
                                else
                                {

                                }
                            }
                            #endregion
                        } break;
                    case "Consulta":
                        {
                            #region Consultar
                            if ((int)e.KeyChar == (int)Keys.Enter)
                            {
                                if (string.IsNullOrEmpty(txtFolio.Text))
                                {
                                    SDK.Documentos.frmListadoDocumentos formulario =
                                        new frmListadoDocumentos(IdDocument/*8*/, txtCardCode.Text.Replace('*', '%'),
                                            txtCardName.Text.Replace('*', '%'), dtpDocDate.Value, string.Empty, Convert.ToInt32(cbSeries.SelectedValue));

                                    if (formulario.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                                    {
                                        sType = formulario.Tipo;
                                        int docentry = formulario.DocEntry;

                                        txtFolio.Text = docentry.ToString();
                                    }
                                    else
                                    {
                                        return;
                                    }

                                }
                                else
                                {
                                }
                            }
                            #endregion
                        } break;
                    #endregion
                }

                if (!string.IsNullOrEmpty(txtFolio.Text))
                {
                    if ((int)e.KeyChar != (int)Keys.Enter)
                        return;

                    #region Documento Encontrado
                    (dgvDatos.DataSource as DataTable).Rows.Clear();

                    Document = new SDK_SAP.Clases.Document();
                    try
                    {
                        Document = Document.Fill(Convert.ToDecimal(txtFolio.Text), DocumentType, sType);
                        
                        #region LLenar Form
                        this.CleanScreen();
                        #region Encabezado
                        txtFolio.Text = Document.DocNum.ToString();
                        txtCardCode.Text = Document.CardCode;
                        txtCardName.Text = Document.CardName;
                        txtMoneda.Text = Document.DocCurrency;
                        cbMoneda.SelectedValue = Document.DocCurrency;
                        cbSeries.SelectedValue = Document.Series;

                        if (DocumentType == (int)Documento.CreditMemo)
                        {
                            cbIndicador.SelectedValue = Document.Indicator;
                            cbMetPago.SelectedValue = Document.U_MPAGO2; 
                            cbMotivo.SelectedValue = Document.U_Motivo;
                        }                       
                       
                        txtTC.Text = Document.DocRate.ToString("N2");
                        VatGroup = Document.VatGroup;
                        txtTaxCode.Text = VatGroup;
                        cbMoneda.Enabled = false;
                        if (Document.DocStatus == "P")
                        {
                            txtStatus.Text = "Pendiente";
                            btnCrear.Enabled = true;
                            btnCopiarDe.Enabled = false;
                            btnCrear.Text = "Autorizar";
                        }
                        else if (Document.DocStatus == "R")
                        {
                            txtStatus.Text = "Rechazado";
                            btnCrear.Enabled = false;
                        }
                        else if (Document.DocStatus == "A")
                        {
                            txtStatus.Text = "Autorizado";
                            btnCrear.Enabled = true;
                            btnCrear.Text = "Crear";
                        }

                        if (Document.DocNumSAP != 0)
                        {
                            btnCrear.Enabled = false;
                            
                        }
                        if (DocumentType == (int)Documento.Devolucion & !sType.Equals("Borrador"))
                        {
                            btnCrear.Enabled = true;
                            btnCrear.Text = "Actualizar";
                        }

                        dtpDocDate.Value = Document.DocDate;
                        dtpDocDueDate.Value = Document.DocDueDate;
                        txtComments.Text = Document.Comments;
                        txtSubtotal.Text = (Document.DocTotal - Document.VatSum).ToString("N2");
                        txtIVA.Text = Document.VatSum.ToString("N2");
                        txtTotal.Text = Document.DocTotal.ToString("N2");
                        #endregion

                        #region Detalle
                        foreach (var item in Document.Lines)
                        {
                            DataRow row = (dgvDatos.DataSource as DataTable).NewRow();
                            row["ItemCode"] = item.ItemCode;
                            row["ItemName"] = item.ItemDescription;
                            row["Quantity"] = item.Quantity;
                            row["Price"] = item.Price;
                            row["Currency"] = item.Currency;
                            row["WhsCode"] = item.WarehouseCode;
                            row["WhsName"] = item.WarehouseCode + " | " + item.WarehouseName;
                            row["LineNum"] = item.LineNum;
                            row["Rate"] = item.Rate;
                            row["LineTotal"] = item.LineTotal;
                            row["LineStatus"] = item.LineStatus;
                            row["U_Comentario"] = item.U_Comentario;

                            row["BaseLine"] = item.BaseLine;
                            row["BaseType"] = item.BaseType;
                            row["BaseEntry"] = item.BaseEntry;
                           
                            row["TaxCode"] = item.TaxCode;
                            row["ManBtchNum"] = item.ManBtchNum;

                            if (DocumentType == (int)Documento.CreditMemo)
                            {
                                row["AccountCode"] = item.AccountCode;
                            }

                            (dgvDatos.DataSource as DataTable).Rows.Add(row);

                            (dgvDatos.DataSource as DataTable).AcceptChanges();
                        }
                        #endregion
                        #endregion
                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message);
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                this.SetMensaje("Error al cargar NC: " + ex.Message, 5000, Color.Red, Color.White);
            }

        }

        #region Eventos de botones toolStrip
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            this.CleanScreen();
            txtCardCode.Focus();
            DocumentMode = "Nuevo";
            tbl_Lotes.Clear();
            this.Formulario_Modo();
        }

        //pendiente
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (this.btnCrear.Text == "Crear")
            {
                if (MessageBox.Show("Los datos no guardados se perderán. ¿Desea continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                    return;
            }
            this.CleanScreen();
            this.DocumentMode = "Consulta";
            this.Formulario_Modo();
        }

        //pendiente
        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (!sType.Equals("RC") & !sType.Equals("DV"))
            {
                this.SetMensaje("Imposible crear versión impresa.", 5000, Color.Red, Color.White);
                return;
            }

            Constantes.Clases.LoadRPT rpt = new Constantes.Clases.LoadRPT(IdDocument);
            Constantes.frmVisor form = new Constantes.frmVisor(rpt.DocRPT);
            rpt.GenerarPDF(Document.DocEntry.ToString());
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        //pendiente
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            /*
            if (MessageBox.Show("¿Desea Cerrar esta orden de compra ?\r\n Numero de documento" + txtFolio.Text, "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == System.Windows.Forms.DialogResult.Yes)
            {
                SDK.SDKDatos.SDK_DI sdk = new SDKDatos.SDK_DI(1);//--Orden de compra
                //--sdk.CerrarOrdenCompra(this, Document);
                this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
            }*/
        }
        #endregion

        private void dgvDatos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                if (e.Row.Cells["LineStatus"].Value.ToString() == "C")
                {
                    e.Row.Appearance.BackColor = Color.LightGray;

                    e.Row.Cells["ItemCode"].Activation = Activation.NoEdit;
                    e.Row.Cells["ItemName"].Activation = Activation.NoEdit;
                    e.Row.Cells["WhsName"].Activation = Activation.NoEdit;
                }
                //else
                //    if (!Convert.ToDecimal(e.Row.Cells["Quantity"].Value).Equals(
                //       Convert.ToDecimal(e.Row.Cells["OpenQty"].Value)))
                //    {
                //        e.Row.Appearance.ForeColor = Color.Red;

                //        e.Row.Cells["ItemCode"].Activation = Activation.NoEdit;
                //        e.Row.Cells["ItemName"].Activation = Activation.NoEdit;
                //        e.Row.Cells["WhsName"].Activation = Activation.NoEdit;
                //    }

                if (config.ModeDocument != "Edit")
                    if (e.Row.Cells["Currency"].Value.ToString() == txtMoneda.Text)
                        e.Row.Cells["Rate"].Value = 1;
                    else
                        e.Row.Cells["Rate"].Value = config.Rate;
            }
            catch (Exception)
            {
            }
        }

        private void txtCardCode_Leave(object sender, EventArgs e)
        {
            if (DocumentMode.Equals("Nuevo"))
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
                        txtTaxCode.Text = VatGroup;
                        config.VatGroup = VatGroup;

                        cbMoneda.Enabled = sn.Rows[0].Field<string>("Currency").Equals("##");
                        cbMoneda_SelectionChangeCommitted(sender, e);

                        DocumentMode = "Registrar";

                        this.Formulario_Modo();
                    }
                }
        }

        private void dgvDatos_SummaryValueChanged(object sender, SummaryValueChangedEventArgs e)
        {
            try
            {
                //if (DocumentMode.Equals( "Registrar"))
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

                    try
                    {
                        decimal d_aplicado = Convert.ToDecimal(txtImporteAplicado.Text);
                        decimal d_total = Convert.ToDecimal(txtTotal.Text);
                        txtSaldoPendiente.Text = (d_total - d_aplicado).ToString("N2");
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnCopiarDe_Click(object sender, EventArgs e)
        {
            if (DocumentMode.Equals("Registrar"))
            {
                #region Nota de credito
                if (DocumentType == (int)Documento.CreditMemo)
                {
                    SDK.Documentos.frmListadoDocumentos formulario =
                                new frmListadoDocumentos(-6, txtCardCode.Text,
                                    string.Empty, dtpDocDate.Value, string.Empty);

                    if (formulario.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                    {
                        DocumentMode = "Registrar";
                        this.Formulario_Modo();
                        foreach (var item in formulario.Seleccionados)
                        {
                            txtComments.Text = formulario.dgvDatos.ActiveRow.Cells["Comments"].Value.ToString();
                            NumAtCard = formulario.dgvDatos.ActiveRow.Cells["DocNum"].Value.ToString();
                            txtImporteAplicado.Text = Convert.ToDecimal(formulario.dgvDatos.ActiveRow.Cells["PaidToDate"].Value).ToString("N2");

                            using (SqlConnection cnnection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                            {
                                SqlCommand cmd = new SqlCommand("sp_SDKDataSource", cnnection);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add(new SqlParameter("@TipoConsulta", 11));
                                cmd.Parameters.Add(new SqlParameter("@Key", item.DocEntry));
                                cmd.Parameters.Add(new SqlParameter("@ObjType", item.DocType));

                                DataTable line_tbl = new DataTable();
                                SqlDataAdapter da = new SqlDataAdapter();
                                da.SelectCommand = cmd;
                                da.Fill(line_tbl);

                                config.FillDetails(line_tbl);
                            }
                        }
                    }
              
                    else
                    {
                        return;
                    }
                }
                #endregion

                #region Devolucion
                if (DocumentType == (int)Documento.Devolucion)
                {
                    SDK.Documentos.frmListadoDocumentos formulario =
                                new frmListadoDocumentos(-8, txtCardCode.Text,
                                    string.Empty, dtpDocDate.Value, string.Empty);

                    if (formulario.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                    {
                        DocumentMode = "Registrar";
                        this.Formulario_Modo();
                        foreach (var item in formulario.Seleccionados)
                        {
                            txtComments.Text = formulario.dgvDatos.ActiveRow.Cells["Comments"].Value.ToString();
                            NumAtCard = formulario.dgvDatos.ActiveRow.Cells["DocNum"].Value.ToString();

                            using (SqlConnection cnnection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                            {
                                SqlCommand cmd = new SqlCommand("sp_SDKDataSource", cnnection);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add(new SqlParameter("@TipoConsulta", 11));
                                cmd.Parameters.Add(new SqlParameter("@Key", item.DocEntry));
                                cmd.Parameters.Add(new SqlParameter("@ObjType", item.DocType));

                                DataTable line_tbl = new DataTable();
                                SqlDataAdapter da = new SqlDataAdapter();
                                da.SelectCommand = cmd;
                                da.Fill(line_tbl);

                                config.FillDetails(line_tbl);
                            }
                        }
                    }

                    else
                    {
                        return;
                    }
                }
                #endregion
                ///////////////////////////////////////////
            }
            else { this.SetMensaje("Proveedor no existe!!!", 5000, Color.Red, Color.White); }
        }

        private void dgvDatos_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            if (DocumentMode.Equals("Consulta"))
            {
                e.Cancel = true;
                return;
            } 
            
            //if (!deleteRow)
            //{
            //    e.Cancel = true;
            //    e.DisplayPromptMsg = false;
            //}
            //deleteRow = false;

        }

        private void udpCuentas_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Header.Caption = "Cuenta";
            e.Layout.Bands[0].Columns[1].Header.Caption = "Nombre";
            e.Layout.Bands[0].Columns[1].Width = 250;
            e.Layout.Bands[0].Columns[0].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[1].CellActivation = Activation.NoEdit;
        }

        private void toolTransacciones_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tbl = (dgvDatos.DataSource as DataTable);

                Int32 qryDocEntry = (from item in tbl.AsEnumerable()
                           select item.Field<Int32>("BaseEntry")).FirstOrDefault();

                Varios.frmTransaccionesFR form = new Varios.frmTransaccionesFR(qryDocEntry);
                form.ShowDialog();
            }
            catch (Exception)
            {

            }
            
        }
    }
}