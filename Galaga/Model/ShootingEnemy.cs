using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///     Represents an enemy capable of shooting in the game.
    /// </summary>
    public class ShootingEnemy : Enemy
    {
        #region Properties

        /// <summary>
        ///     Gets a value indicating whether this enemy is capable of shooting.
        /// </summary>
        public bool CanShoot { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ShootingEnemy" /> class.
        /// </summary>
        /// <param name="sprite">The enemy sprite.</param>
        public ShootingEnemy(BaseSprite sprite) : base(sprite)
        {
            this.CanShoot = true;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Enables this enemy to shoot.
        /// </summary>
        public void EnableShooting()
        {
            this.CanShoot = true;
        }

        /// <summary>
        ///     Disables this enemy from shooting.
        /// </summary>
        public void DisableShooting()
        {
            this.CanShoot = false;
        }

        #endregion
    }
}