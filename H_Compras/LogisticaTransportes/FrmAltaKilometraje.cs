using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;

namespace H_Compras.LogisticaTransportes
{
    public partial class FrmAltaKilometraje : Form
    {
        ClasesSGUV.Logs log;
        public FrmAltaKilometraje()
        {
            InitializeComponent();
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                ConsultaFacturas(dtpDesde.Value, dtpHasta.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public void ConsultaFacturas(DateTime Desde, DateTime Hasta)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_Logistica_Transportes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 1);
                        command.Parameters.AddWithValue("@Desde", new DateTime(Desde.Year, Desde.Month, Desde.Day, 00,00,00));
                        command.Parameters.AddWithValue("@Hasta", new DateTime(Hasta.Year, Hasta.Month, Hasta.Day, 23, 59, 59));
                        command.CommandTimeout = 0;

                        SqlDataAdapter da = new SqlDataAdapter();
                        DataSet DT = new DataSet();
                        da.SelectCommand = command;
                        da.Fill(DT);

                        DT.Tables[0].TableName = "Encabezado";
                        DT.Tables[1].TableName = "Detalle";

                        dgvDatos.DataSource = null;
                        //dgvDatos.DataSource = DT.Tables[0];

                        BindingSource rEncabezadoFlujo = new BindingSource();
                        BindingSource rDetalleFlujo = new BindingSource();

                        DataRelation relation = new DataRelation("Detalle", DT.Tables[0].Columns["Factura"], DT.Tables[1].Columns["Factura"]);
                        DT.Relations.Add(relation);

                        rEncabezadoFlujo.DataSource = DT;
                        rEncabezadoFlujo.DataMember = "Encabezado";
                        rDetalleFlujo.DataSource = rEncabezadoFlujo;
                        rDetalleFlujo.DataMember = "Detalle";

                        dgvDatos.DataSource = rEncabezadoFlujo;
                        dgvDatosDetalle.DataSource = rDetalleFlujo;

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.Override.ExpansionIndicator = ShowExpansionIndicator.Never;
                e.Layout.Bands[0].Columns["Factura"].CellActivation = Activation.NoEdit;
                e.Layout.Bands[0].Columns["Fecha"].CellActivation = Activation.NoEdit;
                e.Layout.Bands[0].Columns["Cliente"].CellActivation = Activation.NoEdit;
                e.Layout.Bands[0].Columns["Nombre"].CellActivation = Activation.NoEdit;
                e.Layout.Bands[0].Columns["Nombre"].Width = 150;
                e.Layout.Bands[0].Columns["Kilometros"].Width = 100;
                e.Layout.Bands[0].Columns["Kilometros"].CellAppearance.TextHAlign = HAlign.Right;
                e.Layout.Bands[0].Columns["Kilometros"].Format = "N0";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }            
        }

        private void dgvDatos_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Key == "Kilometros")
                {
                    int Factura = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Factura"].Value)
                                ? 0 : Convert.ToInt32((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Factura"].Value);

                    int Kilometros = Convert.IsDBNull((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Kilometros"].Value)
                                ? 0 : Convert.ToInt32((sender as UltraGrid).Rows[e.Cell.Row.Index].Cells["Kilometros"].Value);

                    using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                    {
                        using (SqlCommand command = new SqlCommand("up_Logistica_Transportes", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Connection.Open();
                            command.Parameters.AddWithValue("@TipoConsulta", 2);
                            command.Parameters.AddWithValue("@Factura", Factura);
                            command.Parameters.AddWithValue("@Kilometros", Kilometros);
                            command.Parameters.AddWithValue("@Usuario", ClasesSGUV.Login.Id_Usuario);

                            command.ExecuteNonQuery();
                            command.Connection.Close();
                        }
                    }
 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void FrmAltaKilometraje_Load(object sender, EventArgs e)
        {
            try
            {
                GridKeyActionMapping mapping = new GridKeyActionMapping(Keys.Enter, UltraGridAction.BelowCell, (UltraGridState)0, UltraGridState.InEdit, SpecialKeys.All, (SpecialKeys)0);
                this.dgvDatos.KeyActionMappings.Add(mapping);
                log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.Message.ToString());
            }
            
        }

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGrid dgvDatos = (sender as Infragistics.Win.UltraWinGrid.UltraGrid);
            switch (e.KeyCode)
            {
                case Keys.Up:
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false,
                      false);
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.AboveCell, false,
                      false);
                    e.Handled = true;
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode, false,
                      false);
                    break;
                case Keys.Down:
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false,
                      false);
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.BelowCell, false,
                      false);
                    e.Handled = true;
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode, false,
                      false);
                    break;
                case Keys.Right:
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false,
                      false);
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.NextCellByTab, false,
                      false);
                    e.Handled = true;
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode, false,
                      false);
                    break;
                case Keys.Left:
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ExitEditMode, false,
                      false);
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.PrevCellByTab, false,
                      false);
                    e.Handled = true;
                    dgvDatos.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.EnterEditMode, false,
                      false);
                    break;
            }
        }

        private void dgvDatos_KeyPress(object sender, KeyPressEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            UltraGridCell activeCell = grid == null ? null : grid.ActiveCell;

            // if there is an active cell, its not in edit mode and can enter edit mode
            if (null != activeCell && false == activeCell.IsInEditMode && activeCell.CanEnterEditMode)
            {
                // if the character is not a control character
                if (char.IsControl(e.KeyChar) == false)
                {
                    // try to put cell into edit mode
                    grid.PerformAction(UltraGridAction.EnterEditMode);

                    // if this cell is still active and it is in edit mode...
                    if (grid.ActiveCell == activeCell && activeCell.IsInEditMode)
                    {
                        // get its editor
                        EmbeddableEditorBase editor = this.dgvDatos.ActiveCell.EditorResolved;

                        // if the editor supports selectable text
                        if (editor.SupportsSelectableText)
                        {
                            // select all the text so it can be replaced
                            editor.SelectionStart = 0;
                            editor.SelectionLength = editor.TextLength;

                            if (editor is EditorWithMask)
                            {
                                // just clear the selected text and let the grid
                                // forward the keypress to the editor
                                editor.SelectedText = string.Empty;
                            }
                            else
                            {
                                // then replace the selected text with the character
                                editor.SelectedText = new string(e.KeyChar, 1);
                                // mark the event as handled so the grid doesn't process it
                                e.Handled = true;
                            }
                        }
                    }
                }
            }
        }

        private void dgvDatosDetalle_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["Monto"].Format = "C2";
            e.Layout.Bands[0].Columns["Monto"].CellAppearance.TextHAlign = HAlign.Right;

            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Activation.NoEdit;
            }
        }

        private void FrmAltaKilometraje_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void FrmAltaKilometraje_FormClosing(object sender, FormClosingEventArgs e)
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
