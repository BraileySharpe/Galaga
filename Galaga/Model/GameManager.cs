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
        ///     Initializes a new instance of the <see cref="GameManager" /> class.
        /// </summary>
        public GameManager(Canvas canvas)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));

            this.canvas = canvas;
            this.canvasHeight = canvas.Height;
            this.canvasWidth = canvas.Width;

            this.enemyManager = new EnemyManager();
            this.bulletManager = new BulletManager();
            this.initializeGame();
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

        public void MoveEnemiesLeft()
        {
            this.enemyManager.MoveEnemiesLeft();
        }

        public void MoveEnemiesRight()
        {
            this.enemyManager.MoveEnemiesRight();
        }

        public void PlaceBullet()
        {
            this.bulletManager.FireBullet(this.canvas, this.player);
        }

        public void MoveBullet()
        {
            this.bulletManager.MoveBullet(this.canvas);

            if (this.bulletManager.BulletFired)
            {
                this.checkBulletCollision();
            }
        }

        #endregion
    }
}