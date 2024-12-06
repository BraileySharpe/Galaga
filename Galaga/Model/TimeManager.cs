using System;
using Windows.UI.Xaml;

namespace Galaga.Model
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
        private const int PlayerBulletCooldownInMilliseconds = 200;
        private const int PlayerBulletMovementInMilliseconds = 10;
        private const int EnemyBulletMovementInMilliseconds = 100;
        private const int BonusEnemyMovementInMilliseconds = 200;

        private readonly GameManager gameManager;
        private readonly Random random;

        private DispatcherTimer playerBulletTimer;
        private DispatcherTimer enemyMovementTimer;
        private DispatcherTimer enemyBulletTimer;
        private DispatcherTimer enemyBulletMovementTimer;
        private DispatcherTimer playerBulletCooldownTimer;
        private DispatcherTimer bonusEnemyMovementTimer;
        private DispatcherTimer bonusEnemyActivationTimer;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TimeManager" /> class.
        /// </summary>
        /// <param name="gameManager">
        ///     The game manager.
        /// </param>
        public TimeManager(GameManager gameManager)
        {
            this.gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
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
            this.setUpBonusEnemyMovementTimer();
            this.setUpPlayerBulletCooldownTimer();
            this.setUpBonusEnemyActivationTimer();
        }

        /// <summary>
        ///     Starts the player bullet cooldown.
        /// </summary>
        public void StartPlayerBulletCooldown()
        {
            if (!this.playerBulletCooldownTimer.IsEnabled)
            {
                this.playerBulletCooldownTimer.Start();
            }
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
            this.playerBulletTimer.Tick += (sender, e) => this.gameManager.MovePlayerBullet();
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
                this.gameManager.MoveEnemies();
                this.gameManager.ToggleSpritesForAnimation();
            };
            this.enemyMovementTimer.Start();
        }

        private void setUpBonusEnemyMovementTimer()
        {
            this.bonusEnemyMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(BonusEnemyMovementInMilliseconds)
            };
            this.bonusEnemyMovementTimer.Tick += (sender, e) => this.gameManager.MoveBonusEnemy();
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
                this.gameManager.PlaceEnemyBullet();
                this.setRandomEnemyTimeInterval();
            };
            this.enemyBulletMovementTimer.Tick += (sender, e) => this.gameManager.MoveEnemyBullet();

            this.enemyBulletTimer.Start();
            this.enemyBulletMovementTimer.Start();
        }

        private void setRandomEnemyTimeInterval()
        {
            this.enemyBulletTimer.Interval = TimeSpan.FromMilliseconds(
                this.random.Next(MinCooldownForEnemyBulletInMilliseconds, MaxCooldownForEnemyBulletInMilliseconds));
        }

        private void setUpPlayerBulletCooldownTimer()
        {
            this.playerBulletCooldownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(PlayerBulletCooldownInMilliseconds)
            };
            this.playerBulletCooldownTimer.Tick += (sender, e) =>
            {
                this.gameManager.PlayerBulletCooldownComplete();
                this.playerBulletCooldownTimer.Stop();
            };
        }

        #endregion
    }
}