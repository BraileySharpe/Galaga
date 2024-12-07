using Windows.UI.Xaml.Controls;

namespace Galaga.View.Sprites
{
    /// <summary>
    ///     The Level 1 Enemy Sprite.
    /// </summary>
    /// <seealso cref="Galaga.View.Sprites.AnimatedSprite" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public partial class Level1EnemySprite
    {
        #region Properties

        protected Canvas BaseCanvas => this.baseCanvas;
        protected Canvas AlternateCanvas => this.alternateCanvas;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Level1EnemySprite" /> class.
        /// </summary>
        public Level1EnemySprite() : base(["BaseSprite", "AlternateSprite"])
        {
            this.InitializeComponent();
            Y = 250;
        }

        #endregion
    }
}