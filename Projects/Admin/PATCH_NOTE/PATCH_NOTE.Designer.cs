namespace COMBINATION
{
    partial class PATCH_NOTE
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
            this.patchBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // patchBox
            // 
            this.patchBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patchBox.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.patchBox.Location = new System.Drawing.Point(0, 0);
            this.patchBox.Margin = new System.Windows.Forms.Padding(10);
            this.patchBox.Name = "patchBox";
            this.patchBox.ReadOnly = true;
            this.patchBox.Size = new System.Drawing.Size(1184, 761);
            this.patchBox.TabIndex = 0;
            this.patchBox.Text = "";
            // 
            // PATCH_NOTE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Controls.Add(this.patchBox);
            this.Name = "PATCH_NOTE";
            this.Text = "PATCH_NOTE";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox patchBox;
    }
}