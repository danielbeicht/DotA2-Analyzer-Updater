namespace DotaAnalyzerUpdater
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this.labelUpdateStatus = new System.Windows.Forms.Label();
            this.labelInformation = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 31);
            this.progressBar1.Maximum = 110;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(210, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(228, 31);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Update Data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelUpdateStatus
            // 
            this.labelUpdateStatus.AutoSize = true;
            this.labelUpdateStatus.Location = new System.Drawing.Point(9, 60);
            this.labelUpdateStatus.Name = "labelUpdateStatus";
            this.labelUpdateStatus.Size = new System.Drawing.Size(163, 13);
            this.labelUpdateStatus.TabIndex = 2;
            this.labelUpdateStatus.Text = "Next Auto Update in 600 minutes";
            // 
            // labelInformation
            // 
            this.labelInformation.AutoSize = true;
            this.labelInformation.Location = new System.Drawing.Point(12, 87);
            this.labelInformation.Name = "labelInformation";
            this.labelInformation.Size = new System.Drawing.Size(0, 13);
            this.labelInformation.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Fetching Data";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 111);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelInformation);
            this.Controls.Add(this.labelUpdateStatus);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.progressBar1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Dota2 - Analyzer - Updater v1337";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelUpdateStatus;
        private System.Windows.Forms.Label labelInformation;
        private System.Windows.Forms.Label label1;
    }
}

