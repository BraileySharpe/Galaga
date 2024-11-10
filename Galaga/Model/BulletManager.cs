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
        private readonly Player player;
        private bool playerBulletFired;
        private Canvas canvas;

        private Bullet playerBullet;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BulletManager"/> class.
        /// </summary>
        /// <param name="enemyManager">The enemy manager.</param>
        /// <param name="player">The player.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="gameManager">The game manager.</param>
        /// <exception cref="System.ArgumentNullException">
        /// enemyManager
        /// or
        /// gameManager
        /// or
        /// player
        /// or
        /// canvas
        /// </exception>
        public BulletManager(EnemyManager enemyManager, Player player, Canvas canvas, GameManager gameManager)
        {
            this.enemyManager = enemyManager ?? throw new ArgumentNullException(nameof(enemyManager));
            this.gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            this.player = player ?? throw new ArgumentNullException(nameof(player));
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));

            this.playerBullet = new Bullet(new PlayerBulletSprite());
            this.activeEnemyBullets = new List<Bullet>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Places the player bullet onto the Canvas if one isn't already on there.
        /// </summary>
        public void PlacePlayerBullet()
        {
            if (!this.playerBulletFired)
            {
                this.playerBullet = new Bullet(new PlayerBulletSprite());
                this.canvas.Children.Add(this.playerBullet.Sprite);
                this.playerBullet.X = this.player.X + this.player.Width / 2.0 - this.playerBullet.Width / 2.0;
                this.playerBullet.Y = this.player.Y - this.playerBullet.Height;
                this.playerBulletFired = true;
            }
        }

        /// <summary>
        ///     Moves the player bullet after its placed.
        /// </summary>
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
                this.checkEnemyBulletCollision(this.playerBullet);
            }
        }

        /// <summary>
        ///     Checks if player bullet collides with an enemy.
        /// </summary>
        /// <param name="bullet">The bullet.</param>
        /// <exception cref="System.ArgumentNullException">bullet</exception>
        private void checkEnemyBulletCollision(Bullet bullet)
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

        /// <summary>
        ///     Places the enemy bullet.
        /// </summary>
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

        /// <summary>
        ///     Moves the enemy bullet.
        /// </summary>
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
            if (bullet.CollidesWith(this.player))
            {
                this.canvas.Children.Remove(bullet.Sprite);
                this.canvas.Children.Remove(this.player.Sprite);
                this.activeEnemyBullets.RemoveAt(i);
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