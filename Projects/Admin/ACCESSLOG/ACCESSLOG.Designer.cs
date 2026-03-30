namespace RAZER_C
{
    partial class ACCESSLOG
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
            this.pnl_Top = new System.Windows.Forms.Panel();
            this.lblLoginTitle = new DevExpress.XtraEditors.LabelControl();
            this.lblMenuTitle = new DevExpress.XtraEditors.LabelControl();
            this.btn_Search = new DevExpress.XtraEditors.SimpleButton();
            this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
            this.gridControlLogin = new DevExpress.XtraGrid.GridControl();
            this.gridViewLogin = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridControlMenu = new DevExpress.XtraGrid.GridControl();
            this.gridViewMenu = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.pnl_Top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
            this.splitContainerControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMenu)).BeginInit();
            this.SuspendLayout();
            //
            // pnl_Top
            //
            this.pnl_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.pnl_Top.Controls.Add(this.btn_Search);
            this.pnl_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_Top.Location = new System.Drawing.Point(0, 0);
            this.pnl_Top.Name = "pnl_Top";
            this.pnl_Top.Size = new System.Drawing.Size(1400, 60);
            this.pnl_Top.TabIndex = 0;
            //
            // btn_Search
            //
            this.btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Search.Appearance.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
            this.btn_Search.Appearance.Options.UseFont = true;
            this.btn_Search.Location = new System.Drawing.Point(1294, 12);
            this.btn_Search.Name = "btn_Search";
            this.btn_Search.Size = new System.Drawing.Size(88, 36);
            this.btn_Search.TabIndex = 0;
            this.btn_Search.Text = "새로고침";
            //
            // splitContainerControl
            //
            this.splitContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl.Horizontal = false;
            this.splitContainerControl.Location = new System.Drawing.Point(0, 60);
            this.splitContainerControl.Name = "splitContainerControl";
            this.splitContainerControl.SplitterPosition = 340;
            //
            // splitContainerControl.Panel1
            //
            this.splitContainerControl.Panel1.Controls.Add(this.gridControlLogin);
            this.splitContainerControl.Panel1.Controls.Add(this.lblLoginTitle);
            this.splitContainerControl.Panel1.Text = "Panel1";
            //
            // splitContainerControl.Panel2
            //
            this.splitContainerControl.Panel2.Controls.Add(this.gridControlMenu);
            this.splitContainerControl.Panel2.Controls.Add(this.lblMenuTitle);
            this.splitContainerControl.Panel2.Text = "Panel2";
            this.splitContainerControl.Size = new System.Drawing.Size(1400, 700);
            this.splitContainerControl.TabIndex = 1;
            //
            // lblLoginTitle
            //
            this.lblLoginTitle.Appearance.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Bold);
            this.lblLoginTitle.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(80)))), ((int)(((byte)(145)))));
            this.lblLoginTitle.Appearance.Options.UseFont = true;
            this.lblLoginTitle.Appearance.Options.UseForeColor = true;
            this.lblLoginTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLoginTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblLoginTitle.Location = new System.Drawing.Point(0, 0);
            this.lblLoginTitle.Name = "lblLoginTitle";
            this.lblLoginTitle.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblLoginTitle.Size = new System.Drawing.Size(1400, 30);
            this.lblLoginTitle.TabIndex = 0;
            this.lblLoginTitle.Text = "▶ 로그인 접속 기록";
            //
            // lblMenuTitle
            //
            this.lblMenuTitle.Appearance.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Bold);
            this.lblMenuTitle.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(130)))), ((int)(((byte)(32)))));
            this.lblMenuTitle.Appearance.Options.UseFont = true;
            this.lblMenuTitle.Appearance.Options.UseForeColor = true;
            this.lblMenuTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMenuTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblMenuTitle.Location = new System.Drawing.Point(0, 0);
            this.lblMenuTitle.Name = "lblMenuTitle";
            this.lblMenuTitle.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblMenuTitle.Size = new System.Drawing.Size(1400, 30);
            this.lblMenuTitle.TabIndex = 0;
            this.lblMenuTitle.Text = "▶ 메뉴 접속 기록";
            //
            // gridControlLogin
            //
            this.gridControlLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlLogin.Location = new System.Drawing.Point(0, 30);
            this.gridControlLogin.MainView = this.gridViewLogin;
            this.gridControlLogin.Name = "gridControlLogin";
            this.gridControlLogin.Size = new System.Drawing.Size(1400, 310);
            this.gridControlLogin.TabIndex = 1;
            this.gridControlLogin.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewLogin});
            //
            // gridViewLogin
            //
            this.gridViewLogin.GridControl = this.gridControlLogin;
            this.gridViewLogin.Name = "gridViewLogin";
            this.gridViewLogin.OptionsView.ShowGroupPanel = false;
            this.gridViewLogin.OptionsView.ShowIndicator = false;
            this.gridViewLogin.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewLogin.OptionsView.RowAutoHeight = true;
            this.gridViewLogin.OptionsBehavior.Editable = false;
            this.gridViewLogin.RowHeight = 26;
            //
            // gridControlMenu
            //
            this.gridControlMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlMenu.Location = new System.Drawing.Point(0, 30);
            this.gridControlMenu.MainView = this.gridViewMenu;
            this.gridControlMenu.Name = "gridControlMenu";
            this.gridControlMenu.Size = new System.Drawing.Size(1400, 330);
            this.gridControlMenu.TabIndex = 1;
            this.gridControlMenu.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMenu});
            //
            // gridViewMenu
            //
            this.gridViewMenu.GridControl = this.gridControlMenu;
            this.gridViewMenu.Name = "gridViewMenu";
            this.gridViewMenu.OptionsView.ShowGroupPanel = false;
            this.gridViewMenu.OptionsView.ShowIndicator = false;
            this.gridViewMenu.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewMenu.OptionsView.RowAutoHeight = true;
            this.gridViewMenu.OptionsBehavior.Editable = false;
            this.gridViewMenu.RowHeight = 26;
            //
            // ACCESSLOG
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 760);
            this.Controls.Add(this.splitContainerControl);
            this.Controls.Add(this.pnl_Top);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ACCESSLOG";
            this.Text = "접속 기록";
            this.pnl_Top.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
            this.splitContainerControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMenu)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnl_Top;
        private DevExpress.XtraEditors.SimpleButton btn_Search;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl;
        private DevExpress.XtraEditors.LabelControl lblLoginTitle;
        private DevExpress.XtraEditors.LabelControl lblMenuTitle;
        private DevExpress.XtraGrid.GridControl gridControlLogin;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewLogin;
        private DevExpress.XtraGrid.GridControl gridControlMenu;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMenu;
    }
}
