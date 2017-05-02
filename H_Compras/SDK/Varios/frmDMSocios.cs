using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.SDK.Varios
{
    public partial class frmDMSocios : Constantes.frmEmpty
    {
        Object[] valuesOut = new Object[] { };
        Datos.Connection connection = new Datos.Connection();
        SAPbobsCOM.BusinessPartners oBP;
        SAPbobsCOM.BusinessPartners oBBPP;
        int idConnection = 6;

        string QryGroup60 = "N";
        string QryGroup61 = "N";
        string QryGroup62 = "N";
        string QryGroup22 = "N";
        string QryGroup23 = "N";

        public frmDMSocios()
        {
            InitializeComponent();

            SDK_SAP.DI.Connection.InitializeConnection(idConnection);
            SDK_SAP.DI.Connection.StartConnection();

            oBP = (SAPbobsCOM.BusinessPartners) SDK_SAP.DI.Connection.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
        }

        private void ModoFormulario(string _tipo)
        {
            switch (_tipo)
            {
                case "Buscar":
                    {
                        txtCardCode.ReadOnly = false;
                        txtCardCode.BackColor = Color.FromName("Info");
                        txtCardName.BackColor = Color.FromName("Info");
                        txtCardFName.BackColor = Color.FromName("Info");
                        txtVatIdUnCmp.BackColor = Color.FromName("Info");
                        btnCrear.Text = "Actualizar";
                        btnAgregarDir.Visible = false;
                    }
                    break;
                case "Lectura":
                    {
                        txtCardCode.ReadOnly = true;
                        txtCardCode.BackColor = Color.FromName("Control");
                        txtCardName.BackColor = Color.White;
                        txtCardFName.BackColor = Color.White;
                        txtVatIdUnCmp.BackColor = Color.White;
                        btnCrear.Text = "Actualizar";
                        btnAgregarDir.Visible = true;
                        btnAgregarDir.Text = "Actualizar dirección";
                        btnAgregarContact.Text = "Actualizar persona";
                    }
                    break;
                case "Nuevo":
                    {
                        txtCardCode.ReadOnly = false;
                        txtCardCode.BackColor = Color.White;
                        txtCardName.BackColor = Color.White;
                        txtCardFName.BackColor = Color.White;
                        txtVatIdUnCmp.BackColor = Color.White;
                        btnCrear.Text = "Crear";
                        btnAgregarDir.Visible = true;
                        btnAgregarDir.Text = "Agregar dirección";
                        btnAgregarContact.Text = "Agregar persona";
                    }
                    break;
            }
        }

        private void frmDMSocios_Load(object sender, EventArgs e)
        {
            try
            {
                this.ModoFormulario("Buscar");
                buscarStripButton.Click -= new EventHandler(btnBuscar_Click);
                buscarStripButton.Click += new EventHandler(btnBuscar_Click);
                nuevoToolStripButton.Click -= new EventHandler(btnNuevo_Click);
                nuevoToolStripButton.Click += new EventHandler(btnNuevo_Click);

                nuevoToolStripButton.Enabled = true;
                buscarStripButton.Enabled = true;
                exportarToolStripButton.Enabled = false;
                guardarToolStripButton.Enabled = false;

                ClasesSGUV.Form.ControlsForms.setDataSource(cbGroupCode, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GrupoSN, "C", string.Empty), "GroupName", "GroupCode", "--Grupo--");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbSlpCode, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListaVendedores, string.Empty, string.Empty), "Nombre", "Codigo", "--Vendedor--");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbIndustryC, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListaRamosCanales, string.Empty, string.Empty), "IndName", "IndCode", "--Ramo--");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbTaxCode, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListaIndicadorImpuestos, string.Empty, string.Empty), "Code", "Code", "--Indicador--");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbGroupNum, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListaCondicionesPago, string.Empty, string.Empty), "PymntGroup", "GroupNum", string.Empty);
                ClasesSGUV.Form.ControlsForms.setDataSource(cbU_MPAGO2, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListaMetodosPago, string.Empty, string.Empty), "Descr", "FldValue", "--Método pago--");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbU_TipoVisita, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListaTipoVisita, string.Empty, string.Empty), "Descr", "FldValue", "--Tipo visita--");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbListNum, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListasPrecios, "TODO", string.Empty), "ListName", "ListNum", "--Lista Precios--");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbCmpPrivate, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListaTipoSocio, "TODO", string.Empty), "Name", "Code", string.Empty);
                ClasesSGUV.Form.ControlsForms.setDataSource(cbCurrency, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListaMonedas, string.Empty, string.Empty), "Name", "Code", string.Empty);

                ClasesSGUV.Form.ControlsForms.setDataSource(cbCountry, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListaCountry, string.Empty, string.Empty), "Name", "Code", "--Pais--");
                cbCountry.SelectedValue = "MX";
                cbCountry_SelectionChangeCommitted(sender, e);
                cbTaxCode.SelectedValue = "IVAPA16";
                cbListNum.SelectedValue = 13;
                cbCmpPrivate.SelectedValue = "C";

                DataTable tblFiscal =
                                    connection.GetDataTable("LOG",
                                                            "sp_SDKDataSource",
                                                            new string[] { },
                                                            new string[] { "@TipoConsulta", "@Key", "@sType" },
                                                            ref valuesOut, 20, String.Empty, "B");
                dgvFiscal.DataSource = tblFiscal;

                DataTable tblEntrega =
                            connection.GetDataTable("LOG",
                                                    "sp_SDKDataSource",
                                                    new string[] { },
                                                    new string[] { "@TipoConsulta", "@Key", "@sType" },
                                                    ref valuesOut, 20, String.Empty, "S");
                dgvEntrega.DataSource = tblEntrega;

                DataTable tblContacto =
                           connection.GetDataTable("LOG",
                                                   "sp_SDKDataSource",
                                                   new string[] { },
                                                   new string[] { "@TipoConsulta", "@Key" },
                                                   ref valuesOut, 24, String.Empty);
                dgvContactos.DataSource = tblContacto;

                DataTable tblPropiedades =
                                    connection.GetDataTable("LOG",
                                                            "sp_SDKDataSource",
                                                            new string[] { },
                                                            new string[] { "@TipoConsulta" },
                                                            ref valuesOut, 23);
                tblPropiedades.Columns["Valor"].ReadOnly = false;
                dgvPropiedades.DataSource = tblPropiedades;

                dgvFiscal.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.FixedAddRowOnBottom;
                dgvEntrega.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.FixedAddRowOnBottom;

                this.ModoFormulario("Nuevo");
                rbActivo.Checked = true;
                cbTipoDireccion.SelectedIndex = 0;
                cbCurrency.SelectedValue = "$";
                txtCardCode.Focus();

                if (ClasesSGUV.Login.Rol != (int)ClasesSGUV.Propiedades.RolesHalcoNET.Administrador)
                    btnXML.Visible = false;
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void LimpiarTxt(Control control)
        {
            foreach (Control item in control.Controls)
            {
                if (item is TextBox)
                    ((TextBox)item).Clear();

                LimpiarTxt(item);
            }
        }

        private bool ExisteDireccion(string AddressID, SAPbobsCOM.BoAddressType AddressType)
        {
            if (AddressType == SAPbobsCOM.BoAddressType.bo_BillTo)
            {
                foreach (var item in dgvFiscal.Rows)
                {
                    if (!item.IsDeleted)
                        if (item.Cells[0].Value.ToString().Equals(AddressID))
                            return true;
                }
            }
            if (AddressType == SAPbobsCOM.BoAddressType.bo_ShipTo)
            {
                foreach (var item in dgvEntrega.Rows)
                {
                    if (!item.IsDeleted)
                        if (item.Cells[0].Value.ToString().Equals(AddressID))
                            return true;
                }
            }
            return false;
        }

        private bool ValidarSN()
        {
            DataTable tblValidar =
                                   connection.GetDataTable("LOG",
                                                           "sp_SDKValidaciones",
                                                           new string[] { },
                                                           new string[] { "@TipoConsulta"},
                                                           ref valuesOut, 3);
            if (!tblValidar.Rows[0].Field<bool>("result"))
                return true;

            if (String.IsNullOrWhiteSpace(oBBPP.UserFields.Fields.Item("U_TipoVisita").Value.ToString()))
            { this.SetMensaje("ELIJE UN TIPO DE VISITA", 5000, Color.Red, Color.White); return false; }
            ///////////////////
            if ((oBBPP.PriceListNum != 1 & oBBPP.PriceListNum != 2 & oBBPP.PriceListNum != 3 & oBBPP.PriceListNum != 7 & oBBPP.PriceListNum != 8 & oBBPP.PriceListNum != 9) & QryGroup60.Equals("Y"))
            { this.SetMensaje("ELIJE UNA LISTA DE PRECIO DE 'TRANSPORTE'", 5000, Color.Red, Color.White); return false; }

            if ((oBBPP.PriceListNum != 4 & oBBPP.PriceListNum != 5 & oBBPP.PriceListNum != 6 & oBBPP.PriceListNum != 10 & oBBPP.PriceListNum != 11 & oBBPP.PriceListNum != 12) & QryGroup61.Equals("Y"))
            { this.SetMensaje("ELIJE UNA LISTA DE PRECIO DE 'MAYOREO'", 5000, Color.Red, Color.White); return false; }

            if ((oBBPP.PriceListNum != 4 & oBBPP.PriceListNum != 5 & oBBPP.PriceListNum != 6 & oBBPP.PriceListNum != 10 & oBBPP.PriceListNum != 11 & oBBPP.PriceListNum != 12) & QryGroup62.Equals("Y"))
            { this.SetMensaje("ELIJE UNA LISTA DE PRECIO DE 'MAYOREO'", 5000, Color.Red, Color.White); return false; }
            /*
            if ((oBBPP.GroupCode == 100 | oBBPP.GroupCode == 107 | oBBPP.GroupCode == 108 | oBBPP.GroupCode == 102)
                & (oBBPP.PriceListNum != 1 & oBBPP.PriceListNum != 2 & oBBPP.PriceListNum != 3) & QryGroup60.Equals("Y"))
            { this.SetMensaje("ELIJE UNA LISTA DE PRECIO DE 'REGIÓN 1 TRANSPORTE', SI TIENES DUDA CONTACTA A LA GERENCIA COMERCIAL"); return false; }

            if ((oBBPP.GroupCode == 100 | oBBPP.GroupCode == 107 | oBBPP.GroupCode == 108 | oBBPP.GroupCode == 102)
                & (oBBPP.PriceListNum != 4 & oBBPP.PriceListNum != 5 & oBBPP.PriceListNum != 6) & QryGroup61.Equals("Y"))
            { this.SetMensaje("ELIJE UNA LISTA DE PRECIO DE 'REGIÓN 1 MAYOREO', SI TIENES DUDA CONTACTA A LA GERENCIA COMERCIAL"); return false; }

            if ((oBBPP.GroupCode == 106)
                & (oBBPP.PriceListNum != 7 & oBBPP.PriceListNum != 8 & oBBPP.PriceListNum != 9))
            { this.SetMensaje("ELIJE UNA LISTA DE PRECIO DE 'REGIÓN 2 TRANSPORTE', SI TIENES DUDA CONTACTA A LA GERENCIA COMERCIAL"); return false; }

            if ((oBBPP.GroupCode == 105)
                 & (oBBPP.PriceListNum != 10 & oBBPP.PriceListNum != 11 & oBBPP.PriceListNum != 12))
            { this.SetMensaje("ELIJE UNA LISTA DE PRECIO DE 'REGIÓN 2 MAYOREO', SI TIENES DUDA CONTACTA A LA GERENCIA COMERCIAL"); return false; }

            if ((oBBPP.GroupCode == 103 | oBBPP.GroupCode == 104 | oBBPP.GroupCode == 121)
               & (oBBPP.PriceListNum != 7 & oBBPP.PriceListNum != 8 & oBBPP.PriceListNum != 9) & QryGroup60.Equals("Y"))
            { this.SetMensaje("ELIJE UNA LISTA DE PRECIO DE 'REGIÓN 2 TRANSPORTE', SI TIENES DUDA CONTACTA A LA GERENCIA COMERCIAL"); return false; }

            if ((oBBPP.GroupCode == 103 | oBBPP.GroupCode == 104 | oBBPP.GroupCode == 121)
               & (oBBPP.PriceListNum != 10 & oBBPP.PriceListNum != 11 & oBBPP.PriceListNum != 12) & QryGroup61.Equals("Y"))
            { this.SetMensaje("ELIJE UNA LISTA DE PRECIO DE 'REGIÓN 2 MAYOREO', SI TIENES DUDA CONTACTA A LA GERENCIA COMERCIAL"); return false; }
            */
            ////////////////////////
            if (oBBPP.SalesPersonCode == -1)
            { this.SetMensaje("ELIJE UN VENDEDOR PARA EL CLIENTE", 5000, Color.Red, Color.White); return false; }

            if (oBBPP.Industry == 0 | oBBPP.Industry == null)
            { this.SetMensaje("ELIJE UN RAMO PARA EL CLIENTE", 5000, Color.Red, Color.White); return false; }

            if (oBBPP.Industry != 18 & oBBPP.Industry != 19 & oBBPP.Industry != 20 & oBBPP.Industry != 22 & oBBPP.Industry != 23)
            { this.SetMensaje("EL RAMO QUE ELEJISTE ES INCORRECTO, ELIJE UNO DE LA LISTA", 5000, Color.Red, Color.White); return false; }

            if (String.IsNullOrWhiteSpace(oBBPP.UserFields.Fields.Item("U_Correo").Value.ToString()))
            { this.SetMensaje("COLOCA UN CORREO ELECTRÓNICO PARA EL CLIENTE", 5000, Color.Red, Color.White); return false; }

            if (QryGroup60.Equals("N") & QryGroup61.Equals("N") & QryGroup62.Equals("N"))
            { this.SetMensaje("ELIJE UNA PROPIEDAD DE GRAN CANAL: TRANSPORTE/MAYOREO/ARMADORES", 5000, Color.Red, Color.White); return false; }

            if (oBBPP.GroupCode == 105 & (QryGroup61.Equals("N") & QryGroup62.Equals("N")))
            { this.SetMensaje("EL CLIENTE ES DE MAYOREO, ELIJE LA PROPIEDAD 61 O 62", 5000, Color.Red, Color.White); return false; }

            if (oBBPP.GroupCode == 106 & (QryGroup60.Equals("N") & QryGroup62.Equals("N")))
            { this.SetMensaje("EL CLIENTE ES DE MAYOREO, ELIJE LA PROPIEDAD 60 O 62", 5000, Color.Red, Color.White); return false; }

            if ((oBBPP.PayTermsGrpCode == 2 | oBBPP.PayTermsGrpCode == 21) & (QryGroup22.Equals("Y") | QryGroup23.Equals("Y")))
            { this.SetMensaje("NO PUEDES ASIGNAR LA PROPIEDAD DE PP A UN CLIENTE DE CONTADO", 5000, Color.Red, Color.White); return false; }

            if ((oBBPP.PayTermsGrpCode == 2 | oBBPP.PayTermsGrpCode == 21) && String.IsNullOrWhiteSpace(oBBPP.UserFields.Fields.Item("U_TipoVisita").Value.ToString()))
            { this.SetMensaje("ANTES DE GUARDAR, CAPTURA UNA REFERENCIA DE PAGO PARA ESTE CLIENTE", 5000, Color.Red, Color.White); return false; }
            
            return true;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            this.OnLoad(e);
            this.LimpiarTxt(this);
            txtCardCode.Focus();
            this.ModoFormulario("Buscar");

            btnCrear.Text = "Actualizar";
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            this.OnLoad(e);
            this.LimpiarTxt(this);
            txtCardCode.Focus();
            this.ModoFormulario("Nuevo");

            btnCrear.Text = "Crear";

            SDK_SAP.DI.Connection.InitializeConnection(idConnection);
            SDK_SAP.DI.Connection.StartConnection();
            oBP = (SAPbobsCOM.BusinessPartners)SDK_SAP.DI.Connection.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
        }

        private void txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                DataTable tbl1 =
                        connection.GetDataTable("LOG",
                                                "sp_SDKDataSource",
                                                new string[] { },
                                                new string[] { "@TipoConsulta", "@Key" },
                                                ref valuesOut, 19, txtCardCode.Text);
                if (tbl1.Rows.Count == 0)
                {
                    SDK.Documentos.frmListadoDocumentos formulario = new SDK.Documentos.frmListadoDocumentos(-7, txtCardCode.Text, txtCardName.Text, DateTime.Now, cbGroupCode.SelectedValue.ToString());
                    formulario.ShowDialog();
                    if (formulario.Row != null)
                        txtCardCode.Text = formulario.Row.Field<string>("CardCode");
                }

                if (!String.IsNullOrEmpty(txtCardCode.Text))
                {
                    DataTable tbl;
                    if (tbl1.Rows.Count == 0)
                    {
                        tbl = connection.GetDataTable("LOG",
                                                   "sp_SDKDataSource",
                                                   new string[] { },
                                                   new string[] { "@TipoConsulta", "@Key" },
                                                   ref valuesOut, 19, txtCardCode.Text);
                    }
                    else
                        tbl = tbl1;

                    if (tbl.Rows.Count > 0)
                    {
                        SDK_SAP.DI.Connection.InitializeConnection(idConnection);
                        SDK_SAP.DI.Connection.StartConnection();
                        oBBPP = (SAPbobsCOM.BusinessPartners)SDK_SAP.DI.Connection.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                        oBBPP.GetByKey(txtCardCode.Text);

                        //SDK_SAP.DI.Connection.CloseConnection();

                        txtCardCode.Text = tbl.Rows[0].Field<string>("CardCode");
                        txtCardName.Text = tbl.Rows[0].Field<string>("CardName");
                        txtCardFName.Text = tbl.Rows[0].Field<string>("CardFName");
                        cbCurrency.SelectedValue = tbl.Rows[0].Field<string>("Currency");
                        txtLicTradNum.Text = tbl.Rows[0].Field<string>("LicTradNum");
                        cbGroupCode.SelectedValue = tbl.Rows[0]["GroupCode"];

                        txtBalance.Text = tbl.Rows[0].Field<decimal>("Balance").ToString("N2");
                        txtDNotesBal.Text = tbl.Rows[0].Field<decimal>("DNotesBal").ToString("N2");

                        txtPhone1.Text = tbl.Rows[0].Field<string>("Phone1");
                        txtPhone2.Text = tbl.Rows[0].Field<string>("Phone2");
                        txtCellular.Text = tbl.Rows[0].Field<string>("Cellular");
                        txtU_Correo.Text = tbl.Rows[0].Field<string>("U_Correo");
                        txtVatIdUnCmp.Text = tbl.Rows[0].Field<string>("VatIdUnCmp");
                        txtIndustryName.Text = tbl.Rows[0].Field<string>("IndName");
                        cbIndustryC.SelectedValue = tbl.Rows[0].Field<Int32>("IndustryC");
                        cbSlpCode.SelectedValue = tbl.Rows[0]["SlpCode"];
                        cbCmpPrivate.SelectedValue = tbl.Rows[0].Field<string>("CmpPrivate");
                        rbActivo.Checked = tbl.Rows[0].Field<string>("frozenFor").Equals("N");
                        rbInactivo.Checked = tbl.Rows[0].Field<string>("frozenFor").Equals("Y");

                        txtGroupNum.Text = tbl.Rows[0].Field<string>("PymntGroup");
                        cbGroupNum.SelectedValue = tbl.Rows[0]["GroupNum"];
                        txtU_MPAGO2.Text = tbl.Rows[0].Field<string>("U_MPAGO2");
                        if (tbl.Rows[0]["U_MPAGO2"] != DBNull.Value)
                            cbU_MPAGO2.SelectedValue = tbl.Rows[0].Field<string>("U_MPAGO2");
                        txtU_CUENTA.Text = tbl.Rows[0].Field<string>("U_CUENTA");
                        txtU_RefPago.Text = tbl.Rows[0].Field<string>("U_RefPago");
                        txtU_Condiciones.Text = tbl.Rows[0].Field<string>("U_Condiciones");
                        txtU_CreditLineS.Text = tbl.Rows[0]["U_CreditLineS"] == DBNull.Value ? string.Empty : tbl.Rows[0].Field<decimal>("U_CreditLineS").ToString("N2");
                        txtU_CPago.Text = tbl.Rows[0]["U_CPago"] == DBNull.Value ? string.Empty : tbl.Rows[0].Field<Int16>("U_CPago").ToString();
                        cbListNum.SelectedValue = tbl.Rows[0].Field<Int16>("ListNum");
                        txtU_CPago.Text = tbl.Rows[0]["U_CPago"] == DBNull.Value ? string.Empty : tbl.Rows[0].Field<Int16>("U_CPago").ToString();
                        txtU_CreditLine.Text = tbl.Rows[0]["U_CreditLine"] == DBNull.Value ? string.Empty : tbl.Rows[0].Field<decimal>("U_CreditLine").ToString("N2");
                        txtCreditLine.Text = tbl.Rows[0].Field<decimal>("CreditLine").ToString("N2");

                        txtU_TipoVisita.Text = tbl.Rows[0].Field<string>("U_TipoVisita");
                        if (tbl.Rows[0]["U_TipoVisita"] != DBNull.Value)
                            cbU_TipoVisita.SelectedValue = tbl.Rows[0]["U_TipoVisita"];
                        txtU_Zona.Text = tbl.Rows[0].Field<string>("U_Zona");
                        txtU_RefPago.Text = tbl.Rows[0].Field<string>("U_RefPago");
                        txtFree_Text.Text = tbl.Rows[0].Field<string>("Free_Text");

                        DataTable tblFiscal =
                                    connection.GetDataTable("LOG",
                                                            "sp_SDKDataSource",
                                                            new string[] { },
                                                            new string[] { "@TipoConsulta", "@Key", "@sType" },
                                                            ref valuesOut, 20, txtCardCode.Text, "B");
                        dgvFiscal.DataSource = tblFiscal;

                        DataTable tblEntrega =
                                    connection.GetDataTable("LOG",
                                                            "sp_SDKDataSource",
                                                            new string[] { },
                                                            new string[] { "@TipoConsulta", "@Key", "@sType" },
                                                            ref valuesOut, 20, txtCardCode.Text, "S");
                        dgvEntrega.DataSource = tblEntrega;

                        DataTable tblContacto =
                          connection.GetDataTable("LOG",
                                                  "sp_SDKDataSource",
                                                  new string[] { },
                                                  new string[] { "@TipoConsulta", "@Key" },
                                                  ref valuesOut, 24, txtCardCode.Text);
                        dgvContactos.DataSource = tblContacto;

                        DataTable tblPropiedades =
                                    connection.GetDataTable("LOG",
                                                            "sp_SDKDataSource",
                                                            new string[] { },
                                                            new string[] { "@TipoConsulta", "@Key" },
                                                            ref valuesOut, 22, txtCardCode.Text);
                        tblPropiedades.Columns["Valor"].ReadOnly = false;
                        dgvPropiedades.DataSource = tblPropiedades;
                        this.ModoFormulario("Lectura");

                    }
                }
            }
        }

        private void cbTipoDireccion_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //try
            //{
            //     DataTable tbl =
            //        connection.GetDataTable("LOG",
            //                                "sp_SDKDataSource",
            //                                new string[] { },
            //                                new string[] { "@TipoConsulta", "@Key", "@sType"},
            //                                ref valuesOut, 20, txtCardCode.Text, cbTipoDireccion.Text);
            //     if (tbl.Rows.Count > 0)
            //     {
            //         txtAddress.Text = tbl.Rows[0].Field<string>("Address");
            //         txtStreet.Text = tbl.Rows[0].Field<string>("Street");
            //         txtBlock.Text = tbl.Rows[0].Field<string>("City");
            //         txtCity.Text = tbl.Rows[0].Field<string>("Address");
            //         txtZipCode.Text = tbl.Rows[0].Field<string>("ZipCode");
            //         txtCounty.Text = tbl.Rows[0].Field<string>("County");
            //         txtState.Text = tbl.Rows[0].Field<string>("State");
            //         cbState.SelectedValue = tbl.Rows[0].Field<string>("State");
            //         txtCountry.Text = tbl.Rows[0].Field<string>("Country");
            //         cbCountry.SelectedValue = tbl.Rows[0].Field<string>("Country");
            //         txtTaxCode.Text = tbl.Rows[0].Field<string>("TaxCode");
            //         cbTaxCode.SelectedValue = tbl.Rows[0].Field<string>("TaxCode");
            //         txtStreetNo.Text = tbl.Rows[0].Field<string>("StreetNo");
            //         txtBuilding.Text = tbl.Rows[0].Field<string>("Building");
            //     }
            //}
            //catch (Exception)
            //{
            //}
        }

        private void cbCountry_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ClasesSGUV.Form.ControlsForms.setDataSource(cbState, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListaEstados, cbCountry.SelectedValue, string.Empty), "Name", "Code", "--Estado--");
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                SDK_SAP.DI.Connection.InitializeConnection(idConnection);
                SDK_SAP.DI.Connection.StartConnection();

                if (String.IsNullOrEmpty(txtVatIdUnCmp.Text.Trim()))
                {
                    this.SetMensaje("Campo [Jefa de cobranza] obligatorio", 5000, Color.Red, Color.White);
                    return;
                }

                if (btnCrear.Text.Equals("Crear"))
                {
                    oBBPP = (SAPbobsCOM.BusinessPartners)SDK_SAP.DI.Connection.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);

                    #region Encabezado
                    oBBPP.CardType = SAPbobsCOM.BoCardTypes.cCustomer;
                    oBBPP.CardCode = txtCardCode.Text;
                    oBBPP.CardName = txtCardName.Text;
                    if (!string.IsNullOrEmpty(txtCardFName.Text.Trim()))
                        oBBPP.CardForeignName = txtCardFName.Text;
                    oBBPP.GroupCode = Convert.ToInt32(cbGroupCode.SelectedValue);
                    oBBPP.Currency = cbCurrency.SelectedValue.ToString();
                    if (oBBPP.Currency.Equals("$"))
                        oBBPP.DebitorAccount = "1130-001-000";
                    else if (oBBPP.Currency.Equals("USD"))
                        oBBPP.DebitorAccount = "1130-002-000";
                    oBBPP.FederalTaxID = txtLicTradNum.Text;
                    #endregion

                    #region General
                    oBBPP.Phone1 = txtPhone1.Text;
                    oBBPP.Phone2 = txtPhone2.Text;
                    oBBPP.Cellular = txtCellular.Text;
                    oBBPP.UserFields.Fields.Item("U_Correo").Value = txtU_Correo.Text;
                    oBBPP.Industry = Convert.ToInt32(cbIndustryC.SelectedValue);
                    oBBPP.CompanyPrivate = cbCmpPrivate.SelectedValue.ToString().Equals("I") ? SAPbobsCOM.BoCardCompanyTypes.cPrivate : SAPbobsCOM.BoCardCompanyTypes.cCompany;
                    if (!string.IsNullOrEmpty(txtVatIdUnCmp.Text))
                        oBBPP.UnifiedFederalTaxID = txtVatIdUnCmp.Text;
                    oBBPP.SalesPersonCode = Convert.ToInt32(cbSlpCode.SelectedValue);
                    oBBPP.Frozen = rbActivo.Checked ? SAPbobsCOM.BoYesNoEnum.tNO : SAPbobsCOM.BoYesNoEnum.tYES;
                    #endregion

                    #region Condiciones de pago
                    oBBPP.PayTermsGrpCode = Convert.ToInt32(cbGroupNum.SelectedValue);
                    oBBPP.UserFields.Fields.Item("U_CPAGO").Value = txtU_CPago.Text;
                    oBBPP.UserFields.Fields.Item("U_MPAGO2").Value = cbU_MPAGO2.SelectedValue;
                    oBBPP.PriceListNum = Convert.ToInt32(cbListNum.SelectedValue);
                    oBBPP.UserFields.Fields.Item("U_CUENTA").Value = txtU_CUENTA.Text;
                    oBBPP.UserFields.Fields.Item("U_RefPago").Value = txtU_RefPago.Text;
                    oBBPP.UserFields.Fields.Item("U_Condiciones").Value = txtU_Condiciones.Text;
                    oBBPP.UserFields.Fields.Item("U_CreditLine").Value = txtU_CreditLine.Text;
                    oBBPP.UserFields.Fields.Item("U_CreditLineS").Value = txtU_CreditLineS.Text;


                    oBBPP.UserFields.Fields.Item("U_CUENTA").Value = txtU_CUENTA.Text;
                    oBBPP.CreditLimit = Convert.ToDouble(txtCreditLine.Text == string.Empty ? "0" : txtCreditLine.Text);
                    #endregion

                    #region Direcciones
                    for (int i = 0; i <= oBP.Addresses.Count - 1; i++)
                    {
                        oBP.Addresses.SetCurrentLine(i);

                        if (!string.IsNullOrEmpty(oBP.Addresses.AddressName))
                        {
                            if (ExisteDireccion(oBP.Addresses.AddressName, oBP.Addresses.AddressType))
                            {
                                oBBPP.Addresses.AddressName = oBP.Addresses.AddressName;
                                oBBPP.Addresses.AddressType = oBP.Addresses.AddressType;
                                oBBPP.Addresses.Street = oBP.Addresses.Street;
                                oBBPP.Addresses.Block = oBP.Addresses.Block;
                                oBBPP.Addresses.City = oBP.Addresses.City;
                                oBBPP.Addresses.ZipCode = oBP.Addresses.ZipCode;
                                oBBPP.Addresses.County = oBP.Addresses.County;
                                oBBPP.Addresses.State = oBP.Addresses.State;
                                oBBPP.Addresses.Country = oBP.Addresses.Country;
                                if (!String.IsNullOrEmpty(oBP.Addresses.StreetNo.Trim()))
                                    oBBPP.Addresses.StreetNo = oBP.Addresses.StreetNo;
                                oBBPP.Addresses.Add();
                            }
                        }
                    }
                    #endregion

                    #region Personas de contacto
                    for (int i = 0; i <= oBP.ContactEmployees.Count - 1; i++)
                    {
                        oBP.ContactEmployees.SetCurrentLine(i);

                        if (!string.IsNullOrEmpty(oBP.ContactEmployees.Name))
                        {
                            oBBPP.ContactEmployees.Name = oBP.ContactEmployees.Name;
                            oBBPP.ContactEmployees.FirstName = oBP.ContactEmployees.FirstName;
                            oBBPP.ContactEmployees.LastName = oBP.ContactEmployees.LastName;
                            oBBPP.ContactEmployees.Address = oBP.ContactEmployees.Address;
                            oBBPP.ContactEmployees.Phone1 = oBP.ContactEmployees.Phone1;
                            oBBPP.ContactEmployees.Phone2 = oBP.ContactEmployees.Phone2;
                            oBBPP.ContactEmployees.MobilePhone = oBP.ContactEmployees.MobilePhone;
                            oBBPP.ContactEmployees.E_Mail = oBP.ContactEmployees.E_Mail;
                            oBBPP.ContactEmployees.Active = oBP.ContactEmployees.Active;

                            oBBPP.ContactEmployees.Add();
                        }
                    }
                    #endregion

                    #region Propiedades
                    foreach (var item in dgvPropiedades.Rows)
                    {
                        oBBPP.set_Properties(Convert.ToInt32(item.Cells[0].Value), Convert.ToBoolean(item.Cells[2].Value) ? SAPbobsCOM.BoYesNoEnum.tYES : SAPbobsCOM.BoYesNoEnum.tNO);
                    }
                    #endregion

                    #region Adicional
                    oBBPP.UserFields.Fields.Item("U_TipoVisita").Value = cbU_TipoVisita.SelectedValue;
                    oBBPP.UserFields.Fields.Item("U_Zona").Value = txtU_Zona.Text;
                    oBBPP.FreeText = txtFree_Text.Text;

                    #endregion

                    if (oBBPP.Add() != 0)
                        throw new Exception("Error al crear SN [" + SDK_SAP.DI.Connection.oCompany.GetLastErrorDescription() + "]");
                    else
                    {
                        var kea = new KeyPressEventArgs(Convert.ToChar(13));
                        this.txt_KeyPress(sender, kea);
                        this.SetMensaje("Listo", 5000, Color.Green, Color.White);
                    }

                    SDK_SAP.DI.Connection.CloseConnection();
                }
                else if (btnCrear.Text.Equals("Actualizar"))
                {
                    //oBBPP.GetByKey(txtCardCode.Text);

                    #region Encabezado
                    oBBPP.CardType = SAPbobsCOM.BoCardTypes.cCustomer;
                    oBBPP.CardName = txtCardName.Text;
                    if (!string.IsNullOrEmpty(txtCardFName.Text.Trim()))
                        oBBPP.CardForeignName = txtCardFName.Text;
                    oBBPP.GroupCode = Convert.ToInt32(cbGroupCode.SelectedValue);
                    oBBPP.Currency = cbCurrency.SelectedValue.ToString();
                    if (oBBPP.Currency.Equals("$"))
                        oBBPP.DebitorAccount = "1130-001-000";
                    else if (oBBPP.Currency.Equals("USD"))
                        oBBPP.DebitorAccount = "1130-002-000";
                    oBBPP.FederalTaxID = txtLicTradNum.Text;

                    if (rbActivo.Checked)
                    {
                        oBBPP.Frozen = SAPbobsCOM.BoYesNoEnum.tNO;
                        oBBPP.Valid = SAPbobsCOM.BoYesNoEnum.tYES;
                    }
                    else
                    {
                        oBBPP.Frozen = SAPbobsCOM.BoYesNoEnum.tYES;
                        oBBPP.Valid = SAPbobsCOM.BoYesNoEnum.tNO;
                    }
                    #endregion

                    #region General
                    oBBPP.Phone1 = txtPhone1.Text;
                    oBBPP.Phone2 = txtPhone2.Text;
                    oBBPP.Cellular = txtCellular.Text;
                    oBBPP.UserFields.Fields.Item("U_Correo").Value = txtU_Correo.Text;
                    oBBPP.Industry = Convert.ToInt32(cbIndustryC.SelectedValue);
                    oBBPP.CompanyPrivate = cbCmpPrivate.SelectedValue.ToString().Equals("I") ? SAPbobsCOM.BoCardCompanyTypes.cPrivate : SAPbobsCOM.BoCardCompanyTypes.cCompany;
                    oBBPP.UnifiedFederalTaxID = txtVatIdUnCmp.Text;
                    oBBPP.SalesPersonCode = Convert.ToInt32(cbSlpCode.SelectedValue);
                    #endregion

                    #region Condiciones de pago
                    oBBPP.PayTermsGrpCode = Convert.ToInt32(cbGroupNum.SelectedValue);

                    oBBPP.CreditLimit = Convert.ToDouble(txtCreditLine.Text == string.Empty ? "0" : txtCreditLine.Text);
                    oBBPP.PriceListNum = Convert.ToInt32(cbListNum.SelectedValue);

                    oBBPP.UserFields.Fields.Item("U_CPAGO").Value = txtU_CPago.Text;
                    oBBPP.UserFields.Fields.Item("U_MPAGO2").Value = cbU_MPAGO2.SelectedValue;

                    oBBPP.UserFields.Fields.Item("U_CUENTA").Value = txtU_CUENTA.Text;
                    oBBPP.UserFields.Fields.Item("U_RefPago").Value = txtU_RefPago.Text;
                    oBBPP.UserFields.Fields.Item("U_Condiciones").Value = txtU_Condiciones.Text;
                    oBBPP.UserFields.Fields.Item("U_CreditLine").Value = txtU_CreditLine.Text;
                    oBBPP.UserFields.Fields.Item("U_CreditLineS").Value = txtU_CreditLineS.Text;

                    oBBPP.UserFields.Fields.Item("U_CUENTA").Value = txtU_CUENTA.Text;
                    #endregion

                    #region Direcciones
                    //for (int i = 0; i <= oBP.Addresses.Count - 1; i++)
                    //{
                    //    oBP.Addresses.SetCurrentLine(i);

                    //    if (!string.IsNullOrEmpty(oBP.Addresses.AddressName))
                    //    {
                    //        oBBPP.Addresses.AddressName = oBP.Addresses.AddressName;
                    //        oBBPP.Addresses.AddressType = oBP.Addresses.AddressType;
                    //        oBBPP.Addresses.Street = oBP.Addresses.Street;
                    //        oBBPP.Addresses.Block = oBP.Addresses.Block;
                    //        oBBPP.Addresses.City = oBP.Addresses.City;
                    //        oBBPP.Addresses.ZipCode = oBP.Addresses.ZipCode;
                    //        oBBPP.Addresses.County = oBP.Addresses.County;
                    //        oBBPP.Addresses.State = oBP.Addresses.State;
                    //        oBBPP.Addresses.Country = oBP.Addresses.Country;
                    //        oBBPP.Addresses.StreetNo = oBP.Addresses.StreetNo;
                    //        oBBPP.Addresses.Add();
                    //    }
                    //}
                    #endregion

                    #region Contacto
                    if (oBBPP.ContactEmployees.Count > 0)
                        oBBPP.ContactPerson = oBBPP.ContactEmployees.Name;
                    #endregion

                    #region Propiedades
                    foreach (var item in dgvPropiedades.Rows)
                    {
                        oBBPP.set_Properties(Convert.ToInt32(item.Cells[0].Value), Convert.ToBoolean(item.Cells[2].Value) ? SAPbobsCOM.BoYesNoEnum.tYES : SAPbobsCOM.BoYesNoEnum.tNO);

                        if (Convert.ToInt32(item.Cells[0].Value) == 60) QryGroup60 = Convert.ToBoolean(item.Cells[2].Value) ? "Y" : "N";
                        if (Convert.ToInt32(item.Cells[0].Value) == 61) QryGroup61 = Convert.ToBoolean(item.Cells[2].Value) ? "Y" : "N";
                        if (Convert.ToInt32(item.Cells[0].Value) == 62) QryGroup62 = Convert.ToBoolean(item.Cells[2].Value) ? "Y" : "N";
                        if (Convert.ToInt32(item.Cells[0].Value) == 22) QryGroup22 = Convert.ToBoolean(item.Cells[2].Value) ? "Y" : "N";
                        if (Convert.ToInt32(item.Cells[0].Value) == 23) QryGroup23 = Convert.ToBoolean(item.Cells[2].Value) ? "Y" : "N";
                    }
                    #endregion

                    #region Adicional
                    oBBPP.UserFields.Fields.Item("U_TipoVisita").Value = cbU_TipoVisita.SelectedValue;
                    oBBPP.UserFields.Fields.Item("U_Zona").Value = txtU_Zona.Text;
                    oBBPP.FreeText = txtFree_Text.Text;
                    #endregion

                    if (this.ValidarSN())

                        if (oBBPP.Update() != 0)
                        {
                            int intnErr; string strerrMsg;
                            SDK_SAP.DI.Connection.oCompany.GetLastError(out intnErr, out strerrMsg);
                            throw new Exception("Error al actualizar SN [" + intnErr + " " + strerrMsg + "]");
                        }
                        else
                        {
                            var kea = new KeyPressEventArgs(Convert.ToChar(13));
                            this.txt_KeyPress(sender, kea);
                            this.SetMensaje("Listo", 5000, Color.Green, Color.White);
                        }
                    else
                        return;

                    SDK_SAP.DI.Connection.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void dgv_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if (!e.Row.IsAddRow)
                e.Row.Cells[0].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
        }

        private void dgv_ClickCell(object sender, Infragistics.Win.UltraWinGrid.ClickCellEventArgs e)
        {
            try
            {
                string AdressType = (sender as Infragistics.Win.UltraWinGrid.UltraGrid).Name.Equals("dgvFiscal") ? "B" : "S";
                if (!txtCardCode.ReadOnly)//si es articulo nuevo
                {
                    for (int i = 0; i <= oBP.Addresses.Count - 1; i++)
                    {
                        oBP.Addresses.SetCurrentLine(i);

                        if ((oBP.Addresses.AddressName).Equals((sender as Infragistics.Win.UltraWinGrid.UltraGrid).ActiveRow.Cells["Address"].Value)
                            && ((sender as Control).Name.Equals("dgvFiscal") ? SAPbobsCOM.BoAddressType.bo_BillTo : SAPbobsCOM.BoAddressType.bo_ShipTo) == oBP.Addresses.AddressType)
                        {
                            cbTipoDireccion.SelectedIndex = oBP.Addresses.AddressType == SAPbobsCOM.BoAddressType.bo_BillTo ? 0 : 1;
                            txtAddress.Text = oBP.Addresses.AddressName;
                            txtStreet.Text = oBP.Addresses.Street;
                            txtBlock.Text = oBP.Addresses.Block;
                            txtCity.Text = oBP.Addresses.City;
                            txtZipCode.Text = oBP.Addresses.ZipCode;
                            txtCounty.Text = oBP.Addresses.County;
                            txtState.Text = oBP.Addresses.State;
                            cbState.SelectedValue = oBP.Addresses.State;
                            txtCountry.Text = oBP.Addresses.Country;
                            cbCountry.SelectedValue = oBP.Addresses.Country;
                            txtTaxCode.Text = oBP.Addresses.TaxCode;
                            cbTaxCode.SelectedValue = oBP.Addresses.TaxCode;
                            txtStreetNo.Text = oBP.Addresses.StreetNo;
                            txtBuilding.Text = oBP.Addresses.BuildingFloorRoom;
                        }
                    }
                }
                else
                {
                    DataTable tbl =
                       connection.GetDataTable("LOG",
                                               "sp_SDKDataSource",
                                               new string[] { },
                                               new string[] { "@TipoConsulta", "@Key", "@sType", "@adresType" },
                                               ref valuesOut, 21, txtCardCode.Text, (sender as Infragistics.Win.UltraWinGrid.UltraGrid).ActiveRow.Cells["Address"].Value, AdressType);
                    if (tbl.Rows.Count > 0)
                    {
                        cbTipoDireccion.SelectedIndex = AdressType.Equals("B") ? 0 : 1;

                        txtAddress.Text = tbl.Rows[0].Field<string>("Address");
                        txtStreet.Text = tbl.Rows[0].Field<string>("Street");
                        txtBlock.Text = tbl.Rows[0].Field<string>("Block");
                        txtCity.Text = tbl.Rows[0].Field<string>("City");
                        txtZipCode.Text = tbl.Rows[0].Field<string>("ZipCode");
                        txtCounty.Text = tbl.Rows[0].Field<string>("County");
                        txtState.Text = tbl.Rows[0].Field<string>("State");
                        cbState.SelectedValue = tbl.Rows[0].Field<string>("State");
                        txtCountry.Text = tbl.Rows[0].Field<string>("Country");
                        cbCountry.SelectedValue = tbl.Rows[0].Field<string>("Country");
                        txtTaxCode.Text = tbl.Rows[0].Field<string>("TaxCode");
                        cbTaxCode.SelectedValue = tbl.Rows[0].Field<string>("TaxCode");
                        txtStreetNo.Text = tbl.Rows[0].Field<string>("StreetNo");
                        txtBuilding.Text = tbl.Rows[0].Field<string>("Building");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void dgvFiscal_AfterCellActivate(object sender, EventArgs e)
        {

        }

        private int ExistAddress(string ID, SAPbobsCOM.BoAddressType Tipo, SAPbobsCOM.BusinessPartners obp)
        {
            for (int i = 0; i <= obp.Addresses.Count - 1; i++)
            {
                obp.Addresses.SetCurrentLine(i);
                if (obp.Addresses.AddressName.Equals(ID) && obp.Addresses.AddressType == Tipo)
                    return i;
            }
            return -1;
        }

        private int ExistPerson(string ID, SAPbobsCOM.BusinessPartners obp)
        {
            for (int i = 0; i <= obp.ContactEmployees.Count - 1; i++)
            {
                obp.ContactEmployees.SetCurrentLine(i);
                if (obp.ContactEmployees.Name.Equals(ID))
                    return i;
            }
            return -1;
        }

        private void btnAgregarDir_Click(object sender, EventArgs e)
        {
            #region validaciones
            if (String.IsNullOrEmpty(txtStreet.Text))
            {
                this.SetMensaje("Campo [Calle] obligatorio", 5000, Color.Red, Color.White);
                return;
            }
            if (String.IsNullOrEmpty(txtBlock.Text))
            {
                this.SetMensaje("Campo [Colonia] obligatorio", 5000, Color.Red, Color.White);
                return;
            }
            if (String.IsNullOrEmpty(txtCity.Text))
            {
                this.SetMensaje("Campo [Ciudad] obligatorio", 5000, Color.Red, Color.White);
                return;
            }
            if (String.IsNullOrEmpty(txtZipCode.Text))
            {
                this.SetMensaje("Campo [Código postal] obligatorio", 5000, Color.Red, Color.White);
                return;
            }
            if (String.IsNullOrEmpty(txtCounty.Text))
            {
                this.SetMensaje("Campo [Municipio] obligatorio", 5000, Color.Red, Color.White);
                return;
            }
            if (cbState.SelectedIndex == 0)
            {
                this.SetMensaje("Campo [Estado] obligatorio", 5000, Color.Red, Color.White);
                return;
            }
            #endregion


            if (btnAgregarDir.Text.Equals("Agregar dirección"))
            {
                int line = this.ExistAddress(txtAddress.Text, cbTipoDireccion.Text.Equals("Destinatario de factura") ? SAPbobsCOM.BoAddressType.bo_BillTo : SAPbobsCOM.BoAddressType.bo_ShipTo, oBP);
                if (line < 0)
                {
                    oBP.Addresses.Add();
                    oBP.Addresses.SetCurrentLine(oBP.Addresses.Count - 1);

                    if ((cbTipoDireccion.Text.Equals("Destinatario de factura") ? SAPbobsCOM.BoAddressType.bo_BillTo : SAPbobsCOM.BoAddressType.bo_ShipTo) == SAPbobsCOM.BoAddressType.bo_BillTo)// :fisacal
                    {
                        DataRow row = ((dgvFiscal.DataSource) as DataTable).NewRow();
                        row[0] = txtAddress.Text;
                        ((dgvFiscal.DataSource) as DataTable).Rows.Add(row);
                    }
                    if ((cbTipoDireccion.Text.Equals("Destinatario de factura") ? SAPbobsCOM.BoAddressType.bo_BillTo : SAPbobsCOM.BoAddressType.bo_ShipTo) == SAPbobsCOM.BoAddressType.bo_ShipTo)// :entrega
                    {
                        DataRow row = ((dgvEntrega.DataSource) as DataTable).NewRow();
                        row[0] = txtAddress.Text;
                        ((dgvEntrega.DataSource) as DataTable).Rows.Add(row);
                    }
                }
                else
                    oBP.Addresses.SetCurrentLine(line);

                if ((cbTipoDireccion.Text.Equals("Destinatario de factura") ? SAPbobsCOM.BoAddressType.bo_BillTo : SAPbobsCOM.BoAddressType.bo_ShipTo) == SAPbobsCOM.BoAddressType.bo_BillTo)// :fical
                {
                    bool exists = false;
                    foreach (var item in dgvFiscal.Rows)
                    {
                        if (item.Cells[0].Value.ToString().Equals(txtAddress.Text))
                            exists = true;
                    }

                    if (!exists)
                    {
                        DataRow row = ((dgvFiscal.DataSource) as DataTable).NewRow();
                        row[0] = txtAddress.Text;
                        ((dgvFiscal.DataSource) as DataTable).Rows.Add(row);
                    }
                }
                if ((cbTipoDireccion.Text.Equals("Destinatario de factura") ? SAPbobsCOM.BoAddressType.bo_BillTo : SAPbobsCOM.BoAddressType.bo_ShipTo) == SAPbobsCOM.BoAddressType.bo_ShipTo)// :entrega
                {
                    bool exists = false;
                    foreach (var item in dgvEntrega.Rows)
                    {
                        if (item.Cells[0].Value.ToString().Equals(txtAddress.Text))
                            exists = true;
                    }

                    if (!exists)
                    {
                        DataRow row = ((dgvEntrega.DataSource) as DataTable).NewRow();
                        row[0] = txtAddress.Text;
                        ((dgvEntrega.DataSource) as DataTable).Rows.Add(row);
                    }
                }
                oBP.Addresses.AddressName = txtAddress.Text;
                oBP.Addresses.AddressType = cbTipoDireccion.Text.Equals("Destinatario de factura") ? SAPbobsCOM.BoAddressType.bo_BillTo : SAPbobsCOM.BoAddressType.bo_ShipTo;
                oBP.Addresses.Street = txtStreet.Text;
                oBP.Addresses.Block = txtBlock.Text;
                oBP.Addresses.City = txtCity.Text;
                oBP.Addresses.ZipCode = txtZipCode.Text;
                oBP.Addresses.County = txtCounty.Text;
                oBP.Addresses.State = cbState.SelectedValue.ToString();
                oBP.Addresses.Country = cbCountry.SelectedValue.ToString();
                oBP.Addresses.BuildingFloorRoom = txtBuilding.Text;
                oBP.Addresses.TaxCode = cbTaxCode.SelectedValue.ToString();
                if (!string.IsNullOrEmpty(txtStreetNo.Text))
                    oBP.Addresses.StreetNo = txtStreetNo.Text;
            }
            else
                if (btnAgregarDir.Text.Equals("Actualizar dirección"))
                {
                    int line = this.ExistAddress(txtAddress.Text, cbTipoDireccion.Text.Equals("Destinatario de factura") ? SAPbobsCOM.BoAddressType.bo_BillTo : SAPbobsCOM.BoAddressType.bo_ShipTo, oBBPP);
                    if (line < 0)
                    {
                        oBBPP.Addresses.Add();
                        oBBPP.Addresses.SetCurrentLine(oBBPP.Addresses.Count - 1);

                        if ((cbTipoDireccion.Text.Equals("Destinatario de factura") ? SAPbobsCOM.BoAddressType.bo_BillTo : SAPbobsCOM.BoAddressType.bo_ShipTo) == SAPbobsCOM.BoAddressType.bo_BillTo)// :fisacal
                        {
                            DataRow row = ((dgvFiscal.DataSource) as DataTable).NewRow();
                            row[0] = txtAddress.Text;
                            ((dgvFiscal.DataSource) as DataTable).Rows.Add(row);
                        }
                        if ((cbTipoDireccion.Text.Equals("Destinatario de factura") ? SAPbobsCOM.BoAddressType.bo_BillTo : SAPbobsCOM.BoAddressType.bo_ShipTo) == SAPbobsCOM.BoAddressType.bo_ShipTo)// :entrega
                        {
                            DataRow row = ((dgvEntrega.DataSource) as DataTable).NewRow();
                            row[0] = txtAddress.Text;
                            ((dgvEntrega.DataSource) as DataTable).Rows.Add(row);
                        }
                    }
                    else
                        oBBPP.Addresses.SetCurrentLine(line);

                    oBBPP.Addresses.AddressName = txtAddress.Text;
                    oBBPP.Addresses.AddressType = cbTipoDireccion.Text.Equals("Destinatario de factura") ? SAPbobsCOM.BoAddressType.bo_BillTo : SAPbobsCOM.BoAddressType.bo_ShipTo;
                    oBBPP.Addresses.Street = txtStreet.Text;
                    oBBPP.Addresses.Block = txtBlock.Text;
                    oBBPP.Addresses.City = txtCity.Text;
                    oBBPP.Addresses.ZipCode = txtZipCode.Text;
                    oBBPP.Addresses.County = txtCounty.Text;
                    oBBPP.Addresses.State = cbState.SelectedValue.ToString();
                    oBBPP.Addresses.Country = cbCountry.SelectedValue.ToString();
                    if (!String.IsNullOrEmpty(txtStreetNo.Text.Trim()))
                        oBBPP.Addresses.StreetNo = txtStreetNo.Text;
                    oBBPP.Addresses.BuildingFloorRoom = txtBuilding.Text;
                    oBBPP.Addresses.TaxCode = cbTaxCode.SelectedValue.ToString();
                }
        }

        private void dgvPropiedades_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["Num"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["GroupName"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["GroupName"].Width = 400;
            e.Layout.Bands[0].Columns["Valor"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
        }

        private void btnAgregarCon_Click(object sender, EventArgs e)
        {
            if (btnAgregarContact.Text.Equals("Agregar persona"))
            {
                int line = this.ExistPerson(txtName.Text, oBP);
                if (line < 0)
                {
                    oBP.ContactEmployees.Add();
                    oBP.ContactEmployees.SetCurrentLine(oBP.ContactEmployees.Count - 1);

                    DataRow row = ((dgvContactos.DataSource) as DataTable).NewRow();
                    row[0] = txtName.Text;
                    ((dgvContactos.DataSource) as DataTable).Rows.Add(row);
                }
                else
                    oBP.ContactEmployees.SetCurrentLine(line);

                oBP.ContactEmployees.Name = txtName.Text;
                oBP.ContactEmployees.FirstName = txtFirstName.Text;
                oBP.ContactEmployees.LastName = txtLastName.Text;
                oBP.ContactEmployees.Address = txtContactAddress.Text;
                oBP.ContactEmployees.Phone1 = txtPhone1.Text;
                oBP.ContactEmployees.Phone2 = txtPhone2.Text;
                oBP.ContactEmployees.MobilePhone = txtCellolar.Text;
                oBP.ContactEmployees.E_Mail = txtEmaiL.Text;
                oBP.ContactEmployees.Active = cbContactActive.Checked ? SAPbobsCOM.BoYesNoEnum.tYES : SAPbobsCOM.BoYesNoEnum.tNO;
            }
            else if (btnAgregarContact.Text.Equals("Actualizar persona"))
            {
                int line = this.ExistPerson(txtName.Text, oBBPP);
                if (line < 0)
                {
                    oBBPP.ContactEmployees.Add();
                    oBBPP.ContactEmployees.SetCurrentLine(oBBPP.ContactEmployees.Count - 1);

                    DataRow row = ((dgvContactos.DataSource) as DataTable).NewRow();
                    row[0] = txtName.Text;
                    ((dgvContactos.DataSource) as DataTable).Rows.Add(row);
                }
                else
                    oBBPP.ContactEmployees.SetCurrentLine(line);

                oBBPP.ContactEmployees.Name = txtName.Text;
                oBBPP.ContactEmployees.FirstName = txtFirstName.Text;
                oBBPP.ContactEmployees.LastName = txtLastName.Text;
                oBBPP.ContactEmployees.Address = txtContactAddress.Text;
                oBBPP.ContactEmployees.Phone1 = txtPhone1.Text;
                oBBPP.ContactEmployees.Phone2 = txtPhone2.Text;
                oBBPP.ContactEmployees.MobilePhone = txtCellolar.Text;
                oBBPP.ContactEmployees.E_Mail = txtEmaiL.Text;
                oBBPP.ContactEmployees.Active = cbContactActive.Checked ? SAPbobsCOM.BoYesNoEnum.tYES : SAPbobsCOM.BoYesNoEnum.tNO;

            }
        }

        private void dgvContactos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns[0].Width = 150;
        }

        private void dgvContactos_ClickCell(object sender, Infragistics.Win.UltraWinGrid.ClickCellEventArgs e)
        {
            try
            {
                if (!txtCardCode.ReadOnly)//si es cliente nuevo
                {
                    for (int i = 0; i <= oBP.ContactEmployees.Count - 1; i++)
                    {
                        oBP.ContactEmployees.SetCurrentLine(i);

                        if ((oBP.ContactEmployees.Name).Equals((sender as Infragistics.Win.UltraWinGrid.UltraGrid).ActiveRow.Cells["Address"].Value))
                        {
                            txtName.Text = oBP.ContactEmployees.Name;
                            txtFirstName.Text = oBP.ContactEmployees.FirstName;
                            txtLastName.Text = oBP.ContactEmployees.LastName;
                            txtContactAddress.Text = oBP.ContactEmployees.Address;
                            txtPhone1.Text = oBP.ContactEmployees.Phone1;
                            txtPhone2.Text = oBP.ContactEmployees.Phone2;
                            txtCellolar.Text = oBP.ContactEmployees.MobilePhone;
                            txtEmaiL.Text = oBP.ContactEmployees.E_Mail;
                            cbContactActive.Checked = oBP.ContactEmployees.Active == SAPbobsCOM.BoYesNoEnum.tYES;
                        }
                    }
                }
                else
                {
                    DataTable tbl =
                       connection.GetDataTable("LOG",
                                               "sp_SDKDataSource",
                                               new string[] { },
                                               new string[] { "@TipoConsulta", "@Key", "@adresType" },
                                               ref valuesOut, 25, txtCardCode.Text, dgvContactos.ActiveRow.Cells[0].Value);
                    if (tbl.Rows.Count > 0)
                    {
                        txtName.Text = tbl.Rows[0].Field<string>("Name");
                        txtFirstName.Text = tbl.Rows[0].Field<string>("FirstName");
                        txtLastName.Text = tbl.Rows[0].Field<string>("LastName");
                        txtContactAddress.Text = tbl.Rows[0].Field<string>("Address");
                        txtPhone1.Text = tbl.Rows[0].Field<string>("Tel1");
                        txtPhone2.Text = tbl.Rows[0].Field<string>("Tel2");
                        txtCellolar.Text = tbl.Rows[0].Field<string>("Cellolar");
                        txtEmaiL.Text = tbl.Rows[0].Field<string>("E_EmaiL");

                        cbContactActive.Checked = tbl.Rows[0].Field<string>("Active") == "Y";
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void cbSlpCode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //if(llenarJefa)
            //{
            DataTable tbl = connection.GetDataTable("LOG",
                                     "sp_DataSource",
                                     new string[] { },
                                     new string[] { "@TipoConsulta", "@Filtro" },
                                     ref valuesOut, 39, cbSlpCode.SelectedValue);
            if (tbl.Rows.Count > 0)
                txtVatIdUnCmp.Text = tbl.Rows[0]["U_Cobranza"] == DBNull.Value ? string.Empty : tbl.Rows[0]["U_Cobranza"].ToString();
            // }
        }

        private void dgvFiscal_AfterRowsDeleted(object sender, EventArgs e)
        {

        }

        private void dgvFiscal_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
        {
            e.DisplayPromptMsg = false;
            if (MessageBox.Show("¿Desea eliminar la dirección?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            if (btnCrear.Text == "Crear")
            {
                //foreach (var item in e.Rows)
                //{
                //    for (int i = 0; i <= oBP.Addresses.Count - 1; i++)
                //    {
                //        oBP.Addresses.SetCurrentLine(i);
                //        if (oBP.Addresses.AddressName.Equals(item.Cells[0].Value.ToString()) && oBP.Addresses.AddressType == SAPbobsCOM.BoAddressType.bo_BillTo)
                //            oBP.Address.Delete();
                //    }
                //}
            }
            else
            {
                foreach (var item in e.Rows)
                {
                    for (int i = 0; i <= oBBPP.Addresses.Count - 1; i++)
                    {
                        oBBPP.Addresses.SetCurrentLine(i);
                        if (oBBPP.Addresses.AddressName.Equals(item.Cells[0].Value.ToString()) && oBBPP.Addresses.AddressType == SAPbobsCOM.BoAddressType.bo_BillTo)
                            oBBPP.Address.Remove(i);
                    }
                }
            }
        }

        private void dgvEntrega_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
        {
            e.DisplayPromptMsg = false;
            if (MessageBox.Show("¿Desea eliminar la dirección?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            if (btnCrear.Text == "Crear")
            {
                //foreach (var item in e.Rows)
                //{
                //    for (int i = 0; i <= oBP.Addresses.Count - 1; i++)
                //    {
                //        oBP.Addresses.SetCurrentLine(i);
                //        if (oBP.Addresses.AddressName.Equals(item.Cells[0].Value.ToString()) && oBP.Addresses.AddressType == SAPbobsCOM.BoAddressType.bo_BillTo)
                //            oBP.Address.Delete();
                //    }
                //}
            }
            else
            {
                foreach (var item in e.Rows)
                {
                    for (int i = 0; i <= oBBPP.Addresses.Count - 1; i++)
                    {
                        oBBPP.Addresses.SetCurrentLine(i);
                        if (oBBPP.Addresses.AddressName.Equals(item.Cells[0].Value.ToString()) && oBBPP.Addresses.AddressType == SAPbobsCOM.BoAddressType.bo_ShipTo)
                            oBBPP.Address.Remove(i);
                    }
                }
            }
        }

        private void btnXML_Click(object sender, EventArgs e)
        {
            SDK_SAP.DI.Connection.oCompany.XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode;

            oBBPP.SaveXML(@"C:\pp\cliente.xml");
        }
    }
}
