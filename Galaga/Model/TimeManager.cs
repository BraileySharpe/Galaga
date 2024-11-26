using System;
using Windows.UI.Xaml;

namespace Galaga.View
{
    public class TimerManager
    {
        public const int PlayerBulletCooldownMilliseconds = 300;
        public const int EnemyBulletMovementIntervalMilliseconds = 100;
        public const int EnemyMovementIntervalMilliseconds = 350;
        public const int GameLoopTimerIntervalMilliseconds = 16;
        public const int PlayerBulletMovementIntervalMilliseconds = 10;

        private readonly Random random;

        #region Properties

        public DispatcherTimer EnemyBulletTimer { get; private set; }
        public DispatcherTimer EnemyBulletMovementTimer { get; private set; }
        public DispatcherTimer PlayerBulletTimer { get; private set; }
        public DispatcherTimer PlayerBulletCooldownTimer { get; private set; }
        public DispatcherTimer EnemyMovementTimer { get; private set; }
        public DispatcherTimer GameLoopTimer { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TimerManager" /> class.
        ///     Timers are initialized but event handlers need to be attached externally
        /// </summary>
        public TimerManager()
        {
            this.random = new Random();
            this.initializeTimers();
        }

        #endregion

        #region Methods

        private void initializeTimers()
        {
            this.PlayerBulletCooldownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(PlayerBulletCooldownMilliseconds)
            };
            this.EnemyBulletTimer = new DispatcherTimer();
            this.EnemyBulletMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(EnemyBulletMovementIntervalMilliseconds)
            };
            this.PlayerBulletTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(PlayerBulletMovementIntervalMilliseconds)
            };
            this.EnemyMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(EnemyMovementIntervalMilliseconds)
            };
            this.GameLoopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(GameLoopTimerIntervalMilliseconds)
            };
        }

        public void setRandomEnemyTimeInterval()
        {
            this.EnemyBulletTimer.Interval = TimeSpan.FromMilliseconds(this.random.Next(250, 2500));
        }

        public void StartTimers()
        {
            this.EnemyBulletMovementTimer.Start();
            this.EnemyBulletTimer.Start();
            this.PlayerBulletTimer.Start();
            this.EnemyMovementTimer.Start();
            this.GameLoopTimer.Start();
        }

        public void StopAllTimers()
        {
            this.PlayerBulletCooldownTimer.Stop();
            this.EnemyBulletTimer.Stop();
            this.EnemyBulletMovementTimer.Stop();
            this.PlayerBulletTimer.Stop();
            this.EnemyMovementTimer.Stop();
            this.GameLoopTimer.Stop();
        }

        #endregion
    }
}