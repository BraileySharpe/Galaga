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
        private readonly int speedYDirection = 15;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Bullet" /> class.
        /// </summary>
        /// <param name="sprite">The sprite to set for the bullet.</param>
        public Bullet(BaseSprite sprite, GlobalEnums.CharacterType type)
        {
            Sprite = sprite;

            if (type == GlobalEnums.CharacterType.Player)
            {
                this.speedYDirection = -this.speedYDirection;
            }

            SetSpeed(SpeedXDirection, this.speedYDirection);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Move the bullet.
        /// </summary>
        public void Move()
        {
            Y += SpeedY;
        }

        #endregion
    }
}