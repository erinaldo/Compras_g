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

namespace H_Compras.Configuracion
{
    public partial class frmHorizontes : Constantes.frmEmpty
    {
        private enum ColumnasHorizontes
        {
            Code,
            Reporte,
            Origen,
            oA,
            oB,
            oC,
            Destino,
            dA,
            dB,
            dC,
            Activo
        }

        public enum ColumasGrupos
        {
            Code,
            GroupCode,
            GroupName
        }

        public enum ColumnasAlmacenes
        {
            Code, WhsCode, WhsName
        }

        public frmHorizontes()
        {
            InitializeComponent();
        }

        private void frmHorizontes_Load(object sender, EventArgs e)
        {
            try
            {
                actualizarToolStripButton.Click -= new EventHandler(frmHorizontes_Load);
                actualizarToolStripButton.Click += new EventHandler(frmHorizontes_Load);

                this.dgvDatos.PerformAction(UltraGridAction.Copy, true, true);

                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 13);

                        DataTable table = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(table);

                        dgvDatos.DataSource = table;
                    }
                }
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
                #endregion
            }
            catch (Exception) { }
        }

        private void dgvDatos_BeforeRowActivate(object sender, RowEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 14);
                        command.Parameters.AddWithValue("@Reporte", e.Row.Cells[0].Value);

                        DataTable table = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(table);
                        
                        dgvHorizontes.DataSource = table;
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }

        private void dgvHorizontes_BeforeRowActivate(object sender, RowEventArgs e)
        {
            try
            {
                #region Grupos Proveedores
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 15);
                        command.Parameters.AddWithValue("@Reporte", e.Row.Cells[0].Value);

                        DataTable table = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(table);

                        dgvGrupos.DataSource = table;
                    }
                }
                #endregion

                #region Almacenes Origen
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 16);
                        command.Parameters.AddWithValue("@Reporte", e.Row.Cells[0].Value);
                        command.Parameters.AddWithValue("@Tipo", "O");

                        DataTable table = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(table);

                        dgvOrigen.DataSource = table;
                    }
                }
                #endregion

                #region Almacenes Destino
                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("sp_Configuracion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", 16);
                        command.Parameters.AddWithValue("@Reporte", e.Row.Cells[0].Value);
                        command.Parameters.AddWithValue("@Tipo", "D");

                        DataTable table = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(table);

                        dgvDestino.DataSource = table;
                    }
                }
                #endregion
            }
            catch (Exception)
            {

            }
        }

        private void dgvHorizontes_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];
            //band.Columns[(int)ColumnasHorizontes.Code].Hidden = true;
            band.Columns[(int)ColumnasHorizontes.Reporte].Hidden = true;
            band.Columns[(int)ColumnasHorizontes.Code].Width = 50;

            band.Columns[(int)ColumnasHorizontes.Origen].Width = 120;
            band.Columns[(int)ColumnasHorizontes.Destino].Width = 120;
            band.Columns[(int)ColumnasHorizontes.Activo].Width = 50;

            band.Columns[(int)ColumnasHorizontes.Origen].CellActivation = Activation.NoEdit;
            band.Columns[(int)ColumnasHorizontes.Destino].CellActivation = Activation.NoEdit;

            band.Columns[(int)ColumnasHorizontes.oA].Header.Caption = "A";
            band.Columns[(int)ColumnasHorizontes.oB].Header.Caption = "B";
            band.Columns[(int)ColumnasHorizontes.oC].Header.Caption = "C";

            band.Columns[(int)ColumnasHorizontes.dA].Header.Caption = "A";
            band.Columns[(int)ColumnasHorizontes.dB].Header.Caption = "B";
            band.Columns[(int)ColumnasHorizontes.dC].Header.Caption = "C";

            band.Columns[(int)ColumnasHorizontes.oA].Width = 80;
            band.Columns[(int)ColumnasHorizontes.oB].Width = 80;
            band.Columns[(int)ColumnasHorizontes.oC].Width = 80;

            band.Columns[(int)ColumnasHorizontes.dA].Width = 80;
            band.Columns[(int)ColumnasHorizontes.dB].Width = 80;
            band.Columns[(int)ColumnasHorizontes.dC].Width = 80;
        }

        private void dgvGrupos_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[(int)ColumasGrupos.Code].Hidden = true;

            e.Layout.Bands[0].Columns[(int)ColumasGrupos.GroupCode].Width = 50;
            e.Layout.Bands[0].Columns[(int)ColumasGrupos.GroupName].Width = 150;

            e.Layout.Bands[0].Columns[(int)ColumasGrupos.GroupCode].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumasGrupos.GroupName].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumasGrupos.GroupCode].Header.Caption = "Clave";
            e.Layout.Bands[0].Columns[(int)ColumasGrupos.GroupName].Header.Caption = "Grupo";
        }

        private void dgv_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[(int)ColumnasAlmacenes.Code].Hidden = true;

            e.Layout.Bands[0].Columns[(int)ColumnasAlmacenes.WhsCode].Width = 50;
            e.Layout.Bands[0].Columns[(int)ColumnasAlmacenes.WhsName].Width = 90;

            e.Layout.Bands[0].Columns[(int)ColumnasAlmacenes.WhsCode].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[(int)ColumnasAlmacenes.WhsName].CellActivation = Activation.NoEdit;

            e.Layout.Bands[0].Columns[(int)ColumnasAlmacenes.WhsCode].Header.Caption = "Clave";
            e.Layout.Bands[0].Columns[(int)ColumnasAlmacenes.WhsName].Header.Caption = "Almacén";
        }

        private void dgvDatos_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns[1].CellActivation = Activation.NoEdit;
        }
    }
}
