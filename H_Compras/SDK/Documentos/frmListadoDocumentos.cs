using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace H_Compras.SDK.Documentos
{
    public partial class frmListadoDocumentos : Constantes.frmEmpty
    {
        int IdDocumento; 
        string CardCode;
        string CardName;
        string NumAtCard;
        DateTime DocDate;
        int Series;
        int docEntry;
        List<string> docKey = new List<string>();
        List<DocKeys> seleccionados = new List<DocKeys>();

        public List<DocKeys> Seleccionados
        {
            get { return seleccionados; }
            set { seleccionados = value; }
        }

        public string ItemCode
        {
            get;
            set;
        }
        public string Tipo { get; set; }

        public List<string> DocKey
        {
            get { return docKey; }
            set { docKey = value; }
        }

        DataRow row;
        DataTable source;

        public DataRow Row
        {
            get { return row; }
            set { row = value; }
        }

        
        private string nameProc = "sp_SDKDocuments";

        public int DocEntry
        {
            get { return docEntry; }
            set { docEntry = value; }
        }

        public frmListadoDocumentos(int idDocumento, string cardCode, string cardName, 
                                            DateTime docDate, string numAtCard, int series = 0)
        {
            InitializeComponent();

            source = new DataTable();
            IdDocumento = idDocumento;
            CardCode = cardCode;
            DocDate = docDate;
            CardName = cardName;
            NumAtCard = numAtCard;
            Series = series;
        }

        private void frmListadoDocumentos_Load(object sender, EventArgs e)
        {
            try
            {
                nuevoToolStripButton.Enabled = false;
                guardarToolStripButton.Enabled = false;
                ayudaToolStripButton.Enabled = false;

                using (SqlConnection cnnection = new SqlConnection(Datos.Clases.Constantes.conectionLog))
                {
                    SqlCommand cmd = new SqlCommand(nameProc, cnnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoConsulta", 4);
                    cmd.Parameters.AddWithValue("@idDocumento", IdDocumento);
                    cmd.Parameters.AddWithValue("@CardCode", CardCode);
                    cmd.Parameters.AddWithValue("@CardName", CardName);
                    cmd.Parameters.AddWithValue("@NumAtCard", NumAtCard);
                    cmd.Parameters.AddWithValue("@DocDate", DocDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Series", Series);

                    DataTable table = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                 
                    da.Fill(source);
                    dgvDatos.DataSource = source;

                    if (IdDocumento == -2 | IdDocumento == -6 | IdDocumento == -8) 
                        btnSeleccionar.Visible = true;
                }
            }
            catch (Exception ex)
            {
                this.SetMensaje(ex.Message, 5000, Color.Red, Color.White);
            }
        
        }

        private void dgvDatos_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn item in e.Layout.Bands[0].Columns)
            {
                item.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
            if (IdDocumento == 1 || IdDocumento == 10008 || IdDocumento == 10009 || IdDocumento == 8 || IdDocumento == 9)
            {
                e.Layout.Bands[0].Columns["DocEntry"].Hidden = true;

                e.Layout.Bands[0].Columns["Nombre"].Width = 300;
                e.Layout.Bands[0].Columns["Total"].Format = "N2";
                e.Layout.Bands[0].Columns["Total"].CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Right;
            }
            else if (IdDocumento == -1 | IdDocumento == -5)
            {
                e.Layout.Bands[0].Columns["CardCode"].Header.Caption = "S/N";
                e.Layout.Bands[0].Columns["CardName"].Header.Caption = "Nombre";
                e.Layout.Bands[0].Columns["Currency"].Header.Caption = "Moneda";
            }
            else if (IdDocumento == -7)
            {
                e.Layout.Bands[0].Columns["CardCode"].Header.Caption = "S/N";
                e.Layout.Bands[0].Columns["CardName"].Header.Caption = "Nombre SN";
                e.Layout.Bands[0].Columns["Balance"].Header.Caption = "Saldo en cuenta";
                e.Layout.Bands[0].Columns["SlpName"].Header.Caption = "Vendedor";

                e.Layout.Bands[0].Columns["Balance"].Format = "N2";
                e.Layout.Bands[0].Columns["Balance"].CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Right;
            }
            else if (IdDocumento == -2 | IdDocumento == -3 )
            {
                e.Layout.Bands[0].Columns["DocEntry"].Hidden = true;
                e.Layout.Bands[0].Columns["DocNum"].Header.Caption = "Num Documento";
                e.Layout.Bands[0].Columns["DocDate"].Header.Caption = "Fecha de contabilización";
                e.Layout.Bands[0].Columns["CardName"].Header.Caption = "Proveedor";
                e.Layout.Bands[0].Columns["Comments"].Header.Caption = "Comentarios";
            }
            else if(IdDocumento == -6| IdDocumento == -8)
            {
                e.Layout.Bands[0].Columns["DocEntry"].Hidden = true;
                e.Layout.Bands[0].Columns["DocNum"].Header.Caption = "Num Documento";
                e.Layout.Bands[0].Columns["DocDate"].Header.Caption = "Fecha de contabilización";
                e.Layout.Bands[0].Columns["CardName"].Header.Caption = "Proveedor";
                e.Layout.Bands[0].Columns["Comments"].Header.Caption = "Comentarios";
                e.Layout.Bands[0].Columns["PaidToDate"].Header.Caption = "Importe aplicado";
                e.Layout.Bands[0].Columns["PaidToDate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                e.Layout.Bands[0].Columns["PaidToDate"].Format = "N2";
            }
        }

        private void dgvDatos_DoubleClickCell(object sender, Infragistics.Win.UltraWinGrid.DoubleClickCellEventArgs e)
        {
            try
            {
                if (IdDocumento == 1 || IdDocumento == 10008 || IdDocumento == 10009)
                {
                    if (IdDocumento == 1)
                        Tipo = Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["Tipo"].Value);

                    DocEntry = Convert.ToInt32(dgvDatos.Rows[e.Cell.Row.Index].Cells["Número de documento"].Value);

                    this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                }
                   if(  IdDocumento == 8 || IdDocumento == 9)
                {
                    DocEntry = Convert.ToInt32(dgvDatos.Rows[e.Cell.Row.Index].Cells["Número de documento"].Value);
                    Tipo = Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["Doc"].Value);

                    this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                }
                else if (IdDocumento == -1 | IdDocumento == -5 | IdDocumento == -7)
                {
                    string var = string.Empty;
                    var = Convert.ToString(dgvDatos.Rows[e.Cell.Row.Index].Cells["CardCode"].Value);

                    DocKey.Add(var);

                    Row = (from item in source.AsEnumerable()
                           where item.Field<string>("CardCode").Equals(var)
                           select item).FirstOrDefault();

                    this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                }
                else if (IdDocumento == -2 | IdDocumento == -3 | IdDocumento == -6 |IdDocumento == -8)
                {
                    string var = string.Empty;

                   // dgvDatos.Selected.

                    //foreach (var item in dgvDatos.Selected.Rows)
                    //{
                        var = Convert.ToString(dgvDatos.ActiveRow.Cells["DocEntry"].Value);
                        DocKey.Add(var);
                    //}

                    this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                }
                else if (IdDocumento == -4)
                {
                    ItemCode = Convert.ToString(dgvDatos.ActiveRow.Cells[0].Value);

                    this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                }
            }
            catch (Exception)
            {
                
            }
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
           if( IdDocumento == -3)
            {
                string var = string.Empty;

                foreach (var item in dgvDatos.Selected.Rows)
                {
                    var = Convert.ToString(item.Cells["DocEntry"].Value);
                    DocKey.Add(var);
                }

                this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            }
           else if (IdDocumento == -2 | IdDocumento == -6 | IdDocumento == -8)
           {
               foreach (var item in dgvDatos.Selected.Rows)
               {
                   DocKeys ojb = new DocKeys();

                   ojb.DocEntry = Convert.ToString(item.Cells["DocEntry"].Value);
                   ojb.DocType = Convert.ToString(item.Cells["ObjType"].Value);
                   seleccionados.Add(ojb);
               }

               this.DialogResult = System.Windows.Forms.DialogResult.Yes;
           }
        }
    }

    public class DocKeys
    {
        public string DocEntry;
        public string DocType;
    }
}
