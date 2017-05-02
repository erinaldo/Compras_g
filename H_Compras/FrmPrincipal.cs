using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras
{
    public partial class FrmPrincipal : Form
    {
        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Configuracion.frmHorizontes formulario = new Configuracion.frmHorizontes();
            formulario.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Inventarios.frmReubicaciones formulario = new Inventarios.frmReubicaciones();
            formulario.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Inventarios.frmTraspasos formulario = new Inventarios.frmTraspasos();
            formulario.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Compras.Pedido.frmCompras formulario = new Compras.Pedido.frmCompras();
            formulario.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SDK.Documentos.frmDocumentos formulario = new SDK.Documentos.frmDocumentos(1);
            formulario.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ReportesVarios.frmListaPrecios formulario = new ReportesVarios.frmListaPrecios();
            formulario.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Compras.frmConfiguracion formulario = new Compras.frmConfiguracion();
            formulario.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Inventarios.frmAnalisisObsLento formulario = new Inventarios.frmAnalisisObsLento();
            formulario.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ReportesVarios.frmOCAbiertas formulario = new ReportesVarios.frmOCAbiertas();
            formulario.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ReportesVarios.frmBackorder formulario = new ReportesVarios.frmBackorder();
            formulario.Show();

        }

        private void button11_Click(object sender, EventArgs e)
        {
            Compras.frmIndicadores formulario = new Compras.frmIndicadores();
            formulario.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ReportesVarios.frmIdealAlmacen formulario = new ReportesVarios.frmIdealAlmacen();
            formulario.Show();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            ReportesVarios.frmComprasVolumen formulartio = new ReportesVarios.frmComprasVolumen();
            formulartio.Show();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            //ReportesVarios.frmArribosVentas formulartio = new ReportesVarios.frmArribosVentas();
            ReportesVarios.frmArribos formulario = new ReportesVarios.frmArribos(0);
            formulario.Show();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            ReportesVarios.frmListasPrecios formulario = new ReportesVarios.frmListasPrecios();
            formulario.Show();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            SDK.Documentos.frmSalidas formulario = new SDK.Documentos.frmSalidas(2);
            formulario.Show();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            SDK.Documentos.frmTransferencia formulario = new SDK.Documentos.frmTransferencia(4);
            formulario.MdiParent = this.MdiParent;
            formulario.txtNumero.Text = 18483.ToString();
            var kea = new KeyPressEventArgs(Convert.ToChar(13));
            formulario.txtNumero_KeyPress(sender, kea);
            formulario.Show();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            SDK.Documentos.frmDocumentosPrevios formu = new SDK.Documentos.frmDocumentosPrevios();
            formu.Show();

        }
        
        private void button19_Click(object sender, EventArgs e)
        {
            //Inventarios.frmReparto1 fromulario = new Inventarios.frmReparto1("Laredo", 100, "laredo");
            //fromulario.Show();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            //Inventarios.frmReparto1 fromulario = new Inventarios.frmReparto1("Manzanillo", 1, "manzanillo");
            //fromulario.Show();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            ReportesVarios.frmIdealxLinea formulario = new ReportesVarios.frmIdealxLinea();
            formulario.Show();
        }
    }
}
