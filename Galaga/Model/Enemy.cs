using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///     Represents enemies in the game
    /// </summary>
    public class Enemy : GameObject
    {
        #region Data members

        private const int SpeedXDirection = 20;
        private const int SpeedYDirection = 0;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Enemy" /> class.
        /// </summary>
        /// <param name="sprite">The enemy sprite.</param>
        public Enemy(BaseSprite sprite)
        {
            Sprite = sprite;
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        #endregion
    }
}