using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    public class BulletManager
    {
        #region Data members

        private readonly EnemyManager enemyManager;
        private readonly Player player;
        private Bullet playerBullet;
        private readonly IList<Bullet> activeEnemyBullets;
        private bool playerBulletFired;

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
            this.playerBulletFired = false;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fires the player bullet.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="player">The player.</param>
        /// <exception cref="System.ArgumentNullException">
        /// canvas
        /// or
        /// player
        /// </exception>
        public void PlacePlayerBullet(Canvas canvas, Player player)
        {
            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (!this.playerBulletFired)
            {
                this.playerBullet = new Bullet(new PlayerBulletSprite());
                canvas.Children.Add(this.playerBullet.Sprite);
                this.playerBullet.X = player.X + player.Width / 2.0 - this.playerBullet.Width / 2.0;
                this.playerBullet.Y = player.Y - this.playerBullet.Height;

                this.playerBulletFired = true;
            }
        }

        /// <summary>
        /// Moves the player bullet after its placed.
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

            if (this.playerBulletFired)
            {
                foreach (var enemy in this.enemyManager.Level1Enemies)
                {
                    if (this.playerBullet.CollidesWith(enemy))
                    {
                        this.playerBulletFired = false;
                        this.enemyManager.Score += enemy.Score;
                        canvas.Children.Remove(this.playerBullet.Sprite);
                        canvas.Children.Remove(enemy.Sprite);
                        this.enemyManager.Level1Enemies.Remove(enemy);
                        break;
                    }
                }
            }

            if (this.playerBulletFired)
            {
                foreach (var enemy in this.enemyManager.Level2Enemies)
                {
                    if (this.playerBullet.CollidesWith(enemy))
                    {
                        this.playerBulletFired = false;
                        this.enemyManager.Score += enemy.Score;
                        canvas.Children.Remove(this.playerBullet.Sprite);
                        canvas.Children.Remove(enemy.Sprite);
                        this.enemyManager.Level2Enemies.Remove(enemy);
                        break;
                    }
                }
            }

            if (this.playerBulletFired)
            {
                foreach (var enemy in this.enemyManager.Level3Enemies)
                {
                    if (this.playerBullet.CollidesWith(enemy))
                    {
                        this.playerBulletFired = false;
                        this.enemyManager.Score += enemy.Score;
                        canvas.Children.Remove(this.playerBullet.Sprite);
                        canvas.Children.Remove(enemy.Sprite);
                        this.enemyManager.Level3Enemies.Remove(enemy);
                        break;
                    }
                }
            }
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

            if (this.enemyManager.Level3Enemies.Count == 0)
            {
                return;
            }

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

        public void MoveEnemyBullet(Canvas canvas)
        {
            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            for (var i = this.activeEnemyBullets.Count - 1; i >= 0; i--)
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
                return;
            }

            if (bullet.Y > canvas.Height)
            {
                canvas.Children.Remove(bullet.Sprite);
                this.activeEnemyBullets.RemoveAt(i);
            }
        }

        #endregion
    }
}