using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;

namespace H_Compras.Logistica
{
    public partial class FrmAutorizacionesFoliosAux : Constantes.frmEmpty//Form
    {
        TipoMovimiento oTipoMovimiento;
        ClasesSGUV.Logs log;
        public FrmAutorizacionesFoliosAux(/*TipoMovimiento _oTipoMov*/)
        {
            InitializeComponent();

            //oTipoMovimiento = _oTipoMov;
        }

        public enum TipoMovimiento 
        {
            Autorizacion = 1,
            GenerarFolio = 2,
            Administrador = 3
        }


        private void btnConsultar_Click(object sender, EventArgs e)
        {
            if (cboSucursal.SelectedIndex != -1)
            {
                try
                {
                    string Sucursal = cboSucursal.SelectedValue.ToString(); //ClasesSGUV.Login.Sucursal;
                    if (oTipoMovimiento == TipoMovimiento.Autorizacion)
                    {
                        using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                        {
                            using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                DateTime Ini = new DateTime(dtpInicio.Value.Year, dtpInicio.Value.Month, dtpInicio.Value.Day, 00, 00, 00);
                                DateTime Fin = new DateTime(dtpFin.Value.Year, dtpFin.Value.Month, dtpFin.Value.Day, 23, 59, 59);

                                command.Parameters.AddWithValue("@TipoConsulta", 33);
                                command.Parameters.AddWithValue("@FechaSalida", Ini);
                                command.Parameters.AddWithValue("@FechaLlegada", Fin);
                                command.Parameters.AddWithValue("@Sucursal", Sucursal);
                                command.Parameters.AddWithValue("@IDUsu", ClasesSGUV.Login.Id_Usuario);
                                command.CommandTimeout = 0;

                                DataSet dsAutorizaciones = new DataSet();
                                SqlDataAdapter da = new SqlDataAdapter();
                                da.SelectCommand = command;
                                da.Fill(dsAutorizaciones);

                                dsAutorizaciones.Tables[0].TableName = "encabezadofolios";
                                dsAutorizaciones.Tables[1].TableName = "detallefolios";

                                BindingSource rEncabezadoFlujo = new BindingSource();
                                BindingSource rDetalleFlujo = new BindingSource();

                                DataRelation relation = new DataRelation("detallefolios", dsAutorizaciones.Tables[0].Columns["PK"], dsAutorizaciones.Tables[1].Columns["PK"]);
                                dsAutorizaciones.Relations.Add(relation);

                                rEncabezadoFlujo.DataSource = dsAutorizaciones;
                                rEncabezadoFlujo.DataMember = "encabezadofolios";

                                rDetalleFlujo.DataSource = rEncabezadoFlujo;
                                rDetalleFlujo.DataMember = "detallefolios";

                                dgvDatos.DataSource = rEncabezadoFlujo;
                                dgvDatos1.DataSource = rDetalleFlujo;

                            }
                        }
                    }
                    else if (oTipoMovimiento == TipoMovimiento.GenerarFolio)
                    {
                        using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                        {
                            using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                DateTime Ini = new DateTime(dtpInicio.Value.Year, dtpInicio.Value.Month, dtpInicio.Value.Day, 00, 00, 00);
                                DateTime Fin = new DateTime(dtpFin.Value.Year, dtpFin.Value.Month, dtpFin.Value.Day, 23, 59, 59);

                                command.Parameters.AddWithValue("@TipoConsulta", 40);
                                command.Parameters.AddWithValue("@FechaSalida", Ini);
                                command.Parameters.AddWithValue("@FechaLlegada", Fin);
                                command.Parameters.AddWithValue("@Sucursal", Sucursal);
                                command.Parameters.AddWithValue("@IDUsu", ClasesSGUV.Login.Id_Usuario);
                                command.CommandTimeout = 0;

                                DataSet dsAutorizaciones = new DataSet();
                                SqlDataAdapter da = new SqlDataAdapter();
                                da.SelectCommand = command;
                                da.Fill(dsAutorizaciones);

                                dsAutorizaciones.Tables[0].TableName = "encabezadofolios";
                                dsAutorizaciones.Tables[1].TableName = "detallefolios";

                                BindingSource rEncabezadoFlujo = new BindingSource();
                                BindingSource rDetalleFlujo = new BindingSource();

                                DataRelation relation = new DataRelation("detallefolios", dsAutorizaciones.Tables[0].Columns["PK"], dsAutorizaciones.Tables[1].Columns["PK"]);
                                dsAutorizaciones.Relations.Add(relation);

                                rEncabezadoFlujo.DataSource = dsAutorizaciones;
                                rEncabezadoFlujo.DataMember = "encabezadofolios";

                                rDetalleFlujo.DataSource = rEncabezadoFlujo;
                                rDetalleFlujo.DataMember = "detallefolios";

                                dgvDatos.DataSource = rEncabezadoFlujo;
                                dgvDatos1.DataSource = rDetalleFlujo;

                            }
                        }

                    }
                    if (oTipoMovimiento == TipoMovimiento.Administrador)
                    {
                        using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                        {
                            using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                DateTime Ini = new DateTime(dtpInicio.Value.Year, dtpInicio.Value.Month, dtpInicio.Value.Day, 00, 00, 00);
                                DateTime Fin = new DateTime(dtpFin.Value.Year, dtpFin.Value.Month, dtpFin.Value.Day, 23, 59, 59);

                                command.Parameters.AddWithValue("@TipoConsulta", 33);
                                command.Parameters.AddWithValue("@FechaSalida", Ini);
                                command.Parameters.AddWithValue("@FechaLlegada", Fin);
                                command.Parameters.AddWithValue("@Sucursal", Sucursal);
                                command.Parameters.AddWithValue("@IDUsu", ClasesSGUV.Login.Id_Usuario);
                                command.CommandTimeout = 0;

                                DataSet dsAutorizaciones = new DataSet();
                                SqlDataAdapter da = new SqlDataAdapter();
                                da.SelectCommand = command;
                                da.Fill(dsAutorizaciones);

                                dsAutorizaciones.Tables[0].TableName = "encabezadofolios";
                                dsAutorizaciones.Tables[1].TableName = "detallefolios";

                                BindingSource rEncabezadoFlujo = new BindingSource();
                                BindingSource rDetalleFlujo = new BindingSource();

                                DataRelation relation = new DataRelation("detallefolios", dsAutorizaciones.Tables[0].Columns["PK"], dsAutorizaciones.Tables[1].Columns["PK"]);
                                dsAutorizaciones.Relations.Add(relation);

                                rEncabezadoFlujo.DataSource = dsAutorizaciones;
                                rEncabezadoFlujo.DataMember = "encabezadofolios";

                                rDetalleFlujo.DataSource = rEncabezadoFlujo;
                                rDetalleFlujo.DataMember = "detallefolios";

                                dgvDatos.DataSource = rEncabezadoFlujo;
                                dgvDatos1.DataSource = rDetalleFlujo;

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
            {
                MessageBox.Show("Especifique una sucursal", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvDatos_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Override.ExpansionIndicator = ShowExpansionIndicator.Never;
            e.Layout.Bands[0].Columns["PK"].Hidden = true;
            e.Layout.Bands[0].Columns["sCodSucursal"].Hidden = true;
            e.Layout.Bands[0].Columns["nIdCaptura"].Header.Caption = "Folio Auxiliar";
            e.Layout.Bands[0].Columns["FolioReal"].Header.Caption = "Folio Real";
            e.Layout.Bands[0].Columns["sPlacas"].Header.Caption = "Placas";
            e.Layout.Bands[0].Columns["sUsuario"].Header.Caption = "Usuario";
            e.Layout.Bands[0].Columns["dSalida"].Header.Caption = "Salida";
            e.Layout.Bands[0].Columns["dSalida"].Format = "dd/MM/yyyy HH:mm:ss"; 
            e.Layout.Bands[0].Columns["dLlegada"].Header.Caption = "Llegada";
            e.Layout.Bands[0].Columns["dLlegada"].Format = "dd/MM/yyyy HH:mm:ss"; 
            e.Layout.Bands[0].Columns["TipoRuta"].Header.Caption = "Tipo-Ruta";
            e.Layout.Bands[0].Columns["sObservaciones"].Header.Caption = "Observaciones";
            e.Layout.Bands[0].Columns["Estatus"].Header.Caption = "Estatus";

            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Activation.NoEdit;
            }

            e.Layout.Bands[0].Columns["GenerarFolio"].CellActivation = Activation.AllowEdit;

            if (oTipoMovimiento == TipoMovimiento.Autorizacion)
                e.Layout.Bands[0].Columns["GenerarFolio"].Hidden = true;
            else if (oTipoMovimiento == TipoMovimiento.GenerarFolio)
            {
                e.Layout.Bands[0].Columns["AutorizarFolio"].Hidden = true;
                e.Layout.Bands[0].Columns["CancelarFolio"].Hidden = true;
            }
        }

        private void dgvDatos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Band.Index == 0)
            {
                e.Row.Cells["GenerarFolio"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                e.Row.Cells["GenerarFolio"].Value = "Generar";
                e.Row.Cells["AutorizarFolio"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                e.Row.Cells["AutorizarFolio"].Value = "Autorizar";
                e.Row.Cells["CancelarFolio"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                e.Row.Cells["CancelarFolio"].Value = "Cancelar";

                string FolioReal = e.Row.Cells["FolioReal"].Value.ToString();
                string Estatus = e.Row.Cells["Estatus"].Value.ToString();

                if ((FolioReal != "") || Estatus=="PENDIENTE")
                    e.Row.Cells["GenerarFolio"].Activation = Activation.Disabled;
                else if(FolioReal=="" && Estatus == "AUTORIZADA")
                    e.Row.Cells["GenerarFolio"].Activation = Activation.AllowEdit;
                else if (Estatus == "NO APROBADA")
                    e.Row.Cells["GenerarFolio"].Activation = Activation.Disabled;


                if (Estatus != "PENDIENTE")
                {
                    e.Row.Cells["AutorizarFolio"].Activation = Activation.Disabled;
                    e.Row.Cells["CancelarFolio"].Activation = Activation.Disabled;
                }
            }
        }

        private void dgvDatos1_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["PK"].Hidden = true;
            e.Layout.Bands[0].Columns["sCodSucursal"].Hidden = true;

            e.Layout.Bands[0].Columns["nIdCaptura"].Hidden = true;
            e.Layout.Bands[0].Columns["nDocumento"].Header.Caption = "No. Documento";
            e.Layout.Bands[0].Columns["sTipoDoc"].Header.Caption = "Tipo";
            e.Layout.Bands[0].Columns["mMontoMXP"].Header.Caption = "Monto (mxp)";
            e.Layout.Bands[0].Columns["mMontoMXP"].Format = "N2";
            e.Layout.Bands[0].Columns["mMontoUSD"].Header.Caption = "Monto (USD)";
            e.Layout.Bands[0].Columns["mMontoUSD"].Format = "N2";
            e.Layout.Bands[0].Columns["sClaveCliente"].Header.Caption = "Cliente";
            e.Layout.Bands[0].Columns["sCliente"].Header.Caption = "Nombre";
            e.Layout.Bands[0].Columns["nPeso"].Header.Caption = "Peso";
            e.Layout.Bands[0].Columns["nPeso"].Format = "N2";
            e.Layout.Bands[0].Columns["nVolumen"].Header.Caption = "Volumen";
            e.Layout.Bands[0].Columns["nVolumen"].Format = "N2";
            e.Layout.Bands[0].Columns["sCodigoRuta"].Header.Caption = "Codigo Ruta";
            e.Layout.Bands[0].Columns["Exceso"].Header.Caption = "Exceso";

            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Activation.NoEdit;
            }
        }

        private void dgvDatos_ClickCellButton(object sender, CellEventArgs e)
        {
            if (oTipoMovimiento == TipoMovimiento.GenerarFolio)
            {
                try
                {
                    int Folio = -1;
                    string CodSuc = string.Empty;
                    int FolioReal = -1;

                    Folio = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["nIdCaptura"].Value) ? -1 :
                        Convert.ToInt32(dgvDatos.Rows[e.Cell.Row.Index].Cells["nIdCaptura"].Value);

                    CodSuc = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["sCodSucursal"].Value) ? string.Empty :
                        Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["sCodSucursal"].Value);
                    string TipoRuta = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["TipoRuta"].Value) ? string.Empty :
                            Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["TipoRuta"].Value);
                    string TipoAut = "RL";
                    if (TipoRuta == "RepartoWEB")
                        TipoAut = "RW";

