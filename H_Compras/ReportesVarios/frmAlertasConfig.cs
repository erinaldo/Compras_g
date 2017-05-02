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
    public partial class frmAlertasConfig : Constantes.frmEmpty
    {
        DataTable tblCanal = new DataTable();
        DataTable tblTipos = new DataTable();
        Datos.Connection connection = new Datos.Connection();
        Object[] valuesOut = new Object[] { };


        public frmAlertasConfig()
        {
            InitializeComponent();

           
        }

        private void frmAlertasConfig_Load(object sender, EventArgs e)
        {
            ClasesSGUV.Form.ControlsForms.setDataSource(cbLinea, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Lineas, null, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "--Línea--");

            DataTable tbl_Articulos = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetArticulos, string.Empty, string.Empty);
            ClasesSGUV.Form.ControlsForms.Autocomplete(txtArticulo, tbl_Articulos.Copy(), "ItemCode");

            DataTable tbl =
            connection.GetDataTable("LOG",
                                       "sp_ListaPrecios",
                                       new string[] { },
                                       new string[] { "@TipoConsulta" },
                                       ref valuesOut, 14);

            dgvDatos.DataSource = tbl;
            tblCanal = new DataTable();
            tblTipos = new DataTable();

            tblCanal.Columns.Add("Codigo", typeof(Int32));
            tblCanal.Columns.Add("Nombre", typeof(string));

            DataRow r0 = tblCanal.NewRow();
            r0[0] = 0;
            r0[1] = string.Empty;
            tblCanal.Rows.Add(r0);

            DataRow r1 = tblCanal.NewRow();
            r1[0] = 60;
            r1[1] = "Transporte";
            tblCanal.Rows.Add(r1);

            DataRow r2 = tblCanal.NewRow();
            r2[0] = 61;
            r2[1] = "Mayoreo";
            tblCanal.Rows.Add(r2);

            DataRow r3 = tblCanal.NewRow();
            r3[0] = 62;
            r3[1] = "Armadores";
            tblCanal.Rows.Add(r3);

            cbCanal.DataSource = tblCanal;
            cbCanal.DisplayMember = "Nombre";
            cbCanal.ValueMember = "Codigo";

            tblTipos.Columns.Add("Codigo", typeof(Int32));
            tblTipos.Columns.Add("Nombre", typeof(string));

            DataRow r_0 = tblTipos.NewRow();
            r_0[0] = 0;
            r_0[1] = string.Empty;
            tblTipos.Rows.Add(r_0);

            DataRow r_1 = tblTipos.NewRow();
            r_1[0] = 1;
            r_1[1] = "% Descuento";
            tblTipos.Rows.Add(r_1);

            DataRow r_2 = tblTipos.NewRow();
            r_2[0] = 2;
            r_2[1] = "Utilidad";
            tblTipos.Rows.Add(r_2);

            DataRow r_3 = tblTipos.NewRow();
            r_3[0] = 3;
            r_3[1] = "Precio Especial";
            tblTipos.Rows.Add(r_3);

            //DataRow r_4 = tblTipos.NewRow();
            //r_4[0] = 4;
            //r_4[1] = "Volumen";
            //tblTipos.Rows.Add(r_4);

            cbEvaluar.DataSource = tblTipos;
            cbEvaluar.DisplayMember = "Nombre";
            cbEvaluar.ValueMember = "Codigo";

            nuevoToolStripButton.Click -= new EventHandler(btnNuevo_Click);
            nuevoToolStripButton.Click += new EventHandler(btnNuevo_Click);
        }

        private void cbEvaluar_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //if (cbEvaluar.SelectedIndex == 3)
            //{
            //    lblCurrency.Visible = true;
            //    cbCurrency.Visible = true;
            //}
            //else
            //{
            //    lblCurrency.Visible = false;
            //    cbCurrency.Visible = false;
            //}
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnOk.Text.Equals("Guardar"))
                {
                    connection.Ejecutar("LOG",
                                            "sp_ListaPrecios",
                                            new string[] { },
                                            new string[] { "@TipoConsulta", "@ItmsGrpCod", "@ItemCode", "@Canal", "@Currency", "@Aplicar", "@valorReferencia", "@Nivel", "@vol", "@CardCode"},
                                            ref valuesOut, 13, cbLinea.SelectedValue, txtArticulo.Text, cbCanal.SelectedValue, cbCurrency.Text, cbEvaluar.SelectedValue, txtValor.Text, cbNivel.Text, cbVolumen.Checked, txtCardCode.Text);
                    //DataTable tbl =
                    //  connection.GetDataTable("LOG",
                    //                          "su_Almacen",
                    //                          new string[] { },
                    //                          new string[] { "@TipoConsulta", "@ItemCode" },
                    //                          ref valuesOut, 22, itemcode);
                }
                else if (btnOk.Text.Equals("Actualizar"))
                {
                    connection.Ejecutar("LOG",
                                           "sp_ListaPrecios",
                                           new string[] { },
                                           new string[] { "@TipoConsulta", "@ItmsGrpCod", "@ItemCode", "@Canal", "@Currency", "@Aplicar", "@valorReferencia", "@Nivel", "@vol", "@id", "@Activo", "@CardCode"},
                                           ref valuesOut, 16, cbLinea.SelectedValue, txtArticulo.Text, cbCanal.SelectedValue, cbCurrency.Text, cbEvaluar.SelectedValue, txtValor.Text, cbNivel.Text, cbVolumen.Checked, txtID.Text, txtActivo.Text, txtCardCode.Text);
                  
                }
                this.OnLoad(e);
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            txtActivo.Text = "Y";
            txtValor.Clear();
            txtArticulo.Clear();
            cbVolumen.Checked = false;
            cbLinea.SelectedIndex = 0;
            cbCanal.SelectedIndex = 0;
            cbEvaluar.SelectedIndex = 0;
            cbCurrency.Text = string.Empty;
            cbNivel.SelectedIndex = 0;

            btnOk.Text = "Guardar";
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
        }

        private void dgvDatos_AfterRowActivate(object sender, EventArgs e)
        {
            try
            {
                DataTable tbl = connection.GetDataTable("LOG",
                                             "sp_ListaPrecios",
                                             new string[] { },
                                             new string[] { "@TipoConsulta", "@id"},
                                             ref valuesOut, 15, dgvDatos.ActiveRow.Cells["Id"].Value);

                cbLinea.SelectedValue = tbl.Rows[0]["ItmsGrpCod"];
                txtArticulo.Text = tbl.Rows[0].Field<string>("ItemCode");
                cbCanal.SelectedValue = tbl.Rows[0]["Canal"];
                cbEvaluar.SelectedValue = tbl.Rows[0]["Aplicar"];

                txtValor.Text = tbl.Rows[0].Field<decimal>("ValorReferncia").ToString("N2");
                cbCurrency.Text = tbl.Rows[0].Field<string>("Currency");
                cbNivel.Text = tbl.Rows[0]["Nivel"].ToString();
                cbVolumen.Checked = tbl.Rows[0].Field<bool>("Volumen");
                txtID.Text = tbl.Rows[0].Field<Int32>("Id").ToString();
                txtActivo.Text = tbl.Rows[0].Field<string>("Active").ToString();

                btnOk.Text = "Actualizar";
            }
            catch (Exception)
            {
            }
        }
    }
}
