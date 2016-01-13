namespace STKSIMUL
{
    partial class STK2D
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(STK2D));
            this.axAgUiAx2DCntrl1 = new AxAGI.STKX.AxAgUiAx2DCntrl();
            ((System.ComponentModel.ISupportInitialize)(this.axAgUiAx2DCntrl1)).BeginInit();
            this.SuspendLayout();
            // 
            // axAgUiAx2DCntrl1
            // 
            this.axAgUiAx2DCntrl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axAgUiAx2DCntrl1.Enabled = true;
            this.axAgUiAx2DCntrl1.Location = new System.Drawing.Point(0, 0);
            this.axAgUiAx2DCntrl1.Name = "axAgUiAx2DCntrl1";
            this.axAgUiAx2DCntrl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAgUiAx2DCntrl1.OcxState")));
            this.axAgUiAx2DCntrl1.Size = new System.Drawing.Size(608, 442);
            this.axAgUiAx2DCntrl1.TabIndex = 0;
            // 
            // STK2D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 442);
            this.Controls.Add(this.axAgUiAx2DCntrl1);
            this.Name = "STK2D";
            this.Text = "2D视图窗口";
            ((System.ComponentModel.ISupportInitialize)(this.axAgUiAx2DCntrl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxAGI.STKX.AxAgUiAx2DCntrl axAgUiAx2DCntrl1;


    }
}