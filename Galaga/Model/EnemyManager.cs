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

        private const int Level1EnemyScore = 100;
        private const int Level2EnemyScore = 150;
        private const int Level3EnemyScore = 200;

        private const int NumOfLevel1Enemies = 2;
        private const int NumOfLevel2Enemies = 3;
        private const int NumOfLevel3Enemies = 4;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the level1 enemies.
        /// </summary>
        /// <value>
        ///     The level1 enemies.
        /// </value>
        public IList<Enemy> Level1Enemies { get; } = new List<Enemy>();

        /// <summary>
        ///     Gets the level2 enemies.
        /// </summary>
        /// <value>
        ///     The level2 enemies.
        /// </value>
        public IList<Enemy> Level2Enemies { get; } = new List<Enemy>();

        /// <summary>
        ///     Gets the level3 enemies.
        /// </summary>
        /// <value>
        ///     The level3 enemies.
        /// </value>
        public IList<Enemy> Level3Enemies { get; } = new List<Enemy>();

        /// <summary>
        ///     Gets the count.
        /// </summary>
        /// <value>
        ///     The count.
        /// </value>
        public int Count => this.Level1Enemies.Count + this.Level2Enemies.Count + this.Level3Enemies.Count;

        /// <summary>
        ///     Gets or sets the score.
        /// </summary>
        /// <value>
        ///     The score.
        /// </value>
        public int Score { get; set; } = 0;

        /// <summary>
        ///     Gets all in the game enemies.
        /// </summary>
        /// <value>
        ///     All enemies.
        /// </value>
        public IList<IList<Enemy>> AllEnemies => new List<IList<Enemy>>
            { this.Level1Enemies, this.Level2Enemies, this.Level3Enemies };

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the and place enemies.
        ///     Note: For a new enemy to be added to the game, a list of enemies must be created and an offset must be provided.
        /// </summary>
        /// <param name="canvas">The canvas to add enemies too.</param>
        public void CreateAndPlaceEnemies(Canvas canvas)
        {
            this.createAndPlaceEnemiesByLevel(this.Level1Enemies, canvas, NumOfLevel1Enemies, new Level1EnemySprite(),
                Level1EnemyOffset, Level1EnemyScore);
            this.createAndPlaceEnemiesByLevel(this.Level2Enemies, canvas, NumOfLevel2Enemies, new Level2EnemySprite(),
                Level2EnemyOffset, Level2EnemyScore);
            this.createAndPlaceEnemiesByLevel(this.Level3Enemies, canvas, NumOfLevel3Enemies, new Level3EnemySprite(),
                Level3EnemyOffset, Level3EnemyScore);
        }

        private void createAndPlaceEnemiesByLevel(IList<Enemy> enemyList, Canvas canvas, int numOfEnemies,
            BaseSprite sprite, double yOffset, int score)
        {
            if (numOfEnemies < 1)
            {
                throw new ArgumentException("Number of enemies must be greater than 0.");
            }

            if (score < 0)
            {
                throw new ArgumentException("Score must be greater than 0.");
            }

            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            if (sprite == null)
            {
                throw new ArgumentNullException(nameof(sprite));
            }

            if (enemyList == null)
            {
                throw new ArgumentNullException(nameof(enemyList));
            }

            enemyList.Clear();

            var totalSpriteWidth = numOfEnemies * sprite.Width + (numOfEnemies - 1) * Spacing;
            var leftMargin = (canvas.Width - totalSpriteWidth) / 2;

            for (var i = 0; i < numOfEnemies; i++)
            {
                var currEnemy = new Enemy((BaseSprite)Activator.CreateInstance(sprite.GetType())) { Score = score };
                enemyList.Add(currEnemy);
                canvas.Children.Add(currEnemy.Sprite);

                var xPosition = leftMargin + i * (currEnemy.Width + Spacing);
                currEnemy.X = xPosition;
                currEnemy.Y = canvas.Height - currEnemy.Height - yOffset;
            }
        }

        /// <summary>
        ///     Moves the enemies for the game left.
        /// </summary>
        public void MoveEnemiesLeft()
        {
            foreach (var enemyList in this.AllEnemies)
            {
                foreach (var enemy in enemyList)
                {
                    enemy.MoveLeft();
                }
            }
        }

        /// <summary>
        ///     Moves the enemies right.
        /// </summary>
        public void MoveEnemiesRight()
        {
            foreach (var enemyList in this.AllEnemies)
            {
                foreach (var enemy in enemyList)
                {
                    enemy.MoveRight();
                }
            }
        }

        #endregion
    }
}