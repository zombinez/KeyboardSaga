using KeyboardSagaGame.Classes;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace KeyboardSagaGame
{
    public partial class Tutorial : Form
    {
        private Menu menuForm;
        private Image mapImage;
        private Vector coordinates;
        private int animationCycles;
        private bool pauseGame;
        private Game game;
        private readonly Image arrow;
        private readonly Image exclamationMark;
        private int markAnimationCycles;
        private bool tutorialFinished;
        private readonly WindowsMediaPlayer hitSound;
        private bool awareCondition => game.Monsters.Any(monster => 
            MonsterMethods.GetCurrentDistance(monster.Coordinates, game.PlayerTower.Coordinates) < 250);

        public Tutorial(Menu menuF)
        {
            //Form Initialization
            InitializeComponent();
            Icon = Properties.Resources.logo;
            menuForm = menuF;
            hitSound = new WindowsMediaPlayer();
            hitSound.URL = 
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"sounds\hit.wav");
            hitSound.settings.volume = 10;
            hitSound.controls.stop();
            mapImage = GameMethods.GetImageByName("0_");
            tutorialFinished = false;
            arrow = GameMethods.GetImageByName("arrow");
            exclamationMark = GameMethods.GetImageByName("exclamation_mark");
            game = new Game();
            pauseGame = false;
            animationCycles = 0;
            markAnimationCycles = 0;
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoValidate = AutoValidate.Disable;
            DoubleBuffered = true;
            Name = "KeyboardSaga";
            Text = "eyboardSaga";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            ClientSize = new Size(1280, 720);
            coordinates = new Vector(
                (ClientSize.Width - game.MapImage.Width) / 2, 
                (ClientSize.Height - game.MapImage.Height) / 2);
            BackColor = Color.FromArgb(47, 40, 58);
            Paint += new PaintEventHandler(OnPaint);
            KeyDown += new KeyEventHandler(OnKeyPress);
            SizeChanged += new EventHandler((sender, args) =>
            {
                coordinates = new Vector(
                    (ClientSize.Width - game.MapImage.Width) / 2, 
                    (ClientSize.Height - game.MapImage.Height) / 2);
                pause.Location = new Point((int)(ClientSize.Width - pause.Image.Width - 30), 30);
                menu.Location = menu.Location = new Point(
                    (int)(coordinates.X + game.MapImage.Width / 2 - menu.Image.Width / 2), 
                    (int)(ClientSize.Height / 2 - menu.Image.Height));
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
            {
                pause.Image = GameMethods.GetImageByName("PAUSE_PRESSED_ACTIVE");
            });
            pause.MouseClick += new MouseEventHandler((sender, args) =>
            {
                menuForm.PlayClickSound();
                PausePress();
            });
            pause.MouseEnter += new EventHandler((sender, args) => { if (!pauseGame) pause.Image = GameMethods.GetImageByName("PAUSE_ACTIVE"); });
            pause.MouseLeave += new EventHandler((sender, args) => { if (!pauseGame) pause.Image = GameMethods.GetImageByName("PAUSE"); });
            pause.BackColor = Color.Transparent;
            #endregion
            Controls.Add(pause);
            #region Menu Button
            menu = new PictureBox();
            menu.Image = GameMethods.GetImageByName("EXIT");
            menu.Size = new Size(menu.Image.Width, menu.Image.Height);
            menu.Location = new Point((int)(coordinates.X + game.MapImage.Width / 2 - menu.Image.Width / 2), (int)(ClientSize.Height / 2 - menu.Image.Height));
            menu.MouseUp += new MouseEventHandler((sender, args) => menu.Image = GameMethods.GetImageByName("EXIT_ACTIVE"));
            menu.MouseDown += new MouseEventHandler((sender, args) =>
            {
                menu.Image = GameMethods.GetImageByName("EXIT_PRESSED");
            });
            menu.MouseClick += new MouseEventHandler((sender, args) =>
            {
                menuForm.PlayClickSound();
                menuForm.Open(this);
            });
            menu.MouseEnter += new EventHandler((sender, args) => menu.Image = GameMethods.GetImageByName("EXIT_ACTIVE"));
            menu.MouseLeave += new EventHandler((sender, args) => menu.Image = GameMethods.GetImageByName("EXIT"));
            menu.BackColor = Color.Transparent;
            menu.Enabled = false;
            menu.Visible = false;
            #endregion
            Controls.Add(menu);
            Invalidate();
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            if (menuForm.Enabled)
            {
                menuForm.Enabled = false;
                menuForm.Hide();
            }
            if (!game.IsGameFinished && !pauseGame && animationCycles > 39
                && (!(game.PlayerTower.HealthAmount > 20 && game.PlayerTower.HealthAmount < 25)
                    || game.Monsters.TrueForAll(monster =>
                           monster.CurrentAction == MonsterAction.Death && monster.Cycles.DeathCyclesDid < 60)))
                game.DoGameCycle();
            else game.PlayerTower.ChangeState();
            if (game.PlayerTower.LostHealth && menuForm.SoundOn)
                hitSound.controls.play();
            Invalidate();
            if (game.Monsters.TrueForAll(monster => monster.Cycles.DeathCyclesDid >= 60))
            {
                tutorialFinished = true;
                if (animationCycles == 0)
                    menuForm.Open(this);
            }
        }

        private void OnKeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                menuForm.PlayClickSound();
                PausePress();
            }
            else if (!tutorialFinished && !pauseGame)
            {
                Task.WaitAll(game.Monsters.Select(monster =>
                    Task.Run(() => monster.BeAttacked(e.KeyCode, game.Randomizer))).ToArray());
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (markAnimationCycles == 59 && !pauseGame)
            {
                markAnimationCycles = 0;
            }
            else if (!pauseGame)
            {
                markAnimationCycles++;
            }
            DrawMap(e);
            if (animationCycles > 39 || game.IsGameFinished)
                for (var i = game.Monsters.Count - 1; i >= 0; i--)
                    DrawMonster(e, game.Monsters[i]);
            DrawTower(e);
        }

        private void DrawMap(PaintEventArgs e)
        {
            if (!tutorialFinished && animationCycles <= 39)
            {
                e.Graphics.DrawImage(mapImage,
                    (float)coordinates.X,
                    (float)coordinates.Y,
                    game.MapImage.Width,
                    game.MapImage.Height);
                if (!pauseGame)
                    animationCycles++;
                if (animationCycles % 10 == 0)
                    mapImage = GameMethods.GetImageByName($"{animationCycles / 10}_");

            }
            else if (tutorialFinished && animationCycles > -1)
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
            e.Graphics.DrawImage(game.PlayerTower.HealthImage,
                (float)(coordinates.X + game.PlayerTower.Coordinates.X - game.PlayerTower.HealthImage.Width / 2 + 14),
                (float)(coordinates.Y + game.PlayerTower.Coordinates.Y + game.PlayerTower.ImgInfo.ImgHeight + 65),
                game.PlayerTower.HealthImage.Width,
                game.PlayerTower.HealthImage.Height);
            if (game.PlayerTower.HealthAmount <= 40)
                e.Graphics.DrawImage(exclamationMark,
                    (float)(coordinates.X + game.PlayerTower.Coordinates.X + game.PlayerTower.HealthImage.Width - 65),
                    (float)(coordinates.Y + game.PlayerTower.Coordinates.Y 
                        + game.PlayerTower.ImgInfo.ImgHeight + 80 - exclamationMark.Height),
                    new Rectangle(new Point((markAnimationCycles / 10) * 22, 0), new Size(22, 32)), GraphicsUnit.Pixel);
        }

        private void DrawMonster(PaintEventArgs e, Monster monster)
        {
            var keyX = (float)(coordinates.X + monster.Coordinates.X - game.KeysImages[monster.CurrentKeyToPress].Width / 2 + 8);
            var keyY = (float)(coordinates.Y + monster.Coordinates.Y - game.KeysImages[monster.CurrentKeyToPress].Height - 5);
            if (!awareCondition && !monster.IsDead)
                e.Graphics.DrawImage(exclamationMark, (float)(keyX - 1), (float)(keyY - exclamationMark.Height - 15),
                    new Rectangle(new Point((markAnimationCycles / 10) * 22, 0), new Size(22, 32)), GraphicsUnit.Pixel);
            else if (!monster.IsDead)
                e.Graphics.DrawImage(arrow, (float)(keyX - 1), (float)(keyY - arrow.Height - 15),
                    new Rectangle(new Point((markAnimationCycles / 10) * 22, 0), new Size(22, 32)), GraphicsUnit.Pixel);
            if (!monster.IsDead)
                e.Graphics.DrawImage(game.KeysImages[monster.CurrentKeyToPress], keyX, keyY,
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
            menu.Enabled = !menu.Enabled;
            menu.Visible = !menu.Visible;
        }
    }
}
