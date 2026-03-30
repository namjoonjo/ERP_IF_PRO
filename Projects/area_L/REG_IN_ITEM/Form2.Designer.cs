namespace area_L
{
    partial class Form2
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cmd_Upload = new System.Windows.Forms.Button();
            this.cmd_Search = new System.Windows.Forms.Button();
            this.cmd_Reset = new System.Windows.Forms.Button();
            this.cmd_excel = new System.Windows.Forms.Button();
            this.fpExcel = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpExcel)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.fpExcel, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15.77778F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 84.22222F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(406, 450);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Controls.Add(this.cmd_Upload, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.cmd_Search, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.cmd_Reset, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.cmd_excel, 3, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(400, 65);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // cmd_Upload
            // 
            this.cmd_Upload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmd_Upload.Location = new System.Drawing.Point(3, 3);
            this.cmd_Upload.Name = "cmd_Upload";
            this.cmd_Upload.Size = new System.Drawing.Size(94, 59);
            this.cmd_Upload.TabIndex = 0;
            this.cmd_Upload.Text = "Upload";
            this.cmd_Upload.UseVisualStyleBackColor = true;
            this.cmd_Upload.Click += new System.EventHandler(this.cmd_Upload_Click);
            // 
            // cmd_Search
            // 
            this.cmd_Search.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmd_Search.Location = new System.Drawing.Point(103, 3);
            this.cmd_Search.Name = "cmd_Search";
            this.cmd_Search.Size = new System.Drawing.Size(94, 59);
            this.cmd_Search.TabIndex = 0;
            this.cmd_Search.Text = "조회";
            this.cmd_Search.UseVisualStyleBackColor = true;
            this.cmd_Search.Click += new System.EventHandler(this.cmd_Search_Click);
            // 
            // cmd_Reset
            // 
            this.cmd_Reset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmd_Reset.Location = new System.Drawing.Point(203, 3);
            this.cmd_Reset.Name = "cmd_Reset";
            this.cmd_Reset.Size = new System.Drawing.Size(94, 59);
            this.cmd_Reset.TabIndex = 0;
            this.cmd_Reset.Text = "Reset";
            this.cmd_Reset.UseVisualStyleBackColor = true;
            this.cmd_Reset.Click += new System.EventHandler(this.cmd_Reset_Click);
            // 
            // cmd_excel
            // 
            this.cmd_excel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmd_excel.Location = new System.Drawing.Point(303, 3);
            this.cmd_excel.Name = "cmd_excel";
            this.cmd_excel.Size = new System.Drawing.Size(94, 59);
            this.cmd_excel.TabIndex = 0;
            this.cmd_excel.Text = "Excel DOWN";
            this.cmd_excel.UseVisualStyleBackColor = true;
            this.cmd_excel.Click += new System.EventHandler(this.cmd_excel_Click);
            // 
            // fpExcel
            // 
            this.fpExcel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fpExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpExcel.Location = new System.Drawing.Point(3, 74);
            this.fpExcel.Name = "fpExcel";
            this.fpExcel.RowTemplate.Height = 27;
            this.fpExcel.Size = new System.Drawing.Size(400, 373);
            this.fpExcel.TabIndex = 1;
            this.fpExcel.Click += new System.EventHandler(this.fpExcel_Click);
            this.fpExcel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.fpExcel_MouseDoubleClick);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form2";
            this.Text = "부족분관리_List";
            this.Activated += new System.EventHandler(this.Form2_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form2_FormClosing);
            this.Load += new System.EventHandler(this.Form2_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpExcel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button cmd_Upload;
        private System.Windows.Forms.Button cmd_Search;
        private System.Windows.Forms.Button cmd_Reset;
        private System.Windows.Forms.Button cmd_excel;
        private System.Windows.Forms.DataGridView fpExcel;
    }
}