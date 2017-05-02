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
    public partial class frmEM : Constantes.frmEmpty
    {
        int IdDocument;
        public int ObjType;
        Decimal IVA;
        public string VatGroup;
        public string VatGroupName;
        SDK.Configuracion.SDK_Configuracion_EM config;
        SDK_SAP.Clases.Document Document;
        DataTable tbl_Lotes = new DataTable();
        string DocumentMode;/*Nuevo|Registrar|Creado|Consulta*/
        int GroupCode;

        int idConexion = 10008;

        public frmEM(int _idDocument)
        {
            InitializeComponent();

            dgvDatos.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
            //  I  N  I  C  I  A  L  I  Z  A  R
            IdDocument = _idDocument;
            config = new Configuracion.SDK_Configuracion_EM(IdDocument, this);
            config.ModeDocument = "New";
            DocumentMode = "Nuevo";
            config.StartEmpty();
            IVA = config.IVA1;
            this.AccessibleDescription = "SDK " + this.Text;
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
                        buscarStripButton.Enabled = true;
                        guardarToolStripButton.Enabled = false;
                        exportarToolStripButton.Enabled = false;
                        actualizarToolStripButton.Enabled = false;
                        tooStripDuplicar.Enabled = false;
                        btnCrear.Enabled = true;
                        btnCopiarDe.Enabled = true;
                        txtFolio.ReadOnly = true;

                        txtCardCode.BackColor = Color.White;
                        txtCardName.BackColor = Color.White;
                        txtFolio.BackColor = Color.White;

                        txtMoneda.ReadOnly = true;
                        txtTC.ReadOnly = false;

                        txtCardCode.Focus();
                    } break;
                case "Registrar":
                    {
                        buscarStripButton.Enabled = true;
                        guardarToolStripButton.Enabled = false;
                        exportarToolStripButton.Enabled = false;
                        actualizarToolStripButton.Enabled = false;
                        tooStripDuplicar.Enabled = false;
                        btnCrear.Enabled = true;
                        btnCopiarDe.Enabled = true;
                        txtFolio.ReadOnly = true;

                        txtCardCode.BackColor = Color.White;
                        txtCardName.BackColor = Color.White;
                        txtFolio.BackColor = Color.White;

                        txtMoneda.ReadOnly = true;
                        txtTC.ReadOnly = false;
                    } break;
                case "Creado":
                    {
                        //this.CleanScreen();
                        buscarStripButton.Enabled = true;
                        guardarToolStripButton.Enabled = false;
                        exportarToolStripButton.Enabled = false;
                        actualizarToolStripButton.Enabled = false;
                        tooStripDuplicar.Enabled = false;
                        btnCrear.Enabled = false;
                        btnCopiarDe.Enabled = false;
                        txtFolio.ReadOnly = true;

                        txtCardCode.BackColor = Color.White;
                        txtCardName.BackColor = Color.White;
                        txtFolio.BackColor = Color.White;

                        txtMoneda.ReadOnly = true;
                        txtTC.ReadOnly = true;
                    } break;
                case "Consulta":
                    {
                        this.CleanScreen();

                        buscarStripButton.Enabled = true;
                        guardarToolStripButton.Enabled = false;
                        exportarToolStripButton.Enabled = false;
                        actualizarToolStripButton.Enabled = false;
                        tooStripDuplicar.Enabled = false;
                        btnCrear.Enabled = false;
                        txtFolio.ReadOnly = false;
                        btnCopiarDe.Enabled = false;

                        txtCardCode.BackColor = Color.FromName("Info");
                        txtCardName.BackColor = Color.FromName("Info");
                        txtFolio.BackColor = Color.FromName("Info");

                        txtMoneda.ReadOnly = true;
                        txtTC.ReadOnly = true;

                        txtFolio.Focus();
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

        private DataTable Lotes(string ItemCode, string WhsCode, double Quantity, int BaseType, int BaseEntry, int BaseLineNum)
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM [dbo].[fn_GetBaseBatchNum] (@ItemCode, @WhsCode, @Quantity, @BaseType, @BaseEntry, @BaseLineNum)", connection))
                {
                    command.Parameters.AddWithValue("@ItemCode", ItemCode);
                    command.Parameters.AddWithValue("@WhsCode", WhsCode);
                    command.Parameters.AddWithValue("@Quantity", Quantity);
                    command.Parameters.AddWithValue("@BaseType", BaseType);
                    command.Parameters.AddWithValue("@BaseEntry", BaseEntry);
                    command.Parameters.AddWithValue("@BaseLineNum", BaseLineNum);

                    DataTable table = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(table);

                    return table;
                }
            }
        }

        private void frmOrdenCompra_Load(object sender, EventArgs e)
        {
            try
            {
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
                if (string.IsNullOrEmpty(txtMoneda.Text))
                {
                    this.SetMensaje("Campo MONEDA obligatorio", 5000, Color.Red, Color.White);
                    return;
                }
                if (txtMoneda.Text.Equals("USD") & string.IsNullOrEmpty(txtTC.Text))
                {
                    this.SetMensaje("Campo TIPO DE CAMBIO obligatorio", 5000, Color.Red, Color.White);
                    return;
                }
                #region Crear
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    if (ObjType == 20)
                    {
                        #region Entradas de mercancia
                        SDK_SAP.Clases.Document document = new SDK_SAP.Clases.Document();
                        document.DocType = "dDocument_Items";
                        document.Comments = txtComments.Text;
                        document.CardCode = txtCardCode.Text;
                        document.DocDate = dtpDocDate.Value;
                        document.Series = 13;
                        document.DocCurrency = txtMoneda.Text;
                        document.DocRate = txtMoneda.Text == "USD" ? Convert.ToDouble(txtTC.Text) : (double)1;
                        document.DiscPrcnt = Convert.ToDouble(txtDescuentoP.Text == string.Empty ? "0" : txtDescuentoP.Text);

                        foreach (var item in dgvDatos.Rows)
                        {
                            SDK_SAP.Clases.DocumentLines line = new SDK_SAP.Clases.DocumentLines();

                            line.ItemCode = Convert.ToString(item.Cells["ItemCode"].Value);
                            line.ItemDescription = Convert.ToString(item.Cells["ItemName"].Value);
                            line.Quantity = Convert.ToDouble(item.Cells["Quantity"].Value);
                            line.Facturado = Convert.ToDouble(item.Cells["FacturadoQty"].Value);
                            line.Recibido = Convert.ToDouble(item.Cells["OpenQty"].Value);
                            line.Price = Convert.ToDouble(item.Cells["Price"].Value);
                            line.Currency = Convert.ToString(item.Cells["Currency"].Value);
                            line.WarehouseCode = Convert.ToString(item.Cells["WhsCode"].Value);
                            line.U_Comentario = Convert.ToString(item.Cells["U_Comentario"].Value);
                            line.ManBtchNum = Convert.ToString(item.Cells["ManBtchNum"].Value);
                            line.TaxCode = Convert.ToString(item.Cells["TaxCode"].Value);
                            line.NumPerMsr =  Convert.ToInt32(item.Cells["NumPerMsr"].Value);
                            line.Rate = Convert.ToDouble(item.Cells["Rate"].Value); //line.Currency == "USD" ? Convert.ToDouble(item.Cells["Rate"].Value) : (double)1;//aqui

                            line.BaseEntry = (int?)(item.Cells["BaseEntry"].Value != DBNull.Value ? item.Cells["BaseEntry"].Value : null);
                            line.BaseLine = (int?)(item.Cells["BaseLine"].Value != DBNull.Value ? item.Cells["BaseLine"].Value : null);
                            line.BaseType = (int?)(item.Cells["BaseType"].Value != DBNull.Value ? item.Cells["BaseType"].Value : null);

                            document.Lines.Add(line);
                        }

                        #region Lotes
                        bool viewLotes = false;
                        bool finalizar = true;
                        bool LotesOk = true;

                        viewLotes = false;
                        finalizar = true;

                        foreach (var item in document.Lines)
                        {
                            if (item.ManBtchNum.Equals("Y"))
                            {
                                if (item.Facturado * item.NumPerMsr != item.LotesList.Sum(lote_item => lote_item.Cantidad))
                                {
                                    viewLotes = true;
                                }

                                if (GroupCode != 113 & GroupCode != 114)
                                {
                                    SDK_SAP.Clases.Lotes objLote = new SDK_SAP.Clases.Lotes();
                                    #region Obtener ultimo Lote
                                    DataTable tbl = objLote.getUltimoLote(item.ItemCode, item.WarehouseCode);
                                    #endregion
                                    if (tbl.Rows.Count > 0)
                                    {
                                        objLote.Cantidad = item.Facturado * item.NumPerMsr;
                                        objLote.Aduana = tbl.Rows[0].Field<string>("IntrSerial");
                                        objLote.FechaAdmision = tbl.Rows[0].Field<DateTime>("InDate");
                                        objLote.Lote = tbl.Rows[0].Field<string>("BatchNum");
                                        objLote.Pedimento = tbl.Rows[0].Field<string>("SuppSerial");
                                        item.LotesList.Add(objLote);

                                    }
                                }

                                foreach (var lote in item.LotesList)
                                {
                                    if (!string.IsNullOrEmpty(lote.Pedimento))
                                        viewLotes = true;
                                    if (!string.IsNullOrEmpty(lote.Aduana))
                                        viewLotes = true;
                                }
                            }
                        }

                        if (viewLotes)
                        {
                            frmLotes form = new frmLotes(document, tbl_Lotes);
                            if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                                return;
                            else
                                tbl_Lotes = form.Tbl_lotes;
                        }

                        foreach (var item in document.Lines)
                        {
                            if (item.ManBtchNum.Equals("Y"))
                            {
                                if (item.Facturado * item.NumPerMsr != item.LotesList.Sum(lote_item => lote_item.Cantidad))
                                {
                                    finalizar = false;
                                }
                            }
                        }

                        foreach (var item in document.Lines)
                        {
                            if (item.ManBtchNum.Equals("Y"))
                            {
                                foreach (var lote in item.LotesList)
                                {
                                    if (string.IsNullOrEmpty(lote.Pedimento))
                                        LotesOk = false;
                                    if (string.IsNullOrEmpty(lote.Aduana))
                                        LotesOk = false;
                                }
                            }
                        }

                        if (!LotesOk)
                        {
                            this.SetMensaje("Falta ingresar informacion Aduana|Pedimento", 5000, Color.Red, Color.White);
                            return;
                        }

                        #endregion
                        if (finalizar)
                        {
                            SDK_SAP.DI.Documents doc = new SDK_SAP.DI.Documents(idConexion);//8
                            SDK_SAP.DI.Connection.InitializeConnection(idConexion);//8
                            SDK_SAP.DI.Connection.StartConnection();

                            txtFolio.Text = doc.AddDocument("OPDN", document).ToString();

                            SDK_SAP.DI.Connection.CloseConnection();

                            this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
                            DocumentMode = "Creado";
                        }

                        #endregion
                    }
                    else if (ObjType == 21)
                    {
                        #region Devoluciones de mercancia
                        SDK_SAP.Clases.Document document = new SDK_SAP.Clases.Document();
                        document.DocType = "dDocument_Items";
                        document.Comments = txtComments.Text;
                        document.CardCode = txtCardCode.Text;
                        document.DocDate = dtpDocDate.Value;
                        document.Series = 14;
                        document.DocCurrency = txtMoneda.Text;
                        document.DocRate = txtMoneda.Text == "USD" ? Convert.ToDouble(txtTC.Text) : (double)1;

                        foreach (var item in dgvDatos.Rows)
                        {
                            SDK_SAP.Clases.DocumentLines line = new SDK_SAP.Clases.DocumentLines();

                            line.ItemCode = Convert.ToString(item.Cells["ItemCode"].Value);
                            line.ItemDescription = Convert.ToString(item.Cells["ItemName"].Value);
                            line.Quantity = Convert.ToDouble(item.Cells["Quantity"].Value);
                            line.Currency = Convert.ToString(item.Cells["Currency"].Value);
                            line.WarehouseCode = Convert.ToString(item.Cells["WhsCode"].Value);
                            line.Price = Convert.ToDouble(item.Cells["Price"].Value);
                            line.U_Comentario = Convert.ToString(item.Cells["U_Comentario"].Value);
                            line.ManBtchNum = Convert.ToString(item.Cells["ManBtchNum"].Value);

                            line.BaseEntry = (int?)(item.Cells["BaseEntry"].Value != DBNull.Value ? item.Cells["BaseEntry"].Value : null);
                            line.BaseLine = (int?)(item.Cells["BaseLine"].Value != DBNull.Value ? item.Cells["BaseLine"].Value : null);
                            line.BaseType = (int?)(item.Cells["BaseType"].Value != DBNull.Value ? item.Cells["BaseType"].Value : null);

                            #region Lotes
                            if (line.ManBtchNum.Equals("Y"))
                            {
                                DataTable tbl_Lot = this.Lotes(line.ItemCode, line.WarehouseCode, line.Quantity, (int)line.BaseType, (int)line.BaseEntry, (int)line.BaseLine);
                                foreach (DataRow itemLote in tbl_Lot.Rows)
                                {
                                    SDK_SAP.Clases.Lotes oLot = new SDK_SAP.Clases.Lotes();
                                    oLot.Lote = itemLote.Field<string>("BatchNum");
                                    oLot.Cantidad = (double)itemLote.Field<decimal>("Quantity");
                                    line.LotesList.Add(oLot);
                                }
                            }
                            #endregion

                            document.Lines.Add(line);
                        }

                        SDK_SAP.DI.Documents doc = new SDK_SAP.DI.Documents(10009);
                        SDK_SAP.DI.Connection.InitializeConnection(10009);
                        SDK_SAP.DI.Connection.StartConnection();

                        txtFolio.Text = doc.AddDocument("ORPD", document).ToString();

                        SDK_SAP.DI.Connection.CloseConnection();

                        this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
                        DocumentMode = "Creado";

                        #endregion
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
                this.Formulario_Modo();

            }

            if (btnCrear.Text == "Actualizar")
            {

            }

        }

        private void cbMoneda_SelectionChangeCommitted(object sender, EventArgs e)
        {
            txtMoneda.Text = cbMoneda.SelectedValue.ToString();
            if (txtMoneda.Text.Equals("USD"))
                txtTC.Text = config.Rate.ToString("N4");
            else
                txtTC.Text = config.Rate.ToString("N4");
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

            try
            {
                //if (txtMoneda.Text == "$")aqui
                //    (dgvDatos.DataSource as DataTable).Columns["LineTotal"].Expression = "Price*FacturadoQty*Rate";
                //if (txtMoneda.Text == "USD")
                //    (dgvDatos.DataSource as DataTable).Columns["LineTotal"].Expression = "Price*FacturadoQty/Rate";

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
                switch (DocumentMode)
                {
                    #region Nuevo
                    case "Nuevo":
                        {
                            if ((int)e.KeyChar == (int)Keys.Enter)
                            {
                                if (string.IsNullOrEmpty(txtFolio.Text))
                                {
                                    SDK.Documentos.frmListadoDocumentos formulario =
                                        new frmListadoDocumentos(-1, txtCardCode.Text.Replace('*', '%'),
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
                            }
                        } break;
                    #endregion

                    case "Consulta":
                        {
                            if ((int)e.KeyChar == (int)Keys.Enter)
                            {
                                if (string.IsNullOrEmpty(txtFolio.Text))// se proporciona el DocNum completo
                                {
                                    SDK.Documentos.frmListadoDocumentos formulario =
                                        new frmListadoDocumentos(IdDocument, txtCardCode.Text.Replace('*', '%'),
                                            txtCardName.Text.Replace('*', '%'), dtpDocDate.Value, string.Empty);

                                    if (formulario.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                                        txtFolio.Text = formulario.DocEntry.ToString();
                                    else
                                    {
                                        return;
                                    }

                                }

                                Document = new SDK_SAP.Clases.Document();
                                Document = Document.Fill(Convert.ToDecimal(txtFolio.Text), ObjType);
                                #region Documento Encontrado
                                (dgvDatos.DataSource as DataTable).Rows.Clear();

                                #region Llenar Form
                                this.CleanScreen();
                                DocumentMode = "Consulta";
                                Formulario_Modo();

                                txtFolio.Text = Document.DocNum.ToString();
                                txtCardCode.Text = Document.CardCode;
                                txtCardName.Text = Document.CardName;
                                txtMoneda.Text = Document.DocCurrency;
                                cbMoneda.SelectedValue = Document.DocCurrency;
                                txtTC.Text = Document.DocRate.ToString("N2");
                                // cbMoneda.Enabled = false;
                                if (Document.DocStatus == "C")
                                {
                                    txtStatus.Text = "Cerrado";
                                    //btnCrear.Enabled = false;
                                    //cerrarStripButton.Enabled = false;

                                    cerrarLíneaToolStripMenuItem.Enabled = false;
                                    agregarFilaToolStripMenuItem1.Enabled = false;
                                    tooStripDuplicar.Enabled = false;
                                }
                                else if (Document.DocStatus == "O")
                                {
                                    txtStatus.Text = "Abierto";
                                    //btnCrear.Enabled = true;
                                    //cerrarStripButton.Enabled = true;
                                    //cerrarLíneaToolStripMenuItem.Enabled = true;
                                    //agregarFilaToolStripMenuItem1.Enabled = true;
                                    //tooStripDuplicar.Enabled = true;
                                }

                                dtpDocDate.Value = Document.DocDate;
                                dtpDocDueDate.Value = Document.DocDueDate;
                                txtComments.Text = Document.Comments;
                                txtSubtotal.Text = (Document.DocTotal - Document.VatSum).ToString("N2");
                                txtIVA.Text = Document.VatSum.ToString("N2");
                                txtTotal.Text = Document.DocTotal.ToString("N2");

                                #region Detalle
                                foreach (var item in Document.Lines)
                                {
                                    DataRow row = (dgvDatos.DataSource as DataTable).NewRow();
                                    row["ItemCode"] = item.ItemCode;
                                    row["ItemName"] = item.ItemDescription;
                                    if (ObjType == 20)
                                    {
                                        row["Quantity"] = item.Quantity;//solicitado
                                        row["OpenQty"] = item.Recibido;//recibido
                                        row["FacturadoQty"] = item.Facturado;//facturado (EM quantity)
                                    }
                                    else if (ObjType == 21)
                                    {
                                        row["Quantity"] = item.Quantity;//
                                        row["OpenQty"] = item.OpenQty;//
                                    }

                                    row["Price"] = item.Price;
                                    row["Currency"] = item.Currency;
                                    row["WhsCode"] = item.WarehouseCode;
                                    row["WhsName"] = item.WarehouseCode + " | " + item.WarehouseName;
                                    row["LineNum"] = item.LineNum;
                                    row["Rate"] = item.Rate;//aqui
                                    row["LineTotal"] = item.LineTotal;
                                    row["LineStatus"] = item.LineStatus;
                                    row["U_Comentario"] = item.U_Comentario;
                                    row["VolumenU"] = item.VolumenU;
                                    row["PesoU"] = item.PesoU;

                                    (dgvDatos.DataSource as DataTable).Rows.Add(row);

                                    (dgvDatos.DataSource as DataTable).AcceptChanges();
                                }
                                #endregion
                                #endregion
                                #endregion
                            }
                        } break;




                    /*   


                    #region LLenar Form

                   

                    

                    
                    #endregion*/

                }
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
            txtCardCode.Focus();
            DocumentMode = "Nuevo";
            this.Formulario_Modo();
            tbl_Lotes.Clear();
            GroupCode = 0;
            Document = new SDK_SAP.Clases.Document();
        }

        //pendiente
        private void btnBuscar_Click(object sender, EventArgs e)
        {/*
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

            cerrarLíneaToolStripMenuItem.Enabled = true;*/
            DocumentMode = "Consulta";
            this.Formulario_Modo();
        }

        //pendiente
        private void btnImprimir_Click(object sender, EventArgs e)
        {/*
            Constantes.Clases.LoadRPT rpt = new Constantes.Clases.LoadRPT(IdDocument);
            Constantes.frmVisor form = new Constantes.frmVisor(rpt.DocRPT);
            rpt.GenerarPDF(Document.DocEntry.ToString());
            form.MdiParent = this.MdiParent;
            form.Show();
            */
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
                //if (e.Row.Cells["LineStatus"].Value.ToString() == "C")
                //{
                //    e.Row.Appearance.BackColor = Color.LightGray;

                //    e.Row.Cells["ItemCode"].Activation = Activation.NoEdit;
                //    e.Row.Cells["ItemName"].Activation = Activation.NoEdit;
                //    e.Row.Cells["WhsName"].Activation = Activation.NoEdit;
                //}
                //else
                //    if (!Convert.ToDecimal(e.Row.Cells["Quantity"].Value).Equals(
                //       Convert.ToDecimal(e.Row.Cells["OpenQty"].Value)))
                //    {
                //        e.Row.Appearance.ForeColor = Color.Red;

                //        e.Row.Cells["ItemCode"].Activation = Activation.NoEdit;
                //        e.Row.Cells["ItemName"].Activation = Activation.NoEdit;
                //        e.Row.Cells["WhsName"].Activation = Activation.NoEdit;
                //    }

                //if (config.ModeDocument != "Edit")
                //    if (e.Row.Cells["Currency"].Value.ToString() == txtMoneda.Text)
                //        e.Row.Cells["Rate"].Value = 1;
                //    else
                //        e.Row.Cells["Rate"].Value = config.Rate;
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
                        txtMoneda.Text = sn.Rows[0].Field<string>("Currency") == "##" ? string.Empty : sn.Rows[0].Field<string>("Currency");
                        cbMoneda.SelectedValue = sn.Rows[0].Field<string>("Currency") == "##" ? "USD" : sn.Rows[0].Field<string>("Currency");
                        VatGroup = sn.Rows[0].Field<string>("VatGroup");
                        VatGroupName = sn.Rows[0].Field<string>("VatGroupN");
                        GroupCode = sn.Rows[0].Field<Int16>("GroupCode");
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
                {
                    decimal total = Convert.ToDecimal(e.SummaryValue.Value);
                    decimal desc = Convert.ToDecimal(txtDescuentoM.Text.Trim() != string.Empty ? txtDescuentoM.Text : "0");

                    txtSubtotal.Text = total.ToString("N2");

                    total = total - desc;

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

        private void btnCopiarDe_Click(object sender, EventArgs e)
        {
            if (DocumentMode.Equals("Registrar"))
            {
                int tipoDocumento = 0;
                if (ObjType == 20) tipoDocumento = -2;
                else if (ObjType == 21) tipoDocumento = -3;

                SDK.Documentos.frmListadoDocumentos formulario =
                            new frmListadoDocumentos(
                                tipoDocumento, txtCardCode.Text,
                                string.Empty, dtpDocDate.Value, string.Empty);

                if (formulario.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    DocumentMode = "Registrar";
                    this.Formulario_Modo();
                    #region Salidas de mercancias
                    if (tipoDocumento == -3)
                    {
                        foreach (var item in formulario.DocKey)
                        {
                            using (SqlConnection cnnection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                            {
                                SqlCommand cmd = new SqlCommand("sp_SDKDataSource", cnnection);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add(new SqlParameter("@TipoConsulta", 11));
                                cmd.Parameters.Add(new SqlParameter("@ObjType", ObjType));
                                cmd.Parameters.Add(new SqlParameter("@Key", item));

                                DataTable line_tbl = new DataTable();
                                SqlDataAdapter da = new SqlDataAdapter();
                                da.SelectCommand = cmd;
                                da.Fill(line_tbl);

                                config.FillDetails(line_tbl);
                            }
                        }
                    }
                    #endregion
                    #region Entradas de mercancias
                    else if (tipoDocumento == -2)
                    {
                        foreach (var item in formulario.Seleccionados)
                        {
                            using (SqlConnection cnnection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                            {
                                SqlCommand cmd = new SqlCommand("sp_SDKDataSource", cnnection);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add(new SqlParameter("@TipoConsulta", 11));
                                cmd.Parameters.Add(new SqlParameter("@ObjType", item.DocType));
                                cmd.Parameters.Add(new SqlParameter("@Key", item.DocEntry));

                                DataTable line_tbl = new DataTable();
                                SqlDataAdapter da = new SqlDataAdapter();
                                da.SelectCommand = cmd;
                                da.Fill(line_tbl);

                                config.FillDetails(line_tbl);

                                if (line_tbl.Rows.Count > 0)
                                    if (line_tbl.Rows[0]["Comments"] != DBNull.Value)
                                        txtComments.Text = line_tbl.Rows[0]["Comments"].ToString();
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    return;
                }
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

        private void dtpDocDate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbMoneda.SelectedValue.ToString().Equals("USD"))
                    using (SqlConnection cnnection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        SqlCommand cmd = new SqlCommand("sp_SDKDataSource", cnnection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@TipoConsulta", 16));
                        cmd.Parameters.Add(new SqlParameter("@Key", dtpDocDate.Value));

                        cnnection.Open();

                        txtTC.Text = cmd.ExecuteScalar().ToString();
                    }
            }
            catch (Exception)
            {
                this.SetMensaje("No existe definición para TC", 5000, Color.Red, Color.White);
                txtTC.Text = string.Empty;
            }
        }

        private void frmEM_Shown(object sender, EventArgs e)
        {
            if (!ClasesSGUV.Forms.GetPermisoReporte(this.Name))
            {
                MessageBox.Show("Usuario no autorizado. \r\nContacta al administrador.", "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }
        }

        private void txtDescuento_Leave(object sender, EventArgs e)
        {
            try
            {
                decimal total;
                txtDescuentoP.Text = (Convert.ToDecimal(txtDescuentoM.Text) / (Convert.ToDecimal(txtSubtotal.Text)) * 100).ToString("N4");
                total = Convert.ToDecimal(txtSubtotal.Text) - Convert.ToDecimal(txtDescuentoM.Text);
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
            catch (Exception)
            {

            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            try
            {
                decimal total;
                txtDescuentoM.Text = (Convert.ToDecimal(txtSubtotal.Text) * (Convert.ToDecimal(txtDescuentoP.Text)) / 100).ToString("N2");
                total = Convert.ToDecimal(txtSubtotal.Text) - Convert.ToDecimal(txtDescuentoM.Text);
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
            catch (Exception)
            {
            }
        }

        private void txtTC_TextChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataRow item in (dgvDatos.DataSource as DataTable).Rows)
                {
                    if (item["Currency"].ToString().Equals("USD"))
                        item["Rate"] = txtTC.Text == string.Empty ? "1" : txtTC.Text;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}