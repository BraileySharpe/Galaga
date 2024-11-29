using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///     Represents an enemy capable of shooting in the game.
    /// </summary>
    public class ShootingEnemy : Enemy
    {
        #region Data members

        private const int EnemyBulletSpeedY = 15;

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
            var bullet = new Bullet(new EnemyBulletSprite(), GlobalEnums.CharacterType.ENEMY);

            bullet.X = X + Width / 2.0 - bullet.Width / 2.0;
            bullet.Y = Y + Height;
            bullet.SetSpeed(0, EnemyBulletSpeedY);

            return bullet;
        }

        #endregion
    }
}