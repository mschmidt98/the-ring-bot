namespace SelectionPopup
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.Btn_Komme = new System.Windows.Forms.Button();
            this.Btn_Nein = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Btn_Komme
            // 
            this.Btn_Komme.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Komme.Location = new System.Drawing.Point(16, 315);
            this.Btn_Komme.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Btn_Komme.Name = "Btn_Komme";
            this.Btn_Komme.Size = new System.Drawing.Size(236, 107);
            this.Btn_Komme.TabIndex = 2;
            this.Btn_Komme.Text = "Komme";
            this.Btn_Komme.UseVisualStyleBackColor = true;
            this.Btn_Komme.Click += new System.EventHandler(this.Btn_Komme_Click);
            // 
            // Btn_Nein
            // 
            this.Btn_Nein.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Nein.Location = new System.Drawing.Point(359, 318);
            this.Btn_Nein.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Btn_Nein.Name = "Btn_Nein";
            this.Btn_Nein.Size = new System.Drawing.Size(236, 107);
            this.Btn_Nein.TabIndex = 4;
            this.Btn_Nein.Text = "Ablehnen";
            this.Btn_Nein.UseVisualStyleBackColor = true;
            this.Btn_Nein.Click += new System.EventHandler(this.Btn_Nein_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(16, 15);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(579, 295);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 430);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Btn_Nein);
            this.Controls.Add(this.Btn_Komme);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Jemand steht an der Tür!";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Btn_Komme;
        private System.Windows.Forms.Button Btn_Nein;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

