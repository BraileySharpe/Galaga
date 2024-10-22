using System;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    public class BulletManager
    {
        #region Data members

        private Bullet bullet;
        private EnemyManager enemyManager;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether [bullet fired].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [bullet fired]; otherwise, <c>false</c>.
        /// </value>
        public bool BulletFired { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BulletManager"/> class.
        /// </summary>
        public BulletManager(EnemyManager enemyManager)
        {
            this.enemyManager = enemyManager ?? throw new ArgumentNullException(nameof(enemyManager));
            this.bullet = new Bullet();
            this.BulletFired = false;
        }

        #endregion

        #region Methods

        public void FireBullet(Canvas canvas, Player player)
        {
            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (!this.BulletFired)
            {
                this.bullet = new Bullet();
                canvas.Children.Add(this.bullet.Sprite);
                this.bullet.X = player.X + player.Width / 2.0 - this.bullet.Width / 2.0;
                this.bullet.Y = player.Y - this.bullet.Height;

                this.BulletFired = true;
            }
        }

        public void MoveBullet(Canvas canvas)
        {
            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            this.bullet.MoveUp();
            this.checkBulletCollision(canvas);
        }

        private void checkBulletCollision(Canvas canvas)
        {
            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            if (this.bullet.Y + this.bullet.Height < 0)
            {
                this.BulletFired = false;
                canvas.Children.Remove(this.bullet.Sprite);
            }

            if (this.BulletFired == true)
            {
                foreach (var enemy in this.enemyManager.Level1Enemies)
                {
                    if (this.bullet.CollidesWith(enemy))
                    {
                        this.BulletFired = false;
                        canvas.Children.Remove(this.bullet.Sprite);
                        canvas.Children.Remove(enemy.Sprite);
                        this.enemyManager.Level1Enemies.Remove(enemy);
                        break;
                    }
                }
            }

            if (this.BulletFired == true)
            {
                foreach (var enemy in this.enemyManager.Level2Enemies)
                {
                    if (this.bullet.CollidesWith(enemy))
                    {
                        this.BulletFired = false;
                        canvas.Children.Remove(this.bullet.Sprite);
                        canvas.Children.Remove(enemy.Sprite);
                        this.enemyManager.Level2Enemies.Remove(enemy);
                        break;
                    }
                }
            }

            
            if (this.BulletFired == true)
            {
                foreach (var enemy in this.enemyManager.Level3Enemies)
                {
                    if (this.bullet.CollidesWith(enemy))
                    {
                        this.BulletFired = false;
                        canvas.Children.Remove(this.bullet.Sprite);
                        canvas.Children.Remove(enemy.Sprite);
                        this.enemyManager.Level3Enemies.Remove(enemy);
                        break;
                    }
                }
            }
        }

        #endregion
    }
}