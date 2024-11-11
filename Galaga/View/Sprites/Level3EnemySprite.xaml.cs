using Windows.UI.Xaml;

namespace Galaga.View.Sprites
{
    /// <summary>
    ///     The level 3 enemy sprite.
    /// </summary>
    /// <seealso cref="Galaga.View.Sprites.BaseSprite" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public sealed partial class Level3EnemySprite
    {
        #region Data members

        private bool isUsingAlternateSprite;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Level3EnemySprite" /> class.
        /// </summary>
        public Level3EnemySprite()
        {
            this.InitializeComponent();
            Y = 400;
            this.isUsingAlternateSprite = false;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Toggles between the base sprite and the alternate sprite.
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