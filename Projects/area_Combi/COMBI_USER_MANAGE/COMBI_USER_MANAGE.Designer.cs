namespace RAZER_C
{
    partial class COMBI_USER_MANAGE
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.splitMain = new DevExpress.XtraEditors.SplitContainerControl();
            this.xtraTabProcess = new DevExpress.XtraTab.XtraTabControl();
            this.tabInk = new DevExpress.XtraTab.XtraTabPage();
            this.tabWater = new DevExpress.XtraTab.XtraTabPage();
            this.tabSemi = new DevExpress.XtraTab.XtraTabPage();
            this.pnl_Top = new System.Windows.Forms.Panel();
            this.btn_Save = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Delete = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Add = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Search = new DevExpress.XtraEditors.SimpleButton();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain.Panel1)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain.Panel2)).BeginInit();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabProcess)).BeginInit();
            this.xtraTabProcess.SuspendLayout();
            this.pnl_Top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            //
            // splitMain
            //
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            this.splitMain.Size = new System.Drawing.Size(1200, 700);
            this.splitMain.SplitterPosition = 160;
            this.splitMain.TabIndex = 0;
            //
            // splitMain.Panel1 - 왼쪽 탭
            //
            this.splitMain.Panel1.Controls.Add(this.xtraTabProcess);
            this.splitMain.Panel1.MinSize = 140;
            //
            // splitMain.Panel2 - 오른쪽 그리드
            //
            this.splitMain.Panel2.Controls.Add(this.gridControl1);
            this.splitMain.Panel2.Controls.Add(this.pnl_Top);
            //
            // xtraTabProcess
            //
            this.xtraTabProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabProcess.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Left;
            this.xtraTabProcess.HeaderOrientation = DevExpress.XtraTab.TabOrientation.Horizontal;
            this.xtraTabProcess.Location = new System.Drawing.Point(0, 0);
            this.xtraTabProcess.Name = "xtraTabProcess";
            this.xtraTabProcess.SelectedTabPage = this.tabInk;
            this.xtraTabProcess.Size = new System.Drawing.Size(160, 700);
            this.xtraTabProcess.TabIndex = 0;
            this.xtraTabProcess.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
                this.tabInk,
                this.tabWater,
                this.tabSemi
            });
            //
            // tabInk
            //
            this.tabInk.Name = "tabInk";
            this.tabInk.Text = "잉크";
            //
            // tabWater
            //
            this.tabWater.Name = "tabWater";
            this.tabWater.Text = "용수";
            //
            // tabSemi
            //
            this.tabSemi.Name = "tabSemi";
            this.tabSemi.Text = "반제품";
            //
            // pnl_Top
            //
            this.pnl_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(80)))), ((int)(((byte)(145)))));
            this.pnl_Top.Controls.Add(this.btn_Save);
            this.pnl_Top.Controls.Add(this.btn_Delete);
            this.pnl_Top.Controls.Add(this.btn_Add);
            this.pnl_Top.Controls.Add(this.btn_Search);
            this.pnl_Top.Controls.Add(this.lblTitle);
            this.pnl_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_Top.Location = new System.Drawing.Point(0, 0);
            this.pnl_Top.Name = "pnl_Top";
            this.pnl_Top.Size = new System.Drawing.Size(1030, 50);
            this.pnl_Top.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.Appearance.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Appearance.Options.UseFont = true;
            this.lblTitle.Appearance.Options.UseForeColor = true;
            this.lblTitle.Location = new System.Drawing.Point(15, 13);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "원료배합 공정별 사용자 관리";
            //
            // btn_Search
            //
            this.btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Search.Appearance.BackColor = System.Drawing.Color.White;
            this.btn_Search.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btn_Search.Appearance.Options.UseBackColor = true;
            this.btn_Search.Appearance.Options.UseFont = true;
            this.btn_Search.Location = new System.Drawing.Point(638, 10);
            this.btn_Search.Name = "btn_Search";
            this.btn_Search.Size = new System.Drawing.Size(90, 30);
            this.btn_Search.TabIndex = 1;
            this.btn_Search.Text = "조회";
            //
            // btn_Add
            //
            this.btn_Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Add.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(130)))), ((int)(((byte)(32)))));
            this.btn_Add.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btn_Add.Appearance.ForeColor = System.Drawing.Color.White;
            this.btn_Add.Appearance.Options.UseBackColor = true;
            this.btn_Add.Appearance.Options.UseFont = true;
            this.btn_Add.Appearance.Options.UseForeColor = true;
            this.btn_Add.Location = new System.Drawing.Point(738, 10);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(90, 30);
            this.btn_Add.TabIndex = 2;
            this.btn_Add.Text = "추가";
            //
            // btn_Delete
            //
            this.btn_Delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Delete.Appearance.BackColor = System.Drawing.Color.IndianRed;
            this.btn_Delete.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btn_Delete.Appearance.ForeColor = System.Drawing.Color.White;
            this.btn_Delete.Appearance.Options.UseBackColor = true;
            this.btn_Delete.Appearance.Options.UseFont = true;
            this.btn_Delete.Appearance.Options.UseForeColor = true;
            this.btn_Delete.Location = new System.Drawing.Point(838, 10);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(90, 30);
            this.btn_Delete.TabIndex = 3;
            this.btn_Delete.Text = "삭제";
            //
            // btn_Save
            //
            this.btn_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(80)))), ((int)(((byte)(145)))));
            this.btn_Save.Appearance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btn_Save.Appearance.ForeColor = System.Drawing.Color.White;
            this.btn_Save.Appearance.Options.UseBackColor = true;
            this.btn_Save.Appearance.Options.UseFont = true;
            this.btn_Save.Appearance.Options.UseForeColor = true;
            this.btn_Save.Location = new System.Drawing.Point(938, 10);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(90, 30);
            this.btn_Save.TabIndex = 4;
            this.btn_Save.Text = "저장";
            //
            // gridControl1
            //
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 50);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1030, 650);
            this.gridControl1.TabIndex = 1;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { this.gridView1 });
            //
            // gridView1
            //
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.RowHeight = 28;
            //
            // COMBI_USER_MANAGE
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.splitMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "COMBI_USER_MANAGE";
            this.Text = "원료배합 공정별 사용자 관리";
            ((System.ComponentModel.ISupportInitialize)(this.splitMain.Panel1)).EndInit();
            this.splitMain.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain.Panel2)).EndInit();
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabProcess)).EndInit();
            this.xtraTabProcess.ResumeLayout(false);
            this.pnl_Top.ResumeLayout(false);
            this.pnl_Top.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitMain;
        private DevExpress.XtraTab.XtraTabControl xtraTabProcess;
        private DevExpress.XtraTab.XtraTabPage tabInk;
        private DevExpress.XtraTab.XtraTabPage tabWater;
        private DevExpress.XtraTab.XtraTabPage tabSemi;
        private System.Windows.Forms.Panel pnl_Top;
        private DevExpress.XtraEditors.LabelControl lblTitle;
        private DevExpress.XtraEditors.SimpleButton btn_Search;
        private DevExpress.XtraEditors.SimpleButton btn_Add;
        private DevExpress.XtraEditors.SimpleButton btn_Delete;
        private DevExpress.XtraEditors.SimpleButton btn_Save;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
    }
}
