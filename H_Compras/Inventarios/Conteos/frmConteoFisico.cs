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

namespace H_Compras.Inventarios.Conteos
{
    public partial class frmConteoFisico : Constantes.frmEmpty
    {
        private int folio = 0;
        bool isCiego = true;
        string Estado = "";
        string Tipo = "";
        string WhsCode = "";
        DateTime? UltimoConteo;
        int cont_ ;
        string msg = string.Empty;
        int alert = 0;
        DataTable tbl_Lotes = new DataTable();
        
        public int Folio
        {
            get { return folio; }
            set { folio = value; }
        }

        public frmConteoFisico()
        {
            InitializeComponent();
        }

        public frmConteoFisico(int folio_)
        {
            Folio = folio_;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 5);
                        command.Parameters.AddWithValue("@idConteo", Folio);
                        command.Parameters.AddWithValue("@Estado", "M");
                        SqlDataAdapter da = new SqlDataAdapter(command);

                        connection.Open();
                        command.ExecuteNonQuery();

                        this.OnLoad(e);
                    }
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje("ERROR: " + ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            #region Encabezados
            e.Layout.Bands[0].Columns["LineNum"].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns["ItemName"].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns["ItmsGrpNam"].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns["OnHand1"].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns["OnHand2"].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns["OnHand3"].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns["Clasificacion"].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns["U_Ubicacion"].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns["BuyUnitmsr"].CellActivation = Activation.NoEdit;

            e.Layout.Bands[0].Columns["idConteo"].Hidden = true;
            e.Layout.Bands[0].Columns["WhsCode"].Hidden = true;
            e.Layout.Bands[0].Columns["Step"].Hidden = true;
            e.Layout.Bands[0].Columns["LineNum"].Width = 50;
            e.Layout.Bands[0].Columns["Clasificacion"].Width = 80;
            e.Layout.Bands[0].Columns["BuyUnitmsr"].Width = 80;
            e.Layout.Bands[0].Columns["LineNum"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["Tipo"].Hidden = true;

            e.Layout.Bands[0].Columns["LineNum"].Header.Caption = "Num";
            e.Layout.Bands[0].Columns["Comment"].Header.Caption = "Comentario";
            e.Layout.Bands[0].Columns["ItmsGrpNam"].Header.Caption = "Línea";
            e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Artículo";
            e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Descripción";
            e.Layout.Bands[0].Columns["Conteo1"].Header.Caption = "Conteo #1";
            e.Layout.Bands[0].Columns["OnHand1"].Header.Caption = "Stock #1";
            e.Layout.Bands[0].Columns["Diferencia1"].Header.Caption = "Diferencia";
            e.Layout.Bands[0].Columns["Conteo2"].Header.Caption = "Conteo #2";
            e.Layout.Bands[0].Columns["OnHand2"].Header.Caption = "Stock #2";
            e.Layout.Bands[0].Columns["Diferencia2"].Header.Caption = "Diferencia";
            e.Layout.Bands[0].Columns["Conteo3"].Header.Caption = "Conteo #3";
            e.Layout.Bands[0].Columns["OnHand3"].Header.Caption = "Stock #3";
            e.Layout.Bands[0].Columns["Diferencia3"].Header.Caption = "Diferencia";
            e.Layout.Bands[0].Columns["Clasificacion"].Header.Caption = "ABC";
            e.Layout.Bands[0].Columns["U_Ubicacion"].Header.Caption = "Ubicación";

            e.Layout.Bands[0].Columns["BuyUnitmsr"].Header.Caption = "Unidad de medida";

            e.Layout.Bands[0].Columns["OnHand1"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["OnHand2"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["OnHand3"].CellAppearance.TextHAlign = HAlign.Right;

            e.Layout.Bands[0].Columns["Diferencia1"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["Diferencia2"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["Diferencia3"].CellAppearance.TextHAlign = HAlign.Right;
            #endregion

            if (isCiego)
            {
                e.Layout.Bands[0].Columns["OnHand1"].Hidden = true;
                e.Layout.Bands[0].Columns["OnHand2"].Hidden = true;
                e.Layout.Bands[0].Columns["OnHand3"].Hidden = true;

                e.Layout.Bands[0].Columns["Diferencia1"].Hidden = true;
                e.Layout.Bands[0].Columns["Diferencia2"].Hidden = true;
                e.Layout.Bands[0].Columns["Diferencia3"].Hidden = true;
            }

            if (Estado == "A" | Estado == "C" | Estado == "V" | Estado == "J")
            {
                e.Layout.Bands[0].Columns["Conteo1"].CellActivation = Activation.NoEdit;
                e.Layout.Bands[0].Columns["Conteo2"].CellActivation = Activation.NoEdit;
                e.Layout.Bands[0].Columns["Conteo3"].CellActivation = Activation.NoEdit;
            }

            if (ClasesSGUV.Login.Rol == (int)ClasesSGUV.Propiedades.RolesHalcoNET.InventariosAuxiliar)
                if (Estado == "J")
                {
                    e.Layout.Bands[0].Columns["Conteo1"].CellActivation = Activation.NoEdit;
                    e.Layout.Bands[0].Columns["Conteo2"].CellActivation = Activation.NoEdit;
                    e.Layout.Bands[0].Columns["Conteo3"].CellActivation = Activation.NoEdit;
                }

            #region solicitud de eduardo solo se muestra un conteo
            e.Layout.Bands[0].Columns["OnHand2"].Hidden = true;
            e.Layout.Bands[0].Columns["OnHand3"].Hidden = true;
            e.Layout.Bands[0].Columns["Diferencia2"].Hidden = true;
            e.Layout.Bands[0].Columns["Diferencia3"].Hidden = true;
            e.Layout.Bands[0].Columns["Conteo2"].Hidden = true;
            e.Layout.Bands[0].Columns["Conteo3"].Hidden = true;
            #endregion

            Infragistics.Win.UltraWinCalcManager.UltraCalcManager calcManager;
            calcManager = new Infragistics.Win.UltraWinCalcManager.UltraCalcManager(this.Container);
            e.Layout.Grid.CalcManager = calcManager;
            e.Layout.Bands[0].Columns["Diferencia1"].Formula = "[Conteo1] - [OnHand1]";
            e.Layout.Bands[0].Columns["Diferencia2"].Formula = "[Conteo2] - [OnHand2]";
            e.Layout.Bands[0].Columns["Diferencia3"].Formula = "[Conteo3] - [OnHand3]";

            e.Layout.Bands[0].Columns["Diferencia1"].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns["Diferencia2"].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns["Diferencia3"].CellActivation = Activation.NoEdit;

            e.Layout.Bands[0].Columns["ItemCode"].ValueList = ultraDropDown1;
        }

        private void dgvDatos_AfterCellListCloseUp(object sender, CellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Index > -1)
                {
                    if (e.Cell.Column.Key.Equals("ItemCode"))
                    {
                        dgvDatos.Rows[e.Cell.Row.Index].Cells["ItemName"].Value = ultraDropDown1.SelectedRow.Cells["ItemName"].Value;
                        dgvDatos.Rows[e.Cell.Row.Index].Cells["ItmsGrpNam"].Value = ultraDropDown1.SelectedRow.Cells["ItmsGrpNam"].Value;
                        dgvDatos.Rows[e.Cell.Row.Index].Cells["WhsCode"].Value = ultraDropDown1.SelectedRow.Cells["WhsCode"].Value;
                    }
                }
            }
            catch (Exception) { }
        }

        private void btn_Print(object sender, EventArgs e)
        {
            frmAlert2 formulario = new frmAlert2();
            if (formulario.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
            {
                if (formulario.cbLineas.Checked)
                {
                    if (isCiego)
                    {
                        Constantes.Clases.LoadRPT rpt = new Constantes.Clases.LoadRPT(@"\\192.168.2.100\HalcoNET\Crystal\listado_ciego_xLinea.rpt");
                        Constantes.frmVisor form = new Constantes.frmVisor(rpt.DocRPT);
                        rpt.GenerarPDF(Folio.ToString());
                        form.MdiParent = this.MdiParent;
                        form.Show();
                    }
                    else
                    {
                        Constantes.Clases.LoadRPT rpt = new Constantes.Clases.LoadRPT(@"\\192.168.2.100\HalcoNET\Crystal\listado_xLinea.rpt");
                        Constantes.frmVisor form = new Constantes.frmVisor(rpt.DocRPT);
                        rpt.GenerarPDF(Folio.ToString());
                        form.MdiParent = this.MdiParent;
                        form.Show();
                    }
                }
                else
                {
                    if (isCiego)
                    {
                        Constantes.Clases.LoadRPT rpt = new Constantes.Clases.LoadRPT(@"\\192.168.2.100\HalcoNET\Crystal\listado_conteo.rpt");
                        Constantes.frmVisor form = new Constantes.frmVisor(rpt.DocRPT);
                        rpt.GenerarPDF(Folio.ToString());
                        form.MdiParent = this.MdiParent;
                        form.Show();
                    }
                    else
                    {
                        Constantes.Clases.LoadRPT rpt = new Constantes.Clases.LoadRPT(@"\\192.168.2.100\HalcoNET\Crystal\listado_ciego_conteo.rpt");
                        Constantes.frmVisor form = new Constantes.frmVisor(rpt.DocRPT);
                        rpt.GenerarPDF(Folio.ToString());
                        form.MdiParent = this.MdiParent;
                        form.Show();
                    }
                }
            }
        }

        private void dgvDatos_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Header.Caption.Equals("Artículo"))
                dgvDatos_AfterCellListCloseUp(sender, e);

            if (e.Cell.Column.Header.Caption.Equals("Comentario") || e.Cell.Column.Header.Caption.Equals("Autorizado"))
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 10
                            );

                        command.Parameters.AddWithValue("@idConteo", Folio);
                        command.Parameters.AddWithValue("@ItemCode", dgvDatos.Rows[e.Cell.Row.Index].Cells["ItemCode"].Value);
                        command.Parameters.AddWithValue("@Aut", dgvDatos.Rows[e.Cell.Row.Index].Cells["Autorizado"].Value);
                        command.Parameters.AddWithValue("@Comment", dgvDatos.Rows[e.Cell.Row.Index].Cells["Comment"].Value);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private void ultraDropDown1_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[1].Hidden = true;
            e.Layout.Bands[0].Columns[2].Hidden = true;
            e.Layout.Bands[0].Columns[4].Hidden = true;

            e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Artículo";
            e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Descripción";
            e.Layout.Bands[0].Columns["ItemName"].Width = 380;
        }

        private void dgvDatos_BeforeRowUpdate(object sender, CancelableRowEventArgs e)
        {
            bool Update = e.Row.DataChanged;
            string msg = string.Empty;
            int alert = 0;

            try
            {
                if (Convert.ToDecimal(e.Row.Cells["idConteo"].Value == DBNull.Value ? decimal.Zero
                    : Convert.ToDecimal(e.Row.Cells["idConteo"].Value)) == decimal.Zero)
                {
                    #region insert
                    using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@TipoConsulta", 6);

                            command.Parameters.AddWithValue("@ItemCode", e.Row.Cells["ItemCode"].Text);
                            command.Parameters.AddWithValue("@WhsCode", e.Row.Cells["WhsCode"].Text);
                            command.Parameters.AddWithValue("@Comment", e.Row.Cells["Comment"].Text);

                            if (!string.IsNullOrEmpty(e.Row.Cells["Conteo1"].Text))
                                command.Parameters.AddWithValue("@Conteo1", e.Row.Cells["Conteo1"].Text);
                            if (!string.IsNullOrEmpty(e.Row.Cells["Conteo2"].Text))
                                command.Parameters.AddWithValue("@Conteo2", e.Row.Cells["Conteo2"].Text);
                            if (!string.IsNullOrEmpty(e.Row.Cells["Conteo3"].Text))
                                command.Parameters.AddWithValue("@Conteo3", e.Row.Cells["Conteo3"].Text);

                            command.Parameters.AddWithValue("@idConteo", Folio);
                            command.Parameters.AddWithValue("@UserID", ClasesSGUV.Login.Id_Usuario);
                            SqlParameter param = command.Parameters.Add("@Mensaje", SqlDbType.VarChar, 250);
                            param.Direction = ParameterDirection.Output;
                            SqlParameter param2 = command.Parameters.Add("@TipoAlert", SqlDbType.Int);
                            param2.Direction = ParameterDirection.Output;

                            DataTable tbl = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter(command);
                            da.Fill(tbl);

                            if (tbl.Rows.Count > 0)
                            {
                                e.Row.Cells["OnHand1"].Value = tbl.Rows[0]["OnHand1"];
                                e.Row.Cells["OnHand2"].Value = tbl.Rows[0]["OnHand2"];
                                e.Row.Cells["OnHand3"].Value = tbl.Rows[0]["OnHand3"];
                                e.Row.Cells["LineNum"].Value = tbl.Rows[0]["LineNum"];
                                e.Row.Cells["Step"].Value = tbl.Rows[0]["Step"];
                                e.Row.Cells["idConteo"].Value = tbl.Rows[0]["idConteo"];
                                e.Row.Cells["Autorizado"].Value = tbl.Rows[0]["Autorizado"];
                            }
                            msg = Convert.ToString(param.Value);
                            alert = Convert.ToInt32(param2.Value);
                        }
                    }
                    #endregion
                    this.SetMensaje(msg, 5000, alert == 1 ? Color.Red : Color.Green, alert == 1 ? Color.White : Color.Black);
                    if (alert == 1)
                        e.Cancel = true;
                }
                else
                {
                    if (Update)
                        using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                        {
                            using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@TipoConsulta", 6);

                                command.Parameters.AddWithValue("@ItemCode", e.Row.Cells["ItemCode"].Text);
                                command.Parameters.AddWithValue("@WhsCode", e.Row.Cells["WhsCode"].Text);
                                command.Parameters.AddWithValue("@Comment", e.Row.Cells["Comment"].Text);

                                #region col
                                int step = Convert.ToInt32(e.Row.Cells["Step"].Text);
                                command.Parameters.AddWithValue("@Col", step);

                                if (step == 1)
                                {
                                    if (!string.IsNullOrEmpty(e.Row.Cells["Conteo1"].Text))
                                        command.Parameters.AddWithValue("@Cantidad", e.Row.Cells["Conteo1"].Text);
                                }
                                else if (step == 2)
                                {
                                    if (!string.IsNullOrEmpty(e.Row.Cells["Conteo2"].Text))
                                        command.Parameters.AddWithValue("@Cantidad", e.Row.Cells["Conteo2"].Text);
                                }
                                else if (step >= 3)
                                {
                                    if (!string.IsNullOrEmpty(e.Row.Cells["Conteo3"].Text))
                                        command.Parameters.AddWithValue("@Cantidad", e.Row.Cells["Conteo3"].Text);
                                }
                                else return;

                                #endregion

                                SqlParameter param = command.Parameters.Add("@Mensaje", SqlDbType.VarChar, 250);
                                param.Direction = ParameterDirection.Output;
                                SqlParameter param2 = command.Parameters.Add("@TipoAlert", SqlDbType.Int);
                                param2.Direction = ParameterDirection.Output;

                                command.Parameters.AddWithValue("@idConteo", Folio);
                                command.Parameters.AddWithValue("@UserID", ClasesSGUV.Login.Id_Usuario);

                                DataTable tbl = new DataTable();
                                SqlDataAdapter da = new SqlDataAdapter(command);
                                da.Fill(tbl);

                                e.Row.Cells["OnHand1"].Value = tbl.Rows[0]["OnHand1"];
                                e.Row.Cells["OnHand2"].Value = tbl.Rows[0]["OnHand2"];
                                e.Row.Cells["OnHand3"].Value = tbl.Rows[0]["OnHand3"];

                                msg = Convert.ToString(param.Value);
                                alert = Convert.ToInt32(param2.Value);
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje("Error: " + ex.Message, 10000, Color.Red, Color.White);
                e.Row.CancelUpdate();
            }
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {

        }

        private void btnCerrarDocto_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Una vez cancelado el conteo, no se podrán hacer modificaciones.\r\n¿Desa continuar?",
                    "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("su_Conteo", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@TipoConsulta", 10);
                            command.Parameters.AddWithValue("@idConteo", Folio);

                            connection.Open();
                            command.ExecuteNonQuery();

                            btnAjustar.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void frmConteoFisico_Load(object sender, EventArgs e)
        {
            try
            {
                #region btn
                btnFinalizar.Enabled = false;
                btnAprobar.Enabled = false;
                btnAjustar.Enabled = false;
                btnCancelar.Enabled = false;
                #endregion

                this.WindowState = FormWindowState.Maximized;

                Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);
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
                this.imprimirToolStripButton.Enabled = true;

                this.imprimirToolStripButton.Click -= new EventHandler(btn_Print);
                this.imprimirToolStripButton.Click += new EventHandler(btn_Print);
               
                #endregion
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
                            btnIniciar.Visible = false;
                            btnAprobar.Visible = false;
                            btnAjustar.Visible = false;
                            btnCancelar.Visible = false;
                        }
                        break;
                }
                #region Permisos
                
