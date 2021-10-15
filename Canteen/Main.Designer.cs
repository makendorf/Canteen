
namespace Canteen
{
    partial class Main
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.movementPage = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.movementDataGrid = new System.Windows.Forms.DataGridView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.movementProductionProductDataGrid = new System.Windows.Forms.DataGridView();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.productPage = new System.Windows.Forms.TabPage();
            this.productionSalePage = new System.Windows.Forms.TabPage();
            this.reportPage = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.movementPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.movementDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.movementProductionProductDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.movementPage);
            this.tabControl1.Controls.Add(this.productPage);
            this.tabControl1.Controls.Add(this.productionSalePage);
            this.tabControl1.Controls.Add(this.reportPage);
            this.tabControl1.Location = new System.Drawing.Point(23, 63);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(969, 500);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // movementPage
            // 
            this.movementPage.BackColor = System.Drawing.Color.White;
            this.movementPage.Controls.Add(this.splitContainer1);
            this.movementPage.Location = new System.Drawing.Point(4, 22);
            this.movementPage.Name = "movementPage";
            this.movementPage.Padding = new System.Windows.Forms.Padding(3);
            this.movementPage.Size = new System.Drawing.Size(961, 474);
            this.movementPage.TabIndex = 0;
            this.movementPage.Text = "Движение продуктов";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.movementDataGrid);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(955, 468);
            this.splitContainer1.SplitterDistance = 653;
            this.splitContainer1.TabIndex = 0;
            // 
            // movementDataGrid
            // 
            this.movementDataGrid.AllowUserToAddRows = false;
            this.movementDataGrid.AllowUserToDeleteRows = false;
            this.movementDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.movementDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.movementDataGrid.Location = new System.Drawing.Point(0, 0);
            this.movementDataGrid.Name = "movementDataGrid";
            this.movementDataGrid.ReadOnly = true;
            this.movementDataGrid.Size = new System.Drawing.Size(653, 468);
            this.movementDataGrid.TabIndex = 0;
            this.movementDataGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.movementDataGrid_CellClick);
            this.movementDataGrid.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.movementDataGrid_RowHeaderMouseClick);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.movementProductionProductDataGrid);
            this.splitContainer2.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer2.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer2.Size = new System.Drawing.Size(298, 468);
            this.splitContainer2.SplitterDistance = 307;
            this.splitContainer2.TabIndex = 0;
            // 
            // movementProductionProductDataGrid
            // 
            this.movementProductionProductDataGrid.AllowUserToAddRows = false;
            this.movementProductionProductDataGrid.AllowUserToDeleteRows = false;
            this.movementProductionProductDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.movementProductionProductDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.movementProductionProductDataGrid.Location = new System.Drawing.Point(0, 0);
            this.movementProductionProductDataGrid.Name = "movementProductionProductDataGrid";
            this.movementProductionProductDataGrid.ReadOnly = true;
            this.movementProductionProductDataGrid.Size = new System.Drawing.Size(298, 307);
            this.movementProductionProductDataGrid.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::Canteen.Properties.Resources.gmkicon;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(298, 157);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // productPage
            // 
            this.productPage.BackColor = System.Drawing.Color.White;
            this.productPage.Location = new System.Drawing.Point(4, 22);
            this.productPage.Name = "productPage";
            this.productPage.Size = new System.Drawing.Size(961, 474);
            this.productPage.TabIndex = 1;
            this.productPage.Text = "Рецептуры";
            // 
            // productionSalePage
            // 
            this.productionSalePage.Location = new System.Drawing.Point(4, 22);
            this.productionSalePage.Name = "productionSalePage";
            this.productionSalePage.Padding = new System.Windows.Forms.Padding(3);
            this.productionSalePage.Size = new System.Drawing.Size(961, 474);
            this.productionSalePage.TabIndex = 2;
            this.productionSalePage.Text = "Производство и продажи";
            this.productionSalePage.UseVisualStyleBackColor = true;
            // 
            // reportPage
            // 
            this.reportPage.Location = new System.Drawing.Point(4, 22);
            this.reportPage.Name = "reportPage";
            this.reportPage.Size = new System.Drawing.Size(961, 474);
            this.reportPage.TabIndex = 3;
            this.reportPage.Text = "Отчетность";
            this.reportPage.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 586);
            this.Controls.Add(this.tabControl1);
            this.Name = "Main";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Default;
            this.Text = "Движение сырья в Столовой";
            this.Load += new System.EventHandler(this.Main_Load);
            this.tabControl1.ResumeLayout(false);
            this.movementPage.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.movementDataGrid)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.movementProductionProductDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage movementPage;
        private System.Windows.Forms.TabPage productPage;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView movementProductionProductDataGrid;
        public System.Windows.Forms.DataGridView movementDataGrid;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TabPage productionSalePage;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabPage reportPage;
    }
}

