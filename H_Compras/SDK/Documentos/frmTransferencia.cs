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
    public partial class frmTransferencia : Constantes.frmEmpty
    {
        bool _isSolicitud = false;

        public bool IsSolicitud
        {
            get { return _isSolicitud; }
            set { _isSolicitud = value; }
        }

        int IdDocument;
        Decimal IVA;
        //string VatGroup;
        SDK.Configuracion.SDK_Configuracion_Transferencias config;
        SDK.SDKDatos.SDK_OWTR Document = new SDKDatos.SDK_OWTR();

        public frmTransferencia()
        {
            InitializeComponent();
        }

        public frmTransferencia(int _idDocument)
        {
            InitializeComponent();
            dgvDatos.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
            //  I  N  I  C  I  A  L  I  Z  A  R
            //dgvDatos.DataSource = _details;
            IdDocument = _idDocument;
            config = new Configuracion.SDK_Configuracion_Transferencias(IdDocument, this);
            config.ModeDocument = "Edit";
            config.StartEmpty();
            IVA = config.IVA1;
            this.AccessibleDescription = "SDK " + this.Text;
            this.btnCrear.Text = "Actualizar";

            txtNumero.BackColor = Color.FromName("Info");

            cerrarStripButton.Enabled = true;
            nuevoToolStripButton.Enabled = true;
        }

        public frmTransferencia(DataTable _details, int _idDocument, string _filler, string _toWhsCode)
        {
            InitializeComponent();
            dgvDatos.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
            //  I  N  I  C  I  A  L  I  Z  A  R
            IdDocument = _idDocument;
            config = new Configuracion.SDK_Configuracion_Transferencias(IdDocument, this);
            config.ModeDocument = "New";
            config.StartFill(IdDocument, _details, _filler, _toWhsCode);
            IVA = config.IVA1;
            //VatGroup = config.VatGroup;
            this.AccessibleDescription = "SDK " + this.Text;
            this.btnCrear.Text = "Crear";
            nuevoToolStripButton.Enabled = false;

            this.ModoForm(1);
        }

        public frmTransferencia(DataTable _details, int _idDocument, string _filler, string _toWhsCode, DataTable _DetalleSolicitud ,int Solicitud, int TipoSolicitud)
        {
            InitializeComponent();
            dgvDatos.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
            //  I  N  I  C  I  A  L  I  Z  A  R
            IdDocument = _idDocument;
            config = new Configuracion.SDK_Configuracion_Transferencias(IdDocument, this);
            config.ModeDocument = "New";
            config.StartFill(IdDocument, _details, _filler, _toWhsCode);
            IVA = config.IVA1;
            //VatGroup = config.VatGroup;
            this.AccessibleDescription = "SDK " + this.Text;
            this.btnCrear.Text = "Crear";
            nuevoToolStripButton.Enabled = false;

            /**********************************************************/
            label7.Visible = true;
            label8.Visible = true;
            txtIDSolicitud.Visible = true;
            radioButton1.Visible = true;
            radioButton2.Visible = true;
            //---se asignan los valores principales----
            if (TipoSolicitud == 1)
                radioButton1.Checked = true;
            else
                radioButton2.Checked = true;
            txtIDSolicitud.Text = Solicitud.ToString();
            //-----------------------------------------
            this.ModoForm(1);

            //*****************SE CONSULTAN LOS ALMACENES DESTINO*********************************************/
            DataTable DTAlmacenesDestino = new DataTable();
            if (dgvDatos.DisplayLayout.Bands[0].Columns.Exists("WhsName"))
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_SDKDataSource", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 10);
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(DTAlmacenesDestino);
                    }
                }
            }
            //--------------SE OBTIENE EL NOMBRE DEL ALMACEN-------------------------------
            DataRow[] drAlmacenDest = DTAlmacenesDestino.Select("code ='" + _toWhsCode + "'");
            string NomAlmacen = "";
            if (drAlmacenDest.Length > 0)
                NomAlmacen = drAlmacenDest[0]["code"].ToString() + " | " + drAlmacenDest[0]["name"].ToString();
            //-------------------------------------------------------------------------------------------------
            DataTable dtArt = (DataTable)ultraDropDown1.DataSource;


            int Fila = -1;
            foreach (DataRow item in _DetalleSolicitud.Rows)
            {
                dgvDatos.Rows.Band.AddNew();
                Fila = Fila + 1;
                foreach (DataColumn Col in _DetalleSolicitud.Columns)
                {
                    if (dgvDatos.DisplayLayout.Bands[0].Columns.Exists(Col.ColumnName.ToString()))
                    {
                        if (Col.ColumnName == "ItemCode") // si es el articulo entonces se van a asignar los demas campos
                        {
                            dgvDatos.Rows[Fila].Cells[Col.ColumnName].Value = item[Col.ColumnName];
                            DataRow[] Articulo = dtArt.Select("ItemCode = '" + item[Col.ColumnName] + "'");
                            if (Articulo.Length > 0)
                            {
                                if (dgvDatos.DisplayLayout.Bands[0].Columns.Exists("ItemName"))
                                    dgvDatos.Rows[Fila].Cells["ItemName"].Value = Articulo[0]["ItemName"];

                                if (dgvDatos.DisplayLayout.Bands[0].Columns.Exists("ManBtchNum"))
                                    dgvDatos.Rows[Fila].Cells["ManBtchNum"].Value = Articulo[0]["ManBtchNum"];
                            }
                        }
                        else
                            dgvDatos.Rows[Fila].Cells[Col.ColumnName].Value = item[Col.ColumnName];
                    }
                }
                if (dgvDatos.DisplayLayout.Bands[0].Columns.Exists("WhsName"))
                    dgvDatos.Rows[Fila].Cells["WhsName"].Value = NomAlmacen;
                
            }

            if(Fila>-1)
                dgvDatos.Rows.Band.AddNew();

            if (dgvDatos.DisplayLayout.Bands[0].Columns.Exists("ItemCode"))
                dgvDatos.DisplayLayout.Bands[0].Columns["ItemCode"].CellActivation = Activation.NoEdit;
            
            if (dgvDatos.DisplayLayout.Bands[0].Columns.Exists("Quantity"))
                dgvDatos.DisplayLayout.Bands[0].Columns["Quantity"].CellActivation = Activation.NoEdit;

        }

        private void ModoForm(int tipo)
        {
            switch (tipo)
            {
                case 1://Documento nuevo
                    btnCrear.Text = "Crear";
                    break;
                case 2: //consulta previo
                    txtNumero.ReadOnly = true;
                    dtpDocDate.Enabled = false;
                    txtToWhsCode.ReadOnly = true;
                    txtComments.ReadOnly = true;
                    //dgvDatos.Enabled = false;
                    btnCrear.Enabled = false;
                    btnCancelar.Text = "Descartar";

                    this.nuevoToolStripButton.Enabled = false;
                    this.nuevoToolStripButton.Enabled = false;
                    this.exportarToolStripButton.Enabled = false;
                    this.actualizarToolStripButton.Enabled = false;
                    this.ayudaToolStripButton.Enabled = false;

                    break;
                case 3: //consulta solicitud de traslado 
                    txtNumero.ReadOnly = false;
                    dtpDocDate.Enabled = false;
                    txtToWhsCode.ReadOnly = true;
                    txtComments.ReadOnly = true;
                    //dgvDatos.Enabled = false;

                    btnCrear.Enabled = false;
                    btnCancelar.Enabled = false;

                    btnCrear.Text = "Crear";
                    btnCancelar.Text = "Cancelar";

                    this.nuevoToolStripButton.Enabled = false;
                    this.buscarStripButton.Enabled = true;
                    this.exportarToolStripButton.Enabled = false;
                    this.actualizarToolStripButton.Enabled = false;
                    this.ayudaToolStripButton.Enabled = false;

                    break;
                default: break;
            }
        }

        private void CleanScreen()
        {
            txtNumero.Clear();
            txtSerie.Clear();
            dtpDocDate.Value = DateTime.Now;
            txtComments.Clear();
            txtToWhsCode.Clear();

            (dgvDatos.DataSource as DataTable).Rows.Clear();
        }

        private void frmSalidas_Load(object sender, EventArgs e)
        {
            Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

            nuevoToolStripButton.Click -= new EventHandler(btnNuevo_Click);
            nuevoToolStripButton.Click += new EventHandler(btnNuevo_Click);

            buscarStripButton.Click += new EventHandler(btnBuscar_Click);
            buscarStripButton.Click += new EventHandler(btnBuscar_Click);

            imprimirToolStripButton.Click -= new EventHandler(btnImprimir_Click);
            imprimirToolStripButton.Click += new EventHandler(btnImprimir_Click);

            imprimirToolStripButton.Enabled = true;
        }

        #region EVENTOS MENU
        public void btnNuevo_Click(object sender, EventArgs e)
        {
            this.CleanScreen();

            txtNumero.BackColor = txtComments.BackColor;

            btnCrear.Text = "Crear";
            config.ModeDocument = "New";

            //txtNumero.KeyPress -= new KeyPressEventHandler(txtFolio_KeyPress);

            this.dgvDatos.DataSource = (dgvDatos.DataSource as DataTable).Copy();

            //agregarFilaToolStripMenuItem1.Enabled = true;
            //toolStripEliminar.Enabled = true;
            //tooStripDuplicar.Enabled = true;
        }
        public void btnBuscar_Click(object sender, EventArgs e)
        {
            this.CleanScreen();
        }
        public void btnImprimir_Click(object sender, EventArgs e)
        {
            Constantes.Clases.LoadRPT rpt = new Constantes.Clases.LoadRPT(IdDocument);
            Constantes.frmVisor form = new Constantes.frmVisor(rpt.DocRPT);
            rpt.GenerarPDF(Document.DocEntry.ToString());
            form.MdiParent = this.MdiParent;
            form.Show();
        }
        #endregion

        private void dgvDatos_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            try
            {
                //if (e.Row.Cells["LineStatus"].Value.ToString() == "C")
                //{
                //    e.Row.Appearance.BackColor = Color.LightGray;
                //}

                if (config.ModeDocument != "Edit")
                {
                    if (e.Row.Cells["Currency"].Value.ToString() == txtMoneda.Text)
                        e.Row.Cells["Rate"].Value = 1;
                    else
                        e.Row.Cells["Rate"].Value = config.Rate;

                    e.Row.Cells["AcctCode"].Value = "5200-100-008";
                    e.Row.Cells["Quantity"].Value = 1;
                }



                /*29-04-16: CORRECCION DE BUG, YA SE PERMITEN CREAR DOCUMENTOS DESDE CERO*/
                //(dgvDatos.DataSource as DataTable).AcceptChanges();
            }
            catch (Exception)
            {
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            //if (btnCrear.Text == "Crear")
            //{
            //    #region Crear
            //    try
            //    {
            //        Cursor.Current = Cursors.WaitCursor;

            //        if (IdDocument == 4)//Traspaso
            //        {
            //            SDK.SDKDatos.DocumentosPrevios sdk = new SDKDatos.DocumentosPrevios();//--Orden de compra
            //            SDKDatos.SDK_OWTR _document = new SDKDatos.SDK_OWTR();
            //            _document.DocNum = txtNumero.Text == string.Empty ? decimal.Zero : Convert.ToDecimal(txtNumero.Text);
            //            _document.DocStatus = "A";
            //            _document.DocDate = DateTime.Now;
            //            _document.Filler = txtFiller.Text;
            //            _document.ToWhsCode = txtToWhsCode.Text;
            //            _document.Comments = txtComments.Text;
            //            _document.U_FolioSolicitud = txtIDSolicitud.Text;
            //            string TipoSolicitud = "01"; //01 anexo, 02 Vta confirmada
            //            if (radioButton1.Checked)
            //                TipoSolicitud = "01";
            //            else if (radioButton2.Checked)
            //                TipoSolicitud = "02";
            //            _document.U_TipoSolicitud = TipoSolicitud;

            //            int lineNum = 0;
            //            foreach (DataRow line in (dgvDatos.DataSource as DataTable).Rows)
            //            {
            //                SDK.SDKDatos.SDK_WTR1 _item = new SDKDatos.SDK_WTR1();
            //                _item.LineNum = lineNum;
            //                _item.ItemCode = Convert.ToString(line["ItemCode"]);
            //                _item.Dscription = Convert.ToString(line["ItemName"]);
            //                _item.WhsCode = Convert.ToString(line["WhsCode"]);
            //                _item.WhsName = Convert.ToString(line["WhsName"]);
            //                _item.FromWhsCode = Convert.ToString(line["FromWhsCode"]);
            //                _item.FromWhsName = Convert.ToString(line["FromWhsName"]);
            //                _item.Quantity = Convert.ToDecimal(line["Quantity"]);
            //                _item.ManBtchNum = Convert.ToString(line["ManBtchNum"]);
            //                _item.BWeight1 = Convert.IsDBNull(line["PesoU"]) ? 0 : Convert.ToDecimal(line["PesoU"]);
            //                _item.BVolume = Convert.IsDBNull(line["VolumenU"]) ? 0 : Convert.ToDecimal(line["VolumenU"]);
            //                _item.U_Tarima = Convert.ToString(line["U_Tarima"]);
            //                _item.U_TipoAlmName = Convert.ToString(line["U_TipoAlmName"]);
            //                _item.U_TipoAlm = Convert.ToString(line["U_TipoAlm"]);

            //                _document.Lines.Add(_item);

            //                lineNum++;
            //            }

            //            txtNumero.Text = sdk.GuardarSolicitudTraslado(_document).ToString();

            //            btnCrear.Enabled = false;
            //            this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            //    }
            //    finally
            //    {
            //        Cursor.Current = Cursors.Arrow;
            //    }
            //    #endregion
            //}

            if (btnCrear.Text == "Crear")//"A Transferencia")
            {
                #region Copiar a Transferencia
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    if (IdDocument == 4)//Traspaso
                    {
                        SDK.SDKDatos.SDK_DI sdk = new SDKDatos.SDK_DI(10006);//--Orden de compra --10006
                        SDKDatos.SDK_OWTR _document = new SDKDatos.SDK_OWTR();
                        _document.DocNum = txtNumero.Text == string.Empty ? decimal.Zero : Convert.ToDecimal(txtNumero.Text);
                        _document.DocStatus = "A";
                        _document.DocDate = DateTime.Now;
                        _document.Filler = txtFiller.Text;
                        _document.ToWhsCode = txtToWhsCode.Text;
                        _document.Comments = txtComments.Text;
                        _document.U_FolioSolicitud = txtIDSolicitud.Text;
                        string TipoSolicitud = "01"; //01 anexo, 02 Vta confirmada
                        if (radioButton1.Checked)
                            TipoSolicitud = "01";
                        else if (radioButton2.Checked)
                            TipoSolicitud = "02";
                        _document.U_TipoSolicitud = TipoSolicitud;
                        

                        int lineNum = 0;
                        foreach (DataRow line in (dgvDatos.DataSource as DataTable).Rows)
                        {
                            SDK.SDKDatos.SDK_WTR1 _item = new SDKDatos.SDK_WTR1();
                            _item.LineNum = lineNum;
                            _item.ItemCode = Convert.ToString(line["ItemCode"]).Trim();
                            _item.Dscription = Convert.ToString(line["ItemName"]).Trim();
                            _item.WhsCode = Convert.ToString(line["WhsCode"]).Trim();
                            _item.WhsName = Convert.ToString(line["WhsName"]).Trim();
                            _item.FromWhsCode = Convert.ToString(line["FromWhsCode"]).Trim();
                            if (lineNum == 0)
                                _document.Filler = Convert.ToString(line["FromWhsCode"]).Trim();
                            _item.FromWhsName = Convert.ToString(line["FromWhsName"]).Trim();
                            _item.Quantity = Convert.ToDecimal(line["Quantity"]);
                            _item.ManBtchNum = Convert.ToString(line["ManBtchNum"]).Trim();
                            _item.BWeight1 = Convert.IsDBNull(line["PesoU"]) ? 0 : Convert.ToDecimal(line["PesoU"]);
                            _item.BVolume = Convert.IsDBNull(line["VolumenU"]) ? 0 : Convert.ToDecimal(line["VolumenU"]);
                            _item.U_Tarima = Convert.ToString(line["U_Tarima"]).Trim();
                            _item.U_TipoAlmName = Convert.ToString(line["U_TipoAlmName"]).Trim();
                            _item.U_TipoAlm = Convert.ToString(line["U_TipoAlm"]).Trim();

                            _document.Lines.Add(_item);

                            lineNum++;
                        }

                        txtNumero.Text = sdk.CrearSolicitudTraslado(_document).ToString();

                        btnCrear.Enabled = false;
                        this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
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
        }

        public void txtNumero_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar == (int)Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtNumero.Text))
                    {
                        //SDK.Documentos.frmListadoDocumentos formulario =
                        //    new frmListadoDocumentos(IdDocument, txtCardCode.Text.Replace('*', '%'),
                        //        txtCardName.Text.Replace('*', '%'), dtpDocDate.Value, txtNumAtCard.Text.Replace('*', '%'));

                        //if (formulario.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                        //    txtFolio.Text = formulario.DocEntry.ToString();
                        //else
                        //{
                        //    //this.CleanScreen();
                        //    return;
                        //}
                    }

                    #region Documento Encontrado
                    (dgvDatos.DataSource as DataTable).Rows.Clear();
                    //SDK.SDKDatos.SDK_OWTR Document = new SDKDatos.SDK_OWTR();

                    Document = Document.Fill_Previo(Convert.ToDecimal(txtNumero.Text), IsSolicitud);

                    #region LLenar Form
                    this.CleanScreen();
                    txtNumero.Text = Document.DocNum.ToString();
                    dtpDocDate.Value = Document.DocDate;
                    txtComments.Text = Document.Comments;
                    txtToWhsCode.Text = Document.ToWhsCode;

                    #region Detalle
                    foreach (SDKDatos.SDK_WTR1 item in Document.Lines)
                    {
                        DataRow row = (dgvDatos.DataSource as DataTable).NewRow();
                        row["ItemCode"] = item.ItemCode;
                        row["ItemName"] = item.Dscription;

                        row["WhsCode"] = item.WhsCode;
                        row["WhsName"] = item.WhsName;
                        row["FromWhsCode"] = item.FromWhsCode;
                        row["FromWhsName"] = item.FromWhsName;

                        row["Quantity"] = item.Quantity;
                        row["ManBtchNum"] = item.ManBtchNum;

                        row["VolumenU"] = item.BVolume;
                        row["PesoU"] = item.BWeight1;
                        row["U_Tarima"] = item.U_Tarima;
                        row["U_TipoAlmName"] = item.U_TipoAlmName;
                        row["U_TipoAlm"] = item.U_TipoAlm;

                        (dgvDatos.DataSource as DataTable).Rows.Add(row);

                        (dgvDatos.DataSource as DataTable).AcceptChanges();
                    }
                    #endregion
                    #endregion
                    #endregion

                    if (!_isSolicitud)
                        this.ModoForm(2);
                    else
                        this.ModoForm(3);
                    //    txtCardName.ReadOnly = true;
                    //    txtCardName.ReadOnly = true;
                }

            }
            catch (Exception ex)
            {
                this.SetMensaje("Error al cargar OC: " + ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Si continua este documento no se podrá pasar a OC en SAP, ¿Desea continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return;
            if (btnCancelar.Text.Equals("Descartar"))
            {
                //using(SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                //{
                //    using (SqlCommand command = new SqlCommand("sp_SDKDocumentosPrevios", connection))
                //    {
                //        command.CommandType = CommandType.StoredProcedure;
                //        command.Parameters.AddWithValue("@TipoConsulta", 6);
                //        command.Parameters.AddWithValue("@DocStatus", "C");
                //        command.Parameters.AddWithValue("@DocNum", txtNumero.Text);

                //        connection.Open();
                //        command.ExecuteNonQuery();
                Document.DocNum = Convert.ToDecimal(txtNumero.Text);
                Document.CambiarStatus(Document, "C");
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                //    }
                //}
            }
        }

        private void dgvDatos_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Activation.NoEdit;
            }
        }
    }
}