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

            this.enemyManager = new EnemyManager(this.canvas, this.canvasWidth, this.canvasHeight);
            this.initializeGame();
        }

        #endregion

        #region Methods

        private void initializeGame()
        {
            this.createAndPlacePlayer();
            this.enemyManager.CreateAndPlaceEnemies();
        }

        //private void createAndPlaceEnemies()
        //{
        //    this.createAndPlaceLevel1Enemies(2);
        //    this.createAndPlaceLevel2Enemies(3);
        //    this.createAndPlaceLevel3Enemies(4);
        //}

        //private void createAndPlaceLevel1Enemies(int numOfEnemies)
        //{
        //    if (numOfEnemies < 1)
        //    {
        //        throw new ArgumentException("Number of enemies must be greater than 0.");
        //    }

        //    double spacing = 5;
        //    var totalSpriteWidth = numOfEnemies * new Level1EnemySprite().Width + (numOfEnemies - 1) * spacing;
        //    var leftMargin = (this.canvasWidth - totalSpriteWidth) / 2;

        //    for (var i = 0; i < numOfEnemies; i++)
        //    {
        //        var currEnemy = new Enemy(new Level1EnemySprite());
        //        this.enemyManager.Level1Enemies.Add(currEnemy);
        //        this.canvas.Children.Add(currEnemy.Sprite);

        //        var xPosition = leftMargin + i * (currEnemy.Width + spacing);
        //        currEnemy.X = xPosition;
        //        currEnemy.Y = this.canvasHeight - currEnemy.Height - 250;
        //    }
        //}

        //private void createAndPlaceLevel2Enemies(int numOfEnemies)
        //{
        //    if (numOfEnemies < 1)
        //    {
        //        throw new ArgumentException("Number of enemies must be greater than 0.");
        //    }

        //    double spacing = 5;
        //    var totalSpriteWidth = numOfEnemies * new Level2EnemySprite().Width + (numOfEnemies - 1) * spacing;
        //    var leftMargin = (this.canvasWidth - totalSpriteWidth) / 2;

        //    for (var i = 0; i < numOfEnemies; i++)
        //    {
        //        var currEnemy = new Enemy(new Level2EnemySprite());
        //        this.enemyManager.Level2Enemies.Add(currEnemy);
        //        this.canvas.Children.Add(currEnemy.Sprite);

        //        var xPosition = leftMargin + i * (currEnemy.Width + spacing);
        //        currEnemy.X = xPosition;
        //        currEnemy.Y = this.canvasHeight - currEnemy.Height - 325;
        //    }
        //}

        //private void createAndPlaceLevel3Enemies(int numOfEnemies)
        //{
        //    if (numOfEnemies < 1)
        //    {
        //        throw new ArgumentException("Number of enemies must be greater than 0.");
        //    }

        //    double spacing = 5;
        //    var totalSpriteWidth = numOfEnemies * new Level3EnemySprite().Width + (numOfEnemies - 1) * spacing;
        //    var leftMargin = (this.canvasWidth - totalSpriteWidth) / 2;

        //    for (var i = 0; i < numOfEnemies; i++)
        //    {
        //        var currEnemy = new Enemy(new Level3EnemySprite());
        //        this.enemyManager.Level3Enemies.Add(currEnemy);
        //        this.canvas.Children.Add(currEnemy.Sprite);

        //        var xPosition = leftMargin + i * (currEnemy.Width + spacing);
        //        currEnemy.X = xPosition;
        //        currEnemy.Y = this.canvasHeight - currEnemy.Height - 400;
        //    }
        //}

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

        #endregion
    }
}