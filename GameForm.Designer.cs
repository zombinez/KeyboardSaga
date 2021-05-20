using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace KeyboardSagaGame
{
    partial class KeyboardSaga
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.PictureBox pause;
        private System.Windows.Forms.PictureBox restart;
        private System.Windows.Forms.PictureBox menu;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}