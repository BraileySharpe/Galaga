using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///     Represents a player in the game.
    /// </summary>
    public class Player : GameObject
    {
        #region Data members

        private const int SpeedXDirection = 5;
        private const int SpeedYDirection = 0;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Player" /> class.
        /// </summary>
        public Player()
        {
            Sprite = new PlayerSprite();
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Player shoots a bullet.
        /// </summary>
        /// <returns></returns>
        public Bullet Shoot()
        {
            var bullet = new Bullet(new PlayerBulletSprite(), GlobalEnums.ShipType.PLAYER);
            bullet.X = X + Width / 2.0 - bullet.Width / 2.0;
            bullet.Y = Y - bullet.Height;

            return bullet;
        }

        #endregion
    }
}