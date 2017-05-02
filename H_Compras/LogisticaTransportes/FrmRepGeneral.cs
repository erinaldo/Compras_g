using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Data.SqlClient;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;

namespace H_Compras.LogisticaTransportes
{
    public partial class FrmRepGeneral : Form
    {
        //public SqlConnection connection = new SqlConnection(@"Data Source=192.168.2.100;Initial Catalog=SBOLOGTRANSP;user id = sa; password = SAP-PJ1"/*ClasesSGUV.Propiedades.conectionSGUV*/);
        UIElement myUIElement;
        UltraGridCell myCell;
        ClasesSGUV.Logs log;
        public FrmRepGeneral()
        {
            InitializeComponent();
        }

        private void FrmRepGeneral_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dtAnios = new DataTable(); dtAnios.Columns.Add("Anio", typeof(Int32));
                for (int i = 2010; i <= 2099; i++)
                {
                    DataRow dr = dtAnios.NewRow(); dr["Anio"] = i; dtAnios.Rows.Add(dr);
                }
                cboAnio.DataSource = dtAnios; cboAnio.ValueMember = "Anio"; cboAnio.DisplayMember = "Anio";
                cboAnio.SelectedValue = DateTime.Now.Year;

                DataTable dtMeses = new DataTable(); dtMeses.Columns.Add("Mes", typeof(Int32)); dtMeses.Columns.Add("Descripcion", typeof(String));
                for (int i = 1; i <= 12; i++)
                {
                    DataRow dr = dtMeses.NewRow(); dr["Mes"] = i;
                    DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
                    string NameMont = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(dtinfo.GetMonthName(i));
                    dr["Descripcion"] = NameMont;

                    dtMeses.Rows.Add(dr);
                }
                cboMes.DataSource = dtMeses; cboMes.ValueMember = "Mes"; cboMes.DisplayMember = "Descripcion";
                cboMes.SelectedValue = DateTime.Now.Month;

                cboPlacas.DataSource = ConsultaPlacas();
                cboPlacas.ValueMember = "Codigo";
                cboPlacas.DisplayMember = "Nombre";



                log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            
        }

