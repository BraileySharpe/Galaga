using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///     Represents enemies in the game.
    /// </summary>
    public class Enemy : GameObject
    {
        #region Data members

        private const int SpeedXDirection = 13;
        private const int SpeedYDirection = 0;

        #endregion

        #region Properties

        /// <summary>
        ///     Movement pattern of the enemy.
        /// </summary>
        public int MovementPattern { get; set; }

        /// <summary>
        ///     Gets or sets the score.
        /// </summary>
        /// <value>
        ///     The score.
        /// </value>
        public int Score { get; set; }

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