using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace H_Compras.Constantes
{
    public partial class frmVisor : Constantes.frmEmpty
    {
        ReportDocument Document;
        DataSet DS;
        string PathRPT = string.Empty;

        public frmVisor(ReportDocument document)
        {
            InitializeComponent();

            Document = document;
        }

        public frmVisor()
        {
            InitializeComponent();
        }

        public frmVisor(DataSet _ds, string pathRPT)
        {
            InitializeComponent();

            PathRPT = pathRPT;
            DS = _ds;
        }

        private void frmVisor_Load(object sender, EventArgs e)
        {
            try
            {
                nuevoToolStripButton.Enabled = false;
                exportarToolStripButton.Enabled = false;
                guardarToolStripButton.Enabled = false;
                actualizarToolStripButton.Enabled = false;
                ayudaToolStripButton.Enabled = false;

                if (String.IsNullOrEmpty(PathRPT))
                {
                    //  crystalReportViewer1.ReportSource = Document;
                }
                else
                {
                    Document = new ReportDocument();
                    Document.Load(PathRPT);
                    Document.SetDataSource(DS);
                }

                crystalReportViewer1.ReportSource = Document;

            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }
    }
}
