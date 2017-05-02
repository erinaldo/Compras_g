using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.ReportesVarios
{
    public partial class frmComentarios : Form
    {
        string ItemCode;
        Object[] valuesOut = new Object[] { };
        Datos.Connection connection = new Datos.Connection();

        public frmComentarios(string _ItemCode, string _ItemName)
        {
            InitializeComponent();
            ItemCode = _ItemCode;
            txtItemCode.Text = ItemCode;
            txtNombre.Text = _ItemName;
        }

        private void frmComentarios_Load(object sender, EventArgs e)
        {
            this.Icon = ClasesSGUV.Propiedades.IconHalcoNET;

            try
            {
                DataTable tbl =
                   connection.GetDataTable("LOG",
                                           "su_Almacen",
                                           new string[] { },
                                           new string[] { "@TipoConsulta", "@ItemCode"},
                                           ref valuesOut, 31, txtItemCode.Text);

                dataGridView1.DataSource = tbl;

                dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                DataTable tblR =
                          connection.GetDataTable("SGUV",
                                                  "PJ_IdealLinea",
                                                  new string[] { },
                                                  new string[] { "@TipoConsulta"},
                                                  ref valuesOut, 8);

                comboBox1.DataSource = tblR;
                comboBox1.DisplayMember = "Name";
                comboBox1.ValueMember = "Code";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtComment.Text))
                    return;

                DataTable tbl =
                   connection.GetDataTable("LOG",
                                           "su_Almacen",
                                           new string[] { },
                                           new string[] { "@TipoConsulta", "@ItemCode", "@Comentarios", "@Responsable" },
                                           ref valuesOut, 29, txtItemCode.Text, txtComment.Text, comboBox1.Text);
                txtComment.Clear();

                OnLoad(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "HalcoNET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                if(e.Row.Index > -1)

                    if (MessageBox.Show("¿Esta seguro de elimiar este registro?", "HalcoNET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        connection.Ejecutar("LOG",
                                            "su_Almacen",
                                            new string[] { },
                                            new string[] { "@TipoConsulta", "@DocEntry" },
                                            ref valuesOut, 30, e.Row.Cells["Id"].Value);
                    }
            }
            catch (Exception)
            {
                
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.Columns["ID"].Visible = false;
            dataGridView1.Columns["Comentario"].Width = 250;
        }
    }
}
