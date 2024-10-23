using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Galaga.Model;
using Galaga.View.Sprites;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Galaga.View
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameCanvas
    {
        #region Data members

        public const int InitialTickEnemyMovement = 5;
        public const int TicksBeforeEnemyDirectionChange = 10;

        private readonly GameManager gameManager;
        private readonly Random random;

        private DispatcherTimer enemyBulletTimer;
        private DispatcherTimer enemyMovementTimer;
        private DispatcherTimer enemyBulletMovementTimer;
        private DispatcherTimer playerBulletTimer;
        private DispatcherTimer gameLoopTimer;

        private int enemyTickCounter;
        private bool enemyMoveRight;
        private readonly HashSet<VirtualKey> activeKeys;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameCanvas" /> class.
        /// </summary>
        public GameCanvas()
        {
            this.InitializeComponent();

            this.random = new Random();
            this.activeKeys = new HashSet<VirtualKey>();

            this.createPlayerBulletTimer();
            this.createEnemyMovementTimer();
            this.createEnemyBulletTimer();
            this.createGameLoopTimer();

            Width = this.canvas.Width;
            Height = this.canvas.Height;
            ApplicationView.PreferredLaunchViewSize = new Size { Width = Width, Height = Height };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(Width, Height));

            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;
            Window.Current.CoreWindow.KeyUp += this.coreWindowOnKeyUp;

            this.gameManager = new GameManager(this.canvas);
        }

        #endregion

        #region Methods

        private void createEnemyBulletTimer()
        {
            this.enemyBulletTimer = new DispatcherTimer();
            this.enemyBulletMovementTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 100)
            };
            this.setRandomTimeInterval();
            this.enemyBulletTimer.Tick += this.bullet_TimerTick;
            this.enemyBulletMovementTimer.Tick += this.bulletMovement_TimerTick;
            this.enemyBulletMovementTimer.Start();
            this.enemyBulletTimer.Start();
        }

        private void setRandomTimeInterval()
        {
            var randomTime = this.random.Next(250, 2500);
            this.enemyBulletTimer.Interval = TimeSpan.FromMilliseconds(randomTime);
        }

        private void bullet_TimerTick(object sender, object e)
        {
            this.gameManager.PlaceEnemyBullet();
            this.setRandomTimeInterval();
        }

        private void bulletMovement_TimerTick(object sender, object e)
        {
            this.gameManager.MoveEnemyBullet();
            this.loseTheGame();
        }

        private void createPlayerBulletTimer()
        {
            this.playerBulletTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 10)
            };
            this.playerBulletTimer.Tick += this.playerBullet_TimerTick;
            this.playerBulletTimer.Start();
        }

        private void playerBullet_TimerTick(object sender, object e)
        {
            this.gameManager.MovePlayerBullet();
            this.scoreTextBlock.Text = this.gameManager.GetScore().ToString();
            this.winTheGame();

        }

        private void winTheGame()
        {
            if (this.gameManager.GetRemainingEnemyCount() == 0)
            {
                this.disableSpritesAndTimers();
                this.youWinTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void loseTheGame()
        {
            foreach (var sprite in this.canvas.Children)
            {
                if ((sprite is PlayerSprite player))
                {
                    return;
                }
            }

            this.disableSpritesAndTimers();
            this.gameOverTextBlock.Visibility = Visibility.Visible;
        }

        private void disableSpritesAndTimers()
        {
            foreach (var sprite in this.canvas.Children)
            {
                if (sprite is BaseSprite baseSprite)
                {
                    baseSprite.Visibility = Visibility.Collapsed;
                }
            }

            this.gameLoopTimer.Stop();
            this.enemyMovementTimer.Stop();
            this.enemyBulletTimer.Stop();
            this.enemyBulletMovementTimer.Stop();
            this.playerBulletTimer.Stop();
        }

        private void createEnemyMovementTimer()
        {
            this.enemyMovementTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 350)
            };
            this.enemyMovementTimer.Tick += this.enemyMovement_TimerTick;
            this.enemyMovementTimer.Start();
        }

        private void enemyMovement_TimerTick(object sender, object e)
        {
            this.enemyTickCounter++;

            if (this.enemyTickCounter <= InitialTickEnemyMovement)
            {
                this.gameManager.MoveEnemiesLeft();
            }
            else if (this.enemyTickCounter < TicksBeforeEnemyDirectionChange)
            {
                this.enemyTickCounter = TicksBeforeEnemyDirectionChange;
            }

            if (this.enemyTickCounter >= TicksBeforeEnemyDirectionChange)
            {
                if (this.enemyTickCounter % TicksBeforeEnemyDirectionChange == 0)
                {
                    this.enemyMoveRight = !this.enemyMoveRight;
                }

                if (this.enemyMoveRight)
                {
                    this.gameManager.MoveEnemiesRight();
                }
                else
                {
                    this.gameManager.MoveEnemiesLeft();
                }
            }
        }

        private void createGameLoopTimer()
        {
            this.gameLoopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            this.gameLoopTimer.Tick += this.gameLoopTimer_Tick;
            this.gameLoopTimer.Start();
        }

        private void gameLoopTimer_Tick(object sender, object e)
        {
            if (this.activeKeys.Contains(VirtualKey.Left))
            {
                this.gameManager.MovePlayerLeft();
            }

            if (this.activeKeys.Contains(VirtualKey.Right))
            {
                this.gameManager.MovePlayerRight();
            }

            if (this.activeKeys.Contains(VirtualKey.Space))
            {
                this.gameManager.PlacePlayerBullet();
            }
        }

        private void coreWindowOnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            this.activeKeys.Add(args.VirtualKey);
        }

        private void coreWindowOnKeyUp(CoreWindow sender, KeyEventArgs args)
        {
            this.activeKeys.Remove(args.VirtualKey);
        }

        #endregion
    }
}