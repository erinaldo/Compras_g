using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.Inventarios.Conteos
{
    public partial class frmAdministracion : Constantes.frmEmpty
    {
        public frmAdministracion()
        {
            InitializeComponent();
        }

        private void frmAdministracion_Load(object sender, EventArgs e)
        {
            exportarToolStripButton.Enabled = false;
            guardarToolStripButton.Enabled = false;
            nuevoToolStripButton.Enabled = false;

            ClasesSGUV.Form.ControlsForms.setDataSource(clbAlmacen, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListadoAlmecenes, ClasesSGUV.Login.Rol, ClasesSGUV.Login.Almacen),"WhsName", "WhsCode", "---Selecciona un almacén---");

            //if (clbAlmacen.Items.Count <= 2) clbAlmacen.SelectedIndex = 1;

            clbAlmacen_SelectionChangeCommitted(sender, e);

            #region Permisos
            switch (ClasesSGUV.Login.Rol)
            {
                case (int)ClasesSGUV.Propiedades.RolesHalcoNET.Administrador:
                    { }
                    break;
                case (int)ClasesSGUV.Propiedades.RolesHalcoNET.Zulma:
                    { }
                    break;
                case (int)ClasesSGUV.Propiedades.RolesHalcoNET.InventariosAuxiliar:
                    { }
                    break;
                default:
                    {
                        linkLabel1.Visible = false;
                    }
                    break;
            }
            #endregion
        }

        private void clbAlmacen_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 1);
                        command.Parameters.AddWithValue("@WhsCode", clbAlmacen.SelectedValue);

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;

                        DataTable table = new DataTable();
                        da.Fill(table);

                        dgvDatos.DataSource = table;
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
            }
            e.Layout.Bands[0].Columns["Inv ciego"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
            e.Layout.Bands[0].Columns["Comentarios"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;

            e.Layout.Bands[0].Columns["Ver"].Width = 50;

            if (ClasesSGUV.Login.Rol == (int)ClasesSGUV.Propiedades.RolesHalcoNET.Administrador
                || ClasesSGUV.Login.Rol == (int)ClasesSGUV.Propiedades.RolesHalcoNET.Zulma
                || ClasesSGUV.Login.Rol == (int)ClasesSGUV.Propiedades.RolesHalcoNET.InventariosAuxiliar)
            {
            }else
                e.Layout.Bands[0].Columns["Inv ciego"].Hidden = true;
        }

        private void dgvDatos_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            e.Row.Cells["Ver"].ButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            e.Row.Cells["Ver"].ButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
            e.Row.Cells["Ver"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            e.Row.Cells["Ver"].ButtonAppearance.BorderColor = Color.CadetBlue;
            e.Row.Cells["Ver"].ButtonAppearance.Image = Properties.Resources.arrow_right_grey;
            e.Row.Cells["Ver"].Appearance.ImageBackground = Properties.Resources.arrow_right_grey;

            e.Row.Cells["Comentarios"].Column.MaxLength = 250;

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (clbAlmacen.SelectedIndex == 0) return;

            frmAlert1 frm = new frmAlert1();
            if (frm.ShowDialog() != System.Windows.Forms.DialogResult.Yes)
                return;

            DialogResult r = MessageBox.Show("Se creará un nuevo folio para el almacén " + clbAlmacen.Text + "\r\n¿Desea continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r == System.Windows.Forms.DialogResult.Yes)
            {
                string msg = string.Empty;
                int alert = 0;
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())// new SqlCommand("su_Conteos", connection))
                    {
                        command.CommandText = "su_Conteos";
                        command.CommandType = CommandType.StoredProcedure;

                        string tipo = string.Empty;
                        if (frm.rb1.Checked)
                            tipo = "A";
                        else if (frm.rb2.Checked)
                            tipo = "B";
                        else if (frm.rb3.Checked)
                            tipo = "C";
                        else if (frm.rbGeneral.Checked)
                            tipo = "D";

                        command.Parameters.AddWithValue("@TipoConsulta", 2);
                        command.Parameters.AddWithValue("@WhsCode", clbAlmacen.SelectedValue);
                        command.Parameters.AddWithValue("@userID", ClasesSGUV.Login.Id_Usuario);
                        command.Parameters.AddWithValue("@tipoLista", frm.rb1.Checked ? 1 : 0);
                        command.Parameters.AddWithValue("@Comment", frm.txtComments.Text);
                        command.Parameters.AddWithValue("@Tipo", tipo);

                        SqlParameter param = command.Parameters.Add("@Mensaje", SqlDbType.VarChar, 250);
                        param.Direction = ParameterDirection.Output;
                        SqlParameter param2 = command.Parameters.Add("@TipoAlert", SqlDbType.Int);
                        param2.Direction = ParameterDirection.Output;

                        //connection.Open();
                        decimal Folio = Convert.ToDecimal(command.ExecuteScalar());

                        msg = Convert.ToString(param.Value);
                        alert = Convert.ToInt32(param2.Value);

                        
                        #region Conteo import excel
                        try
                        {  
                            int counter = 0;
                            if (!string.IsNullOrEmpty(frm.txtPath.Text))
                            {
                                string line;
                                System.IO.StreamReader file = new System.IO.StreamReader(frm.txtPath.Text);
                                while ((line = file.ReadLine()) != null)
                                {
                                    if (!string.IsNullOrEmpty(line))
                                    {
                                        #region insert
                                        command.CommandText = "su_Conteos";
                                        command.Parameters.Clear();
                                        command.CommandType = CommandType.StoredProcedure;
                                        command.Parameters.AddWithValue("@TipoConsulta", 6);

                                        command.Parameters.AddWithValue("@ItemCode", line.Trim());
                                        command.Parameters.AddWithValue("@WhsCode", clbAlmacen.SelectedValue);
                                        command.Parameters.AddWithValue("@Comment", string.Empty);

                                        command.Parameters.AddWithValue("@idConteo", Folio);
                                        command.Parameters.AddWithValue("@UserID", ClasesSGUV.Login.Id_Usuario);
                                        SqlParameter param_1 = command.Parameters.Add("@Mensaje", SqlDbType.VarChar, 250);
                                        param_1.Direction = ParameterDirection.Output;
                                        SqlParameter param2_2 = command.Parameters.Add("@TipoAlert", SqlDbType.Int);
                                        param2_2.Direction = ParameterDirection.Output;

                                        DataTable tbl = new DataTable();
                                        SqlDataAdapter da = new SqlDataAdapter(command);
                                        da.Fill(tbl);

                                        msg = Convert.ToString(param_1.Value);
                                        alert = Convert.ToInt32(param2_2.Value);
                                        if (alert == 0)
                                            throw new Exception("No se entraron coincidencias. Revisar archivo");
                                        #endregion
                                        counter++;
                                    }
                                }
                            }
                            //transaction.Commit();

                            this.SetMensaje("Listo, " + (counter + 1) + " Filas exportadas!" , 8000, Color.Red, Color.White);

                        }
                        catch (Exception ex)
                        {
                            //transaction.Rollback();
                            this.SetMensaje(ex.Message, 8000, Color.Red, Color.White);
                        }
                        #endregion

                        #region Conteo general
                        try
                        {
                            if (frm.rbGeneral.Checked)
                            {
                                #region inseRT
                                command.CommandText = "su_Conteos";
                                command.Parameters.Clear();
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@TipoConsulta", 15);
                                command.Parameters.AddWithValue("@WhsCode", clbAlmacen.SelectedValue);
                                command.Parameters.AddWithValue("@idConteo", Folio);
                                command.Parameters.AddWithValue("@UserID", ClasesSGUV.Login.Id_Usuario);
                                SqlParameter param_1 = command.Parameters.Add("@Mensaje", SqlDbType.VarChar, 250);
                                param_1.Direction = ParameterDirection.Output;
                                SqlParameter param2_2 = command.Parameters.Add("@TipoAlert", SqlDbType.Int);
                                param2_2.Direction = ParameterDirection.Output;

                                command.ExecuteNonQuery();
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            this.SetMensaje(ex.Message, 8000, Color.Red, Color.White);
                        }
                        #endregion

                        clbAlmacen_SelectionChangeCommitted(sender, e);

                        this.SetMensaje(msg, 5000, alert == 1 ? Color.Red : Color.Green, alert == 1 ? Color.White : Color.Black);
                    }
                }
            }
        }

        private void dgvDatos_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell.Row.Index > -1)
            {
                Conteos.frmConteoFisico form = new frmConteoFisico(Convert.ToInt32(dgvDatos.Rows[e.Cell.Row.Index].Cells["Folio"].Value));
                form.Show();
            }
        }

        private void dgvDatos_BeforeRowUpdate(object sender, Infragistics.Win.UltraWinGrid.CancelableRowEventArgs e)
        {
            if (e.Row.Index < 0 | e.Row.IsAddRow) return;
            string msg = string.Empty;
            int alert = 0;
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TipoConsulta", 8);
                    command.Parameters.AddWithValue("@idConteo", e.Row.Cells["Folio"].Value);
                    command.Parameters.AddWithValue("@isCiego", e.Row.Cells["Inv ciego"].Value);
                    command.Parameters.AddWithValue("@Comment", e.Row.Cells["Comentarios"].Value);

                    SqlParameter param = command.Parameters.Add("@Mensaje", SqlDbType.VarChar, 250);
                    param.Direction = ParameterDirection.Output;
                    SqlParameter param2 = command.Parameters.Add("@TipoAlert", SqlDbType.Int);
                    param2.Direction = ParameterDirection.Output;

                    connection.Open();
                    command.ExecuteNonQuery();

                    msg = Convert.ToString(param.Value);
                    alert = Convert.ToInt32(param2.Value);

                    this.SetMensaje(msg, 5000, alert == 1 ? Color.Red : Color.Green, alert == 1 ? Color.White : Color.Black);

                    if (alert == 1) clbAlmacen_SelectionChangeCommitted(sender, e);
                }
            }
        }

        private void dgvDatos_AfterRowsDeleted(object sender, EventArgs e)
        {
           
        }

        private void dgvDatos_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
        {
            e.DisplayPromptMsg = false;
            e.Cancel = true;
        }
    }
}
