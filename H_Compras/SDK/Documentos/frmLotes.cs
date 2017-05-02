using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.SDK.Documentos
{
    public partial class frmLotes : Constantes.frmEmpty
    {
        SDK_SAP.Clases.Document Document = new SDK_SAP.Clases.Document();

        DataTable tbl_documets = new DataTable();
        DataTable tbl_lotes = new DataTable();

        public DataTable Tbl_lotes
        {
            get { return tbl_lotes; }
            set { tbl_lotes = value; }
        }

        public frmLotes(SDK_SAP.Clases.Document doc, DataTable tbl, bool qty = false)
        {
            InitializeComponent();

            Document = doc;
            tbl_lotes = tbl.Copy();

            tbl_documets.Columns.Add("DocNum", typeof(double));
            tbl_documets.Columns.Add("ID", typeof(string));
            tbl_documets.Columns.Add("ItemCode", typeof(string));
            tbl_documets.Columns.Add("ItemName", typeof(string));
            tbl_documets.Columns.Add("WhsCode", typeof(string));
            tbl_documets.Columns.Add("Quantity", typeof(double));
            tbl_documets.Columns.Add("Total Creado", typeof(double));

            #region Articulos que necesitan Lotes
            foreach (var item in Document.Lines)
            {
                if (item.ManBtchNum.Equals("Y"))
                {
                    DataRow row = tbl_documets.NewRow();
                    string id = item.ItemCode +
                                item.WarehouseCode +
                                (item.BaseEntry == null ? 0 : item.BaseEntry) +
                                (item.BaseLine == null ? 0 : item.BaseLine);
                    row["ID"] = id;
                    row["ItemCode"] = item.ItemCode;
                    row["ItemName"] = item.ItemDescription;

                    row["WhsCode"] = item.WarehouseCode;
                    if (!qty)
                        row["Quantity"] = item.Facturado * item.NumPerMsr;
                    else
                        row["Quantity"] = item.Quantity * item.NumPerMsr;

                    if (tbl_lotes.Columns.Count > 0)
                        row["Total Creado"] = (from l in tbl_lotes.AsEnumerable()
                                               where l.Field<string>("ID").Equals(id)
                                               select l.Field<double>("Cantidad")).Sum();

                    tbl_documets.Rows.Add(row);
                }
            }
            #endregion

            #region Lotes
            if (tbl.Columns.Count <= 0)
            {
                tbl_lotes.Columns.Add("ID", typeof(string));
                tbl_lotes.Columns.Add("Lote", typeof(string));
                tbl_lotes.Columns.Add("Cantidad", typeof(double));
                tbl_lotes.Columns.Add("Pedimento", typeof(string));
                tbl_lotes.Columns.Add("Aduana", typeof(string));
                tbl_lotes.Columns.Add("Fecha de admisión", typeof(DateTime));

                foreach (var item in Document.Lines)
                {
                    if (item.ManBtchNum.Equals("Y"))
                    {
                        foreach (var lote in item.LotesList)
                        {
                            DataRow row = tbl_lotes.NewRow();
                            row["ID"] = item.ItemCode + item.WarehouseCode + "" +
                                        (item.BaseEntry == null ? 0 : item.BaseEntry) + "" +
                                        (item.BaseLine == null ? 0 : item.BaseLine);
                            row["Lote"] = lote.Lote;
                            row["Cantidad"] = lote.Cantidad;
                            row["Pedimento"] = lote.Pedimento;
                            row["Aduana"] = lote.Aduana;
                            row["Fecha de admisión"] = lote.FechaAdmision;

                            tbl_lotes.Rows.Add(row);
                        }
                    }
                }
            }
            else
            {
                foreach (var item in Document.Lines)
                {
                    if (item.ManBtchNum.Equals("Y"))
                    {
                        foreach (var lote in item.LotesList)
                        {
                            DataRow row = tbl_lotes.NewRow();
                            row["ID"] = item.ItemCode + item.WarehouseCode + "" +
                                        (item.BaseEntry == null ? 0 : item.BaseEntry) + "" +
                                        (item.BaseLine == null ? 0 : item.BaseLine);
                            row["Lote"] = lote.Lote;
                            row["Cantidad"] = lote.Cantidad;
                            row["Pedimento"] = lote.Pedimento;
                            row["Aduana"] = lote.Aduana;
                            row["Fecha de admisión"] = lote.FechaAdmision;

                            tbl_lotes.Rows.Add(row);
                        }
                    }
                }
            }
            #endregion
        }

        private DataTable Lotes(string ItemCode, string DistNumber)
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM [dbo].[fu_DistNumberExist] (@ItemCode, @DistNumber)", connection))
                {
                    command.Parameters.AddWithValue("@ItemCode", ItemCode);
                    command.Parameters.AddWithValue("@DistNumber", DistNumber);

                    DataTable table = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(table);

                    return table;
                }
            }
        }

        private void frmLotes_Load(object sender, EventArgs e)
        {
            try
            {
                nuevoToolStripButton.Enabled = false;
                guardarToolStripButton.Enabled = false;
                exportarToolStripButton.Enabled = false;
                ayudaToolStripButton.Enabled = false;
                actualizarToolStripButton.Enabled = false;

                tbl_documets.TableName = "document";
                tbl_lotes.TableName = "lotes";
                DataSet ds = new DataSet();
                ds.Tables.Add(tbl_documets);
                ds.Tables.Add(tbl_lotes);

                DataRelation relation = new DataRelation("relacion", ds.Tables["document"].Columns["ID"], ds.Tables["lotes"].Columns["ID"]);
                ds.Relations.Add(relation);

                BindingSource masterBindingSource = new BindingSource();
                BindingSource detailsBindingSource = new BindingSource();
                masterBindingSource.DataSource = ds;
                masterBindingSource.DataMember = "document";
                detailsBindingSource.DataSource = masterBindingSource;
                detailsBindingSource.DataMember = "relacion";
                dgvDatos.DataSource = masterBindingSource;
                dgvLotes.DataSource = detailsBindingSource;

                this.dgvDatos.DisplayLayout.Override.ExpansionIndicator = Infragistics.Win.UltraWinGrid.ShowExpansionIndicator.Never;
                                
                this.dgvLotes.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
                this.dgvLotes.DisplayLayout.Bands[0].AddNew();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            foreach (var line in Document.Lines)
            {
                line.LotesList.Clear();

                foreach (DataRow lote in tbl_lotes.Rows)
                {
                    if ((line.ItemCode + 
                         line.WarehouseCode + 
                         (line.BaseEntry == null ? 0 : line.BaseEntry) + 
                         (line.BaseLine == null ? 0 : line.BaseLine)).Equals(lote.Field<string>("ID")))
                    {
                        SDK_SAP.Clases.Lotes oLote = new SDK_SAP.Clases.Lotes();
                        oLote.Lote = lote.Field<string>("Lote");
                        oLote.Cantidad = lote.Field<double>("Cantidad");
                        oLote.Pedimento = lote.Field<string>("Pedimento");
                        oLote.Aduana = lote.Field<string>("Aduana");
                        oLote.FechaAdmision = lote.Field<DateTime>("Fecha de admisión");

                        line.LotesList.Add(oLote);

                        this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
                    }
                }
            }
            
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void dgvLotes_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            try
            {
                e.Row.Cells["ID"].Value = dgvDatos.ActiveRow.Cells["ID"].Value;// +"" + dgvDatos.ActiveRow.Cells["WhsCode"].Value;

                if (e.Row.Cells["Cantidad"].Value == DBNull.Value)
                    e.Row.Cells["Cantidad"].Value = dgvDatos.ActiveRow.Cells["Quantity"].Value;
                if (e.Row.Cells["Fecha de admisión"].Value == DBNull.Value)
                    e.Row.Cells["Fecha de admisión"].Value = DateTime.Now;
            }
            catch (Exception)
            {
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["ID"].Hidden = true;
            e.Layout.Bands[0].Columns["DocNum"].Hidden = true;
            e.Layout.Bands[0].Columns["ItemName"].Width = 300;
            e.Layout.Bands[0].Columns["Quantity"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Total Creado"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["WhsCode"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
            e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Artículo";
            e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Descripción";
            e.Layout.Bands[0].Columns["WhsCode"].Header.Caption = "Código de almacén";
            e.Layout.Bands[0].Columns["WhsCode"].Width = 80;
            e.Layout.Bands[0].Columns["Quantity"].Header.Caption = "Total necesitado";
        }

        private void dgvLotes_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["ID"].Hidden = true;
        }

        private void dgvLotes_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            try
            {
                dgvLotes.Rows[e.Cell.Row.Index].Cells["ID"].Value = dgvDatos.ActiveRow.Cells["ID"].Value;

                if(e.Cell.Column.Index == 1) //si es columna de lotes, valida existencia del Lote, si existe llenar campos Aduana|Pedimento|Fecha
                {
                    if (!string.IsNullOrEmpty(e.Cell.Value.ToString()))
                    {
                        DataTable tbl = this.Lotes(dgvDatos.ActiveRow.Cells["ItemCode"].Value.ToString(), e.Cell.Value.ToString());

                   if(tbl.Rows.Count> 0)
                        {
                            if (MessageBox.Show("Lote ya existe. ¿Continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                == System.Windows.Forms.DialogResult.Yes)
                            {
                                dgvLotes.Rows[e.Cell.Row.Index].Cells["Pedimento"].Value = tbl.Rows[0]["MnfSerial"];
                                dgvLotes.Rows[e.Cell.Row.Index].Cells["Aduana"].Value = tbl.Rows[0]["LotNumber"];
                                dgvLotes.Rows[e.Cell.Row.Index].Cells["Fecha de admisión"].Value = tbl.Rows[0]["InDate"];

                                dgvLotes.Rows[e.Cell.Row.Index].Cells["Pedimento"].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                                dgvLotes.Rows[e.Cell.Row.Index].Cells["Aduana"].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                                dgvLotes.Rows[e.Cell.Row.Index].Cells["Fecha de admisión"].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                            }
                            else
                            {
                                dgvLotes.Rows[e.Cell.Row.Index].Cells["Pedimento"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
                                dgvLotes.Rows[e.Cell.Row.Index].Cells["Aduana"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
                                dgvLotes.Rows[e.Cell.Row.Index].Cells["Fecha de admisión"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void dgvLotes_AfterRowUpdate(object sender, Infragistics.Win.UltraWinGrid.RowEventArgs e)
        {

        }
    }
}
