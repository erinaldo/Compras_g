﻿namespace H_Compras.SDK.Documentos
{
    partial class frmDocumentos
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
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            H_Compras.Constantes.Clases.Validaciones validaciones1 = new H_Compras.Constantes.Clases.Validaciones();
            this.dgvDatos = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.agregarFilaToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tooStripDuplicar = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripEliminar = new System.Windows.Forms.ToolStripMenuItem();
            this.cerrarLíneaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtCardCode = new System.Windows.Forms.TextBox();
            this.txtCardName = new System.Windows.Forms.TextBox();
            this.txtNumAtCard = new System.Windows.Forms.TextBox();
            this.dtpDocDate = new System.Windows.Forms.DateTimePicker();
            this.dtpDocDueDate = new System.Windows.Forms.DateTimePicker();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.txtComments = new System.Windows.Forms.RichTextBox();
            this.txtFolio = new System.Windows.Forms.TextBox();
            this.txtSubtotal = new System.Windows.Forms.TextBox();
            this.txtIVA = new System.Windows.Forms.TextBox();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.txtMoneda = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnCrear = new System.Windows.Forms.Button();
            this.cbMoneda = new System.Windows.Forms.ComboBox();
            this.ultraDropDown1 = new Infragistics.Win.UltraWinGrid.UltraDropDown();
            this.lm1 = new System.Windows.Forms.Label();
            this.lm3 = new System.Windows.Forms.Label();
            this.lm2 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).BeginInit();
            this.contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDropDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDatos
            // 
            this.dgvDatos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDatos.ContextMenuStrip = this.contextMenu;
            appearance25.BackColor = System.Drawing.SystemColors.Window;
            appearance25.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.dgvDatos.DisplayLayout.Appearance = appearance25;
            this.dgvDatos.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.dgvDatos.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance26.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance26.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance26.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance26.BorderColor = System.Drawing.SystemColors.Window;
            this.dgvDatos.DisplayLayout.GroupByBox.Appearance = appearance26;
            appearance27.ForeColor = System.Drawing.SystemColors.GrayText;
            this.dgvDatos.DisplayLayout.GroupByBox.BandLabelAppearance = appearance27;
            this.dgvDatos.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.dgvDatos.DisplayLayout.GroupByBox.Hidden = true;
            appearance28.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance28.BackColor2 = System.Drawing.SystemColors.Control;
            appearance28.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance28.ForeColor = System.Drawing.SystemColors.GrayText;
            this.dgvDatos.DisplayLayout.GroupByBox.PromptAppearance = appearance28;
            this.dgvDatos.DisplayLayout.MaxColScrollRegions = 1;
            this.dgvDatos.DisplayLayout.MaxRowScrollRegions = 1;
            appearance29.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance29.ForeColor = System.Drawing.SystemColors.ControlText;
            this.dgvDatos.DisplayLayout.Override.ActiveCellAppearance = appearance29;
            appearance30.ForeColor = System.Drawing.SystemColors.ControlText;
            this.dgvDatos.DisplayLayout.Override.ActiveRowAppearance = appearance30;
            this.dgvDatos.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            this.dgvDatos.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.dgvDatos.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.dgvDatos.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.dgvDatos.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.dgvDatos.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance31.BackColor = System.Drawing.SystemColors.Window;
            this.dgvDatos.DisplayLayout.Override.CardAreaAppearance = appearance31;
            appearance32.BorderColor = System.Drawing.Color.Silver;
            appearance32.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.dgvDatos.DisplayLayout.Override.CellAppearance = appearance32;
            this.dgvDatos.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.dgvDatos.DisplayLayout.Override.CellPadding = 0;
            this.dgvDatos.DisplayLayout.Override.FixedRowIndicator = Infragistics.Win.UltraWinGrid.FixedRowIndicator.None;
            appearance33.BackColor = System.Drawing.SystemColors.Control;
            appearance33.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance33.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance33.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance33.BorderColor = System.Drawing.SystemColors.Window;
            this.dgvDatos.DisplayLayout.Override.GroupByRowAppearance = appearance33;
            appearance34.TextHAlignAsString = "Left";
            this.dgvDatos.DisplayLayout.Override.HeaderAppearance = appearance34;
            this.dgvDatos.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            this.dgvDatos.DisplayLayout.Override.MergedCellStyle = Infragistics.Win.UltraWinGrid.MergedCellStyle.Never;
            appearance35.BackColor = System.Drawing.SystemColors.Window;
            appearance35.BorderColor = System.Drawing.Color.Silver;
            this.dgvDatos.DisplayLayout.Override.RowAppearance = appearance35;
            this.dgvDatos.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.dgvDatos.DisplayLayout.Override.RowSizingArea = Infragistics.Win.UltraWinGrid.RowSizingArea.RowSelectorsOnly;
            appearance36.BackColor = System.Drawing.SystemColors.ControlLight;
            this.dgvDatos.DisplayLayout.Override.TemplateAddRowAppearance = appearance36;
            this.dgvDatos.DisplayLayout.Override.WrapHeaderText = Infragistics.Win.DefaultableBoolean.True;
            this.dgvDatos.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.dgvDatos.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.dgvDatos.Location = new System.Drawing.Point(12, 113);
            this.dgvDatos.Name = "dgvDatos";
            this.dgvDatos.Size = new System.Drawing.Size(834, 221);
            this.dgvDatos.TabIndex = 28;
            this.dgvDatos.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.dgvDatos_InitializeRow);
            this.dgvDatos.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.dgvDatos_BeforeRowsDeleted);
            this.dgvDatos.SummaryValueChanged += new Infragistics.Win.UltraWinGrid.SummaryValueChangedEventHandler(this.dgvDatos_SummaryValueChanged);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.agregarFilaToolStripMenuItem1,
            this.tooStripDuplicar,
            this.toolStripEliminar,
            this.cerrarLíneaToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(147, 92);
            // 
            // agregarFilaToolStripMenuItem1
            // 
            this.agregarFilaToolStripMenuItem1.Name = "agregarFilaToolStripMenuItem1";
            this.agregarFilaToolStripMenuItem1.Size = new System.Drawing.Size(146, 22);
            this.agregarFilaToolStripMenuItem1.Text = "Añadir línea";
            this.agregarFilaToolStripMenuItem1.Click += new System.EventHandler(this.agregarFilaToolStripMenuItem1_Click);
            // 
            // tooStripDuplicar
            // 
            this.tooStripDuplicar.Name = "tooStripDuplicar";
            this.tooStripDuplicar.Size = new System.Drawing.Size(146, 22);
            this.tooStripDuplicar.Text = "Duplicar línea";
            this.tooStripDuplicar.Click += new System.EventHandler(this.tooStripDuplicar_Click);
            // 
            // toolStripEliminar
            // 
            this.toolStripEliminar.Name = "toolStripEliminar";
            this.toolStripEliminar.Size = new System.Drawing.Size(146, 22);
            this.toolStripEliminar.Text = "Borrar línea";
            this.toolStripEliminar.Click += new System.EventHandler(this.toolStripEliminar_Click);
            // 
            // cerrarLíneaToolStripMenuItem
            // 
            this.cerrarLíneaToolStripMenuItem.Enabled = false;
            this.cerrarLíneaToolStripMenuItem.Name = "cerrarLíneaToolStripMenuItem";
            this.cerrarLíneaToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.cerrarLíneaToolStripMenuItem.Text = "Cerrar línea";
            this.cerrarLíneaToolStripMenuItem.Click += new System.EventHandler(this.cerrarLíneaToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(612, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(612, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(612, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "label3";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(612, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "label4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "label5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 34;
            this.label6.Text = "label6";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 90);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "label7";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 347);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 36;
            this.label8.Text = "label8";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(612, 348);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 37;
            this.label9.Text = "label9";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(612, 369);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 13);
            this.label10.TabIndex = 38;
            this.label10.Text = "label10";
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(612, 389);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 13);
            this.label11.TabIndex = 39;
            this.label11.Text = "label11";
            // 
            // txtCardCode
            // 
            this.txtCardCode.Location = new System.Drawing.Point(104, 30);
            this.txtCardCode.Name = "txtCardCode";
            this.txtCardCode.Size = new System.Drawing.Size(100, 18);
            this.txtCardCode.TabIndex = 40;
            this.txtCardCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFolio_KeyPress);
            this.txtCardCode.Leave += new System.EventHandler(this.txtCardCode_Leave);
            // 
            // txtCardName
            // 
            this.txtCardName.BackColor = System.Drawing.SystemColors.Window;
            this.txtCardName.Location = new System.Drawing.Point(104, 49);
            this.txtCardName.Name = "txtCardName";
            this.txtCardName.Size = new System.Drawing.Size(247, 18);
            this.txtCardName.TabIndex = 41;
            this.txtCardName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFolio_KeyPress);
            // 
            // txtNumAtCard
            // 
            this.txtNumAtCard.Location = new System.Drawing.Point(104, 89);
            this.txtNumAtCard.Name = "txtNumAtCard";
            this.txtNumAtCard.Size = new System.Drawing.Size(247, 18);
            this.txtNumAtCard.TabIndex = 42;
            this.txtNumAtCard.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFolio_KeyPress);
            // 
            // dtpDocDate
            // 
            this.dtpDocDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpDocDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDocDate.Location = new System.Drawing.Point(746, 69);
            this.dtpDocDate.Name = "dtpDocDate";
            this.dtpDocDate.Size = new System.Drawing.Size(100, 18);
            this.dtpDocDate.TabIndex = 43;
            this.dtpDocDate.ValueChanged += new System.EventHandler(this.dtpDocDate_ValueChanged);
            this.dtpDocDate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFolio_KeyPress);
            // 
            // dtpDocDueDate
            // 
            this.dtpDocDueDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpDocDueDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDocDueDate.Location = new System.Drawing.Point(746, 89);
            this.dtpDocDueDate.Name = "dtpDocDueDate";
            this.dtpDocDueDate.Size = new System.Drawing.Size(100, 18);
            this.dtpDocDueDate.TabIndex = 44;
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(746, 50);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(100, 18);
            this.txtStatus.TabIndex = 46;
            // 
            // txtComments
            // 
            this.txtComments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtComments.Location = new System.Drawing.Point(104, 348);
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size(294, 68);
            this.txtComments.TabIndex = 47;
            this.txtComments.Text = "";
            // 
            // txtFolio
            // 
            this.txtFolio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolio.Location = new System.Drawing.Point(746, 30);
            this.txtFolio.Name = "txtFolio";
            this.txtFolio.Size = new System.Drawing.Size(100, 18);
            this.txtFolio.TabIndex = 49;
            this.txtFolio.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFolio_KeyPress);
            // 
            // txtSubtotal
            // 
            this.txtSubtotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubtotal.Location = new System.Drawing.Point(746, 345);
            this.txtSubtotal.Name = "txtSubtotal";
            this.txtSubtotal.Size = new System.Drawing.Size(100, 18);
            this.txtSubtotal.TabIndex = 50;
            this.txtSubtotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtIVA
            // 
            this.txtIVA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIVA.Location = new System.Drawing.Point(746, 365);
            this.txtIVA.Name = "txtIVA";
            this.txtIVA.Size = new System.Drawing.Size(100, 18);
            this.txtIVA.TabIndex = 51;
            this.txtIVA.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtTotal
            // 
            this.txtTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotal.Location = new System.Drawing.Point(746, 385);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(100, 18);
            this.txtTotal.TabIndex = 52;
            this.txtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtMoneda
            // 
            this.txtMoneda.Location = new System.Drawing.Point(231, 68);
            this.txtMoneda.Name = "txtMoneda";
            this.txtMoneda.Size = new System.Drawing.Size(120, 18);
            this.txtMoneda.TabIndex = 54;
            this.txtMoneda.TextChanged += new System.EventHandler(this.txtMoneda_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 72);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 13);
            this.label12.TabIndex = 53;
            this.label12.Text = "label12";
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelar.Location = new System.Drawing.Point(104, 433);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(87, 26);
            this.btnCancelar.TabIndex = 55;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnCrear
            // 
            this.btnCrear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCrear.Location = new System.Drawing.Point(12, 433);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(87, 26);
            this.btnCrear.TabIndex = 56;
            this.btnCrear.Text = "Crear";
            this.btnCrear.UseVisualStyleBackColor = true;
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            // 
            // cbMoneda
            // 
            this.cbMoneda.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMoneda.FormattingEnabled = true;
            this.cbMoneda.Location = new System.Drawing.Point(104, 68);
            this.cbMoneda.Name = "cbMoneda";
            this.cbMoneda.Size = new System.Drawing.Size(121, 20);
            this.cbMoneda.TabIndex = 57;
            this.cbMoneda.SelectionChangeCommitted += new System.EventHandler(this.cbMoneda_SelectionChangeCommitted);
            // 
            // ultraDropDown1
            // 
            this.ultraDropDown1.Location = new System.Drawing.Point(404, 348);
            this.ultraDropDown1.Name = "ultraDropDown1";
            this.ultraDropDown1.Size = new System.Drawing.Size(126, 15);
            this.ultraDropDown1.TabIndex = 58;
            this.ultraDropDown1.Text = "ultraDropDown1";
            this.ultraDropDown1.Visible = false;
            this.ultraDropDown1.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraDropDown1_InitializeLayout);
            // 
            // lm1
            // 
            this.lm1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lm1.AutoSize = true;
            this.lm1.Location = new System.Drawing.Point(714, 347);
            this.lm1.Name = "lm1";
            this.lm1.Size = new System.Drawing.Size(10, 13);
            this.lm1.TabIndex = 59;
            this.lm1.Text = " ";
            this.lm1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lm3
            // 
            this.lm3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lm3.AutoSize = true;
            this.lm3.Location = new System.Drawing.Point(714, 388);
            this.lm3.Name = "lm3";
            this.lm3.Size = new System.Drawing.Size(10, 13);
            this.lm3.TabIndex = 60;
            this.lm3.Text = " ";
            this.lm3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lm2
            // 
            this.lm2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lm2.AutoSize = true;
            this.lm2.Location = new System.Drawing.Point(714, 368);
            this.lm2.Name = "lm2";
            this.lm2.Size = new System.Drawing.Size(10, 13);
            this.lm2.TabIndex = 61;
            this.lm2.Text = " ";
            this.lm2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.LightGray;
            this.label13.Location = new System.Drawing.Point(780, 437);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 13);
            this.label13.TabIndex = 62;
            this.label13.Text = "Línea cerrada";
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.SystemColors.Control;
            this.label14.ForeColor = System.Drawing.Color.Red;
            this.label14.Location = new System.Drawing.Point(780, 449);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(54, 13);
            this.label14.TabIndex = 63;
            this.label14.Text = "Backorder";
            // 
            // frmDocumentos
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(858, 486);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtComments);
            this.Controls.Add(this.lm2);
            this.Controls.Add(this.lm3);
            this.Controls.Add(this.lm1);
            this.Controls.Add(this.ultraDropDown1);
            this.Controls.Add(this.cbMoneda);
            this.Controls.Add(this.btnCrear);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.txtMoneda);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.txtIVA);
            this.Controls.Add(this.txtSubtotal);
            this.Controls.Add(this.txtFolio);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.dtpDocDueDate);
            this.Controls.Add(this.dtpDocDate);
            this.Controls.Add(this.txtNumAtCard);
            this.Controls.Add(this.txtCardName);
            this.Controls.Add(this.txtCardCode);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvDatos);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.Name = "frmDocumentos";
            validaciones1.Mensaje = null;
            validaciones1.Result = false;
            this.ObjValidaciones = validaciones1;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmDocumentos";
            this.Ug = this.dgvDatos;
            this.Load += new System.EventHandler(this.frmOrdenCompra_Load);
            this.Shown += new System.EventHandler(this.frmDocumentos_Shown);
            this.Controls.SetChildIndex(this.dgvDatos, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.label5, 0);
            this.Controls.SetChildIndex(this.label6, 0);
            this.Controls.SetChildIndex(this.label7, 0);
            this.Controls.SetChildIndex(this.label8, 0);
            this.Controls.SetChildIndex(this.label9, 0);
            this.Controls.SetChildIndex(this.label10, 0);
            this.Controls.SetChildIndex(this.label11, 0);
            this.Controls.SetChildIndex(this.txtCardCode, 0);
            this.Controls.SetChildIndex(this.txtCardName, 0);
            this.Controls.SetChildIndex(this.txtNumAtCard, 0);
            this.Controls.SetChildIndex(this.dtpDocDate, 0);
            this.Controls.SetChildIndex(this.dtpDocDueDate, 0);
            this.Controls.SetChildIndex(this.txtStatus, 0);
            this.Controls.SetChildIndex(this.txtFolio, 0);
            this.Controls.SetChildIndex(this.txtSubtotal, 0);
            this.Controls.SetChildIndex(this.txtIVA, 0);
            this.Controls.SetChildIndex(this.txtTotal, 0);
            this.Controls.SetChildIndex(this.label12, 0);
            this.Controls.SetChildIndex(this.txtMoneda, 0);
            this.Controls.SetChildIndex(this.btnCancelar, 0);
            this.Controls.SetChildIndex(this.btnCrear, 0);
            this.Controls.SetChildIndex(this.cbMoneda, 0);
            this.Controls.SetChildIndex(this.ultraDropDown1, 0);
            this.Controls.SetChildIndex(this.lm1, 0);
            this.Controls.SetChildIndex(this.lm3, 0);
            this.Controls.SetChildIndex(this.lm2, 0);
            this.Controls.SetChildIndex(this.txtComments, 0);
            this.Controls.SetChildIndex(this.label13, 0);
            this.Controls.SetChildIndex(this.label14, 0);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).EndInit();
            this.contextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraDropDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        public Infragistics.Win.UltraWinGrid.UltraGrid dgvDatos;
        public System.Windows.Forms.TextBox txtCardCode;
        public System.Windows.Forms.TextBox txtCardName;
        public System.Windows.Forms.TextBox txtNumAtCard;
        public System.Windows.Forms.DateTimePicker dtpDocDate;
        public System.Windows.Forms.DateTimePicker dtpDocDueDate;
        public System.Windows.Forms.TextBox txtStatus;
        public System.Windows.Forms.RichTextBox txtComments;
        public System.Windows.Forms.TextBox txtFolio;
        public System.Windows.Forms.TextBox txtSubtotal;
        public System.Windows.Forms.TextBox txtIVA;
        public System.Windows.Forms.TextBox txtTotal;
        public System.Windows.Forms.TextBox txtMoneda;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.Button btnCancelar;
        public System.Windows.Forms.Button btnCrear;
        public System.Windows.Forms.ComboBox cbMoneda;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem tooStripDuplicar;
        private System.Windows.Forms.ToolStripMenuItem toolStripEliminar;
        private System.Windows.Forms.ToolStripMenuItem agregarFilaToolStripMenuItem1;
        public Infragistics.Win.UltraWinGrid.UltraDropDown ultraDropDown1;
        private System.Windows.Forms.Label lm1;
        private System.Windows.Forms.Label lm3;
        private System.Windows.Forms.Label lm2;
        private System.Windows.Forms.ToolStripMenuItem cerrarLíneaToolStripMenuItem;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
    }
}