using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.SDK.Varios
{
    public partial class frmTransaccionesFR : Constantes.frmEmpty
    {
        Datos.Connection connection = new Datos.Connection();
            
        public frmTransaccionesFR()
        {
            InitializeComponent();
        }


        public frmTransaccionesFR(int DocEntry)
        {
            InitializeComponent();

            try
            {
                Object[] valuesOut = new Object[] { };
                DataTable tbl =
                    connection.GetDataTable("LOG",
                                            "sp_SDKDocuments",
                                            new string[] { },
                                            new string[] { "@TipoConsulta", "@DocEntry" },
                                            ref valuesOut, 6, DocEntry);

                textBox1.Text = tbl.Rows[0]["DocNum"].ToString();
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void frmTransaccionesFR_Load(object sender, EventArgs e)
        {
            guardarToolStripButton.Enabled = false;
            actualizarToolStripButton.Enabled = false;

            nuevoToolStripButton.Click -= new EventHandler(btnNuevo_Click);
            nuevoToolStripButton.Click += new EventHandler(btnNuevo_Click);

            if (textBox1.Text != string.Empty)
                btnConsultar_Click(sender, e);
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                    Object[] valuesOut = new Object[] { };
                    DataTable tbl =
                        connection.GetDataTable("LOG",
                                                "sp_SDKDocuments",
                                                new string[] { },
                                                new string[] { "@TipoConsulta", "@idDocumento" },
                                                ref valuesOut, 5, textBox1.Text);

                    dgvDatos.DataSource = tbl;
            }
            catch (Exception ex) 
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            dgvDatos.DataSource = null;
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }

            e.Layout.Bands[0].Columns["Importe"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns["Importe"].Format = "C2";
        }
    }
}
