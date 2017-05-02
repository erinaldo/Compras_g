using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.Inventarios.Conteos
{
    public partial class frmAlert1 : Constantes.frmEmpty
    {
        public frmAlert1()
        {
            InitializeComponent();
        }

        private void btnContinuar_Click(object sender, EventArgs e)
        {
            if(rb3.Checked)
            if(string.IsNullOrEmpty(txtPath.Text))
            {
                MessageBox.Show("Debe seleccionar un archivo", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.Yes;
        }

        private void rbClick(object sender, EventArgs e)
        {
            btnExaminar.Visible = rb3.Checked;
            label3.Visible = rb3.Checked;
            txtPath.Visible = rb3.Checked;
            label4.Visible = rb3.Checked;
            if (!rb3.Checked)
                txtPath.Text = string.Empty;
        }

        private void frmAlert1_Load(object sender, EventArgs e)
        {
            rbClick(sender, e);
            nuevoToolStripButton.Enabled = false;
            exportarToolStripButton.Enabled = false;
            actualizarToolStripButton.Enabled = false;
            guardarToolStripButton.Enabled = false;
        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Selecciona un archivo.";
            dialog.Filter = "Texto (delimitado por tabulaciones)|*.txt";


            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtPath.Text = dialog.FileName;
            }
        }
    }
}
