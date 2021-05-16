
namespace KeyboardSagaGame
{
    partial class Menu
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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.PictureBox play;
        private System.Windows.Forms.PictureBox tutorial;
        private System.Windows.Forms.PictureBox exit;
        private System.Windows.Forms.PictureBox soundToggle;
    }
}