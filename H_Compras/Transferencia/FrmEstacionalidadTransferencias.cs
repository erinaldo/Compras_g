using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;

namespace H_Compras.Transferencia
{
    public partial class FrmEstacionalidadTransferencias : Constantes.frmEmpty
    {
        ClasesSGUV.Logs log;
        public FrmEstacionalidadTransferencias()
        {
            InitializeComponent();
        }

        private void FrmEstacionalidadTransferencias_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable tbl_AlmacenesTemp = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesVenta, string.Empty, string.Empty);
                DataTable tbl_Almacenes = new DataTable();
                tbl_Almacenes = (from obj in tbl_AlmacenesTemp.AsEnumerable()
                                 where obj.Field<string>("Whscode") != "26" && obj.Field<string>("Whscode") != "T 197"
                                 select obj
                                ).CopyToDataTable();

                cboAlmacen.DataSource = tbl_Almacenes;
                cboAlmacen.DisplayMember = "WhsName";
                cboAlmacen.ValueMember = "Whscode";
                cboAlmacen.SelectedIndex = 0;

                buscarStripButton.Enabled=true;
                nuevoToolStripButton.Visible = false;
                imprimirToolStripButton.Visible = false;
                guardarToolStripButton.Visible = false;

                buscarStripButton.Click += btnConsultar_Click;
                exportarToolStripButton.Click += btnExportar_Click;

                log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            
        }

       
        private void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                //Se ejecuta el proceso para cerrar las lineas que han cumplido el stock
                //try
                //{
                //    using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                //    {
                //        using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                //        {
                //            command.Connection.Open();
                //            command.CommandType = CommandType.StoredProcedure;
                //            command.Parameters.AddWithValue("@TipoConsulta", 14);
                //            command.CommandTimeout = 0;
                //            command.ExecuteNonQuery();
                //            command.Connection.Close();
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //}
                
                int SolAnexo = 0; if(chbSolAnexo.Checked) { SolAnexo = 1; }
                int SolVta = 0; if(chbVtaConfirmada.Checked) { SolVta = 1; }

                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_TransferenciasAlmacen", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 11);
                        command.Parameters.AddWithValue("@CodAlmacen", Convert.ToString(cboAlmacen.SelectedValue.ToString().Trim()));
                        command.Parameters.AddWithValue("@Desde", dtpDesde.Value);
                        command.Parameters.AddWithValue("@Hasta", dtpHasta.Value);
                        command.Parameters.AddWithValue("@CAnexo", SolAnexo);
                        command.Parameters.AddWithValue("@CVtaConfirm", SolVta);
                        command.CommandTimeout = 0;

                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);

                        dgvArticulos.DataSource = dt;

                    }
                }
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void dgvArticulos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                if (e.Layout.Bands[0].Columns.Exists("Articulo"))
                    e.Layout.Bands[0].Columns["Articulo"].Width = 100;
                if (e.Layout.Bands[0].Columns.Exists("Descripcion"))
                    e.Layout.Bands[0].Columns["Descripcion"].Width = 200;
                if (e.Layout.Bands[0].Columns.Exists("Sucursal"))
                    e.Layout.Bands[0].Columns["Sucursal"].Width = 100;
                if (e.Layout.Bands[0].Columns.Exists("Cantidad"))
                {
                    e.Layout.Bands[0].Columns["Cantidad"].Width = 80;
                    e.Layout.Bands[0].Columns["Cantidad"].Format = "N0";
                }
                if (e.Layout.Bands[0].Columns.Exists("Facturado"))
                {
                    e.Layout.Bands[0].Columns["Facturado"].Width = 80;
                    e.Layout.Bands[0].Columns["Facturado"].Format = "N0";
                }
                if (e.Layout.Bands[0].Columns.Exists("Total"))
                {
                    e.Layout.Bands[0].Columns["Total"].Width = 80;
                    e.Layout.Bands[0].Columns["Total"].Format = "N0";
                }
                if (e.Layout.Bands[0].Columns.Exists("Estacionalidad"))
                    e.Layout.Bands[0].Columns["Estacionalidad"].Width = 100;

                if (e.Layout.Bands[0].Columns.Exists("Tipo Traspaso"))
                    e.Layout.Bands[0].Columns["Tipo Traspaso"].Width = 100;
                //e.Layout.Bands[0].Columns["nID"].Hidden = true;
                //e.Layout.Bands[0].Columns["sCodAlmacen"].Hidden = true;
                if (e.Layout.Bands[0].Columns.Exists("nTipo"))
                    e.Layout.Bands[0].Columns["nTipo"].Hidden = true;
                //e.Layout.Bands[0].Columns["nTipo"].Hidden = true;
                //e.Layout.Bands[0].Columns["CodVendor"].Hidden = true;
                //e.Layout.Bands[0].Columns["sItemCode"].Header.Caption = "Articulo";
                //e.Layout.Bands[0].Columns["sNombre"].Header.Caption = "Nombre";
                //e.Layout.Bands[0].Columns["CardCode"].Header.Caption = "Cliente";
                //e.Layout.Bands[0].Columns["Cliente"].Header.Caption = "Nombre";
                //e.Layout.Bands[0].Columns["Stock"].Format = "N2";

                foreach (var item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Activation.NoEdit;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                //Set the file name filter to default to .xls to list the type of file to save as, and present
                //.xls as the first file type in the drop down list.
                saveFile.Filter = "Excel files (*.xls)|*.xls|All files (*.*)|*.*";
                saveFile.FilterIndex = 1;
                saveFile.DefaultExt = ".xls";

                //Open a Windows save file dailog box and ask the user to save the file where ever they want. 
                //Pass in the grid and the FileName property to the exporter.
                if (saveFile.ShowDialog() == DialogResult.OK)
                    this.UExport.Export(this.dgvArticulos, saveFile.FileName);

                MessageBox.Show("Archivo exportado correctamente", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
           
        }

        private void FrmEstacionalidadTransferencias_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void FrmEstacionalidadTransferencias_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                log.Fin();
            }
            catch (Exception)
            {

            }
        }

        private void dgvArticulos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                string Tipo = Convert.IsDBNull(e.Row.Cells["nTipo"].Value) ? string.Empty : Convert.ToString(e.Row.Cells["nTipo"].Value);
                int Estacionalidad = Convert.IsDBNull(e.Row.Cells["Estacionalidad"].Value) ? 0 : Convert.ToInt32(e.Row.Cells["Estacionalidad"].Value);

                if (Tipo == "03") //Es solicitud de anexo
                {
                    if (Estacionalidad >= 0 && Estacionalidad <= 14)
                        e.Row.Cells["Estacionalidad"].Appearance.BackColor = Color.Green;
                    else if (Estacionalidad >= 15 && Estacionalidad <= 29)
                        e.Row.Cells["Estacionalidad"].Appearance.BackColor = Color.Yellow;
                    else if (Estacionalidad >= 30) //&& Estacionalidad <= 29)
                        e.Row.Cells["Estacionalidad"].Appearance.BackColor = Color.Red;
                }
                else if(Tipo == "02") //es Vta confirmada
                {
                    if (Estacionalidad >= 0 && Estacionalidad <= 2)
                        e.Row.Cells["Estacionalidad"].Appearance.BackColor = Color.Green;
                    else if (Estacionalidad >= 3 && Estacionalidad <= 6)
                        e.Row.Cells["Estacionalidad"].Appearance.BackColor = Color.Yellow;
                    else if (Estacionalidad >= 7) //&& Estacionalidad <= 29)
                        e.Row.Cells["Estacionalidad"].Appearance.BackColor = Color.Red;
 
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
