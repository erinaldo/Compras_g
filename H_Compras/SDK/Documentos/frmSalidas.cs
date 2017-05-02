using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace H_Compras.SDK.Documentos
{
    public partial class frmSalidas : Constantes.frmEmpty
    {
        int IdDocument;
        Decimal IVA;

        SDK.Configuracion.SDK_Configuracion_Salidas config;
      //  SDK.SDKDatos.SDK_OPOR Document;

        public frmSalidas()
        {
            InitializeComponent();
        }

        public frmSalidas(int _idDocument)
        {
            InitializeComponent();
            dgvDatos.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
            //  I  N  I  C  I  A  L  I  Z  A  R
            //dgvDatos.DataSource = _details;
            IdDocument = _idDocument;
            config = new Configuracion.SDK_Configuracion_Salidas(IdDocument, this);
            config.ModeDocument = "Edit";
            config.StartEmpty();
            IVA = config.IVA1;
            this.AccessibleDescription = "SDK " + this.Text;
            this.btnCrear.Text = "Actualizar";

            //this.CleanScreen();

            //txtCardCode.ReadOnly = false;
            //txtCardCode.Enabled = true;

            //txtCardName.ReadOnly = false;
            //txtCardName.Enabled = true;
            txtNumero.BackColor = Color.FromName("Info");
            
            cerrarStripButton.Enabled = true;
            nuevoToolStripButton.Enabled = true;
            //cerrarLíneaToolStripMenuItem.Enabled = true;

            //tooStripDuplicar.Enabled = false;
            //toolStripEliminar.Enabled = false;
        }

        private void CleanScren()
        {
            txtNumero.Clear();
            txtSerie.Clear();
            dtpDocDate.Value = DateTime.Now;
            txtComentarios.Clear();
        }

        private void frmSalidas_Load(object sender, EventArgs e)
        {
            Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

            nuevoToolStripButton.Click -= new EventHandler(btnNuevo_Click);
            nuevoToolStripButton.Click += new EventHandler(btnNuevo_Click);

            //if (!this.Validar())
            //{
            //    btnCrear.Enabled = false;
            //    btnCancelar.Enabled = false;
            //    return;
            //}

            //eliminar
            btnNuevo_Click(sender, e);
        }

        #region EVENTOS MENU
        public void btnNuevo_Click(object sender, EventArgs e)
        {
            this.CleanScren();

            txtNumero.BackColor = txtComentarios.BackColor;

            btnCrear.Text = "Crear";
            config.ModeDocument = "New";

            //txtNumero.KeyPress -= new KeyPressEventHandler(txtFolio_KeyPress);

            this.dgvDatos.DataSource = (dgvDatos.DataSource as DataTable).Copy();

            //agregarFilaToolStripMenuItem1.Enabled = true;
            //toolStripEliminar.Enabled = true;
            //tooStripDuplicar.Enabled = true;
        }
        #endregion

        private void dgvDatos_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            try
            {
                //if (e.Row.Cells["LineStatus"].Value.ToString() == "C")
                //{
                //    e.Row.Appearance.BackColor = Color.LightGray;
                //}

                if (config.ModeDocument != "Edit")
                {
                    if (e.Row.Cells["Currency"].Value.ToString() == txtMoneda.Text)
                        e.Row.Cells["Rate"].Value = 1;
                    else
                        e.Row.Cells["Rate"].Value = config.Rate;

                    e.Row.Cells["AcctCode"].Value = "5200-100-008";
                    e.Row.Cells["Quantity"].Value = 1;
                }

              

                /*29-04-16: CORRECCION DE BUG, YA SE PERMITEN CREAR DOCUMENTOS DESDE CERO*/
                //(dgvDatos.DataSource as DataTable).AcceptChanges();
            }
            catch (Exception)
            {
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (btnCrear.Text == "Crear")
            {
                if (string.IsNullOrEmpty(txtMoneda.Text))
                {
                    this.SetMensaje("Campo MONEDA obligatorio", 5000, Color.Red, Color.White);
                    return;
                }
                #region Crear
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    if (IdDocument == 2)
                    {
                        string mensaje = string.Empty;

                        SDK.SDKDatos.SDK_DI sdk = new SDKDatos.SDK_DI(2);//--Orden de compra
                        sdk.CrearSalidaDeMercancias(this).ToString();
                        this.SetMensaje("Listo", 5000, Color.Green, Color.Black);
                    }
                }
                catch (Exception ex)
                {
                    this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
                }
                finally
                {
                    Cursor.Current = Cursors.Arrow;
                }
                #endregion
            }
        }

        private void frmSalidas_Shown(object sender, EventArgs e)
        {
            if (!ClasesSGUV.Forms.GetPermisoReporte(this.Name))
            {
                MessageBox.Show("Usuario no autorizado. \r\nContacta al administrador.", "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }
        }
    }
}
