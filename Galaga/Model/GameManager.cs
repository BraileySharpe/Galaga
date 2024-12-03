using System;
using System.ComponentModel;
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
        private readonly SfxManager sfxManager;
        private readonly RoundData roundData;
        private bool hasWon;
        private bool hasLost;
        private bool endOfRound;

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
        public int Score
        {
            get => this.playerManager.Score;
            set
            {
                if (this.playerManager.Score != value)
                {
                    this.playerManager.Score = value;
                    this.OnPropertyChanged(nameof(this.Score));
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance has won.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has won; otherwise, <c>false</c>.
        /// </value>
        public bool HasWon
        {
            get => this.hasWon;
            set
            {
                if (this.hasWon != value)
                {
                    this.hasWon = value;
                    this.OnPropertyChanged(nameof(this.HasWon));
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance has lost.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has lost; otherwise, <c>false</c>.
        /// </value>
        public bool HasLost
        {
            get => this.hasLost;
            set
            {
                if (this.hasLost != value)
                {
                    this.hasLost = value;
                    this.OnPropertyChanged(nameof(this.HasLost));
                }
            }
        }

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

            this.enemyManager.PropertyChanged += this.EnemyManagerOnPropertyChanged;

            this.initializeGame();
        }

        private void EnemyManagerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.enemyManager.HasBonusEnemyStartedMoving) && this.enemyManager.HasBonusEnemyStartedMoving)
            {
                this.sfxManager.Play(GlobalEnums.AudioFiles.BONUSENEMY_SOUND);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

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
            var bullet = this.playerManager.Shoot();
            if (this.bulletManager.PlacePlayerBullet(bullet))
            {
                this.sfxManager.Play(GlobalEnums.AudioFiles.PLAYER_SHOOT);
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

                    if (enemy is BonusEnemy)
                    {
                        this.playerManager.GainExtraLife();
                        this.sfxManager.Stop(GlobalEnums.AudioFiles.BONUSENEMY_SOUND);
                        this.activatePowerup();
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
                if (this.playerManager.hasPowerUp)
                {
                    this.playerManager.HandleHitToShield();
                    if (this.playerManager.hasPowerUp)
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
            if (this.playerManager.RemainingLives <= 0 && !this.hasLost)
            {
                this.sfxManager.Play(GlobalEnums.AudioFiles.GAMEOVER_LOSE);
                this.HasLost = true;
            }

            if (this.enemyManager.RemainingEnemies == 0 && !this.hasWon)
            {
                switch (this.roundData.CurrentRound)
                {
                    case GlobalEnums.GameRound.Round1:
                        this.roundData.MoveToNextRound();
                        this.EndOfRound = true;
                        this.enemyManager.CreateAndPlaceEnemies();
                        this.EndOfRound = false;
                        break;
                    case GlobalEnums.GameRound.Round2:
                        this.roundData.MoveToNextRound();
                        this.EndOfRound = true;
                        this.enemyManager.CreateAndPlaceEnemies();
                        this.EndOfRound = false;
                        break;
                    case GlobalEnums.GameRound.Round3:
                        this.sfxManager.Play(GlobalEnums.AudioFiles.GAMEOVER_WIN);
                        this.HasWon = true;

                        break;
                }
            }
        }

        private void activatePowerup()
        {
            this.playerManager.ActivateShield();
            this.sfxManager.Play(GlobalEnums.AudioFiles.POWERUP_ACTIVATE);
        }

        #endregion
    }
}