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
    public partial class frmIdealLinea : Constantes.frmEmpty
    {
        public frmIdealLinea()
        {
            InitializeComponent();
        }

        private void frmIdealLinea_Load(object sender, EventArgs e)
        {
            try
            {
                ClasesSGUV.Form.ControlsForms.setDataSource(clbLineas, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Lineas, null, string.Empty), "ItmsGrpNam", "ItmsGrpCod", "---Selecciona una línea---");
                ClasesSGUV.Form.ControlsForms.setDataSource(clbProveedor, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Proveedores, null, string.Empty), "CardName", "CardCode", "---Selecciona un proveedor---");
                ClasesSGUV.Form.ControlsForms.setDataSource(clbComprador, ClasesSGUV.DataSource.GetSource((int)ClasesSGUV.DataSource.TipoQry.Compradores, null, string.Empty), "U_comprador", "U_comprador", "---Selecciona un comprador---");
            }
            catch (Exception)
            {
                
            }
        }
    }
}
