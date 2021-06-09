using KeyboardSagaGame.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using WMPLib;
using System.IO;
using System.Reflection;

namespace KeyboardSagaGame
{
    public partial class KeyboardSaga : Form
    {
        private Image mapImage;
        private Point coordinates;
        private int animationCycles;
        private int gameOverCycles;
        private bool pauseGame;
        private bool gameOverSoundPlayed;
        private readonly Menu menuForm;
        private readonly Game game;
        private readonly Font gameOverFont;
        private readonly WindowsMediaPlayer healRechargeSound;
        private readonly WindowsMediaPlayer healSound;
        private readonly WindowsMediaPlayer hitSound;
        private readonly WindowsMediaPlayer gameOverSound;
        private readonly Timer timer;
        private readonly PictureBox pauseButton;
        private readonly PictureBox restartButton;
        private readonly PictureBox menuButton;

        public KeyboardSaga(Menu menuF)
        {
            //Form Initialization
            InitializeComponent();
            Icon = Properties.Resources.logo;
            animationCycles = 0;
            gameOverCycles = 0;
            mapImage = GameMethods.GetImageByName("0_");
            menuForm = menuF;
            Font = new Font(menuForm.FontCollection.Families[0], 20F);
            gameOverFont = new Font(menuForm.FontCollection.Families[0], 40F);
            game = new Game();
            pauseGame = false;
            gameOverSoundPlayed = false;
            #region Sounds Initialization
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            healRechargeSound = new WindowsMediaPlayer();
            healRechargeSound.URL = Path.Combine(currentDirectory, @"sounds\heal_recharge.wav");
            healRechargeSound.settings.volume = 15;
            healRechargeSound.controls.stop();
            healSound = new WindowsMediaPlayer();
            healSound.URL = Path.Combine(currentDirectory, @"sounds\heal.wav");
            healSound.settings.volume = 15;
            healSound.controls.stop();
            hitSound = new WindowsMediaPlayer();
            hitSound.URL = Path.Combine(currentDirectory, @"sounds\hit.wav");
            hitSound.settings.volume = 10;
            hitSound.controls.stop();
            gameOverSound = new WindowsMediaPlayer();
            gameOverSound.URL = Path.Combine(currentDirectory, @"sounds\game_over.wav");
            gameOverSound.settings.volume = 100;
            gameOverSound.controls.stop();
            #endregion
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoValidate = AutoValidate.Disable;
            DoubleBuffered = true;
            Name = "KeyboardSaga";
            Text = "eyboardSaga";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            ClientSize = new Size(1280, 720);
            coordinates = new Point((ClientSize.Width - game.MapImage.Width) / 2, 
                                     (ClientSize.Height - game.MapImage.Height) / 2);
            BackColor = Color.FromArgb(47, 40, 58);
            Paint += new PaintEventHandler(OnPaint);
            KeyDown += new KeyEventHandler(OnKeyPress);
            SizeChanged += new EventHandler((sender, args) =>
            {
                coordinates = new Point((ClientSize.Width - game.MapImage.Width) / 2, 
                                         (ClientSize.Height - game.MapImage.Height) / 2);
                pauseButton.Location = new Point((ClientSize.Width - pauseButton.Image.Width - 30), 30);
                restartButton.Location = new Point(
                    (coordinates.X + game.MapImage.Width / 2 - restartButton.Image.Width / 2), 
                    (ClientSize.Height / 2 - restartButton.Image.Height));
                menuButton.Location = new Point((coordinates.X + game.MapImage.Width / 2 - menuButton.Image.Width / 2), 
                                                (restartButton.Location.Y + 15 + restartButton.Image.Height));
                Invalidate();
            });
            //Timer
            timer = new Timer(components)
            {
                Enabled = true,
                Interval = 20
            };
            timer.Tick += new EventHandler(OnFrameChanged);
            #region Pause Button
            pauseButton = new PictureBox();
            pauseButton.Image = GameMethods.GetImageByName("PAUSE");
            pauseButton.Size = new Size(pauseButton.Image.Width, pauseButton.Image.Height);
            pauseButton.Location = new Point((ClientSize.Width - pauseButton.Image.Width - 40), 20);
            pauseButton.MouseUp += new MouseEventHandler((sender, args) => 
                { if (!pauseGame) pauseButton.Image = GameMethods.GetImageByName("PAUSE_ACTIVE"); });
            pauseButton.MouseDown += new MouseEventHandler((sender, args) => 
                pauseButton.Image = GameMethods.GetImageByName("PAUSE_PRESSED_ACTIVE"));
            pauseButton.MouseClick += new MouseEventHandler((sender, args) =>
            {
                menuForm.PlayClickSound();
                pauseGame = !pauseGame;
                restartButton.Enabled = !restartButton.Enabled;
                restartButton.Visible = !restartButton.Visible;
                menuButton.Enabled = !menuButton.Enabled;
                menuButton.Visible = !menuButton.Visible;
            });
            pauseButton.MouseEnter += new EventHandler((sender, args) => 
                { if (!pauseGame) pauseButton.Image = GameMethods.GetImageByName("PAUSE_ACTIVE"); });
            pauseButton.MouseLeave += new EventHandler((sender, args) => 
                { if (!pauseGame) pauseButton.Image = GameMethods.GetImageByName("PAUSE"); });
            pauseButton.BackColor = Color.Transparent;
            #endregion
            Controls.Add(pauseButton);
            #region Restart Button
            restartButton = new PictureBox();
            restartButton.Image = GameMethods.GetImageByName("RESTART");
            restartButton.Size = new Size(restartButton.Image.Width, restartButton.Image.Height);
            restartButton.Location = new Point((coordinates.X + game.MapImage.Width / 2 - restartButton.Image.Width / 2), 
                                               (ClientSize.Height / 2 - restartButton.Image.Height));
            restartButton.MouseUp += new MouseEventHandler((sender, args) => 
                restartButton.Image = GameMethods.GetImageByName("RESTART_ACTIVE"));
            restartButton.MouseDown += new MouseEventHandler((sender, args) =>
                restartButton.Image = GameMethods.GetImageByName("RESTART_PRESSED"));
            restartButton.MouseClick += new MouseEventHandler((sender, args) =>
            {
                menuForm.PlayClickSound();
                game.Reset();
                animationCycles = 0;
                gameOverCycles = 0;
                gameOverSound.controls.stop();
                gameOverSoundPlayed = false;
                mapImage = GameMethods.GetImageByName("0_");
                PausePress();
            });
            restartButton.MouseEnter += new EventHandler((sender, args) => 
                restartButton.Image = GameMethods.GetImageByName("RESTART_ACTIVE"));
            restartButton.MouseLeave += new EventHandler((sender, args) => 
                restartButton.Image = GameMethods.GetImageByName("RESTART"));
            restartButton.BackColor = Color.Transparent;
            restartButton.Enabled = false;
            restartButton.Visible = false;
            #endregion
            Controls.Add(restartButton);
            #region Exit To Menu Button
            menuButton = new PictureBox();
            menuButton.Image = GameMethods.GetImageByName("EXIT");
            menuButton.Size = new Size(menuButton.Image.Width, menuButton.Image.Height);
            menuButton.Location = new Point((coordinates.X + game.MapImage.Width / 2 - menuButton.Image.Width / 2), 
                                            (restartButton.Location.Y + 15 + restartButton.Image.Height));
            menuButton.MouseUp += new MouseEventHandler((sender, args) => 
                menuButton.Image = GameMethods.GetImageByName("EXIT_ACTIVE"));
            menuButton.MouseDown += new MouseEventHandler((sender, args) =>
                menuButton.Image = GameMethods.GetImageByName("EXIT_PRESSED"));
            menuButton.MouseClick += new MouseEventHandler((sender, args) =>
            {
                menuForm.PlayClickSound();
                menuForm.Open(this);
            });
            menuButton.MouseEnter += new EventHandler((sender, args) => 
                menuButton.Image = GameMethods.GetImageByName("EXIT_ACTIVE"));
            menuButton.MouseLeave += new EventHandler((sender, args) => 
                menuButton.Image = GameMethods.GetImageByName("EXIT"));
            menuButton.BackColor = Color.Transparent;
            menuButton.Enabled = false;
            menuButton.Visible = false;
            #endregion
            Controls.Add(menuButton);
            Invalidate();
        }

