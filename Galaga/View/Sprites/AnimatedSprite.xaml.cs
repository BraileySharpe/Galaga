using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Galaga.View.Sprites
{
    /// <summary>
    ///     Sprite to enable animation.
    /// </summary>
    /// <seealso cref="Galaga.View.Sprites.BaseSprite" />
    public partial class AnimatedSprite
    {
        #region Data members

        private bool isUsingAlternateSprite;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AnimatedSprite" /> class.
        /// </summary>
        public AnimatedSprite()
        {
            this.InitializeComponent();
            this.isUsingAlternateSprite = false;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Toggles the sprite for animation.
        /// </summary>
        public void ToggleSprite()
        {
            if (this.isUsingAlternateSprite)
            {
                VisualStateManager.GoToState(this, "BaseSprite", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "AlternateSprite", true);
            }

            this.isUsingAlternateSprite = !this.isUsingAlternateSprite;
        }

        #endregion
    }
}