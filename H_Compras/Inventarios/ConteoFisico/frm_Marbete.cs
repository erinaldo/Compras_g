using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace H_Compras.Inventarios.ConteoFisico
{
    public partial class frm_Marbete : Constantes.frmEmpty
    {
        DataTable tbl_Items = new DataTable();

        public frm_Marbete()
        {
            InitializeComponent();
        }

        private void frm_Marbete_Load(object sender, EventArgs e)
        {
            try
            {
                Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

               ClasesSGUV.Form.ControlsForms.setDataSource(cbLinea, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Lineas, null, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "Todas");

                dgvDatos.DisplayLayout.Bands[0].AddNew();

                dgvDatos.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

                this.dgvDatos.UpdateMode = UpdateMode.OnRowChange;

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

                dgvDatos.KeyPress -= new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);
                dgvDatos.KeyPress += new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);

                dgvDatos.KeyDown -= new System.Windows.Forms.KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);
                dgvDatos.KeyDown += new System.Windows.Forms.KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);
                #endregion

                #region Toolstrip
                this.exportarToolStripButton.Enabled = false;
                this.imprimirToolStripButton.Enabled = true;
                this.actualizarToolStripButton.Enabled = false;
                this.guardarToolStripButton.Enabled = false;

                this.imprimirToolStripButton.Click -= new EventHandler(btn_Print);
                this.imprimirToolStripButton.Click += new EventHandler(btn_Print);

                this.nuevoToolStripButton.Click += new EventHandler(btn_Nuevo);
                this.nuevoToolStripButton.Click += new EventHandler(btn_Nuevo);

                // btn_Nuevo(sender, e);
                #endregion

                #region Autocompletables
                tbl_Items = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ItemsConteoInventario, "01", string.Empty);

                ultraCombo1.SetDataBinding(tbl_Items.Copy(), null);
                ultraCombo1.ValueMember = "ItemCode";
                ultraCombo1.DisplayMember = "ItemCode";

                ultraCombo2.SetDataBinding(tbl_Items.Copy(), null);
                ultraCombo2.ValueMember = "ItemName";
                ultraCombo2.DisplayMember = "ItemName";

                ////ultraCombo1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[1].Width = 300;

            e.Layout.Bands[0].Columns[0].Header.Caption = "Artículo";
            e.Layout.Bands[0].Columns[1].Header.Caption = "Descripción";
            e.Layout.Bands[0].Columns[2].Header.Caption = "Línea";

            e.Layout.Bands[0].Columns[2].CellActivation = Activation.NoEdit;

            e.Layout.Bands[0].Columns["ItemCode"].ValueList = ultraCombo1;
            e.Layout.Bands[0].Columns["ItemName"].ValueList = ultraCombo2;
        }

        private void btn_Print(object sender, EventArgs e)
        {
            if (dgvDatos.Rows.Count <= 0)
                return;

            string path = string.Empty;
            if (rb4.Checked) path = @"\\192.168.2.100\HalcoNET\Crystal\rpt_marbete_4.rpt";
            if (rb10.Checked) path = @"\\192.168.2.100\HalcoNET\Crystal\rpt_marbete_10.rpt";

            string itms = string.Empty;

            foreach (UltraGridRow item in dgvDatos.Rows)
            {
                itms += "'" + item.Cells["ItemCode"].Value + "',";
            }

            Constantes.Clases.LoadRPT rpt = new Constantes.Clases.LoadRPT(path);
            Constantes.frmVisor form = new Constantes.frmVisor(rpt.DocRPT);
            rpt.SetParameter(itms.Trim(','));
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void btn_Nuevo(object sender, EventArgs e)
        {
            try
            {
                this.dgvDatos.Selected.Rows.AddRange((UltraGridRow[])this.dgvDatos.Rows.All);
                this.dgvDatos.DeleteSelectedRows(false);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void ultraCombo1_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Bands[0].Columns["WhsCode"].Hidden = true;
                e.Layout.Bands[0].Columns["OnHand"].Hidden = true;

                e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Artículo";
                e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Descripción";
                e.Layout.Bands[0].Columns["ItemName"].Width = 200;
                e.Layout.Bands[0].Columns["ItmsGrpNam"].Header.Caption = "Línea";

            }
            catch (Exception)
            {
            }
        }

        private void ultraCombo2_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Bands[0].Columns["WhsCode"].Hidden = true;
                e.Layout.Bands[0].Columns["OnHand"].Hidden = true;

                e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Artículo";
                e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Descripción";
                e.Layout.Bands[0].Columns["ItemName"].Width = 200;
                e.Layout.Bands[0].Columns["ItmsGrpNam"].Header.Caption = "Línea";

            }
            catch (Exception)
            {
            }
        }

        private void dgvDatos_AfterCellListCloseUp(object sender, CellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Index > -1)
                {
                    if (e.Cell.Column.Key.Equals("ItemCode"))
                    {
                        dgvDatos.Rows[e.Cell.Row.Index].Cells["ItemName"].Value = ultraCombo1.SelectedRow.Cells["ItemName"].Value;
                        dgvDatos.Rows[e.Cell.Row.Index].Cells["ItmsGrpNam"].Value = ultraCombo1.SelectedRow.Cells["ItmsGrpNam"].Value;
                    }

                    if (e.Cell.Column.Key.Equals("ItemName"))
                    {
                        dgvDatos.Rows[e.Cell.Row.Index].Cells["ItemCode"].Value = ultraCombo2.SelectedRow.Cells["ItemCode"].Value;
                        dgvDatos.Rows[e.Cell.Row.Index].Cells["ItmsGrpNam"].Value = ultraCombo2.SelectedRow.Cells["ItmsGrpNam"].Value;
                    }
                }
            }
            catch (Exception) { }
        }

        private void dgvDatos_AfterCellUpdate(object sender, CellEventArgs e)
        {
            dgvDatos_AfterCellListCloseUp(sender, e);
        }

        private void btnConsult_Click(object sender, EventArgs e)
        {
            try
            {
                var qry = from item in tbl_Items.AsEnumerable()
                          where item.Field<string>("ItmsGrpNam").Equals(cbLinea.Text)
                          select item;

                if (qry.Count() > 0)
                {
                    foreach (DataRow item in qry.CopyToDataTable().Rows)
                    {
                        int x = dgvDatos.DisplayLayout.Bands[0].AddNew().Index;
                        dgvDatos.Rows[x].Cells["ItemCode"].Value = item["ItemCode"];
                        dgvDatos.Rows[x].Cells["ItemName"].Value = item["ItemName"];
                        dgvDatos.Rows[x].Cells["ItmsGrpNam"].Value = item["ItmsGrpNam"];

                    }
                    dgvDatos.DisplayLayout.Bands[0].AddNew();
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }
    }

}