        private void OnKeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                menuForm.PlayClickSound();
                PausePress();
            }
            else if (e.KeyCode == Keys.Space && !game.IsGameFinished && game.PlayerTower.HealFrame == 7)
            {
                if(menuForm.SoundOn)
                    healSound.controls.play();
                game.PlayerTower.Heal();
            }    
            else if(!game.IsGameFinished && !pauseGame)
            {
                Task.WaitAll(game.Monsters.Select(monster =>
                    Task.Run(() => monster.BeAttacked(e.KeyCode, game.Randomizer))).ToArray());
            }
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            if (menuForm.Enabled)
            {
                menuForm.Enabled = false;
                menuForm.Hide();
            }
            if (!game.IsGameFinished && !pauseGame && animationCycles > 39)
            {
                game.DoGameCycle();
                if (game.PlayerTower.HealRecharged && menuForm.SoundOn)
                    healRechargeSound.controls.play();
                if (game.PlayerTower.LostHealth && menuForm.SoundOn)
                    hitSound.controls.play();
            }
            else if (game.IsGameFinished && !pauseGame && game.Monsters.Count > 0)
            {
                foreach (var monster in game.Monsters)
                {
                    if(!monster.IsDead)
                        monster.GetKilled();
                    monster.Act(game);
                }
            }
            if(game.IsGameFinished && menuForm.SoundOn && !gameOverSoundPlayed)
            {
                gameOverSound.controls.play();
                gameOverSoundPlayed = true;
            }
            Invalidate();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            DrawMap(e);
            if (animationCycles > 39 || game.IsGameFinished)
                foreach(var monster in game.Monsters)
                    DrawMonster(e, monster);
            DrawTower(e);
            var wavesAmountToShow = game.CurrentWave - 3 > 0 ? (game.CurrentWave - 3).ToString() : "1";
            e.Graphics.DrawString(wavesAmountToShow, Font, new SolidBrush(Color.AliceBlue),
                (int)(pauseButton.Location.X - (Font.Size * 5 / 4) * (wavesAmountToShow.Length + 1)),
                     (pauseButton.Location.Y + (pauseButton.Image.Height - Font.Height) / 2));
            if (game.IsGameFinished && gameOverCycles > 30 && gameOverCycles < 60)
            {
                e.Graphics.DrawString("GAME OVER", gameOverFont, new SolidBrush(Color.AliceBlue),
                    (int)(coordinates.X + (1280 - (gameOverFont.Size * 5 / 4) * ("GAME OVER".Length + 1)) / 2 + 10),
                         (coordinates.Y + (720 - gameOverFont.Height) / 2));
                if(!pauseGame)
                    gameOverCycles++;
            }
            else if (game.IsGameFinished && !pauseGame)
            {
                if (gameOverCycles == 60)
                    gameOverCycles = 0;
                else gameOverCycles++;
            }
        }

        private void DrawMap(PaintEventArgs e)
        {
            if (!game.IsGameFinished && animationCycles <= 39)
            {
                e.Graphics.DrawImage(mapImage, coordinates.X, coordinates.Y, game.MapImage.Width, game.MapImage.Height);
                if(!pauseGame)
                    animationCycles++;
                if (animationCycles % 10 == 0)
                    mapImage = GameMethods.GetImageByName($"{animationCycles / 10}_");

            }
            else if (game.IsGameFinished && animationCycles > -1)
            {
                if (animationCycles > 0 && !pauseGame)
                    animationCycles--;
                if (animationCycles % 10 == 9)
                    mapImage = GameMethods.GetImageByName($"{animationCycles / 10}_");
                e.Graphics.DrawImage(mapImage, coordinates.X, coordinates.Y, game.MapImage.Width, game.MapImage.Height);
            }
            else
                e.Graphics.DrawImage(game.MapImage, coordinates.X, coordinates.Y, 
                    game.MapImage.Width, game.MapImage.Height);
        }

        private void DrawTower(PaintEventArgs e)
        {
            e.Graphics.DrawImage(game.SpriteSheets[EntityType.Tower],
                (float)(coordinates.X + game.PlayerTower.Coordinates.X - game.PlayerTower.ImgInfo.ImgWidth / 2),
                (float)(coordinates.Y + game.PlayerTower.Coordinates.Y),
                new Rectangle(new Point(game.PlayerTower.Frame * game.PlayerTower.ImgInfo.ImgWidth, 0),
                new Size(game.PlayerTower.ImgInfo.ImgWidth, game.PlayerTower.ImgInfo.ImgHeight)), GraphicsUnit.Pixel);
            var healCoordinates = new PointF(
                (float)(coordinates.X + game.PlayerTower.Coordinates.X 
                    - game.PlayerTower.ImgInfo.ImgWidth / 2 - game.PlayerTower.HealImgInfo.ImgWidth - 15),
                (float)(coordinates.Y + game.PlayerTower.Coordinates.Y + game.PlayerTower.ImgInfo.ImgHeight / 2));
            e.Graphics.DrawImage(game.PlayerTower.HealImage, healCoordinates.X, healCoordinates.Y,
                new RectangleF(new PointF(game.PlayerTower.HealImgInfo.ImgWidth * game.PlayerTower.HealFrame, 0),
                    new Size(game.PlayerTower.HealImgInfo.ImgWidth, game.PlayerTower.HealImgInfo.ImgHeight)),
                GraphicsUnit.Pixel);
            if(game.PlayerTower.HealFrame == 7 && !pauseGame)
                e.Graphics.DrawImage(game.KeysImages[Keys.Space],
                    healCoordinates.X + 7, healCoordinates.Y + game.PlayerTower.HealImgInfo.ImgHeight + 22,
                    game.KeysImages[Keys.Space].Width, game.KeysImages[Keys.Space].Height);
            e.Graphics.DrawImage(game.PlayerTower.HealthImage,
                (float)(coordinates.X + game.PlayerTower.Coordinates.X - game.PlayerTower.HealthImage.Width / 2 + 14),
                (float)(coordinates.Y + game.PlayerTower.Coordinates.Y + game.PlayerTower.ImgInfo.ImgHeight + 65),
                game.PlayerTower.HealthImage.Width,
                game.PlayerTower.HealthImage.Height);
        }

        private void DrawMonster(PaintEventArgs e, Monster monster)
        {
            var monsterX = (float)(coordinates.X + monster.Coordinates.X - monster.ImgInfo.ImgWidth / 2);
            var monsterY = (float)(coordinates.Y + monster.Coordinates.Y);
            if (!monster.IsDead && !pauseGame)
                e.Graphics.DrawImage(game.KeysImages[monster.CurrentKeyToPress],
                    (float)(monsterX + monster.ImgInfo.ImgWidth / 9 + (monster.ImgInfo.ImgWidth 
                        - game.KeysImages[monster.CurrentKeyToPress].Width) / 2),
                    (float)(monsterY - game.KeysImages[monster.CurrentKeyToPress].Height - 5),
                    game.KeysImages[monster.CurrentKeyToPress].Width, game.KeysImages[monster.CurrentKeyToPress].Height);
            e.Graphics.DrawImage(game.SpriteSheets[monster.Type],
                monsterX,
                monsterY, new Rectangle(
                new Point(monster.Frame * monster.ImgInfo.ImgWidth, (int)monster.CurrentAction * monster.ImgInfo.ImgHeight),
                new Size(monster.ImgInfo.ImgWidth, monster.ImgInfo.ImgHeight)), GraphicsUnit.Pixel);
        }

        private void PausePress()
        {
            pauseGame = !pauseGame;
            if (pauseGame)
                pauseButton.Image = GameMethods.GetImageByName("PAUSE_PRESSED_ACTIVE");
            else pauseButton.Image = GameMethods.GetImageByName("PAUSE");
            restartButton.Enabled = !restartButton.Enabled;
            restartButton.Visible = !restartButton.Visible;
            menuButton.Enabled = !menuButton.Enabled;
            menuButton.Visible = !menuButton.Visible;
        }
    }
}
