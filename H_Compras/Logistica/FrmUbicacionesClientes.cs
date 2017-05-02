using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;

namespace H_Compras.Logistica
{
    public partial class FrmUbicacionesClientes : Form
    {
        string CodNombre = string.Empty;
        string Nombre = string.Empty;
        DataTable dtCodigosRuta;
        DataTable dtCoordenadasRuta;
        public Obj_Captura oCaptura;
        bool bUpdateManual = false;
 
        public FrmUbicacionesClientes(string _CodNombre, string _NombreCl, DataTable _dtCodigos, DataTable _dtCoordenadas)
        {
            InitializeComponent();
            dtCodigosRuta = _dtCodigos;
            dtCoordenadasRuta = _dtCoordenadas;
            CodNombre = _CodNombre;
            Nombre = _NombreCl;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        

        private void FrmUbicacionesClientes_Load(object sender, EventArgs e)
        {
            try
            {
                txtCodCliente.Text = CodNombre;
                txtNombreCliente.Text = Nombre;
                dgvDatos.DataSource = dtCodigosRuta.Copy();
                dgvDatos1.DataSource = dtCoordenadasRuta.Copy();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                oCaptura = new Obj_Captura();
                int ElegidosRuta = 0;
                foreach (var item in dgvDatos.Rows)
                {
                    if (Convert.ToBoolean(item.Cells["Elegido"].Value) == true)
                    {
                        oCaptura.CodigoRuta = item.Cells["CodRuta"].Value.ToString();
                        ElegidosRuta = ElegidosRuta + 1;
                    }                    
                }

                int ElegidosCoor = 0;
                foreach (var item in dgvDatos1.Rows)
                {
                    if (Convert.ToBoolean(item.Cells["Elegido"].Value) == true)
                    {
                        oCaptura.iClienteReparto = Convert.ToInt32(item.Cells["idClienteReparto"].Value);
                        oCaptura.Latitud = Convert.ToDecimal(item.Cells["Latitud"].Value);
                        oCaptura.Longitud = Convert.ToDecimal(item.Cells["Longitud"].Value);
                        oCaptura.DescripcionEntrega = Convert.ToString(item.Cells["sDescripcion"].Value);
                        ElegidosCoor = ElegidosCoor + 1;
                    }                      
                }

                if (ElegidosRuta == 0)
                {
                    MessageBox.Show("Debe especificar al menos un codigo de ruta");
                    return;
                }
                else if (ElegidosCoor == 0)
                {
                    MessageBox.Show("Debe especificar al menos una coordenada");
                    return;
                }
                else
                {
                    DialogResult = System.Windows.Forms.DialogResult.Yes;
                }
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["CardCode"].Hidden = true;
            e.Layout.Bands[0].Columns["CodRuta"].Header.Caption = "Codigo de Ruta";
            e.Layout.Bands[0].Columns["Sucursal"].Hidden = true;
            e.Layout.Bands[0].Columns["Elegido"].Header.Caption = "Elegido";
        }
        private void dgvDatos1_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["idClienteReparto"].Hidden = true;
            e.Layout.Bands[0].Columns["CodigoCliente"].Hidden = true;
            e.Layout.Bands[0].Columns["NombreCliente"].Header.Caption = "Cliente";
            e.Layout.Bands[0].Columns["sDescripcion"].Header.Caption = "Ubicacion Entrega";
            e.Layout.Bands[0].Columns["Latitud"].Hidden = true;
            e.Layout.Bands[0].Columns["Longitud"].Hidden = true;
            e.Layout.Bands[0].Columns["LtLn"].Header.Caption = "Coordenadas";
            e.Layout.Bands[0].Columns["Elegido"].Header.Caption = "Elegido";
            e.Layout.Bands[0].Columns["FechaRegistro"].Hidden = true;
            e.Layout.Bands[0].Columns["sCodRuta"].Hidden = true;
        }

        private void dgvDatos_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            try
            {
                //if (!bUpdateManual)
                //{
                //    for (int i = 0; i < length; i++)
                //    {
                        
                //    }

                //    int Fila = e.Cell.Row.Index;
                //    e.Cancel = true;
                //    bUpdateManual = true;
                //    dgvDatos.DataSource = null;

                //    dgvDatos.DataSource = dtCodigosRuta.Copy();
                //    dgvDatos.Rows[Fila].Cells["Elegido"].Value = true;
                //    bUpdateManual = false;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
           
        }

        private void dgvDatos_ClickCell(object sender, ClickCellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Key == "Elegido")
                {
                    int fila = e.Cell.Row.Index;

                    for (int i = 0; i < dgvDatos.Rows.Count; i++)
                    {
                        dgvDatos.Rows[i].Cells["Elegido"].Value = false;
                    }
                    dgvDatos.Rows[fila].Cells["Elegido"].Value = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void dgvDatos1_ClickCell(object sender, ClickCellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Key == "Elegido")
                {
                    int fila = e.Cell.Row.Index;

                    for (int i = 0; i < dgvDatos1.Rows.Count; i++)
                    {
                        dgvDatos1.Rows[i].Cells["Elegido"].Value = false;
                    }
                    dgvDatos1.Rows[fila].Cells["Elegido"].Value = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        
        }

        //private void btnAgregarCodRut_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DataTable dtCodRu = (DataTable)dgvDatos.DataSource;
        //        Compras.Logistica.FrmAltasCodigosRutaCliente oForm = new FrmAltasCodigosRutaCliente(CodNombre, Nombre,ref dtCodRu);
        //        oForm.ShowDialog();
        //        dgvDatos.DataSource = null;
        //        dgvDatos.DataSource = oForm.dtCodicosRutaCliente;
        //        oForm.Close();
        //        oForm.Dispose();
                

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message.ToString());
        //    }
        //}

        //private void btnAgregarCoord_Click(object sender, EventArgs e)
        //{
        //    DataTable dtCoord = (DataTable)dgvDatos1.DataSource;
        //    Compras.Logistica.FrmAltaCoordenadas oForm = new FrmAltaCoordenadas(CodNombre, Nombre, ref dtCoord);
        //    oForm.ShowDialog();
        //    dgvDatos1.DataSource = null;
        //    dgvDatos1.DataSource = oForm.dtCoordenadasCliente;
        //    oForm.Close();
        //    oForm.Dispose();
            
        //}

        private void dgvDatos_InitializeLayout_1(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["CardCode"].Hidden = true;
            e.Layout.Bands[0].Columns["CodRuta"].Header.Caption = "Codigo de Ruta"; 
            e.Layout.Bands[0].Columns["nID"].Hidden = true;
            e.Layout.Bands[0].Columns["nFrecuencia"].Header.Caption = "No. Visitas Aut.";
            e.Layout.Bands[0].Columns["bSemanal"].Hidden = true;
            e.Layout.Bands[0].Columns["bQuincenal"].Hidden = true;
            e.Layout.Bands[0].Columns["sFrecuencia"].Header.Caption = "Frecuencia";
            e.Layout.Bands[0].Columns["Sucursal"].Hidden = true;
            foreach (var item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Activation.NoEdit;
            }
            e.Layout.Bands[0].Columns["Elegido"].CellActivation = Activation.AllowEdit;
        }
    }

    public class Obj_Captura
    {
        public string CodigoRuta { get; set; }
        public int iClienteReparto { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public string DescripcionEntrega { get; set; }
 
    }
}
