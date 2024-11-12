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

        private const int MaxActivePlayerBullets = 3;
        private const int BulletYOffset = 20;
        private const int EnemyBulletSpeedY = 15;
        private const int BulletBoundary = 0;

        private readonly IList<Bullet> activeEnemyBullets;
        private readonly IList<Bullet> activePlayerBullets;
        private readonly EnemyManager enemyManager;
        private readonly GameManager gameManager;
        private readonly PlayerManager playerManager;
        private readonly Canvas canvas;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BulletManager" /> class.
        /// </summary>
        /// <param name="enemyManager">The enemy manager.</param>
        /// <param name="playerManager">The player manager.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="gameManager">The game manager.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     enemyManager
        ///     or
        ///     gameManager
        ///     or
        ///     playerManager
        ///     or
        ///     canvas
        /// </exception>
        public BulletManager(EnemyManager enemyManager, PlayerManager playerManager, Canvas canvas,
            GameManager gameManager)
        {
            this.enemyManager = enemyManager ?? throw new ArgumentNullException(nameof(enemyManager));
            this.gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            this.playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));

            this.activePlayerBullets = new List<Bullet>();
            this.activeEnemyBullets = new List<Bullet>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Places a new player bullet onto the Canvas if less than 3 are active.
        /// </summary>
        public void PlacePlayerBullet()
        {
            if (this.activePlayerBullets.Count < MaxActivePlayerBullets)
            {
                var playerBullet = new Bullet(new PlayerBulletSprite());
                this.canvas.Children.Add(playerBullet.Sprite);
                playerBullet.X = this.playerManager.Player.X + this.playerManager.Player.Width / 2.0 -
                                 playerBullet.Width / 2.0;
                playerBullet.Y = this.playerManager.Player.Y - playerBullet.Height - BulletYOffset;
                this.activePlayerBullets.Add(playerBullet);
            }
        }

        /// <summary>
        ///     Moves all active player bullets upward.
        /// </summary>
        public void MovePlayerBullet()
        {
            for (var i = this.activePlayerBullets.Count - 1; i >= 0; i--)
            {
                var bullet = this.activePlayerBullets[i];
                bullet.MoveUp();
                this.checkPlayerBulletCollision(bullet, i);
            }
        }

        private void checkPlayerBulletCollision(Bullet bullet, int index)
        {
            if (bullet.Y + bullet.Height < BulletBoundary)
            {
                this.canvas.Children.Remove(bullet.Sprite);
                this.activePlayerBullets.RemoveAt(index);
                return;
            }

            foreach (var enemy in this.enemyManager.Enemies)
            {
                if (bullet.CollidesWith(enemy))
                {
                    this.canvas.Children.Remove(bullet.Sprite);
                    this.canvas.Children.Remove(enemy.Sprite);
                    this.activePlayerBullets.RemoveAt(index);
                    this.enemyManager.RemoveEnemy(enemy);
                    this.gameManager.Score += enemy.Score;
                    break;
                }
            }
        }

        /// <summary>
        ///     Places an enemy bullet on a random enemy.
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

            bullet.SetSpeed(0, EnemyBulletSpeedY);

            this.activeEnemyBullets.Add(bullet);
            this.canvas.Children.Add(bullet.Sprite);
        }

        /// <summary>
        ///     Moves all active enemy bullets downward.
        /// </summary>
        public void MoveEnemyBullet()
        {
            for (var i = this.activeEnemyBullets.Count - 1; i >= 0; i--)
            {
                var bullet = this.activeEnemyBullets[i];
                bullet.MoveDown();
                this.checkEnemyBulletCollision(bullet, i);
            }
        }

        private void checkEnemyBulletCollision(Bullet bullet, int index)
        {
            if (bullet.CollidesWith(this.playerManager.Player))
            {
                this.canvas.Children.Remove(bullet.Sprite);
                this.activeEnemyBullets.RemoveAt(index);

                if (this.playerManager.Lives > 0)
                {
                    this.canvas.Children.Remove(this.playerManager.Player.Sprite);
                    this.playerManager.RespawnPlayer();
                }
                else
                {
                    this.canvas.Children.Remove(this.playerManager.Player.Sprite);
                    this.gameManager.CheckGameStatus();
                }
            }
            else if (bullet.Y > this.canvas.Height)
            {
                this.canvas.Children.Remove(bullet.Sprite);
                this.activeEnemyBullets.RemoveAt(index);
            }
        }

        #endregion
    }
}