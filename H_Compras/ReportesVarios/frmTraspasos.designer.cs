namespace H_Compras.ReportesVarios
{
    partial class frmTraspasos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            H_Compras.Constantes.Clases.Validaciones validaciones1 = new H_Compras.Constantes.Clases.Validaciones();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.txtNoTraspaso = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.gridTrapasos = new System.Windows.Forms.DataGridView();
            this.txtTotalPeso = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridTrapasos)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "No. Traspaso";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(222, 76);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 33);
            this.button1.TabIndex = 1;
            this.button1.Text = "Consultar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Fecha desde";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Fecha hasta";
            // 
            // dtpDesde
            // 
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(99, 57);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(87, 20);
            this.dtpDesde.TabIndex = 9;
            this.dtpDesde.Value = new System.DateTime(2016, 2, 23, 0, 0, 0, 0);
            // 
            // dtpHasta
            // 
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(99, 89);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(87, 20);
            this.dtpHasta.TabIndex = 10;
            this.dtpHasta.Value = new System.DateTime(2016, 2, 23, 0, 0, 0, 0);
            // 
            // txtNoTraspaso
            // 
            this.txtNoTraspaso.Location = new System.Drawing.Point(99, 31);
            this.txtNoTraspaso.Name = "txtNoTraspaso";
            this.txtNoTraspaso.Size = new System.Drawing.Size(87, 20);
            this.txtNoTraspaso.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(338, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Total traspaso";
            // 
            // txtTotal
            // 
            this.txtTotal.Location = new System.Drawing.Point(430, 86);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(100, 20);
            this.txtTotal.TabIndex = 13;
            // 
            // gridTrapasos
            // 
            this.gridTrapasos.AllowUserToAddRows = false;
            this.gridTrapasos.AllowUserToDeleteRows = false;
            this.gridTrapasos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridTrapasos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridTrapasos.Location = new System.Drawing.Point(15, 121);
            this.gridTrapasos.Name = "gridTrapasos";
            this.gridTrapasos.ReadOnly = true;
            this.gridTrapasos.RowHeadersVisible = false;
            this.gridTrapasos.Size = new System.Drawing.Size(1055, 381);
            this.gridTrapasos.TabIndex = 14;
            // 
            // txtTotalPeso
            // 
            this.txtTotalPeso.Location = new System.Drawing.Point(666, 86);
            this.txtTotalPeso.Name = "txtTotalPeso";
            this.txtTotalPeso.ReadOnly = true;
            this.txtTotalPeso.Size = new System.Drawing.Size(100, 20);
            this.txtTotalPeso.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(563, 89);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Peso total traspaso";
            // 
            // frmTraspasos
            // 
            this.AccessibleDescription = "(log)Rpt Traspasos";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1082, 536);
            this.Controls.Add(this.txtTotalPeso);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.gridTrapasos);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtNoTraspaso);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Dg = this.gridTrapasos;
            this.Name = "frmTraspasos";
            validaciones1.Mensaje = null;
            validaciones1.Result = false;
            this.ObjValidaciones = validaciones1;
            this.Text = "Traspasos";
            this.Load += new System.EventHandler(this.Traspasos_Load);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.dtpDesde, 0);
            this.Controls.SetChildIndex(this.dtpHasta, 0);
            this.Controls.SetChildIndex(this.txtNoTraspaso, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.txtTotal, 0);
            this.Controls.SetChildIndex(this.gridTrapasos, 0);
            this.Controls.SetChildIndex(this.label5, 0);
            this.Controls.SetChildIndex(this.txtTotalPeso, 0);
            ((System.ComponentModel.ISupportInitialize)(this.gridTrapasos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.TextBox txtNoTraspaso;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.DataGridView gridTrapasos;
        private System.Windows.Forms.TextBox txtTotalPeso;
        private System.Windows.Forms.Label label5;
    }
}