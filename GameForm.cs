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
        private bool pauseGame;
        private bool gameOverSoundPlayed;
        private Vector coordinates;
        private int animationCycles;
        private int gameOverCycles;
        private readonly Menu menuForm;
        private readonly Game game;
        private readonly WindowsMediaPlayer healRechargeSound;
        private readonly WindowsMediaPlayer healSound;
        private readonly WindowsMediaPlayer hitSound;
        private readonly WindowsMediaPlayer gameOverSound;
        private readonly Font gameOverFont;

        public KeyboardSaga(Menu menuF)
        {
            //Form Initialization
            InitializeComponent();
            Icon = Properties.Resources.logo;
            animationCycles = 0;
            gameOverCycles = 0;
            mapImage = GameMethods.GetImageByName("0_");
            menuForm = menuF;
            Font = new Font(menuForm.PFC.Families[0], 20F);
            gameOverFont = new Font(menuForm.PFC.Families[0], 40F);
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
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.Disable;
            DoubleBuffered = true;
            Name = "KeyboardSaga";
            Text = "eyboardSaga";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            ClientSize = new Size(1280, 720);
            coordinates = new Vector((ClientSize.Width - game.MapImage.Width) / 2, 
                                     (ClientSize.Height - game.MapImage.Height) / 2);
            BackColor = Color.FromArgb(47, 40, 58);
            Paint += new PaintEventHandler(OnPaint);
            KeyDown += new KeyEventHandler(OnKeyPress);
            SizeChanged += new EventHandler((sender, args) =>
            {
                coordinates = new Vector((ClientSize.Width - game.MapImage.Width) / 2, 
                                         (ClientSize.Height - game.MapImage.Height) / 2);
                pause.Location = new Point((int)(ClientSize.Width - pause.Image.Width - 30), 30);
                restart.Location = new Point((int)(coordinates.X + game.MapImage.Width / 2 - restart.Image.Width / 2), 
                                             (int)(ClientSize.Height / 2 - restart.Image.Height));
                menu.Location = new Point((int)(coordinates.X + game.MapImage.Width / 2 - menu.Image.Width / 2), 
                                          (int)(restart.Location.Y + 15 + restart.Image.Height));
                Invalidate();
            });
            //Timer
            timer = new Timer(components)
            {
                Enabled = true,
                Interval = 16
            };
            timer.Tick += new EventHandler(OnFrameChanged);
            #region Pause Button
            pause = new PictureBox();
            pause.Image = GameMethods.GetImageByName("PAUSE");
            pause.Size = new Size(pause.Image.Width, pause.Image.Height);
            pause.Location = new Point((int)(ClientSize.Width - pause.Image.Width - 40), 20);
            pause.MouseUp += new MouseEventHandler((sender, args) => 
                { if (!pauseGame) pause.Image = GameMethods.GetImageByName("PAUSE_ACTIVE"); });
            pause.MouseDown += new MouseEventHandler((sender, args) => 
                pause.Image = GameMethods.GetImageByName("PAUSE_PRESSED_ACTIVE"));
            pause.MouseClick += new MouseEventHandler((sender, args) =>
            {
                menuForm.PlayClickSound();
                pauseGame = !pauseGame;
                restart.Enabled = !restart.Enabled;
                restart.Visible = !restart.Visible;
                menu.Enabled = !menu.Enabled;
                menu.Visible = !menu.Visible;
            });
            pause.MouseEnter += new EventHandler((sender, args) => 
                { if (!pauseGame) pause.Image = GameMethods.GetImageByName("PAUSE_ACTIVE"); });
            pause.MouseLeave += new EventHandler((sender, args) => 
                { if (!pauseGame) pause.Image = GameMethods.GetImageByName("PAUSE"); });
            pause.BackColor = Color.Transparent;
            #endregion
            Controls.Add(pause);
            #region Restart Button
            restart = new PictureBox();
            restart.Image = GameMethods.GetImageByName("RESTART");
            restart.Size = new Size(restart.Image.Width, restart.Image.Height);
            restart.Location = new Point((int)(coordinates.X + game.MapImage.Width / 2 - restart.Image.Width / 2), 
                                         (int)(ClientSize.Height / 2 - restart.Image.Height));
            restart.MouseUp += new MouseEventHandler((sender, args) => 
                restart.Image = GameMethods.GetImageByName("RESTART_ACTIVE"));
            restart.MouseDown += new MouseEventHandler((sender, args) =>
                restart.Image = GameMethods.GetImageByName("RESTART_PRESSED"));
            restart.MouseClick += new MouseEventHandler((sender, args) =>
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
            restart.MouseEnter += new EventHandler((sender, args) => 
                restart.Image = GameMethods.GetImageByName("RESTART_ACTIVE"));
            restart.MouseLeave += new EventHandler((sender, args) => 
                restart.Image = GameMethods.GetImageByName("RESTART"));
            restart.BackColor = Color.Transparent;
            restart.Enabled = false;
            restart.Visible = false;
            #endregion
            Controls.Add(restart);
            #region Exit To Menu Button
            menu = new PictureBox();
            menu.Image = GameMethods.GetImageByName("EXIT");
            menu.Size = new Size(menu.Image.Width, menu.Image.Height);
            menu.Location = new Point((int)(coordinates.X + game.MapImage.Width / 2 - menu.Image.Width / 2), 
                                      (int)(restart.Location.Y + 15 + restart.Image.Height));
            menu.MouseUp += new MouseEventHandler((sender, args) => 
                menu.Image = GameMethods.GetImageByName("EXIT_ACTIVE"));
            menu.MouseDown += new MouseEventHandler((sender, args) =>
                menu.Image = GameMethods.GetImageByName("EXIT_PRESSED"));
            menu.MouseClick += new MouseEventHandler((sender, args) =>
            {
                menuForm.PlayClickSound();
                menuForm.Open(this);
            });
            menu.MouseEnter += new EventHandler((sender, args) => 
                menu.Image = GameMethods.GetImageByName("EXIT_ACTIVE"));
            menu.MouseLeave += new EventHandler((sender, args) => 
                menu.Image = GameMethods.GetImageByName("EXIT"));
            menu.BackColor = Color.Transparent;
            menu.Enabled = false;
            menu.Visible = false;
            #endregion
            Controls.Add(menu);
            Invalidate();
        }

        private void OnKeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
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
            if (menuForm.Enabled)
            {
                menuForm.Enabled = false;
                menuForm.Hide();
            }
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
                (int)(pause.Location.X - (Font.Size * 5 / 4) * (wavesAmountToShow.Length + 1)),
                (int)(pause.Location.Y + (pause.Image.Height - Font.Height) / 2));
            if (game.IsGameFinished && gameOverCycles > 30 && gameOverCycles < 60)
            {
                e.Graphics.DrawString("GAME OVER", gameOverFont, new SolidBrush(Color.AliceBlue),
                    (int)(coordinates.X + (1280 - (gameOverFont.Size * 5 / 4) * ("GAME OVER".Length + 1)) / 2 + 10),
                    (int)(coordinates.Y + (720 - gameOverFont.Height) / 2));
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
                e.Graphics.DrawImage(mapImage,
                    (float)coordinates.X,
                    (float)coordinates.Y, 
                    game.MapImage.Width, 
                    game.MapImage.Height);
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
                e.Graphics.DrawImage(mapImage,
                    (float)coordinates.X,
                    (float)coordinates.Y,
                    game.MapImage.Width,
                    game.MapImage.Height);
            }
            else
                e.Graphics.DrawImage(game.MapImage, 
                    (float)coordinates.X, 
                    (float)coordinates.Y, 
                    game.MapImage.Width, 
                    game.MapImage.Height);
        }

        private void DrawTower(PaintEventArgs e)
        {
            e.Graphics.DrawImage(game.SpriteSheets[EntityType.Tower],
                (float)(coordinates.X + game.PlayerTower.Coordinates.X - game.PlayerTower.ImgInfo.ImgWidth / 2),
                (float)(coordinates.Y + game.PlayerTower.Coordinates.Y),
                new Rectangle(new Point(game.PlayerTower.Frame * game.PlayerTower.ImgInfo.ImgWidth, 0),
                new Size(game.PlayerTower.ImgInfo.ImgWidth, game.PlayerTower.ImgInfo.ImgHeight)), GraphicsUnit.Pixel);
            var healCoordinates = new PointF((float)(coordinates.X + game.PlayerTower.Coordinates.X - game.PlayerTower.ImgInfo.ImgWidth / 2 - game.PlayerTower.HealImgInfo.ImgWidth - 15),
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
            if (!monster.IsDead && !pauseGame)
                e.Graphics.DrawImage(game.KeysImages[monster.CurrentKeyToPress],
                    (float)(coordinates.X + monster.Coordinates.X - game.KeysImages[monster.CurrentKeyToPress].Width / 2 + 8),
                    (float)(coordinates.Y + monster.Coordinates.Y - game.KeysImages[monster.CurrentKeyToPress].Height - 5),
                    game.KeysImages[monster.CurrentKeyToPress].Width, game.KeysImages[monster.CurrentKeyToPress].Height);
            e.Graphics.DrawImage(game.SpriteSheets[monster.Type],
                (float)(coordinates.X + monster.Coordinates.X - monster.ImgInfo.ImgWidth / 2),
                (float)(coordinates.Y + monster.Coordinates.Y), new Rectangle(
                new Point(monster.Frame * monster.ImgInfo.ImgWidth, (int)monster.CurrentAction * monster.ImgInfo.ImgHeight),
                new Size(monster.ImgInfo.ImgWidth, monster.ImgInfo.ImgHeight)), GraphicsUnit.Pixel);
        }

        private void PausePress()
        {
            pauseGame = !pauseGame;
            if (pauseGame)
                pause.Image = GameMethods.GetImageByName("PAUSE_PRESSED_ACTIVE");
            else pause.Image = GameMethods.GetImageByName("PAUSE");
            restart.Enabled = !restart.Enabled;
            restart.Visible = !restart.Visible;
            menu.Enabled = !menu.Enabled;
            menu.Visible = !menu.Visible;
        }
    }
}
