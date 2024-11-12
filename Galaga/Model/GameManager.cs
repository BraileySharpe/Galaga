using System;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    ///     Manages the Galaga game play.
    /// </summary>
    public class GameManager : INotifyPropertyChanged
    {
        #region Data members

        private readonly EnemyManager enemyManager;
        private readonly BulletManager bulletManager;
        private readonly PlayerManager playerManager;
        private bool hasWon;
        private bool hasLost;

        #endregion

        #region Properties

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

            this.enemyManager = new EnemyManager(canvas);
            this.playerManager = new PlayerManager(canvas);
            this.bulletManager = new BulletManager(this.enemyManager, this.playerManager, canvas, this);

            this.initializeGame();
        }

        #endregion

        #region Methods

        private void initializeGame()
        {
            this.enemyManager.CreateAndPlaceEnemies();
        }

        /// <summary>
        /// Moves the player left.
        /// </summary>
        public void MovePlayerLeft()
        {
            this.playerManager.MovePlayerLeft();
        }

        /// <summary>
        /// Moves the player right.
        /// </summary>
        public void MovePlayerRight()
        {
            this.playerManager.MovePlayerRight();
        }

        /// <summary>
        /// Moves the enemies left.
        /// </summary>
        public void MoveEnemiesLeft()
        {
            this.enemyManager.MoveEnemiesLeft();
        }

        /// <summary>
        /// Moves the enemies right.
        /// </summary>
        public void MoveEnemiesRight()
        {
            this.enemyManager.MoveEnemiesRight();
        }

        /// <summary>
        /// Places the player bullet.
        /// </summary>
        public void PlacePlayerBullet()
        {
            this.bulletManager.PlacePlayerBullet();
        }

        /// <summary>
        /// Moves the player bullet.
        /// </summary>
        public void MovePlayerBullet()
        {
            this.bulletManager.MovePlayerBullet();
        }

        /// <summary>
        /// Places the enemy bullet.
        /// </summary>
        public void PlaceEnemyBullet()
        {
            this.bulletManager.EnemyPlaceBullet();
        }

        /// <summary>
        /// Moves the enemy bullet.
        /// </summary>
        public void MoveEnemyBullet()
        {
            this.bulletManager.MoveEnemyBullet();
        }

        /// <summary>
        /// Toggles the sprites for animation.
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
                this.HasWon = true;
            }
        }

        #endregion
    }
}