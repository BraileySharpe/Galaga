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
        private const int StepCountMaxValue = 30;
        private const int StepCountReverseDirectionValue = 15;

        private BonusEnemy bonusEnemy;
        private bool hasBonusEnemyStartedMoving;
        private int stepCounter = 4;

        private readonly Canvas canvas;
        private readonly RoundData roundData;

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
            this.Enemies.Clear();
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

        /// <summary>
        ///     Movement pattern group number for the enemy type.
        /// </summary>
        private int getEnemyMovementPatternGroupNumber(GlobalEnums.ShipType shipType)
        {
            try
            {
                return (int)shipType;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("Invalid ship type.");
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

            if (this.bonusEnemy.X > this.canvas.Width)
            {
                this.canvas.Children.Remove(this.bonusEnemy.Sprite);
                this.Enemies.Remove(this.bonusEnemy);
            }

            if (!this.hasBonusEnemyStartedMoving)
            {
                this.HasBonusEnemyStartedMoving = true;
            }
        }

        public void MoveEnemies()
        {
            var groupedEnemies = this.Enemies.GroupBy(enemy => enemy.MovementPattern)
                .ToDictionary(g => g.Key, g => g.ToList());
            var allEnemiesButBonus = this.Enemies.Where(enemy => !(enemy is BonusEnemy)).ToList();

            switch (this.roundData.CurrentRound)
            {
                case GlobalEnums.GameRound.Round1:
                    this.handleMovement(allEnemiesButBonus, StepCountReverseDirectionValue, true);
                    break;
                case GlobalEnums.GameRound.Round2:
                    this.handleGroupedMovement(groupedEnemies, new[] { 1, 2 }, new[] { 3, 4 });
                    break;
                case GlobalEnums.GameRound.Round3:
                    this.handleGroupedMovement(groupedEnemies, new[] { 1, 3 }, new[] { 2, 4 });
                    break;
                default:
                    throw new ArgumentException("Invalid round.");
            }

            this.stepCounter = (this.stepCounter + 1) % StepCountMaxValue;

            //var groupedEnemies = this.Enemies.GroupBy(enemy => enemy.MovementPattern)
            //    .ToDictionary(g => g.Key, g => g.ToList());

            //switch (this.roundData.CurrentRound)
            //{
            //    case GlobalEnums.GameRound.Round1:
            //        this.moveGroup(groupedEnemies.ToList(), this.stepCounter <= StepCountReverseDirectionValue);
            //        break;
            //    case GlobalEnums.GameRound.Round2:
            //        this.handleGroupedMovement(groupedEnemies, new[] { 1, 2 }, new[] { 3, 4 });
            //        break;
            //    case GlobalEnums.GameRound.Round3:
            //        this.handleGroupedMovement(groupedEnemies, new[] { 1, 3 }, new[] { 2, 4 });
            //        break;
            //    default:
            //        throw new ArgumentException("Invalid round.");
            //}

            //this.stepCounter = (this.stepCounter + 1) % StepCountMaxValue;
        }

        private void handleMovement(IEnumerable<Enemy> enemies, int reverseStepThreshold, bool bidirectional)
        {
            foreach (var enemy in enemies)
            {
                if (this.stepCounter < reverseStepThreshold)
                {
                    enemy.MoveRight();
                }
                else
                {
                    enemy.MoveLeft();
                }
            }
        }

        private void handleGroupedMovement(Dictionary<int, List<Enemy>> groupedEnemies, IEnumerable<int> group1Patterns,
            IEnumerable<int> group2Patterns)
        {
            foreach (var pattern in group1Patterns)
            {
                if (groupedEnemies.TryGetValue(pattern, out var group1))
                {
                    this.moveGroup(group1, this.stepCounter <= StepCountReverseDirectionValue);
                }
            }

            foreach (var pattern in group2Patterns)
            {
                if (groupedEnemies.TryGetValue(pattern, out var group2))
                {
                    this.moveGroup(group2, this.stepCounter > StepCountReverseDirectionValue);
                }
            }
        }

        private void moveGroup(IEnumerable<Enemy> enemies, bool moveRight)
        {
            foreach (var enemy in enemies)
            {
                if (moveRight)
                {
                    enemy.MoveRight();
                }
                else
                {
                    enemy.MoveLeft();
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