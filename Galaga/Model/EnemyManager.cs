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

        private readonly RoundData roundData;
        private ShootingEnemy bonusEnemy;

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
        /// <param name="roundData">The round data.</param>
        /// <exception cref="System.ArgumentNullException">canvas</exception>
        public EnemyManager(Canvas canvas, RoundData roundData)
        {
            this.Enemies = new List<Enemy>();
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            this.roundData = roundData ?? throw new ArgumentNullException(nameof(roundData));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates and places enemies onto the canvas.
        /// </summary>
        public void CreateAndPlaceEnemies()
        {
            var numEnemiesForCurrentRound = this.roundData.GetNumEnemiesForCurrentRound();
            this.createEnemiesForRound(GlobalEnums.ShipType.Lvl1Enemy, numEnemiesForCurrentRound[Level1EnemyIndex],
                new Level1EnemySprite().Width);
            this.createEnemiesForRound(GlobalEnums.ShipType.Lvl2Enemy, numEnemiesForCurrentRound[Level2EnemyIndex],
                new Level2EnemySprite().Width);
            this.createEnemiesForRound(GlobalEnums.ShipType.Lvl3Enemy, numEnemiesForCurrentRound[Level3EnemyIndex],
                new Level3EnemySprite().Width);
            this.createEnemiesForRound(GlobalEnums.ShipType.Lvl4Enemy, numEnemiesForCurrentRound[Level4EnemyIndex],
                new Level4EnemySprite().Width);
            this.createBonusEnemyForRound(GlobalEnums.ShipType.BonusEnemy);
        }

        private void createEnemiesForRound(GlobalEnums.ShipType shipType, int numOfEnemies, double spriteWidth)
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

        private void createBonusEnemyForRound(GlobalEnums.ShipType shipType)
        {
            if (this.bonusEnemy != null)
            {
                this.canvas.Children.Remove(this.bonusEnemy.Sprite);
            }

            this.bonusEnemy = ShipFactory.CreateShip(shipType) as BonusEnemy;

            if (this.bonusEnemy != null)
            {
                this.canvas.Children.Add(this.bonusEnemy.Sprite);
                this.Enemies.Add(this.bonusEnemy);
                this.bonusEnemy.X = 0 - this.bonusEnemy.Width;
                this.bonusEnemy.Y = this.canvas.Height - this.bonusEnemy.Height - this.bonusEnemy.Sprite.Y;
            }
        }

        /// <summary>
        ///     Moves the bonus enemy.
        /// </summary>
        public void MoveBonusEnemy()
        {
            this.bonusEnemy.MoveRight();

            if (this.bonusEnemy.X > this.canvas.Width)
            {
                this.canvas.Children.Remove(this.bonusEnemy.Sprite);
                this.Enemies.Remove(this.bonusEnemy);
            }
        }

        /// <summary>
        ///     Moves all enemies left on the canvas.
        /// </summary>
        public void MoveEnemiesLeft()
        {
            foreach (var enemy in this.Enemies)
            {
                if (!(enemy is BonusEnemy))
                {
                    enemy.MoveLeft();
                }
            }
        }

        /// <summary>
        ///     Moves all enemies right on the canvas.
        /// </summary>
        public void MoveEnemiesRight()
        {
            foreach (var enemy in this.Enemies)
            {
                if (!(enemy is BonusEnemy))
                {
                    enemy.MoveRight();
                }
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
            if (bullet == null)
            {
                throw new ArgumentNullException(nameof(bullet));
            }

            foreach (var enemy in this.Enemies)
            {
                if (bullet.CollidesWith(enemy))
                {
                    this.RemoveEnemy(enemy);
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