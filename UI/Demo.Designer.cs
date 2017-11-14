using System.Windows.Forms;

namespace TriangulatedPolygonAStar.UI
{
    partial class Demo
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
            this.display = new TriangulatedPolygonAStar.UI.Canvas();
            this.SuspendLayout();
            // 
            // display
            // 
//            this.display.BackColor = System.Drawing.Color.White;
            this.display.Location = new System.Drawing.Point(0, 0);
            this.display.Name = "display";
            this.display.Size = new System.Drawing.Size(200, 200);
            this.display.TabIndex = 50;
            this.display.Dock = DockStyle.Fill;
            this.display.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DisplayOnMouseDown);
            this.display.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DisplayOnMouseMove);
            this.display.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DisplayOnMouseUp);
            // 
            // Demo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 670);
            this.Controls.Add(this.display);
            this.Name = "Demo";
            this.Text = "Triangulated Polygon A-star demo";
//            this.BackColor = System.Drawing.Color.White;
            this.Load += new System.EventHandler(this.DemoOnLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TriangulatedPolygonAStar.UI.Canvas display;
    }
}