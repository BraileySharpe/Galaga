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
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Bullet" /> class.
        /// </summary>
        public Bullet(BaseSprite sprite)
        {
            Sprite = sprite;
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        #endregion

        public bool CollidesWith(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }

            return X < gameObject.X + gameObject.Width &&
                   X + Width > gameObject.X &&
                   Y < gameObject.Y + gameObject.Height &&
                   Y + Height > gameObject.Y;
        }
    }
}