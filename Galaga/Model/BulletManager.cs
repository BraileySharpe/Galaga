using System;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    public class BulletManager
    {
        #region Data members

        private Bullet bullet;

        #endregion

        #region Properties

        public bool BulletFired { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BulletManager"/> class.
        /// </summary>
        public BulletManager()
        {
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
        }

        #endregion
    }
}