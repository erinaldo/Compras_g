using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Datos.Clases
{
    public static class DataSource
    {
        public enum Tipos
        {
            GetZonas = 1,
            AlmacenesVenta = 2,
            AlmacenesZona = 3,
            Proveedores = 4,
            GetZonaProveedor = 5,
            GetArticulos = 6,
            GetGruposZona = 7,
            GrupoSN = 8,
            AlmacenesZonaProveedor = 9,
            AlmacenesIndependientesProveedor = 10,
            AlmacenesProveedor = 11
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
                    command.Parameters.AddWithValue("@UserID", Clases.Constantes.UserID);

                    DataTable tbl = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl);

                    return tbl;
                }
            }
        }
    }
}
