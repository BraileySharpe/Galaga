using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///     Manages the bullets in the game and their functionality.
    /// </summary>
    public class BulletManager
    {
        #region Data members

        private readonly IList<Bullet> activeEnemyBullets;
        private readonly EnemyManager enemyManager;
        private readonly GameManager gameManager;
        private readonly PlayerManager playerManager;
        private bool playerBulletFired;
        private Canvas canvas;
        private Bullet playerBullet;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BulletManager"/> class.
        /// </summary>
        /// <param name="enemyManager">The enemy manager.</param>
        /// <param name="playerManager">The player manager.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="gameManager">The game manager.</param>
        /// <exception cref="System.ArgumentNullException">
        /// enemyManager
        /// or
        /// gameManager
        /// or
        /// playerManager
        /// or
        /// canvas
        /// </exception>
        public BulletManager(EnemyManager enemyManager, PlayerManager playerManager, Canvas canvas, GameManager gameManager)
        {
            this.enemyManager = enemyManager ?? throw new ArgumentNullException(nameof(enemyManager));
            this.gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            this.playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));

            this.playerBullet = new Bullet(new PlayerBulletSprite());
            this.activeEnemyBullets = new List<Bullet>();
        }

        #endregion

        #region Methods

        public void PlacePlayerBullet()
        {
            if (!this.playerBulletFired)
            {
                this.playerBullet = new Bullet(new PlayerBulletSprite());
                this.canvas.Children.Add(this.playerBullet.Sprite);
                this.playerBullet.X = this.playerManager.Player.X + this.playerManager.Player.Width / 2.0 - this.playerBullet.Width / 2.0;
                this.playerBullet.Y = this.playerManager.Player.Y - this.playerBullet.Height;
                this.playerBulletFired = true;
            }
        }

        public void MovePlayerBullet()
        {
            this.playerBullet.MoveUp();
            this.checkPlayerBulletCollision();
        }

        private void checkPlayerBulletCollision()
        {
            if (this.playerBullet.Y + this.playerBullet.Height < 0)
            {
                this.playerBulletFired = false;
                this.canvas.Children.Remove(this.playerBullet.Sprite);
            }

            if (this.playerBulletFired)
            {
                this.checkPlayerBulletCollisionOnEnemy(this.playerBullet);
            }
        }

        private void checkPlayerBulletCollisionOnEnemy(Bullet bullet)
        {
            if (bullet == null)
            {
                throw new ArgumentNullException(nameof(bullet));
            }

            foreach (var enemy in this.enemyManager.Enemies)
            {
                if (bullet.CollidesWith(enemy))
                {
                    this.playerBulletFired = false;
                    this.canvas.Children.Remove(bullet.Sprite);
                    this.canvas.Children.Remove(enemy.Sprite);
                    this.enemyManager.RemoveEnemy(enemy);
                    this.gameManager.Score += enemy.Score;
                    break;
                }
            }
        }

        public void EnemyPlaceBullet()
        {
            IList<ShootingEnemy> shootingEnemies = new List<ShootingEnemy>();
            foreach (var currEnemy in this.enemyManager.Enemies)
            {
                if (currEnemy is ShootingEnemy shootingEnemy)
                {
                    shootingEnemies.Add(shootingEnemy);
                }
            }

            var random = new Random();
            var randomIndex = random.Next(0, shootingEnemies.Count);
            var enemy = shootingEnemies[randomIndex];

            var bullet = new Bullet(new EnemyBulletSprite())
            {
                X = enemy.X + enemy.Width / 2.0,
                Y = enemy.Y + enemy.Height
            };

            bullet.SetSpeed(0, 15);

            this.activeEnemyBullets.Add(bullet);
            this.canvas.Children.Add(bullet.Sprite);
        }

        public void MoveEnemyBullet()
        {
            var indexOfLastBullet = this.activeEnemyBullets.Count - 1;
            for (var i = indexOfLastBullet; i >= 0; i--)
            {
                var bullet = this.activeEnemyBullets[i];
                bullet.MoveDown();
                this.checkEnemyBulletCollision(bullet, i);
            }
        }

        private void checkEnemyBulletCollision(Bullet bullet, int i)
        {
            if (bullet.CollidesWith(this.playerManager.Player))
            {
                this.canvas.Children.Remove(bullet.Sprite);
                this.activeEnemyBullets.RemoveAt(i);

                if (this.playerManager.Lives > 1)
                {
                    this.canvas.Children.Remove(this.playerManager.Player.Sprite);
                    this.playerManager.RespawnPlayer();
                }
                else
                {
                    this.canvas.Children.Remove(this.playerManager.Player.Sprite);
                    // Game over logic could go here.
                }
            }
            else if (bullet.Y > this.canvas.Height)
            {
                this.canvas.Children.Remove(bullet.Sprite);
                this.activeEnemyBullets.RemoveAt(i);
            }
        }

        #endregion
    }
}
