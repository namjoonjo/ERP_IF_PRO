namespace RAZER_C.Danpla
{
    partial class INSERT_PALLET_INFO
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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lb_GDCD = new System.Windows.Forms.Label();
            this.lb_GDNM = new System.Windows.Forms.Label();
            this.lb_MATENO = new System.Windows.Forms.Label();
            this.btn_Confirm = new System.Windows.Forms.Button();
            this.tbx_orderNo = new DevExpress.XtraEditors.TextEdit();
            this.tbx_Date = new DevExpress.XtraEditors.TextEdit();
            this.cbx_FACCD = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbx_orderNo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbx_Date.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel2.Controls.Add(this.lb_GDCD, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lb_GDNM, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lb_MATENO, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.btn_Confirm, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.tbx_Date, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.tbx_orderNo, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.cbx_FACCD, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(484, 211);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // lb_GDCD
            // 
            this.lb_GDCD.AutoSize = true;
            this.lb_GDCD.BackColor = System.Drawing.Color.Transparent;
            this.lb_GDCD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_GDCD.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_GDCD.Location = new System.Drawing.Point(1, 1);
            this.lb_GDCD.Margin = new System.Windows.Forms.Padding(1);
            this.lb_GDCD.Name = "lb_GDCD";
            this.lb_GDCD.Size = new System.Drawing.Size(191, 50);
            this.lb_GDCD.TabIndex = 1;
            this.lb_GDCD.Text = "공장구분 :";
            this.lb_GDCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_GDNM
            // 
            this.lb_GDNM.AutoSize = true;
            this.lb_GDNM.BackColor = System.Drawing.Color.Transparent;
            this.lb_GDNM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_GDNM.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_GDNM.Location = new System.Drawing.Point(1, 53);
            this.lb_GDNM.Margin = new System.Windows.Forms.Padding(1);
            this.lb_GDNM.Name = "lb_GDNM";
            this.lb_GDNM.Size = new System.Drawing.Size(191, 50);
            this.lb_GDNM.TabIndex = 2;
            this.lb_GDNM.Text = "구성날짜 :";
            this.lb_GDNM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_MATENO
            // 
            this.lb_MATENO.AutoSize = true;
            this.lb_MATENO.BackColor = System.Drawing.Color.Transparent;
            this.lb_MATENO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_MATENO.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_MATENO.Location = new System.Drawing.Point(1, 105);
            this.lb_MATENO.Margin = new System.Windows.Forms.Padding(1);
            this.lb_MATENO.Name = "lb_MATENO";
            this.lb_MATENO.Size = new System.Drawing.Size(191, 50);
            this.lb_MATENO.TabIndex = 3;
            this.lb_MATENO.Text = "오더번호 :";
            this.lb_MATENO.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btn_Confirm
            // 
            this.btn_Confirm.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.btn_Confirm, 2);
            this.btn_Confirm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Confirm.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Confirm.Location = new System.Drawing.Point(3, 159);
            this.btn_Confirm.Name = "btn_Confirm";
            this.btn_Confirm.Size = new System.Drawing.Size(478, 49);
            this.btn_Confirm.TabIndex = 4;
            this.btn_Confirm.Text = "선택";
            this.btn_Confirm.UseVisualStyleBackColor = true;
            // 
            // tbx_orderNo
            // 
            this.tbx_orderNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbx_orderNo.Location = new System.Drawing.Point(196, 114);
            this.tbx_orderNo.Name = "tbx_orderNo";
            this.tbx_orderNo.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbx_orderNo.Properties.Appearance.Options.UseFont = true;
            this.tbx_orderNo.Size = new System.Drawing.Size(285, 32);
            this.tbx_orderNo.TabIndex = 8;
            // 
            // tbx_Date
            // 
            this.tbx_Date.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbx_Date.Location = new System.Drawing.Point(196, 62);
            this.tbx_Date.Name = "tbx_Date";
            this.tbx_Date.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbx_Date.Properties.Appearance.Options.UseFont = true;
            this.tbx_Date.Properties.ReadOnly = true;
            this.tbx_Date.Size = new System.Drawing.Size(285, 32);
            this.tbx_Date.TabIndex = 9;
            // 
            // cbx_FACCD
            // 
            this.cbx_FACCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbx_FACCD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_FACCD.Font = new System.Drawing.Font("굴림", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbx_FACCD.FormattingEnabled = true;
            this.cbx_FACCD.Location = new System.Drawing.Point(196, 16);
            this.cbx_FACCD.Name = "cbx_FACCD";
            this.cbx_FACCD.Size = new System.Drawing.Size(285, 29);
            this.cbx_FACCD.TabIndex = 10;
            // 
            // INSERT_PALLET_INFO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 211);
            this.Controls.Add(this.tableLayoutPanel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "INSERT_PALLET_INFO";
            this.Text = "INSERT_PALLET_INFO";
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbx_orderNo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbx_Date.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lb_GDCD;
        private System.Windows.Forms.Label lb_GDNM;
        private System.Windows.Forms.Label lb_MATENO;
        private System.Windows.Forms.Button btn_Confirm;
        private DevExpress.XtraEditors.TextEdit tbx_orderNo;
        private DevExpress.XtraEditors.TextEdit tbx_Date;
        private System.Windows.Forms.ComboBox cbx_FACCD;
    }
}