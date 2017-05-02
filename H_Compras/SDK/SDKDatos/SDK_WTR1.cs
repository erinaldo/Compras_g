using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace H_Compras.SDK.SDKDatos
{
    public class SDK_WTR1
    {
        private string itemCode;

        private int lineNum;
        public int LineNum
        {
            get { return lineNum; }
            set { lineNum = value; }
        }
        public string ItemCode
        {
            get { return itemCode; }
            set { itemCode = value; }
        }
        private string itemName;
        public string Dscription
        {
            get { return itemName; }
            set { itemName = value; }
        }
        private string whsCode;
        public string WhsCode
        {
            get { return whsCode; }
            set { whsCode = value; }
        }
        private string whsName;
        public string WhsName
        {
            get { return whsName; }
            set { whsName = value; }
        }
        private string fromWhsCode;
        public string FromWhsCode
        {
            get { return fromWhsCode; }
            set { fromWhsCode = value; }
        }
        private string fromWhsName;
        public string FromWhsName
        {
            get { return fromWhsName; }
            set { fromWhsName = value; }
        }
        private decimal quantity;
        public decimal Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        private decimal bWeight1;
        public decimal BWeight1
        {
            get { return bWeight1; }
            set { bWeight1 = value; }
        }
        private decimal bVolume;
        public decimal BVolume
        {
            get { return bVolume; }
            set { bVolume = value; }
        }
        private string manBtchNum;
        public string ManBtchNum
        {
            get { return manBtchNum; }
            set { manBtchNum = value; }
        }
        private string u_Tarima;
        public string U_Tarima
        {
            get { return u_Tarima; }
            set { u_Tarima = value; }
        }
        private string u_TipoAlmName;
        public string U_TipoAlmName
        {
            get { return u_TipoAlmName; }
            set { u_TipoAlmName = value; }
        }
        private string u_TipoAlm;
        public string U_TipoAlm
        {
            get { return u_TipoAlm; }
            set { u_TipoAlm = value; }
        }
        private string lineStatus;
        public string LineStatus
        {
            get { return lineStatus; }
            set { lineStatus = value; }
        }

        private int baseLine;

        public int BaseLine
        {
            get { return baseLine; }
            set { baseLine = value; }
        }
        private int baseEntry;

        public int BaseEntry
        {
            get { return baseEntry; }
            set { baseEntry = value; }
        }

        private int baseType;

        public int BaseType
        {
            get { return baseType; }
            set { baseType = value; }
        }
    }
}
