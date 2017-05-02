using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace H_Compras.SDK.SDKDatos
{
    public class SDK_OWTR
    {
        decimal docEntry;

        public decimal DocEntry
        {
            get { return docEntry; }
            set { docEntry = value; }
        }

        decimal docNum;

        public decimal DocNum
        {
            get { return docNum; }
            set { docNum = value; }
        }

        string docStatus;

        public string DocStatus
        {
            get { return docStatus; }
            set { docStatus = value; }
        }
        DateTime docDate;

        public DateTime DocDate
        {
            get { return docDate; }
            set { docDate = value; }
        }
        string filler;

        public string Filler
        {
            get { return filler; }
            set { filler = value; }
        }
        string toWhsCode;

        public string ToWhsCode
        {
            get { return toWhsCode; }
            set { toWhsCode = value; }
        }

        string comments;

        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }

        public string U_TipoSolicitud
        {
            get;
            set;
        }

        public string U_FolioSolicitud
        {
            get;
            set;
        }

        List<SDK_WTR1> items = new List<SDK_WTR1>();

        public List<SDK_WTR1> Lines
        {
            get { return items; }
            set { items = value; }
        }

        /// <summary>
        /// Llena un Objeto SDK_OWTR (Traspaso de stock|Solicitud de mercancias)
        /// </summary>
        /// <param name="_docEntry">Key del documento</param>
        /// <param name="_isSolicitud">true(Solicitud de mercancias)|false(Traspaso de mercancias)</param>
        /// <returns></returns>
        public SDK_OWTR Fill_Previo(decimal _docEntry, bool _isSolicitud)
        {
            SDK_OWTR document = new SDK_OWTR();
            document.DocEntry = _docEntry;

            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_SDKDocumentosPrevios", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    if (!_isSolicitud)
                        command.Parameters.AddWithValue("@TipoConsulta", 4);
                    else
                        command.Parameters.AddWithValue("@TipoConsulta", 10);

                    command.Parameters.AddWithValue("@DocKey", document.DocEntry);

                    DataTable tbl = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl);

                    document.DocEntry = Convert.ToDecimal(tbl.Rows[0]["DocEntry"]);
                    document.DocNum = Convert.ToDecimal(tbl.Rows[0]["DocNum"]);
                    document.DocDate = tbl.Rows[0].Field<DateTime>("DocDate");
                    document.Filler = tbl.Rows[0].Field<string>("Filler");
                    document.ToWhsCode = tbl.Rows[0].Field<string>("ToWhsCode");
                    document.Comments = tbl.Rows[0].Field<string>("JrnlMemo");
                }
            }

            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_SDKDocumentosPrevios", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    if (!_isSolicitud)
                        command.Parameters.AddWithValue("@TipoConsulta", 5);
                    else
                        command.Parameters.AddWithValue("@TipoConsulta", 11);

                    command.Parameters.AddWithValue("@DocKey", document.DocEntry);

                    DataTable tbl = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl);

                    foreach (DataRow item in tbl.Rows)
                    {
                        SDK_WTR1 line = new SDK_WTR1();
                        line.LineNum = item.Field<Int32>("LineNum");
                        line.LineStatus = item.Field<string>("LineStatus");
                        line.ItemCode = item.Field<string>("ItemCode");
                        line.Dscription = item.Field<string>("Dscription");

                        line.WhsCode = item.Field<string>("WhsCode");
                        line.WhsName = item.Field<string>("WhsName");
                        line.FromWhsCode = item.Field<string>("FromWhsCode");
                        line.FromWhsName = item.Field<string>("FromWhsName");

                        line.Quantity = item.Field<decimal>("Quantity");

                        line.BWeight1 = item["BWeight1"] == DBNull.Value ? decimal.Zero : item.Field<decimal>("BWeight1");
                        line.BVolume = item["BVolume"] == DBNull.Value ? decimal.Zero : item.Field<decimal>("BVolume");// item.Field<decimal>("BVolume");
                        line.ManBtchNum = item.Field<string>("ManBtchNum");

                        if (item["U_Tarima"] != DBNull.Value)
                            line.U_Tarima = item.Field<Int32>("U_Tarima").ToString();
                        if (item["U_TipoAlmName"] != DBNull.Value)
                            line.U_TipoAlmName = item.Field<string>("U_TipoAlmName");
                        if (item["U_TipoAlm"] != DBNull.Value)
                            line.U_TipoAlm = item.Field<string>("U_TipoAlm");

                        //if (_isSolicitud)
                        //{
                        //    if (item["BaseType"] == DBNull.Value)
                        //        line.BaseType = item.Field<Int32>("BaseType");
                        //    if (item["BaseEntry"] == DBNull.Value)
                        //        line.BaseEntry = item.Field<Int32>("BaseEntry");
                        //    if (item["BaseLine"] == DBNull.Value)
                        //        line.BaseLine = item.Field<Int32>("BaseLine");
                        //}

                        document.Lines.Add(line);
                    }
                }
            }
            return document;
        }

        public void CambiarStatus(SDK_OWTR document, string newValor)
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_SDKDocumentosPrevios", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 6);
                    command.Parameters.AddWithValue("@DocStatus", newValor);
                    command.Parameters.AddWithValue("@DocNum", document.DocNum);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        public void CambiarLineStatus(SDK_OWTR document, string newValor, string origen, string destino)
        {
            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_SDKDocumentosPrevios", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 7);
                    command.Parameters.AddWithValue("@LineStatus", newValor);
                    command.Parameters.AddWithValue("@DocNum", document.DocNum);
                    command.Parameters.AddWithValue("@ToWhsCode", destino);//Destino
                    command.Parameters.AddWithValue("@Filler", origen);//Origen

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }
    }
}
