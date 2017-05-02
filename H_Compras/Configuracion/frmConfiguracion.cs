using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;


namespace H_Compras.Compras
{
    public partial class frmConfiguracion : Constantes.frmEmpty
    {
        string Zona = string.Empty; 
        string ZonaGrupos = string.Empty;
        Datos.Connection connection = new Datos.Connection();
        Object[] valuesOut = new Object[] { };

        public frmConfiguracion()
        {
            InitializeComponent();
        }

        public void GetZonas(DataGridView dgv)
        {
            DataTable tblZonas = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetZonas, string.Empty, string.Empty);

            dgv.DataSource = tblZonas;
            dgv.Columns[1].Visible = false;
            dgv.Columns[0].Width = 120;
            dgv.Columns[1].Width = 100;
        }

        public void GetAlmacenesZonas(string _zona)
        {
            DataTable tblZonas = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesZona, _zona, string.Empty);

            dgvAlmacenes.DataSource = tblZonas;

            dgvAlmacenes.Columns[0].Visible = false;
            dgvAlmacenes.Columns[1].Visible = false;

            dgvAlmacenes.Columns[2].Width = 60;
            dgvAlmacenes.Columns[3].Width = 110;
            dgvAlmacenes.Columns[4].Width = 70;
            dgvAlmacenes.Columns[5].Width = 70;

            dgvAlmacenes.Columns[2].HeaderText = "Código";
            dgvAlmacenes.Columns[3].HeaderText = "Almacén";

