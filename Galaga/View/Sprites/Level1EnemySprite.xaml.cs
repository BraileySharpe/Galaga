using Windows.UI.Xaml.Controls;

namespace Galaga.View.Sprites
{
//
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
        public Level1EnemySprite()
        {
            this.InitializeComponent();
            Y = 250;
        }

        #endregion
    }
}