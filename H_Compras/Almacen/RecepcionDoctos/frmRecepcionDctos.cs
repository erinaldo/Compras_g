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
    public partial class frmRecepcionDctos : Constantes.frmEmpty
    {
        public frmRecepcionDctos()
        {
            InitializeComponent();
        }

        private void frmRecepcionDctos_Load(object sender, EventArgs e)
        {
            nuevoToolStripButton.Click -= new EventHandler(btnNuevo_Click);
            nuevoToolStripButton.Click += new EventHandler(btnNuevo_Click);

            ayudaToolStripButton.Click -= new EventHandler(btnAyuda_Click);
            ayudaToolStripButton.Click += new EventHandler(btnAyuda_Click);

            dgvDatos.KeyPress -= new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);
            dgvDatos.KeyPress += new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);

            dgvDatos.KeyDown -= new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);
            dgvDatos.KeyDown += new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);

            ClasesSGUV.Form.ControlsForms.setDataSource(clbAlmacenes, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.AlmacenPorUsuario, ClasesSGUV.Login.Rol, ClasesSGUV.Login.Id_Usuario.ToString()), "WhsName", "WhsCode", string.Empty);
            clbAlmacenes.Click -= new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
            clbAlmacenes.Click += new EventHandler(ClasesSGUV.Form.ControlsForms.clbBox_Click);
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["DocEntry"].Hidden = true;
            e.Layout.Bands[0].Columns["entregado"].Hidden = true;
            e.Layout.Bands[0].Columns["Objtype"].Hidden = true;

            e.Layout.Bands[0].Columns["DocNum"].Width = 70;
            e.Layout.Bands[0].Columns["CardCode"].Width = 70;
            e.Layout.Bands[0].Columns["CardName"].Width = 180;
            e.Layout.Bands[0].Columns["DocTotal"].Width = 90;
            e.Layout.Bands[0].Columns["Comentarios OC"].Width = 200;
            //e.Layout.Bands[0].Columns["Recepción documentos"].Width = 60;

            e.Layout.Bands[0].Columns["DocNum"].Header.Caption = "Num de OC";
            e.Layout.Bands[0].Columns["CardCode"].Header.Caption = "Proveedor";
            e.Layout.Bands[0].Columns["CardName"].Header.Caption = "Nombre";
            e.Layout.Bands[0].Columns["DocTotal"].Header.Caption = "Total";
            e.Layout.Bands[0].Columns["Comentarios OC"].Header.Caption = "Comentarios";

            e.Layout.Bands[0].Columns["DocTotal"].Format = "C2";

            e.Layout.Bands[0].Columns["DocTotal"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Fecha recepcion"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

            e.Layout.Bands[0].Columns["Documento"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["DocNum"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["CardCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["CardName"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["DocTotal"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["Comentarios OC"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
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
        
        private void txtCardName_KeyPress(object sender, KeyPressEventArgs e)
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

                        button1_Click(sender, e);
                    }
                }
                else
                {
                    button1_Click(sender, e);
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("su_Almacen", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 16);
                        command.Parameters.AddWithValue("@DocNum", txtOC.Text);
                        command.Parameters.AddWithValue("@CardCode", txtCardCode.Text);

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

        private void dgvDatos_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            //e.Row.Cells["Recepción documentos"].ButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            //e.Row.Cells["Recepción documentos"].ButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
            //e.Row.Cells["Recepción documentos"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            //e.Row.Cells["Recepción documentos"].ButtonAppearance.BorderColor = Color.Black;
            //e.Row.Cells["Recepción documentos"].Appearance.BorderColor = Color.Black;
            //e.Row.Cells["Recepción documentos"].ButtonAppearance.BackColor = Color.FromName("Control");
            //e.Row.Cells["Recepción documentos"].Appearance.BackColor = Color.FromName("Control");

            //if (e.Row.Cells["entregado"].Value.ToString().Equals("Y"))
            //{
            //    e.Row.Cells["Recepción documentos"].ButtonAppearance.BackColor = Color.Green;
            //    e.Row.Cells["Recepción documentos"].Appearance.BackColor = Color.Green;
            //}
        }

        private void btnRecibir_Click(object sender, EventArgs e)
        {

            try
            {
                if (dgvDatos.Selected.Rows.Count <= 0)
                {
                    this.SetMensaje("Seleccione una fila", 5000, Color.Red, Color.White);
                    return;
                }
                foreach (var item in dgvDatos.Selected.Rows)
                {
                    using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("su_Almacen", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@TipoConsulta", 17);
                            command.Parameters.AddWithValue("@DocEntry", item.Cells["DocEntry"].Value);
                            command.Parameters.AddWithValue("@ObjType", item.Cells["ObjType"].Value);
                            command.Parameters.AddWithValue("@FechaRecepcion", item.Cells["Fecha recepcion"].Value);
                            command.Parameters.AddWithValue("@UserId", ClasesSGUV.Login.Id_Usuario);
                            command.Parameters.AddWithValue("@WhsCode", ClasesSGUV.Login.Almacen);
                            
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }

                button1_Click(sender, e);
                this.SetMensaje("Listo!", 5000, Color.Green, Color.Black);

            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            txtOC.Clear();
            txtCardCode.Clear();
            txtCardName.Clear();
            dgvDatos.DataSource = null;
        }

        private void btnAyuda_Click(object sender, EventArgs e)
        {
        }

        private void dgvDatos_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("su_Almacen", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 21);
                    command.Parameters.AddWithValue("@DocEntry", dgvDatos.ActiveRow.Cells["DocEntry"].Value);
                    command.Parameters.AddWithValue("@ObjType", dgvDatos.ActiveRow.Cells["ObjType"].Value);
                    command.Parameters.AddWithValue("@UserId", ClasesSGUV.Login.Id_Usuario);
                    command.Parameters.AddWithValue("@Comentarios", dgvDatos.ActiveRow.Cells["Comentarios recepción"].Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private void frmRecepcionDctos_Shown(object sender, EventArgs e)
        {
            if (!ClasesSGUV.Forms.GetPermisoReporte(this.Name))
            {
                MessageBox.Show("Usuario no autorizado. \r\nContacta al administrador.", "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvDatos.Selected.Rows.Count <= 0)
                {
                    this.SetMensaje("Seleccione una fila", 5000, Color.Red, Color.White);
                    return;
                }
                foreach (var item in dgvDatos.Selected.Rows)
                {
                    using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("su_Almacen", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@TipoConsulta", 32);
                            command.Parameters.AddWithValue("@DocEntry", item.Cells["DocEntry"].Value);
                            command.Parameters.AddWithValue("@ObjType", item.Cells["ObjType"].Value);
                            command.Parameters.AddWithValue("@FechaFinConteo", item.Cells["Fecha de finalización de conteo"].Value);
                            command.Parameters.AddWithValue("@UserId", ClasesSGUV.Login.Id_Usuario);
                            command.Parameters.AddWithValue("@WhsCode", ClasesSGUV.Login.Almacen);

                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }

                button1_Click(sender, e);
                this.SetMensaje("Listo!", 5000, Color.Green, Color.Black);

            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }
    }
}
