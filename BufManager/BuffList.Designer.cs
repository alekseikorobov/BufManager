namespace BufManager
{
    partial class BuffList
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
            this.textList = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textList
            // 
            this.textList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textList.Location = new System.Drawing.Point(0, 0);
            this.textList.Multiline = true;
            this.textList.Name = "textList";
            this.textList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textList.Size = new System.Drawing.Size(284, 262);
            this.textList.TabIndex = 0;
            // 
            // BuffList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.textList);
            this.Name = "BuffList";
            this.Text = "BuffList";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox textList;
    }
}