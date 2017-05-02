namespace H_Compras.Inventarios.Conteos
{
    partial class frmAlert1
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
            this.txtComments = new System.Windows.Forms.TextBox();
            this.rb1 = new System.Windows.Forms.RadioButton();
            this.rb2 = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.btnContinuar = new System.Windows.Forms.Button();
            this.rb3 = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.btnExaminar = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rbGeneral = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 137);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Comentarios:";
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point(15, 151);
            this.txtComments.MaxLength = 250;
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size(463, 66);
            this.txtComments.TabIndex = 1;
            // 
            // rb1
            // 
            this.rb1.AccessibleName = "A";
            this.rb1.AutoSize = true;
            this.rb1.Checked = true;
            this.rb1.Location = new System.Drawing.Point(26, 43);
            this.rb1.Name = "rb1";
            this.rb1.Size = new System.Drawing.Size(114, 17);
            this.rb1.TabIndex = 2;
            this.rb1.TabStop = true;
            this.rb1.Text = "Listado predefinido";
            this.rb1.UseVisualStyleBackColor = true;
            this.rb1.Click += new System.EventHandler(this.rbClick);
            // 
            // rb2
            // 
            this.rb2.AccessibleName = "B";
            this.rb2.AutoSize = true;
            this.rb2.Location = new System.Drawing.Point(157, 43);
            this.rb2.Name = "rb2";
            this.rb2.Size = new System.Drawing.Size(109, 17);
            this.rb2.TabIndex = 3;
            this.rb2.Text = "Listado en blanco";
            this.rb2.UseVisualStyleBackColor = true;
            this.rb2.Click += new System.EventHandler(this.rbClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Tipo de listado";
            // 
            // btnContinuar
            // 
            this.btnContinuar.Location = new System.Drawing.Point(403, 220);
            this.btnContinuar.Name = "btnContinuar";
            this.btnContinuar.Size = new System.Drawing.Size(75, 23);
            this.btnContinuar.TabIndex = 5;
            this.btnContinuar.Text = "Continuar";
            this.btnContinuar.UseVisualStyleBackColor = true;
            this.btnContinuar.Click += new System.EventHandler(this.btnContinuar_Click);
            // 
            // rb3
            // 
            this.rb3.AccessibleName = "C";
            this.rb3.AutoSize = true;
            this.rb3.Location = new System.Drawing.Point(279, 43);
            this.rb3.Name = "rb3";
            this.rb3.Size = new System.Drawing.Size(96, 17);
            this.rb3.TabIndex = 6;
            this.rb3.Text = "Importar listado";
            this.rb3.UseVisualStyleBackColor = true;
            this.rb3.Click += new System.EventHandler(this.rbClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Examinar:";
            // 
            // btnExaminar
            // 
            this.btnExaminar.Location = new System.Drawing.Point(326, 72);
            this.btnExaminar.Name = "btnExaminar";
            this.btnExaminar.Size = new System.Drawing.Size(32, 23);
            this.btnExaminar.TabIndex = 8;
            this.btnExaminar.Text = "...";
            this.btnExaminar.UseVisualStyleBackColor = true;
            this.btnExaminar.Click += new System.EventHandler(this.btnExaminar_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(71, 73);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(249, 20);
            this.txtPath.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(360, 39);
            this.label4.TabIndex = 10;
            this.label4.Text = "Debe seleccionar un archivo de texto separado por tabulaciones, con una \r\nuna uni" +
    "ca columna sin titulo que incluya el código de artículo tal y como \r\nse usa en S" +
    "AP.";
            // 
            // rbGeneral
            // 
            this.rbGeneral.AccessibleName = "D";
            this.rbGeneral.AutoSize = true;
            this.rbGeneral.Location = new System.Drawing.Point(381, 43);
            this.rbGeneral.Name = "rbGeneral";
            this.rbGeneral.Size = new System.Drawing.Size(97, 17);
            this.rbGeneral.TabIndex = 11;
            this.rbGeneral.Text = "Conteo general";
            this.rbGeneral.UseVisualStyleBackColor = true;
            // 
            // frmAlert1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 268);
            this.Controls.Add(this.rbGeneral);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.btnExaminar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rb3);
            this.Controls.Add(this.btnContinuar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rb2);
            this.Controls.Add(this.rb1);
            this.Controls.Add(this.txtComments);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAlert1";
            validaciones1.Mensaje = null;
            validaciones1.Result = false;
            this.ObjValidaciones = validaciones1;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Comentarios";
            this.Load += new System.EventHandler(this.frmAlert1_Load);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.txtComments, 0);
            this.Controls.SetChildIndex(this.rb1, 0);
            this.Controls.SetChildIndex(this.rb2, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.btnContinuar, 0);
            this.Controls.SetChildIndex(this.rb3, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.btnExaminar, 0);
            this.Controls.SetChildIndex(this.txtPath, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.rbGeneral, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtComments;
        public System.Windows.Forms.RadioButton rb1;
        public System.Windows.Forms.RadioButton rb2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnContinuar;
        public System.Windows.Forms.RadioButton rb3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnExaminar;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox txtPath;
        public System.Windows.Forms.RadioButton rbGeneral;
    }
}