namespace COMBINATION
{
    partial class PRODUCTION_ANALYSIS
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.StartDate = new System.Windows.Forms.DateTimePicker();
            this.CycleTime = new System.Windows.Forms.NumericUpDown();
            this.cbx_Kind = new System.Windows.Forms.ComboBox();
            this.cbx_Worker = new System.Windows.Forms.ComboBox();
            this.tbx_FileRoute = new System.Windows.Forms.TextBox();
            this.btn_find = new System.Windows.Forms.Button();
            this.EndDate = new System.Windows.Forms.DateTimePicker();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.grid_State = new System.Windows.Forms.DataGridView();
            this.groupcon = new DevExpress.XtraEditors.GroupControl();
            this.grid_State2 = new System.Windows.Forms.DataGridView();
            this.procTimer = new System.Windows.Forms.Timer(this.components);
            this.btn_Status = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CycleTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid_State)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupcon)).BeginInit();
            this.groupcon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid_State2)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.groupControl1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupControl2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupcon, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btn_Status, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1184, 761);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // groupControl1
            // 
            this.groupControl1.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupControl1.AppearanceCaption.Options.UseFont = true;
            this.tableLayoutPanel1.SetColumnSpan(this.groupControl1, 2);
            this.groupControl1.Controls.Add(this.tableLayoutPanel2);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(1, 1);
            this.groupControl1.Margin = new System.Windows.Forms.Padding(1);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(707, 302);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "[생산실적 이전 범위 설정]";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.1116F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.22099F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.22099F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.44642F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label6, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.StartDate, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.CycleTime, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.cbx_Kind, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.cbx_Worker, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.tbx_FileRoute, 2, 4);
            this.tableLayoutPanel2.Controls.Add(this.btn_find, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.EndDate, 4, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 23);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(703, 277);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(1, 1);
            this.label1.Margin = new System.Windows.Forms.Padding(1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 53);
            this.label1.TabIndex = 7;
            this.label1.Text = "생산실적시작날짜:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(1, 56);
            this.label2.Margin = new System.Windows.Forms.Padding(1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 53);
            this.label2.TabIndex = 8;
            this.label2.Text = "주기(minute):";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(1, 111);
            this.label3.Margin = new System.Windows.Forms.Padding(1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 53);
            this.label3.TabIndex = 9;
            this.label3.Text = "종류:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(373, 1);
            this.label6.Margin = new System.Windows.Forms.Padding(1);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 53);
            this.label6.TabIndex = 12;
            this.label6.Text = "~";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(1, 166);
            this.label5.Margin = new System.Windows.Forms.Padding(1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 53);
            this.label5.TabIndex = 11;
            this.label5.Text = "작업자:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(1, 221);
            this.label4.Margin = new System.Windows.Forms.Padding(1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 55);
            this.label4.TabIndex = 10;
            this.label4.Text = "db File 경로:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // StartDate
            // 
            this.StartDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.StartDate, 2);
            this.StartDate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartDate.Location = new System.Drawing.Point(75, 12);
            this.StartDate.Margin = new System.Windows.Forms.Padding(1);
            this.StartDate.Name = "StartDate";
            this.StartDate.Size = new System.Drawing.Size(296, 30);
            this.StartDate.TabIndex = 5;
            // 
            // CycleTime
            // 
            this.CycleTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.CycleTime, 2);
            this.CycleTime.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CycleTime.Location = new System.Drawing.Point(77, 67);
            this.CycleTime.Maximum = new decimal(new int[] {
            10800,
            0,
            0,
            0});
            this.CycleTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CycleTime.Name = "CycleTime";
            this.CycleTime.Size = new System.Drawing.Size(292, 30);
            this.CycleTime.TabIndex = 17;
            this.CycleTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cbx_Kind
            // 
            this.cbx_Kind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.cbx_Kind, 2);
            this.cbx_Kind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_Kind.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbx_Kind.FormattingEnabled = true;
            this.cbx_Kind.Location = new System.Drawing.Point(75, 122);
            this.cbx_Kind.Margin = new System.Windows.Forms.Padding(1);
            this.cbx_Kind.Name = "cbx_Kind";
            this.cbx_Kind.Size = new System.Drawing.Size(296, 31);
            this.cbx_Kind.TabIndex = 14;
            // 
            // cbx_Worker
            // 
            this.cbx_Worker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.cbx_Worker, 2);
            this.cbx_Worker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_Worker.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbx_Worker.FormattingEnabled = true;
            this.cbx_Worker.Location = new System.Drawing.Point(75, 177);
            this.cbx_Worker.Margin = new System.Windows.Forms.Padding(1);
            this.cbx_Worker.Name = "cbx_Worker";
            this.cbx_Worker.Size = new System.Drawing.Size(296, 31);
            this.cbx_Worker.TabIndex = 15;
            // 
            // tbx_FileRoute
            // 
            this.tbx_FileRoute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.tbx_FileRoute, 3);
            this.tbx_FileRoute.Location = new System.Drawing.Point(224, 237);
            this.tbx_FileRoute.Margin = new System.Windows.Forms.Padding(1);
            this.tbx_FileRoute.Name = "tbx_FileRoute";
            this.tbx_FileRoute.Size = new System.Drawing.Size(478, 22);
            this.tbx_FileRoute.TabIndex = 16;
            // 
            // btn_find
            // 
            this.btn_find.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_find.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_find.Location = new System.Drawing.Point(77, 233);
            this.btn_find.Name = "btn_find";
            this.btn_find.Size = new System.Drawing.Size(143, 30);
            this.btn_find.TabIndex = 18;
            this.btn_find.Text = "..찾아보기";
            this.btn_find.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_find.UseVisualStyleBackColor = true;
            // 
            // EndDate
            // 
            this.EndDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.EndDate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EndDate.Location = new System.Drawing.Point(405, 12);
            this.EndDate.Name = "EndDate";
            this.EndDate.Size = new System.Drawing.Size(295, 30);
            this.EndDate.TabIndex = 19;
            // 
            // groupControl2
            // 
            this.groupControl2.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupControl2.AppearanceCaption.Options.UseFont = true;
            this.tableLayoutPanel1.SetColumnSpan(this.groupControl2, 2);
            this.groupControl2.Controls.Add(this.grid_State);
            this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl2.Location = new System.Drawing.Point(1, 305);
            this.groupControl2.Margin = new System.Windows.Forms.Padding(1);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(707, 455);
            this.groupControl2.TabIndex = 3;
            this.groupControl2.Text = "[이전될 생산실적 이력]";
            // 
            // grid_State
            // 
            this.grid_State.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid_State.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_State.Location = new System.Drawing.Point(2, 23);
            this.grid_State.Name = "grid_State";
            this.grid_State.RowTemplate.Height = 23;
            this.grid_State.Size = new System.Drawing.Size(703, 430);
            this.grid_State.TabIndex = 0;
            // 
            // groupcon
            // 
            this.groupcon.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupcon.AppearanceCaption.Options.UseFont = true;
            this.tableLayoutPanel1.SetColumnSpan(this.groupcon, 2);
            this.groupcon.Controls.Add(this.grid_State2);
            this.groupcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupcon.Location = new System.Drawing.Point(710, 305);
            this.groupcon.Margin = new System.Windows.Forms.Padding(1);
            this.groupcon.Name = "groupcon";
            this.groupcon.Size = new System.Drawing.Size(473, 455);
            this.groupcon.TabIndex = 4;
            this.groupcon.Text = "[생산실적 이전 이력]";
            // 
            // grid_State2
            // 
            this.grid_State2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid_State2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_State2.Location = new System.Drawing.Point(2, 23);
            this.grid_State2.Name = "grid_State2";
            this.grid_State2.RowTemplate.Height = 23;
            this.grid_State2.Size = new System.Drawing.Size(469, 430);
            this.grid_State2.TabIndex = 0;
            // 
            // procTimer
            // 
            this.procTimer.Interval = 3000;
            // 
            // btn_Status
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.btn_Status, 2);
            this.btn_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Status.Font = new System.Drawing.Font("굴림", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Status.Location = new System.Drawing.Point(739, 30);
            this.btn_Status.Margin = new System.Windows.Forms.Padding(30);
            this.btn_Status.Name = "btn_Status";
            this.btn_Status.Size = new System.Drawing.Size(415, 244);
            this.btn_Status.TabIndex = 5;
            this.btn_Status.Text = "전송";
            this.btn_Status.UseVisualStyleBackColor = true;
            // 
            // PRODUCTION_ANALYSIS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PRODUCTION_ANALYSIS";
            this.Text = "COMBI_MANAGE_TOOL";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CycleTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid_State)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupcon)).EndInit();
            this.groupcon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid_State2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.GroupControl groupcon;
        private System.Windows.Forms.DateTimePicker StartDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbx_Kind;
        private System.Windows.Forms.ComboBox cbx_Worker;
        private System.Windows.Forms.TextBox tbx_FileRoute;
        private System.Windows.Forms.DataGridView grid_State;
        private System.Windows.Forms.DataGridView grid_State2;
        private System.Windows.Forms.Timer procTimer;
        private System.Windows.Forms.NumericUpDown CycleTime;
        private System.Windows.Forms.Button btn_find;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker EndDate;
        private System.Windows.Forms.Button btn_Status;
    }
}