            dgvAlmacenes.Columns[2].ReadOnly = true;
            dgvAlmacenes.Columns[3].ReadOnly = true;
        }

        public void GetAlmacenesVenta()
        {
            DataTable tblAlmacenesVenta = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesVenta, string.Empty, string.Empty);

            dgvAlmacenesVenta.DataSource = tblAlmacenesVenta;
            dgvAlmacenesVenta.Columns[0].HeaderText = "Código";
            dgvAlmacenesVenta.Columns[1].HeaderText = "Almacén";

            dgvAlmacenesVenta.Columns[0].Width = 50;
            dgvAlmacenesVenta.Columns[1].Width = 150;
        }

        public void GetGruposZonas(DataGridView dgv, string _zona)
        {
            //DataTable tblZonas = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GetGruposZona, _zona, string.Empty);

            //dgv.DataSource = tblZonas;
            //dgv.Columns[0].Width = 50;
            //dgv.Columns[1].Width = 120;

            //dgv.Columns[0].HeaderText = "ID";
            //dgv.Columns[1].HeaderText = "Nombre de grupo";
        }

        public void GetGruposSN(DataGridView dgv)
        {
            DataTable tblZonas = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.GrupoSN, string.Empty, string.Empty);

            dgv.DataSource = tblZonas;
            dgv.Columns[0].Width = 50;
            dgv.Columns[1].Width = 120;

            dgv.Columns[0].HeaderText = "ID";
            dgv.Columns[1].HeaderText = "Nombre de grupo";
        }

        public void GetProveedores()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 7);

                        SqlDataAdapter da = new SqlDataAdapter();
                        DataTable table = new DataTable();
                        da.SelectCommand = command;
                        da.Fill(table);

                        dgvProveedores.DataSource = table;
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public void GetAlmacenesProveedor(string _proveedor)
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TipoConsulta", 8);
                    command.Parameters.AddWithValue("@Zona", Zona);
                    command.Parameters.AddWithValue("@CardCode", _proveedor);

                    connection.Open();

                    DataTable table = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(table);

                    dgvAlmacenZonaProv.DataSource = table;

                    dgvAlmacenZonaProv.Columns[0].Visible = false;
                    dgvAlmacenZonaProv.Columns[1].Width = 130;
                    dgvAlmacenZonaProv.Columns[2].Visible = false;
                    dgvAlmacenZonaProv.Columns[3].Width = 130;
                    dgvAlmacenZonaProv.Columns[4].Width = 80;

                    dgvAlmacenZonaProv.Columns[0].ReadOnly = true;
                    dgvAlmacenZonaProv.Columns[1].ReadOnly = true;
                    dgvAlmacenZonaProv.Columns[2].ReadOnly = true;
                    dgvAlmacenZonaProv.Columns[3].ReadOnly = true;
                }
            }
        }

        private void GetClientes()
        {
           DataTable tbl = connection.GetDataTable("LOG",
                                      "up_Configuracion",
                                      new string[] { },
                                      new string[] { "@TipoConsulta"},
                                      ref valuesOut, 3);

           dgvClientes.DataSource = tbl;
        }

        private void frmConfiguracion_Load(object sender, EventArgs e)
        {
            try
            {
                #region Zonas
                txtZona.Clear();
                this.GetZonas(dgvZonas);

                this.GetAlmacenesVenta();

                cbDestino.DataSource = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenesVenta, string.Empty, string.Empty);
                cbWhsCode.DataSource = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListaTodosAlmacenes, string.Empty, string.Empty);

                cbDestino.DisplayMember = "WhsName";
                cbDestino.ValueMember = "WhsCode";

                cbWhsCode.DisplayMember = "WhsName";
                cbWhsCode.ValueMember = "WhsCode";

                nuevoToolStripButton.Enabled = false;
                guardarToolStripButton.Enabled = false;
                //ayudaToolStripButton.Enabled = false;


                actualizarToolStripButton.Click -= new EventHandler(frmConfiguracion_Load);
                actualizarToolStripButton.Click += new EventHandler(frmConfiguracion_Load);

                ayudaToolStripButton.Click -= new EventHandler(help_Click);
                ayudaToolStripButton.Click += new EventHandler(help_Click);

                cbDestino.SelectedIndex = 0;
                txtZona.Focus();

                #endregion

                #region Proveedores
                this.GetProveedores();
                this.GetZonas(dgvZonasProv);
                #endregion

                this.GetClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvZonas_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Zona = (sender as DataGridView).Rows[e.RowIndex].Cells[0].Value.ToString();

                label3.Text = "Almacenes " + (sender as DataGridView).Rows[e.RowIndex].Cells[0].Value.ToString();
                this.GetAlmacenesZonas((sender as DataGridView).Rows[e.RowIndex].Cells[0].Value.ToString());
            }
            catch (Exception)
            {
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TipoConsulta",1);
                    command.Parameters.AddWithValue("@Zona",Zona);
                    command.Parameters.AddWithValue("@WhsCode", dgvAlmacenesVenta.CurrentRow.Cells[0].Value);

                    connection.Open();

                    command.ExecuteNonQuery();

                    this.GetAlmacenesZonas(Zona);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("El almacén [" + dgvAlmacenes.CurrentRow.Cells[1].Value + "] será eliminada permanentemente de la Zona " + Zona + ", ¿Desea continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)

                    using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@TipoConsulta", 2);
                            command.Parameters.AddWithValue("@Zona", Zona);
                            command.Parameters.AddWithValue("@WhsCode", dgvAlmacenes.CurrentRow.Cells[0].Value);

                            connection.Open();

                            command.ExecuteNonQuery();

                            this.GetAlmacenesZonas(Zona);
                        }
                    }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        private void btnAddZona_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtZona.Text))
                {
                    MessageBox.Show("El Campo [Zona] es obigatorio.", "HalcoNET - Log", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(cbDestino.SelectedValue.ToString()))
                {
                    MessageBox.Show("El Campo [Almacén destino] es obigatorio.", "HalcoNET - Log", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 3);
                        command.Parameters.AddWithValue("@Zona", txtZona.Text);
                        command.Parameters.AddWithValue("@WhsCode", cbDestino.SelectedValue);

                        connection.Open();

                        command.ExecuteNonQuery();

                        this.GetZonas(dgvZonas);
                        txtZona.Clear();
                        txtZona.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if(MessageBox.Show("La  Zona ["+Zona+ "] será eliminada permanentemente, ¿Desea continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)

                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 4);
                        command.Parameters.AddWithValue("@Zona", Zona);
                        command.Parameters.AddWithValue("@WhsCode", cbDestino.SelectedValue);

                        connection.Open();

                        command.ExecuteNonQuery();

                        this.GetZonas(dgvZonas);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void dgvProveedores_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
                {
                    item.CellActivation = Activation.NoEdit;
                }

                e.Layout.Bands[0].Columns[0].Width = 70;
                e.Layout.Bands[0].Columns[1].Width = 150;
                e.Layout.Bands[0].Columns[2].Width = 80;
                e.Layout.Bands[0].Columns[3].Width = 70;
                e.Layout.Bands[0].Columns[4].Width = 70;
                e.Layout.Bands[0].Columns[5].Width = 110;
            }
            catch (Exception ) { }
        }

        private void dgvProveedores_Click(object sender, EventArgs e)
        {
            Ug = (sender as UltraGrid);

            Dg = null;
        }

        private void dgvProveedores_BeforeRowActivate(object sender, RowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[1].Value.ToString().Length >= 30)
                    label5.Text = "Proveedor: " + e.Row.Cells[1].Value.ToString().Substring(0, 30);
                else
                    label5.Text = "Proveedor: " + e.Row.Cells[1].Value.ToString();

                this.GetAlmacenesProveedor(e.Row.Cells[0].Value.ToString());
            }
            catch (Exception)
            {
            }
        }

        string _auxProv = string.Empty;
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("¿Desea continuar con esta operación?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    _auxProv = dgvProveedores.ActiveRow.Cells[0].Value.ToString();
                    foreach (DataGridViewRow item in dgvAlmacenZonaProv.SelectedRows)
                    {
                        using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                        {
                            using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                command.Parameters.AddWithValue("@TipoConsulta", 9);
                                command.Parameters.AddWithValue("@Zona", item.Cells[1].Value);
                                command.Parameters.AddWithValue("@CardCode", item.Cells[0].Value);
                                command.Parameters.AddWithValue("@WhsCode", item.Cells[2].Value);

                                connection.Open();

                                command.ExecuteNonQuery();

                                
                            }
                        }
                    }
                    this.GetAlmacenesProveedor(_auxProv);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 10);
                        command.Parameters.AddWithValue("@Zona", dgvZonasProv.CurrentRow.Cells[0].Value);
                        command.Parameters.AddWithValue("@CardCode", dgvProveedores.ActiveRow.Cells[0].Value);

                        connection.Open();

                        command.ExecuteNonQuery();

                        this.GetAlmacenesProveedor(dgvProveedores.ActiveRow.Cells[0].Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvAlmacenZonaProv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 11);
                        command.Parameters.AddWithValue("@Zona", dgvAlmacenZonaProv.CurrentRow.Cells[1].Value);
                        command.Parameters.AddWithValue("@CardCode", dgvAlmacenZonaProv.CurrentRow.Cells[0].Value);
                        command.Parameters.AddWithValue("@WhsCode", dgvAlmacenZonaProv.CurrentRow.Cells[2].Value);
                        command.Parameters.AddWithValue("@Independiente", dgvAlmacenZonaProv.CurrentRow.Cells[4].Value);

                        connection.Open();

                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvAlmacenes_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 12);
                        command.Parameters.AddWithValue("@Zona", dgvAlmacenes.CurrentRow.Cells[1].Value);
                        command.Parameters.AddWithValue("@WhsCode", dgvAlmacenes.CurrentRow.Cells[0].Value);
                        command.Parameters.AddWithValue("@Resta", dgvAlmacenes.CurrentRow.Cells[4].Value);
                        command.Parameters.AddWithValue("@Obligatorio", dgvAlmacenes.CurrentRow.Cells[5].Value);

                        connection.Open();

                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void help_Click(object sender, EventArgs e)
        {
           // System.Windows.Forms.Help.ShowHelp(this, @"\\192.168.2.205\Carmen\manual\facturas_1.htm", "facturas_1.htm");
        }

        private void btnAddCliente_Click(object sender, EventArgs e)
        {
            try
            {
                int x = connection.Ejecutar("LOG",
                                          "up_Configuracion",
                                          new string[] { },
                                          new string[] { "@TipoConsulta", "@CardCode", "@WhsCode" },
                                          ref valuesOut, 1, txtCardCode.Text, cbWhsCode.SelectedValue);

                this.GetClientes();
                this.SetMensaje("Listo.", 5000, Color.Green, Color.Black);
   
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void dgvClientes_AfterRowsDeleted(object sender, EventArgs e)
        {
           
        }

        private void dgvClientes_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            e.DisplayPromptMsg = false;

            if(MessageBox.Show("¿Desea eliminar los registros?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (var item in e.Rows)
                {
                    int x = connection.Ejecutar("LOG",
                                        "up_Configuracion",
                                        new string[] { },
                                        new string[] { "@TipoConsulta", "@CardCode" },
                                        ref valuesOut, 2, item.Cells["CardCode"].Value);
                }

                //this.GetClientes();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void dgvClientes_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["CardCode"].Header.Caption = "Cliente";
            e.Layout.Bands[0].Columns["CardName"].Header.Caption = "Nombre";
            e.Layout.Bands[0].Columns["WhsCode"].Header.Caption = "Código de almacén";
            e.Layout.Bands[0].Columns["WhsName"].Header.Caption = "Almacén";

            e.Layout.Bands[0].Columns["CardName"].Width = 180;
            e.Layout.Bands[0].Columns["WhsName"].Width = 120;
        }
    }
}
