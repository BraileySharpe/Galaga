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

        private const int Level1EnemyScore = 100;
        private const int Level2EnemyScore = 200;
        private const int Level3EnemyScore = 300;
        private const int Level4EnemyScore = 400;

        private const int NumOfLevel1Enemies = 3;
        private const int NumOfLevel2Enemies = 4;
        private const int NumOfLevel3Enemies = 4;
        private const int NumOfLevel4Enemies = 5;

        private readonly Canvas canvas;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the enemies.
        /// </summary>
        /// <value>
        ///     The enemies.
        /// </value>
        public IList<Enemy> Enemies { get; }

        /// <summary>
        ///     Gets the number of enemies left in the game.
        /// </summary>
        /// <value>
        ///     The count.
        /// </value>
        public int Count => this.Enemies.Count;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EnemyManager" /> class.
        /// </summary>
        public EnemyManager(Canvas canvas)
        {
            this.Enemies = new List<Enemy>();
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates and places enemies onto the canvas.
        /// </summary>
        public void CreateAndPlaceEnemies()
        {
            this.createAndPlaceEnemies(NumOfLevel1Enemies, new Level1EnemySprite(), Level1EnemyScore, false);
            this.createAndPlaceEnemies(NumOfLevel2Enemies, new Level2EnemySprite(), Level2EnemyScore, false);
            this.createAndPlaceEnemies(NumOfLevel3Enemies, new Level3EnemySprite(), Level3EnemyScore, true);
            this.createAndPlaceEnemies(NumOfLevel4Enemies, new Level4EnemySprite(), Level4EnemyScore, true);
        }

        private void createAndPlaceEnemies(int numOfEnemies, BaseSprite sprite, int score, bool canShoot)
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
                var currEnemy = canShoot
                    ? new ShootingEnemy((BaseSprite)Activator.CreateInstance(sprite.GetType()))
                    : new Enemy((BaseSprite)Activator.CreateInstance(sprite.GetType()));

                currEnemy.Score = score;

                this.Enemies.Add(currEnemy);
                this.canvas.Children.Add(currEnemy.Sprite);

                var xPosition = leftMargin + i * (currEnemy.Width + Spacing);
                currEnemy.X = xPosition;
                currEnemy.Y = this.canvas.Height - currEnemy.Height - currEnemy.Sprite.Y;
            }
        }

        /// <summary>
        ///     Moves the enemies for the game left.
        /// </summary>
        public void MoveEnemiesLeft()
        {
            foreach (var enemy in this.Enemies)
            {
                enemy.MoveLeft();
            }
        }

        /// <summary>
        ///     Moves the enemies right.
        /// </summary>
        public void MoveEnemiesRight()
        {
            foreach (var enemy in this.Enemies)
            {
                enemy.MoveRight();
            }
        }

        /// <summary>
        ///     Removes the enemy from the enemy list and the canvas.
        /// </summary>
        /// <param name="enemy">The enemy.</param>
        public void RemoveEnemy(Enemy enemy)
        {
            this.Enemies.Remove(enemy);
            this.canvas.Children.Remove(enemy.Sprite);
        }

        public void ToggleSpritesForAnimation()
        {
            foreach (var enemy in this.Enemies)
            {
                if (enemy.Sprite is Level4EnemySprite enemySprite)
                {
                    enemySprite.ToggleSprite();
                }
                else if (enemy.Sprite is Level3EnemySprite enemySprite2)
                {
                    enemySprite2.ToggleSprite();
                }
            }
        }

        #endregion
    }
}