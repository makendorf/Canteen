
namespace Canteen
{
    partial class Inventarization
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
            this.dataGridInventarization = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridInventarization)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridInventarization
            // 
            this.dataGridInventarization.AllowUserToAddRows = false;
            this.dataGridInventarization.AllowUserToDeleteRows = false;
            this.dataGridInventarization.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridInventarization.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridInventarization.Location = new System.Drawing.Point(0, 0);
            this.dataGridInventarization.Name = "dataGridInventarization";
            this.dataGridInventarization.ReadOnly = true;
            this.dataGridInventarization.Size = new System.Drawing.Size(800, 450);
            this.dataGridInventarization.TabIndex = 0;
            // 
            // Inventarization
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dataGridInventarization);
            this.Name = "Inventarization";
            this.Text = "Inventarization";
            this.Load += new System.EventHandler(this.Inventarization_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridInventarization)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridInventarization;
    }
}