        private DataTable ConsultaPlacas()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_Logistica_Transportes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 3);
                        command.CommandTimeout = 0;
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return dt;
        }

       



        private void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboAnio.SelectedIndex == -1) { MessageBox.Show("Debe seleccionar un año", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (cboMes.SelectedIndex == -1) { MessageBox.Show("Debe seleccionar un mes", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (cboPlacas.SelectedIndex == -1) { MessageBox.Show("Debe seleccionar una placa", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                DataTable dtRepGen = new DataTable();
                DateTime Desde = new DateTime(Convert.ToInt32(cboAnio.SelectedValue), Convert.ToInt32(cboMes.SelectedValue), 01, 00,00,00);
                DateTime Hasta = new DateTime(Convert.ToInt32(cboAnio.SelectedValue), Convert.ToInt32(cboMes.SelectedValue), 
                                              DateTime.DaysInMonth(Convert.ToInt32(cboAnio.SelectedValue), Convert.ToInt32(cboMes.SelectedValue))
                                              ,23,59,59);

                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_Logistica_Transportes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 4);
                        
                        command.Parameters.AddWithValue("@Desde", Desde);
                        command.Parameters.AddWithValue("@Hasta", Hasta);
                        command.Parameters.AddWithValue("@Placa", cboPlacas.SelectedValue.ToString());
                        command.CommandTimeout = 0;
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dtRepGen);
                    }
                } 

                dgvDatos.DataSource = dtRepGen;




                /*************************SE CONSULTAN LOS VALORES DE COSTOS*******************************/
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_Logistica_Transportes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 6);

                        command.Parameters.AddWithValue("@Anio", Convert.ToInt32(cboAnio.SelectedValue));
                        command.Parameters.AddWithValue("@Mes", Convert.ToInt32(cboMes.SelectedValue));
                        command.Parameters.AddWithValue("@Placa", cboPlacas.SelectedValue.ToString());
                        command.CommandTimeout = 0;
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            txtCOp.Value = Convert.IsDBNull(dt.Rows[0]["mCostoOpe"]) ? 0 : Convert.ToDecimal(dt.Rows[0]["mCostoOpe"]);
                            txtCMant.Value = Convert.IsDBNull(dt.Rows[0]["mCostoMto"]) ? 0 : Convert.ToDecimal(dt.Rows[0]["mCostoMto"]);
                            txtCDep.Value = Convert.IsDBNull(dt.Rows[0]["mCostoDep"]) ? 0 : Convert.ToDecimal(dt.Rows[0]["mCostoDep"]);
                        }
                        else
                        {
                            txtCOp.Value = 0;
                            txtCMant.Value = 0;
                            txtCDep.Value = 0;
                        }
                    }
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGridBand band = (sender as UltraGrid).DisplayLayout.Bands[0];
            band.Groups.Clear();
            e.Layout.Bands[0].Summaries.Clear();

            //e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Ruta";
            //e.Layout.Bands[0].Columns["ItemCode"].Width = 100;
            e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;
            e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Ruta";
            e.Layout.Bands[0].Columns["ItemName"].Width = 200;

            e.Layout.Bands[0].Columns["ValorFacturado"].Header.Caption = "Valor Facturado";
            e.Layout.Bands[0].Columns["ValorFacturado"].Format = "N2";
            e.Layout.Bands[0].Columns["ValorFacturado"].CellAppearance.TextHAlign = HAlign.Right;
            SummarySettings summarym1 = band.Summaries.Add("ValorFacturado", SummaryType.Sum, band.Columns["ValorFacturado"], SummaryPosition.UseSummaryPositionColumn);
            summarym1.DisplayFormat = "{0:c}"; summarym1.Appearance.TextHAlign = HAlign.Right; summarym1.Appearance.BackColor = Color.BurlyWood;
            e.Layout.Bands[0].Columns["ValorFacturado"].Width = 110;

            e.Layout.Bands[0].Columns["KilometrosRecorridos"].Header.Caption = "Kilometros Recorridos *";
            e.Layout.Bands[0].Columns["KilometrosRecorridos"].Format = "N";
            e.Layout.Bands[0].Columns["KilometrosRecorridos"].CellAppearance.TextHAlign = HAlign.Right;
            SummarySettings summarym2 = band.Summaries.Add("KilometrosRecorridos", SummaryType.Sum, band.Columns["KilometrosRecorridos"], SummaryPosition.UseSummaryPositionColumn);
            summarym2.DisplayFormat = "{0:c}"; summarym2.Appearance.TextHAlign = HAlign.Right; summarym2.Appearance.BackColor = Color.BurlyWood;
            e.Layout.Bands[0].Columns["KilometrosRecorridos"].Width = 110;

            e.Layout.Bands[0].Columns["Combustible"].Header.Caption = "Combustible (Tarjeta)**";
            e.Layout.Bands[0].Columns["Combustible"].Format = "N2";
            e.Layout.Bands[0].Columns["Combustible"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["Combustible"].Width = 110;

            e.Layout.Bands[0].Columns["Casetas"].Header.Caption = "Casetas (Tarjeta)**";
            e.Layout.Bands[0].Columns["Casetas"].Format = "N2";
            e.Layout.Bands[0].Columns["Casetas"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["Casetas"].Width = 110;

            e.Layout.Bands[0].Columns["CostoOperador"].Header.Caption = "Costo del Operador";
            e.Layout.Bands[0].Columns["CostoOperador"].Format = "N2";
            e.Layout.Bands[0].Columns["CostoOperador"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["CostoOperador"].Width = 110;

            e.Layout.Bands[0].Columns["CostoPropMto"].Header.Caption = "Costo proporcional de Mantenimiento";
            e.Layout.Bands[0].Columns["CostoPropMto"].Format = "N2";
            e.Layout.Bands[0].Columns["CostoPropMto"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["CostoPropMto"].Width = 130;

            e.Layout.Bands[0].Columns["CostoPropDep"].Header.Caption = "Costo proporcional por Depreciación";
            e.Layout.Bands[0].Columns["CostoPropDep"].Format = "N2";
            e.Layout.Bands[0].Columns["CostoPropDep"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["CostoPropDep"].Width = 130;

            e.Layout.Bands[0].Columns["CostoTotal"].Header.Caption = "Costo total del viaje";
            e.Layout.Bands[0].Columns["CostoTotal"].Format = "N2";
            e.Layout.Bands[0].Columns["CostoTotal"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["CostoTotal"].Width = 120;

            e.Layout.Bands[0].Columns["UtilidadNeta"].Header.Caption = "Utilidad Neta";
            e.Layout.Bands[0].Columns["UtilidadNeta"].Format = "P2";
            e.Layout.Bands[0].Columns["UtilidadNeta"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["UtilidadNeta"].Width = 110;

            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Activation.NoEdit;
            }
            e.Layout.Bands[0].Override.HeaderAppearance.BackColor = Color.MidnightBlue;
            e.Layout.Bands[0].Override.HeaderAppearance.BackColor2 = Color.MidnightBlue;
            e.Layout.Bands[0].Override.HeaderAppearance.ForeColor = Color.White;
            e.Layout.Bands[0].SummaryFooterCaption = "";

            e.Layout.Override.AllowMultiCellOperations = AllowMultiCellOperation.Copy;
            e.Layout.Override.SummaryDisplayArea = SummaryDisplayAreas.Bottom;// SummaryDisplayAreas.Top;
            e.Layout.Override.SummaryFooterAppearance.BackColor = Color.BurlyWood;
            e.Layout.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;
        }

        private void dgvDatos_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            
        }

        private void txtCOp_ValueChanged(object sender, EventArgs e)
        {
            decimal TotalKMRecorridos = 0;
            foreach (var item in dgvDatos.Rows)
            {   TotalKMRecorridos += Convert.IsDBNull(item.Cells["KilometrosRecorridos"].Value) ? 0 : Convert.ToDecimal(item.Cells["KilometrosRecorridos"].Value);   }
            decimal CostOP = Convert.ToDecimal(txtCOp.Value);

            foreach (var item in dgvDatos.Rows)
            {
                if (TotalKMRecorridos > 0)
                {
                    decimal LineCOP = (CostOP / TotalKMRecorridos) * (Convert.IsDBNull(item.Cells["KilometrosRecorridos"].Value) ? 0 : Convert.ToDecimal(item.Cells["KilometrosRecorridos"].Value));
                    item.Cells["CostoOperador"].Value = LineCOP;

                    decimal mFact=0, mCOpe = 0, mCMto = 0, mCDep = 0, mTotal=0, mPorcent = 0;
                    mFact = Convert.IsDBNull(item.Cells["ValorFacturado"].Value) ? 0 : Convert.ToDecimal(item.Cells["ValorFacturado"].Value);
                    mCOpe = Convert.IsDBNull(item.Cells["CostoOperador"].Value) ? 0 : Convert.ToDecimal(item.Cells["CostoOperador"].Value);
                    mCMto = Convert.IsDBNull(item.Cells["CostoPropMto"].Value) ? 0 : Convert.ToDecimal(item.Cells["CostoPropMto"].Value);
                    mCDep = Convert.IsDBNull(item.Cells["CostoPropDep"].Value) ? 0 : Convert.ToDecimal(item.Cells["CostoPropDep"].Value);
                    mTotal = mCOpe + mCMto + mCDep;
                    item.Cells["CostoTotal"].Value = mTotal;

                    if (mFact > 0)
                    {
                        mPorcent = ((mFact - mTotal) / mFact);
                        item.Cells["UtilidadNeta"].Value = mPorcent;
                    }
                    else { item.Cells["UtilidadNeta"].Value = 0; }


                }
            }
        }

        private void txtCMant_ValueChanged(object sender, EventArgs e)
        {
            decimal TotalKMRecorridos = 0;
            foreach (var item in dgvDatos.Rows)
            { TotalKMRecorridos += Convert.IsDBNull(item.Cells["KilometrosRecorridos"].Value) ? 0 : Convert.ToDecimal(item.Cells["KilometrosRecorridos"].Value); }
            decimal CostMtt = Convert.ToDecimal(txtCMant.Value);

            foreach (var item in dgvDatos.Rows)
            {
                if (TotalKMRecorridos > 0)
                {
                    decimal LineCMT = (CostMtt / TotalKMRecorridos) * (Convert.IsDBNull(item.Cells["KilometrosRecorridos"].Value) ? 0 : Convert.ToDecimal(item.Cells["KilometrosRecorridos"].Value));
                    item.Cells["CostoPropMto"].Value = LineCMT;

                    decimal mFact = 0, mCOpe = 0, mCMto = 0, mCDep = 0, mTotal = 0, mPorcent = 0;
                    mFact = Convert.IsDBNull(item.Cells["ValorFacturado"].Value) ? 0 : Convert.ToDecimal(item.Cells["ValorFacturado"].Value);
                    mCOpe = Convert.IsDBNull(item.Cells["CostoOperador"].Value) ? 0 : Convert.ToDecimal(item.Cells["CostoOperador"].Value);
                    mCMto = Convert.IsDBNull(item.Cells["CostoPropMto"].Value) ? 0 : Convert.ToDecimal(item.Cells["CostoPropMto"].Value);
                    mCDep = Convert.IsDBNull(item.Cells["CostoPropDep"].Value) ? 0 : Convert.ToDecimal(item.Cells["CostoPropDep"].Value);
                    mTotal = mCOpe + mCMto + mCDep;
                    item.Cells["CostoTotal"].Value = mTotal;

                    if (mFact > 0)
                    {
                        mPorcent = ((mFact - mTotal) / mFact);
                        item.Cells["UtilidadNeta"].Value = mPorcent;
                    }
                    else { item.Cells["UtilidadNeta"].Value = 0; }
                }
            }
        }

        private void txtCDep_ValueChanged(object sender, EventArgs e)
        {
            decimal TotalKMRecorridos = 0;
            foreach (var item in dgvDatos.Rows)
            { TotalKMRecorridos += Convert.IsDBNull(item.Cells["KilometrosRecorridos"].Value) ? 0 : Convert.ToDecimal(item.Cells["KilometrosRecorridos"].Value); }
            decimal CostDep = Convert.ToDecimal(txtCDep.Value);

            foreach (var item in dgvDatos.Rows)
            {
                if (TotalKMRecorridos > 0)
                {
                    decimal LineCDep = (CostDep / TotalKMRecorridos) * (Convert.IsDBNull(item.Cells["KilometrosRecorridos"].Value) ? 0 : Convert.ToDecimal(item.Cells["KilometrosRecorridos"].Value));
                    item.Cells["CostoPropDep"].Value = LineCDep;

                    decimal mFact = 0, mCOpe = 0, mCMto = 0, mCDep = 0, mTotal = 0, mPorcent = 0;
                    mFact = Convert.IsDBNull(item.Cells["ValorFacturado"].Value) ? 0 : Convert.ToDecimal(item.Cells["ValorFacturado"].Value);
                    mCOpe = Convert.IsDBNull(item.Cells["CostoOperador"].Value) ? 0 : Convert.ToDecimal(item.Cells["CostoOperador"].Value);
                    mCMto = Convert.IsDBNull(item.Cells["CostoPropMto"].Value) ? 0 : Convert.ToDecimal(item.Cells["CostoPropMto"].Value);
                    mCDep = Convert.IsDBNull(item.Cells["CostoPropDep"].Value) ? 0 : Convert.ToDecimal(item.Cells["CostoPropDep"].Value);
                    mTotal = mCOpe + mCMto + mCDep;
                    item.Cells["CostoTotal"].Value = mTotal;

                    if (mFact > 0)
                    {
                        mPorcent = ((mFact - mTotal) / mFact);
                        item.Cells["UtilidadNeta"].Value = mPorcent;
                    }
                    else { item.Cells["UtilidadNeta"].Value = 0; }
                }
            }
        }

        private void txtCOp_Click(object sender, EventArgs e)
        {
            txtCOp.SelectAll();
        }
        private void txtCOp_Enter(object sender, EventArgs e)
        {
            txtCOp.SelectAll();
        }

        private void txtCMant_Click(object sender, EventArgs e)
        {
            txtCMant.SelectAll();
        }
        private void txtCMant_Enter(object sender, EventArgs e)
        {
            txtCMant.SelectAll();
        }

        private void txtCDep_Click(object sender, EventArgs e)
        {
            txtCDep.SelectAll();
        }

        private void txtCDep_Enter(object sender, EventArgs e)
        {
            txtCDep.SelectAll();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Esta apunto de guardar los costos con la informacion actual, ¿Seguro que desea continuar?", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                {
                    GuardarValores(Convert.ToInt32(cboAnio.SelectedValue), Convert.ToInt32(cboMes.SelectedValue), cboPlacas.SelectedValue.ToString(),
                        txtCOp.Value, txtCMant.Value, txtCDep.Value);
                    MessageBox.Show("Valores guardados correctamente", "Informe - Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void GuardarValores(int Anio, int Mes, string Filtro, decimal CostoOpe, decimal CostoMant, decimal CostoDep)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_Logistica_Transportes", connection))
                    {
                        command.Connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 5);
                        command.Parameters.AddWithValue("@Anio", Anio);
                        command.Parameters.AddWithValue("@Mes", Mes);
                        command.Parameters.AddWithValue("@Placa", Filtro);
                        command.Parameters.AddWithValue("@CostOpe", CostoOpe);
                        command.Parameters.AddWithValue("@CostMto", CostoMant);
                        command.Parameters.AddWithValue("@CostDep", CostoDep);
                        command.ExecuteNonQuery();
                        command.Connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void FrmRepGeneral_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void FrmRepGeneral_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                log.Fin();
            }
            catch (Exception)
            {

            }
        }
    }
}
