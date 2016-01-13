namespace STKSIMUL
{
    partial class STK3D
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(STK3D));
            this.axAgUiAxVOCntrl1 = new AxAGI.STKX.AxAgUiAxVOCntrl();
            ((System.ComponentModel.ISupportInitialize)(this.axAgUiAxVOCntrl1)).BeginInit();
            this.SuspendLayout();
            // 
            // axAgUiAxVOCntrl1
            // 
            this.axAgUiAxVOCntrl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axAgUiAxVOCntrl1.Enabled = true;
            this.axAgUiAxVOCntrl1.Location = new System.Drawing.Point(0, 0);
            this.axAgUiAxVOCntrl1.Name = "axAgUiAxVOCntrl1";
            this.axAgUiAxVOCntrl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAgUiAxVOCntrl1.OcxState")));
            this.axAgUiAxVOCntrl1.Size = new System.Drawing.Size(610, 434);
            this.axAgUiAxVOCntrl1.TabIndex = 0;
            // 
            // STK3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 434);
            this.Controls.Add(this.axAgUiAxVOCntrl1);
            this.Name = "STK3D";
            this.Text = "3D视图窗口";
            ((System.ComponentModel.ISupportInitialize)(this.axAgUiAxVOCntrl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxAGI.STKX.AxAgUiAxVOCntrl axAgUiAxVOCntrl1;


    }
}