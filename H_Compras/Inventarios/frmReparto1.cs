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

namespace H_Compras.Inventarios
{
    public partial class frmReparto1 : Constantes.frmEmpty
    {
        private int numOC;
        //private string Aduana;
        private enum Columnas
        {
            Articulo,
            Descripcion,
            Cantidad, 
            
            IdealZNORTE,
            StockZNORTE,
            CantidadZNORTE,
           // PZNORTE,
            IdealGDL,
            StockGDL,
            CantidadGDL,
          //  PGDL,
            IdealMEX,
            StockMEX,
            CantidadMEX,

            CEDIS
        }

        public frmReparto1()
        {
            InitializeComponent();
            //this.Text = nameForm;
            //numOC = num;
            //Aduana = aduana;
        }

        private bool duplicados(string DocKey)
        {
            foreach (ListViewItem item in lvOrdenes.Items)
            {
                if (item.Text.Equals(DocKey))
                    return true;
            }

            return false;
        }

        private void frmReparto1_Load(object sender, EventArgs e)
        {
            this.Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

            nuevoToolStripButton.Click -= new EventHandler(nuevo_Click);
            nuevoToolStripButton.Click += new EventHandler(nuevo_Click);

            guardarToolStripButton.Enabled = false;
            actualizarToolStripButton.Enabled = false;
            ayudaToolStripButton.Enabled = false;

            //if (numOC == 1)
            //{
            //    btnAdd.Visible = false;
            //}

            dgvDatos.DisplayLayout.Override.RowSelectorHeaderStyle = Infragistics.Win.UltraWinGrid.RowSelectorHeaderStyle.ColumnChooserButton;
            dgvDatos.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            dgvDatos.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.dgvDatos.PerformAction(UltraGridAction.Copy, true, true);

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
            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ObjValidaciones.ValiarDocto(txtOC.Text, string.Empty, (int)Constantes.Clases.Validaciones.ObjectType.OrdenCompra))
            {
                if (!this.duplicados(txtOC.Text))
                {
                    //if (lvOrdenes.Items.Count < numOC)
                    lvOrdenes.Items.Add(txtOC.Text);
                    txtOC.Clear();
                    //else
                    //   this.SetMensaje(String.Format("Para esta aduana solo se permite ingresar {0} ordenes de compra.", numOC), 5000, Color.Red, Color.White);
                }
                    

                txtOC.Focus();
            }
            else
                this.SetMensaje(ObjValidaciones.Mensaje, 5000, Color.Red, Color.White);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                lvOrdenes.Items.Remove(lvOrdenes.SelectedItems[0]);

                lvOrdenes.Items[0].Selected = true;
            }
            catch (Exception)
            {
            }
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            try
            {
                if (numOC == 1) button1_Click(sender, e);

                string ocs = string.Empty;

                foreach (ListViewItem item in lvOrdenes.Items)
                {
                    ocs += item.Text+ ",";
                }

                if (lvOrdenes.Items.Count > 0)
                {
                    using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("sp_Inventario", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@TipoConsulta", 16);
                            command.Parameters.AddWithValue("@DocKeys", ocs.Trim(','));
                            command.Parameters.AddWithValue("@Doc","OPDN");
                            //command.Parameters.AddWithValue("@aduana", Aduana);

                            DataTable table = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = command;
                            command.CommandTimeout = 0;

                            da.Fill(table);
                            dgvDatos.DataSource = table;
                        }
                    }
                }else
                {
                    this.SetMensaje("Debe ingresar un número de OC", 5000, Color.Red, Color.White);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            Infragistics.Win.UltraWinCalcManager.UltraCalcManager calcManager;
            calcManager = new Infragistics.Win.UltraWinCalcManager.UltraCalcManager(this.Container);
            e.Layout.Grid.CalcManager = calcManager;

            foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn item in e.Layout.Bands[0].Columns)
            {
                item.Format = "N0";
                item.Width = 90;
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                item.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            }

            e.Layout.Bands[0].Columns[(int)Columnas.Articulo].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
            e.Layout.Bands[0].Columns[(int)Columnas.Descripcion].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

            e.Layout.Bands[0].Columns[(int)Columnas.Descripcion].Width = 180;
            //e.Layout.Bands[0].Columns[(int)Columnas.Cantidad].Header.Caption = "Cantidad en OC";

            e.Layout.Bands[0].Columns[(int)Columnas.CantidadZNORTE].Header.Caption = "Transferir a MTY";
            e.Layout.Bands[0].Columns[(int)Columnas.CantidadGDL].Header.Caption = "Transferir a PUE";
            e.Layout.Bands[0].Columns[(int)Columnas.CEDIS].Header.Caption = "Transferir a CEDIS";
            e.Layout.Bands[0].Columns[(int)Columnas.CantidadMEX].Header.Caption = "Transferir a EDOMEX";

            e.Layout.Bands[0].Columns[(int)Columnas.StockGDL].Hidden = true;
            e.Layout.Bands[0].Columns[(int)Columnas.StockMEX].Hidden = true;
            e.Layout.Bands[0].Columns[(int)Columnas.StockZNORTE].Hidden = true;

