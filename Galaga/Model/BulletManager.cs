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
        private readonly Player player;
        private bool playerBulletFired;

        private Bullet playerBullet;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BulletManager" /> class.
        /// </summary>
        public BulletManager(EnemyManager enemyManager, Player player)
        {
            this.enemyManager = enemyManager ?? throw new ArgumentNullException(nameof(enemyManager));
            this.player = player ?? throw new ArgumentNullException(nameof(player));

            this.playerBullet = new Bullet(new PlayerBulletSprite());
            this.activeEnemyBullets = new List<Bullet>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Places the player bullet onto the Canvas if one isn't already on there.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <exception cref="System.ArgumentNullException">canvas</exception>
        public void PlacePlayerBullet(Canvas canvas)
        {
            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            if (!this.playerBulletFired)
            {
                this.playerBullet = new Bullet(new PlayerBulletSprite());
                canvas.Children.Add(this.playerBullet.Sprite);
                this.playerBullet.X = this.player.X + this.player.Width / 2.0 - this.playerBullet.Width / 2.0;
                this.playerBullet.Y = this.player.Y - this.playerBullet.Height;
                this.playerBulletFired = true;
            }
        }

        /// <summary>
        ///     Moves the player bullet after its placed.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <exception cref="System.ArgumentNullException">canvas</exception>
        public void MovePlayerBullet(Canvas canvas)
        {
            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            this.playerBullet.MoveUp();
            this.checkPlayerBulletCollision(canvas);
        }

        private void checkPlayerBulletCollision(Canvas canvas)
        {
            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            if (this.playerBullet.Y + this.playerBullet.Height < 0)
            {
                this.playerBulletFired = false;
                canvas.Children.Remove(this.playerBullet.Sprite);
            }

            IList<IList<Enemy>> enemyLists = new List<IList<Enemy>>
            {
                this.enemyManager.Level1Enemies,
                this.enemyManager.Level2Enemies,
                this.enemyManager.Level3Enemies
            };

            foreach (var currentEnemies in enemyLists)
            {
                if (this.playerBulletFired)
                {
                    this.checkEnemyBulletCollision(canvas, this.playerBullet, currentEnemies);
                }
            }
        }

        private bool checkEnemyBulletCollision(Canvas canvas, Bullet bullet, IList<Enemy> enemyList)
        {
            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            if (bullet == null)
            {
                throw new ArgumentNullException(nameof(bullet));
            }

            if (enemyList == null)
            {
                throw new ArgumentNullException(nameof(enemyList));
            }

            foreach (var enemy in enemyList)
            {
                if (bullet.CollidesWith(enemy))
                {
                    this.playerBulletFired = false;
                    this.enemyManager.Score += enemy.Score;
                    canvas.Children.Remove(bullet.Sprite);
                    canvas.Children.Remove(enemy.Sprite);
                    enemyList.Remove(enemy);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Places the enemy bullet.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <exception cref="System.ArgumentNullException">canvas</exception>
        public void EnemyPlaceBullet(Canvas canvas)
        {
            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            if (this.enemyManager.Level3Enemies.Count != 0)
            {
                var random = new Random();

                var randomIndex = random.Next(0, this.enemyManager.Level3Enemies.Count);
                var enemy = this.enemyManager.Level3Enemies[randomIndex];

                var bullet = new Bullet(new EnemyBulletSprite())
                {
                    X = enemy.X + enemy.Width / 2.0,
                    Y = enemy.Y + enemy.Height
                };

                bullet.SetSpeed(0, 15);

                this.activeEnemyBullets.Add(bullet);
                canvas.Children.Add(bullet.Sprite);
            }
        }

        /// <summary>
        ///     Moves the enemy bullet.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <exception cref="System.ArgumentNullException">canvas</exception>
        public void MoveEnemyBullet(Canvas canvas)
        {
            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            var indexOfLastBullet = this.activeEnemyBullets.Count - 1;
            for (var i = indexOfLastBullet; i >= 0; i--)
            {
                var bullet = this.activeEnemyBullets[i];
                bullet.MoveDown();
                this.checkEnemyBulletCollision(canvas, bullet, i);
            }
        }

        private void checkEnemyBulletCollision(Canvas canvas, Bullet bullet, int i)
        {
            if (bullet.CollidesWith(this.player))
            {
                canvas.Children.Remove(bullet.Sprite);
                canvas.Children.Remove(this.player.Sprite);
                this.activeEnemyBullets.RemoveAt(i);
            }

            else if (bullet.Y > canvas.Height)
            {
                canvas.Children.Remove(bullet.Sprite);
                this.activeEnemyBullets.RemoveAt(i);
            }
        }

        #endregion
    }
}