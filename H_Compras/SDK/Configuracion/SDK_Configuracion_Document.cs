using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace H_Compras.SDK.Configuracion
{
    public class SDK_Configuracion_Document
    {
        decimal IdDocumento;
        SDK.Documentos.frmDocumentos Form_Document;
        DataTable tblLines_Aux;//configuracion de lineas
        DataTable tbl_Lines;//lineas configuradas
        string _modeDocument;
        decimal _rate;
        string _formulaMXP = string.Empty;
        string _formulaUSD = string.Empty;

        #region Parámetros
        public string ModeDocument
        {
            get { return _modeDocument; }
            set { _modeDocument = value; }
        }

        string vatGroup;

        public string VatGroup
        {
            get { return vatGroup; }
            set { vatGroup = value; }
        }

        public DataTable Tbl_Lines
        {
            get { return tbl_Lines; }
            set { tbl_Lines = value; }
        }
        //DataTable tbl_Source;//origen de datos
        Decimal IVA;

        public Decimal IVA1
        {
            get { return IVA; }
            set { IVA = value; }
        }
        DataTable tbl_Items;

        public decimal Rate
        {
            get { return _rate; }
            set { _rate = value; }
        }
        #endregion

        public enum DocumentType
        {
            OrdenCompra =1
        }

        public SDK_Configuracion_Document(decimal _idDocumento, SDK.Documentos.frmDocumentos form)
        {
            tbl_Lines = new DataTable();
            tbl_Items = new DataTable();
            IdDocumento = _idDocumento;
            Form_Document = form;
        }

        public void Header()
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_SDKDocuments", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 2);
                    command.Parameters.AddWithValue("@idDocumento", IdDocumento);

                    DataTable tbl = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl);

                    foreach (Control item in Form_Document.Controls)
                    {
                        foreach (DataRow row in tbl.Rows)
                        {
                            if (item.Name.Equals(row["nameControl"]))
                            {
                                item.Text = row.Field<string>("textControl");
                                if (item is TextBox)
                                {
                                    ((TextBox)item).ReadOnly = row.Field<bool>("readonlyControl" + _modeDocument);
                                }
                                if (item is DateTimePicker || item is Button)
                                {
                                    item.Enabled = !row.Field<bool>("readonlyControl" + _modeDocument);
                                }
                            }
                        }
                    }
                }
            }

            #region Combo monedas
            DataTable tblMoneda = new DataTable();
            tblMoneda.Columns.Add("code", typeof(string));
            tblMoneda.Columns.Add("name", typeof(string));
            DataRow rv = tblMoneda.NewRow();
            rv[0] = string.Empty;
            rv[1] = string.Empty;
            DataRow rn = tblMoneda.NewRow();
            rn[0] = "$";
            rn[1] = "Peso mexicano";
            DataRow re = tblMoneda.NewRow();
            re[0] = "USD";
            re[1] = "Dólar";
            tblMoneda.Rows.Add(rv);
            tblMoneda.Rows.Add(rn);
            tblMoneda.Rows.Add(re);
            Form_Document.cbMoneda.DataSource = tblMoneda;
            Form_Document.cbMoneda.DisplayMember = "name";
            Form_Document.cbMoneda.ValueMember = "code";
            #endregion
        }

        public void Lines()
        {
            tbl_Lines.Columns.Clear();

            #region Configuracion tabla
            tblLines_Aux = new DataTable();
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_SDKDocuments", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 3);
                    command.Parameters.AddWithValue("@idDocumento", IdDocumento);

                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tblLines_Aux);

                    Form_Document.dgvDatos.InitializeLayout -= new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(dgvDatos_InitializeLayout);
                    Form_Document.dgvDatos.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(dgvDatos_InitializeLayout);

                    Form_Document.ultraDropDown1.InitializeLayout -= new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(dgvCombo_InitializeLayout);
                    Form_Document.ultraDropDown1.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(dgvCombo_InitializeLayout);

                    Form_Document.dgvDatos.AfterCellListCloseUp -= new CellEventHandler(dgvDatos_AfterCellListCloseUp);
                    Form_Document.dgvDatos.AfterCellListCloseUp += new CellEventHandler(dgvDatos_AfterCellListCloseUp);

                    Form_Document.dgvDatos.AfterCellUpdate -= new Infragistics.Win.UltraWinGrid.CellEventHandler(dgvDatos_AfterCellUpdate);
                    Form_Document.dgvDatos.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(dgvDatos_AfterCellUpdate);
                }
            }

            tbl_Lines.Clear();
            string curr = Form_Document.txtMoneda.Text == "USD" ? "USD" : string.Empty;
            foreach (DataRow row in tblLines_Aux.Rows)
            {
                DataColumn column = new DataColumn();
                column.ColumnName = row.Field<string>("nameColumn");
                column.DataType = System.Type.GetType(row.Field<string>("typeColumn"));
                if (!string.IsNullOrEmpty(row.Field<string>("formulaColumn" + curr)))
                    column.Expression = row.Field<string>("formulaColumn" + curr);
                tbl_Lines.Columns.Add(column);
            }


            Form_Document.dgvDatos.DataSource = tbl_Lines;
            #endregion

            #region Autocompletables
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_SDKDataSource", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 3);

                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl_Items);

                    _rate = (from item in tbl_Items.AsEnumerable()
                            select item.Field<decimal>("Rate")).FirstOrDefault();

                    Form_Document.ultraDropDown1.SetDataBinding(tbl_Items, null);
                    Form_Document.ultraDropDown1.ValueMember = "ItemCode";
                    Form_Document.ultraDropDown1.DisplayMember = "ItemCode";
                }
            }
            #endregion
        }

        public void StartEmpty()
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_SDKDocuments", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 1);
                    command.Parameters.AddWithValue("@idDocumento", IdDocumento);

                    DataTable tbl = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl);

                    Form_Document.Text = tbl.Rows[0].Field<string>("Nombre");
                    IVA = tbl.Rows[0].Field<decimal>("IVA");
                }
            }

            this.Header();
            this.Lines();
        }

        public void StartFill(int _idDocument, DataTable tblSource, string key)
        {
            //tbl_Source = tblSource;
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_SDKDocuments", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 1);
                    command.Parameters.AddWithValue("@idDocumento", IdDocumento);

                    DataTable tbl = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl);

                    Form_Document.Text = tbl.Rows[0].Field<string>("Nombre");
                    IVA = tbl.Rows[0].Field<decimal>("IVA");
                }
            }

            this.Header();
            this.Lines();

            #region Llenar encabezado
            Form_Document.txtCardCode.Text = key;

            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_SDKDataSource", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 2);
                    command.Parameters.AddWithValue("@key", key);//cardcode

                    DataTable tbl = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl);

                    Form_Document.txtCardName.Text = tbl.Rows[0].Field<string>("CardName");
                    VatGroup = tbl.Rows[0].Field<string>("VatGroup");

                    if (tbl.Rows[0].Field<string>("Currency").Equals("##"))
                        Form_Document.cbMoneda.Enabled = true;
                    else
                    {
                        Form_Document.cbMoneda.Enabled = false;
                        Form_Document.txtMoneda.Text = tbl.Rows[0].Field<string>("Currency");
                        Form_Document.cbMoneda.SelectedValue = tbl.Rows[0].Field<string>("Currency");
                    }
                }
            }

            #endregion

            #region Llenar detalle
            if (IdDocumento == (int)DocumentType.OrdenCompra)
            {
                DataTable details = new DataTable();
                var result = from item in tblSource.AsEnumerable()
                             where item.Field<decimal>("Autorizado") > 0
                             select item;
                if (result.Count() == 0)
                {
                    //Error
                    return;
                }

                details = result.CopyToDataTable();

                foreach (DataRow item in details.Rows)
                {
                    DataRow row = tbl_Lines.NewRow();
                
                    row["ItemCode"] = item["Artículo"];
                    row["ItemName"] = item["Descripción"];
                    row["WhsCode"] = item["WhsCode"];
                    row["WhsName"] = item["WhsCode"] + " | " + item["Almacén"];
                    row["Quantity"] = item["Autorizado"];
                    row["Price"] = item["Price"];
                   
                    row["Currency"] = item["Currency"];
                    row["Rate"] = "USD".Equals(item["Currency"].ToString()) ? Convert.ToDecimal(item["Rate"])
                       : decimal.Zero;

                    #region Line Total

                    if (row["Currency"].ToString().Equals(Form_Document.txtMoneda.Text))
                    {
                        row["LineTotal"] =
                            Convert.ToDecimal(row["Price"])
                            *
                            Convert.ToDecimal(row["Quantity"]);
                    }
                    else
                    {
                        if (row["Currency"].ToString().Equals("USD") & Form_Document.txtMoneda.Text.Equals("$"))
                        {
                            row["LineTotal"] =
                                (Convert.ToDecimal(row["Price"])
                                *
                                Convert.ToDecimal(row["Quantity"])) * Form_Document.Rate;
                        }
                        if (row["Currency"].ToString().Equals("$") & Form_Document.txtMoneda.Text.Equals("USD"))
                        {
                            row["LineTotal"] =
                                (Convert.ToDecimal(row["Price"])
                                *
                                Convert.ToDecimal(row["Quantity"])) / Form_Document.Rate;
                        }
                    }
                   
                    #endregion
                    row["VolumenU"] = item["Volumen"];
                    row["PesoU"] = item["BWeight1"];

                    row["OnHand"] = (from i in tbl_Items.AsEnumerable()
                                     where i.Field<string>("ItemCode").Equals(row["ItemCode"].ToString())
                                     select i.Field<decimal>("OnHand")).FirstOrDefault();
                    row["Ideal"] = (from i in tbl_Items.AsEnumerable()
                                     where i.Field<string>("ItemCode").Equals(row["ItemCode"].ToString())
                                    select i.Field<decimal>("Ideal")).FirstOrDefault();
                    row["ABC"] = (from i in tbl_Items.AsEnumerable()
                                    where i.Field<string>("ItemCode").Equals(row["ItemCode"].ToString())
                                    select i.Field<string>("Clasificacion")).FirstOrDefault();
                                    
                   // row["Ideal"] = item["BWeight1"];

                    tbl_Lines.Rows.Add(row);

                    tbl_Lines.AcceptChanges();
                }
            }
            #endregion
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
            {
                foreach (DataRow row in tblLines_Aux.Rows)
                {
                    if (item.Key.Equals(row["nameColumn"]))
                    {
                        item.Header.Caption = row.Field<string>("dscriptionColumn");
                        item.Width = row.Field<int>("widthColumn");
                        if (row.Field<bool>("readonlyColumn" + _modeDocument))
                            item.CellActivation = Activation.NoEdit;
                        else
                            item.CellActivation = Activation.AllowEdit;

                        item.Hidden = !row.Field<bool>("visibleColumn");
                        item.Format = row.Field<string>("formatColumn");

                        if (row.Field<string>("alineacionColumn").Equals("left"))
                            item.CellAppearance.TextHAlign = HAlign.Left;
                        else if (row.Field<string>("alineacionColumn").Equals("rigth"))
                            item.CellAppearance.TextHAlign = HAlign.Right;
                        else if (row.Field<string>("alineacionColumn").Equals("center"))
                            item.CellAppearance.TextHAlign = HAlign.Center;

                        if (row.Field<bool>("listColumn"))
                        {
                            #region Crear List
                            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                            {
                                using (SqlCommand command = new SqlCommand("sp_SDKDataSource", connection))
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.AddWithValue("@TipoConsulta", row.Field<int>("TipoConsulta"));

                                    DataTable tbl = new DataTable();
                                    SqlDataAdapter da = new SqlDataAdapter();
                                    da.SelectCommand = command;
                                    da.Fill(tbl);
                                    
                                    if (row.Field<string>("nameColumn").Equals("ItemCode"))
                                    {
                                        item.ValueList = this.Form_Document.ultraDropDown1;
                                    }
                                    else
                                    {
                                        ValueList vl;
                                        if (!e.Layout.ValueLists.Exists(row.Field<string>("nameColumn")))
                                        {
                                            vl = e.Layout.ValueLists.Add(row.Field<string>("nameColumn"));
                                            int num = 1;
                                            foreach (DataRow list in tbl.Rows)
                                            {
                                                vl.ValueListItems.Add(num, list.Field<string>("code") + " | " + list.Field<string>("name"));
                                                num++;
                                            }
                                        }
                                        item.ValueList = e.Layout.ValueLists[row.Field<string>("nameColumn")];
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                }
            }
            #region suma LineTotal, Volumen, Peso
            Infragistics.Win.UltraWinGrid.UltraGridLayout layout = e.Layout;
            Infragistics.Win.UltraWinGrid.UltraGridBand band = layout.Bands[0];

            e.Layout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed;
            e.Layout.Override.SummaryFooterCaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            e.Layout.Bands[0].Summaries.Clear();

            Infragistics.Win.UltraWinGrid.SummarySettings summary3 = band.Summaries.Add("VOL", Infragistics.Win.UltraWinGrid.SummaryType.Sum, band.Columns["Volumen"]);
            Infragistics.Win.UltraWinGrid.SummarySettings summary2 = band.Summaries.Add("PESO", Infragistics.Win.UltraWinGrid.SummaryType.Sum, band.Columns["Peso"]);
            Infragistics.Win.UltraWinGrid.SummarySettings summary1 = band.Summaries.Add("Total", Infragistics.Win.UltraWinGrid.SummaryType.Sum, band.Columns["LineTotal"]);

            summary2.DisplayFormat = "{0:N1} Kg";
            summary2.Appearance.TextHAlign = HAlign.Right;
            //summary2.Appearance.FontData.Bold = DefaultableBoolean.True;

            summary3.DisplayFormat = "{0:N1} ft3";
            summary3.Appearance.TextHAlign = HAlign.Right;
            //summary3.Appearance.FontData.Bold = DefaultableBoolean.True;

            summary1.Appearance.BackColor = Color.White;
            summary1.Appearance.ForeColor = Color.White;
            #endregion
        }

        private void dgvDatos_AfterCellListCloseUp(object sender, CellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Index > -1)
                {
                    //cuando sean columa ValueList anteponer una columa oculta para almacener el ID a almacenar en la BD
                    if (e.Cell.Column.Key.Equals("SlpCode") || e.Cell.Column.Key.Equals("WhsName"))
                        Form_Document.dgvDatos.Rows[e.Cell.Row.Index].Cells[e.Cell.Column.Index - 1].Value = e.Cell.Text.Substring(0, e.Cell.Text.IndexOf('|'));

                    if (e.Cell.Column.Key.Equals("ItemCode"))
                    {
                        Form_Document.dgvDatos.Rows[e.Cell.Row.Index].Cells["ItemName"].Value = Form_Document.ultraDropDown1.SelectedRow.Cells["ItemName"].Value;
                        Form_Document.dgvDatos.Rows[e.Cell.Row.Index].Cells["Price"].Value = Form_Document.ultraDropDown1.SelectedRow.Cells["Price"].Value;
                        Form_Document.dgvDatos.Rows[e.Cell.Row.Index].Cells["Currency"].Value = Form_Document.ultraDropDown1.SelectedRow.Cells["Currency"].Value;
                       
                        Form_Document.dgvDatos.Rows[e.Cell.Row.Index].Cells["Rate"].Value =
                           "USD".Equals(Form_Document.ultraDropDown1.SelectedRow.Cells["Currency"].Value) ? Convert.ToDecimal(Form_Document.Rate)
                       : decimal.Zero;

                        Form_Document.dgvDatos.Rows[e.Cell.Row.Index].Cells["OnHand"].Value = Form_Document.ultraDropDown1.SelectedRow.Cells["OnHand"].Value;
                        Form_Document.dgvDatos.Rows[e.Cell.Row.Index].Cells["Ideal"].Value = Form_Document.ultraDropDown1.SelectedRow.Cells["Ideal"].Value;
                        Form_Document.dgvDatos.Rows[e.Cell.Row.Index].Cells["ABC"].Value = Form_Document.ultraDropDown1.SelectedRow.Cells["Clasificacion"].Value;
                    }
                }
            }
            catch (Exception) { }
        }

        private void dgvCombo_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            //e.Layout.Bands[0].Columns[1].Header.Caption = "Artículo";
            //e.Layout.Bands[0].Columns[1].Width = 200;
            //e.Layout.Bands[0].Columns[2].Hidden = true;
            //e.Layout.Bands[0].Columns[3].Hidden = true;
            //e.Layout.Bands[0].Columns[4].Hidden = true;
            //e.Layout.Bands[0].Columns[5].Hidden = true;
        }

        private void dgvDatos_AfterCellUpdate(object sender, CellEventArgs e)
        {
            try
            {
                this.dgvDatos_AfterCellListCloseUp(sender, e);

                #region Line Total
                if (e.Cell.Column.Index != Form_Document.dgvDatos.DisplayLayout.Bands[0].Columns["LineTotal"].Index)
                {
                    if (Form_Document.dgvDatos.ActiveRow.Cells["Currency"].Value.ToString().Equals(Form_Document.txtMoneda.Text))
                    {
                        Form_Document.dgvDatos.ActiveRow.Cells["LineTotal"].Value =
                            Convert.ToDecimal(Form_Document.dgvDatos.ActiveRow.Cells["Price"].Value)
                            *
                            Convert.ToDecimal(Form_Document.dgvDatos.ActiveRow.Cells["Quantity"].Value);
                    }
                    else
                    {
                        if (Form_Document.dgvDatos.ActiveRow.Cells["Currency"].Value.ToString().Equals("USD") & Form_Document.txtMoneda.Text.Equals("$"))
                        {
                            Form_Document.dgvDatos.ActiveRow.Cells["LineTotal"].Value =
                                (Convert.ToDecimal(Form_Document.dgvDatos.ActiveRow.Cells["Price"].Value)
                                *
                                Convert.ToDecimal(Form_Document.dgvDatos.ActiveRow.Cells["Quantity"].Value)) * Form_Document.Rate;
                        }
                        if (Form_Document.dgvDatos.ActiveRow.Cells["Currency"].Value.ToString().Equals("$") & Form_Document.txtMoneda.Text.Equals("USD"))
                        {
                            Form_Document.dgvDatos.ActiveRow.Cells["LineTotal"].Value =
                                (Convert.ToDecimal(Form_Document.dgvDatos.ActiveRow.Cells["Price"].Value)
                                *
                                Convert.ToDecimal(Form_Document.dgvDatos.ActiveRow.Cells["Quantity"].Value)) / Form_Document.Rate;
                        }
                    }
                }
                #endregion
            }
            catch (Exception)
            {

            }
        }
    }
}
