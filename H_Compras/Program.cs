using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace H_Compras
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary> 
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //ClasesSGUV.Login.Rol = 1;
            ClasesSGUV.Login.Id_Usuario = 268;

            ClasesSGUV.Login.Rol = 14;
            //Application.Run(new SDK.Varios.frmTransaccionesFR());
            Application.Run(new SDK.Varios.frmDMSocios()); 
            //Application.Run(new SDK.Documentos.frmCreditMemo(8));

            //Application.Run(new Transferencia.FrmEstacionalidadTransferencias());
            //Application.Run(new Inventarios.ConteoFisico.frmConteoFisico()); 
            //Application.Run(new Almacen.Entregas.frmAsignarPedido());
            //Application.Run(new Inventarios.frmDM_Articulos()); 

            //Application.Run(new ReportesVarios.frmIdealxLinea());
            //Application.Run(new SDK.Documentos.frmEM(8));
            //Application.Run(new SDK.Documentos.frmEM(10009)); 
        }
    }
}