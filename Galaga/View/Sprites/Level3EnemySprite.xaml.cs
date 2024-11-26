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
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Level3EnemySprite" /> class.
        /// </summary>
        public Level3EnemySprite()
        {
            this.InitializeComponent();
            Y = 400;
        }

        #endregion
    }
}