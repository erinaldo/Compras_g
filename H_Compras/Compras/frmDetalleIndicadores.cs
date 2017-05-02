using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

namespace H_Compras.Compras
{
    public partial class frmDetalleIndicadores : Constantes.frmEmpty
    {
        DataSet DS;
        string Tipo;

        public frmDetalleIndicadores(DataSet ds, string tipo)
        {
            InitializeComponent();
            DS = ds;
            Tipo = tipo;

            this.Text += " [" + tipo + "]";
        }

        public frmDetalleIndicadores()
        {
            InitializeComponent();
        }

        private void frmDetalleIndicadores_Load(object sender, EventArgs e)
        {
            try
            {
                nuevoToolStripButton.Enabled = false;
                guardarToolStripButton.Enabled = false;
                ayudaToolStripButton.Enabled = false;

                dgvDatos.DataSource = DS;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }        
            
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            #region pendientes
            if (Tipo.Equals("PENDIENTES"))
            {
                e.Layout.Bands[0].Columns[0].Width = 70;
                e.Layout.Bands[0].Columns[1].Width = 280;

                e.Layout.Bands[0].Columns[2].Width = 80;
                e.Layout.Bands[0].Columns[3].Width = 80;
                e.Layout.Bands[0].Columns[4].Width = 80;

                e.Layout.Bands[0].Columns[2].Format = "N0";
                e.Layout.Bands[0].Columns[3].Format = "N0";
                e.Layout.Bands[0].Columns[4].Format = "N0";

                e.Layout.Bands[0].Columns[2].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[3].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[4].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

                e.Layout.Bands[1].ColHeadersVisible = false;

                e.Layout.Bands[1].Columns[0].Width = 60;
                e.Layout.Bands[1].Columns[1].Hidden = true;
                e.Layout.Bands[1].Columns[3].Hidden = true;
                e.Layout.Bands[1].Columns[7].Hidden = true;
                e.Layout.Bands[1].Columns[8].Hidden = true;
                e.Layout.Bands[1].Columns[9].Hidden = true;

                e.Layout.Bands[1].Columns[4].Format = "N0";
                e.Layout.Bands[1].Columns[5].Format = "N0";
                e.Layout.Bands[1].Columns[6].Format = "N0";

                e.Layout.Bands[1].Columns[4].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[1].Columns[5].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[1].Columns[6].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

                foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Activation.NoEdit;
                }

                foreach (UltraGridColumn item in e.Layout.Bands[1].Columns)
                {
                    item.CellActivation = Activation.NoEdit;
                }
            }
            #endregion

            #region stockout
            if (Tipo.Equals("STOCKOUT___"))
            {
                e.Layout.Bands[0].Columns[2].Width = 280;

                e.Layout.Bands[0].Columns[6].Header.Caption = "Días stock";

                e.Layout.Bands[0].Columns[4].Format = "N0";
                e.Layout.Bands[0].Columns[5].Format = "N0";

                e.Layout.Bands[0].Columns[4].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[5].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[6].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

                foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Activation.NoEdit;
                }
            }
            #endregion

            #region desabasto
            if (Tipo.Equals("DESABASTO") || Tipo.Equals("STOCKOUT"))
            {
                e.Layout.Bands[0].Columns[2].Width = 280;

                e.Layout.Bands[0].Columns["diasStock"].Header.Caption = "Días stock";

                e.Layout.Bands[0].Columns[4].Format = "N0";
                e.Layout.Bands[0].Columns[5].Format = "N0";
                e.Layout.Bands[0].Columns[6].Format = "N0";
                e.Layout.Bands[0].Columns[7].Format = "N0";
                e.Layout.Bands[0].Columns[8].Format = "N0";
                e.Layout.Bands[0].Columns[9].Format = "C0";

                e.Layout.Bands[0].Columns[4].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[5].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[6].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[7].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[8].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[9].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[10].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

                foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Activation.NoEdit;
                }

                e.Layout.Override.AllowMultiCellOperations = AllowMultiCellOperation.Copy;

            }
            #endregion

            #region sobrestock
            if (Tipo.Equals("SOBRESTOCK"))
            {
                e.Layout.Bands[0].Columns[2].Width = 280;
                
                e.Layout.Bands[0].Columns[4].Format = "N0";
                e.Layout.Bands[0].Columns[5].Format = "N0";
                e.Layout.Bands[0].Columns[6].Format = "N0";
                e.Layout.Bands[0].Columns[7].Format = "N0";
                e.Layout.Bands[0].Columns[8].Format = "N0";
                e.Layout.Bands[0].Columns[9].Format = "C0";

                e.Layout.Bands[0].Columns[4].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[5].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[6].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[7].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[8].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns[9].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
               // e.Layout.Bands[0].Columns[10].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

                foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Activation.NoEdit;
                }

                e.Layout.Override.AllowMultiCellOperations = AllowMultiCellOperation.Copy;
            }
            #endregion

            #region arribos
            if (Tipo.Equals("ARRIBOS"))
            {
                e.Layout.Bands[0].Columns[0].Header.Caption = "Num OC";
                e.Layout.Bands[0].Columns[1].Width = 350;

                e.Layout.Bands[1].Columns[1].AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;

                e.Layout.Bands[1].Columns[0].Hidden = true;
                e.Layout.Bands[1].Columns[1].Hidden = true;
                e.Layout.Bands[1].Columns[9].Hidden = true;
                e.Layout.Bands[0].Columns["Vencido"].Hidden = true;

                e.Layout.Bands[1].Columns[0].Header.Caption = "Num OC";
                e.Layout.Bands[1].Columns[1].Header.Caption = "Proveedor";
                e.Layout.Bands[1].Columns[2].Header.Caption = "Artículo";
                e.Layout.Bands[1].Columns[3].Header.Caption = "Descripción";
                e.Layout.Bands[1].Columns[4].Header.Caption = "Cantidad";
                e.Layout.Bands[1].Columns[5].Header.Caption = "Almacén";
                e.Layout.Bands[1].Columns[6].Header.Caption = "Fecha de arribo";
                e.Layout.Bands[1].Columns[7].Header.Caption = "Días";

                //e.Layout.Bands[1].Columns[2].Width = 250;
                //e.Layout.Bands[1].Columns[3].Width = 250;

                e.Layout.Bands[1].Columns[4].Format = "N0";

                e.Layout.Bands[1].Columns[4].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

                foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Activation.NoEdit;
                }
                foreach (UltraGridColumn item in e.Layout.Bands[1].Columns)
                {
                    item.CellActivation = Activation.NoEdit;
                }

                e.Layout.Override.AllowMultiCellOperations = AllowMultiCellOperation.Copy;
            }
            #endregion
        }

        private void dgvDatos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                if (Tipo.Equals("ARRIBOS"))
                {
                    if (Convert.ToDecimal(e.Row.Cells["Vencido"].Value) != decimal.Zero)
                    {
                        e.Row.Cells["Estatus"].Appearance.BackColor = Color.Red;
                        e.Row.Cells["Estatus"].Appearance.ForeColor = Color.White;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
