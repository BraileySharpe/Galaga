using System;
using Windows.UI.Xaml.Controls;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///     Manages the Galaga game play.
    /// </summary>
    public class GameManager
    {
        #region Data members

        private const double PlayerOffsetFromBottom = 30;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly double canvasWidth;

        private readonly EnemyManager enemyManager;
        private readonly BulletManager bulletManager;

        private Player player;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameManager"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <exception cref="System.ArgumentNullException">canvas</exception>
        public GameManager(Canvas canvas)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));

            this.canvas = canvas;
            this.canvasHeight = canvas.Height;
            this.canvasWidth = canvas.Width;

            this.enemyManager = new EnemyManager();
            this.initializeGame();
            this.bulletManager = new BulletManager(this.enemyManager, this.player);
        }

        #endregion

        #region Methods

        private void initializeGame()
        {
            this.createAndPlacePlayer();
            this.enemyManager.CreateAndPlaceEnemies(this.canvas);
        }

        private void createAndPlacePlayer()
        {
            this.player = new Player();
            this.canvas.Children.Add(this.player.Sprite);

            this.placePlayerNearBottomOfBackgroundCentered();
        }

        private void placePlayerNearBottomOfBackgroundCentered()
        {
            this.player.X = this.canvasWidth / 2 - this.player.Width / 2.0;
            this.player.Y = this.canvasHeight - this.player.Height - PlayerOffsetFromBottom;
        }

        /// <summary>
        ///     Moves the player left.
        /// </summary>
        public void MovePlayerLeft()
        {
            if (this.player.X - this.player.SpeedX > 0)
            {
                this.player.MoveLeft();
            }
        }

        /// <summary>
        ///     Moves the player right.
        /// </summary>
        public void MovePlayerRight()
        {
            if (this.player.X + this.player.Width + this.player.SpeedX < this.canvasWidth)
            {
                this.player.MoveRight();
            }
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
            this.bulletManager.PlacePlayerBullet(this.canvas, this.player);
        }

        /// <summary>
        /// Moves the player bullet.
        /// </summary>
        public void MovePlayerBullet()
        { 
            this.bulletManager.MovePlayerBullet(this.canvas);
        }

        /// <summary>
        /// Places the enemy bullet on a random level 3 enemy.
        /// </summary>
        public void PlaceEnemyBullet()
        {
            this.bulletManager.EnemyPlaceBullet(this.canvas);
        }

        /// <summary>
        /// Moves the enemy bullet.
        /// </summary>
        public void MoveEnemyBullet()
        {
            this.bulletManager.MoveEnemyBullet(this.canvas);
        }

        /// <summary>
        /// Gets the current score of the game.
        /// </summary>
        /// <returns>The current score</returns>
        public int GetScore()
        {
            return this.enemyManager.Score;
        }

        /// <summary>
        /// Gets the remaining enemy count.
        /// </summary>
        /// <returns>The remaining enemy count</returns>
        public int GetRemainingEnemyCount()
        {
            return this.enemyManager.Count;
        }

        #endregion
    }
}