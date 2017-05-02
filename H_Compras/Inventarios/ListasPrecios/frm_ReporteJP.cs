using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.Inventarios.ListasPrecios
{
    public partial class frm_ReporteJP : Constantes.frmEmpty
    {
        public frm_ReporteJP()
        {
            InitializeComponent();
        }
        Datos.Connection connection = new Datos.Connection();

        private void frm_ReporteJP_Load(object sender, EventArgs e)
        {
            imprimirToolStripButton.Enabled = false;
            guardarToolStripButton.Enabled = false;
            actualizarToolStripButton.Enabled = false;

            try
            {
                ClasesSGUV.Form.ControlsForms.setDataSource(cbListasPrecios, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.ListasPrecios, ClasesSGUV.Login.Rol, string.Empty), "ListName", "ListNum", "Lista de precios");
                ClasesSGUV.Form.ControlsForms.setDataSource(cbLineas, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.LineasTodas, ClasesSGUV.Login.Rol, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "Línea");

                dgvDatos.KeyDown -= new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);
                dgvDatos.KeyDown += new KeyEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyDown);

                dgvDatos.KeyPress -= new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);
                dgvDatos.KeyPress += new KeyPressEventHandler(ClasesSGUV.Form.ControlsForms.UltraGrid_KeyPress);
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                dgvDatos.DataSource = null;

                Object[] valuesOut = new Object[] { };
                DataTable tbl =
                    connection.GetDataTable("LOG",
                                            "sp_ListaPrecios",
                                            new string[] { },
                                            new string[] { "@TipoConsulta", "@ItmsGrpCod" },
                                            ref valuesOut, 9, cbLineas.SelectedValue);
                foreach (System.Data.DataColumn col in tbl.Columns) col.ReadOnly = false; 
                dgvDatos.DataSource = tbl;
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["ItemName"].Width = 200;
            e.Layout.Bands[0].Columns["PFD"].Width = 90;

            e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Artículo";
            e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Descripción";
            e.Layout.Bands[0].Columns["ItmsGrpNam"].Header.Caption = "Línea";

            e.Layout.Bands[0].Columns["PFD"].Header.Caption = "Precio Final con descuento";

            e.Layout.Bands[0].Columns["FOMT"].Header.Caption = "Factor Original";
            e.Layout.Bands[0].Columns["FFMT"].Header.Caption = "Factor Final";
            e.Layout.Bands[0].Columns["POMT"].Header.Caption = "Precio Original";
            e.Layout.Bands[0].Columns["PFMT"].Header.Caption = "Precio Final";

            e.Layout.Bands[0].Columns["FOMM"].Header.Caption = "Factor Original";
            e.Layout.Bands[0].Columns["FFMM"].Header.Caption = "Factor Final";
            e.Layout.Bands[0].Columns["POMM"].Header.Caption = "Precio Original";
            e.Layout.Bands[0].Columns["PFMM"].Header.Caption = "Precio Final";

            e.Layout.Bands[0].Columns["V"].Header.Caption = "Volumen";
            e.Layout.Bands[0].Columns["FFV"].Header.Caption = "Factor Final";
            e.Layout.Bands[0].Columns["POV"].Header.Caption = "Precio Original";
            e.Layout.Bands[0].Columns["PFV"].Header.Caption = "Precio Final";
            /////////////////////////////////////////////////////////////////////////
            e.Layout.Bands[0].Columns["PFD"].Format = "C2";

            e.Layout.Bands[0].Columns["FOMT"].Format = "N4";
            e.Layout.Bands[0].Columns["FFMT"].Format = "N4";
            e.Layout.Bands[0].Columns["POMT"].Format = "C2";
            e.Layout.Bands[0].Columns["PFMT"].Format = "C2";

            e.Layout.Bands[0].Columns["FOMM"].Format = "N4";
            e.Layout.Bands[0].Columns["FFMM"].Format = "N4";
            e.Layout.Bands[0].Columns["POMM"].Format = "C2";
            e.Layout.Bands[0].Columns["PFMM"].Format = "C2";

            e.Layout.Bands[0].Columns["V"].Format = "N0";
            e.Layout.Bands[0].Columns["FFV"].Format = "N4";
            e.Layout.Bands[0].Columns["POV"].Format = "C2";
            e.Layout.Bands[0].Columns["PFV"].Format = "C2";

            e.Layout.Bands[0].Columns["Validación 2"].Format = "C2";
            /////////////////////////////////////////////////////////////////////////
            e.Layout.Bands[0].Columns["PFD"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            e.Layout.Bands[0].Columns["FOMT"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["FFMT"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["POMT"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["PFMT"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            e.Layout.Bands[0].Columns["FOMM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["FFMM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["POMM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["PFMM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            e.Layout.Bands[0].Columns["V"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["FFV"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["POV"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["PFV"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            e.Layout.Bands[0].Columns["Validación 2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

            Infragistics.Win.UltraWinGrid.UltraGridGroup grupoInicio = new Infragistics.Win.UltraWinGrid.UltraGridGroup("", 0);
            (sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Groups.Add(grupoInicio);
            grupoInicio.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["ItemCode"]);
            grupoInicio.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["ItemName"]);
            grupoInicio.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["ItmsGrpNam"]);
            grupoInicio.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["Clasificación"]);
            grupoInicio.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["PFD"]);
            grupoInicio.Header.Fixed = true;

            Infragistics.Win.UltraWinGrid.UltraGridGroup grupoMT = new Infragistics.Win.UltraWinGrid.UltraGridGroup("Mínimo Transporte", 0);
            (sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Groups.Add(grupoMT);
            grupoMT.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["FOMT"]);
            grupoMT.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["FFMT"]);
            grupoMT.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["POMT"]);
            grupoMT.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["PFMT"]);
            grupoMT.Header.Fixed = true;

            Infragistics.Win.UltraWinGrid.UltraGridGroup grupoMM = new Infragistics.Win.UltraWinGrid.UltraGridGroup("Mínimo Mayoreo", 0);
            (sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Groups.Add(grupoMM);
            grupoMM.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["FOMM"]);
            grupoMM.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["FFMM"]);
            grupoMM.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["POMM"]);
            grupoMM.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["PFMM"]);
            grupoMM.Header.Fixed = true;

            Infragistics.Win.UltraWinGrid.UltraGridGroup grupoV = new Infragistics.Win.UltraWinGrid.UltraGridGroup("Volumen", 0);
            (sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Groups.Add(grupoV);
            grupoV.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["V"]);
            grupoV.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["FFv"]);
            grupoV.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["POv"]);
            grupoV.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["PFv"]);
            grupoV.Header.Fixed = true;

            Infragistics.Win.UltraWinGrid.UltraGridGroup grupoValidacion = new Infragistics.Win.UltraWinGrid.UltraGridGroup("Vaidaciones", 0);
            (sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Groups.Add(grupoValidacion);
            grupoValidacion.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["Validación 1"]);
            grupoValidacion.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["Validación 2"]);
            grupoValidacion.Columns.Add((sender as Infragistics.Win.UltraWinGrid.UltraGrid).DisplayLayout.Bands[0].Columns["Aprobar"]);
            grupoV.Header.Fixed = true;

            e.Layout.Bands[0].Groups["Mínimo Transporte"].Header.Appearance.BackColor = Color.FromArgb(155, 194, 230);
            e.Layout.Bands[0].Groups["Mínimo Transporte"].Header.Appearance.BackColor2 = Color.FromArgb(155, 194, 230);
            e.Layout.Bands[0].Groups["Mínimo Mayoreo"].Header.Appearance.BackColor = Color.FromArgb(255, 255, 0);
            e.Layout.Bands[0].Groups["Mínimo Mayoreo"].Header.Appearance.BackColor2 = Color.FromArgb(255, 255, 0);
            e.Layout.Bands[0].Groups["Volumen"].Header.Appearance.BackColor = Color.FromArgb(146, 208, 80);
            e.Layout.Bands[0].Groups["Volumen"].Header.Appearance.BackColor2 = Color.FromArgb(146, 208, 80);

            e.Layout.Bands[0].Groups["Mínimo Transporte"].Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;
            e.Layout.Bands[0].Groups["Mínimo Mayoreo"].Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;
            e.Layout.Bands[0].Groups["Volumen"].Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;
            e.Layout.Bands[0].Groups["Vaidaciones"].Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center;

            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }

            e.Layout.Bands[0].Columns["FFMM"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
            e.Layout.Bands[0].Columns["FFMT"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
            e.Layout.Bands[0].Columns["FFV"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;

            e.Layout.Bands[0].Columns["FFMM"].CellAppearance.BackColor = Color.FromName("Info");
            e.Layout.Bands[0].Columns["FFMT"].CellAppearance.BackColor = Color.FromName("Info");
            e.Layout.Bands[0].Columns["FFV"].CellAppearance.BackColor = Color.FromName("Info");

            #region Formulas
            Infragistics.Win.UltraWinCalcManager.UltraCalcManager calcManager;
            calcManager = new Infragistics.Win.UltraWinCalcManager.UltraCalcManager(this.Container);
            e.Layout.Grid.CalcManager = calcManager;

            e.Layout.Bands[0].Columns["PFMT"].Formula = "[FFMT]*[CB]";
            e.Layout.Bands[0].Columns["PFMM"].Formula = "[FFMM]*[CB]";
            e.Layout.Bands[0].Columns["PFV"].Formula = "[FFV]*[CB]";

           // e.Layout.Bands[0].Columns["Validación 2"].Formula = "[PFMM]-([PFV]*0.8)";

            #endregion
        }

        private void dgvDatos_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            try
            {
                if(e.Row.Cells["Validación 1"].Value.ToString().Equals("FALSO"))
                {
                    e.Row.Cells["Validación 1"].Appearance.BackColor = Color.Red;
                    e.Row.Cells["Validación 1"].Appearance.ForeColor = Color.White;
                }
                else
                {
                    e.Row.Cells["Validación 1"].Appearance.BackColor = Color.White;
                    e.Row.Cells["Validación 1"].Appearance.ForeColor = Color.Black;
                }

                if (Convert.ToDecimal(e.Row.Cells["Validación 2"].Value) < 0)
                {
                    e.Row.Cells["Validación 2"].Appearance.BackColor = Color.Red;
                    e.Row.Cells["Validación 2"].Appearance.ForeColor = Color.White;
                }
                else
                {
                    e.Row.Cells["Validación 2"].Appearance.BackColor = Color.White;
                    e.Row.Cells["Validación 2"].Appearance.ForeColor = Color.Black;
                }


                e.Row.Cells["Aprobar"].Value = "Aprobar";
                e.Row.Cells["Aprobar"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                e.Row.Cells["Aprobar"].ButtonAppearance.BorderColor = Color.CadetBlue;
                e.Row.Cells["Aprobar"].Value = "Aprobar";
                e.Row.Cells["Aprobar"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                e.Row.Cells["Aprobar"].ButtonAppearance.BorderColor = Color.CadetBlue;
            }
            catch (Exception)
            {
                
            }
        }

        private void dgvDatos_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            try
            {
                decimal PFT = decimal.Zero, PFM = decimal.Zero;
                PFT = Convert.ToDecimal((Convert.ToDecimal(dgvDatos.Rows[e.Cell.Row.Index].Cells["PFMT"].Value) != 0 ? dgvDatos.Rows[e.Cell.Row.Index].Cells["PFMT"].Value : DBNull.Value) == DBNull.Value ? dgvDatos.Rows[e.Cell.Row.Index].Cells["POMT"].Value : dgvDatos.Rows[e.Cell.Row.Index].Cells["PFMT"].Value);
                PFM = Convert.ToDecimal((Convert.ToDecimal(dgvDatos.Rows[e.Cell.Row.Index].Cells["PFMM"].Value) != 0 ? dgvDatos.Rows[e.Cell.Row.Index].Cells["PFMM"].Value : DBNull.Value) == DBNull.Value ? dgvDatos.Rows[e.Cell.Row.Index].Cells["POMM"].Value : dgvDatos.Rows[e.Cell.Row.Index].Cells["PFMM"].Value);

                PFT = Math.Truncate(PFT * 100) / 100;
                PFM = Math.Truncate(PFM * 100) / 100;
                
                //PFM = Convert.ToDecimal(dgvDatos.Rows[e.Cell.Row.Index].Cells["PFMM"].Value == DBNull.Value ? dgvDatos.Rows[e.Cell.Row.Index].Cells["POMM"].Value : dgvDatos.Rows[e.Cell.Row.Index].Cells["PFMM"].Value);

                #region Validacion 1
                if (PFM <= PFT)
                    dgvDatos.Rows[e.Cell.Row.Index].Cells["Validación 1"].Value = "VERDADERO";
                else
                    dgvDatos.Rows[e.Cell.Row.Index].Cells["Validación 1"].Value = "FALSO";
                #endregion

                #region PRECIOS FINALES
                //decimal? FFT, FFM, FFV, CB;
                //FFT = (decimal?)(dgvDatos.Rows[e.Cell.Row.Index].Cells["FFMT"].Value == DBNull.Value ? null : dgvDatos.Rows[e.Cell.Row.Index].Cells["FFMT"].Value);
                //FFM = (decimal?)(dgvDatos.Rows[e.Cell.Row.Index].Cells["FFMM"].Value == DBNull.Value ? null : dgvDatos.Rows[e.Cell.Row.Index].Cells["FFMM"].Value);
                //FFV = (decimal?)(dgvDatos.Rows[e.Cell.Row.Index].Cells["FFV"].Value == DBNull.Value ? null : dgvDatos.Rows[e.Cell.Row.Index].Cells["FFV"].Value);
                //CB = (decimal?)(dgvDatos.Rows[e.Cell.Row.Index].Cells["CB"].Value == DBNull.Value ? null : dgvDatos.Rows[e.Cell.Row.Index].Cells["CB"].Value);

                //dgvDatos.Rows[e.Cell.Row.Index].Cells["PFMT"].Value = FFT == null ? DBNull.Value : FFT * CB;
                //dgvDatos.Rows[e.Cell.Row.Index].Cells["PFMM"].Value = FFM == null ? DBNull.Value : FFM * CB;
                //dgvDatos.Rows[e.Cell.Row.Index].Cells["PFV"].Value = FFV == null ? DBNull.Value : FFV * CB;
                #endregion
            }
            catch (Exception)
            {
            }
        }

        private void dgvDatos_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {

            if (dgvDatos.ActiveRow != null)
            {
                Object[] valuesOut = new Object[] { };
                connection.Ejecutar("LOG",
                                    "sp_ListaPrecios",
                                    new string[] { },
                                    new string[] { "@TipoConsulta", "@ItemCode", "@FactorT", "@FactorM", "@FactorV", "@Volumen" },
                                    ref valuesOut, 10, dgvDatos.ActiveRow.Cells["ItemCode"].Value, dgvDatos.ActiveRow.Cells["FFMT"].Value, dgvDatos.ActiveRow.Cells["FFMM"].Value, dgvDatos.ActiveRow.Cells["FFV"].Value, dgvDatos.ActiveRow.Cells["V"].Value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var item in dgvDatos.Rows)
            {
                if (Convert.ToInt32(cbListasPrecios.SelectedValue) == 9)
                    item.Cells["PFD"].Value = Convert.ToDecimal(item.Cells["POMT"].Value) - (Convert.ToDecimal(item.Cells["POMT"].Value) * (Convert.ToDecimal(txtDescuento.Text) / 100));
                else if (Convert.ToInt32(cbListasPrecios.SelectedValue) == 12)
                    item.Cells["PFD"].Value = Convert.ToDecimal(item.Cells["POMM"].Value) - (Convert.ToDecimal(item.Cells["POMM"].Value) * (Convert.ToDecimal(txtDescuento.Text) / 100));
                else if (Convert.ToInt32(cbListasPrecios.SelectedValue) == 19)
                    item.Cells["PFD"].Value = Convert.ToDecimal(item.Cells["POV"].Value) - (Convert.ToDecimal(item.Cells["POV"].Value) * (Convert.ToDecimal(txtDescuento.Text) / 100));
                else item.Cells["PFD"].Value = DBNull.Value;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var item in dgvDatos.Rows)
                {
                    item.Cells["PFD"].Value = DBNull.Value;
                    item.Cells["FFMT"].Value = DBNull.Value;
                    item.Cells["FFMM"].Value = DBNull.Value;
                    item.Cells["FFV"].Value = DBNull.Value;
                    item.Cells["V"].Value = DBNull.Value;

                    Object[] valuesOut = new Object[] { };
                    connection.Ejecutar("LOG",
                                        "sp_ListaPrecios",
                                        new string[] { },
                                        new string[] { "@TipoConsulta", "@ItemCode", "@FactorT", "@FactorM", "@FactorV", "@Volumen" },
                                        ref valuesOut, 10, item.Cells["ItemCode"].Value, item.Cells["FFMT"].Value, item.Cells["FFMM"].Value, item.Cells["FFV"].Value, item.Cells["V"].Value);

                }
                txtDescuento.Clear();
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            try
            {
                foreach (var item in dgvDatos.Rows)
                {
                    //item.Cells["PFD"].Value = DBNull.Value;
                    //item.Cells["FFMT"].Value = DBNull.Value;
                    //item.Cells["FFMM"].Value = DBNull.Value;
                    //item.Cells["FFV"].Value = DBNull.Value;
                    //item.Cells["V"].Value = DBNull.Value;

                    Object[] valuesOut = new Object[] { };
                    connection.Ejecutar("LOG",
                                        "sp_ListaPrecios",
                                        new string[] { },
                                        new string[] { "@TipoConsulta", "@ItemCode", "@FactorT", "@FactorM", "@FactorV", "@Volumen" },
                                        ref valuesOut, 10, item.Cells["ItemCode"].Value, item.Cells["FFMT"].Value, item.Cells["FFMM"].Value, item.Cells["FFV"].Value, item.Cells["V"].Value);

                }
                //txtDescuento.Clear();
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }
    }
}
