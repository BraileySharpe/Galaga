using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///     Manager for enemies in the game.
    /// </summary>
    public class EnemyManager : INotifyPropertyChanged
    {
        #region Data members

        private const double EnemySpacing = 15;

        private const int Level1EnemyIndex = 0;
        private const int Level2EnemyIndex = 1;
        private const int Level3EnemyIndex = 2;
        private const int Level4EnemyIndex = 3;
        private const int StepCountMaxValue = 28;
        private const int StepCountReverseDirectionValue = 14;
        private readonly RoundData roundData;
        private BonusEnemy bonusEnemy;
        private bool hasBonusEnemyStartedMoving;
        private int stepCounter = 7;

        private readonly Canvas canvas;

        #endregion

        #region Properties

        public bool HasBonusEnemyStartedMoving
        {
            get => this.hasBonusEnemyStartedMoving;
            set
            {
                if (this.hasBonusEnemyStartedMoving != value)
                {
                    this.hasBonusEnemyStartedMoving = value;
                    this.OnPropertyChanged(nameof(this.HasBonusEnemyStartedMoving));
                }
            }
        }

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
        public int RemainingEnemies => this.Enemies.Count(enemy => !(enemy is BonusEnemy));

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

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
            this.hasBonusEnemyStartedMoving = false;
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

                currEnemy.MovementPattern = this.getEnemyMovementPatternGroupNumber(shipType);

                this.Enemies.Add(currEnemy);
                this.canvas.Children.Add(currEnemy.Sprite);

                var xPosition = leftMargin + i * (currEnemy.Width + EnemySpacing);
                currEnemy.X = xPosition;
                currEnemy.Y = this.canvas.Height - currEnemy.Height - currEnemy.Sprite.Y;
            }
        }

        private int getEnemyMovementPatternGroupNumber(GlobalEnums.ShipType shipType)
        {
            switch (shipType)
            {
                case GlobalEnums.ShipType.Lvl1Enemy:
                    return 1;
                case GlobalEnums.ShipType.Lvl2Enemy:
                    return 2;
                case GlobalEnums.ShipType.Lvl3Enemy:
                    return 3;
                case GlobalEnums.ShipType.Lvl4Enemy:
                    return 4;
                default:
                    throw new ArgumentException("Invalid enemy type.");
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
        ///     Moves the bonus enemy, returns true if enemy has just started moving
        /// </summary>
        public void MoveBonusEnemy()
        {
            this.bonusEnemy.MoveRight();

            if (!this.hasBonusEnemyStartedMoving)
            {
                this.HasBonusEnemyStartedMoving = true;
            }

            if (this.bonusEnemy.X > this.canvas.Width)
            {
                this.canvas.Children.Remove(this.bonusEnemy.Sprite);
                this.Enemies.Remove(this.bonusEnemy);
            }
        }

        /// <summary>
        ///     Moves enemies based on the current round and movement pattern group.
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     "Invalid round." if the round is not in GlobalEnums.GameRound
        /// </exception>
        public void MoveEnemies()
        {
            var group1Enemies = this.Enemies.Where(enemy => enemy.MovementPattern == 1).ToList();
            var group2Enemies = this.Enemies.Where(enemy => enemy.MovementPattern == 2).ToList();
            var group3Enemies = this.Enemies.Where(enemy => enemy.MovementPattern == 3).ToList();
            var group4Enemies = this.Enemies.Where(enemy => enemy.MovementPattern == 4).ToList();

            var allEnemiesButBonus = this.Enemies.Where(enemy => !(enemy is BonusEnemy)).ToList();

            switch (this.roundData.CurrentRound)
            {
                case GlobalEnums.GameRound.Round1:
                    this.handleRound1Movement(allEnemiesButBonus);
                    this.stepCounter++;
                    break;
                case GlobalEnums.GameRound.Round2:
                    this.handleRound2Movement(group1Enemies.Concat(group2Enemies), group3Enemies.Concat(group4Enemies));
                    this.stepCounter++;
                    break;
                case GlobalEnums.GameRound.Round3:
                    this.handleRound3Movement(group1Enemies.Concat(group3Enemies), group2Enemies.Concat(group4Enemies));
                    this.stepCounter++;
                    break;
                default:
                    throw new ArgumentException("Invalid round.");
            }

            if (this.stepCounter == StepCountMaxValue)
            {
                this.stepCounter = 0;
            }
        }

        private void handleRound1Movement(IList<Enemy> enemies)
        {
            if (this.stepCounter < StepCountReverseDirectionValue)
            {
                foreach (var enemy in enemies)
                {
                    enemy.MoveRight();
                }
            }
            else
            {
                foreach (var enemy in enemies)
                {
                    enemy.MoveLeft();
                }
            }
        }

        private void handleRound2Movement(IEnumerable<Enemy> group1, IEnumerable<Enemy> group2)
        {
            if (this.stepCounter <= StepCountReverseDirectionValue)
            {
                foreach (var enemy in group1)
                {
                    enemy.MoveRight();
                }
                foreach (var enemy in group2)
                {
                    enemy.MoveLeft();
                }
            }
            else
            {
                foreach (var enemy in group1)
                {
                    enemy.MoveLeft();
                }
                foreach (var enemy in group2)
                {
                    enemy.MoveRight();
                }
            }
        }

        private void handleRound3Movement(IEnumerable<Enemy> group1, IEnumerable<Enemy> group2)
        {
            if (this.stepCounter <= StepCountReverseDirectionValue)
            {
                foreach (var enemy in group1)
                {
                    enemy.MoveRight();
                }
                foreach (var enemy in group2)
                {
                    enemy.MoveLeft();
                }
            }
            else
            {
                foreach (var enemy in group1)
                {
                    enemy.MoveLeft();
                }
                foreach (var enemy in group2)
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
        ///     Checks the which enemy is shot, removes it, and returns it.
        /// </summary>
        /// <param name="bullet">The bullet to compare collision.</param>
        /// <returns>The enemy that was hit, and null if no enemies were hit</returns>
        public Enemy CheckWhichEnemyIsShot(Bullet bullet)
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
                    return enemy;
                }
            }

            return null;
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