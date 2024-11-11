﻿using System;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    ///     Manages the Galaga game play.
    /// </summary>
    public class GameManager
    {
        #region Data members

        private readonly EnemyManager enemyManager;
        private readonly BulletManager bulletManager;
        private readonly PlayerManager playerManager;

        #endregion

        #region Properties

        public int Score
        {
            get => this.playerManager.Score;
            set => this.playerManager.Score = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameManager" /> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <exception cref="System.ArgumentNullException">canvas</exception>
        public GameManager(Canvas canvas)
        {
            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            this.enemyManager = new EnemyManager(canvas);
            this.playerManager = new PlayerManager(canvas);
            this.bulletManager = new BulletManager(this.enemyManager, this.playerManager, canvas, this);

            this.initializeGame();
        }

        #endregion

        #region Methods

        private void initializeGame()
        {
            this.enemyManager.CreateAndPlaceEnemies();
        }

        public void MovePlayerLeft()
        {
            this.playerManager.MovePlayerLeft();
        }

        public void MovePlayerRight()
        {
            this.playerManager.MovePlayerRight();
        }

        public void MoveEnemiesLeft()
        {
            this.enemyManager.MoveEnemiesLeft();
        }

        public void MoveEnemiesRight()
        {
            this.enemyManager.MoveEnemiesRight();
        }

        public void PlacePlayerBullet()
        {
            this.bulletManager.PlacePlayerBullet();
        }

        public void MovePlayerBullet()
        {
            this.bulletManager.MovePlayerBullet();
        }

        public void PlaceEnemyBullet()
        {
            this.bulletManager.EnemyPlaceBullet();
        }

        public void MoveEnemyBullet()
        {
            this.bulletManager.MoveEnemyBullet();
        }

        public int GetRemainingEnemyCount()
        {
            return this.enemyManager.Count;
        }

        public void ToggleSpritesForAnimation()
        {
            this.enemyManager.ToggleSpritesForAnimation();
        }

        #endregion
    }
}