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
    public partial class frmAlert2 : Constantes.frmEmpty
    {
        public frmAlert2()
        {
            InitializeComponent();
        }

        private void btnContinuar_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Yes;
        }

        private void rbClick(object sender, EventArgs e)
        {
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
        }
    }
}
