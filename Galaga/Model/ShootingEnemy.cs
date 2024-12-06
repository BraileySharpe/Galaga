using Galaga.View.Sprites;
using System;

namespace Galaga.Model
{
    /// <summary>
    ///     Represents an enemy capable of shooting in the game.
    /// </summary>
    public class ShootingEnemy : Enemy
    {
        #region Properties

        public int EnemyBulletSpeed { get; protected set; } = 15;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ShootingEnemy" /> class.
        /// </summary>
        /// <param name="sprite">The enemy sprite.</param>
        public ShootingEnemy(BaseSprite sprite) : base(sprite)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Enemy shoots a bullet.
        /// </summary>
        /// <returns>The enemy bullet</returns>
        public Bullet Shoot()
        {
            var bullet = new Bullet(new EnemyBulletSprite(), GlobalEnums.CharacterType.Enemy);

            bullet.X = X + Width / 2.0 - bullet.Width / 2.0;
            bullet.Y = Y + Height;
            bullet.SetSpeed(0, this.EnemyBulletSpeed);

            return bullet;
        }

        public Bullet Shoot(Player player)
        {
            var bullet = new Bullet(new EnemyBulletSprite(), GlobalEnums.CharacterType.Enemy);
            bullet.X = X + Width / 2.0 - bullet.Width / 2.0;
            bullet.Y = Y + Height;

            double deltaX = player.X + player.Width / 2.0 - bullet.X;
            double deltaY = player.Y + player.Height / 2.0 - bullet.Y;

            double magitude = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            double normalizedDeltaX = deltaX / magitude;
            double normalizedDeltaY = deltaY / magitude;

            bullet.SetSpeed((int)(normalizedDeltaX * this.EnemyBulletSpeed), (int)(normalizedDeltaY * this.EnemyBulletSpeed));
            return bullet;
        }

        #endregion
    }
}