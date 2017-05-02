using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using System.Diagnostics;

namespace H_Compras.Logistica
{
    public class csLogistica
    {
        public static DataTable ConsultaCatalogo(int TipoConsulta)
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_DataSource", connection))
                {
                    connection.Open();

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", TipoConsulta);

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = command;

                    adapter.Fill(table);
                }
            }
            return table;
        }
        public static int InsertUpdateSolicitudes(int TipoConsulta, int Folio, DateTime dFecha, int iVendedor, string Vendedor, string Destino, int TipoEntrega)
        {
            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("up_Logistica", connection))
                {
                    connection.Open();

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", TipoConsulta);
                    if (Folio <= 0)
                    {
                        SqlParameter ID = new SqlParameter("@iFolioSolicitud", SqlDbType.Int);
                        ID.Direction = ParameterDirection.Output; ID.Value = -1; command.Parameters.Add(ID);
                    }
                    else
                        command.Parameters.AddWithValue("@iFolioSolicitud", Folio);
                    command.Parameters.AddWithValue("@FechaSolicitud", dFecha);
                    command.Parameters.AddWithValue("@iVendedor", iVendedor);
                    command.Parameters.AddWithValue("@Vendedor", Vendedor);
                    command.Parameters.AddWithValue("@Destino", Destino);
                    command.Parameters.AddWithValue("@TipoEntrega", TipoEntrega);
                    command.Parameters.AddWithValue("@Usuario", ClasesSGUV.Login.Id_Usuario);

                    command.ExecuteNonQuery();

                    if (Folio <= 0)
                    {
                        Folio = Convert.IsDBNull(command.Parameters["@iFolioSolicitud"].Value) ? -1 : Convert.ToInt32(command.Parameters["@iFolioSolicitud"].Value);
                    }
                    return Folio;
                }
            }

        }
        public static DataTable ExisteFactura(int TipoConsulta, int Factura, string TipoDoc)
        {
            bool Existe = false;
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("up_Logistica", connection))
                {
                    connection.Open();

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", TipoConsulta);
                    command.Parameters.AddWithValue("@NoFactura", Factura);
                    command.Parameters.AddWithValue("@TipoDoc", TipoDoc);

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = command;

                    adapter.Fill(table);
                    //if (table.Rows.Count > 0)
                    //    Existe = true;
                }
            }
            return table;//Existe;
        }
        public static int InsertFacturas(int TipoConsulta, int idDetalleSolicitud, int Folio, int Factura, string Tipo)
        {
            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("up_Logistica", connection))
                {
                    connection.Open();

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", TipoConsulta);
                    if (idDetalleSolicitud <= 0)
                    {
                        SqlParameter ID = new SqlParameter("@IDDetalle", SqlDbType.Int);
                        ID.Direction = ParameterDirection.Output; ID.Value = -1; command.Parameters.Add(ID);
                    }
                    else
                        command.Parameters.AddWithValue("@IDDetalle", idDetalleSolicitud);

                    command.Parameters.AddWithValue("@iFolioSolicitud", Folio);
                    command.Parameters.AddWithValue("@NoFactura", Factura);
                    command.Parameters.AddWithValue("@TipoDoc", Tipo);

                    command.ExecuteNonQuery();

                    if (idDetalleSolicitud <= 0)
                    {
                        idDetalleSolicitud = Convert.IsDBNull(command.Parameters["@IDDetalle"].Value) ? -1 : Convert.ToInt32(command.Parameters["@IDDetalle"].Value);
                    }
                    return idDetalleSolicitud;
                }
            }

        }

        public static DataTable ConsultaSolicitudesLogistica(int TipoConsulta, DateTime Inicio, DateTime Fin, int Proceso)
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("up_Logistica", connection))
                {
                    connection.Open();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", TipoConsulta);
                    command.Parameters.AddWithValue("@FechaInicio", Inicio);
                    command.Parameters.AddWithValue("@FechaFin", Fin);
                    command.Parameters.AddWithValue("@Proceso", Proceso);
                    command.Parameters.AddWithValue("@Usuario", ClasesSGUV.Login.Id_Usuario);
                    command.Parameters.AddWithValue("@Rol", ClasesSGUV.Login.Rol);

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = command;
                    adapter.Fill(table);
                }
            }
            return table;
        }

        public static DataTable UpdateSolicitudFactura(int TipoConsulta, int Solicitud, decimal CostoEn, string Company, string NumRastreo, decimal Porciento, string Columna)
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("up_Logistica", connection))
                {
                    connection.Open();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", TipoConsulta);
                    command.Parameters.AddWithValue("@iFolioSolicitud", Solicitud);
                    command.Parameters.AddWithValue("@CostoEnvio", CostoEn);
                    command.Parameters.AddWithValue("@Compania", Company);
                    command.Parameters.AddWithValue("@NumRastreo", NumRastreo);
                    command.Parameters.AddWithValue("@PorcientoVenta", Porciento);
                    command.Parameters.AddWithValue("@ColumUpdate", Columna);

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = command;
                    adapter.Fill(table);
                }
            }
            return table;
        }

        public static DataTable UpdateEstatusSolicitudLogistica(int TipoConsulta, int Solicitud)
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("up_Logistica", connection))
                {
                    connection.Open();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", TipoConsulta);
                    command.Parameters.AddWithValue("@iFolioSolicitud", Solicitud);

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = command;
                    adapter.Fill(table);
                }
            }
            return table;
        }

        public static void EliminaRelacionFactura(int TipoConsulta, int Relacion)
        {
            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("up_Logistica", connection))
                {
                    connection.Open();

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", TipoConsulta);
                    command.Parameters.AddWithValue("@IDDetalle", Relacion);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void ImprimirSolicitud(string Folio, string Ruta, string NomArchivo)
        {
            try
            {
                if (Folio != string.Empty)
                {
                    ReportDocument doc = new ReportDocument();
                    ConnectionInfo crConnectionInfo = new ConnectionInfo();
                    TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                    Tables CrTables;

                    //doc = new Logistica.rptFacturas_Logistica();
                    string _crystal = @"\\192.168.2.98\Archivos SAP\FLOT\Crystal\rptFacturas_Logistica.rpt";
                    doc.Load(_crystal);

                    crConnectionInfo.ServerName = "192.168.2.100";
                    crConnectionInfo.DatabaseName = "PJ-Log";
                    crConnectionInfo.UserID = "sa";
                    crConnectionInfo.Password = "SAP-PJ1";
                    CrTables = doc.Database.Tables;

                    foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
                    {
                        crtableLogoninfo = CrTable.LogOnInfo;
                        crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                        CrTable.ApplyLogOnInfo(crtableLogoninfo);
                    }

                    doc.SetParameterValue(0, Convert.ToInt32(Folio));

                    string RutaCompleta = Ruta + NomArchivo;
                    if (!System.IO.Directory.Exists(Ruta))
                    {
                        System.IO.Directory.CreateDirectory(Ruta);
                    }
                    if (File.Exists(RutaCompleta))
                    {
                        //DialogResult result = MessageBox.Show("¿Ya existe un archivo con el mismo nombre, desea reemplazarlo por el actual?", "Confirmación de Reemplazo", MessageBoxButtons.YesNoCancel);
                        //if (result == DialogResult.Yes)
                        //{
                        //    try
                        //    {
                        doc.ExportToDisk(ExportFormatType.PortableDocFormat, RutaCompleta);
                        Process prc = new Process();
                        prc.StartInfo.FileName = RutaCompleta;
                        prc.Start();
                        //}
                        //catch (Exception ex)
                        //{
                        //    Cursor = Cursors.Default;
                        //    MessageBox.Show(ex.Message.ToString());
                        //}
                        //}
                    }
                    else
                    {
                        doc.ExportToDisk(ExportFormatType.PortableDocFormat, RutaCompleta);
                        Process prc = new Process();
                        prc.StartInfo.FileName = RutaCompleta;
                        prc.Start();
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static void UpdateRutaSolicitud(int TipoConsulta, int Folio, string Ruta)
        {
            using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("up_Logistica", connection))
                {
                    connection.Open();

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", TipoConsulta);
                    command.Parameters.AddWithValue("@iFolioSolicitud", Folio);
                    command.Parameters.AddWithValue("@RutaLogistica", Ruta);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static int ConsultaTipoPermisosProceso(int TipoConsulta)
        {
            int TipoProceso = -1;
            try
            {
                DataTable table = new DataTable();
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_Logistica", connection))
                    {
                        connection.Open();
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", TipoConsulta);
                        command.Parameters.AddWithValue("@Usuario", ClasesSGUV.Login.Id_Usuario);
                        command.Parameters.AddWithValue("@Rol", ClasesSGUV.Login.Rol);

                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = command;
                        adapter.Fill(table);

                        if (table.Rows.Count > 0)
                            TipoProceso = Convert.IsDBNull(table.Rows[0]["Proceso"]) ? -1 : Convert.ToInt32(table.Rows[0]["Proceso"]);

                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
            return TipoProceso;
        }

        public static string ConsultaCorreosNotificar(int TipoConsulta, int Proceso, int Solicitud)
        {
            string Correos = "";
            DataTable table = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(ClasesSGUV.Propiedades.conectionLog))
                {
                    using (SqlCommand command = new SqlCommand("up_Logistica", connection))
                    {
                        connection.Open();
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TipoConsulta", TipoConsulta);
                        command.Parameters.AddWithValue("@Proceso", Proceso);
                        command.Parameters.AddWithValue("@iFolioSolicitud", Solicitud);

                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = command;
                        adapter.Fill(table);

                        if (table.Rows.Count > 0)
                            Correos = table.Rows[0]["Correos"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return Correos;
        }
    }
}
