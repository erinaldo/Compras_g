using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace H_Compras.SDK.SDKDatos
{
    public class DocumentosPrevios
    {
        string nameProc = "sp_SDKDocumentosPrevios";
        public decimal GuardarSolicitudTraslado(SDK_OWTR document)
        {
            try
            {
                decimal Folio = decimal.Zero;

                using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();

                    SqlTransaction transaction = connection.BeginTransaction("Insert_TraspasoStock");
                    command.Transaction = transaction;
                    command.Connection = connection;
                    SqlParameter dockey = new SqlParameter("@DocKey", SqlDbType.VarChar, 50);
                    dockey.Direction = ParameterDirection.Output;
                    try
                    {
                        command.CommandText = nameProc;
                        command.CommandType = CommandType.StoredProcedure;
                        #region HEADER
                        command.Parameters.AddWithValue("@TipoConsulta", 1);
                        command.Parameters.AddWithValue("@DocNum", document.DocNum);
                        command.Parameters.AddWithValue("@DocStatus", document.DocStatus);
                        command.Parameters.AddWithValue("@DocDate", document.DocDate);
                        command.Parameters.AddWithValue("@Filler", document.Filler);
                        command.Parameters.AddWithValue("@ToWhsCode", document.ToWhsCode);
                        command.Parameters.AddWithValue("@Comments", document.Comments);
                        command.Parameters.AddWithValue("@UserID", ClasesSGUV.Login.Id_Usuario);
                        command.Parameters.Add(dockey);
                        command.ExecuteNonQuery();
                        #endregion
                        Folio = Convert.ToDecimal(command.Parameters["@DocKey"].Value);
                        document.DocNum = Folio;
                        #region DETAILS
                        foreach (var item in document.Lines)
                        {
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@TipoConsulta", 2);

                            command.Parameters.AddWithValue("@DocNum", document.DocNum);
                            command.Parameters.AddWithValue("@LineNum", item.LineNum);
                            command.Parameters.AddWithValue("@ItemCode", item.ItemCode);
                            command.Parameters.AddWithValue("@Dscription", item.Dscription);
                            command.Parameters.AddWithValue("@WhsCode", item.WhsCode);
                            command.Parameters.AddWithValue("@WhsName", item.WhsName);
                            command.Parameters.AddWithValue("@FromWhsCode", item.FromWhsCode);
                            command.Parameters.AddWithValue("@FromWhsName", item.FromWhsName);
                            command.Parameters.AddWithValue("@Quantity", item.Quantity);
                            command.Parameters.AddWithValue("@BWeight1", item.BWeight1);
                            command.Parameters.AddWithValue("@BVolume", item.BVolume);
                            command.Parameters.AddWithValue("@ManBtchNum", item.ManBtchNum);
                            command.Parameters.AddWithValue("@U_Tarima", item.U_Tarima);
                            command.Parameters.AddWithValue("@U_TipoAlm", item.U_TipoAlm);
                            command.Parameters.AddWithValue("@U_TipoAlmName", item.U_TipoAlmName);
                            command.Parameters.AddWithValue("@userID", ClasesSGUV.Login.Id_Usuario);

                            command.ExecuteNonQuery();
                        }
                        #endregion
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                    return Folio;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
