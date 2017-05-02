using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.Almacen.Entregas
{
    public partial class frmDetalleOV : Constantes.frmEmpty
    {
        int DocEntry;

        public frmDetalleOV(int _docEntry)
        {
            InitializeComponent();

            DocEntry = _docEntry;
        }

        private void frmDetalleOV_Load(object sender, EventArgs e)
        {
            nuevoToolStripButton.Enabled = false;
            guardarToolStripButton.Enabled = false;
            ayudaToolStripButton.Enabled = false;
            actualizarToolStripButton.Enabled = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("su_Almacen", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 5);
                        command.Parameters.AddWithValue("@DocEntry", DocEntry);

                        SqlDataAdapter da = new SqlDataAdapter(command);
                        DataTable tbl = new DataTable();
                        da.Fill(tbl);

                        dgvDatos.DataSource = tbl;

                        string status = (from item in tbl.AsEnumerable()
                                         select item.Field<string>("DocStatus")).FirstOrDefault();

                        btnEntrega.Enabled = status.Equals("O");
                    }
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 10000, Color.Red, Color.White);
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("su_Almacen", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 20);
                        command.Parameters.AddWithValue("@DocEntry", DocEntry);

                        SqlDataAdapter da = new SqlDataAdapter(command);
                        DataTable tbl = new DataTable();
                        da.Fill(tbl);

                       dgvEntregas.DataSource = tbl;
                    }
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 10000, Color.Red, Color.White);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["DocStatus"].Hidden = true;

            e.Layout.Bands[0].Columns["DocEntry"].Hidden = true;
            e.Layout.Bands[0].Columns["ObjType"].Hidden = true;
            e.Layout.Bands[0].Columns["LineNum"].Hidden = true;
            e.Layout.Bands[0].Columns["Series"].Hidden = true;
            e.Layout.Bands[0].Columns["Currency"].Hidden = true;
            e.Layout.Bands[0].Columns["WhsCode"].Hidden = true;
            e.Layout.Bands[0].Columns["Price"].Hidden = true;
            e.Layout.Bands[0].Columns["SalUnitMsr"].Hidden = true;
            e.Layout.Bands[0].Columns["U_PReal"].Hidden = true;
            e.Layout.Bands[0].Columns["U_Comentario"].Hidden = true;
            e.Layout.Bands[0].Columns["ManBtchNum"].Hidden = true;

            e.Layout.Bands[0].Columns["LugarExp"].Hidden = true;
            e.Layout.Bands[0].Columns["Calle"].Hidden = true;
            e.Layout.Bands[0].Columns["NoExt"].Hidden = true;
            e.Layout.Bands[0].Columns["NoInt"].Hidden = true;
            e.Layout.Bands[0].Columns["Colonia"].Hidden = true;
            e.Layout.Bands[0].Columns["Estado"].Hidden = true;
            e.Layout.Bands[0].Columns["CP"].Hidden = true;
            e.Layout.Bands[0].Columns["Municipio"].Hidden = true;

            e.Layout.Bands[0].Columns["U_Cuenta"].Hidden = true;
            e.Layout.Bands[0].Columns["U_MPAGO2"].Hidden = true;
            e.Layout.Bands[0].Columns["U_TipoOV"].Hidden = true;
            e.Layout.Bands[0].Columns["U_Titular"].Hidden = true;
            e.Layout.Bands[0].Columns["U_TPedido"].Hidden = true;
            e.Layout.Bands[0].Columns["U_OC"].Hidden = true;
            e.Layout.Bands[0].Columns["DocCur"].Hidden = true;
            e.Layout.Bands[0].Columns["CardCode"].Hidden = true;
            
            e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["Dscription"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["Surtido"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["Quantity"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["OpenQty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Artículo";
            e.Layout.Bands[0].Columns["Dscription"].Header.Caption = "Descripción";
            e.Layout.Bands[0].Columns["Surtido"].Header.Caption = "Cantidad surtida";
            e.Layout.Bands[0].Columns["Quantity"].Header.Caption = "Cantidad solicitada";
            e.Layout.Bands[0].Columns["OpenQty"].Header.Caption = "Cantidad pendiente";

            e.Layout.Bands[0].Columns["ItemCode"].Width = 150;
            e.Layout.Bands[0].Columns["Dscription"].Width = 300;
            e.Layout.Bands[0].Columns["Surtido"].Width = 90;
            e.Layout.Bands[0].Columns["Quantity"].Width = 90;
            e.Layout.Bands[0].Columns["OpenQty"].Width = 90;

            e.Layout.Bands[0].Columns["Surtido"].Format = "N0";
            e.Layout.Bands[0].Columns["Quantity"].Format = "N0";
            e.Layout.Bands[0].Columns["OpenQty"].Format = "N0";
                    
            e.Layout.Bands[0].Columns["Surtido"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Quantity"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["OpenQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            dgvDatos.KeyPress -= new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);
            dgvDatos.KeyPress += new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);

            dgvDatos.KeyDown -= new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);
            dgvDatos.KeyDown += new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);
        }

        private void dgvDatos_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            e.Row.Height = 35;

            if (Convert.ToInt32(e.Row.Cells["OpenQty"].Value) == 0)
            {
                e.Row.Cells["Seleccionar"].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Row.CellAppearance.BackColor = Color.LightGray;
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void btnEntrega_Click(object sender, EventArgs e)
        {
            try
            {
                btnConsultar.Focus();

                SDK_SAP.Clases.Document doc = new SDK_SAP.Clases.Document();

                DataTable tbl_Datos = new DataTable();
                tbl_Datos = dgvDatos.DataSource as DataTable;
                doc.DocType = "dDocument_Items";
                doc.DocDate = DateTime.Now;
                doc.CardCode = Convert.ToString(tbl_Datos.Rows[0]["CardCode"]);
                doc.DocCurrency = Convert.ToString(tbl_Datos.Rows[0]["DocCur"]);

                doc.U_Cuenta = Convert.ToString(tbl_Datos.Rows[0]["U_Cuenta"]);
                doc.U_MPAGO2 = Convert.ToString(tbl_Datos.Rows[0]["U_MPAGO2"]);
                doc.U_TipoOV = Convert.ToString(tbl_Datos.Rows[0]["U_TipoOV"]);
                doc.U_Titular = Convert.ToString(tbl_Datos.Rows[0]["U_Titular"]);
                doc.U_TPedido = Convert.ToString(tbl_Datos.Rows[0]["U_TPedido"]);
                doc.U_OC = Convert.ToString(tbl_Datos.Rows[0]["U_OC"]);

                doc.Series = Convert.ToInt32(tbl_Datos.Rows[0]["Series"]);

                /*
                doc.U_LugarExp = Convert.ToString(tbl_Heder.Rows[0]["LugarExp"]);
                doc.FE_Calle = Convert.ToString(tbl_Heder.Rows[0]["Calle"]);
                doc.FE_NoExt = Convert.ToString(tbl_Heder.Rows[0]["NoExt"]);
                doc.FE_NoInt = Convert.ToString(tbl_Heder.Rows[0]["NoInt"]);
                doc.FE_Colonia = Convert.ToString(tbl_Heder.Rows[0]["Colonia"]);
                doc.FE_Municipio = Convert.ToString(tbl_Heder.Rows[0]["Municipio"]);
                doc.FE_Estado = Convert.ToString(tbl_Heder.Rows[0]["Estado"]);
                doc.FE_Pais = "México";
                doc.FE_CP = Convert.ToString(tbl_Heder.Rows[0]["CP"]); 
               */

                foreach (DataRow item in tbl_Datos.Rows)
                {
                    if (Convert.ToBoolean(item["Seleccionar"]))
                    {
                        SDK_SAP.Clases.DocumentLines lineDoc = new SDK_SAP.Clases.DocumentLines();
                        lineDoc.ItemCode = Convert.ToString(item["ItemCode"]);
                        lineDoc.Quantity = Convert.ToDouble(item["Surtido"]);
                        lineDoc.Currency = Convert.ToString(item["Currency"]);
                        lineDoc.WarehouseCode = Convert.ToString(item["WhsCode"]);
                        lineDoc.Price = Convert.ToDouble(item["Price"]);
                        lineDoc.MeasureUnit = Convert.ToString(item["SalUnitMsr"]);
                        if (item["U_PReal"] != DBNull.Value)
                            lineDoc.U_PReal = Convert.ToDouble(item["U_PReal"]);
                        if (item["U_Comentario"] != DBNull.Value)
                            lineDoc.U_Comentario = Convert.ToString(item["U_Comentario"]);
                        lineDoc.ManBtchNum = Convert.ToString(item["ManBtchNum"]);

                        SDK_SAP.Clases.Lotes lote = new SDK_SAP.Clases.Lotes();
                        if (lineDoc.ManBtchNum.Equals("Y"))
                        {
                            DataTable tbl_Lot = lote.getLotes(lineDoc.ItemCode, lineDoc.WarehouseCode, lineDoc.Quantity);
                            foreach (DataRow itemLote in tbl_Lot.Rows)
                            {
                                SDK_SAP.Clases.Lotes oLot = new SDK_SAP.Clases.Lotes();
                                oLot.Lote = itemLote.Field<string>("BatchNum");
                                oLot.Cantidad = (double)itemLote.Field<decimal>("Quantity");
                                lineDoc.LotesList.Add(oLot);
                            }
                        }

                        lineDoc.BaseEntry = Convert.ToInt32(item["DocEntry"]);
                        lineDoc.BaseLine = Convert.ToInt32(item["LineNum"]);
                        lineDoc.BaseType = Convert.ToInt32(tbl_Datos.Rows[0]["ObjType"]);

                        doc.Lines.Add(lineDoc);
                    }
                }

                SDK_SAP.DI.Documents diAPI_Document = new SDK_SAP.DI.Documents(20011);

                decimal FolioSap = diAPI_Document.AddDocument("ODLN", doc);

                this.SetMensaje("Listo! Folio: " + FolioSap, 5000, Color.Green, Color.Black);

                this.OnLoad(e);
            }
            catch (Exception ex) 
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
           
        }

        private void dgvEntregas_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            e.Row.Height = 35;

            e.Row.Cells["Print"].ButtonAppearance.Image = Properties.Resources.print;
            e.Row.Cells["Print"].Appearance.ImageBackground = Properties.Resources.print;
            e.Row.Cells["Print"].ButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            e.Row.Cells["Print"].ButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
            e.Row.Cells["Print"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            e.Row.Cells["Print"].ButtonAppearance.BorderColor = Color.CadetBlue;
            e.Row.Cells["Print"].Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Default;
        }

        private void dgvEntregas_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["DocEntry"].Hidden = true;

            e.Layout.Bands[0].Columns["DocNum"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["DocDate"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["Estado"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            e.Layout.Bands[0].Columns["Print"].Width = 50;
            e.Layout.Bands[0].Columns["DocNum"].Width = 80;
            e.Layout.Bands[0].Columns["DocDate"].Width = 80;
            e.Layout.Bands[0].Columns["Estado"].Width = 80;

            e.Layout.Bands[0].Columns["Print"].Header.Caption = "";
            e.Layout.Bands[0].Columns["DocNum"].Header.Caption = "Entrega";
            e.Layout.Bands[0].Columns["DocDate"].Header.Caption = "Fecha";
            e.Layout.Bands[0].Columns["Estado"].Header.Caption = "Estado";
        }

        private void dgvEntregas_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell.Row.Index > -1 && e.Cell.Band.Index == 0)
            {
                if (e.Cell.Column.Header.Caption == "")
                {
                    Constantes.Clases.LoadRPT rpt = new Constantes.Clases.LoadRPT(20011);
                    Constantes.frmVisor form = new Constantes.frmVisor(rpt.DocRPT);
                    //rpt.GenerarPDF(dgvEntregas.Rows[e.Cell.Row.Index].Cells["DocEntry"].Value.ToString());
                    rpt.Print(dgvEntregas.Rows[e.Cell.Row.Index].Cells["DocEntry"].Value.ToString());
                    //form.Show();
                }
            }
        }
    }
}
