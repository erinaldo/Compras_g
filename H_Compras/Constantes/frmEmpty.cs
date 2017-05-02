using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

namespace H_Compras.Constantes
{
    public partial class frmEmpty : Form
    {
        private int _TIME;
        private UltraGrid ug;
        ClasesSGUV.Logs log;
        private Clases.Validaciones objValidaciones;

        public Clases.Validaciones ObjValidaciones
        {
            get { return objValidaciones; }
            set { objValidaciones = value; }
        }

        public ClasesSGUV.Logs Log
        {
            get { return log; }
            set { log = value; }
        }

        public UltraGrid Ug
        {
            get { return ug; }
            set { ug = value; }
        }
        DataGridView dg;

        public DataGridView Dg
        {
            get { return dg; }
            set { dg = value; }
        }

        public string PathHelp
        {
            get ;
            set ;
        }

        public frmEmpty()
        {
            InitializeComponent();
        }

        private void ClearMensaje()
        {
            System.Threading.Thread.Sleep(_TIME);

            toolStatus.Text = string.Empty;
            toolStatus.BackColor = Color.FromName("Control"); ;
            toolStatus.ForeColor = Color.Black;
        }

        //const Color back = Color.FromName("Control");
        //const Color fore = Color.Black;

         public void SetMensaje(string _mensaje, int _time = 5000, Color _backColor = new Color(), Color _foreColor = new Color())
        {
            try
            {
                toolStatus.Text = _mensaje;
                toolStatus.BackColor = _backColor;
                toolStatus.ForeColor = _foreColor;

                _TIME = _time;

                System.Threading.Thread hilo = new System.Threading.Thread(ClearMensaje);

                hilo.Start();

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void frmEmpty_Load(object sender, EventArgs e)
        {
            try
            {
                objValidaciones = new Clases.Validaciones();
                CheckForIllegalCrossThreadCalls = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exportarToolStripButton_Click(object sender, EventArgs e)
        {
           
                if (ug != null)
                { if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                { 
                    Infragistics.Excel.Workbook book = null;
                    book = ultraGridExcelExporter1.Export(ug, saveFileDialog.FileName);

                    if (book != null)
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                }
                }
                else if(dg != null)
                {
                    ClasesSGUV.Exportar exp = new ClasesSGUV.Exportar();
                    exp.ExportarDatos(dg, false);
             
                    System.Diagnostics.Process.Start(exp.StringPath);
                }
        }

        private void frmEmpty_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                log.Fin();
            }
            catch (Exception)
            {

            }
        }

        private void frmEmpty_Shown(object sender, EventArgs e)
        {
            try
            {
                log.ID = log.Inicio();
            }
            catch (Exception)
            {

            }
        }

        private void ayudaToolStripButton_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, PathHelp);
        }

    }
}
