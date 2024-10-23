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
        private const int SpeedYDirection = 15;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Bullet" /> class.
        /// </summary>
        /// <param name="sprite">The sprite to set for the bullet.</param>
        public Bullet(BaseSprite sprite)
        {
            Sprite = sprite;
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Checks if the bullet collides with the game object.
        /// </summary>
        /// <param name="gameObject">The game object.</param>
        /// <returns>true if gameObject and the bullet intersect, false otherwise</returns>
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

        #endregion
    }
}