                #endregion

                if (Folio > 0)
                {
                    txtFolio.Text = Folio.ToString();
                    using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@TipoConsulta", 3);
                            command.Parameters.AddWithValue("@idConteo", Folio);
                            SqlDataAdapter da = new SqlDataAdapter(command);

                            DataTable tbl = new DataTable();
                            da.Fill(tbl);
                            string whs = tbl.Rows[0].Field<string>("WhsCode") + " - " + tbl.Rows[0].Field<string>("WhsName");
                            txtAlmacen.Text = whs;
                            btnIniciar.Enabled = !(tbl.Rows[0].Field<string>("Estatus") == "M" || tbl.Rows[0].Field<string>("Estatus") == "V");
                            isCiego = tbl.Rows[0].Field<bool>("Ciego");
                            Estado = tbl.Rows[0].Field<string>("Estatus");
                            Tipo = tbl.Rows[0].Field<string>("Tipo");
                            WhsCode = tbl.Rows[0].Field<string>("WhsCode");
                            UltimoConteo = tbl.Rows[0].Field<DateTime?>("UltimoConteo");
                            cont_ = tbl.Rows[0].Field<Int32>("cont");

                            if (UltimoConteo != null)
                                txtUltimoConteo.Text = "Ultimo conteo: " + UltimoConteo.Value.ToShortDateString();

                            string status = tbl.Rows[0].Field<string>("Estatus");

                            if (Tipo.Equals("A"))
                            {
                                txtStep.Text = tbl.Rows[0].Field<string>("Avance");
                            }

                            #region solicitud de Eduardo, solo un conteo
                            if (cont_ == 2)
                            {
                                btnDiferencias.Enabled = true;
                                btnCancelar.Enabled = true;
                                btnAprobar.Enabled = true;
                            }
                            #endregion

                             if (status.Equals("A"))
                            {
                                btnFinalizar.Enabled = true;
                                btnCancelar.Enabled = true;
                            }
                            else if (status.Equals("M"))
                            {
                                btnFinalizar.Enabled = true;
                                btnDiferencias.Enabled = true;
                                btnCancelar.Enabled = true;
                                btnAprobar.Enabled = true;
                            }
                            else if (status.Equals("V"))
                            {
                                btnAprobar.Enabled = false;

                                btnAjustar.Enabled = true;
                                btnCancelar.Enabled = true;
                                dgvDatos.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
                            }
                            else if (status.Equals("C"))
                            {
                                btnIniciar.Enabled = false;
                                dgvDatos.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
                            }
                            else if (status.Equals("J"))
                            {
                                btnIniciar.Enabled = false;
                                dgvDatos.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
                            }
                        }
                    }

                    using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@TipoConsulta", 4);
                            command.Parameters.AddWithValue("@idConteo", Folio);
                            SqlDataAdapter da = new SqlDataAdapter(command);

                            DataTable tbl = new DataTable();
                            da.Fill(tbl);
                            dgvDatos.DataSource = tbl;
                        }
                    }

                    DataTable tbl_Items = new DataTable();
                    tbl_Items = ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ItemsConteoInventario, WhsCode, string.Empty);

                    ultraDropDown1.SetDataBinding(tbl_Items, null);
                    ultraDropDown1.ValueMember = "ItemCode";
                    ultraDropDown1.DisplayMember = "ItemCode";
                }

                btnAjustar.Enabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDatos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Index < -1 | e.Row.IsAddRow)
                return;

            if (e.Row.Cells["idConteo"].Value != DBNull.Value)
                e.Row.Cells["ItemCode"].Activation = Activation.NoEdit;

            if (Convert.ToInt32(e.Row.Cells["Step"].Value) == 1)
            {
                e.Row.Cells["Conteo1"].Activation = Activation.AllowEdit;
                e.Row.Cells["Conteo2"].Activation = Activation.NoEdit;
                e.Row.Cells["Conteo3"].Activation = Activation.NoEdit;
                e.Row.Cells["Conteo1"].Appearance.BackColor = Color.FromName("Info");
            }
            else if (Convert.ToInt32(e.Row.Cells["Step"].Value) == 2)
            {
                e.Row.Cells["Conteo1"].Activation = Activation.NoEdit;
                e.Row.Cells["Conteo2"].Activation = Activation.AllowEdit;
                e.Row.Cells["Conteo3"].Activation = Activation.NoEdit;
                e.Row.Cells["Conteo2"].Appearance.BackColor = Color.FromName("Info");
            }
            else if (Convert.ToInt32(e.Row.Cells["Step"].Value) >= 3)
            {
                e.Row.Cells["Conteo1"].Activation = Activation.NoEdit;
                e.Row.Cells["Conteo2"].Activation = Activation.NoEdit;
                e.Row.Cells["Conteo3"].Activation = Activation.AllowEdit;
                e.Row.Cells["Conteo3"].Appearance.BackColor = Color.FromName("Info");
            }
            else
            {
                e.Row.Cells["Conteo1"].Activation = Activation.NoEdit;
                e.Row.Cells["Conteo2"].Activation = Activation.NoEdit;
                e.Row.Cells["Conteo3"].Activation = Activation.NoEdit;
            }
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            try
            {
                int step = Convert.ToInt32((dgvDatos.DataSource as DataTable).Rows[0]["Step"]);
                if (step > 3) step = 3;

                if (MessageBox.Show("Una vez cerrado el #Conteo " + step + ", no se podrán hacer modificaciones.\r\nSolo las cantidades con diferencias pasarán al siguiente conteo.\r\n¿Desa continuar?",
                    "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@TipoConsulta", 7);
                            command.Parameters.AddWithValue("@idConteo", Folio);
                            command.Parameters.AddWithValue("@UserID", ClasesSGUV.Login.Id_Usuario);
                            command.Parameters.AddWithValue("@Step", step);

                            SqlParameter param = command.Parameters.Add("@Mensaje", SqlDbType.VarChar, 250);
                            param.Direction = ParameterDirection.Output;
                            SqlParameter param2 = command.Parameters.Add("@TipoAlert", SqlDbType.Int);
                            param2.Direction = ParameterDirection.Output;

                            connection.Open();
                            command.ExecuteNonQuery();

                            msg = Convert.ToString(param.Value);
                            alert = Convert.ToInt32(param2.Value);

                            this.SetMensaje(msg, 5000, alert == 1 ? Color.Red : Color.Green, alert == 1 ? Color.White : Color.Black);

                            OnLoad(e);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void btnDiferencias_Click(object sender, EventArgs e)
        {
            int step = Convert.ToInt32((dgvDatos.DataSource as DataTable).Rows[0]["Step"]);
            if (step == 1)
            {
                SetMensaje("Debe Finalizar un conteo para poder calcualar diferencias", 8000, Color.Red, Color.White);
                return;
            }

            dgvDatos.Rows.ColumnFilters["Diferencia1"].FilterConditions.Clear();
            dgvDatos.Rows.ColumnFilters["Diferencia2"].FilterConditions.Clear();
            dgvDatos.Rows.ColumnFilters["Diferencia3"].FilterConditions.Clear();

            if (step == 2)
            {
                Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                myConditions.CompareValue = 0;
                myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.GreaterThan;
                dgvDatos.Rows.ColumnFilters["Diferencia1"].FilterConditions.Add(myConditions);
            }
            else if (step == 3)
            {
                Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                myConditions.CompareValue = 0;
                myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.GreaterThan;
                dgvDatos.Rows.ColumnFilters["Diferencia2"].FilterConditions.Add(myConditions);
            }
            else if (step >= 4)
            {
                Infragistics.Win.UltraWinGrid.FilterCondition myConditions = new Infragistics.Win.UltraWinGrid.FilterCondition();
                myConditions.CompareValue = 0;
                myConditions.ComparisionOperator = Infragistics.Win.UltraWinGrid.FilterComparisionOperator.GreaterThan;
                dgvDatos.Rows.ColumnFilters["Diferencia3"].FilterConditions.Add(myConditions);
            }
        }

        private void btnAprobar_Click(object sender, EventArgs e)
        {
            int step = Convert.ToInt32((dgvDatos.DataSource as DataTable).Rows[0]["Step"]);
            if (step == 1)
            {
                SetMensaje("Debe Finalizar un conteo para poder Aprobarlo", 8000, Color.Red, Color.White);
                return;
            }

            if (step > 3) step = 3;
            if (MessageBox.Show("Una vez Aprobado, No se podrán hacer cambios a la lista de conteo\r\n¿Desea continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == System.Windows.Forms.DialogResult.Yes)
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 5);
                        command.Parameters.AddWithValue("@idConteo", Folio);
                        command.Parameters.AddWithValue("@UserID", ClasesSGUV.Login.Id_Usuario);
                        command.Parameters.AddWithValue("@Estado", "V");

                        SqlParameter param = command.Parameters.Add("@Mensaje", SqlDbType.VarChar, 250);
                        param.Direction = ParameterDirection.Output;
                        SqlParameter param2 = command.Parameters.Add("@TipoAlert", SqlDbType.Int);
                        param2.Direction = ParameterDirection.Output;

                        connection.Open();
                        command.ExecuteNonQuery();

                        msg = Convert.ToString(param.Value);
                        alert = Convert.ToInt32(param2.Value);

                        this.SetMensaje(msg, 5000, alert == 1 ? Color.Red : Color.Green, alert == 1 ? Color.White : Color.Black);

                        OnLoad(e);
                    }
                }
            }

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("El listado de recuento se cancelará\r\n¿Desea continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == System.Windows.Forms.DialogResult.Yes)
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 5);
                        command.Parameters.AddWithValue("@idConteo", Folio);
                        command.Parameters.AddWithValue("@Estado", "C");

                        SqlParameter param = command.Parameters.Add("@Mensaje", SqlDbType.VarChar, 250);
                        param.Direction = ParameterDirection.Output;
                        SqlParameter param2 = command.Parameters.Add("@TipoAlert", SqlDbType.Int);
                        param2.Direction = ParameterDirection.Output;

                        connection.Open();
                        command.ExecuteNonQuery();

                        msg = Convert.ToString(param.Value);
                        alert = Convert.ToInt32(param2.Value);

                        this.SetMensaje(msg, 5000, alert == 1 ? Color.Red : Color.Green, alert == 1 ? Color.White : Color.Black);

                        OnLoad(e);
                    }
                }
            }
        }

        private void dgvDatos_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            e.DisplayPromptMsg = false;

            // Display our own custom message box.
            System.Windows.Forms.DialogResult result =
                System.Windows.Forms.MessageBox.Show(
                "Deleting " + e.Rows.Length.ToString() + " row(s). Continue ?",
                "Delete rows?",
                System.Windows.Forms.MessageBoxButtons.YesNo,
                System.Windows.Forms.MessageBoxIcon.Question);



            // If the user clicked No on the message box, cancel the deletion of rows.
            if (System.Windows.Forms.DialogResult.No == result)
            {
                e.Cancel = true; return;
            }

            foreach (var item in e.Rows)
            {
                if (item.Cells["Tipo"].Value.ToString().Equals("S"))
                {
                    e.Cancel = true;
                    this.SetMensaje("No se puede eliminar esta fila", 5000, Color.Red, Color.White);
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@TipoConsulta", 9);
                            command.Parameters.AddWithValue("@idConteo", Folio);
                            command.Parameters.AddWithValue("@ItemCode", item.Cells["ItemCode"].Value);

                            SqlParameter param = command.Parameters.Add("@Mensaje", SqlDbType.VarChar, 250);
                            param.Direction = ParameterDirection.Output;
                            SqlParameter param2 = command.Parameters.Add("@TipoAlert", SqlDbType.Int);
                            param2.Direction = ParameterDirection.Output;

                            connection.Open();
                            command.ExecuteNonQuery();

                            msg = Convert.ToString(param.Value);
                            alert = Convert.ToInt32(param2.Value);

                            this.SetMensaje(msg, 5000, alert == 1 ? Color.Red : Color.Green, alert == 1 ? Color.White : Color.Black);
                        }
                    }
                }
            }
        }

        private void dgvDatos_AfterRowsDeleted(object sender, EventArgs e)
        {
            foreach (var item in dgvDatos.Rows)
            {
                if (item.IsDeleted)
                {
                    MessageBox.Show(item.Cells["ItemCode"].Value.ToString());
                }
            }
        }

        private void btnAjustar_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Se crearán los ajustes en SAP, ¿Desea continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    != System.Windows.Forms.DialogResult.Yes)
                    return;
                decimal fol_Entrada = decimal.Zero;
                decimal fol_Salida = decimal.Zero;
                #region Generar Entradas
                //SDK_SAP.DI.Documents oDocumentEntrada = new SDK_SAP.DI.Documents(10010);
                //SDK_SAP.DI.Connection.InitializeConnection(10010);
                SDK_SAP.DI.Documents oDocumentEntrada = new SDK_SAP.DI.Documents(10010);
                SDK_SAP.DI.Connection.InitializeConnection(10010);
                SDK_SAP.DI.Connection.StartConnection();

                DataTable tbl_Entrada = new DataTable();
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 12);
                        command.Parameters.AddWithValue("@idConteo", Folio);

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(tbl_Entrada);
                    }
                }
                if (tbl_Entrada.Rows.Count > 0)
                {
                    SDK_SAP.Clases.Document docEntrada = new SDK_SAP.Clases.Document();
                    docEntrada.DocDate = DateTime.Now;
                    docEntrada.DocType = "dDocument_Items";
                    docEntrada.Comments = "Generado por HalcoNET " + DateTime.Now.ToString();
                    docEntrada.Series = 21;

                    this.SetMensaje("Generando Entradas de Mercancias");

                    foreach (DataRow item in tbl_Entrada.Rows)
                    {
                        SDK_SAP.Clases.DocumentLines line = new SDK_SAP.Clases.DocumentLines();
                        line.ItemCode = Convert.ToString(item["ItemCode"]);
                        line.Quantity = Convert.ToDouble(item["Quantity"]);
                        line.WarehouseCode = Convert.ToString(item["WhsCode"]);
                        line.AccountCode = Convert.ToString(item["AcctCode"]);
                        line.CostingCode = Convert.ToString(item["OcrCode"]);
                        line.U_TIPOMOV = Convert.ToString(item["U_TIPOMOV"]);
                        line.U_Comentario = Convert.ToString(item["Comment"]);
                        line.ManBtchNum = Convert.ToString(item["ManBtchNum"]);
                        line.Price = Convert.ToDouble(item["Price"]);

                        docEntrada.Lines.Add(line);
                    }

                    #region Lotes
                    bool viewLotes = false;
                    bool finalizar = true;
                    bool LotesOk = true;
                    viewLotes = false;
                    finalizar = true;
                    foreach (var item in docEntrada.Lines)
                    {
                        if (item.ManBtchNum.Equals("Y"))
                        {
                            if (item.Quantity != item.LotesList.Sum(lote_item => lote_item.Cantidad))
                            {
                                viewLotes = true;
                            }
                            foreach (var lote in item.LotesList)
                            {
                                if (!string.IsNullOrEmpty(lote.Pedimento))
                                    viewLotes = true;
                                if (!string.IsNullOrEmpty(lote.Aduana))
                                    viewLotes = true;
                            }
                        }
                    }
                    if (viewLotes)
                    {
                        SDK.Documentos.frmLotes form = new SDK.Documentos.frmLotes(docEntrada, tbl_Lotes, true);
                        if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                            return;
                        else
                            tbl_Lotes = form.Tbl_lotes;
                    }

                    foreach (var item in docEntrada.Lines)
                    {
                        if (item.ManBtchNum.Equals("Y"))
                        {
                            if (item.Quantity != item.LotesList.Sum(lote_item => lote_item.Cantidad))
                            {
                                finalizar = false;
                            }
                        }
                    }

                    foreach (var item in docEntrada.Lines)
                    {
                        if (item.ManBtchNum.Equals("Y"))
                        {
                            foreach (var lote in item.LotesList)
                            {
                                if (string.IsNullOrEmpty(lote.Pedimento))
                                    LotesOk = false;
                                if (string.IsNullOrEmpty(lote.Aduana))
                                    LotesOk = false;
                            }
                        }
                    }

                    if (!LotesOk)
                    {
                        this.SetMensaje("Falta ingresar informacion Aduana|Pedimento", 5000, Color.Red, Color.White);
                        return;
                    }

                    #endregion
                    if (finalizar)
                    {
                        fol_Entrada = oDocumentEntrada.AddDocument("OIGN", docEntrada);
                        SDK_SAP.DI.Connection.CloseConnection();

                        this.SetMensaje("Listo. Folio: " + fol_Entrada, 5000, Color.Green, Color.Black);
                    }
                }
                #endregion

                System.Threading.Thread.Sleep(5000);

                #region Generar Salidas
                //SDK_SAP.DI.Documents oDocumentSalida = new SDK_SAP.DI.Documents(2);
                //SDK_SAP.DI.Connection.InitializeConnection(2);
                //SDK_SAP.DI.Connection.StartConnection();

                SDK_SAP.DI.Documents oDocumentSalida = new SDK_SAP.DI.Documents(10010);
                SDK_SAP.DI.Connection.InitializeConnection(10010);
                SDK_SAP.DI.Connection.StartConnection();

                DataTable tbl_Salida = new DataTable();
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 11);
                        command.Parameters.AddWithValue("@idConteo", Folio);

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(tbl_Salida);
                    }
                }
                if (tbl_Salida.Rows.Count > 0)
                {

                    this.SetMensaje("Generando Salidas de Mercancias");
                    SDK_SAP.Clases.Document docSalida = new SDK_SAP.Clases.Document();
                    docSalida.DocDate = DateTime.Now;
                    docSalida.DocType = "dDocument_Items";
                    docSalida.Comments = "Generado por HalcoNET " + DateTime.Now.ToString();
                    docSalida.Series = 22;

                    foreach (DataRow item in tbl_Salida.Rows)
                    {
                        SDK_SAP.Clases.DocumentLines line = new SDK_SAP.Clases.DocumentLines();
                        line.ItemCode = Convert.ToString(item["ItemCode"]);
                        line.Quantity = Convert.ToDouble(item["Quantity"]);
                        line.WarehouseCode = Convert.ToString(item["WhsCode"]);
                        line.AccountCode = Convert.ToString(item["AcctCode"]);
                        line.CostingCode = Convert.ToString(item["OcrCode"]);
                        line.U_TIPOMOV = Convert.ToString(item["U_TIPOMOV"]);
                        line.U_Comentario = Convert.ToString(item["Comment"]);
                        line.Price = Convert.ToDouble(item["Price"]);

                        if (Convert.ToString(item["ManBtchNum"]).Equals("Y"))
                        {
                            SDK_SAP.Clases.Lotes oLote = new SDK_SAP.Clases.Lotes();
                            foreach (DataRow lote in oLote.getLotes(line.ItemCode, line.WarehouseCode, line.Quantity).Rows)
                            {
                                oLote.Lote = lote.Field<string>("BatchNum");
                                oLote.Cantidad = Convert.ToDouble(lote["Quantity"]);
                                line.LotesList.Add(oLote);
                            }
                        }

                        docSalida.Lines.Add(line);
                    }
                    fol_Salida = oDocumentSalida.AddDocument("OIGE", docSalida);
                    SDK_SAP.DI.Connection.CloseConnection();

                    this.SetMensaje("Listo. Folio: " + fol_Salida, 5000, Color.Green, Color.Black);

                    btnAjustar.Enabled = false;

                    using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("UPDATE tbl_ConteoHeader SET DocNumSalida = @sal, DocNumEntrada = @ent, Estatus = 'J' Where idConteo = @id", connection))
                        {
                            command.Parameters.AddWithValue("@sal", fol_Salida);
                            command.Parameters.AddWithValue("@ent", fol_Entrada);
                            command.Parameters.AddWithValue("@id", Folio);

                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
                #endregion

                System.Media.SystemSounds.Beep.Play();
                try
                {
                    using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("su_Conteos", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@TipoConsulta", 9);
                            command.Parameters.AddWithValue("@idConteo", Folio);
                            command.Parameters.AddWithValue("@Entrada", fol_Entrada);
                            command.Parameters.AddWithValue("@Salida", fol_Salida);

                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception){
                }
                                
                this.SetMensaje("Listo!", 8000, Color.Green, Color.Black);

                MessageBox.Show("Resumen:\r\nEntrada de mercancias: " + fol_Entrada + "\r\nSalida de mercancias: " + fol_Salida,
                    "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);

            }
        }
    }
}
