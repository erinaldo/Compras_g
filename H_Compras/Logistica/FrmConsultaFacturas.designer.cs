namespace H_Compras.Logistica
{
    partial class FrmConsultaFacturas
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpInicio = new System.Windows.Forms.DateTimePicker();
            this.dtpFin = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConsultarFactura = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRechazar = new System.Windows.Forms.Button();
            this.btnEnviarCorreo = new System.Windows.Forms.Button();
            this.btnAutorizar = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ugvDatos = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraCalcManager1 = new Infragistics.Win.UltraWinCalcManager.UltraCalcManager(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugvDatos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraCalcManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Desde:";
            // 
            // dtpInicio
            // 
            this.dtpInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpInicio.Location = new System.Drawing.Point(63, 20);
            this.dtpInicio.Name = "dtpInicio";
            this.dtpInicio.Size = new System.Drawing.Size(103, 22);
            this.dtpInicio.TabIndex = 1;
            // 
            // dtpFin
            // 
            this.dtpFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFin.Location = new System.Drawing.Point(63, 46);
            this.dtpFin.Name = "dtpFin";
            this.dtpFin.Size = new System.Drawing.Size(103, 22);
            this.dtpFin.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Hasta:";
            // 
            // btnConsultarFactura
            // 
            this.btnConsultarFactura.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnConsultarFactura.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnConsultarFactura.Location = new System.Drawing.Point(38, 75);
            this.btnConsultarFactura.Name = "btnConsultarFactura";
            this.btnConsultarFactura.Size = new System.Drawing.Size(128, 23);
            this.btnConsultarFactura.TabIndex = 7;
            this.btnConsultarFactura.Text = "Consultar";
            this.btnConsultarFactura.UseVisualStyleBackColor = false;
            this.btnConsultarFactura.Click += new System.EventHandler(this.btnConsultarFactura_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRechazar);
            this.groupBox1.Controls.Add(this.btnConsultarFactura);
            this.groupBox1.Controls.Add(this.dtpFin);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpInicio);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnEnviarCorreo);
            this.groupBox1.Controls.Add(this.btnAutorizar);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(175, 262);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rango de Consulta";
            // 
            // btnRechazar
            // 
            this.btnRechazar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRechazar.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnRechazar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRechazar.Location = new System.Drawing.Point(38, 133);
            this.btnRechazar.Name = "btnRechazar";
            this.btnRechazar.Size = new System.Drawing.Size(128, 23);
            this.btnRechazar.TabIndex = 47;
            this.btnRechazar.Text = "Rechazar";
            this.btnRechazar.UseVisualStyleBackColor = false;
            this.btnRechazar.Visible = false;
            this.btnRechazar.Click += new System.EventHandler(this.btnRechazar_Click);
            // 
            // btnEnviarCorreo
            // 
            this.btnEnviarCorreo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEnviarCorreo.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnEnviarCorreo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEnviarCorreo.Location = new System.Drawing.Point(38, 167);
            this.btnEnviarCorreo.Name = "btnEnviarCorreo";
            this.btnEnviarCorreo.Size = new System.Drawing.Size(128, 23);
            this.btnEnviarCorreo.TabIndex = 8;
            this.btnEnviarCorreo.Text = "Enviar Notificación";
            this.btnEnviarCorreo.UseVisualStyleBackColor = false;
            this.btnEnviarCorreo.Visible = false;
            this.btnEnviarCorreo.Click += new System.EventHandler(this.btnEnviarCorreo_Click);
            // 
            // btnAutorizar
            // 
            this.btnAutorizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAutorizar.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnAutorizar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAutorizar.Location = new System.Drawing.Point(38, 104);
            this.btnAutorizar.Name = "btnAutorizar";
            this.btnAutorizar.Size = new System.Drawing.Size(128, 23);
            this.btnAutorizar.TabIndex = 46;
            this.btnAutorizar.Text = "Autorizar";
            this.btnAutorizar.UseVisualStyleBackColor = false;
            this.btnAutorizar.Visible = false;
            this.btnAutorizar.Click += new System.EventHandler(this.btnAutorizar_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.ugvDatos);
            this.groupBox2.Location = new System.Drawing.Point(184, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(666, 495);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            // 
            // ugvDatos
            // 
            this.ugvDatos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ugvDatos.CalcManager = this.ultraCalcManager1;
            appearance1.BackColor = System.Drawing.Color.White;
            appearance1.BorderAlpha = Infragistics.Win.Alpha.Opaque;
            appearance1.BorderColor = System.Drawing.SystemColors.ActiveCaption;
            this.ugvDatos.DisplayLayout.Appearance = appearance1;
            this.ugvDatos.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.ugvDatos.DisplayLayout.Override.FilterOperandStyle = Infragistics.Win.UltraWinGrid.FilterOperandStyle.Combo;
            this.ugvDatos.DisplayLayout.Override.FilterOperatorDefaultValue = Infragistics.Win.UltraWinGrid.FilterOperatorDefaultValue.Contains;
            this.ugvDatos.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.HeaderIcons;
            appearance2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ugvDatos.DisplayLayout.Override.RowSelectorAppearance = appearance2;
            this.ugvDatos.DisplayLayout.Override.WrapHeaderText = Infragistics.Win.DefaultableBoolean.True;
            this.ugvDatos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ugvDatos.Location = new System.Drawing.Point(17, 19);
            this.ugvDatos.Name = "ugvDatos";
            this.ugvDatos.Size = new System.Drawing.Size(643, 439);
            this.ugvDatos.TabIndex = 45;
            this.ugvDatos.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ugvDatos_AfterCellUpdate);
            this.ugvDatos.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ugvDatos_InitializeLayout);
            this.ugvDatos.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.ugvDatos_InitializeRow);
            this.ugvDatos.ClickCellButton += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ugvDatos_ClickCellButton);
            this.ugvDatos.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ugvDatos_BeforeCellUpdate);
            this.ugvDatos.ClickCell += new Infragistics.Win.UltraWinGrid.ClickCellEventHandler(this.ugvDatos_ClickCell);
            this.ugvDatos.DoubleClickCell += new Infragistics.Win.UltraWinGrid.DoubleClickCellEventHandler(this.ugvDatos_DoubleClickCell);
            this.ugvDatos.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ugvDatos_KeyDown);
            this.ugvDatos.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ugvDatos_KeyPress);
            // 
            // ultraCalcManager1
            // 
            this.ultraCalcManager1.ContainingControl = this;
            // 
            // FrmConsultaFacturas
            // 
            this.AccessibleDescription = "Aut. de solicitud logistica";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 510);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmConsultaFacturas";
            this.Text = "Consulta de Solicitudes - Logistica";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmConsultaFacturas_FormClosing);
            this.Load += new System.EventHandler(this.FrmConsultaFacturas_Load);
            this.Shown += new System.EventHandler(this.FrmConsultaFacturas_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugvDatos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraCalcManager1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpInicio;
        private System.Windows.Forms.DateTimePicker dtpFin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConsultarFactura;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnEnviarCorreo;
        private Infragistics.Win.UltraWinCalcManager.UltraCalcManager ultraCalcManager1;
        private Infragistics.Win.UltraWinGrid.UltraGrid ugvDatos;
        private System.Windows.Forms.Button btnAutorizar;
        private System.Windows.Forms.Button btnRechazar;
    }
}