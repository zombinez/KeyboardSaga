
namespace KeyboardSagaGame
{
    partial class Tutorial
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pause;
        private System.Windows.Forms.PictureBox menu;
        private System.Windows.Forms.Timer timer;

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