            e.Layout.Bands[0].Columns[(int)Columnas.IdealGDL].Hidden = true;
            e.Layout.Bands[0].Columns[(int)Columnas.IdealZNORTE].Hidden = true;
            e.Layout.Bands[0].Columns[(int)Columnas.IdealMEX].Hidden = true;


            //e.Layout.Bands[0].Columns[(int)Columnas.PZNORTE].Header.Caption = "%";
            //e.Layout.Bands[0].Columns[(int)Columnas.PGDL].Header.Caption = "%";

            //e.Layout.Bands[0].Columns[(int)Columnas.PZNORTE].Format = "P2";
            //e.Layout.Bands[0].Columns[(int)Columnas.PGDL].Format = "P2";

            //e.Layout.Bands[0].Columns[(int)Columnas.PZNORTE].Width = 70;
            //e.Layout.Bands[0].Columns[(int)Columnas.PGDL].Width = 70;

            Infragistics.Win.UltraWinGrid.UltraGridLayout layout = e.Layout;
            Infragistics.Win.UltraWinGrid.UltraGridBand band = layout.Bands[0];

            e.Layout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.TopFixed;// SummaryDisplayAreas.Top;
            e.Layout.Override.SummaryFooterCaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            e.Layout.Bands[0].Summaries.Clear();

            Infragistics.Win.UltraWinGrid.SummarySettings summary1 = band.Summaries.Add("GDL", Infragistics.Win.UltraWinGrid.SummaryType.Sum, band.Columns[(int)Columnas.CantidadGDL]);
            Infragistics.Win.UltraWinGrid.SummarySettings summary2 = band.Summaries.Add("CEDIS", Infragistics.Win.UltraWinGrid.SummaryType.Sum, band.Columns[(int)Columnas.CEDIS]);
            Infragistics.Win.UltraWinGrid.SummarySettings summary3 = band.Summaries.Add("Total", Infragistics.Win.UltraWinGrid.SummaryType.Sum, band.Columns[(int)Columnas.Cantidad]);
            Infragistics.Win.UltraWinGrid.SummarySettings summary4 = band.Summaries.Add("ZNORTE", Infragistics.Win.UltraWinGrid.SummaryType.Sum, band.Columns[(int)Columnas.CantidadZNORTE]);
            Infragistics.Win.UltraWinGrid.SummarySettings summary5 = band.Summaries.Add("MEX", Infragistics.Win.UltraWinGrid.SummaryType.Sum, band.Columns[(int)Columnas.CantidadMEX]);

            //Infragistics.Win.UltraWinGrid.SummarySettings summary5 = band.Summaries.Add("p1", Infragistics.Win.UltraWinGrid.SummaryType.Formula, band.Columns[(int)Columnas.PZNORTE]);
            //Infragistics.Win.UltraWinGrid.SummarySettings summary6 = band.Summaries.Add("p2", Infragistics.Win.UltraWinGrid.SummaryType.Formula, band.Columns[(int)Columnas.PGDL]);


            //summary5.Formula = "SUM([CantidadMTY])/SUM([Cantidad])";
            //summary6.Formula = "SUM([CantidadPUE])/SUM([Cantidad])";

            summary1.DisplayFormat = "{0:N0}";
            summary2.DisplayFormat = "{0:N0}";
            summary3.DisplayFormat = "{0:N0}";
            summary4.DisplayFormat = "{0:N0}";
            summary5.DisplayFormat = "{0:N0}";
            //summary6.DisplayFormat = "{0:P2}";

            summary1.Appearance.FontData.Bold = DefaultableBoolean.True;
            summary2.Appearance.FontData.Bold = DefaultableBoolean.True;
            summary3.Appearance.FontData.Bold = DefaultableBoolean.True;
            summary4.Appearance.FontData.Bold = DefaultableBoolean.True;
            summary5.Appearance.FontData.Bold = DefaultableBoolean.True;
            //summary6.Appearance.FontData.Bold = DefaultableBoolean.True;

            summary1.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            summary2.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            summary3.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            summary4.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            summary5.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            //summary6.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

            //e.Layout.Bands[0].Columns[(int)Columnas.IdealGDL].Hidden = Aduana.Equals("manzanillo");
            //e.Layout.Bands[0].Columns[(int)Columnas.StockGDL].Hidden = Aduana.Equals("manzanillo");
            //e.Layout.Bands[0].Columns[(int)Columnas.CantidadGDL].Hidden = Aduana.Equals("manzanillo");
            //e.Layout.Bands[0].Columns[(int)Columnas.PGDL].Hidden = Aduana.Equals("manzanillo");

            //e.Layout.Bands[0].Columns[(int)Columnas.IdealZCENTRO].Hidden = Aduana.Equals("manzanillo");
            //e.Layout.Bands[0].Columns[(int)Columnas.StockZCENTRO].Hidden = Aduana.Equals("manzanillo");
        }

        private void nuevo_Click(object sender, EventArgs e)
        {
            txtOC.Clear();
            lvOrdenes.Items.Clear();
            dgvDatos.DataSource = null;
            txtOC.Focus();
        }
    }
}
