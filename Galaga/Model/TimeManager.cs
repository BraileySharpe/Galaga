using System;
using Windows.UI.Xaml;

namespace Galaga.View
{
    /// <summary>
    ///     Manages all timers for the game.
    /// </summary>
    public class TimeManager
    {
        #region Constructors

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="TimeManager" /> class.
        /// </summary>
        /// <param name="gameCanvas">The game canvas.</param>
        /// <exception cref="System.ArgumentNullException">gameCanvas</exception>
        public TimeManager(GameCanvas gameCanvas)
        {
            this.gameCanvas = gameCanvas ?? throw new ArgumentNullException(nameof(gameCanvas));
            this.random = new Random();
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes the timers.
        /// </summary>
        public void InitializeTimers()
        {
            this.setUpPlayerBulletTimer();
            this.setUpEnemyMovementTimer();
            this.setUpEnemyBulletTimer();
            this.setUpGameLoopTimer();
            this.setUpPlayerBulletCooldownTimer();
        }

        /// <summary>
        ///     Starts the player bullet cooldown.
        /// </summary>
        public void StartPlayerBulletCooldown()
        {
            this.playerBulletCooldownTimer.Start();
        }

        /// <summary>
        ///     Stops all timers.
        /// </summary>
        public void StopAllTimers()
        {
            this.playerBulletTimer?.Stop();
            this.enemyMovementTimer?.Stop();
            this.enemyBulletTimer?.Stop();
            this.enemyBulletMovementTimer?.Stop();
            this.gameLoopTimer?.Stop();
            this.playerBulletCooldownTimer?.Stop();
        }

        private void setUpPlayerBulletTimer()
        {
            this.playerBulletTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            this.playerBulletTimer.Tick += (sender, e) => this.gameCanvas.MovePlayerBullet();
            this.playerBulletTimer.Start();
        }

        private void setUpEnemyMovementTimer()
        {
            this.enemyMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(350)
            };
            this.enemyMovementTimer.Tick += (sender, e) =>
            {
                this.enemyTickCounter++;

                if (this.enemyTickCounter <= 5)
                {
                    this.gameCanvas.MoveEnemiesLeft();
                }
                else if (this.enemyTickCounter < 10)
                {
                    this.enemyTickCounter = 10;
                }

                if (this.enemyTickCounter >= 10)
                {
                    if (this.enemyTickCounter % 10 == 0)
                    {
                        this.enemyMoveRight = !this.enemyMoveRight;
                    }

                    if (this.enemyMoveRight)
                    {
                        this.gameCanvas.MoveEnemiesRight();
                    }
                    else
                    {
                        this.gameCanvas.MoveEnemiesLeft();
                    }
                }

                this.gameCanvas.ToggleSpritesForAnimation();
            };
            this.enemyMovementTimer.Start();
        }

        private void setUpEnemyBulletTimer()
        {
            this.enemyBulletTimer = new DispatcherTimer();
            this.enemyBulletMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };

            this.setRandomEnemyTimeInterval();
            this.enemyBulletTimer.Tick += (sender, e) =>
            {
                this.gameCanvas.PlaceEnemyBullet();
                this.setRandomEnemyTimeInterval();
            };
            this.enemyBulletMovementTimer.Tick += (sender, e) => this.gameCanvas.MoveEnemyBullet();

            this.enemyBulletTimer.Start();
            this.enemyBulletMovementTimer.Start();
        }

        private void setRandomEnemyTimeInterval()
        {
            this.enemyBulletTimer.Interval = TimeSpan.FromMilliseconds(this.random.Next(250, 2500));
        }

        private void setUpGameLoopTimer()
        {
            this.gameLoopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            this.gameLoopTimer.Tick += (sender, e) => this.gameCanvas.GameLoop();
            this.gameLoopTimer.Start();
        }

        private void setUpPlayerBulletCooldownTimer()
        {
            this.playerBulletCooldownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            this.playerBulletCooldownTimer.Tick += (sender, e) =>
            {
                this.gameCanvas.EnablePlayerShooting();
                this.playerBulletCooldownTimer.Stop();
            };
        }

        #endregion

        #region Data Members

        private readonly GameCanvas gameCanvas;
        private readonly Random random;

        private DispatcherTimer playerBulletTimer;
        private DispatcherTimer enemyMovementTimer;
        private DispatcherTimer enemyBulletTimer;
        private DispatcherTimer enemyBulletMovementTimer;
        private DispatcherTimer gameLoopTimer;
        private DispatcherTimer playerBulletCooldownTimer;

        private int enemyTickCounter;
        private bool enemyMoveRight;

        #endregion
    }
}