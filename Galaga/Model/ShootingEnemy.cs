using Galaga.View.Sprites;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        /// <summary>
        ///     Gets or sets the alternate sprite for animation.
        /// </summary>
        public BaseSprite AlternateSprite { get; set; }

        /// <summary>
        ///     Tracks the current sprite state for animation.
        /// </summary>
        private bool IsAlternateSprite { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ShootingEnemy" /> class.
        /// </summary>
        /// <param name="sprite">The enemy sprite.</param>
        public ShootingEnemy(BaseSprite sprite) : base(sprite)
        {
            this.CanShoot = true;
            this.IsAlternateSprite = false;
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

        /// <summary>
        ///     Toggles between the primary and alternate sprites.
        /// </summary>
        public void ToggleSprite()
        {
            if (this.AlternateSprite == null)
            {
                return;
            }

            if (this.IsAlternateSprite)
            {
                Canvas.SetLeft(this.Sprite, Canvas.GetLeft(this.AlternateSprite));
                Canvas.SetTop(this.Sprite, Canvas.GetTop(this.AlternateSprite));
            }
            else
            {
                Canvas.SetLeft(this.AlternateSprite, Canvas.GetLeft(this.Sprite));
                Canvas.SetTop(this.AlternateSprite, Canvas.GetTop(this.Sprite));
            }

            this.Sprite.Visibility = this.IsAlternateSprite ? Visibility.Collapsed : Visibility.Visible;
            this.AlternateSprite.Visibility = this.IsAlternateSprite ? Visibility.Visible : Visibility.Collapsed;

            this.IsAlternateSprite = !this.IsAlternateSprite;
        }

        #endregion
    }
}