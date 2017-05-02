using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.Almacen.Entregas
{
    public partial class frmMensaje : Constantes.frmEmpty
    {
        public string Folios { get; set;}

        public frmMensaje()
        {
            InitializeComponent();
        }

        private void frmMensaje_Load(object sender, EventArgs e)
        {
            this.toolStrip1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Folios = txtFacturas.Text.Trim();

            //if(string.IsNullOrEmpty(Folios))
            //{
            //    this.SetMensaje("Ingrese en numero(s) de factura", 5000, Color.Red, Color.White);
            //    return;
            //}
            DialogResult = System.Windows.Forms.DialogResult.Yes;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.No;
        }
    }
}
