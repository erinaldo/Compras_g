using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.Almacen.RecepcionDoctos
{
    public partial class frmReporteTiempos : Constantes.frmEmpty
    {
        public frmReporteTiempos()
        {
            InitializeComponent();
        }

        private bool Exist(string cardCode, string cardName, string colum)
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("su_Almacen", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 18);
                    command.Parameters.AddWithValue("@DocNum", txtOC.Text);
                    command.Parameters.AddWithValue("@CardCode", cardCode);
                    command.Parameters.AddWithValue("@CardName", cardName);
                    command.Parameters.AddWithValue("@Column", colum);

                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    DataTable tbl = new DataTable();
                    da.Fill(tbl);

                    return tbl.Rows.Count > 0;
                }
            }

        }

        private void frmReporteTiempos_Load(object sender, EventArgs e)
        {
            guardarToolStripButton.Enabled = false;
            actualizarToolStripButton.Enabled = false;


            nuevoToolStripButton.Click -= new EventHandler(btnNuevo_Click);
            nuevoToolStripButton.Click += new EventHandler(btnNuevo_Click);

            ClasesSGUV.Form.ControlsForms.setDataSource(clbAlmacenes, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenPorUsuario, ClasesSGUV.Login.Rol, ClasesSGUV.Login.Id_Usuario.ToString()), "WhsName", "WhsCode", "Todos");
            clbAlmacenes.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
            clbAlmacenes.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
        }

        private void txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                if (!Exist(txtCardCode.Text, txtCardName.Text, "CardCode") & !Exist(txtCardCode.Text, txtCardName.Text, "CardName") & string.IsNullOrEmpty(txtOC.Text.Trim()))
                {
                    SDK.Documentos.frmListadoDocumentos formulario =
                                            new SDK.Documentos.frmListadoDocumentos(-1, txtCardCode.Text.Replace('*', '%'),
                                                txtCardName.Text.Replace('*', '%'), DateTime.Now, string.Empty);

                    if (formulario.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                    {
                        txtCardCode.Text = Convert.ToString(formulario.Row["CardCode"]);
                        txtCardName.Text = Convert.ToString(formulario.Row["CardName"]);

                        btnConsult_Click(sender, e);
                    }
                }
                else
                {
                    btnConsult_Click(sender, e);
                }

            }
        }

        private void btnConsult_Click(object sender, EventArgs e)
        {
            try
            {
                string almacenes = ClasesSGUV.Form.ControlsForms.getCadena(clbAlmacenes, string.Empty, false, "WhsCode");
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("su_Almacen", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 19);
                        command.Parameters.AddWithValue("@DocNum", txtOC.Text);
                        command.Parameters.AddWithValue("@CardCode", txtCardCode.Text);
                        command.Parameters.AddWithValue("@CardName", txtCardName.Text);
                        command.Parameters.AddWithValue("@Desde", dtpDesde.Value);
                        command.Parameters.AddWithValue("@Almacenes", almacenes);
                        command.Parameters.AddWithValue("@Hasta", dtpHasta.Value);
                        command.Parameters.AddWithValue("@TipoFecha", rbContabilizacion.Checked ? "C" : "R");
                        
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        DataTable tbl = new DataTable();
                        da.Fill(tbl);

                        dgvDatos.DataSource = tbl;
                    }
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                item.Width = 90;
            }
            e.Layout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy;

            e.Layout.Bands[0].Columns["Nombre"].Width = 180;
            e.Layout.Bands[0].Columns["d2"].Width = 20;
            e.Layout.Bands[0].Columns["d2"].Header.Caption = string.Empty;

            e.Layout.Bands[0].Columns["d2"].CellAppearance.BackColor = Color.White;
            e.Layout.Bands[0].Columns["d2"].CellAppearance.ForeColor = Color.White;

            e.Layout.Bands[0].Columns["Fecha contabilización OC"].Format = "dd/MM/yyyy hh:mm:ss tt";
            e.Layout.Bands[0].Columns["Fecha de recepción documentos"].Format = "dd/MM/yyyy hh:mm:ss tt";
            e.Layout.Bands[0].Columns["Fecha Entrada Mercancia"].Format = "dd/MM/yyyy hh:mm:ss tt";

        }

        private void dgvDatos_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            e.Row.Cells["d2"].SelectedAppearance.ForeColor = Color.White;
            e.Row.Cells["d2"].SelectedAppearance.BackColor = Color.White;

            e.Row.Cells["d2"].Appearance.ForeColor = Color.White;
            e.Row.Cells["d2"].Appearance.BackColor = Color.White;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            txtOC.Clear();
            txtCardCode.Clear();
            txtCardName.Clear();
            dtpDesde.Value = DateTime.Now;
            dtpHasta.Value = DateTime.Now;

            dgvDatos.DataSource = null;
        }
    }
}
