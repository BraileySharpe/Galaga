﻿using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    ///     Manages the bullets in the game and their functionality.
    /// </summary>
    public class BulletManager
    {
        #region Data members

        private const int MaxActivePlayerBullets = 3;

        private readonly IList<Bullet> activeEnemyBullets = new List<Bullet>();
        private readonly IList<Bullet> activePlayerBullets = new List<Bullet>();
        private readonly Canvas canvas;

        private readonly CollisionManager collisionManager;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BulletManager" /> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <exception cref="System.ArgumentNullException">canvas</exception>
        public BulletManager(Canvas canvas)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            this.collisionManager = new CollisionManager(canvas);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Places a new player bullet onto the Canvas if less than 3 are active.
        /// </summary>
        public void PlacePlayerBullet(Bullet bullet)
        {
            if (this.activePlayerBullets.Count < MaxActivePlayerBullets)
            {
                this.canvas.Children.Add(bullet.Sprite);
                this.activePlayerBullets.Add(bullet);
            }
        }

        /// <summary>
        ///     Moves the player bullet.
        /// </summary>
        /// <param name="enemies">The enemies.</param>
        /// <returns>The bullet that hit an enemy, null if no enemies hit</returns>
        public Bullet MovePlayerBullet(IList<Enemy> enemies)
        {
            if (this.activeEnemyBullets.Count != 0)
            {
                for (var i = 0; i <= this.activePlayerBullets.Count - 1; i++)
                {
                    var bullet = this.activePlayerBullets[i];
                    bullet.MoveUp();

                    var playerHitEnemy = this.collisionManager.CheckEnemyCollision(bullet, enemies);

                    if (bullet.Y + bullet.Height < 0)
                    {
                        this.canvas.Children.Remove(bullet.Sprite);
                        this.activePlayerBullets.RemoveAt(i);
                    }

                    if (playerHitEnemy)
                    {
                        this.canvas.Children.Remove(bullet.Sprite);
                        this.activePlayerBullets.RemoveAt(i);
                        return bullet;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Places an enemy bullet on a random enemy.
        /// </summary>
        public void EnemyPlaceBullet(Bullet bullet)
        {
            this.activeEnemyBullets.Add(bullet);
            this.canvas.Children.Add(bullet.Sprite);
        }

        /// <summary>
        ///     Moves all active enemy bullets downward.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns></returns>
        public bool MoveEnemyBullet(Player player)
        {
            for (var i = this.activeEnemyBullets.Count - 1; i >= 0; i--)
            {
                var bullet = this.activeEnemyBullets[i];
                bullet.MoveDown();
                if (bullet.Y > this.canvas.Height)
                {
                    this.canvas.Children.Remove(bullet.Sprite);
                    this.activeEnemyBullets.RemoveAt(i);
                }

                if (this.collisionManager.CheckPlayerCollision(bullet, player))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}