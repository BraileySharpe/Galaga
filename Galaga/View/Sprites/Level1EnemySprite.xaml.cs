using System.Runtime.CompilerServices;
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
        #region Data members

        private const int InitialY = 250;

        #endregion

        #region Properties
        /// <summary>
        ///    Gets the base canvas.
        /// </summary>
        protected Canvas BaseCanvas => this.baseCanvas;
        /// <summary>
        ///   Gets the alternate canvas.
        /// </summary>
        protected Canvas AlternateCanvas => this.alternateCanvas;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Level1EnemySprite" /> class.
        /// </summary>
        public Level1EnemySprite() : base(["BaseSprite", "AlternateSprite"])
        {
            this.InitializeComponent();
            Y = InitialY;
        }

        #endregion
    }
}