using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace H_Compras.Constantes.Clases
{
    public class Validaciones
    {
        private bool _result;

        public bool Result
        {
            get { return _result; }
            set { _result = value; }
        }
        private string _mensaje;

        public string Mensaje
        {
            get { return _mensaje; }
            set { _mensaje = value; }
        }

        public enum ObjectType
        {
            OrdenCompra = 1 // OPOR
        }

        public bool ValiarDocto(string DocKey, string Filter, int Tipo)
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_Validaciones", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", Tipo);
                    command.Parameters.AddWithValue("@DocKey", DocKey);
                    command.Parameters.AddWithValue("@Filter", Filter);
                    command.Parameters.Add("@Mensaje", SqlDbType.VarChar, 5000).Direction = ParameterDirection.Output;

                    connection.Open();

                    bool x = Convert.ToBoolean(command.ExecuteScalar());

                    _mensaje = Convert.ToString(command.Parameters["@Mensaje"].Value.ToString());
                    return x;
                }
            }
        }

    }
}
