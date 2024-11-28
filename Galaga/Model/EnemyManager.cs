using System;
using System.Collections.Generic;
using System.Linq;
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

        private const double EnemySpacing = 15;

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
        public IList<Enemy> Enemies { get; }

        /// <summary>
        ///     Gets the shooting enemies.
        /// </summary>
        public IList<ShootingEnemy> ShootingEnemies => this.Enemies.OfType<ShootingEnemy>().ToList();

        /// <summary>
        ///     Gets the number of enemies left in the game.
        /// </summary>
        public int RemainingEnemies => this.Enemies.Count;

        /// <summary>
        ///     Gets the remaining shooting enemies.
        /// </summary>
        public int RemainingShootingEnemies => this.Enemies.Count(enemy => enemy is ShootingEnemy);

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
            this.createEnemiesForLevel(NumOfLevel1Enemies, new Level1EnemySprite(), Level1EnemyScore, false);
            this.createEnemiesForLevel(NumOfLevel2Enemies, new Level2EnemySprite(), Level2EnemyScore, false);
            this.createEnemiesForLevel(NumOfLevel3Enemies, new Level3EnemySprite(), Level3EnemyScore, true);
            this.createEnemiesForLevel(NumOfLevel4Enemies, new Level4EnemySprite(), Level4EnemyScore, true);
        }

        private void createEnemiesForLevel(int numOfEnemies, BaseSprite sprite, int score, bool canShoot)
        {
            if (numOfEnemies < 1)
            {
                throw new ArgumentException("Number of enemies must be greater than 0.");
            }

            if (sprite == null)
            {
                throw new ArgumentNullException(nameof(sprite));
            }

            var totalEnemyWidth = numOfEnemies * sprite.Width + (numOfEnemies - 1) * EnemySpacing;
            var leftMargin = (this.canvas.Width - totalEnemyWidth) / 2;

            for (var i = 0; i < numOfEnemies; i++)
            {
                var currEnemy = canShoot
                    ? new ShootingEnemy((BaseSprite)Activator.CreateInstance(sprite.GetType()))
                    : new Enemy((BaseSprite)Activator.CreateInstance(sprite.GetType()));

                currEnemy.Score = score;

                this.Enemies.Add(currEnemy);
                this.canvas.Children.Add(currEnemy.Sprite);

                var xPosition = leftMargin + i * (currEnemy.Width + EnemySpacing);
                currEnemy.X = xPosition;
                currEnemy.Y = this.canvas.Height - currEnemy.Height - currEnemy.Sprite.Y;
            }
        }

        /// <summary>
        ///     Moves all enemies left on the canvas.
        /// </summary>
        public void MoveEnemiesLeft()
        {
            foreach (var enemy in this.Enemies)
            {
                enemy.MoveLeft();
            }
        }

        /// <summary>
        ///     Moves all enemies right on the canvas.
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
        /// <param name="enemy">The enemy to remove.</param>
        public void RemoveEnemy(Enemy enemy)
        {
            this.Enemies.Remove(enemy);
            this.canvas.Children.Remove(enemy.Sprite);
        }

        /// <summary>
        ///     Checks the which enemy is shot, removes it, and returns its score.
        /// </summary>
        /// <param name="bullet">The bullet to compare collision.</param>
        /// <returns>The score of the hit enemy, else 0 if there is nothing to be returned</returns>
        public int CheckWhichEnemyIsShot(Bullet bullet)
        {
            foreach (var enemy in this.Enemies)
            {
                if (bullet.CollidesWith(enemy))
                {
                    this.RemoveEnemy(enemy);
                    this.canvas.Children.Remove(bullet.Sprite);
                    return enemy.Score;
                }
            }

            return 0;
        }

        /// <summary>
        ///     Toggles animation for all animated sprites
        /// </summary>
        public void ToggleSpritesForAnimation()
        {
            foreach (var enemy in this.Enemies)
            {
                if (enemy.Sprite is AnimatedSprite enemySprite)
                {
                    enemySprite.ToggleSprite();
                }
            }
        }

        #endregion
    }
}