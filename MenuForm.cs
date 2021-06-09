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
        private Point coordinates;
        private KeyboardSaga gameForm;
        private Tutorial tutorialForm;
        private int animationCycles;
        public bool SoundOn { get; private set; }
        public readonly PrivateFontCollection FontCollection;
        private readonly WindowsMediaPlayer music;
        private readonly WindowsMediaPlayer clickSound;
        private readonly Timer timer;
        private readonly PictureBox playButton;
        private readonly PictureBox tutorialButton;
        private readonly PictureBox exitButton;
        private readonly PictureBox soundToggleButton;

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
            coordinates = new Point((ClientSize.Width - image.Width) / 2, (ClientSize.Height - image.Height) / 2);
            SizeChanged += new EventHandler((sender, args) =>
            {
                coordinates = new Point((ClientSize.Width - image.Width) / 2, (ClientSize.Height - image.Height) / 2);
                playButton.Location = 
                    new Point((coordinates.X + image.Width / 2 - playButton.Image.Width / 2), 
                              (ClientSize.Height / 2 - playButton.Image.Height));
                tutorialButton.Location = 
                    new Point(playButton.Location.X, playButton.Location.Y + 15 + playButton.Image.Height);
                exitButton.Location = 
                    new Point(tutorialButton.Location.X, tutorialButton.Location.Y + 15 + tutorialButton.Image.Height);
                soundToggleButton.Location = 
                    new Point((coordinates.X + 41), 
                              (coordinates.Y + image.Height - 41 - soundToggleButton.Image.Height));
                Invalidate();
            });
            Paint += new PaintEventHandler(OnPaint);
            //Timer
            timer = new Timer(components)
            {
                Enabled = true,
                Interval = 100
            };
            timer.Tick += new EventHandler(OnFrameChanged);
            animationCycles = 0;
            #region Play Button
            playButton = new PictureBox();
            playButton.Image = GameMethods.GetImageByName("MENU_PLAY");
            playButton.Size = new Size(playButton.Image.Width, playButton.Image.Height);
            playButton.Location = new Point((coordinates.X + image.Width / 2 - playButton.Image.Width / 2), 
                                      (ClientSize.Height / 2 - playButton.Image.Height));
            playButton.MouseUp += new MouseEventHandler((sender, args) => 
                playButton.Image = GameMethods.GetImageByName("MENU_PLAY_ACTIVE"));
            playButton.MouseDown += new MouseEventHandler((sender, args) =>
                playButton.Image = GameMethods.GetImageByName("MENU_PLAY_PRESSED"));
            playButton.MouseClick += new MouseEventHandler((sender, args) =>
            {
                PlayClickSound();
                gameForm = new KeyboardSaga(this);
                gameForm.Show();
            });
            playButton.MouseEnter += new EventHandler((sender, args) => 
                playButton.Image = GameMethods.GetImageByName("MENU_PLAY_ACTIVE"));
            playButton.MouseLeave += new EventHandler((sender, args) => 
                playButton.Image = GameMethods.GetImageByName("MENU_PLAY"));
            playButton.BackColor = Color.Transparent;
            #endregion
            Controls.Add(playButton);
            #region Tutorial Button
            tutorialButton = new PictureBox();
            tutorialButton.Image = GameMethods.GetImageByName("MENU_TUTORIAL");
            tutorialButton.Size = new Size(tutorialButton.Image.Width, tutorialButton.Image.Height);
            tutorialButton.Location = 
                new Point(playButton.Location.X, playButton.Location.Y + 15 + playButton.Image.Height);
            tutorialButton.MouseUp += new MouseEventHandler((sender, args) => 
                tutorialButton.Image = GameMethods.GetImageByName("MENU_TUTORIAL_ACTIVE"));
            tutorialButton.MouseDown += new MouseEventHandler((sender, args) =>
                tutorialButton.Image = GameMethods.GetImageByName("MENU_TUTORIAL_PRESSED"));
            tutorialButton.MouseClick += new MouseEventHandler((sender, args) =>
            {
                PlayClickSound();
                tutorialForm = new Tutorial(this);
                tutorialForm.Show();
            });
            tutorialButton.MouseEnter += new EventHandler((sender, args) => 
                tutorialButton.Image = GameMethods.GetImageByName("MENU_TUTORIAL_ACTIVE"));
            tutorialButton.MouseLeave += new EventHandler((sender, args) => 
                tutorialButton.Image = GameMethods.GetImageByName("MENU_TUTORIAL"));
            tutorialButton.BackColor = Color.Transparent;
            #endregion
            Controls.Add(tutorialButton);
            #region Exit Game Button
            exitButton = new PictureBox();
            exitButton.Image = GameMethods.GetImageByName("MENU_EXIT");
            exitButton.Size = new Size(exitButton.Image.Width, exitButton.Image.Height);
            exitButton.Location = 
                new Point(tutorialButton.Location.X, tutorialButton.Location.Y + 15 + tutorialButton.Image.Height);
            exitButton.MouseUp += new MouseEventHandler((sender, ars) => 
                exitButton.Image = GameMethods.GetImageByName("MENU_EXIT_ACTIVE"));
            exitButton.MouseDown += new MouseEventHandler((sender, args) =>
                exitButton.Image = GameMethods.GetImageByName("MENU_EXIT_PRESSED"));
            exitButton.MouseClick += new MouseEventHandler((sender, args) =>
            {
                Application.Exit();
            });
            exitButton.MouseEnter += new EventHandler((sender, args) => 
                exitButton.Image = GameMethods.GetImageByName("MENU_EXIT_ACTIVE"));
            exitButton.MouseLeave += 
                new EventHandler((sender, args) => exitButton.Image = GameMethods.GetImageByName("MENU_EXIT"));
            exitButton.BackColor = Color.Transparent;
            #endregion
            Controls.Add(exitButton);
            #region Sound Toggle Button
            soundToggleButton = new PictureBox();
            soundToggleButton.Image = SoundOn ? GameMethods.GetImageByName("SOUND_ON") 
                                        : GameMethods.GetImageByName("SOUND_OFF");
            soundToggleButton.Size = new Size(soundToggleButton.Image.Width, soundToggleButton.Image.Height);
            soundToggleButton.Location = new Point((coordinates.X + 41), 
                                             (coordinates.Y + image.Height - 41 - soundToggleButton.Image.Height));
            soundToggleButton.MouseUp += new MouseEventHandler((sender, args) =>
                soundToggleButton.Image = SoundOn ? GameMethods.GetImageByName("SOUND_ON_ACTIVE") 
                                            : GameMethods.GetImageByName("SOUND_OFF_ACTIVE"));
            soundToggleButton.MouseDown += new MouseEventHandler((sender, args) =>
                soundToggleButton.Image = SoundOn ? GameMethods.GetImageByName("SOUND_ON_PRESSED")
                                            : GameMethods.GetImageByName("SOUND_OFF_PRESSED"));
            soundToggleButton.MouseClick += new MouseEventHandler((sender, args) =>
            {
                PlayClickSound();
                SoundOn = !SoundOn;
                if (SoundOn)
                    music.controls.play();
                else music.controls.pause();
            });
            soundToggleButton.MouseEnter += new EventHandler((sender, args) =>
                soundToggleButton.Image = SoundOn ? GameMethods.GetImageByName("SOUND_ON_ACTIVE")
                                            : GameMethods.GetImageByName("SOUND_OFF_ACTIVE"));
            soundToggleButton.MouseLeave += new EventHandler((sender, args) =>
                soundToggleButton.Image = SoundOn ? GameMethods.GetImageByName("SOUND_ON")
                                            : GameMethods.GetImageByName("SOUND_OFF"));
            soundToggleButton.BackColor = Color.Transparent;
            #endregion
            Controls.Add(soundToggleButton);
            Invalidate();
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            if (animationCycles < 12)
            {
                if (animationCycles % 3 == 0 && animationCycles != 0)
                    image = GameMethods.GetImageByName($"menu_{animationCycles / 3}");
                animationCycles++;
            }
            else
            {
                animationCycles = 0;
                image = GameMethods.GetImageByName($"menu_0");
            }
            Invalidate();
        }

        private void OnPaint(object sender, PaintEventArgs e) =>
            e.Graphics.DrawImage(image, coordinates.X, coordinates.Y, image.Width, image.Height);

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
