﻿using System;
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

        private const double Spacing = 5;
        private const double Level1EnemyOffset = 250;
        private const double Level2EnemyOffset = 325;
        private const double Level3EnemyOffset = 400;

        #endregion

        #region Properties

        public IList<Enemy> Level1Enemies { get; }
        public IList<Enemy> Level2Enemies { get; }
        public IList<Enemy> Level3Enemies { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EnemyManager" /> class.
        /// </summary>
        public EnemyManager()
        {
            this.Level1Enemies = new List<Enemy>();
            this.Level2Enemies = new List<Enemy>();
            this.Level3Enemies = new List<Enemy>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the and place enemies.
        ///     Note: For a new enemy to be added to the game, a list of enemies must be created and an offset must be provided.
        /// </summary>
        /// <param name="canvas">The canvas to add enemies too.</param>
        public void CreateAndPlaceEnemies(Canvas canvas)
        {
            this.createAndPlaceEnemiesByLevel(this.Level1Enemies, canvas, 2, new Level1EnemySprite(),
                Level1EnemyOffset);
            this.createAndPlaceEnemiesByLevel(this.Level2Enemies, canvas, 3, new Level2EnemySprite(),
                Level2EnemyOffset);
            this.createAndPlaceEnemiesByLevel(this.Level3Enemies, canvas, 4, new Level3EnemySprite(),
                Level3EnemyOffset);
        }

        private void createAndPlaceEnemiesByLevel(IList<Enemy> enemyList, Canvas canvas, int numOfEnemies,
            BaseSprite sprite, double yOffset)
        {
            if (numOfEnemies < 1)
            {
                throw new ArgumentException("Number of enemies must be greater than 0.");
            }

            enemyList.Clear();

            var totalSpriteWidth = numOfEnemies * sprite.Width + (numOfEnemies - 1) * Spacing;
            var leftMargin = (canvas.Width - totalSpriteWidth) / 2;

            for (var i = 0; i < numOfEnemies; i++)
            {
                var currEnemy = new Enemy((BaseSprite)Activator.CreateInstance(sprite.GetType()));
                enemyList.Add(currEnemy);
                canvas.Children.Add(currEnemy.Sprite);

                var xPosition = leftMargin + i * (currEnemy.Width + Spacing);
                currEnemy.X = xPosition;
                currEnemy.Y = canvas.Height - currEnemy.Height - yOffset;
            }
        }

        /// <summary>
        /// Moves the enemies for the game left.
        /// </summary>
        public void MoveEnemiesLeft()
        {
            foreach (var enemy in this.Level1Enemies)
            {
                enemy.MoveLeft();
            }

            foreach (var enemy in this.Level2Enemies)
            {
                enemy.MoveLeft();
            }

            foreach (var enemy in this.Level3Enemies)
            {
                enemy.MoveLeft();
            }
        }

        public void MoveEnemiesRight()
        {
            foreach (var enemy in this.Level1Enemies)
            {
                enemy.MoveRight();
            }

            foreach (var enemy in this.Level2Enemies)
            {
                enemy.MoveRight();
            }

            foreach (var enemy in this.Level3Enemies)
            {
                enemy.MoveRight();
            }
        }

        #endregion
    }
}