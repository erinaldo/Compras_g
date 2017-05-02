using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace H_Compras.Datos.Clases
{
    public static class DataSource_
    {
        public enum TipoQry
        {
            GetZonas = 1,
            AlmacenesVenta = 2,
            AlmacenesZona = 3,
            Proveedores = 4,
            GetZonaProveedor = 5,
            GetArticulos = 6,
            PlaneadoresCompra = 7,
            GrupoSN = 8,
            AlmacenesZonaProveedor = 9,
            AlmacenesIndependientesProveedor = 10,
            AlmacenesProveedor = 11,
            AlmacenesReubicaciones = 12,
            Lineas = 13,
            Compradores = 14,
            OrigenesTraspasos = 15,
            DestinosTraspasos = 16,
            AlmacenesTodos = 17,
            ItemsConteoInventario = 18,
            DocumentosSDK  = 19,
            ListaVendedores = 20,
            ListadoAlmacenistas = 21,
            ListadoAlmecenes = 22,
            LineasTodas = 23,
            AlmacenPorUsuario = 24,
            ListasPrecios = 25,
            DocMetodosPago = 26,
            DocMotivo = 27,
            DocIndicator = 28,
            DocSeries = 29,
            ListaRamosCanales = 30,
            ListaCountry = 31,
            ListaEstados = 32,
            ListaIndicadorImpuestos = 33,
            ListaCondicionesPago = 34,
            ListaMetodosPago = 35,
            ListaTipoVisita = 36,
            ListaTipoSocio = 37,
            ListaMonedas = 38,
            Vendedor = 39,
            ListaTodosAlmacenes = 40,
            StockAlmacenArticulo = 41,
            IdealReubicacionArticulo = 42,
            ListaClientes = 43


        }

        public static DataTable GetSource(int _tipo, object _filter, string _zona)
        {
            using (SqlConnection connection = new SqlConnection(Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_DataSource", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", _tipo);
                    command.Parameters.AddWithValue("@Filtro", _filter);
                    command.Parameters.AddWithValue("@Zona", _zona);
                    command.Parameters.AddWithValue("@UserID", ClasesSGUV.Login.Id_Usuario);

                    DataTable tbl = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl);

                    return tbl;
                }
            }
        }

        public static DataTable GetQry(string name)
        {
            string qry = string.Empty;
            using (SqlConnection connection = new SqlConnection(Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("Select * From tbl_QryRpt Where rptName = @name", connection))
                {
                    command.Parameters.AddWithValue("@name", name);

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable tbl = new DataTable();

                    da.Fill(tbl);

                    return tbl;
              }
            }
        }
    }
}
