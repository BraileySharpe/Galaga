using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    ///     Manages the Galaga gameplay.
    /// </summary>
    public class GameManager : INotifyPropertyChanged
    {
        #region Data members

        private readonly Canvas canvas;
        private readonly EnemyManager enemyManager;
        private readonly BulletManager bulletManager;
        private readonly PlayerManager playerManager;
        private readonly TimeManager timeManager;
        private readonly SfxManager sfxManager;
        private readonly RoundData roundData;
        private bool endOfRound;
        private bool canShoot;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether it is the end of a round.
        /// </summary>
        /// <value>
        ///     true if it's the end of a round, false otherwise.
        /// </value>
        public bool EndOfRound
        {
            get => this.endOfRound;
            set
            {
                if (this.endOfRound != value)
                {
                    this.endOfRound = value;
                    this.OnPropertyChanged(nameof(this.EndOfRound));
                }
            }
        }

        /// <summary>
        ///     Gets or sets the score.
        /// </summary>
        /// <value>
        ///     The score.
        /// </value>
        public int Score { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance has won.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has won; otherwise, <c>false</c>.
        /// </value>
        public bool HasWon { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance has lost.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has lost; otherwise, <c>false</c>.
        /// </value>
        public bool HasLost { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameManager" /> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <exception cref="System.ArgumentNullException">canvas</exception>
        public GameManager(Canvas canvas)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            this.roundData = new RoundData();
            this.enemyManager = new EnemyManager(canvas, this.roundData);
            this.playerManager = new PlayerManager(canvas);
            this.bulletManager = new BulletManager(canvas);
            this.sfxManager = new SfxManager();
            this.timeManager = new TimeManager(this);
            this.canShoot = true;
            this.enemyManager.PropertyChanged += this.EnemyManagerOnPropertyChanged;
            this.PropertyChanged += this.GameManagerOnPropertyChanged;

            this.initializeGame();
        }
        
        #endregion

        #region Methods

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        private void GameManagerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.EndOfRound) && this.EndOfRound)
            {
                this.ResetBonusEnemyTimers();
            }
        }

        private void EnemyManagerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.enemyManager.HasBonusEnemyStartedMoving) &&
                this.enemyManager.HasBonusEnemyStartedMoving)
            {
                this.sfxManager.Play(GlobalEnums.AudioFiles.BONUSENEMY_SOUND);
            }
        }

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void initializeGame()
        {
            try
            {
                await this.sfxManager.WaitForPreloadingAsync();
            }
            catch (Exception exception)
            {
                throw new TimeoutException("Error preloading sfx", exception);
            }

            this.enemyManager.CreateAndPlaceEnemies();
            this.timeManager.InitializeTimers();
        }

        /// <summary>
        ///     Moves the player left.
        /// </summary>
        public void MovePlayerLeft()
        {
            this.playerManager.MovePlayerLeft();
        }

        /// <summary>
        ///     Moves the player right.
        /// </summary>
        public void MovePlayerRight()
        {
            this.playerManager.MovePlayerRight();
        }

        /// <summary>
        ///     Moves the enemies.
        /// </summary>
        public void MoveEnemies()
        {
            this.enemyManager.MoveEnemies();
        }

        /// <summary>
        ///     Moves the bonus enemy.
        /// </summary>
        public void MoveBonusEnemy()
        {
            this.enemyManager.MoveBonusEnemy();
        }

        /// <summary>
        ///     Places the player bullet.
        /// </summary>
        public void PlacePlayerBullet()
        {
            if (this.canShoot)
            {
                var bullet = this.playerManager.Shoot();
                if (this.bulletManager.PlacePlayerBullet(bullet))
                {
                    this.sfxManager.Play(GlobalEnums.AudioFiles.PLAYER_SHOOT);
                }

                this.canShoot = false;
                this.timeManager.StartPlayerBulletCooldown();
            }
        }

        /// <summary>
        ///     Moves the player bullet.
        /// </summary>
        public void MovePlayerBullet()
        {
            var collidingBullet = this.bulletManager.MovePlayerBullet(this.enemyManager.Enemies);
            if (collidingBullet != null)
            {
                var enemy = this.enemyManager.CheckWhichEnemyIsShot(collidingBullet);
                if (enemy != null)
                {
                    this.sfxManager.Play(GlobalEnums.AudioFiles.ENEMY_DEATH);
                    this.Score += enemy.Score;
                    Debug.WriteLine(this.Score);

                    if (enemy is BonusEnemy)
                    {
                        this.playerManager.GainExtraLife();
                        this.sfxManager.Stop(GlobalEnums.AudioFiles.BONUSENEMY_SOUND);
                        this.playerManager.ActivateShield();
                        this.sfxManager.Play(GlobalEnums.AudioFiles.POWERUP_ACTIVATE);
                    }
                }
            }
        }

        /// <summary>
        ///     Places the enemy bullet.
        /// </summary>
        public void PlaceEnemyBullet()
        {
            if (this.enemyManager.RemainingShootingEnemies != 0)
            {
                var random = new Random();
                var randomIndex = random.Next(0, this.enemyManager.RemainingShootingEnemies);
                var enemy = this.enemyManager.ShootingEnemies[randomIndex];

                var bullet = enemy.Shoot();
                this.sfxManager.Play(GlobalEnums.AudioFiles.ENEMY_SHOOT);
                this.bulletManager.PlaceEnemyBullet(bullet);
            }
        }

        /// <summary>
        ///     Moves the enemy bullet.
        /// </summary>
        public void MoveEnemyBullet()
        {
            if (this.bulletManager.MoveEnemyBullet(this.playerManager.Player))
            {
                if (this.playerManager.HasPowerUp)
                {
                    this.playerManager.HandleHitToShield();
                    if (this.playerManager.HasPowerUp)
                    {
                        this.sfxManager.Play(GlobalEnums.AudioFiles.SHIELDHIT);
                    }
                    else
                    {
                        this.sfxManager.Play(GlobalEnums.AudioFiles.POWERUP_DEACTIVATE);
                    }

                    return;
                }

                this.sfxManager.Play(GlobalEnums.AudioFiles.PLAYER_DEATH);
                if (this.playerManager.RemainingLives > 0)
                {
                    this.canvas.Children.Remove(this.playerManager.Player.Sprite);
                    this.playerManager.RespawnPlayer();
                }
                else
                {
                    this.canvas.Children.Remove(this.playerManager.Player.Sprite);
                    this.CheckGameStatus();
                }
            }
        }

        /// <summary>
        ///     Toggles the sprites for animation.
        /// </summary>
        public void ToggleSpritesForAnimation()
        {
            this.enemyManager.ToggleSpritesForAnimation();
        }

        /// <summary>
        ///     Checks the game status to see whether the game has been won or lost.
        /// </summary>
        public void CheckGameStatus()
        {
            if (this.playerManager.RemainingLives <= 0 && !this.HasLost)
            {
                this.sfxManager.Play(GlobalEnums.AudioFiles.GAMEOVER_LOSE);
                this.HasLost = true;
            }

            if (this.enemyManager.RemainingEnemies == 0 && !this.HasWon)
            {
                switch (this.roundData.CurrentRound)
                {
                    case GlobalEnums.GameRound.Round3:
                        this.sfxManager.Play(GlobalEnums.AudioFiles.GAMEOVER_WIN);
                        this.HasWon = true;

                        break;
                    default:
                        this.roundData.MoveToNextRound();
                        this.EndOfRound = true;
                        this.enemyManager.CreateAndPlaceEnemies();
                        this.EndOfRound = false;
                        break;
                }
            }
        }

        /// <summary>
        ///     Stops all timers.
        /// </summary>
        public void StopAllTimers()
        {
            this.timeManager.StopAllTimers();
        }

        /// <summary>
        ///     Resets the bonus enemy timers.
        /// </summary>
        public void ResetBonusEnemyTimers()
        {
            this.timeManager.ResetBonusEnemyTimers();
        }

        /// <summary>
        ///     Called when the player bullet cooldown is complete.
        /// </summary>
        public void PlayerBulletCooldownComplete()
        {
            this.EnableShooting();
        }

        public void EnableShooting()
        {
            this.canShoot = true;
        }

        #endregion
    }
}