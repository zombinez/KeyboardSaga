using KeyboardSagaGame.Classes;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using WMPLib;

namespace KeyboardSagaGame
{
    public partial class Menu : Form
    {
        private Image image;
        private Vector coordinates;
        private KeyboardSaga gameForm;
        private Tutorial tutorialForm;
        private int animationCycles;
        public bool SoundOn { get; private set; }
        public readonly PrivateFontCollection FontCollection;
        private readonly WindowsMediaPlayer music;
        private readonly WindowsMediaPlayer clickSound;

        public Menu()
        {
            SoundOn = true;
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            #region Sound Files Initialization
            var soundsFolder = Path.Combine(currentDirectory, @"sounds");
            if (!Directory.Exists(soundsFolder))
                Directory.CreateDirectory(soundsFolder);
            string musicFile = Path.Combine(currentDirectory, @"sounds\music.wav");
            FileInitialize(musicFile, Properties.Resources.music);
            string clickSoundFile = Path.Combine(currentDirectory, @"sounds\click.wav");
            FileInitialize(clickSoundFile, Properties.Resources.menu_select);
            FileInitialize(Path.Combine(currentDirectory, @"sounds\heal.wav"), Properties.Resources.heal);
            FileInitialize(Path.Combine(currentDirectory, @"sounds\heal_recharge.wav"), Properties.Resources.heal_recharge);
            FileInitialize(Path.Combine(currentDirectory, @"sounds\hit.wav"), Properties.Resources.hit);
            FileInitialize(Path.Combine(currentDirectory, @"sounds\game_over.wav"), Properties.Resources.game_over);
            //Music Player Creation
            music = new WindowsMediaPlayer();
            music.URL = musicFile;
            music.controls.stop();
            music.settings.setMode("loop", true);
            music.settings.volume = 10;
            if(SoundOn)
                music.controls.play();
            //Click Sound Player Creation
            clickSound = new WindowsMediaPlayer();
            clickSound.URL = clickSoundFile;
            clickSound.settings.volume = 10;
            clickSound.controls.stop();
            #endregion
            #region Font File Initialization
            string fontFile = Path.Combine(currentDirectory, @"fontFile.tmp");
            FileInitialize(fontFile, Properties.Resources.Main_Font);
            //Font Creation
            FontCollection = new PrivateFontCollection();
            FontCollection.AddFontFile(fontFile);
            #endregion
            //Form Initialization
            InitializeComponent();
            Icon = Properties.Resources.logo;
            image = GameMethods.GetImageByName("menu_0");
            FormBorderStyle = FormBorderStyle.None;
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoValidate = AutoValidate.Disable;
            Name = "KeyboardSaga";
            Text = "eyboardSaga";
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;
            ClientSize = new Size(1280, 720);
            BackColor = Color.FromArgb(47, 40, 58);
            coordinates = new Vector((ClientSize.Width - image.Width) / 2, (ClientSize.Height - image.Height) / 2);
            SizeChanged += new EventHandler((sender, args) =>
            {
                coordinates = new Vector((ClientSize.Width - image.Width) / 2, (ClientSize.Height - image.Height) / 2);
                play.Location = new Point((int)(coordinates.X + image.Width / 2 - play.Image.Width / 2), 
                                          (int)(ClientSize.Height / 2 - play.Image.Height));
                tutorial.Location = new Point(play.Location.X, play.Location.Y + 15 + play.Image.Height);
                exit.Location = new Point(tutorial.Location.X, tutorial.Location.Y + 15 + tutorial.Image.Height);
                soundToggle.Location = new Point((int)(coordinates.X + 41), 
                                                 (int)(coordinates.Y + image.Height - 41 - soundToggle.Image.Height));
                Invalidate();
            });
            Paint += new PaintEventHandler(OnPaint);
            //Timer
            timer = new Timer(components)
            {
                Enabled = true,
                Interval = 150
            };
            timer.Tick += new EventHandler(OnFrameChanged);
            animationCycles = 0;
            #region Play Button
            play = new PictureBox();
            play.Image = GameMethods.GetImageByName("MENU_PLAY");
            play.Size = new Size(play.Image.Width, play.Image.Height);
            play.Location = new Point((int)(coordinates.X + image.Width / 2 - play.Image.Width / 2), 
                                      (int)(ClientSize.Height / 2 - play.Image.Height));
            play.MouseUp += new MouseEventHandler((sender, args) => 
                play.Image = GameMethods.GetImageByName("MENU_PLAY_ACTIVE"));
            play.MouseDown += new MouseEventHandler((sender, args) =>
                play.Image = GameMethods.GetImageByName("MENU_PLAY_PRESSED"));
            play.MouseClick += new MouseEventHandler((sender, args) =>
            {
                PlayClickSound();
                gameForm = new KeyboardSaga(this);
                gameForm.Show();
            });
            play.MouseEnter += new EventHandler((sender, args) => 
                play.Image = GameMethods.GetImageByName("MENU_PLAY_ACTIVE"));
            play.MouseLeave += new EventHandler((sender, args) => 
                play.Image = GameMethods.GetImageByName("MENU_PLAY"));
            play.BackColor = Color.Transparent;
            #endregion
            Controls.Add(play);
            #region Tutorial Button
            tutorial = new PictureBox();
            tutorial.Image = GameMethods.GetImageByName("MENU_TUTORIAL");
            tutorial.Size = new Size(tutorial.Image.Width, tutorial.Image.Height);
            tutorial.Location = new Point(play.Location.X, play.Location.Y + 15 + play.Image.Height);
            tutorial.MouseUp += new MouseEventHandler((sender, args) => 
                tutorial.Image = GameMethods.GetImageByName("MENU_TUTORIAL_ACTIVE"));
            tutorial.MouseDown += new MouseEventHandler((sender, args) =>
                tutorial.Image = GameMethods.GetImageByName("MENU_TUTORIAL_PRESSED"));
            tutorial.MouseClick += new MouseEventHandler((sender, args) =>
            {
                PlayClickSound();
                tutorialForm = new Tutorial(this);
                tutorialForm.Show();
            });
            tutorial.MouseEnter += new EventHandler((sender, args) => 
                tutorial.Image = GameMethods.GetImageByName("MENU_TUTORIAL_ACTIVE"));
            tutorial.MouseLeave += new EventHandler((sender, args) => 
                tutorial.Image = GameMethods.GetImageByName("MENU_TUTORIAL"));
            tutorial.BackColor = Color.Transparent;
            #endregion
            Controls.Add(tutorial);
            #region Exit Game Button
            exit = new PictureBox();
            exit.Image = GameMethods.GetImageByName("MENU_EXIT");
            exit.Size = new Size(exit.Image.Width, exit.Image.Height);
            exit.Location = new Point(tutorial.Location.X, tutorial.Location.Y + 15 + tutorial.Image.Height);
            exit.MouseUp += new MouseEventHandler((sender, ars) => 
                exit.Image = GameMethods.GetImageByName("MENU_EXIT_ACTIVE"));
            exit.MouseDown += new MouseEventHandler((sender, args) =>
                exit.Image = GameMethods.GetImageByName("MENU_EXIT_PRESSED"));
            exit.MouseClick += new MouseEventHandler((sender, args) =>
            {
                Application.Exit();
            });
            exit.MouseEnter += new EventHandler((sender, args) => 
                exit.Image = GameMethods.GetImageByName("MENU_EXIT_ACTIVE"));
            exit.MouseLeave += 
                new EventHandler((sender, args) => exit.Image = GameMethods.GetImageByName("MENU_EXIT"));
            exit.BackColor = Color.Transparent;
            #endregion
            Controls.Add(exit);
            #region Sound Toggle Button
            soundToggle = new PictureBox();
            soundToggle.Image = SoundOn ? GameMethods.GetImageByName("SOUND_ON") 
                                        : GameMethods.GetImageByName("SOUND_OFF");
            soundToggle.Size = new Size(soundToggle.Image.Width, soundToggle.Image.Height);
            soundToggle.Location = new Point((int)(coordinates.X + 41), 
                                             (int)(coordinates.Y + image.Height - 41 - soundToggle.Image.Height));
            soundToggle.MouseUp += new MouseEventHandler((sender, args) =>
                soundToggle.Image = SoundOn ? GameMethods.GetImageByName("SOUND_ON_ACTIVE") 
                                            : GameMethods.GetImageByName("SOUND_OFF_ACTIVE"));
            soundToggle.MouseDown += new MouseEventHandler((sender, args) =>
                soundToggle.Image = SoundOn ? GameMethods.GetImageByName("SOUND_ON_PRESSED")
                                            : GameMethods.GetImageByName("SOUND_OFF_PRESSED"));
            soundToggle.MouseClick += new MouseEventHandler((sender, args) =>
            {
                PlayClickSound();
                SoundOn = !SoundOn;
                if (SoundOn)
                    music.controls.play();
                else music.controls.pause();
            });
            soundToggle.MouseEnter += new EventHandler((sender, args) =>
                soundToggle.Image = SoundOn ? GameMethods.GetImageByName("SOUND_ON_ACTIVE")
                                            : GameMethods.GetImageByName("SOUND_OFF_ACTIVE"));
            soundToggle.MouseLeave += new EventHandler((sender, args) =>
                soundToggle.Image = SoundOn ? GameMethods.GetImageByName("SOUND_ON")
                                            : GameMethods.GetImageByName("SOUND_OFF"));
            soundToggle.BackColor = Color.Transparent;
            #endregion
            Controls.Add(soundToggle);
            Invalidate();
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if(animationCycles < 40)
            {
                if (animationCycles % 10 == 0 && animationCycles != 0)
                    image = GameMethods.GetImageByName($"menu_{animationCycles / 10}");
                animationCycles++;
            }
            else
            {
                animationCycles = 0;
                image = GameMethods.GetImageByName($"menu_0");
            }
            e.Graphics.DrawImage(image, (float)coordinates.X, (float)coordinates.Y, image.Width, image.Height);
        }

        public void Open(Form form)
        {
            Show();
            Enabled = true;
            form.Enabled = false;
            form.Hide();
            form.Close();
        }

        private void FileInitialize(string fileName, byte[] fileBytes)
        {
            if (!File.Exists(fileName))
                File.WriteAllBytes(fileName, fileBytes);
        }

        public void PlayClickSound()
        {
            if (SoundOn)
                clickSound.controls.play();
        }
    }
}