                    using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                    {
                        connection.Open();
                        SqlTransaction transaction;
                        transaction = connection.BeginTransaction("SampleTransaction");

                        try
                        {
                            //-------------SE INSERTA-------------
                            using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                            {
                                command.Transaction = transaction;
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@TipoConsulta", 34);
                                SqlParameter parameter = new SqlParameter("@iCaptura", SqlDbType.Int);
                                parameter.Direction = ParameterDirection.Output; command.Parameters.Add(parameter);
                                command.Parameters.AddWithValue("@Sucursal", CodSuc);
                                command.Parameters.AddWithValue("@Auxiliar", Folio);
                                command.Parameters.AddWithValue("@TipoAutorizacion", TipoAut);
                                command.ExecuteNonQuery();

                                FolioReal = Convert.IsDBNull(command.Parameters["@iCaptura"].Value) ? -1 : Convert.ToInt32(command.Parameters["@iCaptura"].Value);
                            }
                            transaction.Commit();

                            if (FolioReal > 0)
                            {
                                MessageBox.Show("Se ha generado exitosamente el folio :" + FolioReal.ToString(), "Autorización de Folio", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                dgvDatos.Rows[e.Cell.Row.Index].Cells["FolioReal"].Value = FolioReal;
                                dgvDatos.Rows[e.Cell.Row.Index].Cells["GenerarFolio"].Activation = Activation.Disabled;
                                btnConsultar.PerformClick();
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Autorizacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
            else if (oTipoMovimiento == TipoMovimiento.Autorizacion)
            {
                if (e.Cell.Column.Key == "AutorizarFolio")
                {
                    try
                    {
                        int Folio = -1;
                        string CodSuc = string.Empty;
                        int FolioReal = -1;

                        Folio = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["nIdCaptura"].Value) ? -1 :
                            Convert.ToInt32(dgvDatos.Rows[e.Cell.Row.Index].Cells["nIdCaptura"].Value);

                        CodSuc = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["sCodSucursal"].Value) ? string.Empty :
                            Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["sCodSucursal"].Value);
                        string TipoRuta = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["TipoRuta"].Value) ? string.Empty :
                            Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["TipoRuta"].Value);
                        string TipoAut = "RL";
                        if (TipoRuta == "RepartoWEB")
                            TipoAut = "RW";

                        using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                        {
                            connection.Open();
                            SqlTransaction transaction;
                            transaction = connection.BeginTransaction("SampleTransaction");

                            try
                            {
                                //-------------SE INSERTA-------------
                                using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                                {
                                    command.Transaction = transaction;
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.AddWithValue("@TipoConsulta", 32);
                                    command.Parameters.AddWithValue("@Sucursal", CodSuc);
                                    command.Parameters.AddWithValue("@iCaptura", Folio);
                                    command.Parameters.AddWithValue("@EstatusAuxiliar", 2);
                                    command.Parameters.AddWithValue("@TipoAutorizacion", TipoAut);
                                    command.ExecuteNonQuery();

                                }
                                transaction.Commit();

                                MessageBox.Show("Se ha autorizado exitosamente el folio :" + Folio.ToString(), "Autorización de Folio", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                dgvDatos.Rows[e.Cell.Row.Index].Cells["AutorizarFolio"].Activation = Activation.Disabled;
                                dgvDatos.Rows[e.Cell.Row.Index].Cells["CancelarFolio"].Activation = Activation.Disabled;
                                btnConsultar.PerformClick();

                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Autorizacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if (e.Cell.Column.Key == "CancelarFolio")
                {
                    try
                    {
                        int Folio = -1;
                        string CodSuc = string.Empty;
                        int FolioReal = -1;

                        Folio = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["nIdCaptura"].Value) ? -1 :
                            Convert.ToInt32(dgvDatos.Rows[e.Cell.Row.Index].Cells["nIdCaptura"].Value);

                        CodSuc = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["sCodSucursal"].Value) ? string.Empty :
                            Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["sCodSucursal"].Value);

                        string TipoRuta = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["TipoRuta"].Value) ? string.Empty :
                            Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["TipoRuta"].Value);
                        string TipoAut = "RL";
                        if (TipoRuta == "RepartoWEB")
                            TipoAut = "RW";

                        using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                        {
                            connection.Open();
                            SqlTransaction transaction;
                            transaction = connection.BeginTransaction("SampleTransaction");

                            try
                            {
                                //-------------SE INSERTA-------------
                                using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                                {
                                    command.Transaction = transaction;
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.AddWithValue("@TipoConsulta", 32);
                                    command.Parameters.AddWithValue("@Sucursal", CodSuc);
                                    command.Parameters.AddWithValue("@iCaptura", Folio);                                    
                                    command.Parameters.AddWithValue("@EstatusAuxiliar", 3);
                                    command.Parameters.AddWithValue("@TipoAutorizacion", TipoAut);
                                    command.ExecuteNonQuery();

                                }
                                transaction.Commit();

                                MessageBox.Show("Se ha cancelado exitosamente el folio auxiliar de reparto no :" + Folio.ToString(), "Cancelación de Folio", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                dgvDatos.Rows[e.Cell.Row.Index].Cells["AutorizarFolio"].Activation = Activation.Disabled;
                                dgvDatos.Rows[e.Cell.Row.Index].Cells["CancelarFolio"].Activation = Activation.Disabled;
                                btnConsultar.PerformClick();
                                
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Autorizacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
 
                }
 
            }

            else if (oTipoMovimiento == TipoMovimiento.Administrador)
            {
                if (e.Cell.Column.Key == "AutorizarFolio")
                {
                    try
                    {
                        int Folio = -1;
                        string CodSuc = string.Empty;
                        int FolioReal = -1;

                        Folio = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["nIdCaptura"].Value) ? -1 :
                            Convert.ToInt32(dgvDatos.Rows[e.Cell.Row.Index].Cells["nIdCaptura"].Value);

                        CodSuc = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["sCodSucursal"].Value) ? string.Empty :
                            Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["sCodSucursal"].Value);
                        string TipoRuta = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["TipoRuta"].Value) ? string.Empty :
                            Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["TipoRuta"].Value);
                        string TipoAut = "RL";
                        if (TipoRuta == "RepartoWEB")
                            TipoAut = "RW";

                        using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                        {
                            connection.Open();
                            SqlTransaction transaction;
                            transaction = connection.BeginTransaction("SampleTransaction");

                            try
                            {
                                //-------------SE INSERTA-------------
                                using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                                {
                                    command.Transaction = transaction;
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.AddWithValue("@TipoConsulta", 32);
                                    command.Parameters.AddWithValue("@Sucursal", CodSuc);
                                    command.Parameters.AddWithValue("@iCaptura", Folio);
                                    command.Parameters.AddWithValue("@EstatusAuxiliar", 2);
                                    command.Parameters.AddWithValue("@TipoAutorizacion", TipoAut);
                                    command.ExecuteNonQuery();

                                }
                                transaction.Commit();

                                MessageBox.Show("Se ha autorizado exitosamente el folio :" + Folio.ToString(), "Autorización de Folio", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                dgvDatos.Rows[e.Cell.Row.Index].Cells["AutorizarFolio"].Activation = Activation.Disabled;
                                dgvDatos.Rows[e.Cell.Row.Index].Cells["CancelarFolio"].Activation = Activation.Disabled;
                                btnConsultar.PerformClick();

                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Autorizacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if (e.Cell.Column.Key == "CancelarFolio")
                {
                    try
                    {
                        int Folio = -1;
                        string CodSuc = string.Empty;
                        int FolioReal = -1;

                        Folio = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["nIdCaptura"].Value) ? -1 :
                            Convert.ToInt32(dgvDatos.Rows[e.Cell.Row.Index].Cells["nIdCaptura"].Value);

                        CodSuc = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["sCodSucursal"].Value) ? string.Empty :
                            Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["sCodSucursal"].Value);

                        string TipoRuta = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["TipoRuta"].Value) ? string.Empty :
                            Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["TipoRuta"].Value);
                        string TipoAut = "RL";
                        if (TipoRuta == "RepartoWEB")
                            TipoAut = "RW";

                        using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                        {
                            connection.Open();
                            SqlTransaction transaction;
                            transaction = connection.BeginTransaction("SampleTransaction");

                            try
                            {
                                //-------------SE INSERTA-------------
                                using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                                {
                                    command.Transaction = transaction;
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.AddWithValue("@TipoConsulta", 32);
                                    command.Parameters.AddWithValue("@Sucursal", CodSuc);
                                    command.Parameters.AddWithValue("@iCaptura", Folio);
                                    command.Parameters.AddWithValue("@EstatusAuxiliar", 3);
                                    command.Parameters.AddWithValue("@TipoAutorizacion", TipoAut);
                                    command.ExecuteNonQuery();

                                }
                                transaction.Commit();

                                MessageBox.Show("Se ha cancelado exitosamente el folio auxiliar de reparto no :" + Folio.ToString(), "Cancelación de Folio", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                dgvDatos.Rows[e.Cell.Row.Index].Cells["AutorizarFolio"].Activation = Activation.Disabled;
                                dgvDatos.Rows[e.Cell.Row.Index].Cells["CancelarFolio"].Activation = Activation.Disabled;
                                btnConsultar.PerformClick();

                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Autorizacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                else if (e.Cell.Column.Key == "GenerarFolio")
                {
                    try
                    {
                        int Folio = -1;
                        string CodSuc = string.Empty;
                        int FolioReal = -1;

                        Folio = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["nIdCaptura"].Value) ? -1 :
                            Convert.ToInt32(dgvDatos.Rows[e.Cell.Row.Index].Cells["nIdCaptura"].Value);

                        CodSuc = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["sCodSucursal"].Value) ? string.Empty :
                            Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["sCodSucursal"].Value);
                        string TipoRuta = Convert.IsDBNull(dgvDatos.Rows[e.Cell.Row.Index].Cells["TipoRuta"].Value) ? string.Empty :
                                Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["TipoRuta"].Value);
                        string TipoAut = "RL";
                        if (TipoRuta == "RepartoWEB")
                            TipoAut = "RW";

                        using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                        {
                            connection.Open();
                            SqlTransaction transaction;
                            transaction = connection.BeginTransaction("SampleTransaction");

                            try
                            {
                                //-------------SE INSERTA-------------
                                using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                                {
                                    command.Transaction = transaction;
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.AddWithValue("@TipoConsulta", 34);
                                    SqlParameter parameter = new SqlParameter("@iCaptura", SqlDbType.Int);
                                    parameter.Direction = ParameterDirection.Output; command.Parameters.Add(parameter);
                                    command.Parameters.AddWithValue("@Sucursal", CodSuc);
                                    command.Parameters.AddWithValue("@Auxiliar", Folio);
                                    command.Parameters.AddWithValue("@TipoAutorizacion", TipoAut);
                                    command.ExecuteNonQuery();

                                    FolioReal = Convert.IsDBNull(command.Parameters["@iCaptura"].Value) ? -1 : Convert.ToInt32(command.Parameters["@iCaptura"].Value);
                                }
                                transaction.Commit();

                                if (FolioReal > 0)
                                {
                                    MessageBox.Show("Se ha generado exitosamente el folio :" + FolioReal.ToString(), "Autorización de Folio", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    dgvDatos.Rows[e.Cell.Row.Index].Cells["FolioReal"].Value = FolioReal;
                                    dgvDatos.Rows[e.Cell.Row.Index].Cells["GenerarFolio"].Activation = Activation.Disabled;
                                    btnConsultar.PerformClick();
                                }
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Autorizacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
 
                }

            }

            

        }

        private void FrmAutorizacionesFoliosAux_Load(object sender, EventArgs e)
        {
            try
            {

                nuevoToolStripButton.Enabled = false;
                guardarToolStripButton.Enabled = false;
                actualizarToolStripButton.Enabled = false;
                exportarToolStripButton.Enabled = false;


                if (ClasesSGUV.Login.Rol == 1)
                {
                    oTipoMovimiento = TipoMovimiento.Administrador;
                    PathHelp = "http://hntsolutions.net/manual/module_10_5.htm?ms=AAAAAAA%3D&st=MA%3D%3D&sct=MzQxOA%3D%3D&mw=MjU1";
                }
                else if (ClasesSGUV.Login.Rol == 2)
                {
                    oTipoMovimiento = TipoMovimiento.Autorizacion;
                    PathHelp = "http://hntsolutions.net/manual/module_10_5.htm?ms=AAAAAAA%3D&st=MA%3D%3D&sct=MzQxOA%3D%3D&mw=MjU1";
                }
                else if (ClasesSGUV.Login.Rol != 1 && ClasesSGUV.Login.Rol != 2)
                { //quiere decir que es otro perfil que no es administrador ni gte regional
                    oTipoMovimiento = TipoMovimiento.GenerarFolio;
                    PathHelp = "http://hntsolutions.net/manual/module_10_6.htm?ms=AAAAAAA%3D&st=MA%3D%3D&sct=MzQxOA%3D%3D&mw=MjU1";
                }

                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 11);
                        command.Parameters.AddWithValue("@RolUsuLog", ClasesSGUV.Login.Rol);
                        command.CommandTimeout = 0;

                        DataSet ds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int Permiso = Convert.IsDBNull(ds.Tables[0].Rows[0]["PermisoFolios"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["PermisoFolios"]);
                                if (Permiso == 1)
                                    cboSucursal.Enabled = true;
                                else
                                    cboSucursal.Enabled = false;
                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_RutasADM", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoConsulta", 8);
                        command.CommandTimeout = 0;

                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);
                        cboSucursal.DataSource = dt;
                        cboSucursal.ValueMember = "Codigo";
                        cboSucursal.DisplayMember = "Nombre";
                        cboSucursal.SelectedIndex = -1;
                    }
                }

                if (cboSucursal.DataSource != null)
                    cboSucursal.SelectedValue = ClasesSGUV.Login.ClaveSucursal;

                log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Bitacora de rutas", MessageBoxButtons.OK);
            }
            

        }

        private void FrmAutorizacionesFoliosAux_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void FrmAutorizacionesFoliosAux_FormClosing(object sender, FormClosingEventArgs e)
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
