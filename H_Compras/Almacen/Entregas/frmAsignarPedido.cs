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

namespace H_Compras.Almacen.Entregas
{
    public partial class frmAsignarPedido : Constantes.frmEmpty
    {
        public frmAsignarPedido()
        {
            InitializeComponent();
        }

        private void frmAsignarPedido_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable almacenistasList = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListadoAlmacenistas, null, string.Empty);

                this.udbAlmacenistas.SetDataBinding(almacenistasList, null);
                this.udbAlmacenistas.ValueMember = "Nombre";
                this.udbAlmacenistas.DisplayMember = "Nombre";

                dgvDatos.KeyPress -= new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);
                dgvDatos.KeyPress += new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);

                dgvDatos.KeyDown -= new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);
                dgvDatos.KeyDown += new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);

                SDK_SAP.DI.Connection.InitializeConnection();
                SDK_SAP.DI.Connection.StartConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("su_Almacen", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TipoConsulta", 1);
                    command.Parameters.AddWithValue("@Desde", dtpDesde.Value);
                    command.Parameters.AddWithValue("@Hasta", dtpHasta.Value);

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable tbl = new DataTable();
                    da.Fill(tbl);

                    dgvDatos.DataSource = tbl;
                }
            }
        }

        private void timerConsult_Tick(object sender, EventArgs e)
        {
            btnConsultar_Click(sender, e);
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["DocNum"].Header.Caption = "Número de OV";
            e.Layout.Bands[0].Columns["CardCode"].Header.Caption = "Cliente";
            e.Layout.Bands[0].Columns["CardName"].Header.Caption = "Nombre";
            e.Layout.Bands[0].Columns["DocDate"].Header.Caption = "Fecha de contabilización";
            e.Layout.Bands[0].Columns["DocTime"].Header.Caption = "Inicio";
            e.Layout.Bands[0].Columns["Tiempo"].Header.Caption = "Tiempo (hh:mm:ss)";
            e.Layout.Bands[0].Columns["PymntGroup"].Header.Caption = "Condición de pago";
            e.Layout.Bands[0].Columns["Entregado"].Header.Caption = "Quitar";


            e.Layout.Bands[0].Columns["DocTime"].Format = "hh:mm tt";
            e.Layout.Bands[0].Columns["Tiempo"].Format = "hh:mm";

            e.Layout.Bands[0].Columns["DocTime"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
            e.Layout.Bands[0].Columns["Tiempo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

            e.Layout.Bands[0].Columns["DocEntry"].Hidden = true;
            e.Layout.Bands[0].Columns["IdAlmacenista"].Hidden = true;
            e.Layout.Bands[0].Columns["Series"].Hidden = true;
            e.Layout.Bands[0].Columns["U_TPedido"].Hidden = true;

            e.Layout.Bands[0].Columns["Entregado"].Width = 50;
            e.Layout.Bands[0].Columns["Prioridad"].Width = 83;
            e.Layout.Bands[0].Columns["DocNum"].Width = 90;
            e.Layout.Bands[0].Columns["Estatus"].Width = 90;
            e.Layout.Bands[0].Columns["CardCode"].Width = 80;
            e.Layout.Bands[0].Columns["DocDate"].Width = 105;
            e.Layout.Bands[0].Columns["PymntGroup"].Width = 120;
            e.Layout.Bands[0].Columns["CardName"].Width = 350;
            e.Layout.Bands[0].Columns["Asignado"].Width = 200;
            e.Layout.Bands[0].Columns["DocTime"].Width = 100;
            e.Layout.Bands[0].Columns["Tiempo"].Width = 100;

            e.Layout.Bands[0].Columns["Entregado"].Width = 60;
            e.Layout.Bands[0].Columns["Imprimir"].Width = 60;
            e.Layout.Bands[0].Columns["Imprimir"].Hidden = true;

            e.Layout.Bands[0].Columns["DocEntry"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["DocNum"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["CardCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["CardName"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["DocDate"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["DocTime"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["Tiempo"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["Estatus"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["PymntGroup"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            e.Layout.Bands[0].Columns["DocEntry"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["DocNum"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["CardCode"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["CardName"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["DocDate"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["DocTime"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["Tiempo"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["Asignado"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["Estatus"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
        }

        private void dgvDatos_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            e.Row.Height = 38;

            e.Row.Cells["Asignado"].ValueList = udbAlmacenistas;
            
            if (e.Row.Cells["U_TPedido"].Value.Equals("M"))
            {
                e.Row.Cells["Prioridad"].ButtonAppearance.Image = Properties.Resources.halcon_app_3;
                e.Row.Cells["Prioridad"].Appearance.ImageBackground = Properties.Resources.halcon_app_3;
            }
            else if (e.Row.Cells["U_TPedido"].Value.Equals("RU"))
            {
                e.Row.Cells["Prioridad"].ButtonAppearance.Image = Properties.Resources.halcon_app_2;
                e.Row.Cells["Prioridad"].Appearance.ImageBackground = Properties.Resources.halcon_app_2;
            }
            else
            {
                e.Row.Cells["Prioridad"].ButtonAppearance.Image = Properties.Resources.halcon_app_1;
                e.Row.Cells["Prioridad"].Appearance.ImageBackground = Properties.Resources.halcon_app_1;
            }

            e.Row.Cells["Prioridad"].ButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            e.Row.Cells["Prioridad"].ButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
            e.Row.Cells["Prioridad"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            e.Row.Cells["Prioridad"].ButtonAppearance.BorderColor = Color.CadetBlue;
            e.Row.Cells["Prioridad"].Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;

            e.Row.Cells["Entregado"].ButtonAppearance.Image = Properties.Resources.cancelar;
            e.Row.Cells["Entregado"].Appearance.ImageBackground = Properties.Resources.cancelar;
            e.Row.Cells["Entregado"].ButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            e.Row.Cells["Entregado"].ButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
            e.Row.Cells["Entregado"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            e.Row.Cells["Entregado"].ButtonAppearance.BorderColor = Color.CadetBlue;
            e.Row.Cells["Entregado"].Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Default;

            e.Row.Cells["Imprimir"].ButtonAppearance.Image = Properties.Resources.print;
            e.Row.Cells["Imprimir"].Appearance.ImageBackground = Properties.Resources.print;
            e.Row.Cells["Imprimir"].ButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            e.Row.Cells["Imprimir"].ButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
            e.Row.Cells["Imprimir"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            e.Row.Cells["Imprimir"].ButtonAppearance.BorderColor = Color.CadetBlue;
            e.Row.Cells["Imprimir"].Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Default;
        }

        private void udbAlmacenistas_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].ColHeadersVisible = false;

            e.Layout.Bands[0].Columns[0].Hidden = true;
            e.Layout.Bands[0].Columns[1].Width = 150;
            e.Layout.Bands[0].Columns[2].Hidden = true;

            e.Layout.Bands[0].Columns[1].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
        }

        private void udbAlmacenistas_FilterRow(object sender, Infragistics.Win.UltraWinGrid.FilterRowEventArgs e)
        {
            e.Row.Height = 50;
        }

        private void dgvDatos_AfterCellListCloseUp(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Index > -1)
                {
                    if (e.Cell.Column.Key.Equals("Asignado"))
                    {
                        if (udbAlmacenistas.SelectedRow.Cells["ID"].Value != DBNull.Value){
                            dgvDatos.Rows[e.Cell.Row.Index].Cells["IdAlmacenista"].Value = udbAlmacenistas.SelectedRow.Cells["ID"].Value.ToString();

                            dgvDatos.Rows[e.Cell.Row.Index].Cells["Estatus"].Value = "Proceso de surtido";
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        
        private void dgvDatos_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            frmDetalleOV form = new frmDetalleOV(Convert.ToInt32(e.Row.Cells["DocEntry"].Value));
            form.ShowDialog();
        }

        private void dgvDatos_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell.Row.Index > -1 && e.Cell.Band.Index == 0)
            {
                if (e.Cell.Column.Header.Caption == "Quitar")
                {
                   // if (!dgvDatos.Rows[e.Cell.Row.Index].Cells["Estatus"].Value.ToString().Equals("Surtido"))
                   //     return;

                    Almacen.Entregas.frmMensaje frm = new frmMensaje();
                    
                    if(frm.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                    {
                        SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog);
                        {
                            using (SqlCommand command = new SqlCommand("su_Almacen", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@TipoConsulta", 8);
                                command.Parameters.AddWithValue("@DocEntry", dgvDatos.Rows[e.Cell.Row.Index].Cells["DocEntry"].Value);
                                command.Parameters.AddWithValue("@Facturas", frm.Folios);

                                connection.Open();
                                command.ExecuteNonQuery();

                                btnConsultar_Click(sender, e);
                            }
                        }
                    }
                }
            }
            //if (e.Cell.Row.Index > -1 && e.Cell.Band.Index == 0)
            //{
            //    if (e.Cell.Column.Header.Caption == "Imprimir")
            //    {
            //        Constantes.Clases.LoadRPT rpt = new Constantes.Clases.LoadRPT(20011);
            //        Constantes.frmVisor form = new Constantes.frmVisor(rpt.DocRPT);
            //        rpt.Print(dgvDatos.Rows[e.Cell.Row.Index].Cells["DocEntry"].Value.ToString());
            //        //form.MdiParent = this.MdiParent;
            //        //form.Show();
            //    }
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("su_Almacen", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TipoConsulta", 9);
                    command.Parameters.AddWithValue("@Desde", dtp1.Value);
                    command.Parameters.AddWithValue("@Hasta", dtp2.Value);

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable tbl = new DataTable();
                    da.Fill(tbl);

                    dgvEntregadas.DataSource = tbl;
                }
            }
        }

        private void dgvEntregadas_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["DocNum"].Header.Caption = "Número de OV";
            e.Layout.Bands[0].Columns["CardCode"].Header.Caption = "Cliente";
            e.Layout.Bands[0].Columns["CardName"].Header.Caption = "Nombre";
            e.Layout.Bands[0].Columns["DocDate"].Header.Caption = "Fecha de contabilización";
            e.Layout.Bands[0].Columns["DocTime"].Header.Caption = "Hora de inicio";
            e.Layout.Bands[0].Columns["HoraSurtido"].Header.Caption = "Hora de surtido";
            e.Layout.Bands[0].Columns["HoraEntrega"].Header.Caption = "Hora de entrega";
            e.Layout.Bands[0].Columns["Tiempo"].Header.Caption = "Tiempo (hh:mm:ss)";
            e.Layout.Bands[0].Columns["Asignado"].Header.Caption = "Almacenista";

            e.Layout.Bands[0].Columns["DocTime"].Format = "hh:mm tt";
            e.Layout.Bands[0].Columns["HoraSurtido"].Format = "hh:mm tt";
            e.Layout.Bands[0].Columns["HoraEntrega"].Format = "hh:mm tt";

            e.Layout.Bands[0].Columns["Tiempo"].Format = "hh:mm";

            e.Layout.Bands[0].Columns["DocTime"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
            e.Layout.Bands[0].Columns["Tiempo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

            e.Layout.Bands[0].Columns["DocEntry"].Hidden = true;
            e.Layout.Bands[0].Columns["IdAlmacenista"].Hidden = true;
            e.Layout.Bands[0].Columns["Series"].Hidden = true;
            e.Layout.Bands[0].Columns["U_TPedido"].Hidden = true;

            e.Layout.Bands[0].Columns["Prioridad"].Width = 83;
            e.Layout.Bands[0].Columns["DocNum"].Width = 90;
            e.Layout.Bands[0].Columns["CardCode"].Width = 80;
            e.Layout.Bands[0].Columns["Titular"].Width = 110;
            e.Layout.Bands[0].Columns["DocDate"].Width = 105;
            e.Layout.Bands[0].Columns["CardName"].Width = 350;
            e.Layout.Bands[0].Columns["Asignado"].Width = 200;
            e.Layout.Bands[0].Columns["DocTime"].Width = 85;
            e.Layout.Bands[0].Columns["HoraSurtido"].Width = 85;
            e.Layout.Bands[0].Columns["HoraEntrega"].Width = 85;
            e.Layout.Bands[0].Columns["Tiempo"].Width = 80;

            e.Layout.Bands[0].Columns["DocEntry"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["Titular"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["DocNum"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["Asignado"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["CardCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["CardName"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["DocDate"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["DocTime"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["HoraSurtido"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["HoraEntrega"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            e.Layout.Bands[0].Columns["Tiempo"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;

            e.Layout.Bands[0].Columns["DocEntry"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["DocNum"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["CardCode"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["CardName"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["DocDate"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["DocTime"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["Tiempo"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["HoraSurtido"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["HoraEntrega"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            e.Layout.Bands[0].Columns["Asignado"].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
        }

        private void dgvEntregadas_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            e.Row.Height = 35;

            e.Row.Cells["Asignado"].ValueList = udbAlmacenistas;

            if (e.Row.Cells["U_TPedido"].Value.Equals("M"))
            {
                e.Row.Cells["Prioridad"].ButtonAppearance.Image = Properties.Resources.halcon_app_3;
                e.Row.Cells["Prioridad"].Appearance.ImageBackground = Properties.Resources.halcon_app_3;
            }
            else if (e.Row.Cells["U_TPedido"].Value.Equals("RU"))
            {
                e.Row.Cells["Prioridad"].ButtonAppearance.Image = Properties.Resources.halcon_app_2;
                e.Row.Cells["Prioridad"].Appearance.ImageBackground = Properties.Resources.halcon_app_2;
            }
            else
            {
                e.Row.Cells["Prioridad"].ButtonAppearance.Image = Properties.Resources.halcon_app_1;
                e.Row.Cells["Prioridad"].Appearance.ImageBackground = Properties.Resources.halcon_app_1;
            }
            
            e.Row.Cells["Prioridad"].ButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            e.Row.Cells["Prioridad"].ButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
            e.Row.Cells["Prioridad"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            e.Row.Cells["Prioridad"].ButtonAppearance.BorderColor = Color.CadetBlue;
            e.Row.Cells["Prioridad"].Appearance.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
        }

        private void dgvDatos_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            try
            {
                if (e.Cell.Row.Index > -1 && e.Cell.Column.Header.Caption.Equals("Asignado"))
                {
                    using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("su_Almacen", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@TipoConsulta", 3);
                            command.Parameters.AddWithValue("@DocEntry", dgvDatos.Rows[e.Cell.Row.Index].Cells["DocEntry"].Value);
                            command.Parameters.AddWithValue("@Asignado", dgvDatos.Rows[e.Cell.Row.Index].Cells["IdAlmacenista"].Value);
                            command.Parameters.AddWithValue("@UserID", ClasesSGUV.Login.Id_Usuario);

                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void dgvDatos_AfterRowActivate(object sender, EventArgs e)
        {
            foreach (var item in dgvDatos.Rows)
            {
                foreach (var item1 in item.Cells)
                {
                    item1.Appearance.BackColor = Color.White;
                }

                if (item.Cells["Estatus"].Value.Equals("Surtido"))
                {
                    item.Cells["Estatus"].Appearance.BackColor = Color.Green;
                }

                if (item.Cells["PymntGroup"].Value.Equals("Contado"))
                {
                    item.Cells["PymntGroup"].Appearance.BackColor = Color.Red;
                    item.Cells["PymntGroup"].Appearance.ForeColor = Color.White;
                }
            }

            foreach (var item in dgvDatos.ActiveRow.Cells)
            {
                item.Appearance.BackColor = Color.FromName("Info");
            }

            if (dgvDatos.ActiveRow.Cells["Estatus"].Value.Equals("Surtido"))
            {
                dgvDatos.ActiveRow.Cells["Estatus"].Appearance.BackColor = Color.Green;
            }

            if (dgvDatos.ActiveRow.Cells["PymntGroup"].Value.Equals("Contado"))
            {
                dgvDatos.ActiveRow.Cells["PymntGroup"].Appearance.BackColor = Color.Red;
                dgvDatos.ActiveRow.Cells["PymntGroup"].Appearance.ForeColor = Color.White;
            }
        }

        private void timerClock_Tick(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow item in dgvDatos.Rows)
            {
                try
                {
                    TimeSpan time1 = new TimeSpan(0, 0, 0, 10);
                    if (!item.Cells["Estatus"].Value.ToString().Equals("Surtido"))
                        item.Cells["Tiempo"].Value = ((TimeSpan)item.Cells["Tiempo"].Value) + time1;
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
