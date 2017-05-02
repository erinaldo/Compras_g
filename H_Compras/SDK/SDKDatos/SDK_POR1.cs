using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace H_Compras.SDK.SDKDatos
{
    public class SDK_POR1
    {
        private Int32 lineNum;

        public Int32 LineNum
        {
            get { return lineNum; }
            set { lineNum = value; }
        }

        string itemCode;

        public string ItemCode
        {
            get { return itemCode; }
            set { itemCode = value; }
        }
        string dscription;

        public string Dscription
        {
            get { return dscription; }
            set { dscription = value; }
        }
        decimal quantity;

        public decimal Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        decimal openQty;

        public decimal OpenQty
        {
            get { return openQty; }
            set { openQty = value; }
        }

        decimal price;

        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }
        string whscode;

        public string Whscode
        {
            get { return whscode; }
            set { whscode = value; }
        }
        string whsName;

        public string WhsName
        {
            get { return whsName; }
            set { whsName = value; }
        }
        decimal total;

        public decimal LineTotal
        {
            get { return total; }
            set { total = value; }
        }
        DateTime shipDate;

        public DateTime ShipDate
        {
            get { return shipDate; }
            set { shipDate = value; }
        }

        string lineStatus;

        public string LineStatus
        {
            get { return lineStatus; }
            set { lineStatus = value; }
        }

        string u_Vendedor;

        public string U_Vendedor
        {
            get { return u_Vendedor; }
            set { u_Vendedor = value; }
        }
        string u_Comentario;

        public string U_Comentario
        {
            get { return u_Comentario; }
            set { u_Comentario = value; }
        }

        decimal rate;

        public decimal Rate
        {
            get { return rate; }
            set { rate = value; }
        }

        string currency;

        public string Currency
        {
            get { return currency; }
            set { currency = value; }
        }

        decimal pesoU;

        public decimal PesoU
        {
            get { return pesoU; }
            set { pesoU = value; }
        }
        decimal volumenU;

        public decimal VolumenU
        {
            get { return volumenU; }
            set { volumenU = value; }
        }
    }
}
