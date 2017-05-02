using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.Compras.Pedido
{
    public partial class frmPedidoReubicaciones : Constantes.frmEmpty
    {
        string ItemCode;
        string ToWhsCode;
        string sp_Name = "sp_Compras";
        DataTable tbl_Reubicacicones = new DataTable();

        public DataTable Tbl_Reubicaicones
        {
            get { return tbl_Reubicacicones; }
            set { tbl_Reubicacicones = value; }
        }

        public frmPedidoReubicaciones(DataTable _source)
        {
            InitializeComponent();

            tbl_Reubicacicones = _source;

            dgvDatos.DataSource = tbl_Reubicacicones;
            //dgvDatos.DisplayLayout.Bands[0].AddNew();
        }

        public frmPedidoReubicaciones(string _ItemCode, string _ToWhsCode)
        {
            InitializeComponent();

            ItemCode = _ItemCode;
            ToWhsCode = _ToWhsCode;
        }

        private void frm_PedidoReubicaciones_Load(object sender, EventArgs e)
        {
            try
            {
                nuevoToolStripButton.Enabled = false;
                exportarToolStripButton.Enabled = false;
                actualizarToolStripButton.Enabled = false;
                ayudaToolStripButton.Enabled = false;

                guardarToolStripButton.Click -= new EventHandler(btnSave_Click);
                guardarToolStripButton.Click += new EventHandler(btnSave_Click);

                dgvDatos.KeyDown -= new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);
                dgvDatos.KeyDown += new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);

                dgvDatos.KeyPress -= new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);
                dgvDatos.KeyPress += new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);

                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand(sp_Name, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 7);
                        command.Parameters.AddWithValue("@Almacen_Comprador", ToWhsCode);
                        command.Parameters.AddWithValue("@ItemCode", ItemCode);

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;

                        da.Fill(tbl_Reubicacicones);

                        if (tbl_Reubicacicones.Rows.Count <= 0)
                        {
                            this.SetMensaje("NO HAY CANTIDAD DISP PARA REUBICAR", 5000, Color.Red, Color.White);
                        }

                        dgvDatos.DataSource = tbl_Reubicacicones;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Bands[0].Columns["ToWhsCode"].Hidden = true;
                e.Layout.Bands[0].Columns["DestinoNombre"].Hidden = true;
                e.Layout.Bands[0].Columns["WhsCode"].Hidden = true;
                e.Layout.Bands[0].Columns["OnHand"].Hidden = true;
                e.Layout.Bands[0].Columns["WhsName"].Hidden = true;
                e.Layout.Bands[0].Columns["BWeight1"].Hidden = true;
                e.Layout.Bands[0].Columns["Volumen"].Hidden = true;
                e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;

                e.Layout.Bands[0].Columns["Cantidad"].Format = "N0";
                e.Layout.Bands[0].Columns["QtyOK"].Format = "N0";
                e.Layout.Bands[0].Columns["Cantidad"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

                e.Layout.Bands[0].Columns["Cantidad"].Header.Caption = "Disponible";
                e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Descripcón";
                e.Layout.Bands[0].Columns["QtyOK"].Header.Caption = "Cantidad a Reubicar";

                e.Layout.Bands[0].Columns["Cantidad"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns["OrigenNombre"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                e.Layout.Bands[0].Columns["OrigenNombre"].Width = 120;
                e.Layout.Bands[0].Columns["ItemName"].Width = 150;
                e.Layout.Bands[0].Columns["OrigenNombre"].Header.Caption = "Almacén origen";
                e.Layout.Bands[0].Columns["Cantidad"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns["QtyOK"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e) 
        {
            tbl_Reubicacicones.AcceptChanges();

            this.DialogResult = System.Windows.Forms.DialogResult.Yes;   
        }

        private void dgvDatos_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
        {
            try
            {
                decimal original = decimal.Zero;
                decimal reubicar = decimal.Zero;

                original = Convert.ToDecimal(dgvDatos.Rows[e.Cell.Row.Index].Cells["Cantidad"].Text);
                reubicar = Convert.ToDecimal(dgvDatos.Rows[e.Cell.Row.Index].Cells["QtyOK"].Text);

                if (reubicar > original)
                {
                    this.SetMensaje("Cantidad no disponible!!!", 5000, Color.Red, Color.White);
                    e.Cancel = true;
                    return;
                }
                else
                    this.SetMensaje(string.Empty, 5000, Color.Green, Color.White);
            }
            catch (Exception)
            {
            }
        }
    }
}
