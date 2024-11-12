using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        // Constants for game behavior
        private const int InitialEnemyMovementTick = 5;
        private const int EnemyDirectionChangeTicks = 10;
        private const int PlayerBulletCooldownMilliseconds = 300;
        private const int EnemyBulletMovementIntervalMilliseconds = 100;
        private const int EnemyMovementIntervalMilliseconds = 350;
        private const int GameLoopTimerIntervalMilliseconds = 16;

        // The game manager and other timers
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
            DataContext = this.gameManager;
            this.gameManager.PropertyChanged += this.OnGameManagerPropertyChanged;
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
                Interval = TimeSpan.FromMilliseconds(EnemyBulletMovementIntervalMilliseconds)
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
        }

        private void setUpPlayerBulletTimer()
        {
            this.playerBulletTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            this.playerBulletTimer.Tick += this.playerBulletTimerTick;
            this.playerBulletTimer.Start();
        }

        private void playerBulletTimerTick(object sender, object e)
        {
            this.gameManager.MovePlayerBullet();
        }

        private void OnGameManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.gameManager.HasLost) && this.gameManager.HasLost)
            {
                this.endGame(this.gameOverTextBlock);
            }

            if (e.PropertyName == nameof(this.gameManager.HasWon) && this.gameManager.HasWon)
            {
                this.endGame(this.youWinTextBlock);
            }
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
                Interval = TimeSpan.FromMilliseconds(EnemyMovementIntervalMilliseconds)
            };
            this.enemyMovementTimer.Tick += this.enemyMovement_TimerTick;
            this.enemyMovementTimer.Start();
        }

        private void enemyMovement_TimerTick(object sender, object e)
        {
            this.enemyTickCounter++;

            if (this.enemyTickCounter <= InitialEnemyMovementTick)
            {
                this.gameManager.MoveEnemiesLeft();
            }
            else if (this.enemyTickCounter < EnemyDirectionChangeTicks)
            {
                this.enemyTickCounter = EnemyDirectionChangeTicks;
            }

            if (this.enemyTickCounter >= EnemyDirectionChangeTicks)
            {
                if (this.enemyTickCounter % EnemyDirectionChangeTicks == 0)
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
                Interval = TimeSpan.FromMilliseconds(GameLoopTimerIntervalMilliseconds)
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

            this.gameManager.CheckGameStatus();
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