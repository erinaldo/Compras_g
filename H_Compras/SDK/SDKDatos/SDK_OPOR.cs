using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Sql;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace H_Compras.SDK.SDKDatos
{
    public class SDK_OPOR
    {
        private decimal docEntry;

        public decimal DocEntry
        {
            get { return docEntry; }
            set { docEntry = value; }
        }
        private decimal docNum;

        public decimal DocNum
        {
            get { return docNum; }
            set { docNum = value; }
        }
        private string cardCode;

        public string CardCode
        {
            get { return cardCode; }
            set { cardCode = value; }
        }
        private string cardName;

        public string CardName
        {
            get { return cardName; }
            set { cardName = value; }
        }
        private string docCur;

        public string DocCur
        {
            get { return docCur; }
            set { docCur = value; }
        }
        private string numAtCard;

        public string NumAtCard
        {
            get { return numAtCard; }
            set { numAtCard = value; }
        }
        private string docStatus;

        public string DocStatus
        {
            get { return docStatus; }
            set { docStatus = value; }
        }
        private DateTime docDate;

        public DateTime DocDate
        {
            get { return docDate; }
            set { docDate = value; }
        }
        private DateTime docDueDate;

        public DateTime DocDueDate
        {
            get { return docDueDate; }
            set { docDueDate = value; }
        }
        private string comments;

        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }
        private decimal docTotal;

        public decimal DocTotal
        {
            get { return docTotal; }
            set { docTotal = value; }
        }
        private decimal vatSum;

        public decimal VatSum
        {
            get { return vatSum; }
            set { vatSum = value; }
        }

        private string folioProv;

        public string FolioProv
        {
            get { return folioProv; }
            set { folioProv = value; }
        } 

        private List<SDK_POR1> lines = new List<SDK_POR1>();

        public List<SDK_POR1> Lines
        {
            get { return lines; }
            set { lines = value; }
        }

        private string nameProc = "sp_SDKDataSource";

        public SDK_OPOR Fill(decimal _docEntry)
        {
            SDK_OPOR document = new SDK_OPOR();

            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand(nameProc, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TipoConsulta", 4);
                    command.Parameters.AddWithValue("@Key", _docEntry);

                    DataTable tbl_Header = new DataTable();
                    SqlDataAdapter da= new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl_Header);
                    document.DocEntry = tbl_Header.Rows[0].Field<Int32>("DocEntry");
                    document.DocNum = tbl_Header.Rows[0].Field<Int32>("DocNum");
                    document.CardCode = tbl_Header.Rows[0].Field<string>("CardCode");
                    document.CardName = tbl_Header.Rows[0].Field<string>("CardName");
                    document.DocCur = tbl_Header.Rows[0].Field<string>("DocCur");
                    document.NumAtCard = tbl_Header.Rows[0].Field<string>("NumAtCard");
                    document.DocStatus = tbl_Header.Rows[0].Field<string>("DocStatus");
                    document.DocDate = tbl_Header.Rows[0].Field<DateTime>("DocDate");
                    document.DocDueDate = tbl_Header.Rows[0].Field<DateTime>("DocDueDate");
                    document.Comments = tbl_Header.Rows[0].Field<string>("Comments");
                    document.FolioProv = tbl_Header.Rows[0].Field<string>("U_FPROV");
                    //document.TaxCode = tbl.Rows[0].Field<string>("TaxCode");

                    document.DocTotal = document.DocCur == "USD" ? tbl_Header.Rows[0].Field<decimal>("DocTotalFC") : tbl_Header.Rows[0].Field<decimal>("DocTotal");
                    document.VatSum = document.DocCur == "USD" ? tbl_Header.Rows[0].Field<decimal>("VatSumFC") : tbl_Header.Rows[0].Field<decimal>("VatSum");
                }
            }

            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand(nameProc, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TipoConsulta", 5);
                    command.Parameters.AddWithValue("@Key",  document.DocEntry);

                    DataTable tbl_Details = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl_Details);

                    foreach (DataRow item in tbl_Details.Rows)
                    {
                        SDK_POR1 line = new SDK_POR1();
                        line.LineNum = item.Field<Int32>("LineNum");
                        line.ItemCode = item.Field<string>("ItemCode");
                        line.Dscription = item.Field<string>("Dscription");
                        line.Quantity = item.Field<decimal>("Quantity");
                        line.OpenQty = item.Field<decimal>("OpenQty");
                        line.Price = item.Field<decimal>("Price");
                        line.Whscode = item.Field<string>("Whscode");
                        line.WhsName = item.Field<string>("WhsName");
                        line.LineTotal = item.Field<decimal>("LineTotal");
                        line.ShipDate = item.Field<DateTime>("ShipDate");
                        line.LineStatus = item.Field<string>("LineStatus");
                        line.U_Comentario = item.Field<string>("U_Comentario");
                        line.U_Vendedor = item.Field<string>("U_Vendedor");
                        line.Rate = item.Field<string>("Currency") == document.DocCur ? 1 :
                            item.Field<decimal>("Rate") == decimal.Zero ? 1 : item.Field<decimal>("Rate");
                        line.Currency = item.Field<string>("Currency");

                        line.PesoU = item.Field<decimal>("BWeight1");
                        line.VolumenU = item.Field<decimal>("BVolume");

                        document.Lines.Add(line);
                    }
                }
            }
            return document;
        }

    }
}
