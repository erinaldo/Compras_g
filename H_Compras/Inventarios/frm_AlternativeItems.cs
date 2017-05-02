using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.Inventarios
{
    public partial class frm_AlternativeItems : Constantes.frmEmpty
    {
        Datos.Connection connection = new Datos.Connection();
        Object[] valuesOut = new Object[] { };

        public frm_AlternativeItems()
        {
            InitializeComponent();
        }

        private void frm_AlternativeItems_Load(object sender, EventArgs e)
        {
            Log = new ClasesSGUV.Logs(ClasesSGUV.Login.NombreUsuario, this.AccessibleDescription, 0);

            actualizarToolStripButton.Click -= new EventHandler(btnActualizar_Click);
            actualizarToolStripButton.Click += new EventHandler(btnActualizar_Click);
        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialof = new OpenFileDialog();
            System.IO.Stream myStream = null;
            if(dialof.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    if ((myStream = dialof.OpenFile()) != null)
                    {
                        using (System.IO.StreamReader cargar = new System.IO.StreamReader(dialof.FileName))
                        {
                            string renglon = string.Empty;
                            int index = 0;
                            while (!string.IsNullOrEmpty(renglon = cargar.ReadLine()))
                            {
                                if (index > 0)
                                {
                                    string[] data = renglon.Split('\t');

                                    Datos.Connection conn = new Datos.Connection();

                                    int x = conn.Ejecutar("LOG",
                                                          "su_Almacen",
                                                          new string[] { },
                                                          new string[] { "@TipoConsulta", "@ItemCode", "@AlternativeItem" },
                                                          ref valuesOut, 25, data[0], data[2]);

                                    //System.Threading.Thread.Sleep(60);
                                }
                                index++;
                            }

                        }
                        DataTable tbl = connection.GetDataTable("LOG",
                                                                "su_Almacen",
                                                                new string[] { },
                                                                new string[] { "@TipoConsulta"},
                                                                ref valuesOut, 26);
                        dgvDatos.DataSource = tbl;
                    }
                }
                catch (Exception ex)
                {
                    this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
                }
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            dgvDatos.DataSource = null;

            DataTable tbl = connection.GetDataTable("LOG",
                                                               "su_Almacen",
                                                               new string[] { },
                                                               new string[] { "@TipoConsulta" },
                                                               ref valuesOut, 26);
            dgvDatos.DataSource = tbl;
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();            
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (var item in dgvDatos.DisplayLayout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
        }

        private void dgvDatos_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            try
            {
                if(e.Row.Cells["frozenFor"].Value.ToString().Equals("Y"))
                {
                    e.Row.Cells["AlternativeItem"].Appearance.BackColor = Color.Red;
                    e.Row.Cells["AlternativeItem"].Appearance.ForeColor = Color.White;
                }
            }
            catch (Exception)
            {
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Esta operacion puede tardar varios minutos, ¿Desea Continuar?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    return;

                //activar
                connection.Ejecutar("LOG",
                                    "su_Almacen",
                                    new string[] { },
                                    new string[] { "@TipoConsulta", "@frozenFor" },
                                    ref valuesOut, 27, "N");

                DataTable tbl_Datos = dgvDatos.DataSource as DataTable;

                var lista_Items = (from item in tbl_Datos.AsEnumerable()
                                   select item.Field<string>("ItemCode")).Distinct();

                SDK_SAP.DI.Connection.InitializeConnection(6);

                int Cont = 0;
                foreach (var item in lista_Items)
                {
                    Cont++;

                    this.SetMensaje("Procesando: " + Cont + " de " + lista_Items.Count() + " | " + item, 3000);

                    SDK_SAP.DI.AlternativeItems oItem = new SDK_SAP.DI.AlternativeItems();
                    try
                    {
                        oItem.DeleteAlternativeItems(item);
                    }
                    catch (Exception)
                    {
                    }

                    var qryAlt = (from row in tbl_Datos.AsEnumerable()
                                  where row.Field<string>("ItemCode").Equals(item)
                                  select new
                                  {
                                      AlternativeItem = row.Field<string>("AlternativeItem")
                                  });

                    DataTable tblAlt = ClasesSGUV.ListConverter.ToDataTable(qryAlt);

                    oItem.AddAlternativeItems(item, tblAlt);
                }

                System.Threading.Thread.Sleep(3000);

                MessageBox.Show("Listo", "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);

                DataTable tbl = connection.GetDataTable("LOG",
                                                                "su_Almacen",
                                                                new string[] { },
                                                                new string[] { "@TipoConsulta" },
                                                                ref valuesOut, 26);
                dgvDatos.DataSource = tbl;
            }
            finally
            {
                connection.Ejecutar("LOG",
                                      "su_Almacen",
                                      new string[] { },
                                      new string[] { "@TipoConsulta", "@frozenFor" },
                                      ref valuesOut, 28, "Y");

                //btnActualizar_Click(sender, e);
            }
        }
    }
}
