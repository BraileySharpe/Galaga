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

        private const int Level1EnemyIndex = 0;
        private const int Level2EnemyIndex = 1;
        private const int Level3EnemyIndex = 2;
        private const int Level4EnemyIndex = 3;
        private const int BonusEnemyIndex = 4;

        private readonly LevelData levelData;

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
        /// <param name="canvas">The canvas.</param>
        /// <param name="levelData">The level data.</param>
        /// <exception cref="System.ArgumentNullException">canvas</exception>
        public EnemyManager(Canvas canvas, LevelData levelData)
        {
            this.Enemies = new List<Enemy>();
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            this.levelData = levelData ?? throw new ArgumentNullException(nameof(levelData));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates and places enemies onto the canvas.
        /// </summary>
        public void CreateAndPlaceEnemies()
        {
            var numEnemiesInLevel = this.levelData.GetNumEnemiesForCurrentLevel();
            this.createEnemiesForLevel(GlobalEnums.ShipType.LVL1ENEMY, numEnemiesInLevel[Level1EnemyIndex],
                new Level1EnemySprite().Width);
            this.createEnemiesForLevel(GlobalEnums.ShipType.LVL2ENEMY, numEnemiesInLevel[Level2EnemyIndex],
                new Level2EnemySprite().Width);
            this.createEnemiesForLevel(GlobalEnums.ShipType.LVL3ENEMY, numEnemiesInLevel[Level3EnemyIndex],
                new Level3EnemySprite().Width);
            this.createEnemiesForLevel(GlobalEnums.ShipType.LVL4ENEMY, numEnemiesInLevel[Level4EnemyIndex],
                new Level4EnemySprite().Width);
            this.createBonusEnemyForLevel(GlobalEnums.ShipType.BONUSENEMY, numEnemiesInLevel[BonusEnemyIndex]);
        }

        private void createBonusEnemyForLevel(GlobalEnums.ShipType shipType, int numOfEnemies)
        {

        }

        private void createEnemiesForLevel(GlobalEnums.ShipType shipType, int numOfEnemies, double spriteWidth)
        {
            if (numOfEnemies < 1)
            {
                throw new ArgumentException("Number of enemies must be greater than 0.");
            }

            var totalEnemyWidth = numOfEnemies * spriteWidth + (numOfEnemies - 1) * EnemySpacing;
            var leftMargin = (this.canvas.Width - totalEnemyWidth) / 2;

            for (var i = 0; i < numOfEnemies; i++)
            {
                if (!(ShipFactory.CreateShip(shipType) is Enemy currEnemy))
                {
                    throw new ArgumentException("Invalid enemy type.");
                }

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
                    this.canvas.Children.Remove(enemy.Sprite);
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