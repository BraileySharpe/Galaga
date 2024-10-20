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

        private readonly Canvas canvas;
        private readonly double canvasWidth;
        private readonly double canvasHeight;

        #endregion

        #region Properties

        public List<Enemy> Level1Enemies { get; }
        public List<Enemy> Level2Enemies { get; }
        public List<Enemy> Level3Enemies { get; }

        #endregion

        #region Constructors

        public EnemyManager(Canvas canvas, double canvasWidth, double canvasHeight)
        {
            this.canvas = canvas;
            this.canvasWidth = canvasWidth;
            this.canvasHeight = canvasHeight;

            this.Level1Enemies = new List<Enemy>();
            this.Level2Enemies = new List<Enemy>();
            this.Level3Enemies = new List<Enemy>();
        }

        #endregion

        #region Methods

        public void CreateAndPlaceEnemies()
        {
            this.CreateAndPlaceLevel1Enemies(2);
            this.CreateAndPlaceLevel2Enemies(3);
            this.CreateAndPlaceLevel3Enemies(4);
        }

        private void CreateAndPlaceLevel1Enemies(int numOfEnemies)
        {
            this.PlaceEnemies(numOfEnemies, new Level1EnemySprite(), this.Level1Enemies, 250);
        }

        private void CreateAndPlaceLevel2Enemies(int numOfEnemies)
        {
            this.PlaceEnemies(numOfEnemies, new Level2EnemySprite(), this.Level2Enemies, 325);
        }

        private void CreateAndPlaceLevel3Enemies(int numOfEnemies)
        {
            this.PlaceEnemies(numOfEnemies, new Level3EnemySprite(), this.Level3Enemies, 400);
        }

        private void PlaceEnemies(int numOfEnemies, BaseSprite enemySprite, List<Enemy> enemyList, double yOffset)
        {
            if (numOfEnemies < 1)
            {
                throw new ArgumentException("Number of enemies must be greater than 0.");
            }

            double spacing = 5;
            var totalSpriteWidth = numOfEnemies * enemySprite.Width + (numOfEnemies - 1) * spacing;
            var leftMargin = (this.canvasWidth - totalSpriteWidth) / 2;

            for (var i = 0; i < numOfEnemies; i++)
            {
                var currEnemy = new Enemy(enemySprite);
                enemyList.Add(currEnemy);
                this.canvas.Children.Add(currEnemy.Sprite);

                var xPosition = leftMargin + i * (currEnemy.Width + spacing);
                currEnemy.X = xPosition;
                currEnemy.Y = this.canvasHeight - currEnemy.Height - yOffset;
            }
        }

        #endregion
    }
}