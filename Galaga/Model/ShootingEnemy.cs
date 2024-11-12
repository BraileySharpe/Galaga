using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///     Represents an enemy capable of shooting in the game.
    /// </summary>
    public class ShootingEnemy : Enemy
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ShootingEnemy" /> class.
        /// </summary>
        /// <param name="sprite">The enemy sprite.</param>
        public ShootingEnemy(BaseSprite sprite) : base(sprite)
        {
        }

        #endregion
    }
}