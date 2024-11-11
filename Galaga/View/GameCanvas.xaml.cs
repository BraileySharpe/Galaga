using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.Model;
using Galaga.View.Sprites;

namespace Galaga.View
{
    /// <summary>
    ///     The Canvas for the Galaga Game.
    /// </summary>
    public sealed partial class GameCanvas
    {
        #region Data members

        public const int InitialTickEnemyMovement = 5;
        public const int TicksBeforeEnemyDirectionChange = 10;
        private const int PlayerBulletCooldownMilliseconds = 300;

        private readonly GameManager gameManager;
        private readonly Random random;

        private DispatcherTimer enemyBulletTimer;
        private DispatcherTimer enemyMovementTimer;
        private DispatcherTimer enemyBulletMovementTimer;
        private DispatcherTimer playerBulletTimer;
        private DispatcherTimer gameLoopTimer;
        private DispatcherTimer playerBulletCooldownTimer;

        private int enemyTickCounter;
        private bool enemyMoveRight;
        private bool spacePressedPreviously;
        private bool canShoot = true;
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

            this.initializeTimers();
            this.setupWindowPreferences();

            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;
            Window.Current.CoreWindow.KeyUp += this.coreWindowOnKeyUp;

            this.gameManager = new GameManager(this.canvas);
            this.DataContext = this.gameManager;
        }

        #endregion

        #region Methods

        private void setupWindowPreferences()
        {
            Width = this.canvas.Width;
            Height = this.canvas.Height;
            ApplicationView.PreferredLaunchViewSize = new Size { Width = Width, Height = Height };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(Width, Height));
        }

        private void initializeTimers()
        {
            this.setUpPlayerBulletTimer();
            this.setUpEnemyMovementTimer();
            this.setUpEnemyBulletTimer();
            this.setUpGameLoopTimer();
            this.setUpPlayerBulletCooldownTimer();
        }

        private void setUpPlayerBulletCooldownTimer()
        {
            this.playerBulletCooldownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(PlayerBulletCooldownMilliseconds)
            };
            this.playerBulletCooldownTimer.Tick += (sender, e) =>
            {
                this.canShoot = true;
                this.playerBulletCooldownTimer.Stop();
            };
        }

        private void setUpEnemyBulletTimer()
        {
            this.enemyBulletTimer = new DispatcherTimer();
            this.enemyBulletMovementTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 100)
            };

            this.setRandomEnemyTimeInterval();
            this.enemyBulletTimer.Tick += this.bulletTimerTick;
            this.enemyBulletMovementTimer.Tick += this.bulletMovementTimerTick;

            this.enemyBulletMovementTimer.Start();
            this.enemyBulletTimer.Start();
        }

        private void setRandomEnemyTimeInterval()
        {
            this.enemyBulletTimer.Interval = TimeSpan.FromMilliseconds(this.random.Next(250, 2500));
        }

        private void bulletTimerTick(object sender, object e)
        {
            this.gameManager.PlaceEnemyBullet();
            this.setRandomEnemyTimeInterval();
        }

        private void bulletMovementTimerTick(object sender, object e)
        {
            this.gameManager.MoveEnemyBullet();
            this.checkGameOver();
        }

        private void setUpPlayerBulletTimer()
        {
            this.playerBulletTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 10)
            };
            this.playerBulletTimer.Tick += this.playerBulletTimerTick;
            this.playerBulletTimer.Start();
        }

        private void playerBulletTimerTick(object sender, object e)
        {
            this.gameManager.MovePlayerBullet();
            this.checkGameWin();
        }

        private void checkGameWin()
        {
            if (this.gameManager.GetRemainingEnemyCount() == 0)
            {
                this.endGame(this.youWinTextBlock);
            }
        }

        private void checkGameOver()
        {
            if (!this.playerExists())
            {
                this.endGame(this.gameOverTextBlock);
            }
        }

        private bool playerExists()
        {
            foreach (var sprite in this.canvas.Children)
            {
                if (sprite is PlayerSprite)
                {
                    return true;
                }
            }

            return false;
        }

        private void endGame(TextBlock endgameTextBlock)
        {
            this.disableAllSprites();
            this.stopAllTimers();

            endgameTextBlock.Visibility = Visibility.Visible;
        }

        private void stopAllTimers()
        {
            this.gameLoopTimer.Stop();
            this.enemyMovementTimer.Stop();
            this.enemyBulletTimer.Stop();
            this.enemyBulletMovementTimer.Stop();
            this.playerBulletTimer.Stop();
        }

        private void disableAllSprites()
        {
            foreach (var sprite in this.canvas.Children)
            {
                if (sprite is BaseSprite baseSprite)
                {
                    baseSprite.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void setUpEnemyMovementTimer()
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

            this.gameManager.ToggleSpritesForAnimation();
        }

        private void setUpGameLoopTimer()
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
                if (!this.spacePressedPreviously && this.canShoot)
                {
                    this.gameManager.PlacePlayerBullet();
                    this.canShoot = false;
                    this.spacePressedPreviously = true;
                    this.playerBulletCooldownTimer.Start();
                }
            }
            else
            {
                this.spacePressedPreviously = false;
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