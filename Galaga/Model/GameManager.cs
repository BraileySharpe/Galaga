using System;
using System.ComponentModel;
using System.Threading.Tasks;
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
        private readonly SFXManager sfxManager;
        private readonly LevelData levelData;
        private bool hasWon;
        private bool hasLost;

        #endregion

        #region Properties

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
            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            this.canvas = canvas;
            this.levelData = new LevelData();
            this.enemyManager = new EnemyManager(canvas, this.levelData);
            this.playerManager = new PlayerManager(canvas);
            this.bulletManager = new BulletManager(canvas);
            this.sfxManager = new SFXManager();

            InitializeGame();
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

        private async void InitializeGame()
        {
            try
            {
                await sfxManager.WaitForPreloadingAsync();
            }
            catch (Exception exception)
            {
                throw new TimeoutException("Error preloading sfx", exception);
            }
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
        ///     Moves the enemies left.
        /// </summary>
        public void MoveEnemiesLeft()
        {
            this.enemyManager.MoveEnemiesLeft();
        }

        /// <summary>
        ///     Moves the enemies right.
        /// </summary>
        public void MoveEnemiesRight()
        {
            this.enemyManager.MoveEnemiesRight();
        }

        /// <summary>
        ///     Places the player bullet.
        /// </summary>
        public void PlacePlayerBullet()
        {
            var bullet = this.playerManager.Shoot();
            this.sfxManager.Play("player_shoot");
            this.bulletManager.PlacePlayerBullet(bullet);
        }

        /// <summary>
        ///     Moves the player bullet.
        /// </summary>
        public void MovePlayerBullet()
        {
            var collidingBullet = this.bulletManager.MovePlayerBullet(this.enemyManager.Enemies);
            if (collidingBullet != null)
            {
                this.Score += this.enemyManager.CheckWhichEnemyIsShot(collidingBullet);
                this.sfxManager.Play("enemy_death");
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
                this.sfxManager.Play("enemy_shoot");
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
                this.sfxManager.Play("player_death");
                if (this.playerManager.Lives > 0)
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
            if (this.playerManager.Lives <= 0 && !this.hasLost)
            {
                this.HasLost = true;
            }

            if (this.enemyManager.RemainingEnemies == 0 && !this.hasWon)
            {
                switch (this.levelData.CurrentLevel)
                {
                    case GlobalEnums.GameLevel.LEVEL1:
                        this.levelData.MoveToNextLevel();
                        this.enemyManager.CreateAndPlaceEnemies();
                        break;
                    case GlobalEnums.GameLevel.LEVEL2:
                        this.levelData.MoveToNextLevel();
                        this.enemyManager.CreateAndPlaceEnemies();
                        break;
                    case GlobalEnums.GameLevel.LEVEL3:
                        this.HasWon = true;
                        break;
                }
            }
        }

        #endregion
    }
}