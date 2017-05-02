using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace H_Compras.Datos
{
    public class Connection
    {
        private SqlConnection connection { get; set; }
        private String GetConnectionString(string name)
        {
            switch (name)
            {
                case "SGUV": return ClasesSGUV.Propiedades.conectionSGUV;
                case "LOG": return ClasesSGUV.Propiedades.conectionLog;
                case "RH": return "";
                default: return "";
            }
        }
        private SqlConnection GetConnection(string nameConnection)
        {
            try
            {
                connection = new SqlConnection(GetConnectionString(nameConnection));
                this.connection.Open();
                return this.connection;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private void CloseConnection()
        {
            if (this.connection != null)
            {
                this.connection.Close();
            }
        }

        public DataTable GetDataTable(string nameConnection, string nameProcedure, string[] nameOutParameters, string[] nameInParameters, ref Object[] valuesOut, params Object[] valuesIn)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            Connection conn = new Connection();
            cmd.Connection = conn.GetConnection(nameConnection);
            cmd.CommandText = nameProcedure;
            cmd.CommandType = CommandType.StoredProcedure;

            if (!String.IsNullOrEmpty(nameProcedure) && nameInParameters.Length == valuesIn.Length)
            {
                int i = 0;
                foreach (string parameter in nameInParameters)
                    cmd.Parameters.AddWithValue(parameter, valuesIn[i++]);
                foreach (string parameter in nameOutParameters)
                {
                    SqlParameter p = new SqlParameter(parameter, SqlDbType.VarChar, 250);
                    p.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(p);
                }

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    dt.Load(dr);
                    int j = 0;
                    foreach (SqlParameter item in cmd.Parameters)
                    {
                        if (item.Direction == ParameterDirection.Output)
                        {
                            valuesOut[j] = item.Value;
                            j++;
                        }
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                //   
            }
            throw new Exception("El número de parámetros no coincide con los valores enviados.");
        }
        public DataSet GetDataSet(string nameConnection, string nameProcedure, string[] nameOutParameters, string[] nameInParameters, ref Object[] valuesOut, params Object[] valuesIn)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            Connection conn = new Connection();
            cmd.Connection = conn.GetConnection(nameConnection);
            cmd.CommandText = nameProcedure;
            cmd.CommandType = CommandType.StoredProcedure;

            if (!String.IsNullOrEmpty(nameProcedure) && nameInParameters.Length == valuesIn.Length)
            {
                int i = 0;
                foreach (string parameter in nameInParameters)
                    cmd.Parameters.AddWithValue(parameter, valuesIn[i++]);
                foreach (string parameter in nameOutParameters)
                {
                    SqlParameter p = new SqlParameter(parameter, SqlDbType.VarChar, 250);
                    p.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(p);
                }

                try
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = cmd;
                    da.Fill(ds);
                    int j = 0;
                    foreach (SqlParameter item in cmd.Parameters)
                    {
                        if (item.Direction == ParameterDirection.Output)
                        {
                            valuesOut[j] = item.Value;
                            j++;
                        }
                    }
                    return ds;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                //   
            }
            throw new Exception("El número de parámetros no coincide con los valores enviados.");
        }

        public int Ejecutar(string nameConnection, string nameProcedure, string[] nameOutParameters, string[] nameInParameters, ref Object[] valuesOut, params Object[] valuesIn)
        {
            SqlCommand cmd = new SqlCommand();
            Connection conn = new Connection();
            cmd.Connection = conn.GetConnection(nameConnection);
            cmd.CommandText = nameProcedure;
            cmd.CommandType = CommandType.StoredProcedure;

            if (!String.IsNullOrEmpty(nameProcedure) && nameInParameters.Length == valuesIn.Length)
            {
                int i = 0;
                foreach (string parameter in nameInParameters)
                    cmd.Parameters.AddWithValue(parameter, valuesIn[i++]);
                foreach (string parameter in nameOutParameters)
                {
                    SqlParameter p = new SqlParameter(parameter, SqlDbType.VarChar, 250);
                    p.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(p);
                }

                try
                {
                    int r = cmd.ExecuteNonQuery();
                    int j = 0;
                    foreach (SqlParameter item in cmd.Parameters)
                    {
                        if (item.Direction == ParameterDirection.Output)
                        {
                            valuesOut[j] = item.Value;
                            j++;
                        }
                    }
                    conn.CloseConnection();
                    return r;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                //   
            }
            throw new Exception("El número de parámetros no coincide con los valores enviados.");
        }

    }
}
