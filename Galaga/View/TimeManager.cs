using System;
using Windows.UI.Xaml;

namespace Galaga.View
{
    /// <summary>
    ///     Manages all timers for the game.
    /// </summary>
    public class TimeManager
    {
        #region Data members

        private const int EnemyMovementInMilliseconds = 350;
        private const int MinCooldownForEnemyBulletInMilliseconds = 250;
        private const int MaxCooldownForEnemyBulletInMilliseconds = 2500;
        private const int PlayerBulletCooldownInMilliseconds = 300;
        private const int GameLoopInMilliseconds = 16;
        private const int PlayerBulletMovementInMilliseconds = 10;
        private const int EnemyBulletMovementInMilliseconds = 100;
        private const int BonusEnemyMovementInMilliseconds = 200;

        private readonly GameCanvas gameCanvas;
        private readonly Random random;

        private DispatcherTimer playerBulletTimer;
        private DispatcherTimer enemyMovementTimer;
        private DispatcherTimer enemyBulletTimer;
        private DispatcherTimer enemyBulletMovementTimer;
        private DispatcherTimer gameLoopTimer;
        private DispatcherTimer playerBulletCooldownTimer;
        private DispatcherTimer bonusEnemyMovementTimer;

        private int enemyTickCounter;
        private bool enemyMoveRight;

        private DispatcherTimer bonusEnemyActivationTimer;

        #endregion

        #region Constructors

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
            this.setUpBonusEnemyMovementTimer();
            this.setUpBonusEnemyActivationTimer();
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
            this.bonusEnemyMovementTimer?.Stop();
            this.bonusEnemyActivationTimer?.Stop();
        }

        /// <summary>
        ///     Resets the bonus enemy activation and movement timers for a new round.
        /// </summary>
        public void ResetBonusEnemyTimers()
        {
            this.bonusEnemyActivationTimer?.Stop();
            this.bonusEnemyMovementTimer?.Stop();

            this.setRandomBonusEnemyActivationTime();
            this.bonusEnemyActivationTimer?.Start();
        }

        private void setUpPlayerBulletTimer()
        {
            this.playerBulletTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(PlayerBulletMovementInMilliseconds)
            };
            this.playerBulletTimer.Tick += (sender, e) => this.gameCanvas.MovePlayerBullet();
            this.playerBulletTimer.Start();
        }

        private void setUpEnemyMovementTimer()
        {
            this.enemyMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(EnemyMovementInMilliseconds)
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

        private void setUpBonusEnemyMovementTimer()
        {
            this.bonusEnemyMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(BonusEnemyMovementInMilliseconds)
            };

            this.bonusEnemyMovementTimer.Tick += (sender, e) => { this.gameCanvas.MoveBonusEnemy(); };
        }

        private void setUpBonusEnemyActivationTimer()
        {
            this.bonusEnemyActivationTimer = new DispatcherTimer();
            this.setRandomBonusEnemyActivationTime();

            this.bonusEnemyActivationTimer.Tick += (sender, e) =>
            {
                this.bonusEnemyMovementTimer.Start();

                this.bonusEnemyActivationTimer.Stop();
            };

            this.bonusEnemyActivationTimer.Start();
        }

        private void setRandomBonusEnemyActivationTime()
        {
            var randomInterval = this.random.Next(5000, 15000);
            this.bonusEnemyActivationTimer.Interval = TimeSpan.FromMilliseconds(randomInterval);
        }

        private void setUpEnemyBulletTimer()
        {
            this.enemyBulletTimer = new DispatcherTimer();
            this.enemyBulletMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(EnemyBulletMovementInMilliseconds)
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
            this.enemyBulletTimer.Interval = TimeSpan.FromMilliseconds(
                this.random.Next(MinCooldownForEnemyBulletInMilliseconds, MaxCooldownForEnemyBulletInMilliseconds));
        }

        private void setUpGameLoopTimer()
        {
            this.gameLoopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(GameLoopInMilliseconds)
            };
            this.gameLoopTimer.Tick += (sender, e) => this.gameCanvas.GameLoop();
            this.gameLoopTimer.Start();
        }

        private void setUpPlayerBulletCooldownTimer()
        {
            this.playerBulletCooldownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(PlayerBulletCooldownInMilliseconds)
            };
            this.playerBulletCooldownTimer.Tick += (sender, e) =>
            {
                this.gameCanvas.EnablePlayerShooting();
                this.playerBulletCooldownTimer.Stop();
            };
        }

        #endregion
    }
}