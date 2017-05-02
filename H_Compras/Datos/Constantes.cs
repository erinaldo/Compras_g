using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace H_Compras.Datos.Clases
{
    public static class Constantes
    {
        public static string conectionLog
        {
            get { return @"Data Source=192.168.2.100;Initial Catalog=PJ-Log;user id = sa; password = SAP-PJ1; TrustServerCertificate = true"; }
            //get { return @"Data Source=192.168.2.204;Initial Catalog=PJ-Log;user id = sa; password = SAP-PJ1"; }
            //get { return @"Data Source=LUISOLIVOS-PJ;Initial Catalog=PJ-Log;user id = sa; password = SAP-PJ1"; }
        }

        public static string conectionLogPrueba
        {
            get { return @"Data Source=LUISOLIVOS-PJ;Initial Catalog=PJ-Log;user id = sa; password = SAP-PJ1"; }
        }

        static int userID;

        public static int UserID
        {
            get { return Constantes.userID; }
            set { Constantes.userID = value; }
        }
    }
}
