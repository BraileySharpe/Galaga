using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///     Represents a bullet in the game.
    /// </summary>
    public class Bullet : GameObject
    {
        #region Data members

        private const int SpeedXDirection = 0;
        private const int SpeedYDirection = 10;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Bullet" /> class.
        /// </summary>
        public Bullet()
        {
            Sprite = new BulletSprite();
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        #endregion

        public bool CollidesWith(Enemy enemy)
        {
            if (enemy == null)
            {
                return false;
            }

            return X < enemy.X + enemy.Width &&
                   X + Width > enemy.X &&
                   Y < enemy.Y + enemy.Height &&
                   Y + Height > enemy.Y;
        }
    }
}