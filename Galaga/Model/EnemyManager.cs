using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///     Manager for enemies in the game.
    /// </summary>
    public class EnemyManager
    {
        #region Data members

        private const double Spacing = 15;
        private const double Level1EnemyOffset = 250;
        private const double Level2EnemyOffset = 325;
        private const double Level3EnemyOffset = 400;
        private const double Level4EnemyOffset = 475;

        private const int Level1EnemyScore = 100;
        private const int Level2EnemyScore = 150;
        private const int Level3EnemyScore = 200;
        private const int Level4EnemyScore = 250;

        private const int NumOfLevel1Enemies = 3;
        private const int NumOfLevel2Enemies = 4;
        private const int NumOfLevel3Enemies = 4;
        private const int NumOfLevel4Enemies = 5;

        private readonly Canvas canvas;
        private readonly List<Enemy> enemies;

        #endregion

        #region Properties

        public IList<Enemy> Enemies => this.enemies;

        /// <summary>
        ///     Gets the count.
        /// </summary>
        /// <value>
        ///     The count.
        /// </value>
        public int Count => this.enemies.Count;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EnemyManager" /> class.
        /// </summary>
        public EnemyManager(Canvas canvas)
        {
            this.enemies = new List<Enemy>();
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the and place enemies.
        ///     Note: For a new enemy to be added to the game, a list of enemies must be created and an offset must be provided.
        /// </summary>
        public void CreateAndPlaceEnemies()
        {
            this.createAndPlaceEnemies(NumOfLevel1Enemies, new Level1EnemySprite(), Level1EnemyOffset,
                Level1EnemyScore, false);
            this.createAndPlaceEnemies(NumOfLevel2Enemies, new Level1EnemySprite(), Level2EnemyOffset,
                Level2EnemyScore, false);
            this.createAndPlaceEnemies(NumOfLevel3Enemies, new Level2EnemySprite(), Level3EnemyOffset,
                Level3EnemyScore, true);
            this.createAndPlaceEnemies(NumOfLevel4Enemies, new Level3EnemySprite(), Level4EnemyOffset, Level4EnemyScore, true);
        }

        private void createAndPlaceEnemies(int numOfEnemies, BaseSprite sprite, double yOffset, int score, bool canShoot)
        {
            if (numOfEnemies < 1)
            {
                throw new ArgumentException("Number of enemies must be greater than 0.");
            }

            if (sprite == null)
            {
                throw new ArgumentNullException(nameof(sprite));
            }

            var totalSpriteWidth = numOfEnemies * sprite.Width + (numOfEnemies - 1) * Spacing;
            var leftMargin = (this.canvas.Width - totalSpriteWidth) / 2;

            for (var i = 0; i < numOfEnemies; i++)
            {
                Enemy currEnemy = canShoot
                    ? new ShootingEnemy((BaseSprite)Activator.CreateInstance(sprite.GetType()))
                    : new Enemy((BaseSprite)Activator.CreateInstance(sprite.GetType()));

                currEnemy.Score = score;
                this.enemies.Add(currEnemy);
                this.canvas.Children.Add(currEnemy.Sprite);

                var xPosition = leftMargin + i * (currEnemy.Width + Spacing);
                currEnemy.X = xPosition;
                currEnemy.Y = this.canvas.Height - currEnemy.Height - yOffset;
            }
        }

        /// <summary>
        ///     Moves the enemies for the game left.
        /// </summary>
        public void MoveEnemiesLeft() => this.enemies.ForEach(enemy => enemy.MoveLeft());

        /// <summary>
        ///     Moves the enemies right.
        /// </summary>
        public void MoveEnemiesRight() => this.enemies.ForEach(enemy => enemy.MoveRight());

        /// <summary>
        ///     Removes the enemy from the enemy list and the canvas.
        /// </summary>
        /// <param name="enemy">The enemy.</param>
        public void RemoveEnemy(Enemy enemy)
        {
            this.enemies.Remove(enemy);
            this.canvas.Children.Remove(enemy.Sprite);
        }

        #endregion
    }
}