namespace area_L
{
    partial class RE_INSERT_FORM
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
            this.tx_BARCODE = new System.Windows.Forms.TextBox();
            this.tx_GDNM = new System.Windows.Forms.TextBox();
            this.tx_MATE_NO = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2.SuspendLayout();
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
            this.tableLayoutPanel2.Controls.Add(this.tx_BARCODE, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tx_GDNM, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.tx_MATE_NO, 1, 2);
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
            this.tableLayoutPanel2.Size = new System.Drawing.Size(416, 198);
            this.tableLayoutPanel2.TabIndex = 3;
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
            this.lb_GDCD.Size = new System.Drawing.Size(164, 47);
            this.lb_GDCD.TabIndex = 1;
            this.lb_GDCD.Text = "바코드 :";
            this.lb_GDCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_GDNM
            // 
            this.lb_GDNM.AutoSize = true;
            this.lb_GDNM.BackColor = System.Drawing.Color.Transparent;
            this.lb_GDNM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_GDNM.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_GDNM.Location = new System.Drawing.Point(1, 50);
            this.lb_GDNM.Margin = new System.Windows.Forms.Padding(1);
            this.lb_GDNM.Name = "lb_GDNM";
            this.lb_GDNM.Size = new System.Drawing.Size(164, 47);
            this.lb_GDNM.TabIndex = 2;
            this.lb_GDNM.Text = "제품코드 :";
            this.lb_GDNM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_MATENO
            // 
            this.lb_MATENO.AutoSize = true;
            this.lb_MATENO.BackColor = System.Drawing.Color.Transparent;
            this.lb_MATENO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_MATENO.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_MATENO.Location = new System.Drawing.Point(1, 99);
            this.lb_MATENO.Margin = new System.Windows.Forms.Padding(1);
            this.lb_MATENO.Name = "lb_MATENO";
            this.lb_MATENO.Size = new System.Drawing.Size(164, 47);
            this.lb_MATENO.TabIndex = 3;
            this.lb_MATENO.Text = "멸균no :";
            this.lb_MATENO.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btn_Confirm
            // 
            this.btn_Confirm.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.btn_Confirm, 2);
            this.btn_Confirm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Confirm.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Confirm.Location = new System.Drawing.Point(3, 150);
            this.btn_Confirm.Name = "btn_Confirm";
            this.btn_Confirm.Size = new System.Drawing.Size(410, 45);
            this.btn_Confirm.TabIndex = 4;
            this.btn_Confirm.Text = "입력";
            this.btn_Confirm.UseVisualStyleBackColor = true;
            this.btn_Confirm.Click += new System.EventHandler(this.btn_Confirm_Click);
            // 
            // tx_BARCODE
            // 
            this.tx_BARCODE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tx_BARCODE.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tx_BARCODE.Location = new System.Drawing.Point(169, 7);
            this.tx_BARCODE.Name = "tx_BARCODE";
            this.tx_BARCODE.ReadOnly = true;
            this.tx_BARCODE.Size = new System.Drawing.Size(244, 35);
            this.tx_BARCODE.TabIndex = 5;
            // 
            // tx_GDNM
            // 
            this.tx_GDNM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tx_GDNM.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tx_GDNM.Location = new System.Drawing.Point(169, 56);
            this.tx_GDNM.Name = "tx_GDNM";
            this.tx_GDNM.Size = new System.Drawing.Size(244, 35);
            this.tx_GDNM.TabIndex = 6;
            // 
            // tx_MATE_NO
            // 
            this.tx_MATE_NO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tx_MATE_NO.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tx_MATE_NO.Location = new System.Drawing.Point(169, 105);
            this.tx_MATE_NO.Name = "tx_MATE_NO";
            this.tx_MATE_NO.Size = new System.Drawing.Size(244, 35);
            this.tx_MATE_NO.TabIndex = 7;
            this.tx_MATE_NO.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tx_MATE_NO_KeyDown);
            // 
            // RE_INSERT_FORM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 198);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "RE_INSERT_FORM";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "멸균번호입력";
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lb_GDCD;
        private System.Windows.Forms.Label lb_GDNM;
        private System.Windows.Forms.Label lb_MATENO;
        private System.Windows.Forms.Button btn_Confirm;
        private System.Windows.Forms.TextBox tx_BARCODE;
        private System.Windows.Forms.TextBox tx_GDNM;
        private System.Windows.Forms.TextBox tx_MATE_NO;
    }
}