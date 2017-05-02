using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.ReportesVarios
{
    public partial class frmBackorder : Constantes.frmEmpty
    {
        public frmBackorder()
        {
            InitializeComponent();
        }
        public enum Column
        {
            Docnum, Cardcode, cardname, linenum, itemcode, linestatus, price, currency, openqty, whscode, docdate, shipdate, quantity, dias, num, p, status, stock
        }

        private void frmBackorder_Load(object sender, EventArgs e)
        {
            try
            {
                this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                dgvDatos.DataSource = null;

                ClasesSGUV.Form.ControlsForms.setDataSource(cbProveedor, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Proveedores, null, string.Empty), "CardName", "CardCode", "---Selecciona un proveedor---");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbComprador, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.PlaneadoresCompra, null, string.Empty), "U_VLGX_PLC", "U_VLGX_PLC", "Todos");

                nuevoToolStripButton.Click -= new EventHandler(btnNuevo_Click);
                nuevoToolStripButton.Click += new EventHandler(btnNuevo_Click);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("sp_Reportes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 2);
                        command.Parameters.AddWithValue("@Desde", dtpDesde.Value);
                        command.Parameters.AddWithValue("@Hasta", dtpHasta.Value);
                        command.Parameters.AddWithValue("@CardCode", cbProveedor.SelectedValue);
                        command.Parameters.AddWithValue("@Compradores", cbComprador.SelectedValue);

                        System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter();
                        da.SelectCommand = command;
                        DataTable table = new DataTable();
                        da.Fill(table);

                        dgvDatos.DataSource = table;
                    }
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.Black);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy;

                foreach (var item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                }

                e.Layout.Bands[0].Columns[(int)Column.price].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Column.quantity].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Column.stock].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Column.openqty].Format = "N0";
                e.Layout.Bands[0].Columns[(int)Column.p].Format = "P2";

                e.Layout.Bands[0].Columns[(int)Column.price].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Column.quantity].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Column.openqty].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Column.p].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Column.dias].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[(int)Column.stock].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

                e.Layout.Bands[0].Columns[(int)Column.linestatus].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Column.price].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Column.currency].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Column.linenum].Hidden = true;
                e.Layout.Bands[0].Columns[(int)Column.p].Hidden = true;

                e.Layout.Bands[0].Columns[(int)Column.num].Hidden = true;

                e.Layout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            }
            catch (Exception)
            {
            }

        }

        private void dgvDatos_DoubleClickCell(object sender, Infragistics.Win.UltraWinGrid.DoubleClickCellEventArgs e)
        {
            try
            {
                Int32 docEntry = Convert.ToInt32(dgvDatos.Rows[e.Cell.Row.Index].Cells[(int)Column.Docnum].Value);

                SDK.Documentos.frmDocumentos formulario = new SDK.Documentos.frmDocumentos(1);
                formulario.MdiParent = this.MdiParent;
                formulario.txtFolio.Text = docEntry.ToString();
                var kea = new KeyPressEventArgs(Convert.ToChar(13));
                formulario.txtFolio_KeyPress(sender, kea);
                formulario.Show();
            }
            catch (Exception)
            {

            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            this.frmBackorder_Load(sender, e);
        }

        private void dgvDatos_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if (Convert.ToInt32(e.Row.Cells[(int)Column.dias].Value) > 30)
            {
                e.Row.Cells[(int)Column.dias].Appearance.BackColor = Color.Red;
                e.Row.Cells[(int)Column.dias].Appearance.ForeColor = Color.White;
            }

            //if (Convert.ToInt32(e.Row.Cells[(int)Column.num].Value) > 1)
            //{
            //    e.Row.Cells[(int)Column.itemcode].Appearance.BackColor = Color.Green;
            //    e.Row.Cells[(int)Column.itemcode].Appearance.ForeColor = Color.Black;
            //}
        }

        private void dgvDatos_BeforeColumnChooserDisplayed(object sender, Infragistics.Win.UltraWinGrid.BeforeColumnChooserDisplayedEventArgs e)
        {
            
        }

        private void dgvDatos_AfterColPosChanged(object sender, Infragistics.Win.UltraWinGrid.AfterColPosChangedEventArgs e)
        {
        }
    }
}
