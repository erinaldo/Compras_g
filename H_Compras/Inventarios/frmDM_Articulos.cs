using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.Inventarios
{
    public partial class frmDM_Articulos : Constantes.frmEmpty
    {
        DataTable tbl_Articulos = new DataTable();
        Datos.Connection connection = new Datos.Connection();

        public frmDM_Articulos()
        {
            InitializeComponent();
        }

        private void CleanScreen(Control con, bool ReadOnly)
        {
            foreach (Control item in con.Controls)
            {
                if (item is TextBox)
                {
                    ((TextBox)item).Clear();
                    ((TextBox)item).ReadOnly = ReadOnly;
                }
                CleanScreen(item, ReadOnly);
            }

            dgvDatos.DataSource = null;
            cbLinea.SelectedValue = 0;

            txtItemCode.Focus();
        }

        private void frmDM_Articulos_Load(object sender, EventArgs e)
        {
            try
            {
                Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

                nuevoToolStripButton.Enabled = false;
                guardarToolStripButton.Enabled = false;
                actualizarToolStripButton.Enabled = false;
                buscarStripButton.Enabled = true;

                tbl_Articulos = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetArticulos, string.Empty, string.Empty);
                ClasesSGUV.Form.ControlsForms.Autocomplete(txtItemCode, tbl_Articulos.Copy(), "ItemCode");
                this.dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.Copy, true, true);

                ClasesSGUV.Form.ControlsForms.setDataSource(cbLinea, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.LineasTodas, string.Empty, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "--Línea--");

                buscarStripButton.Click -= new EventHandler(btn_Limpiar);
                buscarStripButton.Click += new EventHandler(btn_Limpiar);
            }
            catch (Exception)
            {
            }
        }

        private void txtItemName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.Consultar();
            }
        }

        private void Consultar()
        {
            try
            {
                string itemcode = txtItemCode.Text;

                Object[] valuesOut = new Object[] { };
                DataTable tbl =
                    connection.GetDataTable("LOG",
                                            "su_Almacen",
                                            new string[] { },
                                            new string[] { "@TipoConsulta", "@ItemCode" },
                                            ref valuesOut, 22, itemcode);

                if (tbl.Rows.Count > 0)
                {
                    this.CleanScreen(this, true);

                    txtItemCode.Text = Convert.ToString(tbl.Rows[0]["ItemCode"]);
                    txtItemName.Text = Convert.ToString(tbl.Rows[0]["ItemName"]);
                    txtU_Subclas.Text = Convert.ToString(tbl.Rows[0]["U_Subclas"]);
                    cbLinea.SelectedValue = Convert.ToInt32(tbl.Rows[0]["ItmsGrpCod"]);
                    txtActive.Text = Convert.ToString(tbl.Rows[0]["Estado"]);

                    txtCardName.Text = Convert.ToString(tbl.Rows[0]["CardName"]);
                    txtNumInBuy.Text = Convert.ToDecimal(tbl.Rows[0]["NumInBuy"]).ToString("N2");
                    txtBuyUnitMsr.Text = Convert.ToString(tbl.Rows[0]["BuyUnitMsr"]);
                    txtPurPackUn.Text = Convert.ToDecimal(tbl.Rows[0]["PurPackUn"]).ToString("N2");

                    txtBHeight1.Text = Convert.ToDecimal(tbl.Rows[0]["BHeight1"]).ToString("N2") + " cm";
                    txtBWidth1.Text = Convert.ToDecimal(tbl.Rows[0]["BWidth1"]).ToString("N2") + " cm";
                    txtBLength1.Text = Convert.ToDecimal(tbl.Rows[0]["BLength1"]).ToString("N2") + " cm";
                    txtBVolume.Text = Convert.ToDecimal(tbl.Rows[0]["BVolume"]).ToString("N2");
                    txtBWeight1.Text = Convert.ToDecimal(tbl.Rows[0]["BWeight1"]).ToString("N2") + " kg";

                    txtInvntryUom.Text = Convert.ToString(tbl.Rows[0]["InvntryUom"]);
                    txtU_VLGX_PLN.Text = Convert.ToString(tbl.Rows[0]["U_VLGX_PLN"]);
                    txtU_VLGX_LT.Text = Convert.ToString(tbl.Rows[0]["U_VLGX_LT"]);
                    txtU_VLGX_OF.Text = Convert.ToString(tbl.Rows[0]["U_VLGX_OF"]);

                    DataTable tblStock =
                    connection.GetDataTable("LOG",
                                            "su_Almacen",
                                            new string[] { },
                                            new string[] { "@TipoConsulta", "@ItemCode" },
                                            ref valuesOut, 23, itemcode);

                    dgvDatos.DataSource = tblStock;

                    cbLinea.Enabled = false;
                }
                else
                {
                    SDK.Documentos.frmListadoDocumentos formulario = new SDK.Documentos.frmListadoDocumentos(-4, txtItemCode.Text, txtItemName.Text, DateTime.Now, cbLinea.SelectedValue.ToString());
                    //formulario.MdiParent = this.MdiParent;
                    formulario.ShowDialog();
                    txtItemCode.Text = formulario.ItemCode;
                    if (!string.IsNullOrEmpty(txtItemCode.Text))
                        SendKeys.Send("{ENTER}");
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (var item in dgvDatos.DisplayLayout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }

            e.Layout.Bands[0].Columns["En stock"].Format = "N0";
            e.Layout.Bands[0].Columns["Comprometido"].Format = "N0";
            e.Layout.Bands[0].Columns["Pedido"].Format = "N0";
            //e.Layout.Bands[0].Columns["Costo del artículo"].Format = "N2";

            e.Layout.Bands[0].Columns["En stock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Comprometido"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Pedido"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            //e.Layout.Bands[0].Columns["Costo del artículo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }

        private void btn_Limpiar(object sender, EventArgs e)
        {
            this.CleanScreen(this, true);

            txtItemCode.ReadOnly = false;
            txtItemName.ReadOnly = false;

            cbLinea.Enabled = true;
        }

    }
}
