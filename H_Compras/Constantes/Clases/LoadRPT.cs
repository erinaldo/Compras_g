using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using System.Drawing.Printing;

namespace H_Compras.Constantes.Clases
{
    public class LoadRPT
    {
        string pathCrystal;

        public string PathCrystal
        {
            get { return pathCrystal; }
            set { pathCrystal = value; }
        }

        ReportDocument docRPT;

        public ReportDocument DocRPT
        {
            get { return docRPT; }
            set { docRPT = value; }
        }
        Tables CrTables;
        TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
        TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
        ConnectionInfo crConnectionInfo = new ConnectionInfo();

        public LoadRPT(int TypeDocument)
        {

            using (SqlConnection connection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
            {
                using (SqlCommand command = new SqlCommand("sp_SDKDocuments", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoConsulta", 1);
                    command.Parameters.AddWithValue("@idDocumento", TypeDocument);

                    DataTable tbl = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    da.Fill(tbl);
                    docRPT = new ReportDocument();
                    pathCrystal = tbl.Rows[0].Field<string>("pathRPT"); ;
                    docRPT.Load(pathCrystal);
                    crConnectionInfo.ServerName = tbl.Rows[0].Field<string>("IPServer"); ;
                    crConnectionInfo.DatabaseName = tbl.Rows[0].Field<string>("CompanyDB"); ;
                    crConnectionInfo.UserID = tbl.Rows[0].Field<string>("DbUserName"); ;
                    crConnectionInfo.Password = tbl.Rows[0].Field<string>("DbPassword"); ;
                    CrTables = docRPT.Database.Tables;
                }
            }

            foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
            {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }
        }

        public LoadRPT(string path)
        {
            docRPT = new ReportDocument();
            pathCrystal = path;
            docRPT.Load(pathCrystal);
            crConnectionInfo.ServerName = "192.168.2.100";
            crConnectionInfo.DatabaseName = "SBO-DistPJ";
            crConnectionInfo.UserID = "sa";
            crConnectionInfo.Password = "SAP-PJ1";
            CrTables = docRPT.Database.Tables;


            foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
            {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }
        }

        public string GenerarPDF(string DocEntry, string path = "")
        {
            try
            {
                docRPT.SetParameterValue("DocKey@", DocEntry);
                string _rutaPDF = string.Empty;
                if (string.IsNullOrEmpty(path))
                {
                    _rutaPDF = Path.GetTempPath() + DocEntry + ".pdf";
                    docRPT.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, _rutaPDF);
                }
                else
                {
                    _rutaPDF = path;
                    docRPT.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, _rutaPDF);
                }
                return _rutaPDF;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string Print(string DocEntry)
        {
            try
            {
                docRPT.SetParameterValue("DocKey@", DocEntry);

                string NombreImpresora = "";
                for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
                {
                    PrinterSettings a = new PrinterSettings();
                    a.PrinterName = PrinterSettings.InstalledPrinters[i].ToString();
                    if (a.IsDefaultPrinter)
                    {
                        NombreImpresora = PrinterSettings.InstalledPrinters[i].ToString();
                    }
                }

                docRPT.PrintOptions.PrinterName = NombreImpresora;
                docRPT.PrintToPrinter(1, false, 0, 0);

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public void SetParameter(string DocEntry)
        {
            try
            {
                docRPT.SetParameterValue("DocKey@", DocEntry);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

    }
}
