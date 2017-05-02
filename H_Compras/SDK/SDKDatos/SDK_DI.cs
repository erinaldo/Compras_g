using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace H_Compras.SDK.SDKDatos
{
    public class SDK_DI
    {
        SAPbobsCOM.Company oCompany;
        public SDK_DI(int TypeDocument)
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_SDKDocuments", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 1);
                    command.Parameters.AddWithValue("@idDocumento", TypeDocument);

                    DataTable tbl = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl);

                    this.oCompany = new SAPbobsCOM.Company();
                    this.oCompany.language = SAPbobsCOM.BoSuppLangs.ln_Spanish_La;

                    this.oCompany.Server = tbl.Rows[0].Field<string>("Server");
                    this.oCompany.CompanyDB = tbl.Rows[0].Field<string>("CompanyDB");
                    //this.oCompany.CompanyDB = "SBO-DistPJ_Test";
                    this.oCompany.DbUserName = tbl.Rows[0].Field<string>("DbUserName");
                    this.oCompany.DbPassword = tbl.Rows[0].Field<string>("DbPassword");
                    this.oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012;
                    this.oCompany.LicenseServer = tbl.Rows[0].Field<string>("LicenseServer");

                    this.oCompany.UserName = tbl.Rows[0].Field<string>("UserName");
                    this.oCompany.Password = tbl.Rows[0].Field<string>("Password");
                }
            }

            if (this.oCompany.Connect() != 0)
            {
                Int32 iError = 0;
                string sErrror = string.Empty;

                iError = this.oCompany.GetLastErrorCode();
                sErrror = this.oCompany.GetLastErrorDescription();

                String sMsg = "Error al conectar SAP " + iError + " : " + sErrror;

                throw new Exception(sMsg);
            }

            oCompany.ProgressIndicator -= new SAPbobsCOM._ICompanyEvents_ProgressIndicatorEventHandler(company_ProgressIndicator);
            oCompany.ProgressIndicator += new SAPbobsCOM._ICompanyEvents_ProgressIndicatorEventHandler(company_ProgressIndicator);
        }

        private DataTable Lotes(string ItemCode, string WhsCode, double Quantity)
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM [dbo].[fn_LotesPrueba] (@ItemCode, @WhsCode, @Quantity)", connection))
                {
                    command.Parameters.AddWithValue("@ItemCode", ItemCode);
                    command.Parameters.AddWithValue("@WhsCode", WhsCode);
                    command.Parameters.AddWithValue("@Quantity", Quantity);

                    DataTable table = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(table);

                    return table;
                }
            }
        }

        #region Documentos
        //ok
        public decimal ActualizarDocumento(SDK.Documentos.frmDocumentos form, SDK_SAP.Clases.Document documentConCambios)
        {
            //SAPbobsCOM.Documents oDocumentCopy = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
            SAPbobsCOM.Documents oDocumento = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
            oDocumento.GetByKey(Convert.ToInt32(documentConCambios.DocEntry));
            //oDocumentCopy = oDocumento;
            oDocumento.DocDueDate = form.dtpDocDueDate.Value;

            #region Encabezado
            if (!string.IsNullOrEmpty(form.txtNumAtCard.Text))
                oDocumento.NumAtCard = form.txtNumAtCard.Text;
            if (!string.IsNullOrEmpty(form.txtComments.Text))
                oDocumento.Comments = form.txtComments.Text;
            #endregion

            #region Detalle
            foreach (UltraGridRow row in form.dgvDatos.Rows)
            {
                if (!row.IsAddRow)
                {
                    //si la linea ya existe la linea solo actiaizar
                    if (Convert.ToInt32(row.Cells["LineNum"].Value == DBNull.Value ? -1 : row.Cells["LineNum"].Value) > -1)
                    {
                        oDocumento.Lines.SetCurrentLine(Convert.ToInt32(row.Index));

                        oDocumento.Lines.Quantity = Convert.ToDouble(row.Cells["Quantity"].Value);
                        if (row.Cells["ShipDate"].Value != DBNull.Value)
                            oDocumento.Lines.ShipDate = Convert.ToDateTime(row.Cells["ShipDate"].Value);

                        if (row.Cells["U_Comentario"].Value != DBNull.Value)
                            oDocumento.Lines.UserFields.Fields.Item("U_Comentario").Value = row.Cells["U_Comentario"].Value;
                        else
                            oDocumento.Lines.UserFields.Fields.Item("U_Comentario").Value = String.Empty;

                        if (row.Cells["U_Vendedor"].Value != DBNull.Value)
                            oDocumento.Lines.UserFields.Fields.Item("U_Vendedor").Value = row.Cells["U_Vendedor"].Value;
                        else
                            oDocumento.Lines.UserFields.Fields.Item("U_Vendedor").Value = String.Empty;

                        if (row.Cells["WhsCode"].Value != DBNull.Value)
                            oDocumento.Lines.UserFields.Fields.Item("U_Warehouse").Value = Convert.ToString(row.Cells["WhsCode"].Value);
                        if (row.Cells["ShipDate"].Value != DBNull.Value)
                            oDocumento.Lines.UserFields.Fields.Item("U_ShipDate").Value = Convert.ToDateTime(row.Cells["ShipDate"].Value);
                        else
                            oDocumento.Lines.UserFields.Fields.Item("U_ShipDate").Value = oDocumento.DocDueDate;
                    }
                    else
                    {
                        oDocumento.Lines.Add();
                        oDocumento.Lines.ItemCode = Convert.ToString(row.Cells["ItemCode"].Value);
                        oDocumento.Lines.Quantity = Convert.ToDouble(row.Cells["Quantity"].Value);
                        oDocumento.Lines.Currency = Convert.ToString(row.Cells["Currency"].Value);
                        oDocumento.Lines.UnitPrice = Convert.ToDouble(row.Cells["Price"].Value);
                        oDocumento.Lines.WarehouseCode = Convert.ToString(row.Cells["WhsCode"].Value);

                        if (row.Cells["ShipDate"].Value != DBNull.Value)
                            oDocumento.Lines.ShipDate = Convert.ToDateTime(row.Cells["ShipDate"].Value);

                        if (row.Cells["U_Comentario"].Value != DBNull.Value)
                            oDocumento.Lines.UserFields.Fields.Item("U_Comentario").Value = row.Cells["U_Comentario"].Value;
                        else
                            oDocumento.Lines.UserFields.Fields.Item("U_Comentario").Value = String.Empty;

                        if (row.Cells["U_Vendedor"].Value != DBNull.Value)
                            oDocumento.Lines.UserFields.Fields.Item("U_Vendedor").Value = row.Cells["U_Vendedor"].Value;
                        else
                            oDocumento.Lines.UserFields.Fields.Item("U_Vendedor").Value = String.Empty;

                        if (row.Cells["WhsCode"].Value != DBNull.Value)
                            oDocumento.Lines.UserFields.Fields.Item("U_Warehouse").Value = Convert.ToString(row.Cells["WhsCode"].Value);
                        if (row.Cells["ShipDate"].Value != DBNull.Value)
                            oDocumento.Lines.UserFields.Fields.Item("U_ShipDate").Value = Convert.ToDateTime(row.Cells["ShipDate"].Value);
                        else
                            oDocumento.Lines.UserFields.Fields.Item("U_ShipDate").Value = oDocumento.DocDueDate;
                    }
                }
            }
            #endregion

            if (oDocumento.Update() != 0)
                throw new Exception("Error [" + oCompany.GetLastErrorDescription() + "]");
            else
            {
                #region Datos SAP
                form.txtFolio.Text = oCompany.GetNewObjectKey();
                oDocumento.GetByKey(Convert.ToInt32(form.txtFolio.Text));

                form.txtFolio.Text = oDocumento.DocNum.ToString();
                if (SAPbobsCOM.BoStatus.bost_Close == oDocumento.DocumentStatus)
                    form.txtStatus.Text = "Cerrado";
                else if (SAPbobsCOM.BoStatus.bost_Open == oDocumento.DocumentStatus)
                    form.txtStatus.Text = "Abierto";

                //form.btnCrear.Enabled = false;

                #endregion
            }
            this.CloseConnection();

            return oDocumento.DocNum;
        }
        #endregion

        #region Ordenes de compra
        //ok
        public decimal CrearOrdenCompra(SDK.Documentos.frmDocumentos form)
        {
            SAPbobsCOM.Documents oDocument = (SAPbobsCOM.Documents) oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);

            #region Encabezado
            oDocument.CardCode = form.txtCardCode.Text;
            oDocument.Series = 15;
            oDocument.DocDate = form.dtpDocDate.Value;
            oDocument.DocDueDate = form.dtpDocDueDate.Value;
            oDocument.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items;
            oDocument.DocCurrency = form.txtMoneda.Text;

            if (!string.IsNullOrEmpty(form.txtNumAtCard.Text))
                oDocument.NumAtCard = form.txtNumAtCard.Text;
            if (!string.IsNullOrEmpty(form.txtComments.Text))
                oDocument.Comments = form.txtComments.Text;
            #endregion

            #region Detalle
            foreach (UltraGridRow row in form.dgvDatos.Rows)
            {
                if (!row.IsAddRow)
                {
                    oDocument.Lines.ItemCode = Convert.ToString(row.Cells["ItemCode"].Value);
                    oDocument.Lines.Quantity = Convert.ToDouble(row.Cells["Quantity"].Value);
                    oDocument.Lines.Currency = Convert.ToString(row.Cells["Currency"].Value);
                    //oDocument.Lines.UnitPrice = Convert.ToDouble(row.Cells["Price"].Value);
                    //No se envia precio porque se toma directamente de la lista de precios 17, esto para evitar 
                    //problemas con los articulos con multiplo de compra
                    oDocument.Lines.WarehouseCode = Convert.ToString(row.Cells["WhsCode"].Value);
                    if (row.Cells["ShipDate"].Value != DBNull.Value)
                        oDocument.Lines.ShipDate = Convert.ToDateTime(row.Cells["ShipDate"].Value);

                    oDocument.Lines.UserFields.Fields.Item("U_Comentario").Value = Convert.ToString(row.Cells["U_Comentario"].Value);
                    oDocument.Lines.UserFields.Fields.Item("U_Vendedor").Value = Convert.ToString(row.Cells["U_Vendedor"].Value);

                    oDocument.Lines.UserFields.Fields.Item("U_Warehouse").Value = Convert.ToString(row.Cells["WhsCode"].Value);
                    if (row.Cells["ShipDate"].Value != DBNull.Value)
                        oDocument.Lines.UserFields.Fields.Item("U_ShipDate").Value = Convert.ToDateTime(row.Cells["ShipDate"].Value);
                    else
                        oDocument.Lines.UserFields.Fields.Item("U_ShipDate").Value = oDocument.DocDueDate;

                    oDocument.Lines.Add();
                }
            }
            #endregion
            if (string.IsNullOrEmpty(form.txtStatus.Text))
            {
                //oDocument.DocType = 22;
                //SDK_SAP.DI.Documents doc = new SDK_SAP.DI.Documents(10006);
                //form.txtFolio.Text = oDocument.SaveDraft(oDocument).ToString();

                //Document = Document.Fill(Document.DocNum, 16, "Borrador");
                //txtFolio.Text = Document.DocNum.ToString();

                //if (Document.DocStatus.Equals("P"))
                //    txtStatus.Text = "Pendiente";
                //else if (Document.DocStatus.Equals("R"))
                //    txtStatus.Text = "Rechazado";
                //else if (Document.DocStatus.Equals("A"))
                //    txtStatus.Text = "Autorizado";

                //var kea = new KeyPressEventArgs(Convert.ToChar(13));
                //this.txtFolio_KeyPress(sender, kea);

                //btnCrear_Click(sender, e);

                //this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
                //DocumentMode = "Creado";
            }

            // if (oDocument.DocumentStatus.Equals("A"))
            if (oDocument.Add() != 0)
                throw new Exception("Error [" + oCompany.GetLastErrorDescription() + "]");
            else
            {
                #region Datos SAP
                form.txtFolio.Text = oCompany.GetNewObjectKey();
                oDocument.GetByKey(Convert.ToInt32(form.txtFolio.Text));

                form.txtFolio.Text = oDocument.DocNum.ToString();
                if (SAPbobsCOM.BoStatus.bost_Close == oDocument.DocumentStatus)
                    form.txtStatus.Text = "Cerrado";
                else if (SAPbobsCOM.BoStatus.bost_Open == oDocument.DocumentStatus)
                    form.txtStatus.Text = "Abierto";

                form.btnCrear.Enabled = false;

                #endregion
            }

            this.CloseConnection();
            return oDocument.DocNum;
        }

        public decimal ActualizarOrdenCompra(SDK.Documentos.frmDocumentos form, SDK_OPOR document)
        {
            SAPbobsCOM.Documents oDocument = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
            oDocument.GetByKey(Convert.ToInt32(document.DocEntry));

            #region Encabezado
            if (!string.IsNullOrEmpty(form.txtNumAtCard.Text))
                oDocument.NumAtCard = form.txtNumAtCard.Text;
            if (!string.IsNullOrEmpty(form.txtComments.Text))
                oDocument.Comments = form.txtComments.Text;

            oDocument.DocDueDate = form.dtpDocDueDate.Value;

            #endregion

            #region Detalle
            foreach (UltraGridRow row in form.dgvDatos.Rows)
            {
                if (!row.IsAddRow)
                {
                    //si la linea ya existe la linea solo actiaizar
                    if (Convert.ToInt32(row.Cells["LineNum"].Value) > -1)
                    {
                        oDocument.Lines.SetCurrentLine(Convert.ToInt32(row.Cells["LineNum"].Value));

                        oDocument.Lines.Quantity = Convert.ToDouble(row.Cells["Quantity"].Value);
                        if (row.Cells["ShipDate"].Value != DBNull.Value)
                            oDocument.Lines.ShipDate = Convert.ToDateTime(row.Cells["ShipDate"].Value);

                        if (row.Cells["U_Comentario"].Value != DBNull.Value)
                            oDocument.Lines.UserFields.Fields.Item("U_Comentario").Value = row.Cells["U_Comentario"].Value;

                        if (row.Cells["U_Vendedor"].Value != DBNull.Value)
                            oDocument.Lines.UserFields.Fields.Item("U_Vendedor").Value = row.Cells["U_Vendedor"].Value;
                    }
                    else
                    {
                        oDocument.Lines.Add();
                        oDocument.Lines.ItemCode = Convert.ToString(row.Cells["ItemCode"].Value);
                        oDocument.Lines.Quantity = Convert.ToDouble(row.Cells["Quantity"].Value);
                        oDocument.Lines.Currency = Convert.ToString(row.Cells["Currency"].Value);
                        oDocument.Lines.UnitPrice = Convert.ToDouble(row.Cells["Price"].Value);
                        oDocument.Lines.WarehouseCode = Convert.ToString(row.Cells["WhsCode"].Value);
                        if (row.Cells["ShipDate"].Value != DBNull.Value)
                            oDocument.Lines.ShipDate = Convert.ToDateTime(row.Cells["ShipDate"].Value);

                        oDocument.Lines.UserFields.Fields.Item("U_Comentario").Value = Convert.ToString(row.Cells["U_Comentario"].Value);
                        oDocument.Lines.UserFields.Fields.Item("U_Vendedor").Value = Convert.ToString(row.Cells["U_Vendedor"].Value);


                        //if (oDocument.Update() != 0)
                        //    throw new Exception("Error [" + oCompany.GetLastErrorDescription() + "]");
                    }
                }
            }
            #endregion


            if (oDocument.Update() != 0)
                throw new Exception("Error [" + oCompany.GetLastErrorDescription() + "]");
            else
            {
                #region Datos SAP
                form.txtFolio.Text = oCompany.GetNewObjectKey();
                oDocument.GetByKey(Convert.ToInt32(form.txtFolio.Text));

                form.txtFolio.Text = oDocument.DocNum.ToString();
                if (SAPbobsCOM.BoStatus.bost_Close == oDocument.DocumentStatus)
                    form.txtStatus.Text = "Cerrado";
                else if (SAPbobsCOM.BoStatus.bost_Open == oDocument.DocumentStatus)
                    form.txtStatus.Text = "Abierto";

                //form.btnCrear.Enabled = false;

                #endregion
            }
            this.CloseConnection();

            return oDocument.DocNum;
        }
        //ok
        public decimal CerrarOrdenCompra(SDK.Documentos.frmDocumentos form, SDK_SAP.Clases.Document document)
        {
            SAPbobsCOM.Documents oDocument = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
            oDocument.GetByKey(Convert.ToInt32(document.DocEntry));

            #region Detalle

            for (int i = 0; i < oDocument.Lines.Count; i++)
            {
                oDocument.Lines.SetCurrentLine(i);
                if (oDocument.Lines.LineStatus != SAPbobsCOM.BoStatus.bost_Close)
                    oDocument.Lines.LineStatus = SAPbobsCOM.BoStatus.bost_Close;
            }

            #endregion

            if (oDocument.Update() != 0)
                throw new Exception("Error [" + oCompany.GetLastErrorDescription() + "]");
            else
            {
                #region Datos SAP
                form.txtFolio.Text = oCompany.GetNewObjectKey();
                oDocument.GetByKey(Convert.ToInt32(form.txtFolio.Text));

                form.txtFolio.Text = oDocument.DocNum.ToString();
                if (SAPbobsCOM.BoStatus.bost_Close == oDocument.DocumentStatus)
                    form.txtStatus.Text = "Cerrado";
                else if (SAPbobsCOM.BoStatus.bost_Open == oDocument.DocumentStatus)
                    form.txtStatus.Text = "Abierto";

                #endregion
            }
            this.CloseConnection();

            return oDocument.DocNum;
        }
        //ok
        public decimal CerrarLineaOrdenCompra(SDK.Documentos.frmDocumentos form, SDK_SAP.Clases.Document document, int Line)
        {

            SAPbobsCOM.Documents oDocument = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
            oDocument.GetByKey(Convert.ToInt32(document.DocEntry));

            #region Detalle
            oDocument.Lines.SetCurrentLine(Line);
            if (oDocument.Lines.LineStatus != SAPbobsCOM.BoStatus.bost_Close)
                oDocument.Lines.LineStatus = SAPbobsCOM.BoStatus.bost_Close;

            #endregion

            if (oDocument.Update() != 0)
                throw new Exception("Error [" + oCompany.GetLastErrorDescription() + "]");
            else
            {
                #region Datos SAP
                form.txtFolio.Text = oCompany.GetNewObjectKey();
                oDocument.GetByKey(Convert.ToInt32(form.txtFolio.Text));

                form.txtFolio.Text = oDocument.DocNum.ToString();
                if (SAPbobsCOM.BoStatus.bost_Close == oDocument.DocumentStatus)
                    form.txtStatus.Text = "Cerrado";
                else if (SAPbobsCOM.BoStatus.bost_Open == oDocument.DocumentStatus)
                    form.txtStatus.Text = "Abierto";

                #endregion
            }
            this.CloseConnection();

            return oDocument.DocNum;
        }
        //ok
        public SAPbobsCOM.Documents EliminarLineaOrdenCompra(SDK.Documentos.frmDocumentos form, SDK_SAP.Clases.Document document, int Line)
        {
            SAPbobsCOM.Documents oDocument = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
            SAPbobsCOM.Documents oDocumentCopy = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
            oDocument.GetByKey(Convert.ToInt32(document.DocEntry));
            oDocumentCopy = oDocument;

            #region Eliminar Linea
            oDocument.Lines.SetCurrentLine(Line);
            if (oDocument.Lines.LineStatus != SAPbobsCOM.BoStatus.bost_Close)
                oDocument.Lines.Delete();


            if (oDocument.Update() != 0)
                throw new Exception("Error [" + oCompany.GetLastErrorDescription() + "]");
            #endregion

            else
            {
                #region Datos SAP
                form.txtFolio.Text = oCompany.GetNewObjectKey();
                oDocumentCopy.GetByKey(Convert.ToInt32(form.txtFolio.Text));

                form.txtFolio.Text = oDocumentCopy.DocNum.ToString();
                if (SAPbobsCOM.BoStatus.bost_Close == oDocumentCopy.DocumentStatus)
                    form.txtStatus.Text = "Cerrado";
                else if (SAPbobsCOM.BoStatus.bost_Open == oDocumentCopy.DocumentStatus)
                    form.txtStatus.Text = "Abierto";

                //form.btnCrear.Enabled = false;

                #endregion
            }
            this.CloseConnection();

            return oDocument;
        }
        #endregion

        #region Salida de Mercancias
        public decimal CrearSalidaDeMercancias(SDK.Documentos.frmSalidas form)
        {
            SAPbobsCOM.Documents oDocument = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);

            #region Encabezado
            oDocument.DocDate = form.dtpDocDate.Value;
            oDocument.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items;

            if (!string.IsNullOrEmpty(form.txtComentarios.Text))
                oDocument.Comments = form.txtComentarios.Text;
            #endregion

            #region Detalle
            foreach (UltraGridRow row in form.dgvDatos.Rows)
            {
                if (!row.IsAddRow)
                {
                    oDocument.Lines.ItemCode = Convert.ToString(row.Cells["ItemCode"].Value);
                    oDocument.Lines.Quantity = Convert.ToDouble(row.Cells["Quantity"].Value);
                    oDocument.Lines.WarehouseCode = Convert.ToString(row.Cells["WhsCode"].Value);
                    oDocument.Lines.AccountCode = Convert.ToString(row.Cells["AcctCode"].Value);
                    oDocument.Lines.CostingCode3 = Convert.ToString(row.Cells["OcrCode3"].Value);

                    if (Convert.ToString(row.Cells["ManBtchNum"].Value).Equals("Y"))
                    {
                        foreach (DataRow item in this.Lotes(oDocument.Lines.ItemCode, oDocument.Lines.WarehouseCode, oDocument.Lines.Quantity).Rows)
                        {
                            oDocument.Lines.BatchNumbers.BatchNumber = item.Field<string>("BatchNum");
                            oDocument.Lines.BatchNumbers.Quantity = Convert.ToDouble(item["Quantity"]);
                            oDocument.Lines.BatchNumbers.Add();
                        }
                    }

                    oDocument.Lines.Add();
                }
            }
            #endregion

            if (oDocument.Add() != 0)
                throw new Exception("Error [" + oCompany.GetLastErrorDescription() + "]");
            else
            {
                #region Datos SAP
                form.txtNumero.Text = oCompany.GetNewObjectKey();
                oDocument.GetByKey(Convert.ToInt32(form.txtNumero.Text));

                form.txtNumero.Text = oDocument.DocNum.ToString();
                //if (SAPbobsCOM.BoStatus.bost_Close == oDocument.DocumentStatus)
                //    form.txtStatus.Text = "Cerrado";
                //else if (SAPbobsCOM.BoStatus.bost_Open == oDocument.DocumentStatus)
                //    form.txtStatus.Text = "Abierto";

                form.btnCrear.Enabled = false;

                #endregion
            }

            return oDocument.DocNum;
        }
        #endregion

        #region Solicitud de traslado
        public decimal CrearTransfereciaStock(SDK.SDKDatos.SDK_OWTR document)
        {
            //SAPbobsCOM.StockTransfer oDocument = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);
            SAPbobsCOM.StockTransfer oDocument = (SAPbobsCOM.StockTransfer)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest);

            #region Encabezado
            oDocument.DocDate = document.DocDate;
            oDocument.FromWarehouse = document.Filler;
            oDocument.ToWarehouse = document.ToWhsCode;
            oDocument.UserFields.Fields.Item("U_FolioHalcon").Value = document.DocNum.ToString();

            // oDocument.Lines.FromWarehouseCode = "01"
            //oDocument.Lines.WarehouseCode = "02"
            #endregion

            #region Detalle
            foreach (SDK_WTR1 row in document.Lines)
            {
                oDocument.Lines.ItemCode = row.ItemCode;
                oDocument.Lines.WarehouseCode = row.WhsCode;
                oDocument.Lines.FromWarehouseCode = row.FromWhsCode;
                oDocument.Lines.Quantity = Convert.ToDouble(row.Quantity);

                if (row.ManBtchNum.Equals("Y"))
                {
                    foreach (DataRow lote in this.Lotes(oDocument.Lines.ItemCode, oDocument.Lines.WarehouseCode, oDocument.Lines.Quantity).Rows)
                    {
                        oDocument.Lines.BatchNumbers.BatchNumber = lote["BatchNum"].ToString();
                        oDocument.Lines.BatchNumbers.Quantity = Convert.ToDouble(lote["Quantity"]);
                        oDocument.Lines.BatchNumbers.Add();
                    }
                }

                oDocument.Lines.UserFields.Fields.Item("U_Tarima").Value = row.U_Tarima;
                oDocument.Lines.UserFields.Fields.Item("U_TipoAlm").Value = row.U_TipoAlm;

                oDocument.Lines.Add();
            }
            #endregion

            if (oDocument.Add() != 0)
                throw new Exception("Error [" + oCompany.GetLastErrorDescription() + " | Linea: " + (oDocument.Lines.LineNum + 1) + "]");
            else
            {
                #region Datos SAP
                oDocument.GetByKey(Convert.ToInt32(oCompany.GetNewObjectKey()));
                #endregion
            }

            return oDocument.DocNum;
        }
        #endregion

        #region Trasnferencia de stock
        public decimal CrearSolicitudTraslado(SDK.SDKDatos.SDK_OWTR document)
        {
            SAPbobsCOM.StockTransfer oDocument_New = (SAPbobsCOM.StockTransfer)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);

            #region Encabezado
            oDocument_New.DocDate = document.DocDate;
            oDocument_New.FromWarehouse = document.Filler;
            oDocument_New.ToWarehouse = document.ToWhsCode;
            oDocument_New.UserFields.Fields.Item("U_FolioHalcon").Value = document.DocNum.ToString();
            oDocument_New.UserFields.Fields.Item("U_TipoSolicitud").Value = document.U_TipoSolicitud;
            oDocument_New.UserFields.Fields.Item("U_FolioSolicitud").Value = document.U_FolioSolicitud;
            ////oDocument_New.Series = 23;

            // oDocument.Lines.FromWarehouseCode = "01"
            //oDocument.Lines.WarehouseCode = "02"
            #endregion

            #region Detalle
            foreach (SDK_WTR1 row in document.Lines)
            {
                //oDocument.Lines.b
                oDocument_New.Lines.ItemCode = row.ItemCode;
                oDocument_New.Lines.WarehouseCode = row.WhsCode;
                oDocument_New.Lines.FromWarehouseCode = row.FromWhsCode;
                oDocument_New.Lines.Quantity = Convert.ToDouble(row.Quantity);

                if (row.ManBtchNum.Equals("Y"))
                {
                    foreach (DataRow lote in this.Lotes(oDocument_New.Lines.ItemCode, oDocument_New.Lines.FromWarehouseCode, oDocument_New.Lines.Quantity).Rows)
                    {
                        oDocument_New.Lines.BatchNumbers.BatchNumber = lote["BatchNum"].ToString();
                        oDocument_New.Lines.BatchNumbers.Quantity = Convert.ToDouble(lote["Quantity"]);
                        oDocument_New.Lines.BatchNumbers.Add();
                    }
                }

                oDocument_New.Lines.UserFields.Fields.Item("U_Tarima").Value = row.U_Tarima;
                oDocument_New.Lines.UserFields.Fields.Item("U_TipoAlm").Value = row.U_TipoAlm;

                oDocument_New.Lines.Add();
            }
            #endregion

            if (oDocument_New.Add() != 0)
                throw new Exception("Error [" + oCompany.GetLastErrorDescription() + " | Linea: " + (oDocument_New.Lines.LineNum + 1) + "]");
            else
            {
                #region Datos SAP
                oDocument_New.GetByKey(Convert.ToInt32(oCompany.GetNewObjectKey()));
                #endregion
            }

            return oDocument_New.DocNum;
        }
        #endregion

        public void company_ProgressIndicator(int MaxValue, int CurrentValue)
        {
            string error = MaxValue.ToString() + " - " + CurrentValue;
        }

        public void CloseConnection()
        {
            this.oCompany.Disconnect();

        }
    }
}
